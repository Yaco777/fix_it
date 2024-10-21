using Godot;
using System;
using System.Collections.Generic;

public partial class Marketing : Employee
{

    private bool _miniGameSuccess = false;
    private GlobalSignals _globalSignals;
    private CanvasLayer _marketingMinigame;
    private static List<string> _chatMessages = new List<string>
    {
        "M1",
        "M2",
        "M3",
        "M4"
    };

    private static List<string> _stopWorkingMessages = new List<string>
    {
        "MM1",
        "MM2"
    };

    private static List<string> _backToWork = new List<string>
    {
        "MMM1",
        "MMM2"
    };

    public Marketing() : base(_chatMessages, _stopWorkingMessages, _backToWork,"Marketing")
    {
    }

    public override void _Ready()
    {
        base._Ready();
        _globalSignals = GetNode<GlobalSignals>("../../GlobalSignals");
        _marketingMinigame = GetNode<CanvasLayer>("MarketingMinigame");
        _marketingMinigame.Visible = false;
        _globalSignals.MarketingMinigameSuccess += MinigameSuccess;
        StartWorking();
    }

    private void MinigameSuccess()
    {
        _miniGameSuccess = true;
    }


    public override void StopWorking()
    {
        base.StopWorking();
        _miniGameSuccess = false;

    }

    public override void Interact(Hero hero)
    {

        if (CurrentState == EmployeeState.NotWorking && _miniGameSuccess == false)
        {
            _marketingMinigame.Visible = true;

        }
        else if (CurrentState == EmployeeState.NotWorking && _miniGameSuccess == true)
        {
            ShowBackToWorkChat();
            SetState(EmployeeState.Working);
        }
        else
        {
            base.Interact(hero);
        }
    }




}
