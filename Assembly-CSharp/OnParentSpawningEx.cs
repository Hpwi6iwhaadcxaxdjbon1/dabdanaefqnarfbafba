using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020003A4 RID: 932
public static class OnParentSpawningEx
{
	// Token: 0x0600178D RID: 6029 RVA: 0x0008AF48 File Offset: 0x00089148
	public static void BroadcastOnParentSpawning(this GameObject go)
	{
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponentsInChildren<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
	}

	// Token: 0x0600178E RID: 6030 RVA: 0x0008AF88 File Offset: 0x00089188
	public static void SendOnParentSpawning(this GameObject go)
	{
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponents<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
	}
}
