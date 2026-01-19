using Godot;
using System;

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

	private Vector2 halfSize;
	[Export] public float MaxUpTiltDeg = -25f;    // nose up
	[Export] public float MaxDownTiltDeg = 25;  // nose down
	[Export] public float TiltSpeed = 10f;        // how fast it rotates to the target

	[Export] public float UpSpeedForMaxTilt = 350f;    // how fast upward to get full up tilt
	[Export] public float DownSpeedForMaxTilt = 600f;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("new_animation");

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
						GetTree().CallGroup("spawners", "StartSpawning");
					}
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

		if (gp.Y > cameraBottom)
		{
			gp.Y = cameraBottom;

			if (!dead)
			{
				Die();
			}
			else
			{
				velocity.Y = 0;
			}
		}

		float v = Mathf.Clamp(velocity.Y, -UpSpeedForMaxTilt, DownSpeedForMaxTilt);
		float alpha = Mathf.InverseLerp(-UpSpeedForMaxTilt, DownSpeedForMaxTilt, v);

		float targetDeg = Mathf.Lerp(MaxUpTiltDeg, MaxDownTiltDeg, alpha);
		float targetRad = Mathf.DegToRad(targetDeg);

		sprite.Rotation = Mathf.LerpAngle(sprite.Rotation, targetRad, TiltSpeed * dt);

		GlobalPosition = gp;

		QueueRedraw();
	}

	public void OnAreaEntered(Area2D area)
	{
		if (dead) return;
		GD.Print("GAME OVER" + area.Name);
		if (area.IsInGroup("pipe"))
		{
			GD.Print("Hit pipe!");
			Die();
		}
	}

	private void Die()
	{
		dead = true;
		GetTree().CallGroup("spawners", "StopSpawning");
		GetTree().Paused = true;
	}
}
