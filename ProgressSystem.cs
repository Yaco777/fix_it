using Godot;
using System;
using System.Collections.Generic;

public partial class ProgressSystem : CanvasLayer
{
    private List<Achievement> playerAchievements = new List<Achievement>(); //we store all the achievements of the player
    private Dictionary<Employee, List<Achievement>> allAchievements = new Dictionary<Employee, List<Achievement>>();
    //all achievements of the game
    private List<Employee> allEmployees = new List<Employee>();
    public int TotalStars { get; set; } = 0;
    private TextureProgressBar _starsProgressBar; //the circular progress bar
    private ProgressBar _totalProgressBar; //the global progress bar
    private AchievementDisplay _achievementDisplay; //shown when the player get an achievements

    //audio stram for the progress bar and the stars 
    private AudioStreamPlayer _starsPlayer;
    private AudioStreamPlayer _totalProgressPlayer;

    private List<string> _unlockableEmployeeOrder = new List<string>();
   

    [Export]
    private int MaxStarsValue { get; set; } = 100;

    [Export]
    private int MaxProgressValue { get; set; } = 100;

    [Export]
    private float WaitTimeBeforeLevelUp { get; set; } = 1f;

    private float WaitTimeBeforeNextAchievement { get; set; } = -1f;



    private Queue<Achievement> achievementQueue = new Queue<Achievement>();
    private bool isProcessingAchievements = false;
    private int _currentAmountsOfStars = 0;





    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        InitUnlockableEmployee();
        _totalProgressBar = GetNode<ProgressBar>("TotalProgress");
        _starsProgressBar = GetNode<TextureProgressBar>("StarsProgress");
        _achievementDisplay = GetNode<AchievementDisplay>("AchievementDisplay");
        _starsPlayer = GetNode<AudioStreamPlayer>("StarsPlayer");
        _totalProgressPlayer = GetNode<AudioStreamPlayer>("TotalProgressPlayer");
        var employees = GetNode<Node2D>("../../Employees").GetChildren();
        _starsProgressBar.MaxValue = MaxStarsValue;
        _totalProgressBar.MaxValue = MaxProgressValue;
        _achievementDisplay.Visible = true;
        WaitTimeBeforeNextAchievement = _achievementDisplay.AchievementDisplayTime + _achievementDisplay.AchievementFadeInTime + _achievementDisplay.AchievementFadeOutTime; ;



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
            40,
            () => musicien.CurrentState == Employee.EmployeeState.NotWorking // Condition to unlock the achievement
        );

        var achievementMusicien2 = new Achievement(
            "Music is the language of emotions",
            "The musicien worked again for the first time",
            70,
            () => musicien.NumberOfTimeWorked == 1
        );

        var achievementMusicien3 = new Achievement(
            "You can feel it",
            "The musicien worked again for the third time",
            70,
            () => musicien.NumberOfTimeWorked == 3
        );

        //---achievements of the painter---
        var painter = (Painter)allEmployees.Find(emp => emp.NameOfEmployee == "Painter");
        var achievementPainter = new Achievement(
            "Roses are red, but this red is all yours!",
            "The red color is back for the first time",
            40,
            () => painter.firstTimeGettingRed == true
        );
        var achievementPainter2 = new Achievement(
            "Why is the sky blue?",
            "The blue color is back for the first time",
            40,
            () => painter.firstTimeGettingBlue == true
        );
        var achievementPainter3 = new Achievement(
            "The nature approves your choice",
            "The green color is back for the first time",
            40,
            () => painter.firstTimeGettingGreen == true
        );

        var achievementPainter4 = new Achievement(
            "You can see in RGB!",
            "You gaved back all the colors to the painter at least one time",
            40,
            () => painter.firstTimeGettingRed == true && painter.firstTimeGettingBlue == true && painter.firstTimeGettingRed == true
        );

        var achievementPainter5 = new Achievement(
            "It was a little bit too hard alone, so we both held hands",
            "You worked 5 times with the painter, you unlocked a new color",
            40,
            () => painter.NumberOfTimeWorked == 1
        );

        var achievementPainter6 = new Achievement(
            "Only the two of us was a little bit sad, so we made a circle of three",
            "You worked 10 times with the painter, you unlocked a new color",
            40,
            () => painter.NumberOfTimeWorked == 2
        );






        //we add all the achievements
        allAchievements[musicien].Add(achievementMusicien);
        allAchievements[musicien].Add(achievementMusicien2);
        allAchievements[musicien].Add(achievementMusicien3);

        allAchievements[painter].Add(achievementPainter);
        allAchievements[painter].Add(achievementPainter2);
        allAchievements[painter].Add(achievementPainter3);
        allAchievements[painter].Add(achievementPainter4);
        allAchievements[painter].Add(achievementPainter5);
        allAchievements[painter].Add(achievementPainter6);

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
        var employee = Employee.createEmployee(name);
       

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
        if (achievementQueue.Count == 0)
        {
            return;
        }

        var achievement = achievementQueue.Dequeue();

        if (!playerAchievements.Contains(achievement))
        {
            playerAchievements.Add(achievement);
            AnimateProgress(achievement.NumberOfStars, _starsProgressBar);
            TotalStars += achievement.NumberOfStars;
            _achievementDisplay.ShowAchievement(achievement);
            _starsPlayer.Play();
            _currentAmountsOfStars += achievement.NumberOfStars;
        }


        //we show all the achievements 
        //wait 5s

        GetTree().CreateTimer(WaitTimeBeforeNextAchievement).Timeout += () =>
        {
            if (achievementQueue.Count > 0)
            {
                ShowAllAchievements();

            }
        };


        GetTree().CreateTimer(WaitTimeBeforeLevelUp).Timeout += () =>
        {

            if (_starsProgressBar.Value >= _starsProgressBar.MaxValue)
            {
                //if yes, we call the increase level method
                _totalProgressPlayer.Play();
                IncreaseLevel();
            }


        };



        //then we will check if the player can level up



    }

    private void IncreaseLevel()
    {
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
        _currentAmountsOfStars = 0;
        AnimateProgress(20, _totalProgressBar);

    }

    private bool isTweening = false;
    private Queue<(int stars, Godot.Range node)> animationQueue = new Queue<(int stars, Godot.Range node)>();

    private void AnimateProgress(int stars, Godot.Range node)
    {
        if (isTweening)
        {
            // Si une animation est en cours, on ajoute simplement à la file d'attente
            animationQueue.Enqueue((stars, node));
            return;
        }

        StartTween(stars, node);
    }

    private void StartTween(int stars, Godot.Range node)
    {
        isTweening = true;

        var targetValue = node.Value + stars;

        Tween tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out);
        tween.SetTrans(Tween.TransitionType.Sine);

        tween.TweenProperty(node, "value", targetValue, 1.0f);

        tween.Finished += () =>
        {
            isTweening = false;

            // On vérifie si la barre des étoiles a atteint le maximum
            if (node == _starsProgressBar && node.Value >= _starsProgressBar.MaxValue)
            {
                _totalProgressPlayer.Play();
                IncreaseLevel();
            }

            // Si des animations sont en attente, on traite la suivante
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

    internal void looseStars(int amountStarsRequired)
    {
        GD.Print("à faire !! Le joueur est censé avoir perdu : " + amountStarsRequired);
    }
}
