using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private GameManager _manager;
    private Vector3 _mousePosition;
    private Vector3 _screenPoint;
    private Vector3 _oldPiecePosition;
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
        _oldPiecePosition = transform.position;
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
        var px = Mathf.Round(_mousePosition.x);
        var py = Mathf.Round(_mousePosition.y);
        var isInBounds = (0 < px && px <= 8) && (0 < py && py <= 8);

        // Ensure the position is in bounds of the board
        if (isInBounds)
        {
            // Snap the piece
            transform.position = new Vector3(px, py);

            // Manage the taking of pieces!
            var allPieces = GameObject.FindGameObjectsWithTag("Pieces");
            
            foreach (var piece in allPieces)
            {
                // Ensure that the piece does not delete itself
                if (piece.transform.position != transform.position || piece == gameObject) continue;
                
                if (piece.GetComponent<Piece>()._isWhite && _isWhite)
                {
                    transform.position = _oldPiecePosition;
                    return;
                }

                if (!piece.GetComponent<Piece>()._isWhite && !_isWhite)
                {
                    transform.position = _oldPiecePosition;
                    return;
                }

                Destroy(piece);
            }

            // Invert the turn variable
            _manager.whiteTurn ^= true;
        }
        else
        {
            transform.position = _oldPiecePosition;
        }
    }
}