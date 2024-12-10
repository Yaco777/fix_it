using System;
using System.Collections.Generic;
using Godot;

public partial class VBoxAchievementsDisplay : VBoxContainer
{

    private OptionButton _optionButton;
    private ProgressSystem _progressSystem;
    private Area2D _exitArea;
    private GlobalSignals _globalSignals;
    private CanvasLayer _canvasLayer;
    private ProgressBar _progressBar;
    private Building _building;

    [Export]
    public int PanelSize { get; set; } = 50; //the panel size that display all achievements
     



    public override void _Ready()
    {
        _optionButton = GetNode<OptionButton>("../../../OptionButton");
        _progressBar = GetNode<ProgressBar>("../../../ProgressBar");
        _progressSystem = GetNode<ProgressSystem>("../../../../../UI/ProgressSystem");
        _exitArea = GetNode<Area2D>("../../../ExitSpriteArea");
        _canvasLayer = GetNode<CanvasLayer>("../../../..");
        _building = _canvasLayer.GetNode<Building>("../Building");
        _exitArea.InputEvent += HideAchievementsInput;
        ShowAchievements();
        _optionButton.ItemSelected += UpdateDisplay;
        DisplayBasedOnCurrentButtonState();
        VisibilityChanged += OnVisibilityChanged; //when the visiblity change, we update the achievements
        _globalSignals = _canvasLayer.GetNode<GlobalSignals>("../GlobalSignals");
        _globalSignals.ShowAchievements += ChangeVisibility;
        _globalSignals.ReverseAchievementsDisplay += ReverseVisiblity;
    }

    private void ReverseVisiblity()
    {
        ChangeVisibility(!_canvasLayer.Visible);
        
    }

    private void ChangeVisibility(bool changeVisiblity)
    {
        _canvasLayer.Visible = changeVisiblity;

        _progressBar.Value = _building.GetNumberOfWorkDone();
        
    }

    private void OnVisibilityChanged()
    {
        DisplayBasedOnCurrentButtonState();

    }

    public void DisplayBasedOnCurrentButtonState()
    {
        UpdateDisplay(_optionButton.Selected);
    }

    private void UpdateDisplay(long index)
    {
        _progressBar.Value = _building.GetNumberOfWorkDone();
        foreach (var child in GetChildren())
        {
            if (child is HBoxContainer)
            {
                RemoveChild(child);
            }
        }
        List<Achievement> list = new List<Achievement>();
        if(index == 0 )
        {
            list = _progressSystem.GetAllAchievements();
        }
        else if(index == 1)
        {
            list = _progressSystem.GetLockedAchievements();
        }
        else
        {
            list = _progressSystem.GetUnlockedAchievements();
        }

        var i = 0;
        foreach (var item in list)
        {
            AddOneAchievement(item, i);
            i++;
        }
    }

    

    private void HideAchievementsInput(Node viewport, InputEvent @event, long shapeIdx)
    {
        /**
         * Hide the canvas layer
         */
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {

            // we check that it's a left click
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                HideAchievements();
            }
        }

    }

    private void ShowAchievements() {

        var allAchievements = _progressSystem.GetAllAchievements();
        var list = _progressSystem.GetUnlockedAchievements();
        var i = 0;
        foreach(var achievement in allAchievements)
        {
            if(list.Contains(achievement)) {
                achievement.Unlocked = true;
            }
            AddOneAchievement(achievement, i);
            i++;
        }
    }

    private void AddOneAchievement(Achievement achievement, int index)
    {
        var hboxContainer = new HBoxContainer();
        hboxContainer.SizeFlagsHorizontal = SizeFlags.Fill;
        var spriteContainer = new Control();
        spriteContainer.SizeFlagsHorizontal = SizeFlags.ShrinkCenter; // Centre l'ensemble dans le HBox
        spriteContainer.SizeFlagsVertical = SizeFlags.ShrinkCenter;

        var label = new Label();

        //sprite
        var sprite = new Sprite2D();
        var texture2D = achievement.GetTextureOfAchievement();
        sprite.Texture = texture2D;
        sprite.Scale = new Vector2(0.8f,0.8f);
        sprite.GlobalPosition = new Vector2(sprite.GlobalPosition.X + 3500, sprite.GlobalPosition.Y);

        spriteContainer.AddChild(sprite);
        spriteContainer.AddChild(label);
        label.Text = achievement.Name;
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.SetAnchorsPreset(LayoutPreset.FullRect);
        label.GlobalPosition = new Vector2(label.GlobalPosition.X + 1500, label.GlobalPosition.Y - 50);
        label.AddThemeFontSizeOverride("font_size", 120);
        hboxContainer.AddChild(spriteContainer);
        AddChild(hboxContainer);
        

    }

    private void HideAchievements()
    {
        
        _canvasLayer.Visible = false;
    }

   


}
