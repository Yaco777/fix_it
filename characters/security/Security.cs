using Godot;
using System;
using System.Collections.Generic;

public partial class Security : Employee
{
    private AudioStreamPlayer _alertStreamPlayer;
    private Area2D _currentArea;
    private GlobalSignals _globalSignals;
    private Collectible _frog;
    private Vector2 _frogDirection;
    private AnimatedSprite2D _frogAnimation;

 

    [Export]
    public int FrogSpeed { get; set; } = 200;
    private bool _hasRemovedFrog;
    private AnimatedSprite2D _securityAnimation;
    private static List<string> _chatMessages = new List<string>
    {
        "I am your shield, fear nothing",
        "V",
        "E",
        "A"
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
        base._Ready();
        _alertStreamPlayer = GetNode<AudioStreamPlayer>("Alert");
        _globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
        _securityAnimation = GetNode<AnimatedSprite2D>("SecuritySprite");
        _globalSignals.FrogCollected += OnFrogCollected; 
        StartWorking();
        _securityAnimation.Animation = "working";
        _securityAnimation.Play();
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
        _globalSignals.EmitAlartStateChanged(false);
        _securityAnimation.Animation = "working";

    }

    public override void StopWorking()
    {
        base.StopWorking();
        _alertStreamPlayer.Play();
        _frog = Collectible.CreateCollectible("Frog");
        
        _frogAnimation.Visible = true;
        _currentArea = Building.getRandomArea2D();
        _frog.GlobalPosition = Building.getRandomPositionForItemForSpecificArea(_currentArea);
        _frogAnimation.GlobalPosition = _frog.GlobalPosition;
        GetTree().Root.GetChild(0).AddChild(_frog);
        _frog.HideSprite();
        _globalSignals.EmitAlartStateChanged(true);
        _securityAnimation.Animation = "idle";
    }


    public override void Interact(Hero hero)
    {
        //if the hero has removed the frog, the employee will go back to work
        if(_hasRemovedFrog)
        {
            _hasRemovedFrog = false;
            _frog = null;
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

        _frog.Position += _frogDirection * FrogSpeed * (float)delta;
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

                // check if the frog's X position is within the bounds
                return _frog.GlobalPosition.X >= minX && _frog.GlobalPosition.X <= maxX;
            }
            else
            {
                throw new InvalidOperationException("The collision shape of the area is not a valid SegmentShape2D.");
            }
        }

        return false;
    }



}
