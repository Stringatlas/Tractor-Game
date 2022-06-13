using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorMove : MonoBehaviour
{
	[SerializeField] Rigidbody rigidBody;

	public Transform resetPos;
	public Transform respawnPos;

	[SerializeField] Transform leftFrontWheel;
	[SerializeField] Transform rightFrontWheel;
	[SerializeField] Transform rightBackWheel;
	[SerializeField] Transform leftBackWheel;

	[SerializeField] WheelCollider leftFrontWheelCollider;
	[SerializeField] WheelCollider rightFrontWheelCollider;
	[SerializeField] WheelCollider leftBackWheelCollider;
	[SerializeField] WheelCollider rightBackWheelCollider;

	[SerializeField] float maxSteeringAngle;
	[SerializeField] float movementSpeed;
	[SerializeField] float steerAngleChangeSpeed;

	[SerializeField] ParticleSystem respawnParticleSystem;
	[SerializeField] Transform centerOfMass;

	[SerializeField] Transform steeringWheel;
	[SerializeField] float steeringWheelDegrees;
	[SerializeField] float steeringWheelDamping;

	[SerializeField] LayerMask terrainGroundLayer;
	[SerializeField] bool alwaysDrive = false;

	float mass;
	float xInput;
	float yInput;
	float originalMaxSteeringAngle;
	float fastMaxSteeringAngle;
	float idleBrakeTorque;

	private void Awake()
	{
		rigidBody.centerOfMass = centerOfMass.localPosition;
		originalMaxSteeringAngle = maxSteeringAngle;
		fastMaxSteeringAngle = maxSteeringAngle / 2f;
		idleBrakeTorque = rigidBody.mass;
	}

	private void Start()
	{
		mass = rigidBody.mass + leftBackWheelCollider.mass + rightBackWheelCollider.mass + leftFrontWheelCollider.mass + rightFrontWheelCollider.mass;
		if (alwaysDrive)
		{
			GameManager.IsDrivingInTractor = true;
		}
	}

	public void Respawn(Vector3 position, Quaternion rotation)
	{
		respawnParticleSystem.Play();
		transform.SetPositionAndRotation(position, rotation);
		rigidBody.velocity = Vector3.zero;
		rigidBody.angularVelocity = Vector3.zero;
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
		
	}

	public void RespawnWithNormals(Vector3 raycastPosition)
	{
		raycastPosition.y = EndlessTerrain.mapGenerator.terrainData.meshHeightMultiplier;
		if (Physics.Raycast(raycastPosition, -1f * Vector3.up, out RaycastHit hit, Mathf.Infinity, terrainGroundLayer))
		{
			Quaternion rotation = Quaternion.LookRotation(Vector3.right, 180f * hit.normal);
			raycastPosition.y = hit.point.y + 7f * transform.localScale.x;
			Respawn(raycastPosition, rotation);

			transform.rotation = rotation;
			Debug.Log(rotation);
		}
		else
		{
			Debug.Log("failed");
			Respawn(raycastPosition, Quaternion.identity);
		}
	}
	private void FixedUpdate()
	{
		rigidBody.AddForce(Physics.gravity, ForceMode.Acceleration);
		UpdateWheels();
		if (!GameManager.IsDrivingInTractor && !alwaysDrive)
		{
			return;
		}
		if (transform.position.y < -100f)
		{
			Respawn(respawnPos.position, respawnPos.rotation);
		}

		yInput = Input.GetAxis("Vertical");
		xInput = Input.GetAxis("Horizontal");

		if (yInput == 0f)
		{
			leftBackWheelCollider.brakeTorque = idleBrakeTorque;
			rightBackWheelCollider.brakeTorque = idleBrakeTorque;
			leftFrontWheelCollider.brakeTorque = idleBrakeTorque;
			rightFrontWheelCollider.brakeTorque = idleBrakeTorque;
		}
		else
		{
			leftBackWheelCollider.brakeTorque = 0;
			rightBackWheelCollider.brakeTorque = 0;
			leftFrontWheelCollider.brakeTorque = 0;
			rightFrontWheelCollider.brakeTorque = 0;
		}

		float steeringAngle = xInput * maxSteeringAngle;
		leftFrontWheelCollider.steerAngle = steeringAngle;
		rightFrontWheelCollider.steerAngle = steeringAngle;

		float currentSpeed = rigidBody.velocity.sqrMagnitude;
		if (currentSpeed < steerAngleChangeSpeed)
		{
			maxSteeringAngle = originalMaxSteeringAngle;
		}
		else
		{
			maxSteeringAngle = fastMaxSteeringAngle;
		}
		float motorTorque = yInput * movementSpeed * mass;
		leftBackWheelCollider.motorTorque = motorTorque;
		rightBackWheelCollider.motorTorque = motorTorque;

	}

	private void Update()
	{
		if (!GameManager.IsDrivingInTractor)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			RespawnWithNormals(respawnPos.position);
		}

		if (Input.GetKey(KeyCode.B))
		{
			leftBackWheelCollider.brakeTorque = Mathf.Infinity;
			rightBackWheelCollider.brakeTorque = Mathf.Infinity;
			leftFrontWheelCollider.brakeTorque = Mathf.Infinity;
			rightFrontWheelCollider.brakeTorque = Mathf.Infinity;
		}
		Quaternion targetRotation = transform.rotation * Quaternion.Euler(maxSteeringAngle * -xInput * Vector3.forward);

		Quaternion rotation = Quaternion.RotateTowards(steeringWheel.rotation, targetRotation, Time.deltaTime * steeringWheelDamping);

		steeringWheel.rotation = rotation;
	}

	void UpdateWheels()
	{
		rightFrontWheelCollider.GetWorldPose(out Vector3 right_front_position, out Quaternion right_front_rotation);
		rightFrontWheel.SetPositionAndRotation(right_front_position, right_front_rotation);

		leftFrontWheelCollider.GetWorldPose(out Vector3 left_front_position, out Quaternion left_front_rotation);
		leftFrontWheel.SetPositionAndRotation(left_front_position, left_front_rotation);

		rightBackWheelCollider.GetWorldPose(out Vector3 right_back_position, out Quaternion right_back_rotation);
		rightBackWheel.SetPositionAndRotation(right_back_position, right_back_rotation);

		leftBackWheelCollider.GetWorldPose(out Vector3 left_back_position, out Quaternion left_back_rotation);
		leftBackWheel.SetPositionAndRotation(left_back_position, left_back_rotation);
	}
}
