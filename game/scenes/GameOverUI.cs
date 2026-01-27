using Godot;

public partial class GameOverUI : Control
{
	private Label gameOverLabel;
	private Label highScoreLabel;

	private Label restartLabel;
	private Label clickMouseLabel;

	public override void _Ready()
	{
		highScoreLabel = GetNode<Label>("VBoxContainer/High Score");
		gameOverLabel = GetNode<Label>("VBoxContainer/GameOver");
		restartLabel = GetNode<Label>("VBoxContainer/Restart");
		clickMouseLabel = GetNode<Label>("VBoxContainer/ClickMouse");
		ShowStart();
	}

	public void ShowStart()
	{
		gameOverLabel.Hide();
		highScoreLabel.Hide();
		restartLabel.Hide();

		clickMouseLabel.Show();
		Show();
	}

	public void ShowGameOver(int highScore)
	{
		highScoreLabel.Text = $"High Score: {highScore}";
		gameOverLabel.Text = "GAME OVER";
		gameOverLabel.Show();
		highScoreLabel.Show();
		restartLabel.Show();

		clickMouseLabel.Hide();
		Show();
	}

	public override void _Process(double delta)
	{
		if (!Visible) return;

		if (Input.IsActionJustPressed("restart"))
		{
			GetTree().Paused = false;
			GetTree().CallGroup("score", "Restart");
			GetTree().ReloadCurrentScene();
		}
	}
	
	public void HideAll()
    {
        Hide();
    }
}
