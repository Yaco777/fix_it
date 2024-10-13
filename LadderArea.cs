using Godot;
using System;

public partial class LadderArea : Area2D
{
    /*
     * Represent the area used to detect the collision with the player
     */

    [Signal]
    public delegate void LadderEnteredEventHandler(); //signal emited when the player enter the area2D

    [Signal]
    public delegate void LadderExitedEventHandler(); //signal emited when the player leave the area2D

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
        EmitSignal(SignalName.LadderExited);
    }
}
