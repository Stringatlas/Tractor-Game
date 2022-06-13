using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
	GameObject _instance;

	private void Awake()
	{
		DontDestroyOnLoad(this);
		if (_instance == null)
		{
			_instance = gameObject;
		}
		else
		{
			DestroyObject(gameObject);
			return;
		}

		//DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		//DestroyObject(gameObject);
	}
}
