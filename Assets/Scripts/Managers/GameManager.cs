using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public AudioManager audioManager;
	public PieceManager pieces;
	public ArrowManager arrows;
	public BoardManager board;
	public StockfishManager stockfish;
	public Counter gameCounter;
	public bool whiteStarts = true;
	public bool whiteTurn = true;
	public List<string> moveNotations = new List<string>();
	public Color32 orange = new Color32(255, 165, 0, 200);
	public Color32 red = new Color32(255, 50, 50, 200);
	public Color32 blue = new Color32(50, 150, 255, 200);
	public Color32 green = new Color32(50, 255, 50, 200);
	public Color32 yellow = new Color32(218, 195, 50, 255);
	public char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

	private int whiteScore = 0;
	private int blackScore = 0;

	public void Start()
	{
		// Find the counters
		var counters = GameObject.FindGameObjectsWithTag("Counters");

		foreach (var counter in counters)
		{
			if (counter.name.ToLower() == "game number")
			{
				gameCounter = counter.GetComponent<Counter>();
			}
		}

		// Draw the board
		board.DrawTiles();

		// Begin the game
		NewGame();
	}

	public void Update()
	{
		// Handle global keybinds
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void NewGame()
	{
		// Clear old highlights and indicators
		board.ClearHighlights();
		board.ClearIndicators();

		// Clear the move notations
		moveNotations.Clear();

		// Configure the game
		whiteStarts = !whiteStarts;
		whiteTurn = whiteStarts;
		stockfish.isWhite = !whiteTurn;
		gameCounter.Add();

		// Draw new pieces
		pieces.DestroyPieces();
		pieces.DrawPieces(whiteStarts);
	}

	public string GetAlgebraicNotation(Vector2 position)
	{
		return $"{alphabet[Mathf.RoundToInt(position.x) - 1]}{position.y}";
	}

	public MoveSet? GetHistoricalMove(int movesBack)
	{
		try
		{
			var lastMoveNotation = moveNotations[moveNotations.Count - movesBack];

			var fromX = Array.IndexOf(alphabet, lastMoveNotation[0]) + 1;
			var fromY = Convert.ToInt32(lastMoveNotation[1].ToString());

			var toX = Array.IndexOf(alphabet, lastMoveNotation.Substring(2)[0]) + 1;
			var toY = Convert.ToInt32(lastMoveNotation.Substring(2)[1].ToString());

			return new MoveSet(new Vector2(fromX, fromY), new Vector2(toX, toY));
		}
		catch (ArgumentOutOfRangeException)
		{
			return null;
		}
	}
}

