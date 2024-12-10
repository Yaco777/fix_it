using Godot;

public partial class EndGameCredits : CanvasLayer
{
    private Label _label;
    private Sprite2D _gameIcon;
    private float _alpha = 0.0f; // Alpha value for fade-in/fade-out transitions
    private float _iconAlpha = 0.0f; // Separate alpha for the image
    private bool _isFadingIn = true;
    private string _save;
    string _filePath = "user://high_score.txt";

    private float _elapsedTime = 0.0f; // Time elapsed while keeping the text visible

    [Export]
    private float FadeDuration = 2.0f;

    [Export]
    public float StayDuration { get; set; } = 3.0f;

    private float _soundFade = 1.0f;

    private float _originalSoundVolume = 0.0f;

    [Export]
    private float SoundFadeDuration = 9.0f;

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
        _gameIcon = GetNode<Sprite2D>("GameIcon");

        _label.Modulate = new Color(1, 1, 1, 0); // Initialize text alpha to 0
        _gameIcon.Modulate = new Color(1, 1, 1, 0); // Initialize icon alpha to 0
        _gameIcon.Visible = false; // Hide the icon initially
        _originalSoundVolume = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Master"));
        UpdateLabelText();
        SaveIntToFile(_save, _filePath);
    }

    public override void _Process(double delta)
    {
        HandleFade((float)delta);
        if (_state == State.SHOWING_TITLE && !_isFadingIn)
        {
            _soundFade -= (float)delta / SoundFadeDuration; // Gradually reduce volume
            if (_soundFade < 0.0f)
            {
                _soundFade = 0.0f; // Ensure it doesn't go below 0
            }
            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), LinearToDb(_soundFade));
        }
    }

    private void SaveIntToFile(string number, string path)
    {
        _save = UI._gameOverTimer.TimeLeft.ToString();
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        file.StoreString(number);
    }

    private static float LinearToDb(float linear)
    {
        return linear > 0 ? 20f * Mathf.Log(linear) / Mathf.Log(10) : -80f;
    }

    private void HandleFade(float delta)
    {
        if (_isFadingIn)
        {
            // Fade-in: Increase the alpha values
            _alpha += delta / FadeDuration;
            _iconAlpha += delta / FadeDuration;

            if (_alpha >= 1.0f)
            {
                _alpha = 1.0f;
                _iconAlpha = 1.0f;
                _isFadingIn = false; // End of fade-in
                _elapsedTime = 0.0f; // Start the stay duration
            }
        }
        else if (_elapsedTime < StayDuration)
        {
            // Keep the text and icon fully visible
            _elapsedTime += delta;
        }
        else
        {
            // Fade-out: Decrease the alpha values
            _alpha -= delta / FadeDuration;
            _iconAlpha -= delta / FadeDuration;

            if (_alpha <= 0.0f)
            {
                _alpha = 0.0f;
                _iconAlpha = 0.0f;
                _isFadingIn = true; // Prepare for the next fade-in
                SwitchState(); // Change the state after the fade-out
            }
        }

        // Apply the alpha values
        _label.Modulate = new Color(1, 1, 1, _alpha);

        // Only fade the icon during SHOWING_TITLE state
        if (_state == State.SHOWING_TITLE)
        {
            _gameIcon.Modulate = new Color(1, 1, 1, _iconAlpha);
        }
        
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
                
                break;
        }

        UpdateLabelText();
    }

    private void UpdateLabelText()
    {
        switch (_state)
        {
            case State.SHOWING_TITLE:
                StayDuration *= 2;
                FadeDuration *= 2;
                _gameIcon.Visible = true;

                _label.AddThemeFontSizeOverride("font_size", 400);
                _label.Text = ""; // Title text
                break;
            case State.SHOWING_NAMES:
                _label.AddThemeFontSizeOverride("font_size", 100);
                _label.Text = "Created by:\nAlberto UDREA\nEnora COURON\nYacine KHATIR"; // Names or credits text
                break;
            case State.SHOWING_TOOLS:
                _label.AddThemeFontSizeOverride("font_size", 60);
                _label.Text = "Tools used:\nGodot\nVisual Studio (code)\nBlender\nIllustrator\nAfter Effect";
                break;
            case State.BETA_TESTERS:
                _label.Text = "Beta testers:\n Name1\nName2\nName3\nName4";
                break;
            case State.END_CREDITS:
                _label.Text = ""; // Clear the text
                                 
                AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), _originalSoundVolume);  //put the music back
                AudioServer.SetBusVolumeDb(2, _originalSoundVolume);
                GetTree().ChangeSceneToFile("res://main_menu.tscn");
                break;
        }
    }
}
