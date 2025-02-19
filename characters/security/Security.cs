using System;
using System.Collections.Generic;
using Godot;

public partial class Security : Employee
{
    private AudioStreamPlayer _alertStreamPlayer;
    private Area2D _currentArea;
    private GlobalSignals _globalSignals;
    private Collectible _frog;
    private Vector2 _frogDirection;
    private AnimatedSprite2D _frogAnimation;
    private double _timeStopWorking = 0;
    private AudioStreamPlayer _successPlayer;

 

    [Export]
    public int FrogSpeed { get; set; } = 200;
    private bool _hasRemovedFrog;
    private AnimatedSprite2D _securityAnimation;
    private static List<string> _chatMessages = new List<string>
    {
        "I am your shield, fear nothing",
        "I don't now why this TV is not working"
    };

    private static List<string> _stopWorkingMessages = new List<string>
    {
        "WE ARE IN WAR",
        "An intruder!!!"
    };

    private static List<string> _backToWork = new List<string>
    {
        "Sorry, I though it was something bigger",
        "Please don't say anything"
    };

    public Security() : base(_chatMessages, _stopWorkingMessages, _backToWork, "Security")
    {
    }

    public override void _Ready()
    {
        _frogAnimation = GetNode<AnimatedSprite2D>("FrogSprite");
        _frogAnimation.Play();
        _frogAnimation.Visible = false;
        _frogAnimation.ZIndex = 3;
        base._Ready();
        _alertStreamPlayer = GetNode<AudioStreamPlayer>("Alert");
        _globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
        _securityAnimation = GetNode<AnimatedSprite2D>("SecuritySprite");
        _successPlayer = GetNode<AudioStreamPlayer>("Success");
        _globalSignals.FrogCollected += OnFrogCollected; 
        StartWorking();
        _securityAnimation.Animation = "working";
        _securityAnimation.Play();
        _timeStopWorking = 0;
    }

    private void OnFrogCollected()
    {
        _hasRemovedFrog = true;
        _frogAnimation.Visible = false;
    }
    public override void StartWorking()
    {
      
        base.StartWorking();
        _alertStreamPlayer.Stop(); //we stop the alert
        _globalSignals.EmitAlertStateChanged(false);
        _securityAnimation.Animation = "working";
        

    }

    public override void StopWorking()
    {
        base.StopWorking();
        _alertStreamPlayer.Play();
        _timeStopWorking = 0;
        _frog = Collectible.CreateCollectible("Frog");
        
        _frogAnimation.Visible = true;
        _currentArea = Building.GetRandomArea2D();
        _frog.GlobalPosition = Building.GetRandomPositionForItemForSpecificArea(_currentArea, false);
        _frogAnimation.GlobalPosition = _frog.GlobalPosition;
        GetTree().Root.GetChild(0).AddChild(_frog);
        _frog.HideSprite();
        _globalSignals.EmitAlertStateChanged(true);
        _securityAnimation.Animation = "idle";
    }


    protected override void Interact(Hero hero)
    {
        //if the hero has removed the frog, the employee will go back to work
        if(_hasRemovedFrog)
        {
            _hasRemovedFrog = false;
            _frog = null;
            _successPlayer.Play();
            ShowBackToWorkChat();
            SetState(EmployeeState.Working);

        }
        else
        {
            base.Interact(hero);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (CurrentState == EmployeeState.NotWorking)
        {
            _timeStopWorking += delta;
        }
       
        CheckAchievements(SignalName.CheckAchievement);
      
        //if the frog is present, the frog will move
        if (_hasRemovedFrog == false && _frog != null)
        {
            MoveFrog(delta);
            _frogAnimation.GlobalPosition = _frog.GlobalPosition;
            if (!IsFrogInCurrentArea())
            {
                _frogDirection *= -1; //when the frog leave the area, it will go in the opposite direction
                if(_frogDirection.X == -1)
                {
                    _frogAnimation.FlipH = true;
                }
                else
                {
                    _frogAnimation.FlipH = false;
                }
            }

        }
    }

    private void MoveFrog(double delta)
    {
        
        if (_frogDirection == Vector2.Zero)
        {
            _frogDirection = new Vector2(1, 0); // Move right initially
        }

        _frog.GlobalPosition += _frogDirection * FrogSpeed * (float)delta;
    }

    private bool IsFrogInCurrentArea()
    {
        /**
         * Check if the frog is in the current Area2D
         */

        var collisionShape = _currentArea.GetNode<CollisionShape2D>("CollisionShape2D");
        if (collisionShape != null)
        {

            if (collisionShape.Shape is SegmentShape2D segmentShape)
            {
                //we compute the bounds
                float minX = Math.Min(segmentShape.A.X, segmentShape.B.X);
                float maxX = Math.Max(segmentShape.A.X, segmentShape.B.X);

                var result = _frog.GlobalPosition.X >= _currentArea.GlobalPosition.X + minX && _frog.GlobalPosition.X <= maxX + _currentArea.GlobalPosition.X;
                // check if the frog's X position is within the bounds

                if(_frog.GlobalPosition.X <= _currentArea.GlobalPosition.X + minX)
                {
                    _frog.GlobalPosition = new Vector2(_currentArea.GlobalPosition.X, _frog.GlobalPosition.Y);
                }
                else if (_frog.GlobalPosition.X >= _currentArea.GlobalPosition.X + maxX)
                {
                    _frog.GlobalPosition = new Vector2(_currentArea.GlobalPosition.X + maxX, _frog.GlobalPosition.Y);
                }

                return result;
            }

            throw new InvalidOperationException("The collision shape of the area is not a valid SegmentShape2D.");
        }

        return false;
    }

    public double GetStopTimeWorking()
    {
        return _timeStopWorking;
    }



}
