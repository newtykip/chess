using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
	public string prefix;
	public string suffix;
	private int _quantity;
	private TextMeshProUGUI _display;

	public void Start()
	{
		_display = GetComponent<TextMeshProUGUI>();
	}

	private void UpdateDisplay()
	{
		_display.text = $"{prefix ?? ""}{_quantity}{suffix ?? ""}";
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
}
