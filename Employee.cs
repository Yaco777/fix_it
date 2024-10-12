using Godot;
using System;

public partial class Employee : Node2D
{
    private bool _playerInRange = false;
    private RichTextLabel _dialogueLabel;

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
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is Hero)
        {
            _playerInRange = true;
            ShowDialogue("Press E to talk.");
        }
    }

    public void OnBodyExited(Node2D body)
    {
        if (body is Hero)
        {
            _playerInRange = false;
            _dialogueLabel.Visible = false;
        }
    }

    public override void _Process(double delta)
    {

        if (new Random().NextDouble() < 0.001)
        {
            if(CurrentState == EmployeeState.Working) {
                SetState(EmployeeState.NotWorking);
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

    private void ShowDialogue(string text)
    {
        _dialogueLabel.Text = text;
        _dialogueLabel.Visible = true;
    }

    public virtual void Interact()
    {
        GD.Print("Interaction avec l'employÃÂ©.");
    }


}
