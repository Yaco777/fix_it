using Godot;
using System;

public partial class UI : CanvasLayer
{
    private TextureRect _objectIcon;
    private RichTextLabel _objectName;

   
    private Vector2 targetScale = new Vector2(1f, 1f); //basic scale

    [Export]
    private  float ScaleSpeed = 4.0f; // transition speed

    [Export]
    private  float animationScaleX = 2f;

    [Export]
    private  float animationScaleY = 2f;

   
   

    public override void _Ready()
    {
        _objectIcon = GetNode<TextureRect>("ObjectIcon");
        _objectName = GetNode<RichTextLabel>("ObjectName");
        _objectName.BbcodeEnabled = true;
        _objectIcon.Visible = false;
        _objectName.AddThemeFontSizeOverride("normal_font_size", 32);
        SetEmptyInventoryLabel();
    }

    public override void _Process(double delta)
    {
        //we change the scale of the item
        _objectIcon.Scale = _objectIcon.Scale.Lerp(targetScale, (float)(ScaleSpeed * delta));
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
}
