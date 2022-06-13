using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorFirstPersonCamera : MonoBehaviour
{
	[SerializeField] float mouseSensitivity = 500f;
	[SerializeField] float zoomedFOV = 15f;
	float normalFOV;
	Camera cam;

	[SerializeField] float maxYRotation = 165f;

	public float xRotation;
	public float yRotation;

	private void Start()
	{
		try
		{
			cam = transform.GetComponent<Camera>();
		}
		catch
		{
			Debug.LogError($"No Camera Attached To {transform.gameObject.name} For TractorFirstPersonCamera");
		}
		
		normalFOV = cam.fieldOfView;
	}

	private void Update()
	{
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90, 90);

		yRotation += mouseX;
		yRotation = Mathf.Clamp(yRotation, -maxYRotation, maxYRotation);

		transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

		if (Input.GetAxisRaw("Zoom") == 1)
		{
			cam.fieldOfView = zoomedFOV;
		}
		else
		{
			cam.fieldOfView = normalFOV;
		}
	}
}
