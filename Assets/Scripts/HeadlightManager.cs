using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadlightManager : MonoBehaviour
{
	[SerializeField] Transform[] lights;

	private void Update()
	{
		if (GameManager.IsDrivingInTractor && Input.GetKeyDown(KeyCode.L))
		{
			foreach (Transform light in lights)
			{
				light.gameObject.SetActive(!light.gameObject.activeInHierarchy);
			}
		}
	}
}
