using System;
using ConVar;
using UnityEngine;

// Token: 0x020001DB RID: 475
public class CameraSettings : MonoBehaviour, IClientComponent
{
	// Token: 0x04000C39 RID: 3129
	private Camera cam;

	// Token: 0x06000F1F RID: 3871 RVA: 0x0000D904 File Offset: 0x0000BB04
	private void OnEnable()
	{
		this.cam = base.GetComponent<Camera>();
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x0000D912 File Offset: 0x0000BB12
	private void Update()
	{
		this.cam.farClipPlane = Mathf.Clamp(ConVar.Graphics.drawdistance, 500f, 2500f);
	}
}
