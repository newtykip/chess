using System;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private GameManager _manager;
    private Vector3 _screenPoint;
    private Vector3 _offset;

    private void Start()
    {
        // Find and assign the game manager
        _manager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
    }
    
    void OnMouseDown()
    {
        // Handle the initial grabbing of the piece
        _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
    }
 
    // Handle the dragging of the piece
    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
        transform.position = curPosition;
    }
}