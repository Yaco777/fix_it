using Godot;

public partial class WarningLabel : Label
{

    private float speed = 0.001f;

    private float offset;

    private ShaderMaterial _shaderMaterial;

    public override void _Ready()
    {
        _shaderMaterial = Material as ShaderMaterial;
    }
    public override void _Process(double delta)
    {
        offset += speed;
        _shaderMaterial.SetShaderParameter("offset", offset);
    }
}
