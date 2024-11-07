﻿using Godot;
using System;

public partial class Tutorial : Node2D
{

    [Export]
    public string FirstMoveMessage { get; set; } = "Welcome!To start the reparation of the game" +
        "let's try to put some music. Follow this corridor to enter the musicien's room with QD or with →, ←, ";

    [Export]
    public string SecondMessage { get; set; } = "Congrat !! Now take the ladder by pressing ↑ or ↓";

    [Export]
    public string ThirdMessage { get; set; } = "You did it ! Now let's try to unlock the musicien room by pressing E. This required a certain amount" +
        "of stars but the first room will be free";

    [Export]
    public string ForthMessage { get; set; } = "Well played, you just unlocked the musicien and your first achievement!";

    [Export]
    public string FifthMessage { get; set; } = "Oh no! The musicien fall asleep. Try to find an object that could wake him up." +
        "You can collect the object by pressing E and then talk to the musicien again.";

    [Export]

    public string SixthMessage { get; set; } = "Fin";

    [Export]
    public int TimeBeforeMusicienSleep { get; set; } = 10; //time that the player need to wait before the horn appear

    [Export]
    public float MoveDistanceRequiredX { get; set; } = 300; //distance for the first goal (go to the right)

    private float _moveDistanceRequiredY = 40; //distance required for the ladder

    [Export]

    public int YMargin { get; set; } = 175; //margin used to move the musicien and the horn

    private Hero _hero;

    private CanvasLayer _canvasLayer;

    private Label _label;

    private float _initialXPos;

    private float _initialYPos;

    private Room _musicienRoom;

    private Musicien _musicien;

    private Timer _timer;

    private ProgressSystem _progressSystem;

    private enum State
    {
        FirstMessage,
        TakeTheLadder,
        UnlockRoom,
        WaitForMusicien,
        MusicienSleep,
        EndTutorial
    }

    private State _state = State.FirstMessage;

    public override void _Ready()
    {
        _hero = GetNode<Hero>("Hero");
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        _label = _canvasLayer.GetNode<Label>("ColorRect/TutorialLabel");
        _label.Text = FirstMoveMessage;
        _initialXPos = _hero.Position.X;
        _initialYPos = _hero.Position.Y;
        _musicienRoom = GetNode<Room>("Building/Rooms/Room");

        //timer for the musicien
        _timer = new Timer();
        _timer.WaitTime = 10.0f;
        _timer.OneShot = true; // Makes the timer stop after it reaches zero
        AddChild(_timer); // Add Timer to the scene tree
        _timer.Timeout += OnTimerTimeout;
        _timer.Stop();

        //achievements
        _progressSystem = GetNode<ProgressSystem>("UI/ProgressSystem");


    }

    public override void _Process(double delta)
    {

        
     
        if(Input.IsActionPressed("message_interaction"))
        {
            //do something
        }

        

        //the hero need to go in the right
        if(State.FirstMessage == _state && _hero.Position.X >= _initialXPos + MoveDistanceRequiredX)
        {
            GD.Print("On passe au prochain !");
            _label.Text = SecondMessage;
            _state = State.TakeTheLadder;
        }

        //the hero need to take the ladder
        else if(State.TakeTheLadder == _state && _hero.Position.Y <= _initialYPos - _moveDistanceRequiredY)
        {
            _label.Text = ThirdMessage;
            _state = State.UnlockRoom;
            
        }

        //the musicien is working
        else if(State.UnlockRoom == _state && _musicienRoom._hasUnlockedRoom)
        {
            _label.Text = ForthMessage;
            _state = State.WaitForMusicien;
            _musicien = GetNode<Musicien>("Employees/Musicien");
            _musicien.Position = new Vector2(_musicien.Position.X, _musicien.Position.Y + YMargin);
            _musicien.StopWorkProbability = 0;
            _timer.Start(); //the musicien will stop to work
            var achievement = new Achievement("Room unlocked", "You unlocked your first room", 10, () => (1 == 1));
            _progressSystem.playAchievement(achievement);


        }

        //when the musicien go to work again
        else if(State.MusicienSleep == _state && _musicien.CurrentState == Employee.EmployeeState.Working)
        {
            _label.Text = SixthMessage;
            _state = State.EndTutorial;
            var achievement = new Achievement("Tutorial", "You completed the tutorial", 10,() => (1 == 1));
            _progressSystem.playAchievement(achievement);

        }
        
        
    }

    //when the timeout end we add the horn to the game
    private void OnTimerTimeout()
    {
        if (_musicien != null)
        {
            _state = State.MusicienSleep;
            _label.Text = FifthMessage;
            _musicien.StopWorking();
            var horn = Collectible.CreateCollectible("Horn");
            horn.Position = _musicienRoom.Position;
            horn.Position = new Vector2(horn.Position.X - 500, horn.Position.Y + YMargin);
            AddChild(horn);
     
        }
    }


}
