using Godot;
using System;
using System.IO;

public partial class ScoreManager : Control
{
	public int Score { get; private set; } = 0;

	public int HighScore { get; private set; } = 0;
	private const string HighScorePath = "user://highscore.txt";
	private Label scoreLabel;
	public override void _Ready()
	{
		AddToGroup("score");

		scoreLabel = GetNode<Label>("scorelabel");
		scoreLabel.Text = "0";
		LoadHighScore();
	}

	public void AddPoint()
	{
		Score++;
		scoreLabel.Text = Score.ToString();
		GD.Print($"Score: {Score}");
	}

	public void ResetScore()
	{
		GD.Print($"ResetScore called. Score={Score}, HighScore={HighScore}");
		if (Score > HighScore)
		{
			HighScore = Score;
			using var file = Godot.FileAccess.Open(HighScorePath, Godot.FileAccess.ModeFlags.Write); //I guess if you put using it will close it after
			file.StoreLine(HighScore.ToString());
		}
		
	}

	public void Restart()
	{
		Score = 0;
		scoreLabel.Text = "0";
	}

	private void LoadHighScore()
	{
		if (!Godot.FileAccess.FileExists(HighScorePath))
		{
			using var file = Godot.FileAccess.Open(
				HighScorePath,
				Godot.FileAccess.ModeFlags.Write
			);
			file.StoreLine("0");
			HighScore = 0;
			return;
		}

		using var readFile = Godot.FileAccess.Open(
			HighScorePath,
			Godot.FileAccess.ModeFlags.Read
		);
		HighScore = int.Parse(readFile.GetLine());
		GD.Print($"Loaded HighScore: {HighScore}");
	}
}
