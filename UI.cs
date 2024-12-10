using Godot;
using System;
using System.Collections.Generic;

public partial class UI : CanvasLayer
{
	private TextureRect _objectIcon; //icon of the item in the inventory
	private Building _building;

	private Vector2 targetScale = new Vector2(1f, 1f); //basic scale

	[Export]
	private float ScaleSpeed = 4.0f; // transition speed

	[Export]
	private float animationScaleX = 2f; 

	[Export]
	private float animationScaleY = 2f;

	[Export]
	public int NumberOfMinutesBeforeGameOver { get; set; } = 12;

	[Export]
	public int NumberOfGhostsToSlay { get; set; } = 1;

	private ProgressBar _ghostProgressBar;

	private GlobalSignals _globalSignals;


	private TextureRect _dialogRect;

	private Label _dialogLabel;

	private AnimatedSprite2D _glassesWear;

	private ProgressSystem _progressSystem;

	public static Timer _gameOverTimer;

	private Label _gameOverLabel;

	private Sprite2D _notebookSprite;

	private Notebook _notebook;

	//the three black square
	private Sprite2D _emptyGlass;

	private Sprite2D _emptyNotebook;

	private Sprite2D _emptyItem;

	private bool _canInputLetters = true;





	[Export]
	public string NotWearningGlasses { get; set; } = "Not wearing";

	[Export]
	public string WearingGlasses { get; set; } = "Wearing !";

	[Export]
	public string GlassesUnlockedMessage { get; set; } = "There are ghosts everywhere! You need to put the glasses to see them by pressing R";

	[Export]
	public string GhostMessage { get; set; } = "Great! Press R again to remove the glasses. Try to fight against the ghosts!";

	[Export]
	public string EndGameMessage { get; set; } = "You did it, you fixed the game! Press E to end it";

	[Export]
	public float FadeDuration = 5f; // Duration of the fade-out effect
	[Export]
	public float CameraZoomSpeed = 0.02f; // Speed at which the camera zooms out


	[Export]
	public float MaxCameraZoomOut = 0.5f; //the camera will zoom out when we finish the game

	private int _ghostsSlayed = 0;

	private bool _isFading = false;

	private ColorRect _endGameRect;

	private Camera2D _camera;

	private enum State //this state is used to know if we need to show the message asking the player to wear the glasses
	{
		GLASSES_NOT_UNLOCKED,
		SHOW_GLASSES_UNLOCKED_MESSAGE,
		GHOST_MESSAGE,
		HIDE_GLASSES_UNLOCKED_MESSAGE,
		END_GAME_MESSAGE,
		FADING_OUT

	}

	private State _state = State.GLASSES_NOT_UNLOCKED;


	public override void _Ready()
	{
		_objectIcon = GetNode<TextureRect>("ObjectIcon");
		_notebookSprite = GetNode<Sprite2D>("NotebookSprite");
		_notebook = GetNode<Notebook>("Notebook");
		_emptyGlass = GetNode<Sprite2D>("GlassIconEmpty");
		_emptyNotebook = GetNode<Sprite2D>("NotebookEmpty");
		_emptyItem = GetNode<Sprite2D>("ItemEmpty");
		_notebook.Visible = false;
		_notebookSprite.Visible = false;
		_objectIcon.Visible = false;
		_globalSignals = GetNode<GlobalSignals>("../GlobalSignals");
		_camera = GetNode<Camera2D>("../Hero/Camera");
		_endGameRect = GetNode<ColorRect>("EndGameRect");
		_progressSystem = GetNode<ProgressSystem>("ProgressSystem");
		_gameOverTimer = GetNode<Timer>("GameOverTimer");
		_ghostProgressBar = GetNode<ProgressBar>("GhostProgressBar");
        
        _gameOverLabel = GetNode<Label>("TimeLeft/TimerLabel");
		_globalSignals.StopAllInteractionsWhileCookminigameOpen += ChangeCanInputLetters;
		_endGameRect.Color = new Color(0,0, 0, 0);
		_gameOverTimer.WaitTime = NumberOfMinutesBeforeGameOver * 60; //We convert it in seconds
		_gameOverTimer.OneShot = true;
		_ghostProgressBar.Visible = false;
		_ghostProgressBar.Value = 0;
        _gameOverTimer.Start();

		_globalSignals.CookUnlocked += ShowNotebook;
		UpdateTimerLabel();
		//the glasses
		_globalSignals.UnlockGlasses += GlassesUnlocked;
		_dialogRect = GetNode<TextureRect>("DialogRect");
		_dialogLabel = _dialogRect.GetNode<Label>("DialogLabel");
		_glassesWear = GetNode<AnimatedSprite2D>("GlassesWear");
		_glassesWear.Visible = false;
		_dialogRect.Visible = false;
		_globalSignals.GlassesChange += UpdateGlassesLabel;
		_globalSignals.GhostSlayed += OnGhostSlayed;


	}

	private void ChangeCanInputLetters(bool shouldStop)
	{
		_canInputLetters = shouldStop;
	}



	//function called when we unlock the cook
	private void ShowNotebook()
	{
		_notebookSprite.Visible = true;
		_emptyNotebook.Visible = false;
	}


	private void UpdateTimerLabel()
	{
		/**
		 * Update the timer label according to the remaining time
		 */
		if (_gameOverTimer.TimeLeft > 0)
		{
			int minutes = Mathf.FloorToInt((float)_gameOverTimer.TimeLeft / 60);
			int seconds = Mathf.FloorToInt((float)_gameOverTimer.TimeLeft % 60);
			_gameOverLabel.Text = $"Time Left: {minutes:00}:{seconds:00}";
		}
		else 
		{
			_gameOverTimer.WaitTime = double.MaxValue;
			_globalSignals.EmitGameOver();
			GetTree().ChangeSceneToFile("res://game_over_screen.tscn");

		}
	}

	public override void _Process(double delta)
	{
		UpdateTimerLabel();
		//we change the scale of the item
		_objectIcon.Scale = _objectIcon.Scale.Lerp(targetScale, (float)(ScaleSpeed * delta));

		//we update the message that will be displayed if the player has unlocked the glasses
		if(_state != State.GLASSES_NOT_UNLOCKED)
		{
			if(Input.IsActionJustPressed("wear_glasses"))
			{
				
				if(_state == State.GHOST_MESSAGE)
				{
					_state = State.HIDE_GLASSES_UNLOCKED_MESSAGE;
					_dialogRect.Visible = false;
				   
				}
				else if (_state == State.SHOW_GLASSES_UNLOCKED_MESSAGE)
				{
					_state = State.GHOST_MESSAGE;
					_dialogLabel.Text = GhostMessage;

				}

			}
			else if(Input.IsActionJustPressed("message_interaction"))
			{
				if(_state == State.END_GAME_MESSAGE)
				{
					_state = State.FADING_OUT;
					_dialogRect.Visible = false;
					_isFading = true;
					_endGameRect.Visible = true;
					PlayEndingSound();
					_gameOverLabel.Visible = false;
					_gameOverTimer.WaitTime = 9999;
					
				  
				 
				}
			}
			
		}
		if (_isFading)
		{
			// Fade the screen out by increasing the alpha of the endGameRect
			float newAlpha = _endGameRect.Color.A + (float)(delta / FadeDuration);
			
			_endGameRect.Color = new Color(0, 0, 0, Mathf.Clamp(newAlpha, 0, 1));

			// Zoom out the camera by decreasing the zoom value
			if (_camera.Zoom.Length() > MaxCameraZoomOut) 
			{
				_camera.Zoom -= new Vector2(CameraZoomSpeed * (float)delta, CameraZoomSpeed * (float)delta);
			}

			// When the fade-out is complete, trigger the game end logic
			if (_endGameRect.Color.A >= 1.0f)
			{
				// Trigger game over, disable the fading
				_isFading = false;
				var endGameScene = GD.Load<PackedScene>("res://end_game_credits.tscn");
				var endGame = (EndGameCredits)endGameScene.Instantiate();
				GetParent().AddChild(endGame);
				Visible = false;
				
			}
		}

		if(_notebookSprite.Visible && Input.IsActionJustPressed("open_notebook") && _canInputLetters) {
			_notebook.Visible = !_notebook.Visible;

		}

		if(Input.IsActionJustPressed("show_achievements") && _canInputLetters)
		{
			_globalSignals.EmitReverseAchievementsDisplay();
		}

	}

	public void CloseNotebookAndAchievements()
	{
		_notebook.Visible = false;
		_globalSignals.EmitShowAchievements(false);

	}

	private void  PlayEndingSound()
	{
		//disable all sound except the ending sound
		AudioServer.SetBusVolumeDb(2, -80);

		AudioStreamPlayer soundPlayer = new AudioStreamPlayer();
		soundPlayer.Bus = "EndGameSound";
		AddChild(soundPlayer);  

	  
		AudioStream audioStream = (AudioStream)ResourceLoader.Load("res://audio/end_of_the_game.mp3");

		
		soundPlayer.Stream = audioStream;
	   // AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("EndGameSound"), 0);

		soundPlayer.Play();
	}

	

	public void UpdateCollectedItem(string itemName)
	{
		var texture = Collectible.getTextureOfCollectible(itemName);
		_objectIcon.Texture = texture;
		_objectIcon.Scale = new Vector2(animationScaleX, animationScaleY); //base scale of the item, the size will decrease
		_objectIcon.Visible = true;
	}

	public void ClearItem()
	{
		_objectIcon.Visible = false;

	}

	private void GlassesUnlocked()
	{
		
		if(_state == State.GLASSES_NOT_UNLOCKED)
		{
			_state = State.SHOW_GLASSES_UNLOCKED_MESSAGE;
			_dialogRect.Visible = true;
			_glassesWear.Visible = true;
			_glassesWear.Animation = "not_wearing";
			_emptyGlass.Visible = false;
			_dialogLabel.Text = GlassesUnlockedMessage;
		}
		_ghostProgressBar.Visible = true;



    }

	private void UpdateGlassesLabel(bool isWearingGlasses)
	{
		if(isWearingGlasses)
		{
			_glassesWear.Animation = "wearing";
		}
		else
		{
			_glassesWear.Animation = "not_wearing";
		}
	}
	
	public void OnGhostSlayed(string name)
	{

		_ghostsSlayed++;
		_ghostProgressBar.Visible = true;
        _ghostProgressBar.Value = _ghostsSlayed;
		CheckEndGame();
	}

	private void CheckEndGame()
	{
		if(_ghostsSlayed == 1)
		{
			_globalSignals.EmitEndOfTheGame();
			_state = State.END_GAME_MESSAGE;
			_dialogRect.Visible = true;
			_dialogLabel.Text = EndGameMessage;
			_objectIcon.Visible = false;
			_glassesWear.Visible = false;
			_ghostProgressBar.Visible = false;
			_progressSystem.Visible = false;
		

		}
	}

	public void ShowMessage(string message)
	{
		_dialogRect.Visible = true;
		_dialogLabel.Text = message;
	}

	public void HideDIalog()
	{
		_dialogRect.Visible = false;
	}

   
}
