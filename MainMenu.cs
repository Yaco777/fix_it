using Godot;
using System;

public partial class MainMenu : Control
{


    [Export]
    private float OnHoverIncreaseX { get; set; } = 1.1f;

    [Export]
    private float OnHoverIncreaseY { get; set; } = 1.1f;

    [Export]
    private float OnHoverAnimationDuration { get; set; } = 0.3f;

    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
    {
        // Set fullscreen mode
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen); // We put the game in fullscreen

        
        Button playButton = GetNode<Button>("PlayButton");
        Button quitButton = GetNode<Button>("QuitButton");

        // Set pivot to the center for both buttons
        CenterButtonPivot(playButton);
        CenterButtonPivot(quitButton);

      
        playButton.Pressed += PlayGame;
        quitButton.Pressed += QuitGame;

        // Connect mouse enter/exit signals for hover effects
        playButton.MouseEntered += () => AnimateButton(playButton, new Vector2(OnHoverIncreaseX, OnHoverIncreaseY)); 
        playButton.MouseExited += () => AnimateButton(playButton, new Vector2(1.0f, 1.0f));

        quitButton.MouseEntered += () => AnimateButton(quitButton, new Vector2(OnHoverIncreaseX, OnHoverIncreaseY)); 
        quitButton.MouseExited += () => AnimateButton(quitButton, new Vector2(1.0f, 1.0f));
    }

    
    private void AnimateButton(Button button, Vector2 targetScale)
    {
        //we create a smooth animation
        var tween = CreateTween();
        tween.TweenProperty(button, "scale", targetScale, OnHoverAnimationDuration)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut);
    }

    
    private void CenterButtonPivot(Button button)
    {
        // Set the pivot of the button to its center
        button.PivotOffset = button.Size / 2;
    }

    public void PlayGame()
    {
        //method used to play the game
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
        //var game_scene = GD.Load<PackedScene>("res://main.tscn");

        GetTree().ChangeSceneToFile("res://main.tscn");
        //RemoveAllCollectibles();
        
    }

    

    public void QuitGame()
    {
        //method used to quit the game
        
        GetTree().Quit();
    }
}
