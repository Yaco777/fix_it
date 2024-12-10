using System;
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

    private List<string> _unlockableEmployeeOrder = new List<string>();
   

    [Export]
    public int MaxStarsValue { get; set; } = 18;

    [Export]
    private int MaxProgressValue { get; set; } = 100; //not used anymore

    [Export]
    private float WaitTimeBeforeLevelUp { get; set; } = 1f;

    private float WaitTimeBeforeNextAchievement; //by default this value will be the time to display one achievement + fade in time + fade out time

    private int _currentLevel = 0; //the level that will be shown at the top left corner. The level will increase when the star will be full



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

        _totalProgressBar = GetNode<ProgressBar>("TotalProgress");
        _starsProgressBar = GetNode<TextureProgressBar>("StarsProgress");
        _achievementDisplay = GetNode<AchievementDisplay>("AchievementDisplay");
        _starsPlayer = GetNode<AudioStreamPlayer>("StarsPlayer");
        _totalProgressPlayer = GetNode<AudioStreamPlayer>("TotalProgressPlayer");
        _starsLevel = _starsProgressBar.GetNode<Label>("StarsLevel");
        _globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
        var employees = GetNode<Node2D>("../../Employees").GetChildren();
        
        _starsLevel.Text = _currentLevel.ToString(); //we start at the level 1
        _starsProgressBar.MaxValue = MaxStarsValue;
        _totalProgressBar.MaxValue = MaxProgressValue;
        _achievementDisplay.Visible = true;
        WaitTimeBeforeNextAchievement = _achievementDisplay.AchievementDisplayTime + _achievementDisplay.AchievementFadeInTime + _achievementDisplay.AchievementFadeOutTime; ;
        _globalSignals.NewWorkDone += IncreaseStar;
        

        //this loop is used for the employess that are already in the game when we start it, not the one added by the rooms
        foreach (var emp in employees)
        {
            //we store all employees and when the state of the employee change we will call CheckNewAchievements

            if (emp is Employee empConv)
            {
                allEmployees.Add((Employee)emp);
                empConv.EmployeeStateChanged += CheckNewAchievements;
                empConv.CheckAchievement += CheckNewAchievements;
                allAchievements[(Employee)emp] = new List<Achievement>();
                AddAchievementsForOneEmployee(empConv);
            }
        }

        

    }

    private void IncreaseStar()
    {
        AnimateProgress(1, _starsProgressBar);
        _currentLevel++;
        TotalStars++;
        _starsLevel.Text = _currentLevel.ToString();

        GetTree().CreateTimer(5).Timeout += () =>
        {
            if (_currentLevel >= 18)
            {
                IncreaseLevel();
            }
        };
        
        


    }

    private void IncreaseLevel()
    {

        
        AnimateProgress(-(int)_starsProgressBar.Value, _starsProgressBar);
 
        _starsLevel.Text = "?";
        _globalSignals.EmitUnlockGlasses();


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
            "The music has stopped for the first time",
            40,
            () => musicien.CurrentState == Employee.EmployeeState.NotWorking, // Condition to unlock the achievement
            3
        );

            var achievementMusicien2 = new Achievement(
                "Music is the language of emotions",
                "The Musician has worked for the first time",
                70,
                () => musicien.NumberOfTimeWorked == 1,
                1
            );

            var achievementMusicien3 = new Achievement(
                "The Symphony has started",
                "The Musician has worked for the second time",
                70,
                () => musicien.NumberOfTimeWorked == 2,
                2
            );

            var achievementMusicien4 = new Achievement(
                "You can feel it",
                "The Musician has worked for the third time",
                70,
                () => musicien.NumberOfTimeWorked == 3,
                3
            );
            allAchievements[musicien].Add(achievementMusicien);
            allAchievements[musicien].Add(achievementMusicien2);
            allAchievements[musicien].Add(achievementMusicien3);
            allAchievements[musicien].Add(achievementMusicien4);

        }
        else if(employee.NameOfEmployee == "Painter")
        {
            var painter = (Painter)employee;
            var achievementPainter = new Achievement(
                "Roses are red, but this red is all yours!",
                "The red color is back for the first time",
                40,
                () => painter.firstTimeGettingRed,
                3
            );
            var achievementPainter2 = new Achievement(
                "Why is the sky blue?",
                "The blue color is back for the first time",
                40,
                () => painter.firstTimeGettingBlue,
                3
            );
            var achievementPainter3 = new Achievement(
                "Nature approves your choice",
                "The green color is back for the first time",
                40,
                () => painter.firstTimeGettingGreen,
                3
            );

            var achievementPainter4 = new Achievement(
                "You can see in RGB!",
                "You gaved back all the colors to the Painter at least one time",
                40,
                () => painter.firstTimeGettingRed && painter.firstTimeGettingBlue && painter.firstTimeGettingGreen,
                3
            );

            var achievementPainter5 = new Achievement(
                "When you say blue, let others mean red", 
                "You worked 1 times with the Painter, you unlocked a new color",
                40,
                () => painter.NumberOfTimeWorked == 1,
                1
            );

            var achievementPainter6 = new Achievement(
                "It was a little bit too hard alone, so we both held hands",
                "You worked 2 times with the Painter, you unlocked a new color",
                40,
                () => painter.NumberOfTimeWorked == 2,
                2
            );

            var achievementPainter7 = new Achievement(
                "Only the two of us was a little bit sad, so we made a circle of three",
                "You worked 3 times with the Painter, you unlocked a new color",
                40,
                () => painter.NumberOfTimeWorked == 3,
                3
            );
            allAchievements[painter].Add(achievementPainter);
            allAchievements[painter].Add(achievementPainter2);
            allAchievements[painter].Add(achievementPainter3);
            allAchievements[painter].Add(achievementPainter4);
            allAchievements[painter].Add(achievementPainter5);
            allAchievements[painter].Add(achievementPainter6);
            allAchievements[painter].Add(achievementPainter7);
        }
        else if(employee.NameOfEmployee == "Security")
        {
            var security = (Security)employee;
            var achievementSecurity1 = new Achievement(
                "We have the best security ever", 
                "You worked with the Security for the first time",
                40,
                () => security.NumberOfTimeWorked == 1,
                1
            );
            var achievementSecurity2 = new Achievement(
               "We are in war!",
               "The Security has stopped working for the first time",
               40,
               () => security.CurrentState == Employee.EmployeeState.NotWorking,
               3
           );
            var achievementSecurity3 = new Achievement(
               "She is starting to get very afraid",
               "You haven't chased the frog for a long time",
               40,
               () => security.GetStopTimeWorking() >= 60, 
               3
           );
            var achievementSecurity4 = new Achievement(
              "Playing it safe is the riskiest choice we can make",
              "You have worked with the Security two time",
              40,
              () => security.NumberOfTimeWorked == 2,
              2
          );
            var achievementSecurity5 = new Achievement( 
               "I am not afraid of frogs",
               "You have worked with the Security three time",
               40,
               () => security.NumberOfTimeWorked == 3,
               3
           );

            allAchievements[security].Add(achievementSecurity1);
            allAchievements[security].Add(achievementSecurity2);
            allAchievements[security].Add(achievementSecurity3);
            allAchievements[security].Add(achievementSecurity4);
            allAchievements[security].Add(achievementSecurity5);
        }
        else if(employee.NameOfEmployee == "Technicien")
        {
            var technicien = (Technician)employee;
            var achievementTechnicien1 = new Achievement(
                "I am a vampire",
                "You worked with the Technician for the first time",
                40,
                () => technicien.NumberOfTimeWorked == 1, 
                1
            );
            var achievementTechnicien2 = new Achievement(
                "Where are you?",
                "The Technician stopped working for the first time",
                40,
                () => technicien.CurrentState == Employee.EmployeeState.NotWorking,
                3
            );
            var achievementTechnicien3 = new Achievement(
               "Moonlight drowns out all but the brightest stars",
               "You worked with the Technician for the second time",
               40,
               () => technicien.NumberOfTimeWorked == 2,
               2
           );
            var achievementTechnicien4 = new Achievement(
               "Give light, and the darkness will disappear of itself",
               "You worked with the Technician for the third time",
               40,
               () => technicien.NumberOfTimeWorked == 3,
               3
           );
            allAchievements[technicien].Add(achievementTechnicien1);
            allAchievements[technicien].Add(achievementTechnicien2);
            allAchievements[technicien].Add(achievementTechnicien3);
            allAchievements[technicien].Add(achievementTechnicien4);
        }
        else if(employee.NameOfEmployee == "Marketing")
        {
            var marketing = (Accountant)employee;
            var achievementMarketing1 = new Achievement(
                " 1 + 1 = 3",
                "The Accountant has stopped working for the first time",
                40,
                () => marketing.CurrentState== Employee.EmployeeState.NotWorking,
                3
            );
            var achievementMarketing2 = new Achievement(
                "You are better than me",
                "You completed the accountant minigame one time",
                40,
                () => marketing.NumberOfTimeWorked == 1, 
                1
            );
            var achievementMarketing3 = new Achievement(
                "You are WAY better than me",
                "You completed the accountant minigame two time",
                40,
                () => marketing.NumberOfTimeWorked == 2, 
                2
            );
            var achievementMarketing4 = new Achievement(
                "Could you replace me?",
                "You completed the accountant minigame three time",
                40,
                () => marketing.NumberOfTimeWorked == 3,
                3
            );
            allAchievements[marketing].Add(achievementMarketing1);
            allAchievements[marketing].Add(achievementMarketing2);
            allAchievements[marketing].Add(achievementMarketing3);
            allAchievements[marketing].Add(achievementMarketing4);

        }
        else if(employee.NameOfEmployee == "Cook")
        {
            var cook = (Cook)employee;
            var achievementCook1 = new Achievement(
                "Let him cook",
                "You completed the cook minigame for the first time",
                40,
                () => cook.NumberOfTimeWorked == 1, 
                1
                );
            var achievementCook2 = new Achievement(
                "Where are my ingredients?",
                "The Cook has stopped working for the first time",
                40,
                () => cook.CurrentState == Employee.EmployeeState.NotWorking,
                3
                );
            var achievementCook3 = new Achievement(
                "The fire is burning",
                "You completed the cook minigame for the second time",
                40,
                () => cook.NumberOfTimeWorked == 2,
                2
                );
            var achievementCook4 = new Achievement(
                "No one is born a great cook",
                "You completed the cook minigame for the third time",
                40,
                () => cook.NumberOfTimeWorked == 3,
                3
                );
            allAchievements[cook].Add(achievementCook1);
            allAchievements[cook].Add(achievementCook2);
            allAchievements[cook].Add(achievementCook3);
            allAchievements[cook].Add(achievementCook4);

        }
    }

   

    private void AddNewEmployee()
    {

        if(_unlockableEmployeeOrder.Count <= 0)
        {
            return;
        }
        _unlockableEmployeeOrder.RemoveAt(0);
       

    }

    private void CheckNewAchievements(int newState, string employeeName)
    {
        /**
         *  Check if an achievements has been unlocked for the employee "employeeName"
         */

        //we get the employee
        var employee = allEmployees.Find(emp => emp.NameOfEmployee == employeeName) ?? throw new ArgumentException("The state of " + employeeName + " changed but it seems that the name is wrong, we can't check their achievements !");
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
           
            return;
        }

        var achievement = achievementQueue.Dequeue();

        if (!playerAchievements.Contains(achievement))
        {
            //the player got the achievement
            achievement.Unlocked = true;
            playerAchievements.Add(achievement); //we add it 
            //AnimateProgress(achievement.NumberOfStars, _starsProgressBar); //circular animation
            _achievementDisplay.ShowAchievement(achievement);
            _starsPlayer.Play(); //we play the sound
            _currentAmountsOfStars += achievement.NumberOfStars;
            
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
        var unlockedNames = new List<Achievement>();

        foreach (var achievement in playerAchievements)
        {
            achievement.Unlocked = true;
            unlockedNames.Add(achievement);
        }

        return unlockedNames;
    }

    public List<Achievement> GetAllAchievements()
    {
        var list = new List<Achievement>
    {
        new Achievement("After the last breath, only the silence remains", 3),
        new Achievement("Music is the language of emotions", 1),
        new Achievement("The Symphony has started", 2),
        new Achievement("You can feel it", 3),
        new Achievement("Roses are red, but this red is all yours!", 1),
        new Achievement("Why is the sky blue?", 1),
        new Achievement("Nature approves your choice", 1),
        new Achievement("You can see in RGB!", 3),
        new Achievement("It was a little bit too hard alone, so we both held hands", 2),
        new Achievement("Only the two of us was a little bit sad, so we made a circle of three", 3),
        new Achievement("We have the best security ever", 1),
        new Achievement("We are in war!", 1),
        new Achievement("She is starting to get very afraid", 3),
        new Achievement("I am not afraid of frogs", 3),
        new Achievement("I am a vampire", 1),
        new Achievement("Where are you?", 1),
        new Achievement("1 + 1 = 3", 1),
        new Achievement("You are better than me", 1),
        new Achievement("You are WAY better than me", 2),
        new Achievement("Could you replace me?", 3),
        new Achievement("Let him cook", 1),
        new Achievement("Where are my ingredients?", 1),
        new Achievement("The fire is burning", 2),
        new Achievement("No one is born a great cook", 3),
        new Achievement("Moonlight drowns out all but the brightest stars", 2),
        new Achievement("Give light, and the darkness will disappear of itself", 3),
        new Achievement("Playing it safe is the riskiest choice we can make", 3)
    };
        foreach(var a in list)
        {
            if(playerAchievements.Contains(a))
            {
                a.Unlocked = true;
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
