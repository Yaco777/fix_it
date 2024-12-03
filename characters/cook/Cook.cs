using Godot;
using System;
using System.Collections.Generic;

public partial class Cook : Employee
{
	private bool _miniGameSuccess;
	private GlobalSignals _globalSignals;
	private CanvasLayer _cookMinigame;
	private AnimatedSprite2D _marketingAnimation; //CHANGE


	private static List<string> _chatMessages = new List<string>
	{
		"Anyone can cook! ",
		"I’m going to make a seafood stew that will knock your socks off!"

	};

	private static List<string> _stopWorkingMessages = new List<string>
	{
		"No...it can't be...",
		"Again a weak aroma, it's a mystery"
	};

	private static List<string> _backToWork = new List<string>
	{
		"Ah! There is my cook book!",
		"Finally, salt and pepper!"
	};

	// Called when the node enters the scene tree for the first time.
	public Cook() : base(_chatMessages, _stopWorkingMessages, _backToWork, "Cook")
	{
	}

	public override void _Ready()
	{
		base._Ready();
		_globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
		_cookMinigame = GetNode<CanvasLayer>("CookMinigame");
		_marketingAnimation = GetNode<AnimatedSprite2D>("MarketingSprites"); //CHANGE
		_cookMinigame.Visible = false;
		_globalSignals.CookMinigameSuccess += MinigameSuccess;
		StartWorking();
		_marketingAnimation.Play();
	}

	public override void StartWorking()
	{

		_marketingAnimation.Animation = "working"; //CHANGE


	}

	private void MinigameSuccess()
	{
		_miniGameSuccess = true;
	}


	public override void StopWorking()
	{
		base.StopWorking();
		_marketingAnimation.Animation = "notWorking";
		GD.Print("stop working");
		_miniGameSuccess = false;

	}

	protected override void Interact(Hero hero)
	{

		if (CurrentState == EmployeeState.NotWorking && _miniGameSuccess == false)
		{
			_cookMinigame.Visible = true;
			Input.MouseMode = Input.MouseModeEnum.Visible;

		}
		else if (CurrentState == EmployeeState.NotWorking && _miniGameSuccess)
		{
			ShowBackToWorkChat();
			SetState(EmployeeState.Working);
		}
		else
		{
			base.Interact(hero);
		}
	}

	public bool IsCookMiniGameVisible()
	{
		return _cookMinigame.Visible;
	}
}
