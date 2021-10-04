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

