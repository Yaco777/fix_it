using System;
using System.Collections.Generic;
using Godot;

public partial class VBoxCookGame : VBoxContainer
{

	[Export]
	private int DefaultFontSize { get; set; } = 40; //font size of the the labels

	private Random _random = new Random();

	private List<int> _questionsAnswers = new List<int>();

	private Button _validateButton;

	private Area2D _exitArea;

	private GlobalSignals _globalSignals;

	private AudioStreamPlayer _failurePlayer;

	private AudioStreamPlayer _successPlayer;

	private Panel _cookGamePanel;


	[Export]

	public Color FailureColor { get; set; } = new Color(0.7f, 0, 0);

	[Export]
	public Color DefaultBackgroundColor { get; set; } = new Color(0.458f, 0.642f, 0.627f);

	[Export]
	public Color CorrectAnswerColor { get; set; } = new Color(0.585f, 0.909f, 0.755f);

	[Export]
	public Color WrongAnswerColor { get; set; } = new Color(1, 0.611f, 0.639f);


	public override void _Ready()
	{
		_validateButton = GetNode<Button>("CenterContainer/Validate");
		_validateButton.Pressed += CheckAnswers;
		_exitArea = GetNode<Area2D>("../../../ExitSprite/ExitSpriteArea");
		_globalSignals = GetNode<GlobalSignals>("../../../../../../../GlobalSignals");
		_exitArea.InputEvent += GiveUp;
		_failurePlayer = GetNode<AudioStreamPlayer>("Failure");
		_successPlayer = GetNode<AudioStreamPlayer>("Success");
		_cookGamePanel = GetNode<Panel>("../../..");
		ResetPanelColor();
		ResetAll();
		
	}

	

	public void ResetAll()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		ResetPanelColor();
		//we first remove all the HBoxContainer
		foreach (var child in GetChildren())
		{
			if(child is HBoxContainer)
			{
				RemoveChild(child);
			}
		}
		_questionsAnswers.Clear();

		//then we create the questions;
		for (int i = 0; i < 4; i++)
		{
			AddOneQuestion();
		}
	}

	private void AddOneQuestion()
	{
		var hboxContainer = new HBoxContainer();
		hboxContainer.SizeFlagsHorizontal = SizeFlags.Fill;
		var randomQuestion = RandomQuestion();
		var answerLineEdit = new LineEdit();
		answerLineEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		answerLineEdit.Name = "Answer";
		hboxContainer.AddChild(randomQuestion);
		hboxContainer.AddChild(answerLineEdit);
		AddChild(hboxContainer);
	}

	private Label RandomQuestion()
	{
		var label = new Label();
		var num1 = _random.Next(1, 10);
		label.Text = "The ingredients are...?";
		label.Name = "Label";
		
		//label.AddThemeColorOverride("font_color", new Color(0, 0, 0));
		label.AddThemeFontSizeOverride("font_size", DefaultFontSize);
		return label;
	}

	public void CheckAnswers()
	{
		throw new NotImplementedException();

	}

	private void ChangeAnswerBackgroundColor(LineEdit answer, Color color)
	{
	   
		var originalStyle = answer.GetThemeStylebox("normal") as StyleBoxFlat;
		if (originalStyle != null)
		{
			
			var newStyle = new StyleBoxFlat();
		 
			
			newStyle.BgColor = color;
			
			answer.AddThemeStyleboxOverride("normal", newStyle);
		}
	}

	private void ChangePanelColor(Color color)
	{
		// Create a new StyleBoxFlat and set its background color
		/*var styleBox = new StyleBoxFlat();
		styleBox.BgColor = color;
		var st
		// Apply the StyleBox to the Panel
		_cookGamePanel.AddThemeStyleboxOverride("panel", styleBox);*/
		var style = _cookGamePanel.GetThemeStylebox("panel") as StyleBoxFlat;
		style.BgColor = color;
		_cookGamePanel.AddThemeStyleboxOverride("panel", style);


	}

	private void ResetPanelColor()
	{
		// Reset the panel background to a neutral color (e.g., white or transparent)
		ChangePanelColor(DefaultBackgroundColor); // Transparent by default, or choose another color

	}

	private void GiveUp(Node viewport, InputEvent @event, long shapeIdx)
	{
		/**
		 * Hide the canvas layer
		 */
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			
			// we check that it's a left click
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				HideGame();  
			}
		}
		
	}

	private void HideGame()
	{
		var cookGame = GetNode<CanvasLayer>("../../../..");
		cookGame.Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

   




}
