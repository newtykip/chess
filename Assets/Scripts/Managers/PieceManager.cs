using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
	public GameObject piecePrefab;
	public GameObject pieces;

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
				var piece = Instantiate(piecePrefab, new Vector3(x, y), Quaternion.identity, pieces.transform);
				var pieceScript = piece.GetComponent<Piece>();
				pieceScript.code = row[x - 1];
			}
		}
	}
}
