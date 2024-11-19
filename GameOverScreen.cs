using Godot;
using System;

public partial class GameOverScreen : CanvasLayer
{
    private Button _button;

    public override void _Ready()
    {
        _button = GetNode<Button>("ColorRect/MainMenuButton");
        _button.Pressed += LaunchMainMenu;
        GetNode<AudioStreamPlayer>("GameOverSound").Play();
    }

    public void LaunchMainMenu()
    {
        GetTree().ChangeSceneToFile("res://main_menu.tscn");
    }
}
