using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
	public GameObject piecePrefab;
	public GameObject piecesContainer;
	public bool drawRays;

	// Black Sprites
	public Sprite blackRook;
	public Sprite blackKnight;
	public Sprite blackBishop;
	public Sprite blackQueen;
	public Sprite blackKing;
	public Sprite blackPawn;

	// White Sprites
	public Sprite whiteRook;
	public Sprite whiteKnight;
	public Sprite whiteBishop;
	public Sprite whiteQueen;
	public Sprite whiteKing;
	public Sprite whitePawn;

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
	}

	public void DrawPieces()
	{
		// Draw the board's pieces
		for (var y = 1; y < _startingPosition.Count + 1; y++)
		{
			var row = _startingPosition[y - 1];

			for (var x = 1; x < row.Length + 1; x++)
			{
				var piece = Instantiate(piecePrefab, new Vector3(x, y, -2), Quaternion.identity, piecesContainer.transform);
				var pieceScript = piece.GetComponent<Piece>();
				pieceScript.code = row[x - 1];
			}
		}
	}
}
