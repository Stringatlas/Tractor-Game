using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Noise Data", menuName = "Noise Data")]
public class NoiseData : UpdateableData
{ 
	public float noiseScale;
	public int octaves;
	[Range(0, 1)]
	public float persistence;
	public float lacunarity;
	public int seed;
	public Vector2 offset;

	public Noise.NormalizeMode normalizeMode;

	#if UNITY_EDITOR
	protected override void OnValidate()
	{
		if (octaves < 0)
		{
			octaves = 0;
		}

		base.OnValidate();
	}

	#endif
}
