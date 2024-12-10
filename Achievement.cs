using System;
using Godot;
using Godot.Collections;

public class Achievement
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int NumberOfStars { get; private set; }

    public Func<bool> Condition { get; private set; }

    public int Level { get; private set; }

    public bool Unlocked { get; set; }

    public Achievement(string name, string description, int numberOfStars, Func<bool> predicate, int level)
    {
        Name = name;
        Description = description;
        NumberOfStars = numberOfStars;
        Condition = predicate;
        Level = level;
        Unlocked = false;
    }

    public Achievement(string name, int level)
    : this(name, "NA", 0, () => true, level)
    {
    }

    public Dictionary ToDictionary()
    {
        return new Dictionary
        {
            { "Name", Name },
            { "Description", Description },
            { "NumberOfStars", NumberOfStars },
            { "Level", Level },    
        };
    }

    public bool IsCompleted()
    {
        return Condition != null && Condition();
    }

    public Texture2D GetTextureOfAchievement()
    {
        if(!Unlocked)
        {
            return GD.Load<Texture2D>("res://UI/AchL0.png");
        }

        switch (Level)
        {
            case 0:
                return GD.Load<Texture2D>("res://UI/AchL0.png");
            case 1:
                return GD.Load<Texture2D>("res://UI/AchL1.png");
            case 2:
                return GD.Load<Texture2D>("res://UI/AchL2.png");
            case 3:
                return GD.Load<Texture2D>("res://UI/AchL3.png");
            default:
                GD.PrintErr("Invalid level for achievement: " + Level);
                return null; // Return null if the level is invalid
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is Achievement other)
        {
            return Name == other.Name;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }



}
