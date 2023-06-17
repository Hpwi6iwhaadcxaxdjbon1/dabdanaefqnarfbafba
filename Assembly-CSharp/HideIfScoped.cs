using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class HideIfScoped : MonoBehaviour
{
	// Token: 0x04000838 RID: 2104
	public Renderer[] renderers;

	// Token: 0x06000BB5 RID: 2997 RVA: 0x0005A364 File Offset: 0x00058564
	public void SetVisible(bool vis)
	{
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = vis;
		}
	}
}
