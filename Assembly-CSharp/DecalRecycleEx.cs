using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020001FB RID: 507
public static class DecalRecycleEx
{
	// Token: 0x06000FBB RID: 4027 RVA: 0x0006B5BC File Offset: 0x000697BC
	public static void BroadcastDecalRecycle(this GameObject go)
	{
		List<DecalRecycle> list = Pool.GetList<DecalRecycle>();
		go.GetComponentsInChildren<DecalRecycle>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Recycle();
		}
		Pool.FreeList<DecalRecycle>(ref list);
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x0006B5FC File Offset: 0x000697FC
	public static void SendDecalRecycle(this GameObject go)
	{
		List<DecalRecycle> list = Pool.GetList<DecalRecycle>();
		go.GetComponents<DecalRecycle>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Recycle();
		}
		Pool.FreeList<DecalRecycle>(ref list);
	}
}
