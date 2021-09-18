using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
		for (var x = 1; x < boardSize + 1; x++)
		{
			for (var y = 1; y < boardSize + 1; y++)
			{
				// Create the tile
				var tile = Instantiate(tilePrefab, new Vector3(x, y, 1), Quaternion.identity, board.transform);
				tile.name = $"{x},{y}";

				// Colour tiles
				ColourTile(tile);
			}
		}
	}

	public void ColourTile(GameObject tile)
	{
		var position = tile.transform.position;
		var isDark = (position.x + position.y) % 2 == 0;
		var spriteRenderer = tile.GetComponent<SpriteRenderer>();

		if (isDark)
		{
			spriteRenderer.color = Tile.DarkColour;
		}
		else
		{
			spriteRenderer.color = Tile.LightColour;
		}
	}
	
	// Draws the board's pieces
	private void DrawPieces()
	{
		var py = 0;
		
		foreach (var row in _startingPosition)
		{
			py++;
			var px = 0;
			
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

	private static string GetPieceName(char code)
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

