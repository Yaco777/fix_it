using System;
using Godot;

public partial class Room : Node2D
{
	/**
	 * Represent a room with two area : one used to detect the interaction and the other one to prevent the player to enter the room (if the room isn't unlocked)
	 */

	[Export] public Texture2D ObjectTexture { get; set; }


	public static int AmountStarsRequired { get; set; } = 1;

	[Export] public string NotEnoughStarsMessage { get; set; } = "Not enough stars!";

	[Export] public string EmployeeUnlockedName {  get; set; }

	private bool _playerInRange;

	public bool _hasUnlockedRoom; // need to be public for the tutorial

	private AnimatedSprite2D _interactAnimation;

	private Area2D _interactionArea; //interaction area where the player can press E

	private StaticBody2D _hitBoxArea; //area where the player cannot enter. The area will be removed when the player will unlock the room

	private Label _unlockLabel;

	private string _defaultLabel; //string showing the number of stars required to unlock the room

	private ProgressSystem _progressSystem;

	private AudioStreamPlayer2D _unlockPlayer; //sound played when the player unlock the room

	private Sprite2D _roomSprite;

	private Building _building;

	private CanvasLayer _canvasLayer;

	[Export]
	public int YMargin { get; set; } = 175; //margin used to move the employee down

	private GlobalSignals _globalSignals;

	[Export]

	public int RoomPriceIncrease { get; set; } = 0; //default = 2

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
		_roomSprite = GetNode<Sprite2D>("RoomSprite");
		_roomSprite.Texture = ObjectTexture;
		_interactAnimation = GetNode<AnimatedSprite2D>("InteractAnimation");
		_unlockLabel = GetNode<Label>("CanvasLayer/UnlockLabel");
		_progressSystem = GetNode<ProgressSystem>("../../../UI/ProgressSystem");
		_unlockPlayer = GetNode<AudioStreamPlayer2D>("UnlockPlayer");
		_canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
		_globalSignals = GetNode<GlobalSignals>("../../../GlobalSignals");
		_building = (Building)GetParent().GetParent();
		_unlockLabel.Visible = false;
		_interactAnimation.Visible = false;
		_canvasLayer.Visible = false;
		_interactAnimation.Animation = "can_interact";
		_interactAnimation.Play();
		_interactionArea = GetNode<Area2D>("RoomUnlock");
		if(EmployeeUnlockedName == "Musicien")
		{
			AmountStarsRequired = 0;

        }

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
			//if the room isn't unlocked, the plaer will now be able to interact and we show the label
			if (_state != State.Unlocked)
			{
				_state = State.CanInteract;
				_defaultLabel = "Do you want to unlock this room? You need at least " + AmountStarsRequired.ToString() + " stars";
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
			//if the player isn't in the interaction area, we hide everything
			_playerInRange = false;
			_interactAnimation.Visible = false;
			_unlockLabel.Visible = false;
			_canvasLayer.Visible = false;

			if (_state != State.Unlocked)
			{
				_state = State.CanInteract;
			}
		   
		}
	}

	private void FirstInteraction()
	{
		//show the interaction animation
		_state = State.ShowNumbersOfStars;
		_interactAnimation.Visible = false;
		_unlockLabel.Visible = true;
		_canvasLayer.Visible = true;
	}

	private void UnlockRoom()
	{
		//when the room will be unlocked, we will hide everything and we remove the hitbox area
		_unlockLabel.Text = _defaultLabel;
		_roomSprite.Material = null;
		_hitBoxArea.QueueFree();
		_unlockPlayer.Play();
		_state = State.Unlocked;
		_interactAnimation.Visible = false;
		_unlockLabel.Visible = false;
		_state = State.Unlocked;
		_hasUnlockedRoom = true;
		_canvasLayer.Visible = false;
		if(EmployeeUnlockedName == "Cook")
		{
			_globalSignals.EmitCookUnlocked();
		}
		AddNewEmployee();
		AmountStarsRequired += RoomPriceIncrease;
        _defaultLabel = _unlockLabel.Text.Replace("{amount}", AmountStarsRequired.ToString());


    }

	private void AddNewEmployee()
	{
		/*
		 * Create an employee and add it at the center of the room
		 */
		var employee = EmployeeUnlockedName switch
		{
			"Musicien" => GD.Load<PackedScene>("res://characters/musicien/musicien.tscn"),
			"Technician" => GD.Load<PackedScene>("res://characters/technician/technician.tscn"),
			"Painter" => GD.Load<PackedScene>("res://characters/painter/painter.tscn"),
			"Accountant" => GD.Load<PackedScene>("res://characters/accountant/accountant.tscn"),
			"Security" => GD.Load<PackedScene>("res://characters/security/security.tscn"),
			"Cook" => GD.Load<PackedScene>("res://characters/cook/cook.tscn"),
			_ => throw new ArgumentException("The room need to add the employee " + EmployeeUnlockedName + " but it's not possible")
		};
		
		
		var instance = (Employee) employee.Instantiate();
		instance.InitializeEmployee();
		instance.GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y + YMargin); //we adjust the position with the YMargin
		instance.Name = EmployeeUnlockedName;
		instance.ZIndex = 2;
		GetParent().GetParent().GetParent().GetNode<Node2D>("Employees").AddChild(instance);
		_building.AddEmployeeToCheckForStateChange(instance); //when the room create an employee, the building should connect the StateChange signal
		_progressSystem.AddAchievementsForOneEmployee(instance);

	}

	private void NotEnoughStars(int required)
	{
		//if the player doesn't have enough stars, we will print a message
		_unlockLabel.Text = "You need "+required+" more stars to unlock the room";
		_unlockLabel.Visible = true;
	}

	public override void _Process(double delta)
	{
		if (_playerInRange && _state != State.Unlocked)
		{
			if (Input.IsActionJustPressed("interact_with_rooms")) //when the player interact
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
						NotEnoughStars(AmountStarsRequired - _progressSystem.TotalStars);
						
					}
				}
			}
		}
	}
}
