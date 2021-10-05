using System;
using System.Linq;
using UnityEngine;

public class StockfishManager : MonoBehaviour
{
	private GameManager _gameManager;
	private NETfish.Stockfish _stockfish;
	private char[] _alphabet;
	public bool isEnabled = false;
	public bool isWhite = false;

	public void Start()
	{
		// Find the game manager
		_gameManager = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
		_alphabet = _gameManager.alphabet;

		// Start Stockfish
		_stockfish = new NETfish.Stockfish($"{Application.streamingAssetsPath}/stockfish14.exe");
	}

	public MoveSet GetBestMove()
	{
		_stockfish.SetPosition(_gameManager.moveNotations);

		var k = 0f;
		var stockfishMove = _stockfish.GetBestMove().ToLookup(c => Math.Floor(k++ / 2)).Select(e => new string(e.ToArray())).ToArray();

		var stockfishFrom = new Vector2(Array.IndexOf(_alphabet, stockfishMove[0][0]) + 1, 0);
		float.TryParse(stockfishMove[0][1].ToString(), out stockfishFrom.y);

		var stockfishTo = new Vector2(Array.IndexOf(_alphabet, stockfishMove[1][0]) + 1, 0);
		float.TryParse(stockfishMove[1][1].ToString(), out stockfishTo.y);

		return new MoveSet(stockfishFrom, stockfishTo);
	}
}
