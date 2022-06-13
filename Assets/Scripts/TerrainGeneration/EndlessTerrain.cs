using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class EndlessTerrain : MonoBehaviour
{
	//public delegate void TerrainGenerated();
	//public event TerrainGenerated onTerrainGenerated;

	public static float maxViewDistance;
	public Transform player;
	const float minPlayerMovementForChunkUpdate = 30f;
	const float sqrMinPlayerMovementForChunkUpdate = minPlayerMovementForChunkUpdate * minPlayerMovementForChunkUpdate;
	bool hasTurnedOnPlayer;

	Vector2 oldViewerPosition;

	public float viewDistance;

	public static Vector2 viewerPosition;
	int chunkSize;
	int chunksVisible;

	public Transform chunkParent;
	public static MapGenerator mapGenerator;

	public Material mapMaterial;

	static Dictionary<Vector2, TerrainChunk> terrainDictionary = new Dictionary<Vector2, TerrainChunk>();
	static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	void Start()
	{
		mapGenerator = FindObjectOfType<MapGenerator>();

		maxViewDistance = viewDistance;

		chunkSize = mapGenerator.mapChunkSize - 3;
		chunksVisible = Mathf.RoundToInt(maxViewDistance / chunkSize);

		UpdateVisibleChunks();

		//if (onTerrainGenerated != null)
		//{
		//	onTerrainGenerated();
		//}
		//else
		//{
		//	print("nulled");
		//}

	}

	void Update()
	{
		viewerPosition = new Vector2(player.position.x, player.position.z) / mapGenerator.terrainData.uniformScale;
		float sqrPlayerMovement = (oldViewerPosition - viewerPosition).sqrMagnitude;
		if (!(sqrPlayerMovement > sqrMinPlayerMovementForChunkUpdate))
		{
			return;
		}

		UpdateVisibleChunks();


		oldViewerPosition = viewerPosition;
	}

	void UpdateVisibleChunks()
	{
		foreach (TerrainChunk terrainChunk in terrainChunksVisibleLastUpdate)
		{
			terrainChunk.SetChunkVisible(false);
		}

		terrainChunksVisibleLastUpdate.Clear();

		int currentChunkX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
		int currentChunkZ = Mathf.RoundToInt(viewerPosition.y / chunkSize);

		for (int zOffest = -chunksVisible; zOffest <= chunksVisible; zOffest++)
		{
			for (int xOffset = -chunksVisible; xOffset <= chunksVisible; xOffset++)
			{
				Vector2 viewedChunkCoord = new Vector2(currentChunkX + xOffset, currentChunkZ + zOffest);

				if (terrainDictionary.ContainsKey(viewedChunkCoord))
				{
					terrainDictionary[viewedChunkCoord].UpdateChunkVisibility();
				}
				else
				{
					terrainDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, chunkParent, mapMaterial));
					if (!hasTurnedOnPlayer)
					{
						player.gameObject.SetActive(true);
						hasTurnedOnPlayer = true;
					}

				}
			}
		}

	}

	public class TerrainChunk
	{
		GameObject meshObject;
		Vector2 position;
		Vector2 coordinate;
		Bounds bounds;

		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		MeshCollider meshCollider;

		ChunkMesh renderedMesh;


		MapData mapData;
		bool hasMapDataRecieved;

		public TerrainChunk(Vector2 coordinate, int size, Transform parent, Material material)
		{
			this.coordinate = coordinate;
			this.position = coordinate * size;

			bounds = new Bounds(position, Vector2.one * size);

			renderedMesh = new ChunkMesh(CreateRenderedMesh);

			// bug with spacing or size with mesh
			if (position.y != 0)
			{
				position.y = position.y - coordinate.y;
			}

			Vector3 positionVector3 = new Vector3(position.x, 0, position.y);

			meshObject = new GameObject("Terrain Chunk");
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshRenderer.material = material;
			meshCollider = meshObject.AddComponent<MeshCollider>();
			meshFilter = meshObject.AddComponent<MeshFilter>();

			meshObject.transform.position = positionVector3 * mapGenerator.terrainData.uniformScale;
			meshObject.transform.parent = parent;

			//meshObject.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformScale;
			meshObject.gameObject.layer = LayerMask.NameToLayer("Ground");

			SetChunkVisible(false);

			mapGenerator.RequestMapData(position, OnMapDataRecieved);
		}

		void OnMapDataRecieved(MapData mapData)
		{
			this.mapData = mapData;
			hasMapDataRecieved = true;

			UpdateChunkVisibility();
		}


		public void UpdateChunkVisibility()
		{
			if (!hasMapDataRecieved)
			{
				return;
			}

			float playerDistanceFromEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

			if (playerDistanceFromEdge > mapGenerator.deleteChunkDistance)
			{
				terrainDictionary.Remove(coordinate);
				Destroy(meshObject);
				return;
			}

			bool isVisible = playerDistanceFromEdge <= maxViewDistance;

			if (!isVisible)
			{
				SetChunkVisible(isVisible);
				return;
			}

			if (!renderedMesh.hasMesh)
			{
				CreateRenderedMesh();
			}

			terrainChunksVisibleLastUpdate.Add(this);

			SetChunkVisible(isVisible);
		}

		public void CreateRenderedMesh()
		{
			if (renderedMesh.hasMesh)
			{
				meshFilter.mesh = renderedMesh.mesh;
				meshCollider.sharedMesh = renderedMesh.mesh;

			}
			else if (!renderedMesh.hasRequestedMesh)
			{
				renderedMesh.RequestMesh(mapData, 0);
			}
		}

		public void SetChunkVisible(bool isVisible)
		{
			meshObject.SetActive(isVisible);
		}

		public bool IsVisible()
		{
			return meshObject.activeSelf;
		}
	}

	class ChunkMesh
	{
		public Mesh mesh;
		public bool hasMesh;
		public bool hasRequestedMesh;
		System.Action updateCallback;

		public ChunkMesh(System.Action updateCallback)
		{
			this.updateCallback = updateCallback;
		}

		public void RequestMesh(MapData mapData, int detail)
		{
			mapGenerator.RequestMeshData(mapData, detail, OnMeshDataRecieved);
			hasRequestedMesh = true;
		}

		public void OnMeshDataRecieved(MeshData meshData)
		{
			mesh = meshData.CreateMesh();
			hasMesh = true;

			updateCallback();
		}
	}
}

public struct PooledPrefab
{
	public readonly GameObject prefab;
	public readonly bool isUsed;
	public readonly float radius;

	public PooledPrefab(GameObject prefab, float radius)
	{
		this.prefab = prefab;
		this.radius = radius;
		this.isUsed = false;
	}
}
