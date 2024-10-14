using Godot;
using System;

public partial class GlobalSignals : Node2D
{
    [Signal]
    public delegate void ColorLostEventHandler(string colorName);

    public void EmitColorLost(String colorName)
    {
        GD.Print("Emission ici !!");
        EmitSignal(SignalName.ColorLost, colorName);
    }
}
