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


    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        var ladders = GetNode<Node2D>("../Building/Ladders").GetChildren();

        //we add the actions for the ladder, all the ladders are in the building
        foreach (var ladder in ladders)
        {
            if (ladder is Node2D ladderNode && ladderNode.HasNode("LadderArea"))
            {
                var ladderArea = ladderNode.GetNode<LadderArea>("LadderArea");
                ladderArea.LadderEntered += OnLadderAreaEntered;
                ladderArea.LadderExited += OnLadderAreaExited;
            }
        }

        //we do the same for the floors
        var floors = GetNode<Node2D>("../Building/Floors").GetChildren();
        foreach (var floor in floors)
        {
            if (floor is Area2D floorArea)
            {
                floorArea.BodyEntered += (body) => OnFloorEntered(body, floorArea);
                floorArea.BodyExited += (body) => OnFloorExited(body, floorArea);
            }
        }

        //test
        var employee = GetNode<Employee>("../Musicien");
        employee.EmployeeStateChanged += OnEmployeeStateChanged;

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
              
                if (Position.Y < floorArea.Position.Y)
                {
                    _canGoUp = false; // the floor is on top of the player
                }
                else
                {
                    _canGoDown = false; //the floor is below the player
                   
                }
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

    private void OnEmployeeStateChanged(int newState)
    {
        //Test
        GD.Print("Hey ! Changement : "+ (Employee.EmployeeState)newState);
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
        _isClimbing = false; 
        _canClimb = false; //the player cannot climb anymore
    }


    public void CollectItem(string itemType)
    {
        /**
         * Method used to collect an item. This method doesn't check that the inventory is empty! 
         * throw InvalidOperationException if you try to collect an item even if your inventory is full
         */
        if (_collectedItem == null)
        {
            _collectedItem = itemType;  //we collect the item
            GD.Print("Objet collecté : " + itemType);
        }
        else
        {
            GD.Print("Vous avez déjà un objet : " + _collectedItem);
            //this exception should never be thrown
            throw new InvalidOperationException("You are trying to collect an item but the inventory is full!");
        }
    }


    public bool CanPickItem()
    {
        //check if it's possible to pick an item (the inventory is empty)
        return _collectedItem == null;
    }
}
