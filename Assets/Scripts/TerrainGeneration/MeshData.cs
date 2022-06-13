using UnityEngine;

public class MeshData
{
	public Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;

	public Color[] colors;

	Vector3[] borderVertices;
	int[] borderTriangles;

	int triangleIndex;
	int borderTriangleIndex;

	Vector3[] bakedNormals;

	public MeshData(int verticesPerLine)
	{
		vertices = new Vector3[verticesPerLine * verticesPerLine];
		uvs = new Vector2[vertices.Length];

		borderVertices = new Vector3[verticesPerLine * 4 + 4];
		borderTriangles = new int[24 * verticesPerLine];

		colors = new Color[vertices.Length];

		triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
	}

	public void AddVertex(Vector3 vertexPosition, Vector2 vertexUv, Color color, int vertexIndex)
	{
		if (vertexIndex < 0)
		{
			borderVertices[-vertexIndex - 1] = vertexPosition;
		}
		else
		{
			vertices[vertexIndex] = vertexPosition;
			colors[vertexIndex] = color;
			uvs[vertexIndex] = vertexUv;
		}
	}

	public void AddTriangle(int vertA, int vertB, int vertC)
	{
		bool isBorderVertex = vertA < 0 || vertB < 0 || vertC < 0;
		if (isBorderVertex)
		{
			borderTriangles[borderTriangleIndex] = vertA;
			borderTriangles[borderTriangleIndex + 1] = vertA;
			borderTriangles[borderTriangleIndex + 2] = vertA;
			borderTriangleIndex += 3;
		}
		else
		{
			triangles[triangleIndex] = vertA;
			triangles[triangleIndex + 1] = vertB;
			triangles[triangleIndex + 2] = vertC;

			triangleIndex += 3;
		}
	}

	Vector3[] CalculateNormals()
	{
		Vector3[] vertexNormals = new Vector3[vertices.Length];
		int triangleCount = triangles.Length / 3;

		for (int i = 0; i < triangleCount; i++)
		{
			int normalTriangleIndex = i * 3;
			int vertexIndexA = triangles[normalTriangleIndex];
			int vertexIndexB = triangles[normalTriangleIndex + 1];
			int vertexIndexC = triangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalsFromIndex(vertexIndexA, vertexIndexB, vertexIndexC);
			vertexNormals[vertexIndexA] += triangleNormal;
			vertexNormals[vertexIndexB] += triangleNormal;
			vertexNormals[vertexIndexC] += triangleNormal;
		}

		int borderTriangleCount = borderTriangles.Length / 3;

		for (int i = 0; i < borderTriangleCount; i++)
		{
			int normalTriangleIndex = i * 3;
			int vertexIndexA = borderTriangles[normalTriangleIndex];
			int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
			int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalsFromIndex(vertexIndexA, vertexIndexB, vertexIndexC);

			if (vertexIndexA >= 0)
			{
				vertexNormals[vertexIndexA] += triangleNormal;
			}
			if (vertexIndexB >= 0)
			{

				vertexNormals[vertexIndexB] += triangleNormal;
			}
			if (vertexIndexC >= 0)
			{
				vertexNormals[vertexIndexC] += triangleNormal;
			}
		}

		for (int i = 0; i < vertexNormals.Length; i++)
		{
			vertexNormals[i].Normalize();
		}

		return vertexNormals;
	}

	Vector3 SurfaceNormalsFromIndex(int indexA, int indexB, int indexC)
	{
		// use border vertices if index less than 0, else use mesh vertices
		Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
		Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
		Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

		Vector3 sideAB = pointB - pointA;
		Vector3 sideAC = pointC - pointA;

		return Vector3.Cross(sideAB, sideAC).normalized;
	}

	public void BakeNormals()
	{
		bakedNormals = CalculateNormals();
	}

	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.colors = colors;
		mesh.normals = bakedNormals;

		return mesh;
	}
}
