using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Employee;

public partial class Building : Node2D
{


    private static Node2D _itemsGenerationsArea;
    private static Random random = new Random();
    private Dictionary<string, int> _employeeWorkedHashMap = new Dictionary<string, int>();
    private GlobalSignals _globalSignals;
    private bool _hasUnlockedGlasses;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var employees = GetNode<Node2D>("../Employees").GetChildren();
        _globalSignals = GetNode<GlobalSignals>("../GlobalSignals");
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

        if ((EmployeeState)newState == EmployeeState.Working)
        {
            if (_employeeWorkedHashMap.ContainsKey(n))
            {
                GD.Print("on monte pour " + n);
                _employeeWorkedHashMap[n] += 1;
            }
            else
            {
                GD.Print("premiere via " + n);
                _employeeWorkedHashMap[n] = 1;
            }
            GD.Print(_employeeWorkedHashMap);
            var shouldUnlockGlasses = true;
            if (_employeeWorkedHashMap.Count >= 6 && !_hasUnlockedGlasses)
            {
                foreach (var a in _employeeWorkedHashMap.Values)
                {
                    if (a < 3)
                    {
                        shouldUnlockGlasses = false;
                        break;
                    }
                }
                if (shouldUnlockGlasses)
                {
                    _globalSignals.EmitUnlockGlasses();
                    _hasUnlockedGlasses = true;
                }
            }
        }

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
                    GetTree().Root.GetChild(0).CallDeferred("add_child", collectible);
                }



            }

        }

        public static Vector2 GetRandomPositionForItemForSpecificArea(Area2D selectedArea, bool random)
        {

            var collisionShape = selectedArea.GetNode<CollisionShape2D>("CollisionShape2D");
            if (collisionShape != null)
            {

                if (collisionShape.Shape is SegmentShape2D segmentShape)
                {
                    float t = GD.Randf(); //t will be a number between 0 and 1
                    var xPos = 1.0f;
                    if (random)
                    {
                        xPos = selectedArea.GlobalPosition.X + (segmentShape.B.X - segmentShape.A.X) * t;
                    }
                    else
                    {
                        xPos = selectedArea.GlobalPosition.X;
                    }

                    // we compute the xPos of the collectible. The Y pos will be the same than the selectedArea
                    var vector = new Vector2(xPos, selectedArea.GlobalPosition.Y);


                    return vector;
                }

                throw new InvalidOperationException("The collision shape of an area used to generate items is invalid");
            }
            return new Vector2(0, 0);

        }

        public static Vector2 GetRandomPositionForItem()
        {
            var areas = _itemsGenerationsArea.GetChildren().OfType<Area2D>().ToArray();
            if (areas.Length > 0)
            {
                var selectedArea = areas[random.Next(areas.Length)];


                return GetRandomPositionForItemForSpecificArea(selectedArea, true);
            }

            throw new InvalidOperationException("There is no area2D in the area used to genereate items");

        }

        public static Area2D GetRandomArea2D()
        {
            var areas = _itemsGenerationsArea.GetChildren().OfType<Area2D>().ToArray();
            return areas[random.Next(areas.Length)];
        }

    }

