using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class Employee : Node2D
{
	/**
	 * Represent an employee of our game. ALl employee have a working state and can interact with the player
	 */

	[Export]
	public double StopWorkProbability { get; set; } = 0.0001;

	private bool _playerInRange; //boolean to check if the player is close to the employee
	private Label _dialogueLabel; //dialogue label for interactions
	private TextureRect _textureRect; //the background behind the text
	private Hero _hero;
	private GlobalSignals _globalSignals;
	private bool _canInteract = true;

	private List<string> _chat; //messages shown when the player interact and the employee is working
	private List<string> _stopWorkingChat; //same thing but when the employee isn't working
	private List<string> _backToWorkChat; //when the employee go back to work
	private Random random = new Random();
	private bool currentTimerPresent; //we want to only have 1 timer (to avoid having multiples messages at the same time)
	private int WAIT_TIME = 3; //the time the message will appear
	public string NameOfEmployee { get; private set; }

	public int NumberOfTimeWorked { get; private set; } //number of time this employee returned to work


	private double _maximumTimeBeforeStopWorking;

	private double _increaseTimeBeforeStopWorking; //at the end of each work, we will increase the maximum time before stop working by this value

	private double _actualTimeBeforeStopWorking;

	private AnimatedSprite2D _interactAnimation;

	private bool _cannotStopWorking = false;

	private int _debug = 0;

	public Employee(List<string> chat, List<string> stopWorkingChat, List<string> backToWork, string nameOfEmployee)
	{
		_chat = chat;
		_stopWorkingChat = stopWorkingChat;
		_backToWorkChat = backToWork;
		NameOfEmployee = nameOfEmployee;
		_increaseTimeBeforeStopWorking = 30;
		_maximumTimeBeforeStopWorking = 30;


    }

	public enum EmployeeState
	{
		Working,
		NotWorking
	}

	[Export]
	public EmployeeState CurrentState { get; set; } = EmployeeState.Working;

	[Signal]
	public delegate void EmployeeStateChangedEventHandler(int newState, string nameOfEmployee);

	[Signal]
	public delegate void CheckAchievementEventHandler(int newState, string nameOfEmployee);


	public override void _Ready()
	{
        _textureRect = GetNode<TextureRect>("TextureRect");
		_dialogueLabel = GetNode<Label>("TextureRect/Label");
		_globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
        _textureRect.Visible = false;
		var area = GetNode<Area2D>("EmployeeArea");
		area.BodyEntered += OnBodyEntered;
		area.BodyExited += OnBodyExited;
		_interactAnimation = GetNode<AnimatedSprite2D>("InteractAnimation");
		_interactAnimation.ZIndex = 11;
		_interactAnimation.Visible = false;
		_globalSignals.EndOfTheGame += StopInteractions;
		_globalSignals.GhostSlayed += CheckShouldNotBeAllowedToStopWorking;
		_actualTimeBeforeStopWorking = _maximumTimeBeforeStopWorking;

		_debug = 30;

    }

	private void CheckShouldNotBeAllowedToStopWorking(string name)
	{
		if(name.Contains(NameOfEmployee))
		{
			_cannotStopWorking = true;

		}
	

	}

	private void StopInteractions()
	{
		_canInteract = false;
		_interactAnimation.Visible = false;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is not Hero) return;
		_playerInRange = true; //we update the variable
		_hero = (Hero)body; //we keep the hero
		//ShowDialogue("[center][color=red] Press [b]E[/b] to interact [/color][/center]");
		_interactAnimation.Visible = true;
		_interactAnimation.Animation = "can_interact";
		_canInteract = true;
		_interactAnimation.Play();
	}

	private void OnBodyExited(Node2D body)
	{
		if (body is not Hero) return;
		_playerInRange = false;
		_hero = null;
		_textureRect.Visible = false; //the player won't be able to see the label if he is far
		_interactAnimation.Visible = false;

		//we remove the timer
		if (!currentTimerPresent) return;
		OnTemporaryDialogTimeout();
		currentTimerPresent = false;
	}

	public override void _Process(double delta)
	{


		if(!_canInteract) { return; };

		if (_playerInRange && Input.IsActionJustPressed("interact_with_employees") && _hero.CooldownIsZero())
		{
			Interact(_hero); //methode redefined by all the employees
		}
		if((int)_actualTimeBeforeStopWorking != _debug)
		{
			_debug = (int)_actualTimeBeforeStopWorking;
            GD.Print(NameOfEmployee + " " + _actualTimeBeforeStopWorking);

        }

		if (CurrentState == EmployeeState.Working && !_cannotStopWorking)
		{
			
			_actualTimeBeforeStopWorking -= delta;
			if(_actualTimeBeforeStopWorking <= 0)
			{
				
				SetState(EmployeeState.NotWorking);
				_textureRect.Visible = false;
                
				return;
			}

		}

		/*if (!(new Random().NextDouble() < StopWorkProbability)) return;
		if (CurrentState != EmployeeState.Working) return;
		SetState(EmployeeState.NotWorking);
		_colorRect.Visible = false;*/
	}



	protected void SetState(EmployeeState newState)
	{
        /**
		 * Switch the state to the other one and emit a signal
		 */
        if (CurrentState == newState) return;

        if (newState == EmployeeState.Working)
		{
			NumberOfTimeWorked++;
		}

		if (CurrentState != newState)
		{

			CurrentState = newState;
			EmitSignal(SignalName.EmployeeStateChanged, (int)newState, NameOfEmployee);
		}
		
		switch (newState)
		{
			case EmployeeState.Working:
				StartWorking();
				break;

			case EmployeeState.NotWorking:
				StopWorking();
				break;
		}
	}

	public void CheckAchievements(StringName signal)
	{
		EmitSignal(signal, (int)CurrentState, NameOfEmployee);
	}

	public virtual void StartWorking()
	{
        _actualTimeBeforeStopWorking = _maximumTimeBeforeStopWorking;
        _maximumTimeBeforeStopWorking += _increaseTimeBeforeStopWorking;
        
		GD.Print("on commence avec : " + _maximumTimeBeforeStopWorking + " pour " + NameOfEmployee); ;
        GD.Print($"Employee: {NameOfEmployee}, MaxTime: {_maximumTimeBeforeStopWorking}, Address: {GetHashCode()}, CurrentTime : {_actualTimeBeforeStopWorking}");

        //we update the numberOfTimeWorked before emitting the signal
        EmitSignal(SignalName.CheckAchievement, (int)CurrentState, NameOfEmployee);
		CurrentState = EmployeeState.Working;

	}

	public virtual void StopWorking()
	{
		EmitSignal(SignalName.CheckAchievement, (int)CurrentState, NameOfEmployee);
		CurrentState = EmployeeState.NotWorking;
	}

	private void ShowDialogue(string text)
	{
		/**
		 * Show a dialogue that won't diseppear
		 */
		_interactAnimation.Visible = false;
		_dialogueLabel.Text = text;
		if(_playerInRange)
		{
            _textureRect.Visible = true;
		}
		
	}

	protected virtual void Interact(Hero hero)
	{
		if (!_canInteract) { return; };


		//method that need to be redefined by the employees, that's why there is the keyword "virtual"
		string message;
		if (EmployeeState.Working == CurrentState)
		{
			message = GetRandomChat();
		}
		else
		{
			message = GetRandomStopWorkingChat();
		}
		ShowTemporaryDialog(message);
	}

	protected void ShowBackToWorkChat()
	{
		ShowTemporaryDialog(GetRandomBackToWorkChat());
	}

	private string GetRandomChat()
	{
		return _chat[random.Next(_chat.Count)];
	}

	private string GetRandomStopWorkingChat()
	{
		return _stopWorkingChat[random.Next(_stopWorkingChat.Count)];
	}

	private string GetRandomBackToWorkChat()
	{
		return _backToWorkChat[random.Next(_backToWorkChat.Count)];
	}

	protected void ShowTemporaryDialog(string text)
	{
		//this method will display a message for a specific amount of time (wait_time) by using a timer
		if (currentTimerPresent) return;
		ShowDialogue(text);
		var timer = new Timer();
		timer.WaitTime = WAIT_TIME;
		timer.Name = "DialogTimer";
		timer.OneShot = true;
		timer.Timeout += OnTemporaryDialogTimeout;
		AddChild(timer);
		timer.Start();
		currentTimerPresent = true; //we use this variable to avoid having multiple timers

	}

	private void OnTemporaryDialogTimeout()
	{
        //remove the timer and update the currentTimePresent variable
        _textureRect.Visible = false;
		var timer = GetNode<Timer>("DialogTimer");
		CallDeferred(nameof(RemoveChildDeferred), timer);
		currentTimerPresent = false; //now we can interact again
		timer.QueueFree();
		if(_playerInRange)
		{
			_interactAnimation.Visible = true;
		}
		

	}

	private void RemoveChildDeferred(Node node)
	{
		RemoveChild(node);
	}

	public static PackedScene CreateEmployee(string name)
	{
		PackedScene employee = name switch
		{
			"Painter" => GD.Load<PackedScene>("res://characters/painter/painter.tscn"),
			"Musicien" => GD.Load<PackedScene>("res://characters/musicien/musicien.tscn"),
			"Marketing" => GD.Load<PackedScene>("res://characters/marketing/marketing.tscn"),
			"Security" => GD.Load<PackedScene>("res://characters/security/security.tscn"),
			"Technicien" => GD.Load<PackedScene>("res://characters/technicien/technicien.tscn"),
			"Cook" => GD.Load<PackedScene>("res://characters/cook/cook.tscn"),
			_ => throw new ArgumentException("The name of the employee : " + name + " is invalid, it's impossible to create the employee")
		};
		return employee;
	}

	public void InitializeEmployee()
	{
		NumberOfTimeWorked = 0;
        _maximumTimeBeforeStopWorking = 30;
		_actualTimeBeforeStopWorking = 30;



	}
}
