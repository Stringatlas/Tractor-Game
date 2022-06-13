using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraLook : MonoBehaviour
{
    float xRotation;
    //[SerializeField] float rotationSpeed = 20f;
    [SerializeField] float mouseSensitivity = 500f;
    [SerializeField] Transform player;
    Camera thirdPersonCamera;
    [SerializeField] float zoomedFOV = 15f;
    float normalFOV;

	private void Awake()
	{
        thirdPersonCamera = transform.GetComponent<Camera>();
        normalFOV = thirdPersonCamera.fieldOfView;
    }

	void Start()
	{
        

    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		//float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		//transform.RotateAround(player.position, Vector3.forward, rotationSpeed * mouseY * Time.deltaTime);

		//xRotation -= mouseY;
		//xRotation = Mathf.Clamp(xRotation, -90, 90);
		//transform.LookAt(player);
		//transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


		player.Rotate(Vector3.up * mouseX);

		if (Input.GetAxisRaw("Zoom") == 1)
        {
            thirdPersonCamera.fieldOfView = zoomedFOV;
        }
        else
        {
            thirdPersonCamera.fieldOfView = normalFOV;
        }
    }
}


