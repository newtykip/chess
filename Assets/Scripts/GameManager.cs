using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject tilePrefab;
	public GameObject board;
	private int rowCount = 8;
	private int tilePerRow = 8;

	// Start is called before the first frame update
	void Start()
	{
		DrawTiles();
	}

	// Update is called once per frame
	void Update()
	{
	}

	// Draws the board's tiles
	private void DrawTiles()
	{
		for (var x =0; x < rowCount; x++)
		{
			for (var y = 0; y < tilePerRow; y++)
			{
				// Create the tile
				var tile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, board.transform);
				tile.name = $"{x},{y}";

				// If the tile should not be dark, we are finished!
				var isDark = (x + y) % 2 == 0;
				if (!isDark) return;
				
				// Otherwise, make the tile dark
				var spriteRenderer = tile.GetComponent<SpriteRenderer>();
				ColorUtility.TryParseHtmlString("#d18b47", out var colour);
				
				spriteRenderer.color = colour;
			}
		}
	}
}
