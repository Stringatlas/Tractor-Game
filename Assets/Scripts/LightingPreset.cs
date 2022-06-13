using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName ="LightingPreset", menuName="Scriptable Objects/Lighting Preset")]
public class LightingPreset : ScriptableObject
{
	public float maxLinearFogDistance;

	public Gradient ambientColor;
	public Gradient fogColor;
	public AnimationCurve fogIntensity;
	public Gradient directionalColor;
}
