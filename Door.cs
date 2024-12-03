using Godot;
using System;

public partial class Door : Node2D
{
    private enum State
    {
        CLOSED,
        OPEN
    }

    private State _state = State.CLOSED;

    private bool _playerInRange = false;

    private AnimatedSprite2D _doorSprite;

    private AnimatedSprite2D _interactionSprite;


    public override void _Ready()
    {
        base._Ready();
        var area = GetNode<Area2D>("DoorArea");
        area.BodyEntered += PlayerInRange;
        area.BodyExited += PlayerNotInRange;
        _interactionSprite = GetNode<AnimatedSprite2D>("InteractAnimation");
        _doorSprite = GetNode<AnimatedSprite2D>("DoorSprite");
        _interactionSprite.Visible = false;
       
        _doorSprite.Animation = "close";
       


    }

    public override void _Process(double delta)
    {
        if (_playerInRange && _state == State.OPEN && Input.IsActionJustPressed("message_interaction"))
        {
            GetTree().ChangeSceneToFile("res://main_menu.tscn");
        }
    }

    public void OpenDoor()
    {
        _state = State.OPEN;
        _doorSprite.Animation = "open";
    }

    private void PlayerNotInRange(Node2D body)
    {
        if(body is Hero)
        {
            _playerInRange = false;
            _interactionSprite.Visible = false;
        }
    }

    private void PlayerInRange(Node2D body)
    {
        if(body is Hero)
        {
            _playerInRange = true;
            _interactionSprite.Visible = true;
            _interactionSprite.Play();
            if(_state == State.CLOSED)
            {
                _interactionSprite.Animation = "cannot_interact";
            }
            else
            {
                _interactionSprite.Animation = "can_interact";
            }

        }
    }
}
