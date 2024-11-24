using Godot;

public partial class EndGameCredits : CanvasLayer
{
    private Label _label;
    private float _alpha = 0.0f; // Alpha value for fade-in/fade-out transitions
    private bool _isFadingIn = true; 

    private float _elapsedTime = 0.0f; // Time elapsed while keeping the text visible

    [Export]
    private  float FadeDuration = 2.0f;

    [Export]
    public float StayDuration { get; set; } = 3.0f;

    private enum State
    {
        SHOWING_NAMES,
        SHOWING_TOOLS,
        BETA_TESTERS,
        SHOWING_TITLE,   
        END_CREDITS    
    }

    private State _state = State.SHOWING_NAMES;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _label.Modulate = new Color(1, 1, 1, 0); //alpha = 0
        UpdateLabelText();
    }

    public override void _Process(double delta)
    {
        HandleFade((float)delta);
    }

    private void HandleFade(float delta)
    {
        if (_isFadingIn)
        {
            // Fade-in: Increase the alpha value
            _alpha += delta / FadeDuration;
            if (_alpha >= 1.0f)
            {
                _alpha = 1.0f;
                _isFadingIn = false; // End of fade-in
                _elapsedTime = 0.0f; // Start the stay duration
            }
        }
        else if (_elapsedTime < StayDuration)
        {
            // Keep the text fully visible
            _elapsedTime += delta;
        }
        else
        {
            // Fade-out: Decrease the alpha value
            _alpha -= delta / FadeDuration;
            if (_alpha <= 0.0f)
            {
                _alpha = 0.0f;
                _isFadingIn = true; // Prepare for the next fade-in
                SwitchState(); // Change the state after the fade-out
            }
        }

        // Apply the alpha value to the label's modulate property
        _label.Modulate = new Color(1, 1, 1, _alpha);
    }

    private void SwitchState()
    {
        // Transition between different states
        switch (_state)
        {
            case State.SHOWING_NAMES:
                _state = State.SHOWING_TOOLS; 
                break;
            case State.SHOWING_TOOLS:
                _state = State.BETA_TESTERS; 
                break;
            case State.BETA_TESTERS:
                _state = State.SHOWING_TITLE;
                break;
            case State.SHOWING_TITLE:
                _state = State.END_CREDITS;
                break;
            
            case State.END_CREDITS:
                GD.Print("Credits finished."); 
            
                break;
            
        }

        // we update the label text based on the new state
        UpdateLabelText();
    }

    private void UpdateLabelText()
    {
        
        switch (_state)
        {
            case State.SHOWING_TITLE:
                StayDuration *= 2;
                FadeDuration *= 2;
                _label.AddThemeFontSizeOverride("font_size", 400);
                _label.Text = "Fix it!"; // Title text
                break;
            case State.SHOWING_NAMES:
                _label.AddThemeFontSizeOverride("font_size", 100);
                _label.Text = "Created by:\nAlberto UDREA\nEnora COURON\nYacine KHATIR"; // Names or credits text
                break;
            case State.SHOWING_TOOLS:
                _label.AddThemeFontSizeOverride("font_size", 60);
                _label.Text = "Tools used:\nGodot\nVisual Studio (code)\nBlender";
                break;
            case State.BETA_TESTERS:
                _label.Text = "Beta testers:\n Name1\nName2\nName3\nName4";
                break;
            case State.END_CREDITS:
                _label.Text = ""; // Clear the text
                GetTree().ChangeSceneToFile("res://main_menu.tscn");
                break;
        }
    }
}
