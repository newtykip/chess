using System;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private GameManager _manager;
    private Vector3 _mousePosition;
    private Vector3 _screenPoint;
    private Vector3 _oldPiecePosition;

    public void Start()
    {
        // Find and assign the game manager
        _manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
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

        // Ensure the position is in bounds of the board
        if ((0 < px && px <= 8) && (0 < py && py <= 8))
        {
            // Snap the piece
            transform.position = new Vector3(px, py);
        }
        else
        {
            transform.position = _oldPiecePosition;
        }
    }
}