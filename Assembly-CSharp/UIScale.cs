using System;
using ConVar;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006E3 RID: 1763
public class UIScale : MonoBehaviour
{
	// Token: 0x040022FD RID: 8957
	public CanvasScaler scaler;

	// Token: 0x06002702 RID: 9986 RVA: 0x000CBA5C File Offset: 0x000C9C5C
	private void Update()
	{
		Vector2 vector = new Vector2(1280f / ConVar.Graphics.uiscale, 720f / ConVar.Graphics.uiscale);
		if (this.scaler.referenceResolution != vector)
		{
			this.scaler.referenceResolution = vector;
		}
	}
}
