using UnityEngine;

public class Tile : MonoBehaviour
{
	public bool isLight;
	public bool indicator = false;
	public bool highlighted = false;
	public Color32 defaultColour;
	public Color32 currentColour;
	private SpriteRenderer _spriteRenderer;

	public void Start()
	{
		// Grab the sprite renderer
		_spriteRenderer = GetComponent<SpriteRenderer>();

		// Figure out the tiles colours
		var position = transform.position;
		isLight = (position.x + position.y) % 2 == 0;

		defaultColour = isLight ? new Color32(240, 217, 183, 255) : new Color32(180, 136, 102, 255);
		_spriteRenderer.color = defaultColour;
		currentColour = _spriteRenderer.color;

		// Name the tile
		gameObject.name = $"({position.x}, {position.y})";
	}
}
