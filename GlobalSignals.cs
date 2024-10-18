using Godot;
using Godot.Collections;
using System;

public partial class GlobalSignals : Node2D
{
    [Signal]
    public delegate void ColorLostEventHandler(string colorName);

    [Signal]
    public delegate void ColorBackEventHandler(string colorName);

    [Signal]
    public delegate void NewAchievementCollecteddEventHandler(Dictionary achievement);

    public void EmitColorLost(string colorName)
    {
        EmitSignal(SignalName.ColorLost, colorName);
    }

    public void EmitColorBack(string colorName)
    {
        EmitSignal(SignalName.ColorBack, colorName);
    }

    public void EmitNewAchievementCollected(Achievement achievement)
    {
        EmitSignal(SignalName.NewAchievementCollectedd,achievement.ToDictionary());
    }

    
}
