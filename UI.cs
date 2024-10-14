using Godot;
using System;

public partial class UI : CanvasLayer
{

	private TextureRect _objectIcon;
	private RichTextLabel _objectName;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		_objectIcon = GetNode<TextureRect>("ObjectIcon");
		_objectName = GetNode<RichTextLabel>("ObjectName");
        _objectName.BbcodeEnabled = true;
		_objectIcon.Visible = false;
        _objectName.AddThemeFontSizeOverride("normal_font_size", 32);
        setEmptyInventoryLabel();


    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void setEmptyInventoryLabel()
    {
        _objectName.Text = "";
    }

    public void UpdateCollectedItem(string itemName)
    {

        var texture = itemName switch
        {
            "Horn" => (Texture2D)GD.Load("res://building/collectible/horn.png"),
            _ => null
        };

        
        _objectIcon.Texture = texture;
        _objectIcon.Scale = new Vector2(0.1f, 0.1f);


        _objectName.Text = "[center][color=yellow]" + itemName+"[/color][/center]";

        _objectIcon.Visible = true;
        _objectName.Visible = true;
    }

   
    public void ClearItem()
    {
        _objectIcon.Visible = false;
        _objectName.Visible = false;
    }
}
