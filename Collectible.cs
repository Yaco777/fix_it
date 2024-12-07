using System;
using Godot;

public partial class Collectible : Area2D
/**
Represent a collectible object. Every collectible have an unique name
*/
{

	[Export] public Texture2D ObjectTexture { get; set; } //the texture of the collectible (the sprite)
	[Export] public string CollectibleName { get; set; } //the unique name of the collectible

	[Export] public AudioStream PickUpSound { get; set; } //sound when the player pick the up

	[Export] public AudioStream DropSound { get; set; }  //sound when the player drop the item

	private bool _playerInRange; //bool used to check if the player can take the object
	private AudioStreamPlayer _audioPlayer;
	private AudioStreamPlayer _dropSoundPlayer;
	private Sprite2D _sprite;
	private AnimatedSprite2D _collectAnimation;
	private AnimatedSprite2D _frogAnimation;
	private bool _isRemoving; //tell if the object should not be visible


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{


		//we assign the texture
		_sprite = GetNode<Sprite2D>("CollectibleSprite");
		_sprite.Texture = ObjectTexture;
		_collectAnimation = GetNode<AnimatedSprite2D>("CollectAnimation");
		_collectAnimation.Visible = false;
		// _frogAnimation = GetNode<AnimatedSprite2D>("FrogSprites");

		//we assign the sound
		_audioPlayer = GetNode<AudioStreamPlayer>("PickupSound");
		_audioPlayer.Stream = PickUpSound;
		_audioPlayer.Finished += OnAudioFinished; //the onAudioFinished method is used to free the item


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

		if (_playerInRange)
		{
			if(_isRemoving)
			{
				return;
			}
			Hero hero = GetParent().GetNode<Hero>("Hero");
			UpdateDisplay(hero);

			//if the hero can pick the item, we play the pickupsound and hide the sprite
			if (hero.CollectedItemIsNull() && hero.CooldownIsZero() && Input.IsActionJustPressed("interact_with_objects"))
			{
				_collectAnimation.Visible = false;
				hero.CollectItem(CollectibleName);
				_audioPlayer.Play();
				_sprite.Visible = false;
				_isRemoving = true;
				



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
			_collectAnimation.Visible = true;
			if (hero.CollectedItemIsNull())
			{

				_collectAnimation.Animation = "can_interact";

			}
			else
			{
				_collectAnimation.Animation = "cannot_interact";
			}
			_collectAnimation.Play();
		}
		else
		{
			//if the player isn't in range, we show hide the collect animation
			_collectAnimation.Visible = false;
			
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
			"Ingredient" => (AudioStream)GD.Load("res://audio/collectible/paper.mp3"),
			_ => null
		};


		collectible.ObjectTexture = texture;
		collectible.PickUpSound = pickUpSound;
		collectible.CollectibleName = nameOfTheObject;
		collectible.Name = nameOfTheObject;
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
		return nameOfTheObject switch
		{

			"Red brush" => (Texture2D)GD.Load("res://building/collectible/red_brush.png"),
			"Blue brush" => (Texture2D)GD.Load("res://building/collectible/blue_brush.png"),
			"Green brush" => (Texture2D)GD.Load("res://building/collectible/green_brush.png"),
			"Horn" => (Texture2D)GD.Load("res://building/collectible/horn.png"),
			"Frog" => (Texture2D)GD.Load("res://building/collectible/Grenouille01.png"),
			"Ingredient" => (Texture2D)GD.Load("res://building/collectible/PaperCook.png"),
			_ => throw new ArgumentException("The name of the object is wrong (for applying the texture) " +
											 nameOfTheObject)
		};
	  
	}

	public void HideSprite()
	{
		_sprite.Visible = false;
	}
}
