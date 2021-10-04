using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public GameObject arrows;
    public Material arrowMaterial;

    private void Start()
    {
        DrawArrow(new Vector2(1, 1), new Vector2(3, 3));
    }

    void DrawArrow(Vector2 start, Vector2 end)
    {
        // Instantiate the arrow
        var arrowGameObject = new GameObject($"Arrow #{arrows.transform.childCount + 1}");
        arrowGameObject.transform.parent = arrows.transform;
     
        // Create the line renderer
        var lineRenderer = arrowGameObject.AddComponent<LineRenderer>();
        
        // Configure the line renderer
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.material = arrowMaterial;
        lineRenderer.SetPosition(0, new Vector3(start.x, start.y, -1));
        lineRenderer.SetPosition(1, new Vector3(end.x, end.y, -1));
    }
}