using System;
using System.Collections.Generic;
using Godot;

public partial class VBoxCookGame : VBoxContainer
{
	private readonly HashSet<string> _chosenIngredientList = new();
	private readonly List<string> _ingredientList = new();

	private readonly List<LineEdit> _questionIngredient = new();

	private readonly Random _random = new();

	private Panel _cookGamePanel;

	private Area2D _exitArea;

	private AudioStreamPlayer _failurePlayer;

	private GlobalSignals _globalSignals;

	private AudioStreamPlayer _successPlayer;

	private Button _validateButton;

	[Export] private int DefaultFontSize { get; set; } = 40; //font size of the the labels

	[Export] public Color FailureColor { get; set; } = new(0.7f, 0, 0);

	[Export] public Color DefaultBackgroundColor { get; set; } = new(0.458f, 0.642f, 0.627f);

	[Export] public Color CorrectAnswerColor { get; set; } = new(0.585f, 0.909f, 0.755f);

	[Export] public Color WrongAnswerColor { get; set; } = new(1, 0.611f, 0.639f);


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
		var lines = GetNode<VBoxContainer>("CookGameRect/MarginContainer/VBoxContainer");
		foreach (var line in lines.GetChildren()) _questionIngredient.Add((LineEdit)line);
		InitIngredient();
		ResetPanelColor();
		ResetAll();
	}

	private void InitIngredient()
	{
		while (_chosenIngredientList.Count < 3)
			_chosenIngredientList.Add(_ingredientList[_random.Next(_ingredientList.Count)]);
	}

	public List<string> GetIngredientList()
	{
		return new List<string>(_chosenIngredientList);
	}


	public void ResetAll()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		ResetPanelColor();
		//we first remove all the HBoxContainer
		foreach (var child in GetChildren())
			if (child is HBoxContainer)
				RemoveChild(child);

		_questionIngredient.Clear();
	}

	private void CheckAnswers()
	{
		var i = 0;
		var correctAnswer = true;
		foreach (var ingredient in _questionIngredient)
			if (ingredient.Text != _ingredientList[i])
				correctAnswer = false;
		if (correctAnswer)
			_globalSignals.EmitCookMinigameSuccess();
		HideGame();
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
			// we check that it's a left click
			if (mouseEvent.ButtonIndex == MouseButton.Left)
				HideGame();
	}

	private void HideGame()
	{
		Visible = false;
		InitIngredient();
	}
}
