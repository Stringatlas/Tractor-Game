using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain Data", menuName = "Terrain Data")]
public class TerrainData : UpdateableData
{
	public float uniformScale = 0.5f;

	public float meshHeightMultiplier; 
	public AnimationCurve meshHeightCurve;

	public bool useFalloffMap;
	public float fallOffA;
	public float fallOffB;

	public Gradient terrainGradient;
}
