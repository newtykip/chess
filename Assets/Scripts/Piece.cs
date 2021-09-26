using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Piece : MonoBehaviour
{
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
    
    // Ray casting constants
    private readonly List<Vector2> _rookDirections = new List<Vector2>() { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

    private readonly List<Vector2> _bishopDirections = new List<Vector2>()
    {
        Vector2.up + Vector2.left,
        Vector2.up + Vector2.right,
        Vector2.down + Vector2.left,
        Vector2.down + Vector2.right
    };

    private GameManager _gameManager;
    private Vector3 _mousePosition;
    private Vector3 _screenPoint;
    private Vector2 _oldPosition;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private List<Vector2> _moves = new List<Vector2>();
    private static char[] _alpha = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    private bool _isWhite;
    private string _pieceType;

    public bool drawRays;
    public char code;

    public void Start()
    {
        // Get components
        _gameManager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _boxCollider = gameObject.GetComponent<BoxCollider2D>();

        // Determine the piece's colour and type
        _isWhite = char.IsLower(code);
        
        _pieceType = char.ToLower(code) switch
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
        gameObject.name = $"{(_isWhite ? "White" : "Black")} {_pieceType}";

        _spriteRenderer.sprite = _pieceType switch
        {
            "Rook" => _isWhite ? whiteRook : blackRook,
            "Knight" => _isWhite ? whiteKnight : blackKnight,
            "Bishop" => _isWhite ? whiteBishop : blackBishop,
            "Queen" => _isWhite ? whiteQueen : blackQueen,
            "King" => _isWhite ? whiteKing : blackKing,
            "Pawn" => _isWhite ? whitePawn : blackPawn,
            _ => _isWhite ? whitePawn : blackPawn
        };
    }

    public void OnMouseDown()
    {
        // Save the old piece position
        _oldPosition = transform.position;

        // Figure out potential moves
        var moves = new List<Vector2>();

        switch (_pieceType)
        {
            case "Pawn":
                // Move forward 1
                moves.Add(new Vector3(_oldPosition.x, _isWhite ? _oldPosition.y + 1 : _oldPosition.y - 1, 1));
                // Initial move forward 2
                if ((_isWhite && _oldPosition.y == 2) || (!_isWhite && _oldPosition.y == _gameManager.boardSize - 1))
                {
                    moves.Add(new Vector3(_oldPosition.x, _isWhite ? _oldPosition.y + 2 : _oldPosition.y - 2, 1));
                }

                // todo: En Passant
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
                for (var i = 0; i < _gameManager.boardSize; i++)
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
                for (var i = 0; i < _gameManager.boardSize; i++)
                {
                    // Vertical
                    moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y + i));
                    moves.Add(new Vector2(_oldPosition.x - i, _oldPosition.y - i));
                    moves.Add(new Vector2(_oldPosition.x - i, _oldPosition.y + i));
                    moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y - i));
                }

                break;
            case "Rook":
                for (var i = 0; i < _gameManager.boardSize; i++)
                {
                    // Forward/Backwards
                    moves.Add(new Vector2(_oldPosition.x, _oldPosition.y + i));
                    moves.Add(new Vector2(_oldPosition.x, _oldPosition.y - i));

                    // Left/Right
                    moves.Add(new Vector2(_oldPosition.x + i, _oldPosition.y));
                    moves.Add(new Vector2(_oldPosition.x - i, _oldPosition.y));
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
        var allPieces = GameObject.FindGameObjectsWithTag("Pieces");

        foreach (var move in moves.ToArray())
        {
            foreach (var piece in allPieces.ToArray())
            {
                var piecePosition = (Vector2) piece.transform.position;

                // Check if the move is overlapping any friendly pieces
                var pieceScript = piece.GetComponent<Piece>();
                var isMoveOverlapping = move == piecePosition && pieceScript._isWhite == _isWhite;

                // Check if the move is out of bounds
                var isMoveOutOfBounds = move.x > _gameManager.boardSize || move.x < 1 ||
                                        move.y > _gameManager.boardSize ||
                                        move.y < 1;

                if (isMoveOutOfBounds || isMoveOverlapping || move == _oldPosition)
                {
                    moves.Remove(move);
                }
            }
        }

        // Remove any moves where the piece would have to jump
        _boxCollider.enabled = false;

        moves = _pieceType switch
        {
            "Rook" => RookRayChecks(moves),
            "Bishop" => BishopRayChecks(moves),
            "Queen" => QueenRayChecks(moves),
            _ => moves
        };

        _boxCollider.enabled = true;

        // Create highlights for the moves
        var allTiles = GameObject.FindGameObjectsWithTag("Tiles");

        foreach (var tile in allTiles)
        {
            foreach (var move in moves)
            {
                if ((Vector2) tile.transform.position != move) continue;

                var tileScript = tile.GetComponent<Tile>();
                tileScript.SetColor(true);
            }
        }

        _moves = moves;
    }

    public void OnMouseDrag()
    {
        // Update the position of the piece to the new mouse positions
        _mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        _mousePosition.z = -1f;
        
        transform.position = _mousePosition;
    }

    public void OnMouseUp()
    {
        // Clear all highlights
        var allTiles = GameObject.FindGameObjectsWithTag("Tiles");

        foreach (var tile in allTiles)
        {
            var tileScript = tile.GetComponent<Tile>();
            tileScript.SetColor(false);
        }

        // Figure out the piece's new position
        var px = Mathf.Round(_mousePosition.x);
        var py = Mathf.Round(_mousePosition.y);
        var newPosition = new Vector2(px, py);
        
        // Check:
        // - the position is in bounds
        // - the position is different to what it previously was
        // - that it is the player's turn
        // - that the move is legal
        var isInBounds = (0 < px && px <= 8) && (0 < py && py <= 8);
        var positionHasChanged = newPosition != _oldPosition;
        var isPlayersTurn = (_isWhite && _gameManager.whiteTurn) || (!_isWhite && !_gameManager.whiteTurn);
        var isMoveLegal = _moves.Contains(new Vector2(newPosition.x, newPosition.y));

        if (isInBounds && positionHasChanged && isPlayersTurn && isMoveLegal)
        {
            MakeMove(gameObject, _oldPosition, newPosition);

            _gameManager.Stockfish.SetPosition(_gameManager.moveNotations);

            // Temporary Stockfish AI for black
            // todo: make this... better
            if (_gameManager.blackIsStockfish)
            {
                var k = 0f;
                var blackMove = _gameManager.Stockfish.GetBestMove().ToLookup(c => Math.Floor(k++ / 2)).Select(e => new string(e.ToArray())).ToArray();
                
                var blackFrom = new Vector2(Array.IndexOf(_alpha, blackMove[0][0]) + 1, 0);
                float.TryParse(blackMove[0][1].ToString(), out blackFrom.y);
                
                var blackTo = new Vector2(Array.IndexOf(_alpha, blackMove[1][0]) + 1, 0);
                float.TryParse(blackMove[1][1].ToString(), out blackTo.y);

                var allPieces = GameObject.FindGameObjectsWithTag("Pieces");
                
                foreach (var piece in allPieces)
                {
                    if ((Vector2) piece.transform.position != blackFrom) continue;
                    MakeMove(piece, blackFrom, blackTo);
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

    private void MakeMove(GameObject piece, Vector2 oldPosition, Vector2 newPosition)
    {
        var pieceScript = piece.GetComponent<Piece>();
        
        // Move the piece
        piece.transform.position = newPosition;
            
        // Play the place sound
        _gameManager.audioManager.PlayPlace();

        // Manage the taking of pieces!
        var allPieces = GameObject.FindGameObjectsWithTag("Pieces");
            
        foreach (var p in allPieces)
        {
            var pScript = p.GetComponent<Piece>();

            // Ensure that the piece does not delete itself
            if (p.transform.position != piece.transform.position || p == piece) continue;
                
            // Protect pieces from being taken by pieces of their own colour
            if (pScript._isWhite == pieceScript._isWhite)
            {
                piece.transform.position = oldPosition;
                return;
            }

            Destroy(p);
        }
        
        // Append the move to notation
        _gameManager.moveNotations.Add($"{GetAlgebraicNotation(oldPosition)}{GetAlgebraicNotation(newPosition)}");
        
        // Invert the turn variable
        _gameManager.whiteTurn ^= true;
    }

    private static string GetAlgebraicNotation(Vector2 position)
    {
        return $"{_alpha[Mathf.RoundToInt(position.x) - 1]}{position.y}";
    }
    
    private Collider2D DrawRay(Vector2 direction, float distance, Color color)
    {
        var position = transform.position;
        var hit = Physics2D.Raycast(position, direction, distance);
        
        if (drawRays) Debug.DrawRay(position, direction * distance, color, 3);

        return hit.collider;
    }
    
    private List<Vector2> RookRayChecks(List<Vector2> moves)
    {
        foreach (var direction in _rookDirections.ToArray())
        {
            var ray = DrawRay(direction, _gameManager.boardSize, Color.blue);

            if (ray != null)
            {
                var position = transform.position;
                var collidedPosition = ray.transform.position;

                switch (direction.y)
                {
                    // If a piece has hits another piece with its top ray, remove all of the moves ahead of the collided piece
                    case 1:
                        for (var i = 1; i < _gameManager.boardSize + 1; i++)
                        {
                            moves.Remove(new Vector2(position.x, collidedPosition.y + i));
                        }

                        break;

                    // If a piece has hits another piece with its bottom ray, remove all of the moves behind the collided piece
                    case -1:
                        for (var i = 1; i < _gameManager.boardSize + 1; i++)
                        {
                            moves.Remove(new Vector2(position.x, collidedPosition.y - i));
                        }

                        break;
                }

                switch (direction.x)
                {
                    // If a piece hits another piece with its left ray, remove all of the moves to the left of the collided piece
                    case -1:
                        for (var i = 0; i < _gameManager.boardSize + 1; i++)
                        {
                            moves.Remove(new Vector2(collidedPosition.x - i, position.y));
                        }

                        break;

                    // If a piece hits another piece with its right ray, remove all of the moves to the right of the collided piece
                    case 1:
                        for (var i = 0; i < _gameManager.boardSize + 1; i++)
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
        foreach (var direction in _bishopDirections.ToArray()) {
            var ray = DrawRay(direction, _gameManager.boardSize, Color.red);

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
                                for (var i = 0; i < _gameManager.boardSize + 1; i++)
                                {
                                    moves.Remove(new Vector2(collidedPosition.x - i, collidedPosition.y + i));
                                }

                                break;

                            // ...right...
                            case 1:
                                for (var i = 0; i < _gameManager.boardSize + 1; i++)
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
                                for (var i = 0; i < _gameManager.boardSize + 1; i++)
                                {
                                    moves.Remove(new Vector2(collidedPosition.x - i, collidedPosition.y - i));
                                }

                                break;

                            // ...right...
                            case 1:
                                for (var i = 0; i < _gameManager.boardSize + 1; i++)
                                {
                                    moves.Remove(new Vector2(collidedPosition.x + i, collidedPosition.y - i));
                                }

                                break;
                        }

                        break;
                }

                // Add back the collided position to the moves, as it has to be capturable
                if (_isWhite != ray.gameObject.GetComponent<Piece>()._isWhite) moves.Add(collidedPosition);
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