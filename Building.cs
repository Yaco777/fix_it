using Godot;
using System;
using System.Linq;
using static Employee;

public partial class Building : Node2D
{


    private Node2D _itemsGenerationsArea;
    private Random random = new Random();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        var employees = GetNode<Node2D>("../Employees").GetChildren();
        foreach (var emp in employees)
        {
            if (emp is Employee)
            {
                var empCast = emp as Employee;
                empCast.EmployeeStateChanged += OnEmployeeStateChanged;
            }

        }

        _itemsGenerationsArea = GetNode<Node2D>("ItemsGenerationArea");

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void OnEmployeeStateChanged(int newState, string n)
    {
        
        GD.Print("Hey (building) ! Changement : " + (Employee.EmployeeState)newState + " avec n = " + n);
        if (n == "Musicien" && (EmployeeState)newState == EmployeeState.NotWorking)
        {
            var collectibleScene = GD.Load<PackedScene>("res://collectible.tscn");
        
            // we create a collectible
            var collectible = collectibleScene.Instantiate<Collectible>();
            collectible.CollectibleName = "Horn";
            collectible.ObjectTexture = (Texture2D)GD.Load("res://building/collectible/horn.png");
            collectible.PickUpSound = (AudioStream)GD.Load("res://audio/collectible/horn_pickup.mp3");


            // we select a random area
            var areas = _itemsGenerationsArea.GetChildren().OfType<Area2D>().ToArray();
            if (areas.Length > 0)
            {
                var selectedArea = areas[random.Next(areas.Length)];

               
                var collisionShape = selectedArea.GetNode<CollisionShape2D>("CollisionShape2D");
                if (collisionShape != null)
                {
                    
                    if (collisionShape.Shape is SegmentShape2D segmentShape)
                    {
                        
                        float t = (float)GD.Randf(); //t will be a number between 0 and 1
                        var xPos = segmentShape.A.X + (segmentShape.B.X - segmentShape.A.X) * t;
                        // we compute the xPos of the collectible. The Y pos will be the same than the selectedArea
                        
                        collectible.GlobalPosition = new Vector2(xPos,selectedArea.GlobalPosition.Y);
                    }
                    else
                    {
                        throw new InvalidOperationException("The collision shape of an area used to generate items is invalid");
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("There is no area2D in the area used to genereate items");
            }

            // we add the collectible
            collectible.ZIndex = 1;
            GetTree().Root.GetChild(0).AddChild(collectible);
           
            
        }
    }

}
