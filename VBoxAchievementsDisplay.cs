using System.Collections.Generic;
using Godot;

public partial class VBoxAchievementsDisplay : VBoxContainer
{

    private OptionButton _optionButton;
    private ProgressSystem _progressSystem;
    private Area2D _exitArea;

    [Export]
    public int PanelSize { get; set; } = 50; //the panel size that display all achievements
     



    public override void _Ready()
    {
        _optionButton = GetNode<OptionButton>("../../../OptionButton");
        _progressSystem = GetNode<ProgressSystem>("../../../../../UI/ProgressSystem");
        _exitArea = GetNode<Area2D>("../../../ExitSprite/ExitSpriteArea");
        _exitArea.InputEvent += HideAchievementsInput;
        ShowAchievements();
        _optionButton.ItemSelected += UpdateDisplay;
        DisplayBasedOnCurrentButtonState();
        VisibilityChanged += OnVisibilityChanged; //when the visiblity change, we update the achievements


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

        foreach (var item in list)
        {
            AddOneAchievement(item);
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
        foreach(var achievement in allAchievements)
        {
            AddOneAchievement(achievement);
        }
    }

    private void AddOneAchievement(Achievement achievement)
    {
        var hboxContainer = new HBoxContainer();
        hboxContainer.SizeFlagsHorizontal = SizeFlags.Fill;
        var panel = new Panel();
        var label = new Label();
        panel.AddChild(label);
        hboxContainer.AddChild(panel);
        label.Text = achievement.Name;
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.SetAnchorsPreset(LayoutPreset.FullRect);
        panel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        panel.CustomMinimumSize = new Vector2(panel.CustomMinimumSize.X, PanelSize);
        label.AddThemeFontSizeOverride("font_size", 40);
        
    

        
        
        AddChild(hboxContainer);

    }

    private void HideAchievements()
    {
        var canvasLayer = GetNode<CanvasLayer>("../../../..");
        canvasLayer.Visible = false;
    }

   
}
