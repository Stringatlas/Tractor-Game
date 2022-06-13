using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraLook : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 500f;
    [SerializeField] Transform player;

    public float xRotation = 0f;

    [SerializeField] float zoomedFOV = 15f;
    private float normalFOV;

    [SerializeField] Camera mainCamera;

    void Start()
    {
        normalFOV = mainCamera.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        player.Rotate(Vector3.up * mouseX);

        if (Input.GetAxisRaw("Zoom") == 1)
        {
            mainCamera.fieldOfView = zoomedFOV;
        }
        else
        {
            mainCamera.fieldOfView = normalFOV;
        }
    }

}
