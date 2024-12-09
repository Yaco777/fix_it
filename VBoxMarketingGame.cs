using System;
using System.Collections.Generic;
using Godot;

public partial class VBoxMarketingGame : VBoxContainer
{
	[Export]
	public int MinNumberQuestions { get; set; } = 2;

	[Export]
	public int MaxNumberQuestions { get; set; } = 4;

	[Export]
	public int NumbersMaxValue { get; set; } = 10; //the maximum value (not included) for the numbers in the questions

	[Export]
	private int DefaultFontSize { get; set; } = 40; //font size of the the labels

	private Random _random = new Random();

	private List<int> _questionsAnswers = new List<int>();

	private Button _validateButton;

	private Area2D _exitArea;

	private GlobalSignals _globalSignals;

	private AudioStreamPlayer _failurePlayer;

	private AudioStreamPlayer _successPlayer;

	private TextureRect _marketingGamePanel;


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
		_exitArea = GetNode<Area2D>("../../../ExitSpriteArea");
		_globalSignals = GetNode<GlobalSignals>("../../../../../../../GlobalSignals");
		_exitArea.InputEvent += GiveUp;
		_failurePlayer = GetNode<AudioStreamPlayer>("Failure");
		_successPlayer = GetNode<AudioStreamPlayer>("Success");
		_marketingGamePanel = GetNode<TextureRect>("../../..");
	
		ResetAll();
		
	}

	

	public void ResetAll()
	{
		
		//we first remove all the HBoxContainer
		foreach (var child in GetChildren())
		{
			if(child is HBoxContainer)
			{
				RemoveChild(child);
			}
		}
		_questionsAnswers.Clear();

		//then we create the questions
		var nbQuestions = MinNumberQuestions + _random.Next(MaxNumberQuestions - MinNumberQuestions);
		for (int i = 0; i < nbQuestions; i++)
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
        MoveChild(hboxContainer, 0);
    }

	private Label RandomQuestion()
	{
		var label = new Label();
		var num1 = _random.Next(1, NumbersMaxValue);
		int num2 = _random.Next(1, NumbersMaxValue);
		var operation = GetRandomOperation();
		var result = CalculateResult(num1, num2, operation);
		_questionsAnswers.Insert(0,result); //we add the answer to the question
		label.Text = $"{num1} {operation} {num2} = ";
		label.Name = "Label";
		
		//label.AddThemeColorOverride("font_color", new Color(0, 0, 0));
		label.AddThemeFontSizeOverride("font_size", DefaultFontSize);
		return label;
	}

	private String GetRandomOperation()
	{
		//return a random operator
		string[] operations = { "+", "-", "*" };
		return operations[_random.Next(operations.Length)];
	}

	private int CalculateResult(int num1, int num2, string operation)
	{
	 //compture the result of the question   
		return operation switch
		{
			"+" => num1 + num2,
			"-" => num1 - num2,
			"*" => num1 * num2,
			_ => 0
		};
	}





	public void CheckAnswers()
	{
		//check if all the answers are correct 
	   
		var index = 0;
		var isCorrect = true;
		foreach (var child in GetChildren())
		{
			if (child is HBoxContainer)
			{
				var childBox = (HBoxContainer)child;
				var answer = childBox.GetNode<LineEdit>("Answer");
				var label = childBox.GetNode<Label>("Label");
				int answerToInt;
				if (int.TryParse(answer.Text, out answerToInt)) //we try to cast the answer in int
				{
					if (answerToInt == _questionsAnswers[index]) 
					{
						index++;
						ChangeAnswerBackgroundColor(answer, CorrectAnswerColor);
					   
						//make answer bg in green

					}
					else
					{
						isCorrect = false;
						ChangeAnswerBackgroundColor(answer, WrongAnswerColor);
						//make answer bg in red
						index++;
					}
				}
				else
				{
					isCorrect = false;
					break;
				}

			}
		}

		if (isCorrect)
		{

			_successPlayer.Play();
			_globalSignals.EmitMarketingMinigameSuccess();
			HideGame(); //we hide the game
			//ChangePanelColor(new Color(0, 1, 0));
			ResetAll();
		}
		else
		{
			_failurePlayer.Play();
			//ChangePanelColor(FailureColor);
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
		var marketingGame = GetNode<CanvasLayer>("../../../..");
		marketingGame.Visible = false;
	}

   




}
