using System;
using ConVar;
using UnityEngine;

// Token: 0x0200022D RID: 557
public class SunSettings : MonoBehaviour, IClientComponent
{
	// Token: 0x04000DCC RID: 3532
	private Light light;

	// Token: 0x060010DE RID: 4318 RVA: 0x0000EC38 File Offset: 0x0000CE38
	private void OnEnable()
	{
		this.light = base.GetComponent<Light>();
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x00071D5C File Offset: 0x0006FF5C
	private void Update()
	{
		LightShadows lightShadows = (LightShadows)Mathf.Clamp(ConVar.Graphics.shadowmode, 1, 2);
		if (this.light.shadows != lightShadows)
		{
			this.light.shadows = lightShadows;
		}
	}
}
