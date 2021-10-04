using System;
using System.Collections.Generic;
using UnityEngine;

public struct HistoricalMove
{
	public HistoricalMove(Vector2 From, Vector2 To)
	{
		from = From;
		to = To;
	}

	public Vector2 from { get; }
	public Vector2 to { get; }
}

public class GameManager : MonoBehaviour
{
	public AudioManager audioManager;
	public PieceManager pieceManager;
	public ArrowManager arrowManager;
	public BoardManager boardManager;

	public bool whiteTurn = true;
	public bool stockfishEnabled = false;
	public List<string> moveNotations = new List<string>();
	public NETfish.Stockfish stockfish;

	public Color32 orange = new Color32(255, 165, 0, 200);
	public Color32 red = new Color32(255, 50, 50, 200);
	public Color32 blue = new Color32(50, 150, 255, 200);
	public Color32 green = new Color32(50, 255, 50, 200);
	public Color32 yellow = new Color32(218, 195, 50, 255);

	public char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

	public void Start()
	{
		// Start Stockfish
		stockfish = new NETfish.Stockfish($"{Application.streamingAssetsPath}/stockfish14.exe");

		// Begin the game
		boardManager.DrawTiles();
		pieceManager.DrawPieces();
	}

	public void Update()
	{
		// Handle global keybinds
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public string GetAlgebraicNotation(Vector2 position)
	{
		return $"{alphabet[Mathf.RoundToInt(position.x) - 1]}{position.y}";
	}

	public HistoricalMove? GetHistoricalMove(int movesBack)
	{
		try
		{
			var lastMoveNotation = moveNotations[moveNotations.Count - movesBack];

			var fromX = Array.IndexOf(alphabet, lastMoveNotation[0]) + 1;
			var fromY = Convert.ToInt32(lastMoveNotation[1].ToString());

			var toX = Array.IndexOf(alphabet, lastMoveNotation.Substring(2)[0]) + 1;
			var toY = Convert.ToInt32(lastMoveNotation.Substring(2)[1].ToString());

			return new HistoricalMove(new Vector2(fromX, fromY), new Vector2(toX, toY));
		}
		catch (ArgumentOutOfRangeException)
		{
			return null;
		}
	}
}

