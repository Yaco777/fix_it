using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Employee : Node2D
{
    /**
     * Represent an employee of our game. ALl employee have a working state and can interact with the player
     */

    [Export]
    public double StopWorkProbability { get; set; } = 0.001;

    private bool _playerInRange = false; //boolean to check if the player is close to the employee
    private RichTextLabel _dialogueLabel; //dialogue label for interactions
    private Hero _hero;

    private List<string> _chat = new List<string>(); //messages shown when the player interact and the employee is working
    private List<string> _stopWorkingChat = new List<string>(); //same thing but when the employee isn't working
    private List<string> _backToWorkChat = new List<string>(); //when the employee go back to work
    private Random random = new Random();
    private bool currentTimerPresent = false; //we wants to only have 1 timer (to avoid having multiples messages at the same time)
    private int WAIT_TIME = 3; //the time the message will appear
    public string NameOfEmployee { get; private set; }

    public int NumberOfTimeWorked { get; private set; } = 0; //number of time this employee returned to work

    private AnimatedSprite2D _interactAnimation;


    [Export]
    public Vector2 SpawnPosition { get; set; } //spawn position of the employee in the building


    public Employee(List<string> chat, List<string> stopWorkingChat, List<string> backToWork, string nameOfEmployee)
    {
        _chat = chat;
        _stopWorkingChat = stopWorkingChat;
        _backToWorkChat = backToWork;
        NameOfEmployee = nameOfEmployee;
        
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
        _dialogueLabel = GetNode<RichTextLabel>("RichTextLabel");
        _dialogueLabel.BbcodeEnabled = true;
        _dialogueLabel.Visible = false;
        var area = GetNode<Area2D>("EmployeeArea");
        area.BodyEntered += OnBodyEntered;
        area.BodyExited += OnBodyExited;
        //GD.Print(Position+ "employeeNam : e"+NameOfEmployee);
        _interactAnimation = GetNode<AnimatedSprite2D>("InteractAnimation");
        _interactAnimation.ZIndex = 11;
        _interactAnimation.Visible = false; 


    }

    public void OnBodyEntered(Node2D body)
    {

        if (body is Hero)
        {
            _playerInRange = true; //we update the variable
            _hero = (Hero)body; //we keep the hero
            //ShowDialogue("[center][color=red] Press [b]E[/b] to interact [/color][/center]");
            _interactAnimation.Visible = true;
            _interactAnimation.Animation = "can_interact";
            _interactAnimation.Play();

        }
    }

    public void OnBodyExited(Node2D body)
    {
        if (body is Hero)
        {
            _playerInRange = false;
            _hero = null;
            _dialogueLabel.Visible = false; //the player won't be able to see the label if he is far
            _interactAnimation.Visible = false;

            //we remove the timer
            if (currentTimerPresent)
            {
                OnTemporaryDialogTimeout();
                currentTimerPresent = false;
            }
        }
    }

    public override void _Process(double delta)
    {

        

        if (_playerInRange && Input.IsActionJustPressed("interact_with_employees") && _hero.CooldownIsZero())
        {
            Interact(_hero); //methode redefined by all the employees
        }

        if (new Random().NextDouble() < StopWorkProbability)
        {

            if (CurrentState == EmployeeState.Working)
            {
                SetState(EmployeeState.NotWorking);
                _dialogueLabel.Visible = false;
            }
        }
    }



    public void SetState(EmployeeState newState)
    {
        /**
         * Switch the state to the other one and emit a signal
         */

        if (newState == EmployeeState.Working)
        {
            NumberOfTimeWorked++;
        }

        if (CurrentState != newState)
        {

            CurrentState = newState;
            EmitSignal(SignalName.EmployeeStateChanged, (int)newState, NameOfEmployee);
        }

        // Appliquer les actions selon l'Ã©tat
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

    public virtual void StartWorking()
    {
        //we update the numberOfTimeWorked before emitting the signal
        EmitSignal(SignalName.CheckAchievement, (int)CurrentState, NameOfEmployee);
        CurrentState = EmployeeState.Working;

    }

    public virtual void StopWorking()
    {
        EmitSignal(SignalName.CheckAchievement, (int)CurrentState, NameOfEmployee);
        CurrentState = EmployeeState.NotWorking;
    }

    public void ShowDialogue(string text)
    {
        /**
         * Show a dialogue that won't diseppear
         */
        _interactAnimation.Visible = false;
        _dialogueLabel.Text = text;
        if(_playerInRange)
        {
            _dialogueLabel.Visible = true;
        }
        
    }

    public virtual void Interact(Hero hero)
    {
        //method that need to be redefined by the employees, that's why there is the keyword "virtual"
        string message;
        if (EmployeeState.Working == CurrentState)
        {
            message = getRandomChat();
        }
        else
        {
            message = getRandomStopWorkingChat();
        }
        ShowTemporaryDialog(message);
    }

    public void ShowBackToWorkChat()
    {
        ShowTemporaryDialog(getRandomBackToWorkChat());
    }

    public string getRandomChat()
    {
        return _chat[this.random.Next(_chat.Count)];
    }

    public string getRandomStopWorkingChat()
    {
        return _stopWorkingChat[this.random.Next(_stopWorkingChat.Count)];
    }

    public string getRandomBackToWorkChat()
    {
        return _backToWorkChat[this.random.Next(_backToWorkChat.Count)];
    }

    public void ShowTemporaryDialog(string text)
    {
        //this method will display a message for a specific ammount of time (wait_time) by using a timer
        if (!currentTimerPresent)
        {
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

    }

    private void OnTemporaryDialogTimeout()
    {
        //remove the timer and update the currentTimePresent variable
        _dialogueLabel.Visible = false;
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

    public static PackedScene createEmployee(string name)
    {
        // Appliquer les actions selon l'Ã©tat
        PackedScene employee = name switch
        {
            "Painter" => GD.Load<PackedScene>("res://characters/painter/painter.tscn"),
            "Musicien" => GD.Load<PackedScene>("res://characters/musicien/musicien.tscn"),
            "Marketing" => GD.Load<PackedScene>("res://characters/marketing/marketing.tscn"),
            "Security" => GD.Load<PackedScene>("res://characters/security/security.tscn"),
            "Technicien" => GD.Load<PackedScene>("res://characters/technicien/technicien.tscn"),
            _ => throw new ArgumentException("The name of the employee : " + name + " is invalid, it's impossible to create the employee")
        };

        var scene = (Node2D) employee.Instantiate();
        return employee;
    }
}
