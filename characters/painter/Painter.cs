using Godot;
using System;
using System.Collections.Generic;

public partial class Painter : Employee
{

    public static List<string> ColorsUnlocked { get; set; } = new List<string>(); //the colors that the player has unlocked
    public static List<string> CurrentColors { get; set; } = new List<string>(); //the current colors of the camera
    public static List<string> ColorsMissings { get; set; } = new List<string>(); //the missing colors


    private GlobalSignals _globalSignals;
    private AnimatedSprite2D _painterAnimation;

    private Random _random = new Random();

    private static List<string> REQUIRED_ITEMS = new List<string>
    {
    "Red brush",
    "Blue brush",
    "Green brush"
    };

    private static List<string> _chatMessages = new List<string>

    {
        "In a single brushstroke!",
        "I'm so clumsy",
        "I will never fall asleep, there is no way...",
        "Music soothes aches and pains!"
    };

    private static List<string> _stopWorkingMessages = new List<string>
    {
        "Where is it ? Here ? No...",
        "AHHHHHHHHH"
    };

    private static List<string> _backToWork = new List<string>
    {
        "Me? Lose something? Never",
        "My inspiration is back!"
    };

    private static List<string> _oneColorUnlocked = new List<string>
    {
        "It's better than nothing",
        "One more...?"
    };

    public bool firstTimeGettingRed = false;
    public bool firstTimeGettingGreen = false;
    public bool firstTimeGettingBlue = false;



    public Painter() : base(_chatMessages, _stopWorkingMessages, _backToWork, "Painter")
    {
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
        _painterAnimation = GetNode<AnimatedSprite2D>("PainterSprites");
        ColorsUnlocked.Add("Green brush");
        ColorsUnlocked.Add("Red brush");
        ColorsUnlocked.Add("Blue brush");
        CurrentColors.Add("Green brush");
        CurrentColors.Add("Red brush");
        CurrentColors.Add("Blue brush");
        StopWorking();

        _painterAnimation.Play();

    }

    public override void StartWorking()
    {

        _painterAnimation.Animation = "working";
    }

    public override void StopWorking()
    {
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




    public override void Interact(Hero hero)
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
            ShowBackToWorkChat(); //if all items have been collected, the painter will go back to work
            SetState(EmployeeState.Working);
        }
        else
        {
            base.Interact(hero);
        }

    }

    private void ShowNewColorUnlocked()
    {
        base.ShowTemporaryDialog(GetRandomColorBackMessage());
    }

    private string GetRandomColorBackMessage()
    {
        return _oneColorUnlocked[_random.Next(_oneColorUnlocked.Count)];

    }

    private string RemoveOneRandomColor()
    {
        //the painter will loose one random color
        var color = CurrentColors[_random.Next(CurrentColors.Count)];
        CurrentColors.Remove(color);
        ColorsMissings.Add(color);
        return color;
    }
}
