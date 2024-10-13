using Godot;
using System;
using System.Collections.Generic;

public partial class Musicien : Employee
{
    private AudioStreamPlayer _musicPlayer;
    private AudioStreamPlayer2D _snoringMusicPlayer;
    private static string REQUIRED_ITEM = "Horn";
    private static List<string> _chatMessages = new List<string>
    
    {
        "You wants to hear my new song?",
        "Feel the music!",
        "I will never fall asleep, there is no way...",
        "La musique adoucit les mÅurs !",
        "Music soothes aches and pains!"
    };

    private static List<string> _stopWorkingMessages = new List<string>
    {
        "zzzzzzzzzz",
        "One more minute...?"
    };

    private static List<string> _backToWork = new List<string>
    {
        "I am backkk",
        "yeaaahh"
    };

    public Musicien() : base(_chatMessages,_stopWorkingMessages, _backToWork)
    {
    }




    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        base._Ready();
        _musicPlayer = GetNode<AudioStreamPlayer>("Music");
        _snoringMusicPlayer = GetNode<AudioStreamPlayer2D>("Snoring");
        StartWorking();

    }




    public override void StartWorking()
    {
        _snoringMusicPlayer.Stop();
        _musicPlayer.Play();

    }

    public override void StopWorking()
    {
        _musicPlayer.Stop();
        _snoringMusicPlayer.Play();
    }

    public override void Interact(Hero hero)
    {

        string message;
        if(EmployeeState.Working == CurrentState)
        {
            message = getRandomChat(); 
        }
        else if(hero.HasItem(REQUIRED_ITEM))
        {
            SetState(EmployeeState.Working);
            hero.RemoveItem();
            message = getRandomBackToWorkChat();
       
        }
        else
        {
            message = getRandomStopWorkingChat();
        }
        ShowTemporaryDialog(message);
    }


}
