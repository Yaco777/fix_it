using System;
using System.Collections.Generic;
using Godot;

public partial class VBoxCookGame : CanvasLayer
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
		_validateButton = GetNode<Button>("CookGameRect/MarginContainer/VBoxContainer/MarginContainer/CenterContainer/Button");
		_validateButton.Pressed += CheckAnswers;
		_exitArea = GetNode<Area2D>("CookGameRect/ExitSprite/Area2D");
		_globalSignals = GetNode<GlobalSignals>("../../../GlobalSignals");
		_exitArea.InputEvent += GiveUp;
		_failurePlayer = GetNode<AudioStreamPlayer>("CookGameRect/MarginContainer/VBoxContainer/Failure"); 
		_successPlayer = GetNode<AudioStreamPlayer>("CookGameRect/MarginContainer/VBoxContainer/Success");
		_cookGamePanel = GetNode<Panel>("CookGameRect");
		var lines = GetNode<VBoxContainer>("CookGameRect/MarginContainer/VBoxContainer");
		foreach (var line in lines.GetChildren())
		{
			if(line is LineEdit edit)
				_questionIngredient.Add(edit);
			else
			{
				GD.Print("Line is null	");
			}
		}
		ResetAll();
		InitIngredient();
	}

	private void InitIngredient()
	{
		_ingredientList.Add("Tomato");
		_ingredientList.Add("Pasta");
		_ingredientList.Add("Eggs");
		_ingredientList.Add("Burger");
		_ingredientList.Add("Garlic");
		_ingredientList.Add("Chocolate");
		_ingredientList.Add("Flour");
		_ingredientList.Add("Cheese");
		_ingredientList.Add("Chicken");
		_ingredientList.Add("Pineapple");
		while (_chosenIngredientList.Count < 4)
			_chosenIngredientList.Add(_ingredientList[_random.Next(_ingredientList.Count)]);
	}

	public List<string> GetIngredientList()
	{
		return new List<string>(_chosenIngredientList);
	}


	public void ResetAll(){

        _chosenIngredientList.Clear();
	}

	private void CheckAnswers()
	{
		var i = 0;
		var correctAnswer = true;
		GD.Print(_questionIngredient.Count);
		foreach (var ingredient in _chosenIngredientList)
		{
			if (ingredient != _questionIngredient[i].Text)
				correctAnswer = false;
			i++;
		}

		if (correctAnswer)
		{
			_globalSignals.EmitCookMinigameSuccess();
			HideGame();
			_successPlayer.Play();
		}
		else
		{
			_failurePlayer.Play();
		}
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
