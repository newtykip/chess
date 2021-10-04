using UnityEngine;

public class Tile : MonoBehaviour
{
	public bool _isLight;
	public bool _highlighted = false;
	public Color32 _defaultColour;
	private SpriteRenderer _spriteRenderer;

	public void Start()
	{
		// Grab the sprite renderer
		_spriteRenderer = GetComponent<SpriteRenderer>();

		// Figure out the tiles colours
		var position = transform.position;
		_isLight = (position.x + position.y) % 2 == 0;

		_defaultColour = _isLight ? new Color32(240, 217, 183, 255) : new Color32(180, 136, 102, 255);
		_spriteRenderer.color = _defaultColour;

		// Name the tile
		gameObject.name = $"({position.x}, {position.y})";
	}
}
