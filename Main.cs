using Godot;
using System;

public partial class Main : Node
{
    public override void _Ready()
    {
        // Get the root node
        var root = GetTree().Root;

        // Iterate through all children of the root node
        foreach (Node child in root.GetChildren())
        {
            // Check if the child is not this node ("Main")
            if (child != this)
            {
                // Remove the child from the tree
                child.QueueFree();
            }
        }
        Room.AmountStarsRequired = 1;
    }
}
