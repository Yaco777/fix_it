using System.Collections.Generic;
using Godot;

public partial class Technician : Employee
{
    private AnimatedSprite2D _technicianAnimation;
    private static List<string> _chatMessages = new List<string>

    {
        "Red with red, blue with blue!",
        "I tangled all the wires",
        "I am a master of electricity"
    };

// One to delete between _stopWorkingMessages and _backToWork
    private static List<string> _stopWorkingMessages = new List<string>
    {
        "Sorry, I made a mistake",
        "Again a short circuit, it's a mystery"
    };

    private static List<string> _backToWork = new List<string>
    {
        "Sorry, I made a mistake",
        "Again a short circuit, it's a mystery"
    };

    [Export]
    private float DarknessAmount { get; set; } = 0.9f; //the maximum value for the darkness

    [Export]
    private float LowestDarknessAmount { get; set; } = 0.7f; //the lowest value for the darkness

    [Export]
    private float DarknessTransitionSpeed { get; set; } = 1f; //transition from light on to light off

    [Export]
    private float BackToLightTransitionSpeed { get; set; } = 1f; //transition from light off to light on


    [Export]
    private float OscillationSpeed { get; set; } = 1f; //oscillation speed between darknessAmount and LowestDarknessAMount

    private float _time; //variable used for the oscillation

    private bool _firstStart = true; //when the technicien work for the first start, the light on sound won't be used


    private ColorRect _colorRect;

    [Export]
    private AudioStream _lightOnStream;
    [Export]
    private AudioStream _lightOffStream;
    private AudioStreamPlayer _lightOnPlayer;
    private AudioStreamPlayer _lightOffPlayer;
    private AudioStreamPlayer _successPlayer;



    public Technician() : base(_chatMessages, _stopWorkingMessages, _backToWork, "Technician")
    {
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _colorRect = GetNode<ColorRect>("../../Hero/Camera/CanvasWithShader/TechnicienBlackRectangle");
        _colorRect.Color = new Color(0, 0, 0, 0);
        _lightOnPlayer = GetNode<AudioStreamPlayer>("LightOnSound");
        _lightOnPlayer.Stream = _lightOnStream;
        _lightOffPlayer = GetNode<AudioStreamPlayer>("LightOffSound");
        _lightOffPlayer.Stream = _lightOffStream;
        _technicianAnimation = GetNode<AnimatedSprite2D>("TechnicienSprites");
        _successPlayer = GetNode<AudioStreamPlayer>("Success");
        StartWorking();
        _technicianAnimation.Play();
        
    }

    public override void StopWorking()
    {
        base.StopWorking();
        var tween = GetTree().CreateTween();
        //we put the "darknessAmount" value for the alpha 
        tween.TweenProperty(_colorRect, "color", new Color(0, 0, 0, DarknessAmount), DarknessTransitionSpeed);
        _lightOffPlayer.Play();
        _technicianAnimation.Animation = "not_working";
    }

    public override void StartWorking()
    {
        base.StartWorking();
        if(_firstStart == false)
        {
            //if it's not the first time that the employee start working, we play the sound
            _lightOnPlayer.Play();
        }
        _firstStart = false;
        var tween = GetTree().CreateTween();
        //we remove the black rectangle
        tween.TweenProperty(_colorRect, "color", new Color(0, 0, 0, 0), BackToLightTransitionSpeed);
        _technicianAnimation.Animation = "working";
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        // if the technicien isn't working, we will oscillate the lights
        if (CurrentState == EmployeeState.NotWorking)
        {
            _time += (float)delta * OscillationSpeed; 
            float alphaValue = Mathf.Lerp(LowestDarknessAmount, DarknessAmount, (Mathf.Cos(_time) + 1) / 2);
            _colorRect.Color = new Color(0, 0, 0, alphaValue); 
        }
        else
        {
            _time = 0;
        }
    }


    protected override void Interact(Hero hero)
    {
        //the technicien will return to it's working state when we talk to him
        if (EmployeeState.NotWorking == CurrentState)
        {
            _successPlayer.Play();
            ShowBackToWorkChat();
            GD.Print("avant on a travail : " + _numberOfTimeWorked);
            SetState(EmployeeState.Working);
            GD.Print("after : " + _numberOfTimeWorked);


        }
        else
        {
            base.Interact(hero);
        }

    }


}
