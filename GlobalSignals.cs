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

    [Signal]
    public delegate void AlarmStateChangedEventHandler(bool isAlarmOn);

    [Signal]
    public delegate void FrogCollectedEventHandler();

    [Signal]
    public delegate void MarketingMinigameSuccessEventHandler();




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

    public void EmitAlartStateChanged(bool isAlarmOn)
    {
        EmitSignal(SignalName.AlarmStateChanged, isAlarmOn);
    }

    public void EmitFrogCollected()
    {
        EmitSignal(SignalName.FrogCollected);
    }

    public void EmitMarketingMinigameSuccess()
    {
        EmitSignal(SignalName.MarketingMinigameSuccess);
    }
    
}
