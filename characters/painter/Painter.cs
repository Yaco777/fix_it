using System;
using System.Collections.Generic;
using Godot;

public partial class Painter : Employee
{

    private HashSet<string> ColorsUnlocked { get; set; } = new HashSet<string>(); //the colors that the player has unlocked
    private HashSet<string> CurrentColors { get; set; } = new HashSet<string>(); //the current colors of the camera
    public static HashSet<string> ColorsMissings { get; set; } = new HashSet<string>(); //the missing colors, it need to be static to be accessible by the building

    [Export]
    public string FirstBrush { get; set; } = "Red brush";

    [Export]
    public string NewColorUnlockedMessage { get; set; } = "Look what I've found! A new color!";

    [Export]

    public int NumberOfWorkToUnlockedSecondColor { get; set; } = 1;

    [Export]
    public int NumberOfWorkToUnlockedThirdColor { get; set; } = 2;


  

    private GlobalSignals _globalSignals;
    private AnimatedSprite2D _painterAnimation;
    private AudioStreamPlayer _successPlayer;

    private Random _random = new Random();

    private static readonly List<string> REQUIRED_ITEMS = new List<string>
    {
    "Red brush",
    "Blue brush",
    "Green brush"
    };

    private static readonly List<string> _chatMessages = new List<string>

    {
        "In a single brushstroke!",
        "I'm so clumsy",
        "I will never fall asleep, there is no way..."
    };

    private static readonly List<string> _stopWorkingMessages = new List<string>
    {
        "Where is it ? Here ? No...",
        "AHHHHHHHHH"
    };

    private static readonly List<string> _backToWork = new List<string>
    {
        "Me? Lose something? Never",
        "My inspiration is back!"
    };

    private List<string> _oneColorUnlocked = new List<string>
    {
        "It's better than nothing",
        "One more...?"
    };

    public bool firstTimeGettingRed;
    public bool firstTimeGettingGreen;
    public bool firstTimeGettingBlue;



    public Painter() : base(_chatMessages, _stopWorkingMessages, _backToWork, "Painter")
    {
        ColorsMissings.Clear(); //we clear the static hashSet
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
        _painterAnimation = GetNode<AnimatedSprite2D>("PainterSprites");
        _successPlayer = GetNode<AudioStreamPlayer>("Success");
        ColorsUnlocked.Add(FirstBrush);
        CurrentColors.Add(FirstBrush);
        //StopWorking();
        _globalSignals.EmitColorBack("Red brush");
        _painterAnimation.Play();

        

    }

    public override void StartWorking()
    {

        base.StartWorking();
        _painterAnimation.Animation = "working";

        
    }

    public override void StopWorking()
    {
        base.StopWorking();
        //the painter will loose a random amount of colors
        var numberOfColorsToRemove = 1 + _random.Next(CurrentColors.Count);
        for (int i = 0; i < numberOfColorsToRemove; i++)
        {
            var colorToRemove = RemoveOneRandomColor();
            _globalSignals.EmitColorLost(colorToRemove);
        }


        _painterAnimation.Animation = "not_working";
        CurrentState = EmployeeState.NotWorking;
        EmitSignal(SignalName.EmployeeStateChanged, (int)EmployeeState.NotWorking, "Painter"); //we emit the signal manually
        //we cannot use the ChangeState of Employee because it will call the stop working method again

    }


    protected override void Interact(Hero hero)
    {
        var hasOneItem = false; //check if the player has at least of the required item
        foreach (var item in REQUIRED_ITEMS)
        {
            if (hero.HasItem(item))
            {
                hasOneItem = true;
                if (item == "Red brush" && firstTimeGettingRed == false)
                {
                    firstTimeGettingRed = true;
                }
                else if (item == "Blue brush" && firstTimeGettingBlue == false)
                {
                    firstTimeGettingBlue = true;
                }
                else if (item == "Green brush" && firstTimeGettingGreen == false)
                {
                    firstTimeGettingGreen = true;
                }
                _successPlayer.Play();
                EmitSignal(SignalName.CheckAchievement, (int)CurrentState, NameOfEmployee);
                _globalSignals.EmitColorBack(item);
                ColorsMissings.Remove(item);
                CurrentColors.Add(item);
                hero.RemoveItem();


            }

        }

        if (hasOneItem && ColorsMissings.Count != 0)
        {
            ShowNewColorUnlocked(); //if the player got one item and there are still items that need to be collected, we show a specific message
        }
        else if (hasOneItem && ColorsMissings.Count == 0)
        {
            SetState(EmployeeState.Working);
            if(!CheckNewColors()) //we either show the new color message or the back to work message
            {
                ShowBackToWorkChat(); //if all items have been collected, the painter will go back to work

            }
          
            
            
        }
        else
        {
            base.Interact(hero);
        }

    }

    private bool CheckNewColors()
    {
       
        //we check if we need to unlock the second color
        if (_numberOfTimeWorked == NumberOfWorkToUnlockedSecondColor)
        {
            var allRemaingColors = new List<string>(REQUIRED_ITEMS);
            allRemaingColors.Remove(FirstBrush);
            var color = allRemaingColors[_random.Next(allRemaingColors.Count)];
            ColorsUnlocked.Add(color);
            CurrentColors.Add(color);
            ShowTemporaryDialog(NewColorUnlockedMessage);
            return true;

        }
        //we unlock the third color

        if (_numberOfTimeWorked == NumberOfWorkToUnlockedThirdColor)
        {
            ColorsUnlocked.Clear();
            CurrentColors.Clear();
            foreach (var color in REQUIRED_ITEMS)
            {
                ColorsUnlocked.Add(color);
                CurrentColors.Add(color);

            }
            ShowTemporaryDialog(NewColorUnlockedMessage);
            return true;

        }
        return false;
    }


    private void ShowNewColorUnlocked()
    {
        ShowTemporaryDialog(GetRandomColorBackMessage());
    }

    private string GetRandomColorBackMessage()
    {
        return _oneColorUnlocked[_random.Next(_oneColorUnlocked.Count)];

    }

    private string RemoveOneRandomColor()
    {
        //the painter will loose one random color
        var colorList = new List<string>(CurrentColors);
        var color = colorList[_random.Next(colorList.Count)];
        CurrentColors.Remove(color);
        ColorsMissings.Add(color);
        return color;
    }
}
