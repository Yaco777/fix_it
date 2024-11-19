using Godot;

public partial class UI : CanvasLayer
{
	private TextureRect _objectIcon; //icon of the item in the inventory
	private RichTextLabel _objectName; //name of the item in the inventory


	private Vector2 targetScale = new Vector2(1f, 1f); //basic scale

	[Export]
	private float ScaleSpeed = 4.0f; // transition speed

	[Export]
	private float animationScaleX = 2f; 

	[Export]
	private float animationScaleY = 2f;

	[Export]
	private int NumberOfMinutesBeforeGameOver { get; set; } = 12;

	[Export]
	public int NumberOfGhostsToSlay { get; set; } = 1;

	private GlobalSignals _globalSignals;


	private ColorRect _dialogRect;

	private Label _dialogLabel;

	private Label _glassesWear;
	
	private Label _ghostCounterLabel;

	private ProgressSystem _progressSystem;

	private Timer _gameOverTimer;

	private Label _gameOverLabel;



    [Export]
	public string NotWearningGlasses { get; set; } = "Not wearing";

	[Export]
	public string WearingGlasses { get; set; } = "Wearing !";

	[Export]
	public string GlassesUnlockedMessage { get; set; } = "You unlocked the glasses! Press R to equip them";

	[Export]
	public string GhostMessage { get; set; } = "There are ghost everywhere! Press R again to remove the glasses. Try to fight against the ghosts!";

	[Export]
	public string EndGameMessage { get; set; } = "You did it, you fixed the game! Press E to end";

    [Export]
    public float FadeDuration = 5f; // Duration of the fade-out effect
    [Export]
    public float CameraZoomSpeed = 0.02f; // Speed at which the camera zooms out


	[Export]
	public float MaxCameraZoomOut = 0.5f;

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
		_objectName = GetNode<RichTextLabel>("ObjectName");
		_objectName.BbcodeEnabled = true;
		_objectIcon.Visible = false;
		_objectName.AddThemeFontSizeOverride("normal_font_size", 32);
		_globalSignals = GetNode<GlobalSignals>("../GlobalSignals");
		_camera = GetNode<Camera2D>("../Hero/Camera");
		_endGameRect = GetNode<ColorRect>("EndGameRect");
		_progressSystem = GetNode<ProgressSystem>("ProgressSystem");
		_gameOverTimer = GetNode<Timer>("GameOverTimer");
		_gameOverLabel = GetNode<Label>("TimerLabel");
        _endGameRect.Color = new Color(0,0, 0, 0);
		_gameOverTimer.WaitTime = NumberOfMinutesBeforeGameOver * 60; //We convert it in seconds
        _gameOverTimer.OneShot = true;
		_gameOverTimer.Start();



        UpdateTimerLabel();
        SetEmptyInventoryLabel();

		//the glasses
		_globalSignals.UnlockGlasses += GlassesUnlocked;
		_dialogRect = GetNode<ColorRect>("DialogRect");
		_dialogLabel = _dialogRect.GetNode<Label>("DialogLabel");
		_glassesWear = GetNode<Label>("GlassesWear");
		_glassesWear.Visible = false;
		_dialogRect.Visible = false;
		_globalSignals.GlassesChange += UpdateGlassesLabel;
		_ghostCounterLabel = GetNode<Label>("GhostCounterLabel");
		_ghostCounterLabel.Visible = false; 
		_globalSignals.GhostSlayed += OnGhostSlayed;


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

	public void SetEmptyInventoryLabel()
	{
		_objectName.Text = "";
	}

	public void UpdateCollectedItem(string itemName)
	{
		var texture = Collectible.getTextureOfCollectible(itemName);

		_objectIcon.Texture = texture;
		_objectIcon.Scale = new Vector2(animationScaleX, animationScaleY); //base scale of the item, the size will decrease
		_objectName.Text = "[center][color=yellow]" + itemName + "[/color][/center]";
		_objectIcon.Visible = true;
		_objectName.Visible = true;
	}

	public void ClearItem()
	{
		_objectIcon.Visible = false;
		_objectName.Visible = false;
	}

	private void GlassesUnlocked()
	{
		
		if(_state == State.GLASSES_NOT_UNLOCKED)
		{
			_state = State.SHOW_GLASSES_UNLOCKED_MESSAGE;
			_dialogRect.Visible = true;
			_glassesWear.Visible = true;
			_dialogLabel.Text = GlassesUnlockedMessage;
		}
		

	}

	private void UpdateGlassesLabel(bool isWearingGlasses)
	{
		if(isWearingGlasses)
		{
			_glassesWear.Text = WearingGlasses;
		}
		else
		{
			_glassesWear.Text = NotWearningGlasses;
		}
	}
	
	public void OnGhostSlayed()
	{
		_ghostsSlayed++;
		_ghostCounterLabel.Text = "Ghosts Slayed: " + _ghostsSlayed;
		_ghostCounterLabel.Visible = true; // Show the counter
        CheckEndGame();
	}

	private void CheckEndGame()
	{
		if(_ghostsSlayed == NumberOfGhostsToSlay)
		{
			_globalSignals.EmitEndOfTheGame();
			_state = State.END_GAME_MESSAGE;
            _dialogRect.Visible = true;
			_dialogLabel.Text = EndGameMessage;
			_objectIcon.Visible = false;
			_objectName.Visible = false;
			_glassesWear.Visible = false;
			_ghostCounterLabel.Visible = false;
			_progressSystem.Visible = false;
		

        }
	}

   
}
