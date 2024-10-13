using Godot;
using System;
using System.Collections.Generic;

public partial class Musicien : Employee
{
    private AudioStreamPlayer _musicPlayer;
    private AudioStreamPlayer2D _snoringMusicPlayer;
    private AudioStreamPlayer2D _hornMusicPlayer;
    private static string REQUIRED_ITEM = "Horn";
    private Node2D _noteNode;
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

    public Musicien() : base(_chatMessages,_stopWorkingMessages, _backToWork,"Musicien")
    {
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        base._Ready();
        _musicPlayer = GetNode<AudioStreamPlayer>("Music");
        _snoringMusicPlayer = GetNode<AudioStreamPlayer2D>("Snoring");
        _hornMusicPlayer = GetNode<AudioStreamPlayer2D>("Horn");
        _noteNode = GetNode<Node2D>("Notes");
        StartWorking();

    }




    public override void StartWorking()
    {
        //when the employee work, the music start
        _snoringMusicPlayer.Stop();
        _musicPlayer.Play();

    }

    public override void StopWorking()
    {
        //when the employee stop working, the music will stop and we will hear him snoring and a horn will be generated in the building
        _musicPlayer.Stop();
        _snoringMusicPlayer.Play();
        StartGeneratingNotes();
    }

    private void StartGeneratingNotes()
    {
        // Lancer la coroutine pour gÃ©nÃ©rer des notes
        _ = GenerateNotes();
    }

    private async System.Threading.Tasks.Task GenerateNotes()
    {
        while (CurrentState == EmployeeState.NotWorking) // Tant que le musicien ne travaille pas
        {
            CreateNote(); // CrÃ©e une note
            await ToSignal(GetTree().CreateTimer(3f), "timeout"); // Attendre 0.5 seconde avant de crÃ©er la prochaine note
        }
    }

    private void CreateNote()
    {
        var note = (Notes) _noteNode.Duplicate(); // Dupliquer le nÅud de note
        AddChild(note); // Ajouter la note Ã  la scÃ¨ne

        // DÃ©finir la position de dÃ©part de la note
        note.GlobalPosition = GlobalPosition; // Positionner la note au mÃªme endroit que le musicien

        // GÃ©nÃ©rer une direction alÃ©atoire
        Vector2 direction = new Vector2(GD.Randf() * 2 - 1, GD.Randf() * 2 - 1).Normalized();

        Random random = new Random();
        if(random.Next(2) == 0)
        {
            note.GetNode<Sprite2D>("Note1").Visible = true;
            note.GetNode<Sprite2D>("Note2").Visible = false;
            note.SetSprite(0);
        }
        else
        {
            note.GetNode<Sprite2D>("Note1").Visible = false;
            note.GetNode<Sprite2D>("Note2").Visible = true;
            note.SetSprite(1);
        }
         // Activer un des sprites de la note
        //note.GetNode<Sprite2D>("note2").Visible = true; // Activer l'autre sprite de la note

        // Passer la direction Ã  la note pour son mouvement
        note.SetDirection(direction); // Appeler une mÃ©thode pour dÃ©finir la direction
    }

    public override void Interact(Hero hero)
    {
        /**
         * The interaction with the Musicien will display a message according to it's working state.
         * If the Hero has the REQUIRED_ITEM, the musicien will work again
         */
        string message;
        if(EmployeeState.Working == CurrentState)
        {
            message = getRandomChat(); 
        }
        else if(hero.HasItem(REQUIRED_ITEM))
        {
            SetState(EmployeeState.Working);
            hero.RemoveItem();
            _hornMusicPlayer.Play();
            message = getRandomBackToWorkChat();
       
        }
        else
        {
            message = getRandomStopWorkingChat();
        }
        ShowTemporaryDialog(message);
    }


}
