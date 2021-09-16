using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject tilePrefab;
	public GameObject board;
	public GameObject piecePrefab;
	public GameObject pieces;
	public bool whiteTurn = true;
	
	private const int RowCount = 8;
	private const int TilesPerRow = 8;

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

	public void Start()
	{
		// Reverse the starting position because I want the list to mirror the board in the code kljfsadjkhsa
		_startingPosition.Reverse();
		
		// Set up the board
		DrawTiles();
		DrawPieces();
	}

	public void Update()
	{
		// Handle global keybinds
		if (Input.GetKeyDown("escape"))
		{
			Application.Quit();
		}
	}

	// Draws the board's tiles
	private void DrawTiles()
	{
		for (var x = 0; x < RowCount; x++)
		{
			for (var y = 0; y < TilesPerRow; y++)
			{
				// Create the tile
				var tile = Instantiate(tilePrefab, new Vector3(x, y, 1), Quaternion.identity, board.transform);
				tile.name = $"{x},{y}";

				// Handle dark tiles
				var isDark = (x + y) % 2 == 0;
				if (!isDark) continue;
				
				var spriteRenderer = tile.GetComponent<SpriteRenderer>();
				ColorUtility.TryParseHtmlString("#b48866", out var colour);
				
				spriteRenderer.color = colour;
			}
		}
	}
	
	// Draws the board's pieces
	private void DrawPieces()
	{
		var py = -1;
		
		foreach (var row in _startingPosition)
		{
			py++;
			var px = -1;
			
			foreach (var code in row)
			{
				px++;
				
				var piece = Instantiate(piecePrefab, new Vector3(px, py), Quaternion.identity, pieces.transform);
				piece.name = GetPieceName(code);
				
				// Apply the relevant sprite
				var spriteRenderer = piece.GetComponent<SpriteRenderer>();

				spriteRenderer.sprite = code switch
				{
					'R' => blackRook,
					'N' => blackKnight,
					'B' => blackBishop,
					'Q' => blackQueen,
					'K' => blackKing,
					'P' => blackPawn,
					'r' => whiteRook,
					'n' => whiteKnight,
					'b' => whiteBishop,
					'q' => whiteQueen,
					'k' => whiteKing,
					'p' => whitePawn,
					_ => spriteRenderer.sprite
				};
			}
		}
	}

	private string GetPieceName(char code)
	{
		var colour = char.IsUpper(code) ? "Black" : "White";
				
		var pieceName = char.ToLower(code) switch
		{
			'r' => "Rook",
			'n' => "Knight",
			'b' => "Bishop",
			'q' => "Queen",
			'k' => "King",
			'p' => "Pawn",
			_ => "Unknown"
		};

		return $"{colour} {pieceName}";
	}
}

