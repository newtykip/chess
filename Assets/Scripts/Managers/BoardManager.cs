using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
	public GameObject tilePrefab;
	public GameObject board;
	public int size = 8;

	public void DrawTiles()
	{
		// Draw the board's tiles
		for (var x = 1; x < size + 1; x++)
		{
			for (var y = 1; y < size + 1; y++)
			{
				// Create the tile
				Instantiate(tilePrefab, new Vector3(x, y, 1), Quaternion.identity, board.transform);
			}
		}
	}
}
