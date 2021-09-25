using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Color32 _defaultColour;
    private Color32 _highlightColour;

    public void Start()
    {
        // Grab the sprite renderer
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Figure out the tiles colours
        var position = transform.position;
        var isLight = (position.x + position.y) % 2 == 0;
        
        _defaultColour = isLight ? new Color32(240, 217, 183, 255) : new Color32(180, 136, 102, 255);
        _highlightColour = isLight ? new Color32(140, 104, 76, 255) : new Color32(124, 91, 67, 255);

        _spriteRenderer.color = _defaultColour;

        // Name the tile
        gameObject.name = $"({position.x}, {position.y})";
    }

    public void SetColor(bool highlighted)
    {
        _spriteRenderer.color = highlighted ? _highlightColour : _defaultColour;
    }
}
