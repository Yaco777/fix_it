using Godot;
using System;

public partial class Credits : Control
{

    public override void _Ready()
    {
        Button mainMenu = GetNode<Button>("ReturnToMainMenu");
        mainMenu.Pressed += ReturnToMainMenu;
    }

    private void ReturnToMainMenu()
    {
        GetTree().ChangeSceneToFile("res://main_menu.tscn");
    }
}
