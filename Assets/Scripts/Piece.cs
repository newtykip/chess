﻿using System;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private GameManager _manager;
    private Vector3 _mousePosition;
    private Vector3 _screenPoint;
    private Vector2 _oldPosition;
    private bool _isWhite;

    public void Start()
    {
        // Find and assign the game manager
        _manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        
        // Determine the piece's colour
        _isWhite = gameObject.name.Contains("White");
    }
    
    public void OnMouseDown()
    {
        // Save the old piece position
        _oldPosition = transform.position;
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