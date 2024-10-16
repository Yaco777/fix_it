using Godot;
using System;

public partial class GlobalSignals : Node2D
{
    [Signal]
    public delegate void ColorLostEventHandler(string colorName);

    [Signal]
    public delegate void ColorBackEventHandler(string colorName);

    public void EmitColorLost(string colorName)
    {
        GD.Print("Emission ici !!");
        EmitSignal(SignalName.ColorLost, colorName);
    }

    public void EmitColorBack(string colorName)
    {
        EmitSignal(SignalName.ColorBack, colorName);
    }

    
}
