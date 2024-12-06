using Godot;
using System;

public partial class Opening : Control
{

	private VideoStreamPlayer _videoPlayer;

	[Export]
	public VideoStream VideoStream { get; set; } = (VideoStream)ResourceLoader.Load("res://videos/op1.ogv");

	private Button _skipButton;

	[Export]
	public String Destination { get; set; } = "MainMenu";

	public override void _Ready()
	{
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);

		_videoPlayer = GetNode<VideoStreamPlayer>("VideoStreamPlayer");
		_videoPlayer.Stream = VideoStream;
		_videoPlayer.Play();
		_skipButton = GetNode<Button>("SkipButton");
		_skipButton.Pressed += GoToNextScene;
		_videoPlayer.Finished += GoToNextScene;
	}

	private void GoToNextScene()
	{
		_videoPlayer.Stop();

        
        if (Destination == "MainMenu")
        {
            GetTree().ChangeSceneToFile("res://main_menu.tscn");
        }
        else
        {
            GetTree().ChangeSceneToFile("res://main.tscn");
        }
     


	}
}
