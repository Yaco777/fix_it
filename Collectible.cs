using Godot;
using System;
using System.Reflection.Emit;

public partial class Collectible : Area2D
/**
Represent a collectible object. Every collectible have an unique name
*/
{

    [Export] public Texture2D ObjectTexture { get; set; } //the texture of the collectible (the sprite)
    [Export] public string CollectibleName { get; set; } //the unique name of the collectible

    [Export] public AudioStream PickUpSound { get; set; } //sound when the player pick the up

    [Export] public AudioStream DropSound { get; set; }  //sound when the player drop the item

    private bool _playerInRange = false; //bool used to check if the player can take the object
    private RichTextLabel _pickLabel; //label that will be shown when the player can pick the item
    private RichTextLabel _cannotPickLabel; //label that will be shown when the player cannont pick the item
    private AudioStreamPlayer _audioPlayer;
    private AudioStreamPlayer _dropSoundPlayer;
    private Sprite2D _sprite;



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {


        //we assign the texture
        _sprite = GetNode<Sprite2D>("CollectibleSprite");
        _sprite.Texture = ObjectTexture;


        //we assign the sound
        _audioPlayer = GetNode<AudioStreamPlayer>("PickupSound");
        _audioPlayer.Stream = PickUpSound;
        _audioPlayer.Finished += OnAudioFinished; //the onAudioFinished method is used to free the item

        _pickLabel = GetNode<RichTextLabel>("CollectDisplay");
        _cannotPickLabel = GetNode<RichTextLabel>("CannotPick");
        //we hide all the labels
        _pickLabel.Visible = false;
        _cannotPickLabel.Visible = false;
        if (CollectibleName == "Frog")
        {
            _pickLabel.Text = "[center][color=red] Press [b]E[/b] to scare away: {item_name} [/color][/center]";
        }
        _pickLabel.Text = _pickLabel.Text.Replace("{item_name}", CollectibleName); //replace the placeholder with the name of the item
        _dropSoundPlayer = GetNode<AudioStreamPlayer>("DropSound");

        

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

            //if the hero can pick the item, we play the pickupsound and hide the sprite
            if (hero.CollectedItemIsNull() && hero.CooldownIsZero())
            {
                _pickLabel.Visible = false;
                _cannotPickLabel.Visible = false;
                hero.CollectItem(CollectibleName);
                _audioPlayer.Play();
                _sprite.Visible = false;



            }

        }

    }

    private void OnAudioFinished()
    {
        QueueFree(); // we will delete the object when the pickup item sound will be done
    }

    private void UpdateDisplay(Hero hero)
    /**
    Update the two labels according to the playerInRange variable
    */
    {

        if (_playerInRange)
        {
            if (hero.CollectedItemIsNull())
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

    public static Collectible CreateCollectible(string nameOfTheObject)
    {
        /**
         * Create a collectible based on the name of the object
         */

        var collectibleScene = GD.Load<PackedScene>("res://collectible.tscn");
        var collectible = (Collectible)collectibleScene.Instantiate();

        var texture = getTextureOfCollectible(nameOfTheObject);


        var pickUpSound = nameOfTheObject switch
        {
            "Horn" => (AudioStream)GD.Load("res://audio/collectible/horn_pickup.mp3"),
            "Blue brush" => (AudioStream)GD.Load("res://audio/collectible/brush_pickup.mp3"),
            "Green brush" => (AudioStream)GD.Load("res://audio/collectible/brush_pickup.mp3"),
            "Red brush" => (AudioStream)GD.Load("res://audio/collectible/brush_pickup.mp3"),
            "Frog" => (AudioStream)GD.Load("res://audio/collectible/frog.mp3"), 
            _ => null
        };


        collectible.ObjectTexture = texture;
        collectible.PickUpSound = pickUpSound;
        collectible.CollectibleName = nameOfTheObject;
        collectible.ZIndex = 1;
       

        return collectible;
    }

    public void PlayDropSound()
    {
        /**
         * Play the drop sound (the same for every object)
         */

        _dropSoundPlayer.Play();
        _dropSoundPlayer.Finished += () => _dropSoundPlayer.QueueFree();

    }


    public static Texture2D getTextureOfCollectible(string nameOfTheObject)
    {
        var a = nameOfTheObject switch
        {

            "Red brush" => (Texture2D)GD.Load("res://building/collectible/red_brush.png"),
            "Blue brush" => (Texture2D)GD.Load("res://building/collectible/blue_brush.png"),
            "Green brush" => (Texture2D)GD.Load("res://building/collectible/green_brush.png"),
            "Horn" => (Texture2D)GD.Load("res://building/collectible/horn.png"),
            "Frog" => (Texture2D)GD.Load("res://building/collectible/horn.png"), //TODO CHANGE
            _ => throw new ArgumentException("The name of the object is wrong (for applying the texture) " + nameOfTheObject)
        };
    }

}
