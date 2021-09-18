using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private GameManager _manager;
    private Vector3 _mousePosition;
    private Vector3 _screenPoint;
    private Vector2 _oldPosition;
    private bool _isWhite;
    private string _pieceType;

    public void Start()
    {
        // Find and assign the game manager
        _manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        
        // Determine the piece's colour and type
        _isWhite = gameObject.name.Contains("White");
        _pieceType = _isWhite
            ? gameObject.name.Split(new[] {"White"}, StringSplitOptions.None)[1].Trim()
            : gameObject.name.Split(new[] {"Black"}, StringSplitOptions.None)[1].Trim();
    }
    
    public void OnMouseDown()
    {
        // Save the old piece position
        _oldPosition = transform.position;
        
        // Figure out potential moves
        var moves = new List<Vector3>();

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
            // todo: Queen
            // todo: Bishop
            // todo: Knight
            // todo: Rook
        }

        // Filter moves
        var allPieces = GameObject.FindGameObjectsWithTag("Pieces");
        var toRemove = new List<Vector3>();

        for (var i = 0; i < moves.Count; i++)
        {
            for (var j = 0; j < allPieces.Length; j++)
            {
                var move = moves[i];
                var isMoveOutOfBounds = move.x > _manager.boardSize || move.x < 1 || move.y > _manager.boardSize ||
                                        move.y < 1;
                var piece = allPieces[j];
                var piecePosition = piece.transform.position;
                piecePosition.z = 1;

                var isMoveOverlapping = move == piecePosition;

                if (isMoveOverlapping || isMoveOutOfBounds)
                {
                    toRemove.Add(move);
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
                    var spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = Tile.HighlightedColour;
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
            _manager.ColourTile(tile);
        }

        // Figure out the piece's new position
        var px = Mathf.Round(_mousePosition.x);
        var py = Mathf.Round(_mousePosition.y);
        var newPosition = new Vector2(px, py);
        
        // Ensure the position is in bounds of the board, that it has changed, and that it is the player's turn
        var isInBounds = (0 < px && px <= 8) && (0 < py && py <= 8);
        var positionHasChanged = newPosition != _oldPosition;
        var isPlayersTurn = (_isWhite && _manager.whiteTurn) || (!_isWhite && !_manager.whiteTurn);
        
        if (isInBounds && positionHasChanged && isPlayersTurn)
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
    }
}