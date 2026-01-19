using Godot;

public partial class PipeSpawner : Node
{
    [Export] public PackedScene PipePairScene;

    [Export] public float SpawnX = 600f;
    [Export] public float SpawnInterval = 1.5f;

    private Timer _timer;

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.WaitTime = SpawnInterval;
		_timer.Timeout += SpawnPipe;

		_timer.Stop();
		AddToGroup("spawners");
	}

	public void StartSpawning()
	{
		_timer.Start();
	}

    private void SpawnPipe()
	{
		var instanced = PipePairScene.Instantiate();
		GetParent().AddChild(instanced);

		if (instanced is not PipePair pipe)
		{
			GD.PrintErr("PipePairScene is not a PipePair. Check Inspector slot!");
			return;
		}

		Camera2D camera = GetViewport().GetCamera2D();
		Vector2 viewSize = GetViewport().GetVisibleRect().Size;

		float cameraTop = camera.GlobalPosition.Y - viewSize.Y / 2f;
		float cameraBottom = camera.GlobalPosition.Y + viewSize.Y / 2f;

		float topMargin = 120f;     // prevents gap going too high
		float bottomMargin = 200f;  // prevents gap going too low (THIS fixes “top pipe near floor” feel)

		float minCenter = cameraTop + topMargin;
		float maxCenter = cameraBottom - bottomMargin;

		float gapCenterY = (float)GD.RandRange(minCenter, maxCenter);

		pipe.GlobalPosition = new Vector2(SpawnX, gapCenterY);

		GD.Print($"Spawned pipe Gap = {pipe.Gap}");
		pipe.ApplyGap();
	}
	public void StopSpawning()
	{
		_timer.Stop();
	}
}
