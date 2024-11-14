using System;
using Godot;

public partial class SimpleHero : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 400; //speed of the hero
    public Vector2 ScreenSize; //size of the screen

    private bool _canClimb; //if the player enter the area of a ladder, he can climb
    private bool _isClimbing; //tell if the player is currently climbing (he can't go on the left or right)

    private string _collectedItem; //current item in the inventory
    private bool _canGoUp = true; //check if there is a floor at the top of the player
    private bool _canGoDown = true; //check if there is a floor below the player

    private int _actionCooldown; //cooldown to prevent duplication for the items (by spamming)

    private AnimatedSprite2D _animatedSprite2D;

    private UI _ui;



    public override void _Ready()
    {
       
        _animatedSprite2D = GetNode<AnimatedSprite2D>("HeroSprites");
        _ui = GetNode<UI>("../UI");
    }

    private Vector2 ClimbLadder(Vector2 velocity)
    {
        /**
         * Method used to climb a ladder. It's possible only if the player is in the area of a ladder and the movment is legal :
         * if the player wants to go up and is in the area of a roof, we won't allow the move
         * if the play wants to go down and is in the area of a floor, we won't allow the move
         */
        var isMoving = false;
        if (Input.IsActionPressed("move_up") && _canGoUp)
        {
            if (_canGoUp)
            {
                velocity.Y -= 1; // go up
                isMoving = true;

            }
            else
            {
                _isClimbing = false; //if the player wants to go up but he can't, that means that he touched the floor


            }

        }
        else if (Input.IsActionPressed("move_down"))
        {
            if (_canGoDown)
            {
                velocity.Y += 1; // go down
                isMoving = true;
            }
            else
            {
                _isClimbing = false; //if the player wants to go down but he can't, that means that he touched the floor

            }

        }

        _animatedSprite2D.Animation = "ladder";
        velocity = velocity.Normalized() * Speed;
        //isMoving variable is used to know if we have to play the animation
        if (isMoving)
        {
            _animatedSprite2D.Play();
        }
        else
        {
            _animatedSprite2D.Stop();
        }

        return velocity;
    }

    private Vector2 MoveLeftRight(Vector2 velocity)
    {
        /**
         * Move the player to the left or right depending on the action pressed
         */
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
            _animatedSprite2D.Play();

            if (velocity.X < 0)
            {
                _animatedSprite2D.Animation = "left";
            }
            else
            {
                _animatedSprite2D.Animation = "right";
            }
        }
        else
        {
            //when the player doesn't move, we stop the animation
            _animatedSprite2D.Stop();
            _animatedSprite2D.Animation = "default";
        }
        return velocity;
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Vector2.Zero;




        


        //if the player decided to climb, we update the value of is_climbing
        if (_canClimb && ((Input.IsActionJustPressed("move_up") && _canGoUp) || (_canGoDown && Input.IsActionJustPressed("move_down"))))
        {
            _isClimbing = true;
        }

        //if the player is climbing, he can't go on the left or the right
        if (_isClimbing)
        {
            velocity = ClimbLadder(velocity);
        }
        else
        {
            velocity = MoveLeftRight(velocity);
        }



        //update the position of the player
        Position += velocity * (float)delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );

        //drop the item
        if (Input.IsActionJustPressed("drop_item") && _actionCooldown == 0 && _collectedItem != null)
        {
            DropItem();
        }
        MoveAndSlide(); //we block the player if he try to enter a room hitbox
    }

    public void CollectItem(string itemType)
    {
        /**
         * Method used to collect an item. This method doesn't check that the inventory is empty! 
         * throw InvalidOperationException if you try to collect an item even if your inventory is full
         */
        if (CanPickItem())
        {
            _collectedItem = itemType;  //we collect the item
            _ui.UpdateCollectedItem(itemType);

        }
        else
        {
            //this exception should never be thrown
            throw new InvalidOperationException("You are trying to collect an item but the inventory is full!");
        }
    }


    public bool CanPickItem()
    {
        //check if it's possible to pick an item (the inventory is empty)
        return _collectedItem == null && _actionCooldown == 0;
    }

    public bool CollectedItemIsNull()
    {
        return _collectedItem == null;
    }

    public bool HasItem(string itemType)
    {
        return _collectedItem == itemType;
    }

    public void RemoveItem()
    {
        _collectedItem = null;
        _ui.ClearItem();
    
    }

    private void DropItem()
    {

       
        var collectible = Collectible.CreateCollectible(_collectedItem);
        collectible.Position = Position;

        GetParent().AddChild(collectible);
        collectible.PlayDropSound();
        RemoveItem();




    }
}
