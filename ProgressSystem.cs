﻿using System;
using System.Collections.Generic;
using Godot;
using Range = Godot.Range;

public partial class ProgressSystem : CanvasLayer
{
    private List<Achievement> playerAchievements = new List<Achievement>(); //we store all the achievements of the player
    private Dictionary<Employee, List<Achievement>> allAchievements = new Dictionary<Employee, List<Achievement>>(); //all achievements of the game

    private List<Employee> allEmployees = new List<Employee>();
    public int TotalStars { get; set; }
    private TextureProgressBar _starsProgressBar; //the circular progress bar
    private ProgressBar _totalProgressBar; //the global progress bar
    private AchievementDisplay _achievementDisplay; //shown when the player get an achievements

    //audio stram for the progress bar and the stars 
    private AudioStreamPlayer _starsPlayer;
    private AudioStreamPlayer _totalProgressPlayer;

    private Label _starsLevel;
    private Label _totalNumberOfStarsLabel;

    private List<string> _unlockableEmployeeOrder = new List<string>();
   

    [Export]
    public int MaxStarsValue { get; set; } = 100;

    [Export]
    private int MaxProgressValue { get; set; } = 100; //not used anymore

    [Export]
    private float WaitTimeBeforeLevelUp { get; set; } = 1f;

    private float WaitTimeBeforeNextAchievement; //by default this value will be the time to display one achievement + fade in time + fade out time

    private int _currentLevel = 1; //the level that will be shown at the top left corner. The level will increase when the star will be full



    private Queue<Achievement> achievementQueue = new Queue<Achievement>();
    private bool isProcessingAchievements;
    private int _currentAmountsOfStars;


    //these two variables are used to avoid concurrent access
    private bool isTweening;
    private Queue<(int stars, Range node)> animationQueue = new Queue<(int stars, Range node)>();


    private GlobalSignals _globalSignals;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        InitUnlockableEmployee();
        _totalProgressBar = GetNode<ProgressBar>("TotalProgress");
        _starsProgressBar = GetNode<TextureProgressBar>("StarsProgress");
        _achievementDisplay = GetNode<AchievementDisplay>("AchievementDisplay");
        _starsPlayer = GetNode<AudioStreamPlayer>("StarsPlayer");
        _totalProgressPlayer = GetNode<AudioStreamPlayer>("TotalProgressPlayer");
        _starsLevel = _starsProgressBar.GetNode<Label>("StarsLevel");
        _totalNumberOfStarsLabel = GetNode<Label>("NumberOfStars");
        _globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
        var employees = GetNode<Node2D>("../../Employees").GetChildren();
        
        _starsLevel.Text = _currentLevel.ToString(); //we start at the level 1
        _starsProgressBar.MaxValue = MaxStarsValue;
        _totalProgressBar.MaxValue = MaxProgressValue;
        _achievementDisplay.Visible = true;
        WaitTimeBeforeNextAchievement = _achievementDisplay.AchievementDisplayTime + _achievementDisplay.AchievementFadeInTime + _achievementDisplay.AchievementFadeOutTime; ;
        
        _totalNumberOfStarsLabel.Text = "0";
        

        //this loop is used for the employess that are already in the game when we start it, not the one added by the rooms
        foreach (var emp in employees)
        {

            if (emp is Employee)
            {
                //we store all employees and when the state of the employee change we will call CheckNewAchievements
                var empConv = (Employee)emp;
                allEmployees.Add((Employee)emp);
                empConv.EmployeeStateChanged += CheckNewAchievements;
                empConv.CheckAchievement += CheckNewAchievements;
                allAchievements[(Employee)emp] = new List<Achievement>();
                AddAchievementsForOneEmployee(empConv);
            }
        }

        

    }

    public void AddAchievementsForOneEmployee(Employee employee)
    {
        /**
         * When an employee has been created by a room, we need to add the achievements associated
         */
        allEmployees.Add(employee);
        allAchievements[employee] = new List<Achievement>();
        employee.EmployeeStateChanged += CheckNewAchievements;
        employee.CheckAchievement += CheckNewAchievements;
        if (employee.NameOfEmployee == "Musicien")
        {
            var musicien = (Musicien)employee;
            var achievementMusicien = new Achievement(
            "After the last breath, only the silence remains",
            "The music has stop for the first time",
            40,
            () => musicien.CurrentState == Employee.EmployeeState.NotWorking // Condition to unlock the achievement
        );

            var achievementMusicien2 = new Achievement(
                "Music is the language of emotions",
                "The Musician worked again for the first time",
                70,
                () => musicien.NumberOfTimeWorked == 1
            );

            var achievementMusicien3 = new Achievement(
                "You can feel it",
                "The Musician worked again for the third time",
                70,
                () => musicien.NumberOfTimeWorked == 3
            );
            allAchievements[musicien].Add(achievementMusicien);
            allAchievements[musicien].Add(achievementMusicien2);
            allAchievements[musicien].Add(achievementMusicien3);
        }
        else if(employee.NameOfEmployee == "Painter")
        {
            var painter = (Painter)employee;
            var achievementPainter = new Achievement(
                "Roses are red, but this red is all yours!",
                "The red color is back for the first time",
                40,
                () => painter.firstTimeGettingRed
            );
            var achievementPainter2 = new Achievement(
                "Why is the sky blue?",
                "The blue color is back for the first time",
                40,
                () => painter.firstTimeGettingBlue
            );
            var achievementPainter3 = new Achievement(
                "The nature approves your choice",
                "The green color is back for the first time",
                40,
                () => painter.firstTimeGettingGreen
            );

            var achievementPainter4 = new Achievement(
                "You can see in RGB!",
                "You gaved back all the colors to the Painter at least one time",
                40,
                () => painter.firstTimeGettingRed && painter.firstTimeGettingBlue && painter.firstTimeGettingRed
            );

            var achievementPainter5 = new Achievement(
                "It was a little bit too hard alone, so we both held hands",
                "You worked 5 times with the Painter, you unlocked a new color",
                40,
                () => painter.NumberOfTimeWorked == 1
            );

            var achievementPainter6 = new Achievement(
                "Only the two of us was a little bit sad, so we made a circle of three",
                "You worked 10 times with the Painter, you unlocked a new color",
                40,
                () => painter.NumberOfTimeWorked == 2
            );
            allAchievements[painter].Add(achievementPainter);
            allAchievements[painter].Add(achievementPainter2);
            allAchievements[painter].Add(achievementPainter3);
            allAchievements[painter].Add(achievementPainter4);
            allAchievements[painter].Add(achievementPainter5);
            allAchievements[painter].Add(achievementPainter6);
        }
        else if(employee.NameOfEmployee == "Security")
        {
            var security = (Security)employee;
            var achievementSecurity1 = new Achievement(
                "We have the best security ever",
                "You worked for the first time with the security",
                40,
                () => security.NumberOfTimeWorked == 1
            );
            var achievementSecurity2 = new Achievement(
               "We are in war!",
               "The security stopped working for the first time",
               40,
               () => security.CurrentState == Employee.EmployeeState.NotWorking
           );
            var achievementSecurity3 = new Achievement(
               "She is starting to get very afraid",
               "You haven't chased the frog for a long time",
               40,
               () => security.GetStopTimeWorking() == 10
           );
            var achievementSecurity4 = new Achievement(
               "I am not afraid of frogs",
               "You have worked with the Security five time",
               40,
               () => security.NumberOfTimeWorked == 5
           );

            allAchievements[security].Add(achievementSecurity1);
            allAchievements[security].Add(achievementSecurity2);
            allAchievements[security].Add(achievementSecurity3);
            allAchievements[security].Add(achievementSecurity4);
        }
        else if(employee.NameOfEmployee == "Technicien")
        {
            var technicien = (Technicien)employee;
            var achievementTechnicien1 = new Achievement(
                "I am a vampire",
                "You worked with the Technician for the first time",
                40,
                () => technicien.NumberOfTimeWorked == 1
            );
            var achievementTechnicien2 = new Achievement(
                "Where are you?",
                "The Technician stopped working for the first time",
                40,
                () => technicien.CurrentState == Employee.EmployeeState.NotWorking
            );
            allAchievements[technicien].Add(achievementTechnicien1);
            allAchievements[technicien].Add(achievementTechnicien2);
        }
        else if(employee.NameOfEmployee == "Marketing")
        {
            var marketing = (Marketing)employee;
            var achievementMarketing1 = new Achievement(
                " 1 + 1 = 3",
                "The Accountant has stopped working for the first time",
                40,
                () => marketing.NumberOfTimeWorked == 1
            );
            var achievementMarketing2 = new Achievement(
                "Where are you?",
                "The Technician stopped working for the first time",
                40,
                () => marketing.CurrentState == Employee.EmployeeState.NotWorking
            );
            allAchievements[marketing].Add(achievementMarketing1);
            allAchievements[marketing].Add(achievementMarketing2);

        }
    }

    

    private void InitUnlockableEmployee()
    {
        _unlockableEmployeeOrder.Add("Painter");
        _unlockableEmployeeOrder.Add("Technicien");
        _unlockableEmployeeOrder.Add("Security");
        _unlockableEmployeeOrder.Add("Marketing");
        //TODO ADD OTHER EMPLOYEE EHRE
    }

    private void AddNewEmployee()
    {

        if(_unlockableEmployeeOrder.Count <= 0)
        {
            return;
        }
        var name = _unlockableEmployeeOrder[0];
        _unlockableEmployeeOrder.RemoveAt(0);
        var employee = Employee.CreateEmployee(name);
       

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
            //if the achievement is completed, we will add it to the queue
            if (!playerAchievements.Contains(achievement) && achievement.IsCompleted())
            {

                achievementQueue.Enqueue(achievement);
            }


        }

        //we show all the achievements that the player unlocked
        ShowAllAchievements();


    }

    private void ShowAllAchievements()
    {
        /**
         * Recursive method based on a queue. It will show all the achievements of the queue
         */

        //when the queue is empty,we check if we should increase the level
        if (achievementQueue.Count == 0)
        {
            GetTree().CreateTimer(WaitTimeBeforeLevelUp).Timeout += () =>
            {

                if (_starsProgressBar.Value >= _starsProgressBar.MaxValue)
                {
                    //if yes, we call the increase level method
                    IncreaseLevel();
                }


            };
            return;
        }

        var achievement = achievementQueue.Dequeue();

        if (!playerAchievements.Contains(achievement))
        {
            //the player got the achievement
            playerAchievements.Add(achievement); //we add it 
            AnimateProgress(achievement.NumberOfStars, _starsProgressBar); //circular animation
            TotalStars += achievement.NumberOfStars;
            _totalNumberOfStarsLabel.Text = TotalStars.ToString();
            _achievementDisplay.ShowAchievement(achievement);
            _starsPlayer.Play(); //we play the sound
            _currentAmountsOfStars += achievement.NumberOfStars;
            //we check if the player has all the achievment

            //TODO CHANGE THE 3 with the number of achievements
            if(playerAchievements.Count >= 3)
            {
                _globalSignals.EmitUnlockGlasses();
            }
        }


        //we show the next achievement after a short delay
        GetTree().CreateTimer(WaitTimeBeforeNextAchievement).Timeout += () =>
        {
            if (achievementQueue.Count >= 0)
            {
                ShowAllAchievements();

            }
        };


        



        //then we will check if the player can level up



    }

    private void IncreaseLevel()
    {
        
        //sometime this method can be called even if we haven't got the max value, we use this return to do nothing in this case
        if(_currentAmountsOfStars < _starsProgressBar.MaxValue)
        {
            return;
        }
        if (_starsProgressBar.Value == _starsProgressBar.MaxValue)
        {
            AnimateProgress((int)-(2* _starsProgressBar.Value - _currentAmountsOfStars), _starsProgressBar);
        }
        else
        {
            AnimateProgress((int)-(_starsProgressBar.Value), _starsProgressBar);
        }
        //_totalProgressPlayer.Play();
        //we reset the number of stars
        _currentAmountsOfStars = 0;
        //we update the label to show the current level
        _currentLevel += 1;
        _starsLevel.Text = _currentLevel.ToString();
        //AnimateProgress(20, _totalProgressBar);

    }

    

    private void AnimateProgress(int stars, Range node)
    {
        if (isTweening)
        {
            // if an animation is going, we add it in the queue
            animationQueue.Enqueue((stars, node));
            return;
        }

        StartTween(stars, node);
    }

    private void StartTween(int stars, Range node)
    {
        /**
         * Start the tween animation for the number of stars
         */
        isTweening = true;

        var targetValue = node.Value + stars;

        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out);
        tween.SetTrans(Tween.TransitionType.Sine);

        tween.TweenProperty(node, "value", targetValue, 1.0f);

        tween.Finished += () =>
        {
            isTweening = false;

            // we check if we have reached the maximum stars value
            if (node == _starsProgressBar && node.Value >= _starsProgressBar.MaxValue)
            {
                IncreaseLevel();
            }

            // we animate the next animation
            if (animationQueue.Count > 0)
            {
                var nextAnimation = animationQueue.Dequeue();
                StartTween(nextAnimation.stars, nextAnimation.node);
            }
        };

        tween.Play();
    }


    public List<Achievement> GetUnlockedAchievements()
    {
        return playerAchievements;
    }

    public List<Achievement> GetAllAchievements()
    {
        var list = new List<Achievement>();
        foreach( var achievements in allAchievements.Values)
        {
            foreach(var achievement in achievements)
            {
                list.Add(achievement);
            }
        }
        return list;
    }

    public List<Achievement> GetLockedAchievements()
    {
        /**
         * Return the list of achievement that he player hasn't unlocked yet 
         */
        var list = new List<Achievement>();
        var allAchievementsList = GetAllAchievements();
        foreach(var achievement in allAchievementsList)
        {
            if(!playerAchievements.Contains(achievement))
            {
                list.Add(achievement);
            }
        }
        return list;
    }

   

    public void PlayAchievement(Achievement achievement)
    {
        achievementQueue.Enqueue(achievement);
        ShowAllAchievements();

    }
}
