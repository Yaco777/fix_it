using Godot;

public partial class SecurityWarningRectangle : TextureRect
{
    [Export]
    public int TriangleSize { get; set; } = 150;

    [Export]
    public float DefaultAlphaSpeed { get; set; } = 0.3f; // Speed of scaling

    [Export]
    private float AlphaSpeed { get; set; }

    [Export]
    public float AlphaSpeedIncrease { get; set; } = 0.001f;

    [Export]
    public float MaxAlphaSpeed { get; set; } = 2f;

    [Export]
    public float MinAlpha { get; set; } = 0.4f; // Minimum scale size

    [Export]
    public float MaxAlpha { get; set; } = 1.0f; // Maximum scale size

    private float triangleAlpha = 1.0f;
    private bool AlphaUp = true; // Direction of scaling

    private GlobalSignals _globalSignals;

    public override void _Ready()
    {
        Vector2 size = GetViewportRect().Size;

        CreateCornerTriangle(new Vector2(0, 0), new Vector2(TriangleSize, 0), new Vector2(0, TriangleSize));
        CreateCornerTriangle(new Vector2(size.X, 0), new Vector2(size.X - TriangleSize, 0), new Vector2(size.X, TriangleSize));
        CreateCornerTriangle(new Vector2(0, Size.Y), new Vector2(0, Size.Y - TriangleSize), new Vector2(TriangleSize, Size.Y));
        CreateCornerTriangle(new Vector2(size.X, Size.Y), new Vector2(size.X - TriangleSize, Size.Y), new Vector2(size.X, Size.Y - TriangleSize));
        AlphaSpeed = DefaultAlphaSpeed;

        _globalSignals = GetNode<GlobalSignals>("../../../../GlobalSignals");
        _globalSignals.AlarmStateChanged += ShowWarning;

    
        
    }

    public void ShowWarning(bool newAlarmState)
    {
        
        if(newAlarmState)
        {
            Visible = true;
        }
        else
        {
            
            Visible = false;
            AlphaSpeed = DefaultAlphaSpeed;
        }
    }

    private void CreateCornerTriangle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        var triangle = new Polygon2D();
        triangle.Polygon = new[] { point1, point2, point3 };
        triangle.Color = new Color(1, 0, 0);
        AddChild(triangle);
    }

    public override void _Process(double delta)
    {
        
        // Update alpha based on time
        if (AlphaUp && Visible)
        {
            triangleAlpha += (float)(AlphaSpeed * delta);
            if (triangleAlpha >= MaxAlpha)
            {
                triangleAlpha = MaxAlpha;
                AlphaUp = false;
            }
        }
        else
        {
            triangleAlpha -= (float)(AlphaSpeed * delta);
            if (triangleAlpha <= MinAlpha)
            {
                triangleAlpha = MinAlpha;
                AlphaUp = true;
            }
        }

        
        // Apply the color change to all child triangles
        foreach (var child in GetChildren())
        {
            if (child is Polygon2D triangle)
            {
                triangle.Color = new Color(triangle.Color.R, triangle.Color.G, triangle.Color.B, triangleAlpha);
            }
        }

        AlphaSpeed += AlphaSpeedIncrease;
        if(AlphaSpeed >= MaxAlphaSpeed)
        {
            AlphaSpeed = MaxAlphaSpeed;
        }
    }
}
