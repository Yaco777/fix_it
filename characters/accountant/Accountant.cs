using System.Collections.Generic;
using Godot;

public partial class Accountant : Employee
{

	private bool _miniGameSuccess;
	private GlobalSignals _globalSignals;
	private CanvasLayer _marketingMinigame;
	private AnimatedSprite2D _marketingAnimation;
	private static List<string> _chatMessages = new List<string>
	{
		"Shhh... I'm working on something",
		"1+1=2",
		"According to the pythagorean theorem...",
		"Math is the key to the universe"
	};

	private static List<string> _stopWorkingMessages = new List<string>
	{
		"I am lost...",
		"Could you help me?"
	};

	private static List<string> _backToWork = new List<string>
	{
		"I don't know why my calculator stopped working...",
		"Something is wrong with my calculations."
	};

	public Accountant() : base(_chatMessages, _stopWorkingMessages, _backToWork,"Accountant")
	{
	}

	public override void _Ready()
	{
		base._Ready();
		_globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
		_marketingMinigame = GetNode<CanvasLayer>("MarketingMinigame");
		_marketingAnimation = GetNode<AnimatedSprite2D>("MarketingSprites");
		_marketingMinigame.Visible = false;
		_globalSignals.MarketingMinigameSuccess += MinigameSuccess;
		StartWorking();
		_marketingAnimation.Play();
	}

	public override void StartWorking()
	{

		_marketingAnimation.Animation = "working";

		
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
			_marketingMinigame.Visible = true;
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

	public bool IsMarketingMiniGameVisible()
	{
		return _marketingMinigame.Visible;
	}




}
