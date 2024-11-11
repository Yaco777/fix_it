using Godot;
using System;

public partial class Ghost : Node2D
{
    private GlobalSignals _globalSignals;

    public override void _Ready()
    {
        Visible = false;
        _globalSignals = GetNode<GlobalSignals>("../GlobalSignals");
        _globalSignals.GlassesChange += UpdateGhostDisplay;
        
    }

    private void UpdateGhostDisplay(bool shouldShow)
    {
        Visible = shouldShow;
    }
}
