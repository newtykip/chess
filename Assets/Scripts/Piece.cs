﻿using System.Collections.Generic;
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
    
    private GameManager _manager;
    private Vector3 _mousePosition;
    private Vector3 _screenPoint;
    private Vector2 _oldPosition;
    private SpriteRenderer _spriteRenderer;
    private List<Vector3> moves = new List<Vector3>();

    private bool _isWhite;
    private string _pieceType;
    public char code;

    public void Start()
    {
        // Find and assign the game manager
        _manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        
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
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

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
        switch (_pieceType)
        {
            case "Pawn":
                // Move forward 1
                moves.Add(new Vector3(_oldPosition.x, _isWhite ? _oldPosition.y + 1 : _oldPosition.y - 1, 1));
                // Initial move forward 2
                if ((_isWhite && _oldPosition.y == 2) || (!_isWhite && _oldPosition.y == _manager.boardSize - 1))
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
                        moves.Add(new Vector3(_oldPosition.x + i, _oldPosition.y + j, 1));
                    }
                }
                break;
            case "Queen":
                for (var i = 0; i < _manager.boardSize; i++)
                {
                    // Forward/Backwards
                    moves.Add(new Vector3(_oldPosition.x, _oldPosition.y + i, 1));
                    moves.Add(new Vector3(_oldPosition.x, _oldPosition.y - i, 1));
                    
                    // Left/Right
                    moves.Add(new Vector3(_oldPosition.x + i, _oldPosition.y, 1));
                    moves.Add(new Vector3(_oldPosition.x - i, _oldPosition.y, 1));
                    
                    // Vertical
                    moves.Add(new Vector3(_oldPosition.x + i, _oldPosition.y + i, 1));
                    moves.Add(new Vector3(_oldPosition.x - i, _oldPosition.y - i, 1));
                    moves.Add(new Vector3(_oldPosition.x - i, _oldPosition.y + i, 1));
                    moves.Add(new Vector3(_oldPosition.x + i, _oldPosition.y - i, 1));
                }
                break;
            case "Bishop":
                for (var i = 0; i < _manager.boardSize; i++)
                {
                    // Vertical
                    moves.Add(new Vector3(_oldPosition.x + i, _oldPosition.y + i, 1));
                    moves.Add(new Vector3(_oldPosition.x - i, _oldPosition.y - i, 1));
                    moves.Add(new Vector3(_oldPosition.x - i, _oldPosition.y + i, 1));
                    moves.Add(new Vector3(_oldPosition.x + i, _oldPosition.y - i, 1));
                }
                break;
            case "Rook":
                for (var i = 0; i < _manager.boardSize; i++)
                {
                    // Forward/Backwards
                    moves.Add(new Vector3(_oldPosition.x, _oldPosition.y + i, 1));
                    moves.Add(new Vector3(_oldPosition.x, _oldPosition.y - i, 1));
                    
                    // Left/Right
                    moves.Add(new Vector3(_oldPosition.x + i, _oldPosition.y, 1));
                    moves.Add(new Vector3(_oldPosition.x - i, _oldPosition.y, 1));
                }
                break;
            case "Knight":
                // L
                moves.Add(new Vector3(_oldPosition.x + 1, _oldPosition.y + 2, 1));
                moves.Add(new Vector3(_oldPosition.x + 1, _oldPosition.y - 2, 1));
                moves.Add(new Vector3(_oldPosition.x + 2, _oldPosition.y + 1, 1));
                moves.Add(new Vector3(_oldPosition.x + 2, _oldPosition.y - 1, 1));
                moves.Add(new Vector3(_oldPosition.x - 1, _oldPosition.y + 2, 1));
                moves.Add(new Vector3(_oldPosition.x - 1, _oldPosition.y - 2, 1));
                moves.Add(new Vector3(_oldPosition.x - 2, _oldPosition.y + 1, 1));
                moves.Add(new Vector3(_oldPosition.x - 2, _oldPosition.y - 1, 1));
                break;
        }

        // Filter moves
        var allPieces = GameObject.FindGameObjectsWithTag("Pieces");
        var toRemove = new List<Vector3>();

        for (var i = 0; i < moves.Count; i++)
        {
            var move = moves[i];
            
            for (var j = 0; j < allPieces.Length; j++)
            {
                var piece = allPieces[j];
                var piecePosition = piece.transform.position;
                piecePosition.z = 1;
                
                // Check if the move is overlapping any friendly pieces
                var pieceScript = piece.GetComponent<Piece>();
                var isMoveOverlapping = move == piecePosition && pieceScript._isWhite == _isWhite;
                
                // Check if the move is out of bounds
                var isMoveOutOfBounds = move.x > _manager.boardSize || move.x < 1 || move.y > _manager.boardSize ||
                                        move.y < 1;

                if (isMoveOutOfBounds)
                {
                    toRemove.Add(move);
                }
                
                else if (isMoveOverlapping)
                {
                    toRemove.Add(move);
                    
                    // todo: make sure that pieces can not jump over others
                }
            }
        }

        moves.RemoveAll(m => toRemove.Contains(m));

        // Create highlights for the moves
        var allTiles = GameObject.FindGameObjectsWithTag("Tiles");

        foreach (var tile in allTiles)
        {
            foreach (var move in moves)
            {
                if (tile.transform.position == move)
                {
                    var tileScript = tile.GetComponent<Tile>();
                    tileScript.Highlight();
                }
            }
        }
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
            tileScript.SetDefaultColour();
        }

        // Figure out the piece's new position
        var px = Mathf.Round(_mousePosition.x);
        var py = Mathf.Round(_mousePosition.y);
        var newPosition = new Vector2(px, py);
        
        // Check:
        // - the position is in bounds
        // - the position is different to what it previously was
        // - that it is the player's turn
        // - the move is legal
        var isInBounds = (0 < px && px <= 8) && (0 < py && py <= 8);
        var positionHasChanged = newPosition != _oldPosition;
        var isPlayersTurn = (_isWhite && _manager.whiteTurn) || (!_isWhite && !_manager.whiteTurn);
        var isMoveLegal = moves.Contains(new Vector3(newPosition.x, newPosition.y, 1));

        if (isInBounds && positionHasChanged && isPlayersTurn && isMoveLegal)
        {
            // Snap the piece
            transform.position = newPosition;

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
            _manager.whiteTurn ^= true;
        }
        else
        {
            transform.position = _oldPosition;
        }
        
        // Clear the moves
        moves.Clear();
    }
}