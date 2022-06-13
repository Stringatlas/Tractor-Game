using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscSampling : MonoBehaviour
{
	public static List<Vector2> GeneratePoints(float radius, Vector2 regionSize, int samplesBeforeRejection = 15)
	{
		float cellSize = radius / Mathf.Sqrt(2);
		int[,] grid = new int[Mathf.CeilToInt(regionSize.x / cellSize), Mathf.CeilToInt(regionSize.y / cellSize)];
		List<Vector2> points = new List<Vector2>();
		List<Vector2> spawnPoints = new List<Vector2>();

		spawnPoints.Add(Vector2.one * regionSize / 2);
		while (spawnPoints.Count > 0)
		{
			int spawnIndex = Random.Range(0, spawnPoints.Count);
			Vector2 spawnCenter = spawnPoints[spawnIndex];

			bool candidateAccepted = false;
			for (int i = 0; i < samplesBeforeRejection; i++)
			{
				float angle = Random.value * Mathf.PI * 2;
				Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				Vector2 candidate = spawnCenter + direction * Random.Range(radius, radius * 2);
				if (IsValid(candidate, regionSize, cellSize, radius, points, grid))
				{
					points.Add(candidate);
					spawnPoints.Add(candidate);
					grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
					candidateAccepted = true;
					break;
				}
			}

			if (!candidateAccepted)
			{
				spawnPoints.RemoveAt(spawnIndex);
			}
		}

		return points;
	}
	static bool IsValid(Vector2 candidate, Vector2 regionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
	{
		bool isInRegion = candidate.x >= 0 && candidate.x < regionSize.x && candidate.y >= 0 && candidate.y < regionSize.y;
		if (!isInRegion)
		{
			return false;
		}

		int cellPositionX = (int)(candidate.x / cellSize);
		int cellPositionY = (int)(candidate.y / cellSize);
		int searchStartX = Mathf.Max(0, cellPositionX - 2);
		int searchStartY = Mathf.Max(0, cellPositionY - 2);
		int searchEndX = Mathf.Min(grid.GetLength(0) - 1, cellPositionX + 2);
		int searchEndY = Mathf.Min(grid.GetLength(1) - 1, cellPositionY + 2);

		for (int x = searchStartX; x <= searchEndX; x++)
		{
			for (int y = searchStartY; y <= searchEndY; y++)
			{
				int pointIndex = grid[x, y] - 1;
				if (pointIndex != -1)
				{
					float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
					if (sqrDst < radius * radius)
					{
						return false;
					}
				}
			}
		}
		return true;
	}
}
