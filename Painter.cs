using Godot;
using System;
using System.Collections.Generic;

public partial class Painter : Employee
{

	private List<string> _colorsUnlocked = new List<string>();
	private GlobalSignals _globalSignals;

	private static List<string> _chatMessages = new List<string>

	{
		"In a single brushstroke!",
		"I'm so clumsy",
		"I will never fall asleep, there is no way...",
		"Music soothes aches and pains!"
	};

	private static List<string> _stopWorkingMessages = new List<string>
	{
		"Where is it ? Here ? No...",
		"AHHHHHHHHH"
	};

	private static List<string> _backToWork = new List<string>
	{
		"Me? Lose something? Never",
		"My inspiration is back!"
	};

	public Painter() : base(_chatMessages, _stopWorkingMessages, _backToWork, "Painter")
	{
		_colorsUnlocked.Add("Red");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		_globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
		StopWorking();
	}

	public override void StopWorking()
	{
		base.StopWorking();
		GD.Print("envoi du signal...");
		_globalSignals.EmitColorLost("Red");
	}



	public override void Interact(Hero hero)
	{
		base.Interact(hero);
	}
}
