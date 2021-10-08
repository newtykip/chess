using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{
	public string prefix;
	public string suffix;
	private int _quantity = 0;

	private void UpdateDisplay()
	{
		GetComponent<TextMeshProUGUI>().text = $"{prefix ?? ""}{_quantity}{suffix ?? ""}";
	}

	public int Add(int amount = 1)
	{
		_quantity += amount;
		UpdateDisplay();

		return _quantity;
	}

	public int Remove(int amount = 1)
	{
		_quantity -= amount;
		UpdateDisplay();

		return _quantity;
	}

	public int Restart()
	{
		_quantity = 0;
		UpdateDisplay();

		return _quantity;
	}
}
