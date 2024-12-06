using System.Collections.Generic;
using Godot;

public partial class MainMenu : Control
{


    [Export]
    private float OnHoverIncreaseX { get; set; } = 1.1f;

    [Export]
    private float OnHoverIncreaseY { get; set; } = 1.1f;

    [Export]
    private float OnHoverAnimationDuration { get; set; } = 0.3f;

    [Export]
    private float FadeInDuration { get; set; } = 1.0f; // Duration for the fade-in effect

    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
    {
        // Set fullscreen mode
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen); // We put the game in fullscreen

        StartFadeIn();
        Button playButton = GetNode<Button>("PlayButton");
        Button quitButton = GetNode<Button>("QuitButton");
        Button tutorialButton = GetNode<Button>("TutorialButton");
        Button creditsButton = GetNode<Button>("CreditsButton");
        List<Button> buttons = new()
        {
            playButton,
            quitButton,
            tutorialButton,
            creditsButton
        };


        CenterButtonPivot(playButton);
        CenterButtonPivot(quitButton);

      
        playButton.Pressed += PlayGame;
        quitButton.Pressed += QuitGame;
        tutorialButton.Pressed += StartTutorial;
        creditsButton.Pressed += ShowCredits;

        // Connect mouse enter/exit signals for hover effects
        foreach (var button in buttons)
        {
            button.MouseEntered += () => AnimateButton(button, new Vector2(OnHoverIncreaseX, OnHoverIncreaseY));
            button.MouseExited += () => AnimateButton(button, new Vector2(1.0f, 1.0f));

            // Set pivot to the center for the button
            CenterButtonPivot(button);
        }


    }

    private void StartFadeIn()
    {
        // Set the initial modulation to fully transparent
        Modulate = new Color(1, 1, 1, 0);

        // Animate the alpha channel of the Modulate property
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 1.0f, FadeInDuration)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut);
    }


    private void AnimateButton(Button button, Vector2 targetScale)
    {
        //we create a smooth animation
        var tween = CreateTween();
        tween.TweenProperty(button, "scale", targetScale, OnHoverAnimationDuration)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut);
    }

    
    private static void CenterButtonPivot(Button button)
    {
        // Set the pivot of the button to its center
        button.PivotOffset = button.Size / 2;
    }

    private void StartTutorial()
    {
        GetTree().ChangeSceneToFile("res://tutorial.tscn");
    }

    private void ShowCredits()
    {
        GetTree().ChangeSceneToFile("res://credits.tscn");
    }

    private void PlayGame()
    {
        //method used to play the game
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
        //var game_scene = GD.Load<PackedScene>("res://main.tscn");
        var openingScene =(Opening) GD.Load<PackedScene>("res://Opening.tscn").Instantiate();
        openingScene.Destination = "Main";
        openingScene.VideoStream = (VideoStream)ResourceLoader.Load("res://videos/op2.ogv");
        //GetTree().ChangeSceneToFile("res://main.tscn");
        //RemoveAllCollectibles();
        QueueFree();
        GetTree().Root.AddChild(openingScene);

    }


    private void QuitGame()
    {
        //method used to quit the game
        
        GetTree().Quit();
    }
}
