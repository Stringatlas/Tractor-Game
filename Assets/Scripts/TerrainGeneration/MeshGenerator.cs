using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
	public static MeshData GenerateMesh(float[,] heightMap, float heightMultiplier, AnimationCurve aHeightCurve, int detail, Gradient gradient, float scale)
	{
		// make new instance so that each thread has its own heightcurve
		AnimationCurve heightCurve = new AnimationCurve(aHeightCurve.keys);

		// if detail is 0, make the increment 1 or else for loops would not iterate
		int simplificationIncrement = (detail == 0) ? 1 : detail * 2;

		int borderSize = heightMap.GetLength(0);
		int meshSize = borderSize - 2 * simplificationIncrement;
		int meshSizeUnsimplified = borderSize - 2;

		float topleftX = (meshSizeUnsimplified - 1) / -2f;
		float topleftZ = (meshSizeUnsimplified - 1) / 2f;

		int verticesPerLine = (meshSize - 1) / simplificationIncrement + 1;
		MeshData meshData = new MeshData(verticesPerLine);

		int[,] vertexIndicesMap = new int[borderSize, borderSize];
		int meshVertexIndex = 0;
		int borderVertexIndex = -1;

		for (int z = 0; z < borderSize; z += simplificationIncrement)
		{
			for (int x = 0; x < borderSize; x += simplificationIncrement)
			{
				bool isBorderVertex = (z == 0) || (z == borderSize - 1) || (x == 0) || (x == borderSize - 1);

				if (isBorderVertex)
				{
					vertexIndicesMap[x, z] = borderVertexIndex;
					borderVertexIndex--;
				}
				else
				{
					vertexIndicesMap[x, z] = meshVertexIndex;
					meshVertexIndex++;
				}
			}
		}

		for (int z = 0; z < borderSize; z += simplificationIncrement)
		{
			for (int x = 0; x < borderSize; x += simplificationIncrement)
			{
				int vertexIndex = vertexIndicesMap[x, z];

				// x and z subtracted by simplificationIncrement to make sure uvs are center properly
				Vector3 uvPercent = new Vector2((x - simplificationIncrement) / (float)meshSize, (z - simplificationIncrement) / (float)meshSize);

				float height = heightCurve.Evaluate(heightMap[x, z]) * heightMultiplier;
				
				Vector3 vertexPosition = new Vector3(topleftX + uvPercent.x * meshSizeUnsimplified, height, topleftZ - uvPercent.y * meshSizeUnsimplified) * scale;

				meshData.AddVertex(vertexPosition, uvPercent, gradient.Evaluate(heightMap[x, z]), vertexIndex);

				if (x < (borderSize - 1) && z < (meshSize - 1))
				{
					int pointA = vertexIndicesMap[x, z];
					int pointB = vertexIndicesMap[x + simplificationIncrement, z];
					int pointC = vertexIndicesMap[x, z + simplificationIncrement];
					int pointD = vertexIndicesMap[x + simplificationIncrement, z + simplificationIncrement];

					// add triangles clockwise
					meshData.AddTriangle(pointC, pointA, pointD);
					meshData.AddTriangle(pointD, pointA, pointB);
				}
			}
		}

		meshData.BakeNormals();
		return meshData;
	}
}
