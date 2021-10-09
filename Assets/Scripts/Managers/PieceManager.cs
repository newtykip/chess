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

	public void DrawPieces(bool whiteStarts)
	{
		// If black starts, reverse the starting position
		if (!whiteStarts)
		{
			_startingPosition.Reverse();
		}

		// Draw the board's pieces
		for (var y = 1; y < _startingPosition.Count + 1; y++)
		{
			var row = _startingPosition[y - 1];

			for (var x = 1; x < row.Length + 1; x++)
			{
				var piece = Instantiate(piecePrefab, new Vector3(x, y, -2), Quaternion.identity, piecesContainer.transform);

				var type = char.ToLower(row[x - 1]) switch
				{
					'p' => PieceType.Pawn,
					'r' => PieceType.Rook,
					'n' => PieceType.Knight,
					'b' => PieceType.Bishop,
					'q' => PieceType.Queen,
					'k' => PieceType.King,
					_ => PieceType.Unknown
				};

				InitPiece(piece, type, char.IsUpper(row[x - 1]), false);
			}
		}

		// If black started, restore the starting position
		if (!whiteStarts)
		{
			_startingPosition.Reverse();
		}
	}

	public void InitPiece(GameObject piece, PieceType type, bool white, bool promoted = true)
	{
		var script = piece.GetComponent<Piece>();

		// Update the script of the piece
		script.white = white;
		script.type = type;
		piece.name = $"{(promoted ? "[PROMOTED] " : "")}{(script.white ? "White" : "Black")} {script.type}";

		// Update the piece's sprite
		piece.GetComponent<SpriteRenderer>().sprite = script.type switch
		{
			PieceType.Rook => script.white ? whiteRook : blackRook,
			PieceType.Knight => script.white ? whiteKnight : blackKnight,
			PieceType.Bishop => script.white ? whiteBishop : blackBishop,
			PieceType.Queen => script.white ? whiteQueen : blackQueen,
			PieceType.King => script.white ? whiteKing : blackKing,
			PieceType.Pawn => script.white ? whitePawn : blackPawn,
			_ => script.white ? whitePawn : blackPawn
		};
	}

	public void DestroyPieces()
	{
		foreach (Transform piece in piecesContainer.transform)
		{
			Destroy(piece.gameObject);
		}
	}
}
