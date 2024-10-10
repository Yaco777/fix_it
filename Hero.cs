using Godot;
using System;

public partial class Hero : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 400; //speed of the hero
    public Vector2 ScreenSize; //size of the screen

    private bool _canClimb = false; //if the player enter the area of a ladder, he can climb
    private bool _isClimbing = false; //tell if the player is currently climbing (he can't go on the left or right)


    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        var ladderArea = GetNode<LadderArea>("../Ladder/LadderArea"); 
        ladderArea.LadderEntered += OnLadderAreaEntered;
        ladderArea.LadderExited += OnLadderAreaExited;


    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Vector2.Zero; 
        var animatedSprite2D = GetNode<AnimatedSprite2D>("HeroSprites");

        if(_canClimb && (Input.IsActionJustPressed("move_up") || Input.IsActionJustPressed("move_down"))) {
            _isClimbing = true;
        }

        //if the player is climbing, he can't go on the left or the right
        if (_isClimbing)
        {
            var isMoving = false;
            if (Input.IsActionPressed("move_up"))
            {
                velocity.Y -= 1;
                isMoving = true;
            }
            else if (Input.IsActionPressed("move_down"))
            {
                velocity.Y += 1; // go down
                isMoving = true;
            }
           
            animatedSprite2D.Animation = "ladder";
            velocity = velocity.Normalized() * Speed;
            if (isMoving)
            {
                animatedSprite2D.Play();
            }
            else
            {
                animatedSprite2D.Stop();
            }
            
            
        }
        else
        {
            if (Input.IsActionPressed("move_right"))
            {
                velocity.X += 1;
            }

            if (Input.IsActionPressed("move_left"))
            {
                velocity.X -= 1;
            }

            if (velocity.Length() > 0)
            {
                velocity = velocity.Normalized() * Speed;
                animatedSprite2D.Play();

                if (velocity.X < 0)
                {
                    animatedSprite2D.Animation = "left";
                }
                else
                {
                    animatedSprite2D.Animation = "right";
                }
            }
            else
            {
                animatedSprite2D.Stop();
                animatedSprite2D.Animation = "default"; // Animation par défaut
            }
        }

        Position += velocity * (float)delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );
    }

    // when the player enter the area2D of a ladder
    private void OnLadderAreaEntered()
    {
        _canClimb = true; ;
    }

    // when the player get out of the area2D of a ladder
    private void OnLadderAreaExited()
    {
        _isClimbing = false; // Le joueur ne peut plus grimper
        _canClimb = false;
    }
}
