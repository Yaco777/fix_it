using Godot;
using System;

public partial class GameOverLabel : Label
{
    private ShaderMaterial _shaderMaterial;
    [Export] public float waitTime = 3.0f;
    private bool goingDown = true;
    private float currentAmount = 1000f;
    private float targetAmount;
    private double elapsedTime = 0.0;
    private float lerpSpeed = 5f;

    public override void _Ready()
    {
        _shaderMaterial = Material as ShaderMaterial;
        targetAmount = currentAmount;
       
    }

    public override void _Process(double delta)
    {
        elapsedTime += delta;

        if (elapsedTime >= waitTime)
        {
            if (goingDown)
            {
                targetAmount = 50f;
                goingDown = false;
            }
            else
            {
                targetAmount = 1000f;
                goingDown = true;
            }

            elapsedTime = 0.0;
        }

        // Continue interpolating even when close to target to avoid sudden jumps
        currentAmount = Mathf.Lerp(currentAmount, targetAmount, (float)(delta * lerpSpeed));

        // Apply the interpolated value to the shader
        _shaderMaterial.SetShaderParameter("amount", currentAmount);

        // Ensure we don't overshoot the target
        if (Mathf.Abs(currentAmount - targetAmount) < 0.1f)
        {
            currentAmount = targetAmount;
        }
    }
}
