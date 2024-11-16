using Godot;
using System;

public partial class Ghost : Node2D
{
	private GlobalSignals _globalSignals;
	private Area2D _ghostArea;
	private bool _inRange;
	private ColorRect _interactionRect;
	private AudioStreamPlayer2D _ghostFailurePlayer;
	private Label _interactionLabel;

	[Export]
	public int TimeOutBeforeNextInteraction { get; set; } = 3; //time to wait if the player choose the wrong answer

	[Export]
	public int TimeOutBeforeFading { get; set; } = 3; // time before the ghost will disappear

	private bool _isInCooldown;

	[Export]
	public string InteractionQuestion { get; set; } = "Default question";

	[Export]
	public string InteractionFailure { get; set; } = "Wrong answer!";

	[Export]
	public string InteractionSuccess { get; set; } = "Good answer! I will now leave";

	[Export]
	public int CorrectAnswer { get; set; } = 1;

	private bool _isDisposed = false; //this boolean is used to prevent showing the ghost if he was disposed

    private float _floatSpeed = 2.0f; // Speed of the floating motion
	private float _floatAmplitude = 10.0f; // Amplitude of the floating motion
	private float _floatOffset; // Offset for the sine wave

	public override void _Ready()
	{
		Visible = false;
		_globalSignals = GetNode<GlobalSignals>("../../../GlobalSignals");
		_ghostFailurePlayer = GetNode<AudioStreamPlayer2D>("FailurePlayer");
		_ghostArea = GetNode<Area2D>("GhostArea");
		_interactionRect = GetNode<ColorRect>("InteractionRect");
		_interactionLabel = _interactionRect.GetNode<Label>("InteractionLabel");
		_globalSignals.GlassesChange += UpdateGhostDisplay;
		_interactionLabel.Text = InteractionQuestion;
		_ghostArea.BodyEntered += OnBodyEntered;
		_ghostArea.BodyExited += OnBodyExited;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (_inRange && !_isInCooldown)
		{
			// Loop through answer options (1 to 4)
			for (int i = 1; i <= 4; i++)
			{
				// Construct the action name dynamically (e.g., "answer_1", "answer_2", etc.)
				string action = "answer_" + i;

				// Check if the action (key press) is just pressed
				if (!Input.IsActionJustPressed(action)) continue;
				HandleAnswer(i);
				break; // Exit the loop once the correct answer is detected
			}
		}

		// Floating effect
		_floatOffset += _floatSpeed * (float)delta;
		Position = new Vector2(Position.X, Position.Y + Mathf.Sin(_floatOffset) * _floatAmplitude * (float)delta);
	}

	private void HandleAnswer(int i)
	{
		if (i == CorrectAnswer)
		{
			_interactionLabel.Text = InteractionSuccess;
			_isInCooldown = true; //we cannot interact now
			var timer = GetTree().CreateTimer(TimeOutBeforeFading);
			_globalSignals.EmitGhostSlayed(); 
			timer.Timeout += () =>
			{
				_isDisposed = true;
                QueueFree(); //TODO decide what to do here when the ghost is gone
			};
		}
		else
		{
			_ghostFailurePlayer.Play();
			_isInCooldown = true;
			_interactionLabel.Text = InteractionFailure;
			var timer = GetTree().CreateTimer(TimeOutBeforeNextInteraction); //the player won't be able to interact for a few seconds
			timer.Timeout += () =>
			{
				_isInCooldown = false;
				_interactionLabel.Text = InteractionQuestion;
			};
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body is Hero)
		{
			_inRange = false;
			_interactionRect.Visible = false;
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Hero)
		{
			_inRange = true;
			_interactionRect.Visible = true;
		}
	}

	private void UpdateGhostDisplay(bool shouldShow)
	{
		if(!_isDisposed)
		{
            Visible = shouldShow;
        }
		
	}

}
