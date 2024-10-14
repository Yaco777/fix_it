using Godot;
using System;

public partial class CanvasLayer : Godot.CanvasLayer
{
    // Référence au ShaderMaterial du ColorRect
    private ShaderMaterial shaderMaterial;
    private ColorRect colorRect;

    public override void _Ready()
    {
        // Trouver le ColorRect dans le CanvasLayer
        colorRect = GetNode<ColorRect>("RectangleWithShader");

        // Vérifier si le ColorRect a un ShaderMaterial
        shaderMaterial = colorRect.Material as ShaderMaterial;

        if (shaderMaterial == null)
        {
            GD.PrintErr("ShaderMaterial non trouvé sur le ColorRect !");
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("switch_camera_mode"))
        {
            var currentMode = shaderMaterial.GetShaderParameter("mode");
            if((int) currentMode == 3)
            {
                
                shaderMaterial.SetShaderParameter("mode", 0);
            }
            else
            {
                shaderMaterial.SetShaderParameter("mode", (int)currentMode+1);
            }
         
        }
     
    }
}
