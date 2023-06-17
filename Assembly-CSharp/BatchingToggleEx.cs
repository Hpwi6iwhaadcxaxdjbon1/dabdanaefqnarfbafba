using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020003B2 RID: 946
public static class BatchingToggleEx
{
	// Token: 0x060017A9 RID: 6057 RVA: 0x0008B1FC File Offset: 0x000893FC
	public static void BroadcastBatchingToggle(this GameObject go, bool state)
	{
		List<RendererBatch> list = Pool.GetList<RendererBatch>();
		go.GetComponentsInChildren<RendererBatch>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Toggle(state);
		}
		Pool.FreeList<RendererBatch>(ref list);
	}

	// Token: 0x060017AA RID: 6058 RVA: 0x0008B23C File Offset: 0x0008943C
	public static void SendBatchingToggle(this GameObject go, bool state)
	{
		List<RendererBatch> list = Pool.GetList<RendererBatch>();
		go.GetComponents<RendererBatch>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Toggle(state);
		}
		Pool.FreeList<RendererBatch>(ref list);
	}
}
