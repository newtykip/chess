using System.Collections.Generic;
using System.Linq;
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

    private GameManager _gameManager;
    private Vector3 _mousePosition;
    private Vector3 _screenPoint;
    private Vector2 _oldPosition;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private List<Vector2> _moves = new List<Vector2>();
    private readonly float _rayDistance = 11.5f;
    private bool _isWhite;
    private string _pieceType;
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

        // Remove any moves where the piece would have to jump, unless the piece is a knight
        if (_pieceType != "Knight")
        {
            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    // Temporarily disable box colliders so that rays do not collide with the piece's own box collider
                    _boxCollider.enabled = false;

                    var direction = new Vector2(x, y);

                    var up = direction.Equals(Vector2.up);
                    var down = direction.Equals(Vector2.down);
                    var bottomLeft = direction.Equals(Vector2.down + Vector2.left);
                    var bottomRight = direction.Equals(Vector2.down + Vector2.right);
                    var topLeft = direction.Equals(Vector2.up + Vector2.left);
                    var topRight = direction.Equals(Vector2.up + Vector2.right);

                    switch (_pieceType)
                    {
                        case "Rook":
                            if (!(bottomLeft || bottomRight || topLeft || topRight))
                            {
                                var rookRay = DrawRay(direction);
                                if (rookRay != null) moves = RookRayChecks(rookRay, direction, moves);
                            }

                            break;
                        case "Bishop":
                            if (!(up || down))
                            {
                                var bishopRay = DrawRay(direction);
                                if (bishopRay != null) moves = BishopRayChecks(bishopRay, direction, moves);
                            }
                            
                            break;
                        case "Queen":
                            var queenRay = DrawRay(direction);

                            if (queenRay != null)
                            {
                                moves = RookRayChecks(queenRay, direction, moves);
                                moves = BishopRayChecks(queenRay, direction, moves);
                            }

                            break;
                    }

                    _boxCollider.enabled = true;
                }
            }
        }

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
            // Snap the piece
            transform.position = newPosition;
            
            // Play the place sound
            _gameManager.audioManager.PlayPlace();

            // Manage the taking of pieces!
            var allPieces = GameObject.FindGameObjectsWithTag("Pieces");
            
            foreach (var piece in allPieces)
            {
                // Ensure that the piece does not delete itself
                if (piece.transform.position != transform.position || piece == gameObject) continue;
                
                // Protect pieces from being taken by pieces of their own colour
                if ((piece.GetComponent<Piece>()._isWhite && _isWhite) || (!piece.GetComponent<Piece>()._isWhite && !_isWhite))
                {
                    transform.position = _oldPosition;
                    return;
                }

                Destroy(piece);
            }

            // Invert the turn variable
            _gameManager.whiteTurn ^= true;
        }
        else
        {
            transform.position = _oldPosition;
        }
        
        // Clear the moves
        _moves.Clear();
    }
    
    private Collider2D DrawRay(Vector2 direction)
    {
        var position = transform.position;
        var hit = Physics2D.Raycast(position, direction, _rayDistance);
        Debug.DrawRay(position, direction * _rayDistance, Color.red, 3);

        return hit.collider;
    }
    
    private List<Vector2> RookRayChecks(Collider2D ray, Vector2 direction, List<Vector2> moves)
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

        return moves;
    }

    private List<Vector2> BishopRayChecks(Collider2D ray, Vector2 direction, List<Vector2> moves)
    {
        var position = transform.position;
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
        
        // Add back the collided position to the moves, as it has to be takeable
        if (_isWhite != ray.gameObject.GetComponent<Piece>()._isWhite) moves.Add(collidedPosition);

        return moves;
    }
}