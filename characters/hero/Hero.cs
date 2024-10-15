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
    private bool _canGoUp = true; //check if there is a floor at the top of the player
    private bool _canGoDown = true; //check if there is a floor below the player

    private int _actionCooldown = 0; //cooldown to prevent duplication for the items (by spamming)

    [Export]
    private int _defaultCooldown = 100;
    private AnimatedSprite2D _animatedSprite2D;


    private UI _ui;


    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        var ladders = GetNode<Node2D>("../Building/Ladders").GetChildren();

        //we add the actions for the ladder, all the ladders are in the building
        SetupLadders();
        SetupFloorsAndRoofs();
        _ui = GetNode<UI>("../UI");
        _animatedSprite2D = GetNode<AnimatedSprite2D>("HeroSprites");

    }

    private void SetupLadders()
    {
        /**
         * The hero will listen to the "ladder entered" and "ladder exited" signal to be able to climb a ladder
         */
        var ladders = GetNode<Node2D>("../Building/Ladders").GetChildren();
        foreach (var ladder in ladders)
        {
            if (ladder is Node2D ladderNode && ladderNode.HasNode("LadderArea"))
            {
                var ladderArea = ladderNode.GetNode<LadderArea>("LadderArea");
                ladderArea.LadderEntered += OnLadderAreaEntered;
                ladderArea.LadderExited += OnLadderAreaExited;
            }
        }
    }

    private void SetupFloorsAndRoofs()
    {

        /**
         * We also listen to the body signals of the floors and the roofs. They prevent the player from getting in an illegal position
         */
        var floors = GetNode<Node2D>("../Building/Floors").GetChildren();
        foreach (var floor in floors)
        {
            if (floor is Area2D floorArea)
            {

                floorArea.BodyEntered += (body) => OnFloorEntered(body, floorArea);
                floorArea.BodyExited += (body) => OnFloorExited(body, floorArea);
            }
        }

        var roofs = GetNode<Node2D>("../Building/Roofs").GetChildren();
        foreach (var roof in roofs)
        {
            if (roof is Area2D roofArea)
            {

                roofArea.BodyEntered += (body) => OnRoofEntered(body, roofArea);
                roofArea.BodyExited += (body) => OnRoofExited(body, roofArea);
            }
        }
    }

    private void OnFloorEntered(Node body, Area2D floorArea)
        /**
         * Method called when the player enter the Area2D of a floor. He won't be able to, either go up or go down (based on its position)
         */
    {
   
        if (body is Hero)
        {
            _isClimbing = false;  //we stop the climb animation
            if (floorArea != null)
            {
                _canGoDown = false; //the floor is below the player
                _canGoUp = true;
            }

        }
    }


    private void OnFloorExited(Node body, Area2D area)
        /**
         * Method called when the player isn't in the area2D of a floor
         */
    {
       
        if (body is Hero)
        {
            _canGoUp = true; //we allow the player to go up and down 
            _canGoDown = true;
           
        }
    }


    private void OnRoofEntered(Node body, Area2D floorArea)
    /**
     * Method called when the player enter the Area2D of a floor. He won't be able to, either go up or go down (based on its position)
     */
    {
   
        if (body is Hero)
        {
            _isClimbing = false;  //we stop the climb animation
            if (floorArea != null)
            {
                _canGoDown = true; //the floor is below the player
                _canGoUp = false;
            }

        }
    }
    private void OnRoofExited(Node body, Area2D floorArea)
    /**
     * Method called when the player enter the Area2D of a floor. He won't be able to, either go up or go down (based on its position)
     */
    {
     
        if (body is Hero)
        {
           
            if (floorArea != null)
            {
                _canGoDown = true; //the floor is below the player
                _canGoUp = true;
            }

        }
    }



    private Vector2 ClimbLadder(Vector2 velocity, AnimatedSprite2D animatedSprite2D)
    {
        /**
         * Method used to climb a ladder. It's possible only if the player is in the area of a ladder and the movment is legal :
         * if the player wants to go up and is in the area of a roof, we won't allow the move
         * if the play wants to go down and is in the area of a floor, we won't allow the move
         */
        var isMoving = false;
            if (Input.IsActionPressed("move_up") && _canGoUp)
            {
                if(_canGoUp)
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
                if(_canGoDown)
                {
                    velocity.Y += 1; // go down
                    isMoving = true;
                }
                else
                {
                    _isClimbing = false; //if the player wants to go down but he can't, that means that he touched the floor
                    
                }
                
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

            return velocity;
    }

    private Vector2 MoveLeftRight(Vector2 velocity, AnimatedSprite2D animatedSprite2D)
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
        return velocity;
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Vector2.Zero; 
        

     
        if(_actionCooldown > 0)
        {
            _actionCooldown--;
        }
       

        //if the player decided to climb, we update the value of is_climbing
        if(_canClimb && ((Input.IsActionJustPressed("move_up") && _canGoUp) || (_canGoDown && Input.IsActionJustPressed("move_down")))) {
            _isClimbing = true;
        }

        //if the player is climbing, he can't go on the left or the right
        if (_isClimbing)
        {
            velocity = ClimbLadder(velocity,animatedSprite2D);
        }
        else
        {
            velocity = MoveLeftRight(velocity,animatedSprite2D);
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
    }

    // when the player enter the area2D of a ladder
    private void OnLadderAreaEntered()
    {
        _canClimb = true;
    }

    // when the player get out of the area2D of a ladder
    private void OnLadderAreaExited()
    {
        _isClimbing = false; 
        _canClimb = false; //the player cannot climb anymore
   
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
            ResetCooldown();
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
        ResetCooldown();
    }

    private void DropItem()
    {

        ResetCooldown();
        var collectible = Collectible.CreateCollectible(_collectedItem);
        collectible.Position = Position;
        
        GetParent().AddChild(collectible);
        collectible.PlayDropSound();
        RemoveItem();
        



    }

    public bool CooldownIsZero()
    {
        return _actionCooldown == 0;
    }

    public void ResetCooldown()
    {
        _actionCooldown = _defaultCooldown;
    }
}
