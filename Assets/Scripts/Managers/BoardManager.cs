using UnityEngine;

public class BoardManager : MonoBehaviour
{
	public GameObject tilePrefab;
	public GameObject indicatorPrefab;
	public GameObject tileContainer;
	public int size = 8;

	public void DrawTiles()
	{
		// Draw the board's tiles
		for (var x = 1; x < size + 1; x++)
		{
			for (var y = 1; y < size + 1; y++)
			{
				// Create the tile
				Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, tileContainer.transform);
			}
		}
	}

	public void DrawIndicator(GameObject tile, Vector2 move)
	{
		var indicator = Instantiate(indicatorPrefab, new Vector3(move.x, move.y, -3), Quaternion.identity);
		indicator.transform.parent = tile.transform;
		indicator.name = $"{tile.name} Indicator";
	}

	public void ClearIndicators()
	{
		// Check if there is an indicator under each tile - if there is one, destroy it
		foreach (Transform tile in tileContainer.transform)
		{
			if (tile.childCount > 0)
			{
				var indicator = tile.GetChild(0);
				Destroy(indicator.gameObject);
			}
		}
	}
}
