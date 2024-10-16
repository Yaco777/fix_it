using Godot;
using System;
using System.Collections.Generic;

public partial class Painter : Employee
{

	public static List<string> ColorsUnlocked {  get; set; } = new List<string>();
	public static List<string> CurrentColors { get; set; } = new List<string>();
	public static List<string> ColorsMissings { get; set; } = new List<string>();
	private GlobalSignals _globalSignals;

    private static List<string> REQUIRED_ITEMS = new List<string>
	{
    "Red brush",
    "Blue brush",
    "Green brush"
	};

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

    private static List<string> _oneColorUnlocked = new List<string>
    {
        "It's better than nothing",
        "One more...?"
    };



    public Painter() : base(_chatMessages, _stopWorkingMessages, _backToWork, "Painter")
	{
		ColorsUnlocked.Add("Green brush");

		ColorsMissings.Add("Green brush");

    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		_globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
		SetState(EmployeeState.NotWorking);

    }

	public override void StopWorking()
	{
		base.StopWorking();
		_globalSignals.EmitColorLost("Green brush");
	}




	public override void Interact(Hero hero)
	{
		var hasOneItem = false;
		foreach (var item in REQUIRED_ITEMS)
		{
			if (hero.HasItem(item))
			{
                hasOneItem = true;

                ColorsMissings.Remove(item);
				_globalSignals.EmitColorBack(item);
				
				hero.RemoveItem();
			

			}

		}

		
		if(hasOneItem && ColorsMissings.Count != 0) 
		{
            ShowNewColorUnlocked();
        }
		else if(hasOneItem && ColorsMissings.Count == 0) {
            ShowBackToWorkChat();
            SetState(EmployeeState.Working);
        }
		else
		{
			base.Interact(hero);
		}
        
    }

	private void ShowNewColorUnlocked()
	{
		base.ShowTemporaryDialog(GetRandomColorBackMessage());
	}

	private string GetRandomColorBackMessage()
	{
		var random = new Random();
		return _oneColorUnlocked[random.Next(_oneColorUnlocked.Count)];

    }
}
