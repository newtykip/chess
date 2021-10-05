using UnityEngine;

public struct MoveSet
{
	public MoveSet(Vector2 from, Vector2 to)
	{
		From = from;
		To = to;
	}

	public Vector2 From { get; }
	public Vector2 To { get; }
}
