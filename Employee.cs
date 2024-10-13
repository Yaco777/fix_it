using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Employee : Node2D
{
    /**
     * Represent an employee of our game. ALl employee have a working state and can interact with the player
     */

    private bool _playerInRange = false; //boolean to check if the player is close to the employee
    private RichTextLabel _dialogueLabel; //dialogue label for interactions
    private Hero _hero; 

    private List<string> _chat = new List<string>(); //messages shown when the player interact and the employee is working
    private List<string> _stopWorkingChat = new List<string>(); //same thing but when the employee isn't working
    private List<string> _backToWorkChat = new List<string>(); //when the employee go back to work
    private Random random = new Random();
    private bool currentTimerPresent = false; //we wants to only have 1 timer (to avoid having multiples messages at the same time)
    private static int WAIT_TIME = 3; //the time the message will appear
    private string _nameOfEmployee;


    public Employee(List<string> chat, List<string> stopWorkingChat, List<string> backToWork, string nameOfEmployee)
    {
        _chat = chat;
        _stopWorkingChat = stopWorkingChat;
        _backToWorkChat = backToWork;
        _nameOfEmployee = nameOfEmployee;
    }

    public enum EmployeeState
    {
        Working,
        NotWorking
    }

    [Export]
    public EmployeeState CurrentState { get; private set; } = EmployeeState.Working;

    [Signal]
    public delegate void EmployeeStateChangedEventHandler(int newState, string nameOfEmployee);


    public override void _Ready()
    {
        _dialogueLabel = GetNode<RichTextLabel>("RichTextLabel");
        _dialogueLabel.BbcodeEnabled = true;
        _dialogueLabel.Visible = false;
        var area = GetNode<Area2D>("EmployeeArea");
        area.BodyEntered += OnBodyEntered;
        area.BodyExited += OnBodyExited;
    }

    public void OnBodyEntered(Node2D body)
    {

        if (body is Hero)
        {
            _playerInRange = true; //we update the variable
            _hero = (Hero)body; //we keep the hero
            ShowDialogue("[center][color=red] Press [b]E[/b] to interact [/color][/center]");

        }
    }

    public void OnBodyExited(Node2D body)
    {
        if (body is Hero)
        {
            _playerInRange = false;
            _hero = null;
            _dialogueLabel.Visible = false; //the player won't be able to see the label if he is far


            //we remove the timer
            if(currentTimerPresent)
            {
                OnTemporaryDialogTimeout();
                currentTimerPresent = false;
            }
        }
    }

    public override void _Process(double delta)
    {

        if(_playerInRange && Input.IsActionJustPressed("interact_with_employees"))
        {
            Interact(_hero); //methode redefined by all the employees
        }

        if (new Random().NextDouble() < 0.0001)
        {
            if(CurrentState == EmployeeState.Working) {
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

        if (CurrentState != newState)
        {
            CurrentState = newState;
            EmitSignal(SignalName.EmployeeStateChanged, (int)newState, _nameOfEmployee);
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
        CurrentState = EmployeeState.Working;
    }

    public virtual void StopWorking()
    {
        CurrentState = EmployeeState.NotWorking;
    }

    public void ShowDialogue(string text)
    {
        /**
         * Show a dialogue that won't diseppear
         */

        _dialogueLabel.Text = text;
        _dialogueLabel.Visible = true;
    }

    public virtual void Interact(Hero hero)
    {
        GD.Print("Interaction ! (Employee)"); //method that need to be redefined by the employees, that's why there is the keyword "virtual"
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
        if(!currentTimerPresent)
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
        _dialogueLabel.Visible= false;
        var timer = GetNode<Timer>("DialogTimer");
        RemoveChild(timer);
        currentTimerPresent = false; //now we can interact again
        timer.QueueFree();
        
    }


}
