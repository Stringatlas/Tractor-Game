using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	public Animator transitonAnimator;
	public float transitionTime = 1f;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.N))
		{
			LoadNextScene();
		}
	}

	public void LoadNextScene()
	{
		StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
	}

	IEnumerator LoadScene(int currentSceneIndex)
	{
		transitonAnimator.SetTrigger("Start");

		yield return new WaitForSeconds(transitionTime);
		if (currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
		{
			SceneManager.LoadScene(0);
		}
		else
		{
			SceneManager.LoadScene(currentSceneIndex + 1);
		}

	}
}
