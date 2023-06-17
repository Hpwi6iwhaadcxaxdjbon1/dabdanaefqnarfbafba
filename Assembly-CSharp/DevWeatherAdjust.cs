using System;
using UnityEngine;

// Token: 0x0200025F RID: 607
public class DevWeatherAdjust : MonoBehaviour
{
	// Token: 0x060011D2 RID: 4562 RVA: 0x00075C74 File Offset: 0x00073E74
	protected void Awake()
	{
		SingletonComponent<Climate>.Instance.Overrides.Clouds = 0f;
		SingletonComponent<Climate>.Instance.Overrides.Fog = 0f;
		SingletonComponent<Climate>.Instance.Overrides.Wind = 0f;
		SingletonComponent<Climate>.Instance.Overrides.Rain = 0f;
	}

	// Token: 0x060011D3 RID: 4563 RVA: 0x00075CD4 File Offset: 0x00073ED4
	protected void OnGUI()
	{
		float num = (float)Screen.width * 0.2f;
		GUILayout.BeginArea(new Rect((float)Screen.width - num - 20f, 20f, num, 400f), "", "box");
		GUILayout.Box("Weather", Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		GUILayout.Label("Clouds", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Clouds = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Clouds, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.Label("Fog", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Fog = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Fog, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.Label("Wind", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Wind = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Wind, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.Label("Rain", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Rain = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Rain, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}
}
