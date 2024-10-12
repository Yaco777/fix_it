using Godot;
using System;

public partial class Musicien : Employee
{
    private AudioStreamPlayer2D _musicPlayer;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        _musicPlayer = GetNode<AudioStreamPlayer2D>("Music");
        StartWorking();
    }

    
   

    public override void StartWorking()
    {
        _musicPlayer.Play();
        GD.Print("Le musicien commence à jouer.");
    }

    public override void StopWorking()
    {
        _musicPlayer.Stop();
        GD.Print("Le musicien s'endort.");
    }


}
