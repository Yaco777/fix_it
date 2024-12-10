using System;
using Godot;

public partial class PauseMenu : CanvasLayer
{

   
    private bool _paused;
    private Button _resumeButton;
    private Button _returnToMainMenu;
    private Button _exitButton;
    private Button _achievementsButton;
    private CanvasLayer _achievementsDisplay;
    private Label _highScoreLabel;
    private double _min = -1;
    private double _highScore;
    private int _time_left;

    public override void _Ready()
    {
        _resumeButton = GetNode<Button>("VBoxContainer/Resume");
        _exitButton = GetNode<Button>("VBoxContainer/Exit");
        _returnToMainMenu = GetNode<Button>("VBoxContainer/MainMenu");
        _achievementsButton = GetNode<Button>("VBoxContainer/Achievements");
        _achievementsDisplay = GetNode<CanvasLayer>("../AchievementsDisplay");
        _achievementsDisplay.Visible = false;
        _highScoreLabel = GetNode<Label>("VBoxContainer/HighScore");
        _highScoreLabel.Visible = false;

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
           
            GetTree().Paused = true;
            Show();
            HighScore();
        }
        else
        {
            
            Hide();
            GetTree().Paused = false;

        }
       
    }
    private bool FileExists(string path)
    {
        FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file != null)
        {
            file.Close();
            return true; 
        }
        return false;
    }
    public void HighScore()
    {
        if(FileExists(@"user://high_score.txt")){
            GD.Print("High score file exists");
            using var file = FileAccess.Open("user://high_score.txt", FileAccess.ModeFlags.Read);
            // string content = file.GetAsText();
            // GD.Print(content);
           // _time_left = content.ToInt();
            if (_highScore > 0)
                _highScoreLabel.Visible = true;
            if (_min < _time_left)
                _min = _time_left;
            _highScore = 12 * 60 - _min;
            int minutes = Mathf.FloorToInt((float)_highScore / 60);
            int seconds = Mathf.FloorToInt((float)_highScore % 60);
            _highScoreLabel.Text = $"High Score: {minutes:00}:{seconds:00}";
        }
        else
            GD.Print("No high score found");
    }

    public void OnResumePressed()
    {
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