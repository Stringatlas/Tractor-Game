using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4ThirdPersonMovement : MonoBehaviour
{
	public CharacterController controller;
	public float speed;
	public float turnSmoothTime = 0.1f;
	public Transform cam;

	float turnSmoothVelocity;

	void Update()
	{
		Move();

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			transform.position =  Vector3.zero;
		}

	}

	// moves the character 
	void Move()
	{
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");

		Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

		float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

		transform.rotation = Quaternion.Euler(0f, angle, 0f);

		if (direction.magnitude >= 0.1f)
		{
			Vector3 moveDirection = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized;

			controller.Move(moveDirection * speed * Time.deltaTime);
		}
	}
}
