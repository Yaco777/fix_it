using System.Collections.Generic;
using Godot;

public partial class AchievementDisplay : Control
{
    private ColorRect _colorRect;
    private Label _labelAchievementName;
    private Label _labelAchievementDescription;

    [Export]
    public float AchievementDisplayTime { get; set; } = 4f;

    [Export]
    public float AchievementFadeInTime { get; set; } = 0.2f;

    [Export]
    public float AchievementFadeOutTime { get; set; } = 0.2f;

    private Queue<Achievement> _achievementQueue = new Queue<Achievement>();
    private bool _isDisplaying;

    public override void _Ready()
    {
        _colorRect = GetNode<ColorRect>("AchievementRect");
        _labelAchievementName = GetNode<Label>("AchievementRect/AchievementName");
        _labelAchievementDescription = GetNode<Label>("AchievementRect/AchievementDescription");
        _colorRect.Visible = false;
    }

    public void ShowAchievement(Achievement achievement)
    {
        _achievementQueue.Enqueue(achievement);
        if (!_isDisplaying)
        {
            DisplayNextAchievement();
        }
    }

    private void DisplayNextAchievement()
    {
        if (_achievementQueue.Count == 0)
        {
            _isDisplaying = false;
            return;
        }

        _isDisplaying = true;
        var achievement = _achievementQueue.Dequeue();
        _labelAchievementName.Text = achievement.Name;
        _labelAchievementDescription.Text = achievement.Description;

        _colorRect.Visible = true;
        _colorRect.Modulate = new Color(_colorRect.Modulate.R, _colorRect.Modulate.G, _colorRect.Modulate.B, 0);

        Tween fadeInTween = CreateTween();
        fadeInTween.TweenProperty(
            _colorRect,
            "modulate:a",
            1,
            AchievementFadeInTime
        );

        fadeInTween.Finished += () =>
        {
            var timer = GetTree().CreateTimer(AchievementDisplayTime);
            timer.Timeout += () =>
            {
                Tween fadeOutTween = CreateTween();
                fadeOutTween.TweenProperty(
                    _colorRect,
                    "modulate:a",
                    0,
                    AchievementFadeOutTime
                );

                fadeOutTween.Finished += () =>
                {
                    _colorRect.Visible = false;
                    _colorRect.Modulate = new Color(_colorRect.Modulate.R, _colorRect.Modulate.G, _colorRect.Modulate.B);
                    DisplayNextAchievement(); // Show the next achievement after this one fades out
                };

                fadeOutTween.Play();
            };
        };

        fadeInTween.Play();
    }
}
