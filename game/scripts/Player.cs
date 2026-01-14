using Godot;
using System;

public partial class Player : Node2D
{

	private float _gravity = 900f;
    private float _flapStrength = -350f;
    private Vector2 _velocity = Vector2.Zero;

	private AnimatedSprite2D _sprite;
	
	private Vector2 _halfSize;

	public override void _Ready()
	{
		GD.Print("Player ready");
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_sprite.Play("new_animation");

		_halfSize = _sprite.SpriteFrames.GetFrameTexture(_sprite.Animation, 0).GetSize() * _sprite.Scale / 2f;
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;

		_velocity.Y += _gravity * dt;
		Position += _velocity * dt;
		
		Camera2D camera = GetViewport().GetCamera2D();
        Vector2 viewSize = GetViewport().GetVisibleRect().Size;

        float cameraTop = camera.GlobalPosition.Y - viewSize.Y / 2f + _halfSize.Y;
        float cameraBottom = camera.GlobalPosition.Y + viewSize.Y / 2f - _halfSize.Y;

        Vector2 gp = GlobalPosition;

        // Ceiling: clamp only
        if (gp.Y < cameraTop)
        {
            gp.Y = cameraTop;
            _velocity.Y = 0;
        }

        // Bottom: clamp for now (later you’ll "die" here)
        if (gp.Y > cameraBottom)
        {
            gp.Y = cameraBottom;
            _velocity.Y = 0;
        }

        GlobalPosition = gp;

        QueueRedraw();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.ButtonIndex == MouseButton.Left &&
            mouseEvent.Pressed)
        {
            _velocity.Y = _flapStrength;
        }
    }

// 	public override void _Draw()
// 	{
// 		base._Draw();

// 		// DrawCircle(new Vector2(Position.X, Position.Y), 10, Colors.Red);
// 		DrawCircle(Vector2.Zero, Radius, Colors.Red);
//     }
}
