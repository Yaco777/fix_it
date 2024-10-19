﻿using Godot;
using System;
using System.Collections.Generic;

public partial class ProgressSystem : CanvasLayer
{
    private List<Achievement> playerAchievements = new List<Achievement>(); //we store all the achievements of the player
    private Dictionary<Employee, List<Achievement>> allAchievements = new Dictionary<Employee, List<Achievement>>();
    //all achievements of the game
    private List<Employee> allEmployees = new List<Employee>();
    private int totalStars = 0;
    private TextureProgressBar _starsProgressBar; //the circular progress bar
    private ProgressBar _totalProgressBar; //the global progress bar
    private AchievementDisplay _achievementDisplay; //shown when the player get an achievements

    //audio stram for the progress bar and the stars 
    private AudioStreamPlayer _starsPlayer;
    private AudioStreamPlayer _totalProgressPlayer;



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {


        _totalProgressBar = GetNode<ProgressBar>("TotalProgress");
        _starsProgressBar = GetNode<TextureProgressBar>("StarsProgress");
        _achievementDisplay = GetNode<AchievementDisplay>("AchievementDisplay");
        _starsPlayer = GetNode<AudioStreamPlayer>("StarsPlayer");
        _totalProgressPlayer = GetNode<AudioStreamPlayer>("TotalProgressPlayer");
        var employees = GetNode<Node2D>("../Employees").GetChildren();
        foreach (var emp in employees)
        {

            if (emp is Employee)
            {
                //we store all employees and when the state of the employee change we will call CheckNewAchievements
                var empConv = (Employee)emp;
                allEmployees.Add((Employee)emp);
                empConv.EmployeeStateChanged += CheckNewAchievements;
                allAchievements[(Employee)emp] = new List<Achievement>();
            }
        }

        CreateAchievements();

    }

    private void CreateAchievements()
    {
        /**
         * Create all the achievements of the game
         */

        //---achievements of the musicien---
        var musicien = allEmployees.Find(emp => emp.NameOfEmployee == "Musicien");
        var achievementMusicien = new Achievement(
            "After the last breath, only the silence remains",
            "The music has stop for the first time",
            10,
            () => musicien.CurrentState == Employee.EmployeeState.NotWorking // Condition to unlock the achievement
        );

        var achievementMusicien2 = new Achievement(
            "Music is the language of emotions",
            "The musicien worked again for the first time",
            70,
            () => musicien.NumberOfTimeWorked == 2
        );



        //we add all the achievements
        allAchievements[musicien].Add(achievementMusicien);
        allAchievements[musicien].Add(achievementMusicien2);

    }

    private void CheckNewAchievements(int newState, string employeeName)
    {
        /**
         *  Check if an achievements has been unlocked for the employee "employeeName"
         */

        //we get the employee
        var employee = allEmployees.Find(emp => emp.NameOfEmployee == employeeName);
        if (employee == null)
        {
            throw new ArgumentException("The state of " + employeeName + " changed but it seems that the name is wrong, we can't check their achievements !");
        }
        var achivements = allAchievements[employee];
        foreach (Achievement achievement in achivements)
        {
            CheckIndividualAchievement(achievement);
            
        }


    }

    private void CheckIndividualAchievement(Achievement achievement)
    {
        /**
         * Check if a specific achievement has been completed
         */
        if (!playerAchievements.Contains(achievement))
        {
            if (achievement.IsCompleted())
            {
                playerAchievements.Add(achievement);
                AnimateStarsProgress(achievement.NumberOfStars);
                _achievementDisplay.ShowAchievement(achievement);
                _starsPlayer.Play();

            }

        }
    }

    private void AnimateStarsProgress(int stars)
    {
        /**
         * We create a smooth animation when we add new stars by using a tween
         */
         
        var targetValue = _starsProgressBar.Value + stars; 

        
        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out);
        tween.SetTrans(Tween.TransitionType.Sine);

        
        tween.TweenProperty(
        _starsProgressBar,
        "value",
        targetValue,
        1.0f
    );

        tween.Play();

    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
