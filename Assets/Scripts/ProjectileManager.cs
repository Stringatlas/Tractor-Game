using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
	[SerializeField] ProjectileType cornProjectile;
	[SerializeField] int startingPooledCornProjectiles;

	List<Projectile> firedProjectiles;
	List<Projectile> availibleProjectiles;

	Transform projectileParentObject;

	[SerializeField] LayerMask ignoreProjectileLayer;

	float cornProjectileLifetime;

	void FireProjectile(Vector3 position, Quaternion rotation)
	{
		if (availibleProjectiles != null && availibleProjectiles.Count > 0)
		{
			Projectile projectile = availibleProjectiles[0];
			projectile.gameObject.transform.SetPositionAndRotation(position, rotation);
			projectile.lifetimeInSec = cornProjectileLifetime;
			availibleProjectiles.RemoveAt(0);
			firedProjectiles.Add(projectile);
			projectile.gameObject.transform.GetChild(0).GetComponent<TrailRenderer>().Clear();
			projectile.gameObject.SetActive(true);
		}
		else
		{
			Projectile projectile = CreateNewProjectile(cornProjectile, position, rotation);
			firedProjectiles.Add(projectile);
		}

	}
	Projectile CreateNewProjectile(ProjectileType projectileType, Vector3 firingStartPos, Quaternion rotation)
	{
		GameObject projectileGameObject = Instantiate(projectileType.gameObject, firingStartPos, rotation);
		projectileGameObject.name = "Projectile";
		projectileGameObject.transform.SetParent(projectileParentObject);
		Projectile projectile = new Projectile(projectileGameObject, projectileType.moveSpeedPerSec, projectileType.lifetimeInSec);
		return projectile;
	}

	private void Awake()
	{
		firedProjectiles = new List<Projectile>();
		ignoreProjectileLayer = 1 << ignoreProjectileLayer;
		availibleProjectiles = new List<Projectile>();
	}

	private void Start()
	{
		projectileParentObject = new GameObject("Projectiles").transform;
		cornProjectileLifetime = cornProjectile.lifetimeInSec;
		for (int i = 0; i < startingPooledCornProjectiles; i++)
		{
			Projectile projectile = CreateNewProjectile(cornProjectile, Vector3.zero, Quaternion.identity);
			availibleProjectiles.Add(projectile);
			projectile.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			FireProjectile(transform.position, transform.rotation);
		}

		int projectilesRemoved = 0;
		for (int i = 0; i < firedProjectiles.Count; i++)
		{
			Projectile projectile = firedProjectiles[i - projectilesRemoved];

			Transform projectileTransform = projectile.gameObject.transform;
			Vector3 positionChange = projectile.moveSpeedPerSec * Time.deltaTime * projectileTransform.forward;

			projectileTransform.position += positionChange;
			projectile.lifetimeInSec -= Time.deltaTime;

			
			if (Physics.Raycast(projectileTransform.position, positionChange, out RaycastHit hit, positionChange.magnitude, ignoreProjectileLayer))
			{
				Debug.DrawRay(projectileTransform.position, positionChange, Color.yellow, hit.distance);
				firedProjectiles.RemoveAt(i - projectilesRemoved);
				availibleProjectiles.Add(projectile);
				projectile.gameObject.SetActive(false);
				projectilesRemoved++;
			}
			else
			{
				Debug.DrawRay(projectileTransform.position, positionChange, Color.white, positionChange.magnitude);

				if (projectile.lifetimeInSec <= 0)
				{
					firedProjectiles.RemoveAt(i - projectilesRemoved);
					availibleProjectiles.Add(projectile);
					projectile.gameObject.SetActive(false);
					projectilesRemoved++;
				}
			}
		}
	}
}

class Projectile
{
	public readonly GameObject gameObject;
	public readonly float moveSpeedPerSec;

	public float lifetimeInSec;

	public Projectile(GameObject gameObject, float moveSpeedPerSec, float lifetimeInSec)
	{
		this.gameObject = gameObject;
		this.moveSpeedPerSec = moveSpeedPerSec;
		this.lifetimeInSec = lifetimeInSec;
	}
}