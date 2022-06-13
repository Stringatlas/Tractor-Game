using UnityEngine;

public class CenterOfMassSetter : MonoBehaviour
{
	[SerializeField] Rigidbody rigidBody;
	[SerializeField] Transform centerOfMass;

	private void Awake()
	{
		if (centerOfMass != null)
			rigidBody.centerOfMass = centerOfMass.position;
	}
}
