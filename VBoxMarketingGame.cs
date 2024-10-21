using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class VBoxMarketingGame : VBoxContainer
{
    [Export]
    public int MinNumberQuestions { get; set; } = 1;

    [Export]
    public int MaxNumberQuestions { get; set; } = 1;

    [Export]
    public int NumbersMaxValue { get; set; } = 10;

    [Export]
    private int DefaultFontSize { get; set; } = 40;

    private Random _random = new Random();

    private List<int> _questionsAnswers = new List<int>();

    private Button _validateButton;


    public override void _Ready()
    {
        _validateButton = GetNode<Button>("CenterContainer/Validate");
        _validateButton.Pressed += CheckAnswers;

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
        string[] operations = { "+", "-", "*" };
        return operations[_random.Next(operations.Length)];
    }

    private int CalculateResult(int num1, int num2, string operation)
    {
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
        GD.Print("on verifie !");
        var index = 0;
        var isCorrect = true;
        foreach(var child in GetChildren())
        {
            if(child is HBoxContainer)
            {
                var childBox = (HBoxContainer)child;
                var answer = childBox.GetNode<LineEdit>("Answer");
                GD.Print("Le texte est : "+answer.Text);
                int answerToInt;
                if(int.TryParse(answer.Text, out answerToInt))
                {
                    if(answerToInt == _questionsAnswers[index])
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

        GD.Print("correct ? :" + isCorrect);
    }

}