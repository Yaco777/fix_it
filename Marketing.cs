using Godot;
using System;
using System.Collections.Generic;

public partial class Marketing : Employee
{
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

    public Marketing() : base(_chatMessages, _stopWorkingMessages, _backToWork, "Marketing")
    {
    }


}
