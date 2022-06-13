using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
	[SerializeField] Light directionalLight;
	[SerializeField] LightingPreset lightingPreset;
	[SerializeField] [Range(0, 24)] float timeOfDay;
	[SerializeField] bool updateTime;

	private void OnValidate()
	{
		if (directionalLight != null)
		{
			return;
		}

		if (RenderSettings.sun != null)
		{
			directionalLight = RenderSettings.sun;
		}
		else
		{
			Light[] lights = FindObjectsOfType<Light>();
			foreach (Light light in lights)
			{
				if (light.type == LightType.Directional)
				{
					directionalLight = light;
					return;
				}
			}
		}

		Debug.LogError("No directional lights in scene");

	}

	private void Update()
	{
		if (updateTime)
		{
			if (Application.isPlaying)
			{
				timeOfDay += Time.deltaTime * 0.1f;
				timeOfDay %= 24f;

				UpdateLighting(timeOfDay / 24f);
			}
			else
			{
				UpdateLighting(timeOfDay / 24f);
			}
		}
		else
		{
			UpdateLighting(timeOfDay / 24f);
		}
	}
	void UpdateLighting(float percentOfDay)
	{
		RenderSettings.ambientLight = lightingPreset.ambientColor.Evaluate(percentOfDay);
		RenderSettings.fogColor = lightingPreset.fogColor.Evaluate(percentOfDay);
		RenderSettings.fogEndDistance = lightingPreset.maxLinearFogDistance * (lightingPreset.fogIntensity.Evaluate(timeOfDay / 24f));
		directionalLight.color = lightingPreset.directionalColor.Evaluate(percentOfDay);
		directionalLight.transform.rotation = Quaternion.Euler(new Vector3(percentOfDay * 360f - 90f, 170f, 0));

	}
}
