using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public AudioManager audioManager;
	public PieceManager pieceManager;
	public ArrowManager arrowManager;
	public BoardManager boardManager;

	public bool whiteTurn = true;
	public bool stockfishEnabled = false;
	public List<string> moveNotations = new List<string>();
	public Stockfish Stockfish;

	public Color32 orange = new Color32(255, 165, 0, 200);
	public Color32 red = new Color32(255, 50, 50, 200);
	public Color32 blue = new Color32(50, 150, 255, 200);
	public Color32 green = new Color32(50, 255, 50, 200);

	public void Start()
	{
		// Start Stockfish
		Stockfish = new Stockfish($"{Application.streamingAssetsPath}/stockfish14.exe");

		// Begin the game
		boardManager.DrawTiles();
		pieceManager.DrawPieces();
	}

	public void Update()
	{
		// Handle global keybinds
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}

