using Godot;
using System;

public partial class LadderArea : Area2D
{

    [Signal]
    public delegate void LadderEnteredEventHandler();

    [Signal]
    public delegate void LadderExitedEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
      
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    public void OnBodyEntered(Node2D body)
    {
        if(body is Hero)
        {
            EmitSignal(SignalName.LadderEntered);
        }
    }

    public void OnBodyExited(Node2D body)
    {
        if(body is Hero)
        {
            EmitSignal(SignalName.LadderExited);
        }
    }
}
