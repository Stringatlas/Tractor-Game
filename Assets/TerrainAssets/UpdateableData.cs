using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Updateable Data", menuName = "Updateable Data")]
public class UpdateableData : ScriptableObject
{
	public event System.Action OnValuesUpdated;
	public bool autoUpate;

	#if UNITY_EDITOR
	protected virtual void OnValidate()
	{
		if (autoUpate)
		{
			NotifyOfUpdatedValues();
		}
	}

	public void NotifyOfUpdatedValues()
	{
		if (OnValuesUpdated != null)
		{
			OnValuesUpdated();
		}
	}

	#endif
}
