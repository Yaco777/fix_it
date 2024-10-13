using Godot;

public partial class Notes : Node2D
{
    private Vector2 _direction;
    private float _speed = 100f; // Vitesse de déplacement
    private float _lifetime = 3f; // Durée de vie de la note
    private Color _initialColor;
    private Sprite2D _sprite;

    

    public void SetDirection(Vector2 direction)
    {
        _direction = direction.Normalized(); // Normaliser la direction pour qu'elle soit unitaire
        StartLifeCycle(); // Démarrer le cycle de vie de la note
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
    }

    public override void _Process(double delta)
    {
        // Déplacer la note dans sa direction
        Position += _direction * _speed * (float)delta;
        // TODO
        float elapsed = _lifetime - (float)GetTree().CurrentScene.GetNode<Notes>("Notes").GetChildCount() * (float)delta;
        if (elapsed > 0)
        {
            // Calculer la nouvelle transparence
            float newAlpha = Mathf.Clamp(_initialColor.A * (elapsed / _lifetime), 0f, 1f);
            _sprite.Modulate = new Color(_initialColor.R, _initialColor.G, _initialColor.B, newAlpha);
        }
    }

    private async void StartLifeCycle()
    {
        await ToSignal(GetTree().CreateTimer(_lifetime), "timeout");
        QueueFree(); // Supprime la note après sa durée de vie
    }
}
