using Godot;
using System;

public partial class ScoreManager : Node
{
	public int Score { get; private set; } = 0;
	public int HighScore { get; private set; } = 0;
	public override void _Ready()
	{
		AddToGroup("score");
		LoadHighScore();

	}

	public void AddPoint()
	{
		Score++;
		GD.Print($"Score: {Score}");
	}
	public void GameOver()
	{
		if (Score > HighScore)
		{
			HighScore = Score;
			SaveHighScore();
		}
	}

	private void SaveHighScore()
	{
		var cfg = new ConfigFile();
		cfg.SetValue("score", "highscore", HighScore);
		cfg.Save("user://score.cfg");
	}

	private void LoadHighScore()
	{
		var cfg = new ConfigFile();
		if (cfg.Load("user://score.cfg") == Error.Ok)
			HighScore = (int)cfg.GetValue("score", "highscore", 0);
	}

	public void ResetScore()
	{
		Score = 0;
	}
}
