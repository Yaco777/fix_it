using Godot;
using System;

public partial class CanvasWithShader : Godot.CanvasLayer
{

    private ShaderMaterial shaderMaterial;
    private ColorRect colorRect;


    private float transitionSpeed = 1.0f;  // speed of the transition
    private bool transitioning = false;    // tell if is a transition is in progress
    private const int MAX_MODE = 3;

    public override void _Ready()
    {
        // Find the ColorRect in the CanvasLayer
        colorRect = GetNode<ColorRect>("RectangleWithShader");
        var globalSignals = GetNode<GlobalSignals>("/root/Main/GlobalSignals");
        if (globalSignals != null)
        {
            globalSignals.ColorLost += OnColorLost;
        }


        // Verify if the ColorRect has a ShaderMaterial
        shaderMaterial = colorRect.Material as ShaderMaterial;

        if (shaderMaterial == null)
        {
            GD.PrintErr("ShaderMaterial not found on the ColorRect!");
        }
    }

    public override void _Process(double delta)
    {
        //we incremente the transition value
        if (transitioning)
        {
            float transition = (float)shaderMaterial.GetShaderParameter("transition");
            transition = Mathf.Min(transition + (transitionSpeed * (float)delta), 1.0f);

            shaderMaterial.SetShaderParameter("transition", transition);

            // if the transition is complete, we reset it
            if (transition >= 1.0f)
            {
                shaderMaterial.SetShaderParameter("mode_current", shaderMaterial.GetShaderParameter("mode_target"));
                shaderMaterial.SetShaderParameter("transition", 0.0f);
                transitioning = false;
            }
        }

        // switch mode when the input is pressed
        if (Input.IsActionJustPressed("switch_camera_mode") && !transitioning)
        {
            StartTransition();
        }
    }

    private void StartTransition()
    {

        int currentMode = (int)shaderMaterial.GetShaderParameter("mode_current");
        int newMode = (currentMode + 1) % (MAX_MODE + 1);  // cycle from 0 to MAX_MODE

        // set the target mode and start the transition
        shaderMaterial.SetShaderParameter("mode_target", newMode);
        transitioning = true;
    }

    public void OnColorLost(String name)
    {
        GD.Print("connection réussi !! On perd la couleur : " + name);
    }
}
