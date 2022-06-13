using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
	public enum NormalizeMode { Local, Global }

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
	{
		if (scale <= 0)
		{
			scale = 0.001f;
		}

		float maximumPossibleHeight = 0;

		System.Random randomNumber = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		float halfMapWidth = mapWidth / 2f;
		float halfMapHeight = mapHeight / 2f;

		float amplitude = 1;
		float frequency = 1;

		for (int i = 0; i < octaves; i++)
		{
			float xOffset = randomNumber.Next(-100_000, 100_000) + offset.x;
			float zOffset = randomNumber.Next(-100_000, 100_000) - offset.y;
			octaveOffsets[i] = new Vector2(xOffset, zOffset);

			maximumPossibleHeight += amplitude;
			amplitude *= persistence;
		}

		float[,] noiseMap = new float[mapWidth, mapHeight];
		for (int z = 0; z < mapHeight; z++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				for (int octaveIndex = 0; octaveIndex < octaves; octaveIndex++)
				{
					float sampleX = (x - halfMapWidth + octaveOffsets[octaveIndex].x) / scale * frequency;
					float sampleZ = (z - halfMapHeight + octaveOffsets[octaveIndex].y) / scale * frequency;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;

					noiseHeight += perlinValue * amplitude;

					amplitude *= persistence;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxLocalNoiseHeight)
				{
					maxLocalNoiseHeight = noiseHeight;
				}
				if (noiseHeight < minLocalNoiseHeight)
				{
					minLocalNoiseHeight = noiseHeight;
				}

				noiseMap[x, z] = noiseHeight;
			}
		}

		for (int z = 0; z < mapHeight; z++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				if (normalizeMode == NormalizeMode.Local)
				{
					noiseMap[x, z] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, z]);
				}
				else
				{
					noiseMap[x, z] = (noiseMap[x, z] + 1) / (maximumPossibleHeight * 1.1f);
				}
			}
		}

		return noiseMap;
	}
}
