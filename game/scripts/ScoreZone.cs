using Godot;

public partial class ScoreZone : Area2D
{
	private bool scored = false;
	
	private AudioStreamPlayer2D scoresound;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;

		scoresound = GetNode<AudioStreamPlayer2D>("scoresound");
	}

	private void OnAreaEntered(Area2D area)
	{
		if (scored)
			return;

		if (area.IsInGroup("player"))
		{
			scoresound.Play();
			scored = true;
			CallDeferred("set_monitoring", false);
			GetTree().CallGroup("score", "AddPoint");
		}
	}
}
