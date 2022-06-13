using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public enum DrawMode
{
	NoiseMap,
	FalloffMap,
	Mesh
}

public class MapGenerator : MonoBehaviour
{
	public TerrainData terrainData;
	public NoiseData noiseData;

	public int mapChunkSize = 61;
	public uint deleteChunkDistance = 10000;

	public DrawMode drawMode;
	public bool autoUpdate;

	public float minValueRegion = 0.5f;
	Queue<ThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<ThreadInfo<MapData>>();
	Queue<ThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ThreadInfo<MeshData>>();

	float[,] falloffMap;


	public void RequestMapData(Vector2 mapCenter, Action<MapData> callback)
	{
		ThreadStart threadStart = delegate
		{
			MapDataThread(mapCenter, callback);
		};

		new Thread(threadStart).Start();
	}

	void MapDataThread(Vector2 mapCenter, Action<MapData> callback)
	{
		MapData mapData = GenerateMapData(mapCenter);
		lock (mapDataThreadInfoQueue)
		{
			mapDataThreadInfoQueue.Enqueue(new ThreadInfo<MapData>(callback, mapData));
		}
	}

	public void RequestMeshData(MapData mapData, int detail, Action<MeshData> callback)
	{
		ThreadStart threadStart = delegate
		{
			MeshDataThread(mapData, detail, callback);
		};

		new Thread(threadStart).Start();
	}

	void MeshDataThread(MapData mapData, int detail, Action<MeshData> callback)
	{
		MeshData meshData = MeshGenerator.GenerateMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, detail, terrainData.terrainGradient, terrainData.uniformScale);
		lock (meshDataThreadInfoQueue)
		{
			meshDataThreadInfoQueue.Enqueue(new ThreadInfo<MeshData>(callback, meshData));
		}
	}

	void Update()
	{
		if (mapDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
			{
				lock (mapDataThreadInfoQueue)
				{
					ThreadInfo<MapData> mapThreadInfo = mapDataThreadInfoQueue.Dequeue();
					mapThreadInfo.callback(mapThreadInfo.parameter);
				}
			}
		}

		if (meshDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
			{
				lock (meshDataThreadInfoQueue)
				{
					ThreadInfo<MeshData> meshThreadInfo = meshDataThreadInfoQueue.Dequeue();
					meshThreadInfo.callback(meshThreadInfo.parameter);
				}
			}
		}
	}

	private MapData GenerateMapData(Vector2 mapCenter)
	{
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistence, noiseData.lacunarity, mapCenter + noiseData.offset, noiseData.normalizeMode);

		if (terrainData.useFalloffMap)
		{
			if (falloffMap == null)
			{
				falloffMap = FalloffMapGenerator.GenerateFalloffMap(mapChunkSize, terrainData.fallOffA, terrainData.fallOffB);
			}

			for (int z = 0; z < mapChunkSize; z++)
			{
				for (int x = 0; x < mapChunkSize; x++)
				{
					noiseMap[x, z] = Mathf.Clamp01(noiseMap[x, z] - falloffMap[x, z]);
				}
			}
		}


		return new MapData(noiseMap);
	}

	struct ThreadInfo<T>
	{
		public readonly Action<T> callback;
		public readonly T parameter;

		public ThreadInfo(Action<T> callback, T parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}
	}
}

public struct MapData
{
	public readonly float[,] heightMap;

	public MapData(float[,] heightMap)
	{
		this.heightMap = heightMap;
	}
}