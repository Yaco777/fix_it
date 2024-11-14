using Godot;

public partial class UI : CanvasLayer
{
    private TextureRect _objectIcon; //icon of the item in the inventory
    private RichTextLabel _objectName; //name of the item in the inventory


    private Vector2 targetScale = new Vector2(1f, 1f); //basic scale

    [Export]
    private float ScaleSpeed = 4.0f; // transition speed

    [Export]
    private float animationScaleX = 2f; 

    [Export]
    private float animationScaleY = 2f;

    private GlobalSignals _globalSignals;


    private ColorRect _dialogRect;

    private Label _dialogLabel;

    private Label _glassesWear;

    [Export]
    public string NotWearningGlasses { get; set; } = "Not wearing";

    [Export]
    public string WearingGlasses { get; set; } = "Wearing !";

    [Export]
    public string GlassesUnlockedMessage { get; set; } = "You unlocked the glasses! Press R to equip them";

    [Export]
    public string GhostMessage { get; set; } = "There are ghost everywhere! Press R again to remove the glasses. Try to fight against the ghosts!";

    private enum State //this state is used to know if we need to show the message asking the player to wear the glasses
    {
        GLASSES_NOT_UNLOCKED,
        SHOW_GLASSES_UNLOCKED_MESSAGE,
        GHOST_MESSAGE,
        HIDE_GLASSES_UNLOCKED_MESSAGE,

    }

    private State _state = State.GLASSES_NOT_UNLOCKED;


    public override void _Ready()
    {
        _objectIcon = GetNode<TextureRect>("ObjectIcon");
        _objectName = GetNode<RichTextLabel>("ObjectName");
        _objectName.BbcodeEnabled = true;
        _objectIcon.Visible = false;
        _objectName.AddThemeFontSizeOverride("normal_font_size", 32);
        _globalSignals = GetNode<GlobalSignals>("../GlobalSignals");
        SetEmptyInventoryLabel();

        //the glasses
        _globalSignals.UnlockGlasses += GlassesUnlocked;
        _dialogRect = GetNode<ColorRect>("DialogRect");
        _dialogLabel = _dialogRect.GetNode<Label>("DialogLabel");
        _glassesWear = GetNode<Label>("GlassesWear");
        _glassesWear.Visible = false;
        _dialogRect.Visible = false;
        _globalSignals.GlassesChange += UpdateGlassesLabel;

        
    }

    public override void _Process(double delta)
    {
        //we change the scale of the item
        _objectIcon.Scale = _objectIcon.Scale.Lerp(targetScale, (float)(ScaleSpeed * delta));

        //we update the message that will be displayed if the player has unlocked the glasses
        if(_state != State.GLASSES_NOT_UNLOCKED)
        {
            if(Input.IsActionJustPressed("wear_glasses"))
            {
                
                if(_state == State.GHOST_MESSAGE)
                {
                    _state = State.HIDE_GLASSES_UNLOCKED_MESSAGE;
                    _dialogRect.Visible = false;
                   
                }
                else if (_state == State.SHOW_GLASSES_UNLOCKED_MESSAGE)
                {
                    _state = State.GHOST_MESSAGE;
                    _dialogLabel.Text = GhostMessage;

                }



            }
        }
        
    }

    public void SetEmptyInventoryLabel()
    {
        _objectName.Text = "";
    }

    public void UpdateCollectedItem(string itemName)
    {
        var texture = Collectible.getTextureOfCollectible(itemName);

        _objectIcon.Texture = texture;
        _objectIcon.Scale = new Vector2(animationScaleX, animationScaleY); //base scale of the item, the size will decrease
        _objectName.Text = "[center][color=yellow]" + itemName + "[/color][/center]";
        _objectIcon.Visible = true;
        _objectName.Visible = true;
    }

    public void ClearItem()
    {
        _objectIcon.Visible = false;
        _objectName.Visible = false;
    }

    private void GlassesUnlocked()
    {
        
        if(_state == State.GLASSES_NOT_UNLOCKED)
        {
            GD.Print("on debloque les glasses !!");
            _state = State.SHOW_GLASSES_UNLOCKED_MESSAGE;
            _dialogRect.Visible = true;
            _glassesWear.Visible = true;
            _dialogLabel.Text = GlassesUnlockedMessage;
        }
        

    }

    private void UpdateGlassesLabel(bool isWearingGlasses)
    {
        if(isWearingGlasses)
        {
            _glassesWear.Text = WearingGlasses;
        }
        else
        {
            _glassesWear.Text = NotWearningGlasses;
        }
    }

   
}
