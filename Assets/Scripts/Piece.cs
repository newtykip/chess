using System;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private GameManager _manager;
    private Vector3 _screenPoint;
    private Vector3 _offset;

    public void Start()
    {
        // Find and assign the game manager
        _manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
    }

    // When the piece has been clicked
    public void OnMouseDown()
    {
        // Handle the initial grabbing of the piece
        var position = transform.position;
        
        _screenPoint = Camera.main.WorldToScreenPoint(position);
        _offset = position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
    }
 
    // Handle the dragging of the piece
    public void OnMouseDrag()
    {
        var curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
        var curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
        
        transform.position = curPosition;
    }
    
    // When the mouse has been released
    public void OnMouseUp()
    {
        // Snap to the nearest block
        var position = transform.position;
        var x = Mathf.Round(position.x);
        var y = Mathf.Round(position.y);

        transform.position = new Vector3(x, y, position.z);
    }
}