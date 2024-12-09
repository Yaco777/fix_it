using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Cook : Employee
{
	private bool _miniGameSuccess;
	private GlobalSignals _globalSignals;
	private VBoxCookGame _cookMinigame;
	private AnimatedSprite2D _cookAnimation;
	private int _numberIngredientCollected;
	private UI _ui;
	private bool _firstTimeShowingMEssage = true;

	[Export]
	public string CookRoomUnlockMessage { get; set; } = "Ok look, the cook gave you a notebook! You will probably need it! You can open and close it by pressing N!";

		


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
		_cookMinigame = GetNode<VBoxCookGame>("CookMinigame");
		_cookAnimation = GetNode<AnimatedSprite2D>("CookSprites");
		_ui = GetNode<UI>("../../UI");
		_ui.ShowMessage(CookRoomUnlockMessage);

        _cookMinigame.Visible = false;
		_globalSignals.CookMinigameSuccess += MinigameSuccess;

		StartWorking();
		_cookAnimation.Play();
	}

	public String GetNextIngredient()
	{
		var ingredient = _cookMinigame.GetIngredientList()[_numberIngredientCollected];
		_numberIngredientCollected++;
		return ingredient;

	}

	public override void StartWorking()
	{
		base.StartWorking();
		_cookAnimation.Animation = "cooking";


	}

	private void MinigameSuccess()
	{

		_miniGameSuccess = true;
	}


	public override void StopWorking()
	{
		_numberIngredientCollected = 0;
		base.StopWorking();
		_cookAnimation.Animation = "sleeping";
		_miniGameSuccess = false;
		_cookMinigame.ResetAll();
		_cookMinigame.InitIngredient();
		var ingredient_list = _cookMinigame.GetIngredientList();
		foreach (var ingredient in ingredient_list)
		{
			var collectible = Collectible.CreateCollectible("Ingredient");
			collectible.GlobalPosition = Building.GetRandomPositionForItem();
			collectible.CollectibleName = "Ingredient";
			collectible.Name = "Ingredient";
			GetTree().Root.GetChild(0).AddChild(collectible);
		}

	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if(Input.IsActionJustPressed("open_notebook"))
		{
			if(_firstTimeShowingMEssage)
			{
				_firstTimeShowingMEssage = false;
				_ui.HideDIalog();
			}
		}
    }

    protected override void Interact(Hero hero)
	{

		
		if (CurrentState == EmployeeState.NotWorking && _miniGameSuccess == false)
		{
			_cookMinigame.Visible = true;

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
}
