using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Employee : Node2D
{
    private bool _playerInRange = false;
    private RichTextLabel _dialogueLabel;
    private Hero _hero;

    private List<string> _chat = new List<string>();
    private List<string> _stopWorkingChat = new List<string>();
    private List<string> _backToWorkChat = new List<string>();
    private Random random = new Random();

    public Employee(List<string> chat, List<string> stopWorkingChat, List<string> backToWork)
    {
        _chat = chat;
        _stopWorkingChat = stopWorkingChat;
        _backToWorkChat = backToWork;
    }

    public enum EmployeeState
    {
        Working,
        NotWorking
    }

    [Export]
    public EmployeeState CurrentState { get; private set; } = EmployeeState.Working;

    [Signal]
    public delegate void EmployeeStateChangedEventHandler(int newState);


    public override void _Ready()
    {
        _dialogueLabel = GetNode<RichTextLabel>("RichTextLabel");
        _dialogueLabel.Visible = false;
        var area = GetNode<Area2D>("EmployeeArea");
        area.BodyEntered += OnBodyEntered;
        area.BodyExited += OnBodyExited;
    }

    public Area2D Area { get; private set; }

    public void OnBodyEntered(Node2D body)
    {

        if (body is Hero)
        {
            _playerInRange = true;
            _hero = (Hero)body;
            if(CurrentState == EmployeeState.Working)
            {
                ShowDialogue("Press E to talk.");
            }
            
        }
    }

    public void OnBodyExited(Node2D body)
    {
        if (body is Hero)
        {
            GD.Print("Au revoir...");
            _playerInRange = false;
            _hero = null;
            _dialogueLabel.Visible = false;
        }
    }

    public override void _Process(double delta)
    {

        if(_playerInRange && Input.IsActionJustPressed("interact_with_employees"))
        {
            Interact(_hero);
        }

        if (new Random().NextDouble() < 0.001)
        {
            if(CurrentState == EmployeeState.Working) {
                SetState(EmployeeState.NotWorking);
                _dialogueLabel.Visible = false;
            }
        }
    }

    public void SetState(EmployeeState newState)
    {
        if (CurrentState != newState)
        {
            CurrentState = newState;
            EmitSignal(SignalName.EmployeeStateChanged, (int)newState);
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
        GD.Print("L'employÃ© commence Ã  travailler.");
        CurrentState = EmployeeState.Working;
    }

    public virtual void StopWorking()
    {
        GD.Print("L'employÃ© arrÃªte de travailler.");
        CurrentState = EmployeeState.NotWorking;
    }

    public void ShowDialogue(string text)
    {
        _dialogueLabel.Text = text;
        _dialogueLabel.Visible = true;
    }

    public virtual void Interact(Hero hero)
    {
        GD.Print("Interaction avec l'employÃÂ©.");
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
        ShowDialogue(text);
        var timer = new Timer();
        timer.WaitTime = 3;
        timer.Name = "DialogTimer";
        timer.OneShot = true;
        timer.Timeout += OnTemporaryDialogTimeout;
        AddChild(timer);
        timer.Start();
        GD.Print("On démarre !");
    }

    private void OnTemporaryDialogTimeout()
    {
        GD.Print("c'est terminé");
        _dialogueLabel.Visible= false;
        var timer = GetNode<Timer>("DialogTimer");
        RemoveChild(timer);
        timer.QueueFree();
        
    }


}
