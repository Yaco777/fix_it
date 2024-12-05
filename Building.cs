using System;
using System.Linq;
using Godot;
using static Employee;

public partial class Building : Node2D
{


    private static Node2D _itemsGenerationsArea;
    private static Random random = new Random();

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

    

    public void AddEmployeeToCheckForStateChange(Employee emp)
    {
        emp.EmployeeStateChanged += OnEmployeeStateChanged; 
    }

    private void OnEmployeeStateChanged(int newState, string n)
    {

   

        if (n == "Musicien" && (EmployeeState)newState == EmployeeState.NotWorking)
        {
          

            // we create a collectible
            var collectible = Collectible.CreateCollectible("Horn");
            // we select a random area
            collectible.GlobalPosition = GetRandomPositionForItem();
            // we add the collectible
            GetTree().Root.GetChild(0).AddChild(collectible);
        }
        else if (n == "Painter" && (EmployeeState)newState == EmployeeState.NotWorking)
        {
            foreach (var brushColors in Painter.ColorsMissings)
            {

                var color = brushColors;
                var itemName = color;
                var collectible = Collectible.CreateCollectible(itemName);
                collectible.GlobalPosition = GetRandomPositionForItem();
                GD.Print("La position des brush est : " + collectible.GlobalPosition);
                GetTree().Root.GetChild(0).CallDeferred("add_child", collectible);
            }



        }

    }

    public static Vector2 GetRandomPositionForItemForSpecificArea(Area2D selectedArea)
    {
        
        var collisionShape = selectedArea.GetNode<CollisionShape2D>("CollisionShape2D");
        if (collisionShape != null)
        {

            if (collisionShape.Shape is SegmentShape2D segmentShape)
            {
                float t = GD.Randf(); //t will be a number between 0 and 1
                var xPos = segmentShape.A.X + (segmentShape.B.X - segmentShape.A.X) * t;
                // we compute the xPos of the collectible. The Y pos will be the same than the selectedArea
                return new Vector2(xPos, selectedArea.GlobalPosition.Y);
            }

            throw new InvalidOperationException("The collision shape of an area used to generate items is invalid");
        }
        return new Vector2(0, 0);
    }

    private static Vector2 GetRandomPositionForItem()
    {
        var areas = _itemsGenerationsArea.GetChildren().OfType<Area2D>().ToArray();
        if (areas.Length > 0)
        {
            var selectedArea = areas[random.Next(areas.Length)];


            return GetRandomPositionForItemForSpecificArea(selectedArea);
        }

        throw new InvalidOperationException("There is no area2D in the area used to genereate items");

    }

    public static Area2D GetRandomArea2D()
    {
        var areas = _itemsGenerationsArea.GetChildren().OfType<Area2D>().ToArray();
        return areas[random.Next(areas.Length)];
    }

}
