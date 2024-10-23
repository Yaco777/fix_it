using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{

   
    private bool _paused;
    private Button _resumeButton;
    private Button _returnToMainMenu;
    private Button _exitButton;
    private Button _achievementsButton;
    private CanvasLayer _achievementsDisplay;

    public override void _Ready()
    {
        _resumeButton = GetNode<Button>("VBoxContainer/Resume");
        _exitButton = GetNode<Button>("VBoxContainer/Exit");
        _returnToMainMenu = GetNode<Button>("VBoxContainer/MainMenu");
        _achievementsButton = GetNode<Button>("VBoxContainer/Achievements");
        _achievementsDisplay = GetNode<CanvasLayer>("../AchievementsDisplay");
        _achievementsDisplay.Visible = false;



    }
    public override void _Process(double delta)
    {

   
        if (Input.IsActionJustPressed("pause_game") && _achievementsDisplay.Visible == false)
        {
            _paused = !_paused;
            Pause();
        }
    }

    private void Pause()
    {
        if (_paused)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            GetTree().Paused = true;
            Show();
        }
        else
        {
            

            Input.MouseMode = Input.MouseModeEnum.Captured;
            Hide();
            GetTree().Paused = false;

        }
       
    }

    public void OnResumePressed()
    {
        GD.Print("Resume clicked !!");
        _paused = false;
        Pause();
    }

    public void ExitGame()
    {
        GetTree().Quit();
    }

    public void ReturnMainMenu()
    {
       
        _paused = false;
        GetTree().Paused = false;
       // var main_menu = GD.Load<PackedScene>("res://main_menu.tscn");
       
        GetTree().ChangeSceneToFile("res://main_menu.tscn");
      
    }

    public void ShowAchievements()
    {
        
        _achievementsDisplay.Visible = true;
       
    }

}