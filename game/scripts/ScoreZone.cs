using Godot;

public partial class ScoreZone : Area2D
{
	private bool scored = false;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		
	}

	private void OnAreaEntered(Area2D area)
	{
		if (scored)
			return;

		if (area.IsInGroup("player"))
		{
			scored = true;
			Monitoring = false;
			GetTree().CallGroup("score", "AddPoint");
		}
	}
}
