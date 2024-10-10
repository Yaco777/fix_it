using Godot;
using System;

public partial class Hero : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 400; //speed of the hero
    public Vector2 ScreenSize; //size of the screen

    private bool _canClimb = false; //if the player enter the area of a ladder, he can climb
    private bool _isClimbing = false; //tell if the player is currently climbing (he can't go on the left or right)

    private string _collectedItem = null; //current item in the inventory


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

        //if the player decided to climb, we update the value of is_climbing
        if(_canClimb && (Input.IsActionJustPressed("move_up") || Input.IsActionJustPressed("move_down"))) {
            _isClimbing = true;
        }

        //if the player is climbing, he can't go on the left or the right
        if (_isClimbing)
        {
            var isMoving = false;
            if (Input.IsActionPressed("move_up"))
            {
                velocity.Y -= 1; //go up
                isMoving = true;
            }
            else if (Input.IsActionPressed("move_down"))
            {
                velocity.Y += 1; // go down
                isMoving = true;
            }
           
            animatedSprite2D.Animation = "ladder";
            velocity = velocity.Normalized() * Speed;
            //isMoving variable is used to know if we have to play the animation
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
                velocity.X += 1; //right
            }

            if (Input.IsActionPressed("move_left"))
            {
                velocity.X -= 1; //left
            }

            if (velocity.Length() > 0)
            {
                velocity = velocity.Normalized() * Speed; //we normalize the vector
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
                //when the player doesn't move, we stop the animation
                animatedSprite2D.Stop();
                animatedSprite2D.Animation = "default"; 
            }
        }

        //update the position of the player
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


    public void CollectItem(string itemType)
    {
        /**
         * Method used to collect an item. This method doesn't check that the inventory is empty! 
         * throw InvalidOperationException if you try to collect an item even if your inventory is full
         */
        if (_collectedItem == null)
        {
            _collectedItem = itemType;  // Ajoute l'objet à l'inventaire
            GD.Print("Objet collecté : " + itemType);
            // Vous pouvez ajouter d'autres actions ici (comme des effets visuels, des sons, etc.)
        }
        else
        {
            GD.Print("Vous avez déjà un objet : " + _collectedItem);
            throw new InvalidOperationException("You are trying to collect an item but the inventory is full!");
        }
    }


    public bool CanPickItem()
    {
        //check if it's possible to pick an item (the inventory is empty)
        return _collectedItem == null;
    }
}
