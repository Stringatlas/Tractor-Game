using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffMapGenerator
{
	public static float[,] GenerateFalloffMap(int size, float a, float b)
	{
		float[,] falloffMap = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = (i / (float)size) * 2 - 1;
				float y = (j / (float)size) * 2 - 1;

				float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				falloffMap[i, j] = Evaluate(value, a, b);
			}
		}

		return falloffMap;
	}

	static float Evaluate(float value, float a, float b)
	{
		return Mathf.Pow(value, a) / (Mathf.Pow(value, b) + Mathf.Pow(b * (1 - value), a));
	}
}
