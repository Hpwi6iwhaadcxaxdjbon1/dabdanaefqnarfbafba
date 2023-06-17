using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020003A2 RID: 930
public static class OnParentDestroyingEx
{
	// Token: 0x0600178A RID: 6026 RVA: 0x0008AEC8 File Offset: 0x000890C8
	public static void BroadcastOnParentDestroying(this GameObject go)
	{
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponentsInChildren<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x0008AF08 File Offset: 0x00089108
	public static void SendOnParentDestroying(this GameObject go)
	{
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponents<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
	}
}
