using Godot;
using Godot.Collections;
using System;

public partial class GlobalSignals : Node2D
{
	[Signal]
	public delegate void ColorLostEventHandler(string colorName); //one color has been lost (painter)

	[Signal]
	public delegate void ColorBackEventHandler(string colorName); //one color is back (painter)

	[Signal]
	public delegate void NewAchievementCollecteddEventHandler(Dictionary achievement); //a new achievement has been unlocked

	[Signal]
	public delegate void AlarmStateChangedEventHandler(bool isAlarmOn); //the alarm state has changed

	[Signal]
	public delegate void FrogCollectedEventHandler(); //the frog has been collected

	[Signal]
	public delegate void MarketingMinigameSuccessEventHandler(); //the minigame was completed
	 
	[Signal]
	public delegate void UnlockGlassesEventHandler(); //signal when the glasses are unlocked

	[Signal]
	public delegate void GlassesChangeEventHandler(bool isWearningGlasses);
	
	[Signal]
	public delegate void GhostSlayedEventHandler();

	[Signal]
	public delegate void EndOfTheGameEventHandler();

	[Signal]
	public delegate void GameOverEventHandler();


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

	public void EmitAlertStateChanged(bool isAlarmOn)
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

	public void EmitUnlockGlasses()
	{
		EmitSignal(SignalName.UnlockGlasses);
	}

	public void EmitGlassesChange(bool isWearingGlasses)
	{
		EmitSignal(SignalName.GlassesChange,isWearingGlasses);
	}

	public void EmitGhostSlayed()
	{
		EmitSignal(nameof(GhostSlayed));
	}

	public void EmitEndOfTheGame()
	{
		EmitSignal(SignalName.EndOfTheGame);
	}

	public void EmitGameOver()
	{
		EmitSignal(SignalName.GameOver);
	}

	




}
