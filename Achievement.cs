using System;
using Godot.Collections;

public class Achievement
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int NumberOfStars { get; private set; }

    public Func<bool> Condition { get; private set; }

    public Achievement(string name, string description, int numberOfStars, Func<bool> predicate)
    {
        Name = name;
        Description = description;
        NumberOfStars = numberOfStars;
        Condition = predicate;
    }

    public Dictionary ToDictionary()
    {
        return new Dictionary
        {
            { "Name", Name },
            { "Description", Description },
            { "NumberOfStars", NumberOfStars }
        };
    }

    public bool IsCompleted()
    {
        return Condition != null && Condition();
    }



}
