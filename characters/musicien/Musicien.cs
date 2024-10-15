using Godot;
using System;
using System.Collections.Generic;

public partial class Musicien : Employee
{
	private AudioStreamPlayer _musicPlayer;
	private AudioStreamPlayer2D _snoringMusicPlayer;
	private AudioStreamPlayer2D _hornMusicPlayer;
	private static string REQUIRED_ITEM = "Horn";
	private AnimatedSprite2D _musicianAnimation;
	private Node2D _noteNode;
	private static List<string> _chatMessages = new List<string>
	
	
	{
		"You wants to hear my new song?",
		"Feel the music!",
		"I will never fall asleep, there is no way...",
		"Music soothes aches and pains!"
	};

	private static List<string> _stopWorkingMessages = new List<string>
	{
		"zzzzzzzzzz",
		"One more minute...?"
	};

	private static List<string> _backToWork = new List<string>
	{
		"I am backkk",
		"yeaaahh"
	};

	public Musicien() : base(_chatMessages,_stopWorkingMessages, _backToWork,"Musicien")
	{
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		base._Ready();
		_musicPlayer = GetNode<AudioStreamPlayer>("Music");
		_snoringMusicPlayer = GetNode<AudioStreamPlayer2D>("Snoring");
		_hornMusicPlayer = GetNode<AudioStreamPlayer2D>("Horn");
		_musicianAnimation = GetNode<AnimatedSprite2D>("MusicianSprites");
		_noteNode = GetNode<Node2D>("Notes");
		StartWorking();
		_musicianAnimation.Play();


	}

	public override void StartWorking()
	{
		//when the employee work, the music start
		_snoringMusicPlayer.Stop();
		_musicPlayer.Play();
		StartGeneratingNotes();
		_musicianAnimation.Animation = "playing";


	}

	public override void StopWorking()
	{
		//when the employee stop working, the music will stop and we will hear him snoring and a horn will be generated in the building
		_musicPlayer.Stop();
		_snoringMusicPlayer.Play();
		_musicianAnimation.Animation = "sleeping";
		
	}

	private void StartGeneratingNotes()
	{
		// coroutine that will generate the notes
		_ = GenerateNotes();
	}

	private async System.Threading.Tasks.Task GenerateNotes()
	{
	   var random = new Random();
		while (CurrentState == EmployeeState.Working) 
		{
			for (var i = 0; i < random.Next(3); i++) {
				CreateNote();
			}
			
			await ToSignal(GetTree().CreateTimer(1f - random.Next(1)), "timeout"); //we wait a little bit before creating the next notes
		}
	}

	private void CreateNote()
	{

		var note = (Notes) _noteNode.Duplicate(); 
		AddChild(note); // we add a new node to the tree

		
		note.GlobalPosition = GlobalPosition;  //the position won't be the exact same, we change the position a little bit in Notes


		// GÃ©nÃ©rer une direction alÃ©atoire
		Vector2 direction = new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1).Normalized();

		Random random = new Random();
		if(random.Next(2) == 0)
		{
			note.GetNode<Sprite2D>("Note1").Visible = true;
			note.GetNode<Sprite2D>("Note2").Visible = false;
			note.SetSprite(0);
		}
		else
		{
			note.GetNode<Sprite2D>("Note1").Visible = false;
			note.GetNode<Sprite2D>("Note2").Visible = true;
			note.SetSprite(1);
		}
	   

		// Passer la direction Ã  la note pour son mouvement
		note.SetDirection(direction); // Appeler une mÃ©thode pour dÃ©finir la direction
	}

	public override void Interact(Hero hero)
	{
		/**
		 * The interaction with the Musicien will display a message according to it's working state.
		 * If the Hero has the REQUIRED_ITEM, the musicien will work again
		 */
		if(EmployeeState.NotWorking == CurrentState && hero.HasItem(REQUIRED_ITEM))
		{
			ShowBackToWorkChat();
			hero.RemoveItem();
			SetState(EmployeeState.Working);
			_hornMusicPlayer.Play();
		   
		}
		else
		{
			base.Interact(hero);
		}
		
	}


}
