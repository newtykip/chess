using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject tilePrefab;
	public GameObject board;
	public GameObject piecePrefab;
	public GameObject pieces;
	
	public bool whiteTurn = true;
	public int boardSize = 8;

	private readonly List<string> _startingPosition = new List<string>()
	{
		"RNBQKBNR",
		"PPPPPPPP",
		"",
		"",
		"",
		"",
		"pppppppp",
		"rnbqkbnr"
	};

	public void Start()
	{
		// Reverse the starting position because I want the list to mirror the board in the code kljfsadjkhsa
		_startingPosition.Reverse();

		// Draw the board's tiles
		for (var x = 1; x < boardSize + 1; x++)
		{
			for (var y = 1; y < boardSize + 1; y++)
			{
				// Create the tile
				var tile = Instantiate(tilePrefab, new Vector3(x, y, 1), Quaternion.identity, board.transform);
			}
		}
		
		// Draw the board's pieces
		for (var y = 1; y < _startingPosition.Count + 1; y++)
		{
			var row = _startingPosition[y - 1];

			for (var x = 1; x < row.Length + 1; x++)
			{
				var piece = Instantiate(piecePrefab, new Vector3(x, y), Quaternion.identity, pieces.transform);
				var pieceScript = piece.GetComponent<Piece>();
				pieceScript.code = row[x - 1];
			}
		}
	}

	public void Update()
	{
		// Handle global keybinds
		if (Input.GetKeyDown("escape"))
		{
			Application.Quit();
		}
	}
}

