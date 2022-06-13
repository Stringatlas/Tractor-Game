using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileType", menuName = "Scriptable Objects/Projectile Type")]
public class ProjectileType : ScriptableObject
{
	public GameObject gameObject;
	public float moveSpeedPerSec;
	public float lifetimeInSec;
}
