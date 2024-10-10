using Godot;
using System;
using System.Reflection.Emit;

public partial class Collectible : Area2D
    /**
    Represent a collectible object. Every collectible have an unique name
    */
{

    [Export] public Texture2D ObjectTexture { get; set; } //the texture of the collectible (the sprite)
    [Export] public string Name { get; set; } //the unique name of the collectible

    private bool _playerInRange = false; //bool used to check if the player can take the object
    private RichTextLabel _pickLabel; //label that will be shown when the player can pick the item
    private RichTextLabel _cannotPickLabel; //label that will be shown when the player cannont pick the item



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<Sprite2D>("CollectibleSprite").Texture = ObjectTexture;
        _pickLabel = GetNode<RichTextLabel>("CollectDisplay");
        _cannotPickLabel = GetNode<RichTextLabel>("CannotPick");
        //we hide all the labels
        _pickLabel.Visible = false;
        _cannotPickLabel.Visible = false;
        _pickLabel.Text = _pickLabel.Text.Replace("{item_name}", Name); //replace the placeholder with the name of the item
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is Hero)  
        {
            _playerInRange = true;  //now the hero can pick the item
            UpdateDisplay(body as Hero);

        }
    }

    public void OnBodyExited(Node2D body)
    {
        if (body is Hero)  
        {
            _playerInRange = false; //the hero cannont pick the item
            UpdateDisplay(body as Hero);
        }
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        if (_playerInRange && Input.IsActionJustPressed("interact_with_objects"))
        {

            Hero hero = GetParent().GetNode<Hero>("Hero");
            UpdateDisplay(hero);

            //if the hero can pick the item, we remove the item
            if (hero.CanPickItem())
            {
                _pickLabel.Visible = false;
                _cannotPickLabel.Visible = false;
                hero.CollectItem(Name); 
                QueueFree(); //remove the object
            }

        }

    }

    private void UpdateDisplay(Hero hero)
        /**
        Update the two labels according to the playerInRange variable
        */
    {
        
        if (_playerInRange) 
        {
            if (hero.CanPickItem())
            {
                _pickLabel.Visible = true; 
                _cannotPickLabel.Visible = false; 
            }
            else
            {
                _cannotPickLabel.Visible = true;
                _pickLabel.Visible = false;
            }
        }
        else
        {
            //if the player isn't in range, we hide the two labels
            _cannotPickLabel.Visible = false;
            _pickLabel.Visible = false;
        }
    }
}
