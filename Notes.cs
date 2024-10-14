using Godot;
using System;

public partial class Notes : Node2D
{
    private Vector2 _direction;

    [Export]
    private float _speed = 100f; // speed of the notes

    [Export]
    private float _lifetime = 3f; // lifetime of the notes
    private Color _initialColor; //color of the notes (we will change the alpha)
    private Sprite2D _sprite; //sprite of the notes

    [Export]
    private int randomOffset = 200; //offset based on the centre of the 2D node

    [Export]
    private float alphaDecrease = 0.002f; //each frame we will decrease the alpha of the sprite
    private Random random = new Random();



    public void SetDirection(Vector2 direction)
    {
        _direction = direction.Normalized();
        
        float randomOffsetX = (float)(random.NextDouble() * randomOffset - randomOffset / 2);
        float randomOffsetY = (float)(random.NextDouble() * randomOffset - randomOffset / 2);
        Position = new Vector2(Position.X + randomOffsetX, Position.Y + randomOffsetY);
        StartLifeCycle(); 
    }

    public void SetSprite(int i)
    {
        if(i == 0)
        {
            _sprite = GetNode<Sprite2D>("Note1");
        }
        else
        {
            _sprite = GetNode<Sprite2D>("Note2");
        }
        _initialColor = new Color(1, 1, 1, GD.Randf());
        _sprite.Modulate = _initialColor;
        _sprite.Rotate(random.Next(90));
        var scalingFactor = GD.Randf();
        var basicScaleX = _sprite.Scale.X;
        var basicScaleY = _sprite.Scale.Y;
        _sprite.Scale = new Vector2(scalingFactor * basicScaleX, scalingFactor * basicScaleY);
    }

    public override void _Process(double delta)
    {
        
        if (_sprite != null)
        {
            Position += _direction * _speed * (float)delta;
            _initialColor = new Color(1, 1, 1, _initialColor.A - alphaDecrease);
            _sprite.Modulate = _initialColor;
        }
    }

    private async void StartLifeCycle()
    {
        await ToSignal(GetTree().CreateTimer(_lifetime), "timeout");
        QueueFree(); // Supprime la note après sa durée de vie
    }
}
