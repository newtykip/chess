using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PieceType
{
	Unknown,
	Pawn,
	Rook,
	Knight,
	Bishop,
	Queen,
	King
}

public class Piece : MonoBehaviour
{

	public bool isWhite;
	public PieceType pieceType;
	public bool selected = false;
	public List<Vector2> moveList = new List<Vector2>();
	public int moveCount = 0;

	private GameManager _gameManager;
	private Vector3 _mousePosition;
	private Vector3 _oldPosition;
	private bool _toPassant = false;

	public void Start()
	{
		// Get components
		_gameManager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
	}

	public void OnMouseDown()
	{
		// Save the old piece position
		var piecePosition = transform.position;
		_oldPosition = piecePosition;

		if (isWhite == _gameManager.whiteTurn)
		{
			// Unselect all other pieces
			_gameManager.board.ClearHighlights();
			_gameManager.board.ClearIndicators();
			_gameManager.board.HighlightLastMove();

			var allPieces = GameObject.FindGameObjectsWithTag("Pieces");

			foreach (var piece in allPieces)
			{
				var script = piece.GetComponent<Piece>();
				script.selected = false;
			}

			selected = true;

			// Mark the tile behind it as highlighted/selected
			var allTiles = GameObject.FindGameObjectsWithTag("Tiles");

			foreach (var tile in allTiles)
			{
				if ((Vector2)tile.transform.position == (Vector2)piecePosition)
				{
					var script = tile.GetComponent<Tile>();
					var renderer = tile.GetComponent<SpriteRenderer>();

					script.highlighted = true;
					renderer.color = _gameManager.yellow;
				}
			}

			// Figure out potential moves
			var moves = new List<Vector2>();

			switch (pieceType)
			{
				case PieceType.Pawn:
					var forwardOne = new Vector2(piecePosition.x, piecePosition.y + (_gameManager.whiteStarts ? isWhite ? 1 : -1 : isWhite ? -1 : 1));
					var forwardOneObstructed = false;

					var forwardTwo = new Vector2(piecePosition.x, piecePosition.y + (_gameManager.whiteStarts ? isWhite ? 2 : -2 : isWhite ? -2 : 2));
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
					if (moveCount == 0 && !forwardTwoObstructed)
					{
						moves.Add(forwardTwo);
					}

					// Offer the option to take pieces that are diagonally nearby
					if (_gameManager.whiteStarts == isWhite)
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
					var lastMove = _gameManager.GetHistoricalMove(1);

					if (isWhite)
					{
						var leftCast = DrawRay(Vector2.left, 1, Color.yellow);
						var rightCast = DrawRay(Vector2.right, 1, Color.yellow);

						if (leftCast != null && lastMove != null)
						{
							var enemyScript = leftCast.gameObject.GetComponent<Piece>();

							if (!enemyScript.isWhite && enemyScript.pieceType == PieceType.Pawn)
							{
								var enemyPosition = (Vector2)leftCast.transform.position;

								if (enemyPosition == lastMove.Value.To && enemyScript.moveCount == 1)
								{
									enemyScript._toPassant = true;
									moves.Add(enemyPosition + new Vector2(0, _gameManager.whiteStarts ? 1 : -1));
								}
							}
						}

						if (rightCast != null && lastMove != null)
						{
							var enemyScript = rightCast.gameObject.GetComponent<Piece>();

							if (!enemyScript.isWhite && enemyScript.pieceType == PieceType.Pawn)
							{
								var enemyPosition = (Vector2)rightCast.transform.position;

								if (enemyPosition == lastMove.Value.To && enemyScript.moveCount == 1)
								{
									enemyScript._toPassant = true;
									moves.Add(enemyPosition + new Vector2(0, _gameManager.whiteStarts ? 1 : -1));
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

							if (enemyScript.isWhite && enemyScript.pieceType == PieceType.Pawn)
							{
								var enemyPosition = (Vector2)leftCast.transform.position;

								if (enemyPosition == lastMove.Value.To && enemyScript.moveCount == 1)
								{
									enemyScript._toPassant = true;
									moves.Add(enemyPosition + new Vector2(0, _gameManager.whiteStarts ? -1 : 1));
								}
							}
						}

						if (rightCast != null && lastMove != null)
						{
							var enemyScript = rightCast.gameObject.GetComponent<Piece>();

							if (enemyScript.isWhite && enemyScript.pieceType == PieceType.Pawn)
							{
								var enemyPosition = (Vector2)rightCast.transform.position;

								if (enemyPosition == lastMove.Value.To && enemyScript.moveCount == 1)
								{
									enemyScript._toPassant = true;
									moves.Add(enemyPosition + new Vector2(0, _gameManager.whiteStarts ? -1 : 1));
								}
							}
						}
					}

					break;

				case PieceType.King:
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

				case PieceType.Queen:
					for (var i = 0; i < _gameManager.board.size; i++)
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

				case PieceType.Bishop:
					for (var i = 0; i < _gameManager.board.size; i++)
					{
						// Vertical
						moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y + i));
						moves.Add(new Vector2(_oldPosition.x - i, _oldPosition.y - i));
						moves.Add(new Vector2(_oldPosition.x - i, _oldPosition.y + i));
						moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y - i));
					}

					break;

				case PieceType.Rook:
					for (var i = 0; i < _gameManager.board.size; i++)
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

				case PieceType.Knight:
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
					var isMoveOutOfBounds = move.x > _gameManager.board.size || move.x < 1 ||
											move.y > _gameManager.board.size ||
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
				PieceType.Rook => RookRayChecks(moves),
				PieceType.Bishop => BishopRayChecks(moves),
				PieceType.Queen => QueenRayChecks(moves),
				_ => moves
			};

			// Create highlights for the moves
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

					_gameManager.board.DrawIndicator(tile, move, takes);
				}
			}

			moveList = moves;
		}
	}

	public void OnMouseDrag()
	{
		// Update the position of the piece to the new mouse positions
		_mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));

		transform.position = new Vector3(_mousePosition.x, _mousePosition.y, -5);
	}

	public void OnMouseUp()
	{
		if (isWhite == _gameManager.whiteTurn)
		{
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
			var isMoveLegal = moveList.Contains(new Vector2(newPosition.x, newPosition.y));

			if (isInBounds && positionHasChanged && isPlayersTurn && isMoveLegal)
			{
				MakeMove(gameObject, _oldPosition, newPosition);
			}
			else
			{
				transform.position = _oldPosition;
			}
		}
		else
		{
			transform.position = _oldPosition;
		}
	}

	public void MakeMove(GameObject piece, Vector3 oldPosition, Vector3 newPosition, bool handleStockfish = true)
	{
		oldPosition.z = -2;
		newPosition.z = -2;

		var pieceScript = piece.GetComponent<Piece>();
		pieceScript.moveCount++;

		// Move the piece
		piece.transform.position = newPosition;

		// Play the place sound
		_gameManager.audioManager.PlayPlace();

		// Append the move to notation
		_gameManager.moveNotations.Add($"{_gameManager.GetAlgebraicNotation(oldPosition)}{_gameManager.GetAlgebraicNotation(newPosition)}");

		// Clear all highlights and indicators
		_gameManager.board.ClearHighlights();
		_gameManager.board.ClearIndicators();

		// Highlight the last move
		_gameManager.board.HighlightLastMove();

		// Promotion
		if (pieceScript.pieceType == PieceType.Pawn)
		{
			var position = piece.transform.position;

			// todo: allow for promotion to pieces other than a queen
			if (pieceScript.isWhite)
			{
				if (_gameManager.whiteStarts && position.y == _gameManager.board.size || !_gameManager.whiteStarts && position.y == 1)
				{
					_gameManager.pieces.InitPiece(piece, PieceType.Queen, isWhite);
				}
			}
			else
			{
				if (_gameManager.whiteStarts && position.y == 1 || !_gameManager.whiteStarts && position.y == _gameManager.board.size)
				{
					_gameManager.pieces.InitPiece(piece, PieceType.Queen, isWhite);
				}
			}
		}

		// Manage the taking of pieces!
		var allPieces = GameObject.FindGameObjectsWithTag("Pieces");

		foreach (var p in allPieces)
		{
			var script = p.GetComponent<Piece>();

			// En Passant
			if (script._toPassant)
			{
				if (script.isWhite && (newPosition.y == p.transform.position.y + (_gameManager.whiteStarts ? -1 : 1)))
				{
					Destroy(p);
					return;
				}

				if (!script.isWhite && (newPosition.y == p.transform.position.y + (_gameManager.whiteStarts ? 1 : -1)))
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

		// Clear the move list
		pieceScript.moveList.Clear();

		// Invert the turn variable
		_gameManager.whiteTurn = !_gameManager.whiteTurn;

		// Handle Stockfish's move
		if (_gameManager.stockfish.isEnabled && handleStockfish && _gameManager.whiteTurn == _gameManager.stockfish.isWhite)
		{
			var bestMove = _gameManager.stockfish.GetBestMove();

			foreach (var p in allPieces)
			{
				if ((Vector2)p.transform.position == bestMove.From)
				{
					MakeMove(p, bestMove.From, bestMove.To, false);
				}
			}
		}
	}

	private Collider2D DrawRay(Vector2 direction, float distance, Color color)
	{
		// Disable the collider
		var collider = gameObject.GetComponent<BoxCollider2D>();
		collider.enabled = false;

		// Draw the ray
		var position = transform.position;
		var hit = Physics2D.Raycast(position, direction, distance);
		if (_gameManager.pieces.drawRays) Debug.DrawRay(position, direction * distance, color, 3);

		// Re-enable the collider and return the hit
		collider.enabled = true;
		return hit.collider;
	}

	private List<Vector2> RookRayChecks(List<Vector2> moves)
	{
		var directions = new Vector2[] { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

		foreach (var direction in directions)
		{
			var ray = DrawRay(direction, _gameManager.board.size, Color.blue);

			if (ray != null)
			{
				var position = transform.position;
				var collidedPosition = ray.transform.position;

				switch (direction.y)
				{
					// If a piece has hits another piece with its top ray, remove all of the moves ahead of the collided piece
					case 1:
						for (var i = 1; i < _gameManager.board.size + 1; i++)
						{
							moves.Remove(new Vector2(position.x, collidedPosition.y + i));
						}

						break;

					// If a piece has hits another piece with its bottom ray, remove all of the moves behind the collided piece
					case -1:
						for (var i = 1; i < _gameManager.board.size + 1; i++)
						{
							moves.Remove(new Vector2(position.x, collidedPosition.y - i));
						}

						break;
				}

				switch (direction.x)
				{
					// If a piece hits another piece with its left ray, remove all of the moves to the left of the collided piece
					case -1:
						for (var i = 1; i < _gameManager.board.size + 1; i++)
						{
							moves.Remove(new Vector2(collidedPosition.x - i, position.y));
						}

						break;

					// If a piece hits another piece with its right ray, remove all of the moves to the right of the collided piece
					case 1:
						for (var i = 1; i < _gameManager.board.size + 1; i++)
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
			var ray = DrawRay(direction, _gameManager.board.size, Color.red);

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
								for (var i = 0; i < _gameManager.board.size + 1; i++)
								{
									moves.Remove(new Vector2(collidedPosition.x - i, collidedPosition.y + i));
								}

								break;

							// ...right...
							case 1:
								for (var i = 0; i < _gameManager.board.size + 1; i++)
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
								for (var i = 0; i < _gameManager.board.size + 1; i++)
								{
									moves.Remove(new Vector2(collidedPosition.x - i, collidedPosition.y - i));
								}

								break;

							// ...right...
							case 1:
								for (var i = 0; i < _gameManager.board.size + 1; i++)
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
