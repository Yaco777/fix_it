using Godot;
using System;

public partial class Notebook : CanvasLayer
{

    private Label _ingredientsLabel;
    private GlobalSignals _globalSignals;

    public override void _Ready()
    {
        _ingredientsLabel = GetNode<Label>("NotebookSprite/IngredientsLabel");
        _globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
        _globalSignals.IngredientCollected += AddNewIngredient;
        _globalSignals.CookMinigameSuccess += ResetIngredients;
    }

    private void ResetIngredients()
    {
        _ingredientsLabel.Text = "";
    }

    private void AddNewIngredient()
    {
        var cook = GetNode<Cook>("../../Employees/Cook");
        var nextIngredient = cook.GetNextIngredient();
        _ingredientsLabel.Text += "\n" + nextIngredient;
    }
}
