using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] Transform playerThirdPersonCamera;
	[SerializeField] Transform playerThirdPersonCM;
	[SerializeField] Transform playerFirstPersonCamera;
	[SerializeField] Transform tractorThirdPersonCamera;
	[SerializeField] Transform tractorFirstPersonCamera;

	[SerializeField] Transform UICanvas;

	[SerializeField] float minDistanceToEnterTractor;

	[SerializeField] Transform player;
	[SerializeField] Transform tractor;

	[SerializeField] Transform driver;
	[SerializeField] TractorFirstPersonCamera tractorFirstPersonCameraScript;

	bool thirdpersonCameraEnabled;
	static bool _isDrivingInTractor;

	public static bool IsDrivingInTractor
	{
		get
		{
			return _isDrivingInTractor;
		}
		set
		{
			_isDrivingInTractor = value;
		}
	}
	bool canEnterTractor;

	Transform activeCamera;

	TractorMove tractorMoveScript;

	private void Awake()
	{
		thirdpersonCameraEnabled = false;
		_isDrivingInTractor = false;
	}

	void Start()
	{
		try
		{
			tractorMoveScript = tractor.GetComponent<TractorMove>();
		}
		catch (System.NullReferenceException)
		{
			Debug.LogError("Tractor does not have TractorMove script attached");
		}
		catch (System.Exception ex)
		{
			Debug.LogError(ex);
			Debug.LogError("Error getting script TractorMove on Tractor");
		}

		try
		{
			tractorFirstPersonCameraScript = tractorFirstPersonCamera.GetComponent<TractorFirstPersonCamera>();
		}
		catch (System.Exception)
		{
			Debug.LogError($"Couldn't access script TractorFirstPersonCamera.cs on {tractor.gameObject.name}");
		}
		driver.gameObject.SetActive(false);
		//tractorMoveScript.enabled = false;
		UpdateCameras();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			thirdpersonCameraEnabled = !thirdpersonCameraEnabled;
			UpdateCameras();
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (_isDrivingInTractor)
			{
				_isDrivingInTractor = false;
				ExitTractor();
			}
			else if (canEnterTractor)
			{
				_isDrivingInTractor = true;
				EnterTractor();
			}
		}

		if (!_isDrivingInTractor)
		{
			CheckPlayerTractorDistance();
			UpdateUI();
			if (Input.GetKeyDown(KeyCode.R))
			{
				tractorMoveScript.RespawnWithNormals(tractorMoveScript.respawnPos.position);
			}
		}
		else
		{
			player.position = driver.position;
		}
	}


	void ExitTractor()
	{
		driver.gameObject.SetActive(false);
		//tractorMoveScript.enabled = false;
		player.gameObject.SetActive(true);
		UpdateCameras();
		tractorFirstPersonCameraScript.enabled = false;
		player.position = tractor.position + 5f * player.localScale.x * tractor.right;
	}

	void EnterTractor()
	{
		canEnterTractor = false;
		UICanvas.gameObject.SetActive(false);
		driver.gameObject.SetActive(true);
		//tractorMoveScript.enabled = true;
		player.gameObject.SetActive(false);
		tractorFirstPersonCameraScript.enabled = true;
		UpdateCameras();
	}


	void UpdateUI()
	{
		if (canEnterTractor)
		{
			UICanvas.gameObject.SetActive(true);
			UICanvas.rotation = activeCamera.rotation;
		}
		else
		{
			UICanvas.gameObject.SetActive(false);
		}
	}

	void CheckPlayerTractorDistance()
	{
		if ((tractor.position - player.position).sqrMagnitude < minDistanceToEnterTractor)
		{
			canEnterTractor = true;
		}
		else
		{
			canEnterTractor = false;
		}
	}

	void UpdateCameras()
	{
		playerFirstPersonCamera.gameObject.SetActive(false);
		playerThirdPersonCamera.gameObject.SetActive(false);
		playerThirdPersonCM.gameObject.SetActive(false);
		tractorThirdPersonCamera.gameObject.SetActive(false);
		tractorFirstPersonCamera.gameObject.SetActive(false);

		if (thirdpersonCameraEnabled && !_isDrivingInTractor)
		{
			playerThirdPersonCamera.gameObject.SetActive(true);
			playerThirdPersonCamera.gameObject.SetActive(true);
			activeCamera = playerThirdPersonCamera;
		}
		else if (!thirdpersonCameraEnabled && !_isDrivingInTractor)
		{
			playerFirstPersonCamera.gameObject.SetActive(true);
			activeCamera = playerFirstPersonCamera;
		}
		else if (thirdpersonCameraEnabled && _isDrivingInTractor)
		{
			tractorThirdPersonCamera.gameObject.SetActive(true);
			activeCamera = tractorThirdPersonCamera;
		}
		else
		{
			tractorFirstPersonCamera.gameObject.SetActive(true);
			activeCamera = tractorFirstPersonCamera;
		}
	}
}
