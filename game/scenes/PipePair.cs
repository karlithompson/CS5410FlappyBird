using Godot;

public partial class PipePair : Node2D
{
    [Export] public float Speed = 200f;
    [Export] public float Gap = 20f;

    private Area2D _top;
    private Area2D _bottom;
    private CollisionShape2D _topShape;
    private CollisionShape2D _bottomShape;
	
	private Sprite2D _topCap;
    private Sprite2D _topBody;

    private Sprite2D _bottomCap;
    private Sprite2D _bottomBody;

	public override void _Ready()
	{
		_top = GetNode<Area2D>("TopPipe");
		_bottom = GetNode<Area2D>("BottomPipe");

		_topShape = _top.GetNode<CollisionShape2D>("CollisionShape2D");
		_bottomShape = _bottom.GetNode<CollisionShape2D>("CollisionShape2D");
		ApplyGap();
		
	}

	public float GetPipeHalfHeight()
    {
        var rect = _topShape.Shape as RectangleShape2D;
        return rect.Size.Y / 2f;
    }
	
	public void ApplyGap()
    {
        float halfH = GetPipeHalfHeight();

        float topCenterLocalY = -(Gap / 2f + halfH);
        float bottomCenterLocalY = +(Gap / 2f + halfH);

        _top.Position = new Vector2(0, topCenterLocalY);
        _bottom.Position = new Vector2(0, bottomCenterLocalY);
    }

    public override void _Process(double delta)
	{
		Position += new Vector2(-Speed * (float)delta, 0);

		if (GlobalPosition.X < -600)
			QueueFree();
	}
}
