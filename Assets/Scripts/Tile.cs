using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private readonly Color32 _lightColour = new Color32(240, 217, 183, 255);
    private readonly Color32 _darkColour = new Color32(180, 136, 102, 255);
    private readonly Color32 _lightHighlight = new Color32(140, 104, 76, 255);
    private readonly Color32 _darkHighlight = new Color32(124, 91, 67, 255);

    public bool isLight;
    public bool highlighted = false;

    public void Start()
    {
        // Grab the sprite renderer
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Colour the tile
        var position = transform.position;
        isLight = (position.x + position.y) % 2 == 0;
        SetDefaultColour();

        // Name the tile
        gameObject.name = $"({position.x}, {position.y})";
    }

    public void Highlight()
    {
        _spriteRenderer.color = isLight ? _lightHighlight : _darkHighlight;
        highlighted ^= highlighted;
    }

    public void SetDefaultColour()
    {
        _spriteRenderer.color = isLight ? _lightColour : _darkColour;
    }
}
