using UnityEngine;

public enum CastleType
{
	Kingside,
	Queenside
}

public struct CastleData
{
	public CastleData(bool possible, CastleType type, GameObject? rook, GameObject? king)
	{
		canCastle = possible;
		Type = type;
		Rook = rook;
		King = king;
	}

	public bool canCastle { get; }
	public CastleType Type { get; }
	public GameObject? Rook { get; }
	public GameObject? King { get; }
}
