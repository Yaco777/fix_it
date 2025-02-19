using System;
using Godot;

public partial class CanvasWithShader : CanvasLayer
{
    private ShaderMaterial shaderMaterial;
    private ColorRect colorRect;

    [Export]
    private float transitionSpeed = 1.0f;  // Speed of the transition

    private float redTransition;
    private float greenTransition;
    private float blueTransition;
    private float bwTransition = 1.0f;    // Start with black and white (1.0 means fully black and white)

    // By default, the game will be in black and white
    private bool redEnabled;
    private bool greenEnabled;
    private bool blueEnabled;

    [Export]
    private float minColorValueWhenOnlyShowingBlue = 0.8f;


    public override void _Ready()
    {
        // Initialize the ShaderMaterial
        colorRect = GetNode<ColorRect>("RectangleWithShader");
        shaderMaterial = colorRect.Material as ShaderMaterial;

        if (shaderMaterial == null)
        {
            GD.PrintErr("ShaderMaterial not found on the ColorRect!");
        }

        // Start with black and white
        shaderMaterial.SetShaderParameter("mode", 0);
        UpdateShader();

        // Connect signals
        var globalSignals = GetNode<GlobalSignals>("/root/Main/GlobalSignals");
        if (globalSignals != null)
        {
            globalSignals.ColorLost += OnColorLost;
            globalSignals.ColorBack += OnColorBack;
        }
    }

    public override void _Process(double delta)
    {
        HandleColorTransitions(delta);

        // Update the shader mode based on enabled colors
        if (!greenEnabled && !redEnabled && !blueEnabled)
        {
            bwTransition = Mathf.Min(bwTransition + (float)(transitionSpeed * delta), 1.0f); // Increase black and white transition
            shaderMaterial.SetShaderParameter("mode", 0); // Black and white mode
        }
        else
        {
            bwTransition = Mathf.Max(bwTransition - (float)(transitionSpeed * delta), 0.0f); // Decrease black and white transition
            shaderMaterial.SetShaderParameter("mode", 1); // Color mode
        }

        UpdateShader(); // Update the shader with the latest values
    }

    private void HandleColorTransitions(double delta)
    {

        //when we only show the blue color, we add a little bit of the other colors
        if (blueEnabled && redEnabled == false && greenEnabled == false)
        {
            if(redTransition > minColorValueWhenOnlyShowingBlue)
            {
                redTransition -= (float)(transitionSpeed * delta);
            }
            if(greenTransition > minColorValueWhenOnlyShowingBlue)
            {
                greenTransition -= (float)(transitionSpeed * delta);
            }
            if (blueEnabled && blueTransition > 0.0f)
            {

                blueTransition -= (float)(transitionSpeed * delta);
                if (blueTransition < 0.0f) blueTransition = 0.0f;
            }

            return;
        }


        // Handle red transition
        if (redEnabled && redTransition > 0.0f)
        {
            
            redTransition -= (float)(transitionSpeed * delta);
            if (redTransition < 0.0f) redTransition = 0.0f; 
        }
        else if (!redEnabled && redTransition < 1.0f)
        {
            
            redTransition += (float)(transitionSpeed * delta);
            if (redTransition > 1.0f) redTransition = 1.0f; 
        }

        // Handle green transition
        if (greenEnabled && greenTransition > 0.0f)
        {
            
            greenTransition -= (float)(transitionSpeed * delta);
            if (greenTransition < 0.0f) greenTransition = 0.0f;
        }
        else if (!greenEnabled && greenTransition < 1.0f)
        {
            
            greenTransition += (float)(transitionSpeed * delta);
            if (greenTransition > 1.0f) greenTransition = 1.0f;
        }

        // Handle blue transition
        if (blueEnabled && blueTransition > 0.0f)
        {
            
            blueTransition -= (float)(transitionSpeed * delta);
            if (blueTransition < 0.0f) blueTransition = 0.0f; 
        }
        else if (!blueEnabled && blueTransition < 1.0f)
        {
            
            blueTransition += (float)(transitionSpeed * delta);
            if (blueTransition > 1.0f) blueTransition = 1.0f;
        }

        
    }


    public void OnColorLost(String name)
    {
        // Update the state of colors
        _ = name.ToLower() switch
        {
            "red brush" => redEnabled = false,
            "green brush" => greenEnabled = false,
            "blue brush" => blueEnabled = false,
            _ => throw new ArgumentException($"Invalid color: {name}")
        };
    }

    public void OnColorBack(string name)
    {
       

        // Reset the color states
        _ = name.ToLower() switch
        {
            "red brush" => redEnabled = true,
            "green brush" => greenEnabled = true,
            "blue brush" => blueEnabled = true,
            _ => throw new ArgumentException($"Invalid color: {name}")
        };
    }

    private void UpdateShader()
    {
        // Update the shader with the latest transition values
        shaderMaterial.SetShaderParameter("red_transition", redTransition);
        shaderMaterial.SetShaderParameter("green_transition", greenTransition);
        shaderMaterial.SetShaderParameter("blue_transition", blueTransition);
        shaderMaterial.SetShaderParameter("bw_transition", bwTransition); // Add black and white transition

  


    }
}
