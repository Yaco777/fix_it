using Godot;
using System;

public partial class Room : Node2D
{

    [Export] public Texture2D ObjectTexture { get; set; }


    [Export] public int AmountStarsRequired { get; set; } = 5;

    [Export] public string NotEnoughStarsMessage { get; set; } = "Not enough stars!";

    private bool _playerInRange = false;

    private bool _hasUnlockedRoom = false;

    private AnimatedSprite2D _interactAnimation;

    private Area2D _interactionArea;

    private StaticBody2D _hitBoxArea;

    private Label _unlockLabel;

    private string _defaultLabel;

    private ProgressSystem _progressSystem;

    private AudioStreamPlayer2D _unlockPlayer;

    private enum State
    {
        CanInteract,
        ShowNumbersOfStars,
        NotEnoughStars,
        Unlocked
    }

    private State _state = State.CanInteract;


    public override void _Ready()
    {
        var sprite = GetNode<Sprite2D>("RoomSprite");
        sprite.Texture = ObjectTexture;
        _interactAnimation = GetNode<AnimatedSprite2D>("InteractAnimation");
        _unlockLabel = GetNode<Label>("CanvasLayer/UnlockLabel");
        _progressSystem = GetNode<ProgressSystem>("../../../UI/ProgressSystem");
        _unlockPlayer = GetNode<AudioStreamPlayer2D>("UnlockPlayer");
        _unlockLabel.Visible = false;
        _interactAnimation.Visible = false;
        _interactAnimation.Animation = "can_interact";
        _interactAnimation.Play();
        _interactionArea = GetNode<Area2D>("RoomUnlock");
        _defaultLabel = _unlockLabel.Text.Replace("{amount}",AmountStarsRequired.ToString()); //we store the default label text
        _unlockLabel.Text = _defaultLabel;
        _hitBoxArea = GetNode<StaticBody2D>("StaticBodyHitBox");
        _interactionArea.BodyEntered += ShowInteraction;
        _interactionArea.BodyExited += RemoveInteraction;
    }

    private void ShowInteraction(Node2D body)
    {
        if(body is Hero && _state != State.Unlocked)
        {
            if (_state != State.Unlocked)
            {
                _state = State.CanInteract;
                _unlockLabel.Text = _defaultLabel;
            }
            _playerInRange = true;
            _interactAnimation.Visible = true;

        }
    }

    private void RemoveInteraction(Node2D body)
    {
        if(body is Hero)
        {
            _playerInRange = false;
            _interactAnimation.Visible = false;
            _unlockLabel.Visible = false;
            if(_state != State.Unlocked)
            {
                _state = State.CanInteract;
            }
           
        }
    }

    private void FirstInteraction()
    {
        _state = State.ShowNumbersOfStars;
        _interactAnimation.Visible = false;
        _unlockLabel.Visible = true;
    }

    private void UnlockRoom()
    {
        _unlockLabel.Text = _defaultLabel;
        _hitBoxArea.QueueFree();
        _unlockPlayer.Play();
        _progressSystem.looseStars(AmountStarsRequired);
        _state = State.Unlocked;
        _interactAnimation.Visible = false;
        _unlockLabel.Visible = false;
        _state = State.Unlocked;
    }

    private void NotEnoughStars()
    {
        _unlockLabel.Text = NotEnoughStarsMessage;
        _unlockLabel.Visible = true;
    }

    public override void _Process(double delta)
    {
        if (_playerInRange && _state != State.Unlocked)
        {
            if (Input.IsActionJustPressed("interact_with_rooms"))
            {
                if (_state == State.CanInteract)
                {
                    FirstInteraction();
                    
                }
                else if (_state == State.ShowNumbersOfStars)
                {
                    if(_progressSystem.TotalStars >= AmountStarsRequired)
                    {
                        UnlockRoom();
                    }
                    else
                    {
                        NotEnoughStars();
                        
                    }
                }
            }
        }
    }
}
