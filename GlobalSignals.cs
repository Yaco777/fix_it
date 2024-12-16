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
	public delegate void IngredientCollectedEventHandler(); //the ingredient has been collected

	[Signal]
	public delegate void MarketingMinigameSuccessEventHandler(); //the minigame was completed
	
	[Signal]
	public delegate void CookMinigameSuccessEventHandler(); //the minigame was completed
	 
	[Signal]
	public delegate void UnlockGlassesEventHandler(); //signal when the glasses are unlocked

	[Signal]
	public delegate void GlassesChangeEventHandler(bool isWearningGlasses);
	
	[Signal]
	public delegate void GhostSlayedEventHandler(string name);

	[Signal]
	public delegate void EndOfTheGameEventHandler();

	[Signal]
	public delegate void GameOverEventHandler();

	[Signal]
	public delegate void AllowPlayerMoveEventHandler();

    [Signal]
    public delegate void CookUnlockedEventHandler();

	[Signal]
	public delegate void NewWorkDoneEventHandler();

	[Signal]
	public delegate void BlockPlayerMovementEventHandler();

	[Signal]
	public delegate void ShowAchievementsEventHandler(bool shouldShow);

	[Signal]
	public delegate void ReverseAchievementsDisplayEventHandler(); //if the achievements are shown, they will be hidden

	[Signal]
	public delegate void StopAllInteractionsWhileCookminigameOpenEventHandler(bool shouldStop);


	[Signal]
	public delegate void AccountantStopWorkingEventHandler();

	public void EmitAccountantStopWorking()
	{
		EmitSignal(SignalName.AccountantStopWorking);
	}

    public void EmitReverseAchievementsDisplay()
	{
		EmitSignal(SignalName.ReverseAchievementsDisplay);
	}



    public void EmitStopAllInteractionsWhileCookminigameOpen(bool shouldStop)
	{
		EmitSignal(SignalName.StopAllInteractionsWhileCookminigameOpen, shouldStop);
	}


    public void EmitShowAchievements(bool shouldShow)
	{
		EmitSignal(SignalName.ShowAchievements, shouldShow);
	}

	public void EmitBlockPlayerMovement()
	{
		EmitSignal(SignalName.BlockPlayerMovement);
	}

	public void EmitNewWorkDone()
	{
		EmitSignal(SignalName.NewWorkDone);
	}

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

	public void EmitIngredientCollected()
	{
		EmitSignal(SignalName.IngredientCollected);
	}

	public void EmitMarketingMinigameSuccess()
	{
		EmitSignal(SignalName.MarketingMinigameSuccess);
	}
	
	public void EmitCookMinigameSuccess()
	{
		EmitSignal(SignalName.CookMinigameSuccess);
	}

	public void EmitUnlockGlasses()
	{
		EmitSignal(SignalName.UnlockGlasses);
	}

	public void EmitGlassesChange(bool isWearingGlasses)
	{
		EmitSignal(SignalName.GlassesChange,isWearingGlasses);
	}

	public void EmitGhostSlayed(string name)
	{
		GD.Print("ghost : " + name);
		EmitSignal(nameof(GhostSlayed), name);
	}

	public void EmitEndOfTheGame()
	{
		EmitSignal(SignalName.EndOfTheGame);
	}

	public void EmitGameOver()
	{
		EmitSignal(SignalName.GameOver);
	}

	public void EmitAllowPlayerMove()
	{
		EmitSignal(SignalName.AllowPlayerMove);
	}

	public void EmitCookUnlocked()
	{
		EmitSignal(SignalName.CookUnlocked);
	}







}
