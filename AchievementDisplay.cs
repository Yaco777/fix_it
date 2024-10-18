using Godot;
using System;

public partial class AchievementDisplay : Control
{
    private ColorRect _colorRect;
    private Label _label;



    public override void _Ready()
    {
        _colorRect = GetNode<ColorRect>("AchievementRect");
        _label = GetNode<Label>("AchievementRect/AchievementName");
        _colorRect.Visible = false;
    }


    public void ShowAchievement(Achievement achievement)
    {
        t
        _label.Text = achievement.Name; //we change the text of the label

       
        _colorRect.Visible = true;
        _colorRect.Modulate = new Color(_colorRect.Modulate.R, _colorRect.Modulate.G, _colorRect.Modulate.B, 0); 
        // the alpha start at 0

        Tween tween = CreateTween();

       
        tween.TweenProperty(
            _colorRect,
            "modulate:a", // //we will change the alpha value
            1,           //max value
            1.0f         //duration of animation
        );

        //after 5 second, we will make it dissepear
        tween.Finished += () =>
        {
            
            var timer = GetTree().CreateTimer(5.0f);
            timer.Timeout += () =>
            {
                //tween used for the fading out effect
                Tween fadeOutTween = CreateTween();
                fadeOutTween.TweenProperty(
                    _colorRect,
                    "modulate:a", 
                    0,            //  0 = transparent
                    1.0f        
                );

                // we hide the color rect
                fadeOutTween.Finished += () =>
                {
                    _colorRect.Visible = false;
                    _colorRect.Modulate = new Color(_colorRect.Modulate.R, _colorRect.Modulate.G, _colorRect.Modulate.B, 1); 
                };

                fadeOutTween.Play(); 
            };

            
        };
        //we play the fade in animation
        tween.Play();
    }


}
