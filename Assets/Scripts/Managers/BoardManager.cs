using UnityEngine;

public class BoardManager : MonoBehaviour
{
	public GameObject tilePrefab;
	public GameObject indicatorPrefab;
	public GameObject tileContainer;
	public int size = 8;

	private GameManager _gameManager;
	private Vector2 _startPosition;


	public void Start()
	{
		// Fetch the game manager
		_gameManager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
	}

	public void Update()
	{
		var mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		var tx = Mathf.Round(mousePosition.x);
		var ty = Mathf.Round(mousePosition.y);
		var tilePosition = new Vector2(tx, ty);

		// If the right click button is lowered
		if (Input.GetMouseButtonDown(1))
		{
			_startPosition = new Vector2(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y));
		}

		// If the right click button has been released
		if (Input.GetMouseButtonUp(1))
		{
			var endPosition = new Vector2(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y));

			if (_startPosition == endPosition)
			{
				// Detect whether the click was on a tile, and if it was figure out which tile it was on
				foreach (Transform tile in tileContainer.transform)
				{
					if ((Vector2)tile.position == tilePosition)
					{
						// Get the tile's components
						var script = tile.gameObject.GetComponent<Tile>();
						var renderer = tile.gameObject.GetComponent<SpriteRenderer>();

						// If the tile was already highlighted, reset its colours
						if (script._highlighted)
						{
							renderer.color = script._defaultColour;
						}
						else
						{
							// Hold down control for orange
							if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
							{
								renderer.color = script._isLight ? new Color32(_gameManager.orange.r, _gameManager.orange.g, _gameManager.orange.b, 255) : _gameManager.orange;
							}

							// Hold down shift for green
							else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
							{
								renderer.color = script._isLight ? new Color32(_gameManager.green.r, _gameManager.green.g, _gameManager.green.b, 255) : _gameManager.green;
							}

							// Hold down alt for blue
							else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
							{
								renderer.color = script._isLight ? new Color32(_gameManager.blue.r, _gameManager.blue.g, _gameManager.blue.b, 255) : _gameManager.blue;
							}

							// The default colour is red
							else
							{
								renderer.color = script._isLight ? new Color32(_gameManager.red.r, _gameManager.red.g, _gameManager.red.b, 255) : _gameManager.red;
							}
						}

						// Invert the highlighted indicator
						script._highlighted = !script._highlighted;
					}
				}
			}
		}

		// If a left click is detected, reset all tile colours
		else if (Input.GetMouseButtonDown(0))
		{
			foreach (Transform tile in tileContainer.transform)
			{
				var script = tile.gameObject.GetComponent<Tile>();
				var renderer = tile.gameObject.GetComponent<SpriteRenderer>();

				renderer.color = script._defaultColour;
				script._highlighted = false;
			}
		}
	}

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
		// todo: check for takes, if there is a take draw a circle instead of a dot
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
