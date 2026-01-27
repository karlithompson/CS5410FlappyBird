using System.Collections;
using Godot;


public partial class Floor : Node2D
{
	[Export] public float Speed = 200f;

	private float width;

	private bool started = false;

	public override void _Ready()
	{
		var sprite = GetNode<Sprite2D>("Sprite2D");
		width = sprite.Texture.GetSize().X * sprite.Scale.X;
	}

	public override void _Process(double delta)
	{
		if (!started)
			return;

		Position += Vector2.Left * Speed * (float)delta;

		if (GlobalPosition.X <= -width)
		{
			float rightmostX = GlobalPosition.X;

			foreach (Node node in GetTree().GetNodesInGroup("floor"))
			{
				if (node is Node2D f)
					rightmostX = Mathf.Max(rightmostX, f.GlobalPosition.X);
			}

			GlobalPosition = new Vector2(
				rightmostX + width,
				GlobalPosition.Y
			);
		}
	}

	public void StartMoving()
	{
		started = true;
	}

	public void StopMoving()
	{
		started = false;
	}
}
