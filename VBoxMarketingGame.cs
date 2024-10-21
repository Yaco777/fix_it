using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

    private Button _giveUpButton;

    private GlobalSignals _globalSignals;


    public override void _Ready()
    {
        _validateButton = GetNode<Button>("CenterContainer/Validate");
        _validateButton.Pressed += CheckAnswers;
        _giveUpButton = GetNode<Button>("../../../GiveUp");
        _globalSignals = GetNode<GlobalSignals>("../../../../../../../GlobalSignals");
        _giveUpButton.Pressed += GiveUp;

        ResetAll();
        
    }

    public void ResetAll()
    {
        //we first remove all the HBoxContainer
        foreach(var child in GetChildren())
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
    }

    private Label RandomQuestion()
    {
        var label = new Label();
        var num1 = _random.Next(1, NumbersMaxValue);
        int num2 = _random.Next(1, NumbersMaxValue);
        var operation = GetRandomOperation();
        var result = CalculateResult(num1, num2, operation);
        _questionsAnswers.Add(result); //we add the answer to the question
        label.Text = $"{num1} {operation} {num2} = ";
        label.AddThemeColorOverride("font_color", new Color(0, 0, 0));
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
            "*" => num1 - num2,
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
                int answerToInt;
                if (int.TryParse(answer.Text, out answerToInt)) //we try to cast the answer in int
                {
                    if (answerToInt == _questionsAnswers[index]) 
                    {
                        index++;
                    }
                    else
                    {
                        isCorrect = false;
                        break;
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
            _globalSignals.EmitMarketingMinigameSuccess();
            GiveUp(); //we use the give up method to hide the canvas
            ResetAll();
        }
        else
        {
            //TODO ?
        }

    }

    private void GiveUp()
    {
        /**
         * Hide the canvas layer
         */
        var marketingGame = GetNode<CanvasLayer>("../../../..");
        marketingGame.Visible = false;
    }


}