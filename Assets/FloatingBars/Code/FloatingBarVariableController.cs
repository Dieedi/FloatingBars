using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingBarVariableController : MonoBehaviour
{

	//==================================================
	// Use as example to manage your Floating bar values
	//==================================================

	public FloatVariable currentValue;
	public bool resetValue;
	public FloatVariable startingValue; // Max Value
	public FloatVariable RegenRate;

	private bool regenerating = false;
	
	void Awake()
	{
		// instantiate the ScriptableObject to have a unique reference
		currentValue = Instantiate(currentValue);

		if (resetValue)
			currentValue.SetValue(startingValue);
	}
	
	void Update()
	{
		if (currentValue.Value < startingValue.Value && !regenerating) {
			regenerating = true;
			StartCoroutine("RegenResource");
		}
	}

	IEnumerator RegenResource()
	{
		while (currentValue.Value < startingValue.Value) {
			yield return new WaitForSeconds(1f);

			if (currentValue.Value >= startingValue.Value) {
				regenerating = false;
			} else {
				if (RegenRate != null)
					currentValue.ApplyChange(RegenRate.Value);
				else
					currentValue.ApplyChange(0f);
			}
		}
	}
}
