using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour
{

	public bool isWhite;
	public string pieceType;
	public char code;

	private GameManager _gameManager;
	private BoxCollider2D _boxCollider;
	private Vector3 _mousePosition;
	private Vector3 _screenPoint;
	private Vector3 _oldPosition;
	private List<Vector2> _moves = new List<Vector2>();
	private static char[] _alpha = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
	private bool _toPassant = false;
	private int _moveCount = 0;

	public void Start()
	{
		// Get components
		_gameManager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
		_boxCollider = gameObject.GetComponent<BoxCollider2D>();

		// Determine the piece's colour and type
		isWhite = char.IsUpper(code);

		pieceType = char.ToLower(code) switch
		{
			'r' => "Rook",
			'n' => "Knight",
			'b' => "Bishop",
			'q' => "Queen",
			'k' => "King",
			'p' => "Pawn",
			_ => "Unknown"
		};

		// Update the game object's name and sprite
		gameObject.name = $"{(isWhite ? "White" : "Black")} {pieceType}";

		gameObject.GetComponent<SpriteRenderer>().sprite = pieceType switch
		{
			"Rook" => isWhite ? _gameManager.pieceManager.whiteRook : _gameManager.pieceManager.blackRook,
			"Knight" => isWhite ? _gameManager.pieceManager.whiteKnight : _gameManager.pieceManager.blackKnight,
			"Bishop" => isWhite ? _gameManager.pieceManager.whiteBishop : _gameManager.pieceManager.blackBishop,
			"Queen" => isWhite ? _gameManager.pieceManager.whiteQueen : _gameManager.pieceManager.blackQueen,
			"King" => isWhite ? _gameManager.pieceManager.whiteKing : _gameManager.pieceManager.blackKing,
			"Pawn" => isWhite ? _gameManager.pieceManager.whitePawn : _gameManager.pieceManager.blackPawn,
			_ => isWhite ? _gameManager.pieceManager.whitePawn : _gameManager.pieceManager.blackPawn
		};
	}

	public void OnMouseDown()
	{
		// Save the old piece position
		var piecePosition = transform.position;
		_oldPosition = piecePosition;

		if (isWhite == _gameManager.whiteTurn)
		{
			// Figure out potential moves
			var moves = new List<Vector2>();
			var allPieces = GameObject.FindGameObjectsWithTag("Pieces");

			switch (pieceType)
			{
				case "Pawn":
					var forwardOne = new Vector2(piecePosition.x, isWhite ? piecePosition.y + 1 : piecePosition.y - 1);
					var forwardOneObstructed = false;

					var forwardTwo = new Vector2(piecePosition.x, isWhite ? piecePosition.y + 2 : piecePosition.y - 2);
					var forwardTwoObstructed = false;

					foreach (var piece in allPieces)
					{
						if ((Vector2)piece.transform.position == forwardOne)
						{
							forwardOneObstructed = true;
							forwardTwoObstructed = true;
						}

						if ((Vector2)piece.transform.position == forwardTwo)
						{
							forwardTwoObstructed = true;
						}
					}

					// Move forward 1 if another piece is not in the way
					if (!forwardOneObstructed)
					{
						moves.Add(forwardOne);
					}

					// Initial move forward 2 if another piece is not in the way
					if (((isWhite && piecePosition.y == 2) || (!isWhite && _oldPosition.y == _gameManager.boardManager.size - 1)) && !forwardTwoObstructed)
					{
						moves.Add(forwardTwo);
					}

					// Offer the option to take pieces that are diagonally nearby
					if (isWhite)
					{
						var topLeft = DrawRay(Vector2.up + Vector2.left, 1, Color.magenta);
						var topRight = DrawRay(Vector2.up + Vector2.right, 1, Color.magenta);

						if (topLeft != null)
						{
							var collisionPosition = topLeft.transform.position;
							moves.Add(new Vector2(collisionPosition.x, collisionPosition.y));
						}

						if (topRight != null)
						{
							var collisionPosition = topRight.transform.position;
							moves.Add(new Vector2(collisionPosition.x, collisionPosition.y));
						}
					}
					else
					{
						var bottomLeft = DrawRay(Vector2.down + Vector2.left, 1, Color.magenta);
						var bottomRight = DrawRay(Vector2.down + Vector2.right, 1, Color.magenta);

						if (bottomLeft != null)
						{
							var collisionPosition = bottomLeft.transform.position;
							moves.Add(new Vector2(collisionPosition.x, collisionPosition.y));
						}

						if (bottomRight != null)
						{
							var collisionPosition = bottomRight.transform.position;
							moves.Add(new Vector2(collisionPosition.x, collisionPosition.y));
						}
					}

					// En Passant
					if (pieceType == "Pawn")
					{
						var lastMove = GetHistoricalMove(1);

						if (isWhite)
						{
							var leftCast = DrawRay(Vector2.left, 1, Color.yellow);
							var rightCast = DrawRay(Vector2.right, 1, Color.yellow);

							if (leftCast != null && lastMove != null)
							{
								var enemyScript = leftCast.gameObject.GetComponent<Piece>();

								if (!enemyScript.isWhite && enemyScript.pieceType == "Pawn")
								{
									var enemyPosition = (Vector2)leftCast.transform.position;

									if (enemyPosition == lastMove.Value && lastMove.Value.y + 2 == 7 && enemyScript._moveCount == 1)
									{
										enemyScript._toPassant = true;
										moves.Add(enemyPosition + new Vector2(0, 1));
									}
								}
							}

							if (rightCast != null && lastMove != null)
							{
								var enemyScript = rightCast.gameObject.GetComponent<Piece>();

								if (!enemyScript.isWhite && enemyScript.pieceType == "Pawn")
								{
									var enemyPosition = (Vector2)rightCast.transform.position;

									if (enemyPosition == lastMove.Value && lastMove.Value.y + 2 == 7 && enemyScript._moveCount == 1)
									{
										enemyScript._toPassant = true;
										moves.Add(enemyPosition + new Vector2(0, 1));
									}
								}
							}
						}
						else
						{
							var leftCast = DrawRay(Vector2.left, 1, Color.yellow);
							var rightCast = DrawRay(Vector2.right, 1, Color.yellow);

							if (leftCast != null && lastMove != null)
							{
								var enemyScript = leftCast.gameObject.GetComponent<Piece>();

								if (enemyScript.isWhite && enemyScript.pieceType == "Pawn")
								{
									var enemyPosition = (Vector2)leftCast.transform.position;

									if (enemyPosition == lastMove.Value && lastMove.Value.y - 2 == 2 && enemyScript._moveCount == 1)
									{
										enemyScript._toPassant = true;
										moves.Add(enemyPosition + new Vector2(0, -1));
									}
								}
							}

							if (rightCast != null && lastMove != null)
							{
								var enemyScript = rightCast.gameObject.GetComponent<Piece>();

								if (enemyScript.isWhite && enemyScript.pieceType == "Pawn")
								{
									var enemyPosition = (Vector2)rightCast.transform.position;

									if (enemyPosition == lastMove.Value && lastMove.Value.y - 2 == 2 && enemyScript._moveCount == 1)
									{
										enemyScript._toPassant = true;
										moves.Add(enemyPosition + new Vector2(0, -1));
									}
								}
							}
						}
					}

					break;
				case "King":
					// x x x
					// x K x
					// x x x
					for (var i = -1; i <= 1; i++)
					{
						for (var j = -1; j <= 1; j++)
						{
							moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y + j));
						}
					}

					break;
				case "Queen":
					for (var i = 0; i < _gameManager.boardManager.size; i++)
					{
						// Forward/Backwards
						moves.Add(new Vector2(_oldPosition.x, _oldPosition.y + i));
						moves.Add(new Vector2(_oldPosition.x, _oldPosition.y - i));

						// Left/Right
						moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y));
						moves.Add(new Vector2(_oldPosition.x - i, _oldPosition.y));

						// Vertical
						moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y + i));
						moves.Add(new Vector2(_oldPosition.x - i, _oldPosition.y - i));
						moves.Add(new Vector2(_oldPosition.x - i, _oldPosition.y + i));
						moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y - i));
					}

					break;
				case "Bishop":
					for (var i = 0; i < _gameManager.boardManager.size; i++)
					{
						// Vertical
						moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y + i));
						moves.Add(new Vector2(_oldPosition.x - i, _oldPosition.y - i));
						moves.Add(new Vector2(_oldPosition.x - i, _oldPosition.y + i));
						moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y - i));
					}

					break;
				case "Rook":
					for (var i = 0; i < _gameManager.boardManager.size; i++)
					{
						// Forward/Backwards
						moves.Add(new Vector2(_oldPosition.x, _oldPosition.y + i));
						moves.Add(new Vector2(_oldPosition.x, _oldPosition.y - i));

						// Left/Right
						moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y));
						moves.Add(new Vector2(_oldPosition.x - i, _oldPosition.y));

						// todo: Castling
					}

					break;
				case "Knight":
					// L
					moves.Add(new Vector2(_oldPosition.x + 1, _oldPosition.y + 2));
					moves.Add(new Vector2(_oldPosition.x + 1, _oldPosition.y - 2));
					moves.Add(new Vector2(_oldPosition.x + 2, _oldPosition.y + 1));
					moves.Add(new Vector2(_oldPosition.x + 2, _oldPosition.y - 1));
					moves.Add(new Vector2(_oldPosition.x - 1, _oldPosition.y + 2));
					moves.Add(new Vector2(_oldPosition.x - 1, _oldPosition.y - 2));
					moves.Add(new Vector2(_oldPosition.x - 2, _oldPosition.y + 1));
					moves.Add(new Vector2(_oldPosition.x - 2, _oldPosition.y - 1));
					break;
			}

			// Filter moves
			foreach (var move in moves.ToArray())
			{
				foreach (var piece in allPieces.ToArray())
				{
					// Check if the move is overlapping any friendly pieces
					var pieceScript = piece.GetComponent<Piece>();
					var isMoveOverlapping = move == (Vector2)piece.transform.position && pieceScript.isWhite == isWhite;

					// Check if the move is out of bounds
					var isMoveOutOfBounds = move.x > _gameManager.boardManager.size || move.x < 1 ||
											move.y > _gameManager.boardManager.size ||
											move.y < 1;

					if (isMoveOutOfBounds || isMoveOverlapping || move == (Vector2)_oldPosition)
					{
						moves.Remove(move);
					}
				}
			}

			// Remove any moves where the piece would have to jump
			moves = pieceType switch
			{
				"Rook" => RookRayChecks(moves),
				"Bishop" => BishopRayChecks(moves),
				"Queen" => QueenRayChecks(moves),
				_ => moves
			};

			// Create highlights for the moves
			var allTiles = GameObject.FindGameObjectsWithTag("Tiles");

			foreach (var tile in allTiles)
			{
				foreach (var move in moves)
				{
					if ((Vector2)tile.transform.position != move) continue;

					var takes = false;

					foreach (var piece in allPieces)
					{
						var script = piece.GetComponent<Piece>();

						// If the piece being iterated over is an enemy
						if (isWhite != script.isWhite)
						{
							// If the piece is at the move, mark it as takeable
							if ((Vector2)piece.transform.position == move)
							{
								takes = true;
							}
						}
					}

					_gameManager.boardManager.DrawIndicator(tile, move, takes);
				}
			}

			_moves = moves;
		}
	}

	public void OnMouseDrag()
	{
		// Update the position of the piece to the new mouse positions
		_mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		_mousePosition.z = -2;

		transform.position = _mousePosition;
	}

	public void OnMouseUp()
	{
		if (isWhite == _gameManager.whiteTurn)
		{
			// Clear all highlights
			_gameManager.boardManager.ClearIndicators();

			// Figure out the piece's new position
			var px = Mathf.Round(_mousePosition.x);
			var py = Mathf.Round(_mousePosition.y);
			var newPosition = new Vector3(px, py, _oldPosition.z);

			// Check:
			// - the position is in bounds
			// - the position is different to what it previously was
			// - that it is the player's turn
			// - that the move is legal
			var isInBounds = (0 < px && px <= 8) && (0 < py && py <= 8);
			var positionHasChanged = newPosition != _oldPosition;
			var isPlayersTurn = (isWhite && _gameManager.whiteTurn) || (!isWhite && !_gameManager.whiteTurn);
			var isMoveLegal = _moves.Contains(new Vector2(newPosition.x, newPosition.y));

			if (isInBounds && positionHasChanged && isPlayersTurn && isMoveLegal)
			{
				MakeMove(gameObject, _oldPosition, newPosition);

				// Temporary Stockfish AI for black 
				// todo: make this... better
				if (_gameManager.stockfishEnabled)
				{
					_gameManager.Stockfish.SetPosition(_gameManager.moveNotations);

					var k = 0f;
					var stockfishMove = _gameManager.Stockfish.GetBestMove().ToLookup(c => Math.Floor(k++ / 2)).Select(e => new string(e.ToArray())).ToArray();

					var stockfishFrom = new Vector3(Array.IndexOf(_alpha, stockfishMove[0][0]) + 1, 0, _oldPosition.z);
					float.TryParse(stockfishMove[0][1].ToString(), out stockfishFrom.y);

					var stockfishTo = new Vector3(Array.IndexOf(_alpha, stockfishMove[1][0]) + 1, 0, _oldPosition.z);
					float.TryParse(stockfishMove[1][1].ToString(), out stockfishTo.y);

					var allPieces = GameObject.FindGameObjectsWithTag("Pieces");

					foreach (var piece in allPieces)
					{
						if (piece.transform.position != stockfishFrom) continue;
						MakeMove(piece, stockfishFrom, stockfishTo);
					}
				}
			}
			else
			{
				transform.position = _oldPosition;
			}

			// Clear the moves
			_moves.Clear();
		}
		else
		{
			transform.position = _oldPosition;
		}
	}

	private void MakeMove(GameObject piece, Vector3 oldPosition, Vector3 newPosition)
	{
		var pieceScript = piece.GetComponent<Piece>();
		pieceScript._moveCount++;

		// Move the piece
		piece.transform.position = newPosition;

		// Play the place sound
		_gameManager.audioManager.PlayPlace();

		// Append the move to notation
		_gameManager.moveNotations.Add($"{GetAlgebraicNotation(oldPosition)}{GetAlgebraicNotation(newPosition)}");

		// Invert the turn variable
		_gameManager.whiteTurn = !_gameManager.whiteTurn;

		// Manage the taking of pieces!
		var allPieces = GameObject.FindGameObjectsWithTag("Pieces");

		foreach (var p in allPieces)
		{
			var script = p.GetComponent<Piece>();

			if (script._toPassant)
			{
				if (script.isWhite && (newPosition.y == p.transform.position.y - 1))
				{
					Destroy(p);
					return;
				}

				if (!script.isWhite && (newPosition.y == p.transform.position.y + 1))
				{
					Destroy(p);
					return;
				}

				script._toPassant = false;
			}

			// Ensure that the piece does not delete itself
			if (p.transform.position != piece.transform.position || p == piece) continue;

			// Protect pieces from being taken by pieces of their own colour
			if (script.isWhite == pieceScript.isWhite)
			{
				piece.transform.position = oldPosition;
				return;
			}

			Destroy(p);
		}
	}

	private static string GetAlgebraicNotation(Vector2 position)
	{
		return $"{_alpha[Mathf.RoundToInt(position.x) - 1]}{position.y}";
	}

	private Vector2? GetHistoricalMove(int movesBack)
	{
		try
		{
			var lastMoveNotation = _gameManager.moveNotations[_gameManager.moveNotations.Count - movesBack];
			var x = Array.IndexOf(_alpha, lastMoveNotation.Substring(2)[0]) + 1;
			var y = Convert.ToInt32(lastMoveNotation.Substring(2)[1].ToString());

			return new Vector2(x, y);
		}
		catch (ArgumentOutOfRangeException)
		{
			return null;
		}
	}

	private Collider2D DrawRay(Vector2 direction, float distance, Color color)
	{
		_boxCollider.enabled = false;

		var position = transform.position;
		var hit = Physics2D.Raycast(position, direction, distance);

		if (_gameManager.pieceManager.drawRays) Debug.DrawRay(position, direction * distance, color, 3);

		_boxCollider.enabled = true;

		return hit.collider;
	}

	private List<Vector2> RookRayChecks(List<Vector2> moves)
	{
		var directions = new Vector2[] { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

		foreach (var direction in directions)
		{
			var ray = DrawRay(direction, _gameManager.boardManager.size, Color.blue);

			if (ray != null)
			{
				var position = transform.position;
				var collidedPosition = ray.transform.position;

				switch (direction.y)
				{
					// If a piece has hits another piece with its top ray, remove all of the moves ahead of the collided piece
					case 1:
						for (var i = 1; i < _gameManager.boardManager.size + 1; i++)
						{
							moves.Remove(new Vector2(position.x, collidedPosition.y + i));
						}

						break;

					// If a piece has hits another piece with its bottom ray, remove all of the moves behind the collided piece
					case -1:
						for (var i = 1; i < _gameManager.boardManager.size + 1; i++)
						{
							moves.Remove(new Vector2(position.x, collidedPosition.y - i));
						}

						break;
				}

				switch (direction.x)
				{
					// If a piece hits another piece with its left ray, remove all of the moves to the left of the collided piece
					case -1:
						for (var i = 0; i < _gameManager.boardManager.size + 1; i++)
						{
							moves.Remove(new Vector2(collidedPosition.x - i, position.y));
						}

						break;

					// If a piece hits another piece with its right ray, remove all of the moves to the right of the collided piece
					case 1:
						for (var i = 0; i < _gameManager.boardManager.size + 1; i++)
						{
							moves.Remove(new Vector2(collidedPosition.x + i, position.y));
						}

						break;
				}
			}
		}

		return moves;
	}

	private List<Vector2> BishopRayChecks(List<Vector2> moves)
	{
		var directions = new Vector2[] {
			Vector2.up + Vector2.left,
			Vector2.up + Vector2.right,
			Vector2.down + Vector2.left,
			Vector2.down + Vector2.right
		};

		foreach (var direction in directions)
		{
			var ray = DrawRay(direction, _gameManager.boardManager.size, Color.red);

			if (ray != null)
			{
				var collidedPosition = ray.transform.position;

				switch (direction.y)
				{
					// If a piece hits a piece with its top...
					// ...ray, remove all of the moves diagonally behind the collided piece
					case 1:
						switch (direction.x)
						{
							// ...left...
							case -1:
								for (var i = 0; i < _gameManager.boardManager.size + 1; i++)
								{
									moves.Remove(new Vector2(collidedPosition.x - i, collidedPosition.y + i));
								}

								break;

							// ...right...
							case 1:
								for (var i = 0; i < _gameManager.boardManager.size + 1; i++)
								{
									moves.Remove(new Vector2(collidedPosition.x + i, collidedPosition.y + i));
								}

								break;
						}

						break;

					// If a piece hits a piece with its bottom...
					// ...ray, remove all of the moves diagonally behind the collided piece
					case -1:
						switch (direction.x)
						{
							// ...left...
							case -1:
								for (var i = 0; i < _gameManager.boardManager.size + 1; i++)
								{
									moves.Remove(new Vector2(collidedPosition.x - i, collidedPosition.y - i));
								}

								break;

							// ...right...
							case 1:
								for (var i = 0; i < _gameManager.boardManager.size + 1; i++)
								{
									moves.Remove(new Vector2(collidedPosition.x + i, collidedPosition.y - i));
								}

								break;
						}

						break;
				}

				// Add back the collided position to the moves, as it has to be capturable
				if (isWhite != ray.gameObject.GetComponent<Piece>().isWhite) moves.Add(collidedPosition);
			}
		}

		return moves;
	}

	private List<Vector2> QueenRayChecks(List<Vector2> moves)
	{
		moves = RookRayChecks(moves);
		moves = BishopRayChecks(moves);

		return moves;
	}
}
