using Godot;
using System;
using System.ComponentModel;

public partial class Player : Node2D
{

	[Export]
	public float gravity = 900f;

	[Export]
	public float flapStrength = -350f;
	private Vector2 velocity = Vector2.Zero;

	private AnimatedSprite2D sprite;

	private Area2D box;

	private bool dead = false;

	private bool started = false;

	private bool dying = false;

	private Vector2 halfSize;
	[Export] public float MaxUpTiltDeg = -25f; 
	[Export] public float MaxDownTiltDeg = 25;
	[Export] public float TiltSpeed = 10f;  

	[Export] public float UpSpeedForMaxTilt = 350f;  
	[Export] public float DownSpeedForMaxTilt = 600f;

	public AudioStreamPlayer2D flapSound;

	public AudioStreamPlayer2D deathSound;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("new_animation");
		flapSound = GetNode<AudioStreamPlayer2D>("flapsound");
		deathSound = GetNode<AudioStreamPlayer2D>("diesound");
		halfSize = sprite.SpriteFrames.GetFrameTexture(sprite.Animation, 0).GetSize() * sprite.Scale / 2f;
		started = false;
		dead = false;
		velocity = Vector2.Zero;
		box = GetNode<Area2D>("Box");
		box.AreaEntered += OnAreaEntered;
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;

		if (!dead)
		{
			if (Input.IsActionJustPressed("flap"))
			{
				if (!started)
				{
					started = true;
					var ui = GetTree().Root.GetNode<GameOverUI>("Main/CanvasLayer/GameOver");
    				ui.HideAll();
					GetTree().CallGroup("spawners", "StartSpawning");
					GetTree().CallGroup("floor", "StartMoving");
				}
    			flapSound.Play();
				velocity.Y = flapStrength;
			}
			
		}

		if (started)
		{
			velocity.Y += gravity * dt;
		}
		Position += velocity * dt;

		Camera2D camera = GetViewport().GetCamera2D();
		Vector2 viewSize = GetViewport().GetVisibleRect().Size;

		float cameraTop = camera.GlobalPosition.Y - viewSize.Y / 2f + halfSize.Y;
		float cameraBottom = camera.GlobalPosition.Y + viewSize.Y / 2f - halfSize.Y;

		Vector2 gp = GlobalPosition;

		if (gp.Y < cameraTop)
		{
			gp.Y = cameraTop;
			velocity.Y = 0;
		}

		float floorOffset = 62f; 

		if (gp.Y > cameraBottom - floorOffset)
		{
			gp.Y = cameraBottom - floorOffset;
			velocity.Y = 0;

			if (!dead)
				Die();
		}

		if (!dead){
			
			float v = Mathf.Clamp(velocity.Y, -UpSpeedForMaxTilt, DownSpeedForMaxTilt);
			float alpha = Mathf.InverseLerp(-UpSpeedForMaxTilt, DownSpeedForMaxTilt, v);

			float targetDeg = Mathf.Lerp(MaxUpTiltDeg, MaxDownTiltDeg, alpha);
			float targetRad = Mathf.DegToRad(targetDeg);

			sprite.Rotation = Mathf.LerpAngle(sprite.Rotation, targetRad, TiltSpeed * dt);
		}

		GlobalPosition = gp;

		QueueRedraw();
	}

	public void OnAreaEntered(Area2D area)
	{
		if (dead) return;
		if (area.IsInGroup("pipe") || area.IsInGroup("floor"))
		{
			GD.Print("Hit pipe!");
			Die();
		}
	}

	private void Die()
	{
		if (dead) return;
		deathSound.Play();
		sprite.Stop();
		dead = true;
		GetTree().CallGroup("score", "ResetScore");
		GD.Print(dead);
		var ui = GetTree().Root.GetNode<GameOverUI>("Main/CanvasLayer/GameOver");
		var sm = GetTree().GetFirstNodeInGroup("score") as ScoreManager;
		ui.ShowGameOver(sm.HighScore);
		GetTree().CallGroup("spawners", "StopSpawning");
		GetTree().CallGroup("floor", "StopMoving");
		GetTree().Paused = true;
	}
}
