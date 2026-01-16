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
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;

		if (Input.IsActionJustPressed("flap"))
		{
			velocity.Y = flapStrength;
		}

		velocity.Y += gravity * dt;
		Position += velocity * dt;

		Camera2D camera = GetViewport().GetCamera2D();
		Vector2 viewSize = GetViewport().GetVisibleRect().Size;

		float cameraTop = camera.GlobalPosition.Y - viewSize.Y / 2f + halfSize.Y;
		float cameraBottom = camera.GlobalPosition.Y + viewSize.Y / 2f - halfSize.Y;

		Vector2 gp = GlobalPosition;

		// Ceiling: clamp only
		if (gp.Y < cameraTop)
		{
			gp.Y = cameraTop;
			velocity.Y = 0;
		}

		// Bottom: clamp for now (later you’ll "die" here)
		if (gp.Y > cameraBottom)
		{
			gp.Y = cameraBottom;
			velocity.Y = 0;
		}

		// Map velocity.Y from [-UpSpeedForMaxTilt, +DownSpeedForMaxTilt] -> [0..1]
		float v = Mathf.Clamp(velocity.Y, -UpSpeedForMaxTilt, DownSpeedForMaxTilt);
		float alpha = Mathf.InverseLerp(-UpSpeedForMaxTilt, DownSpeedForMaxTilt, v);

		// alpha=0 => MaxUpTiltDeg, alpha=1 => MaxDownTiltDeg
		float targetDeg = Mathf.Lerp(MaxUpTiltDeg, MaxDownTiltDeg, alpha);
		float targetRad = Mathf.DegToRad(targetDeg);

		// Smooth rotation (use sprite if you only want the bird to rotate)
		sprite.Rotation = Mathf.LerpAngle(sprite.Rotation, targetRad, TiltSpeed * dt);

		GlobalPosition = gp;

		QueueRedraw();
	}
}
