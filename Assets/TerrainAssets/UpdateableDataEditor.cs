using UnityEditor;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR

[CustomEditor(typeof(UpdateableData), true)]
public class UpdateableDataEditor : Editor
{

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		UpdateableData updateableData = (UpdateableData)target;

		if (GUILayout.Button("Update"))
		{
			updateableData.NotifyOfUpdatedValues();
		}
	}

}

#endif