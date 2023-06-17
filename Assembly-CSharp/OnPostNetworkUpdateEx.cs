using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020003A6 RID: 934
public static class OnPostNetworkUpdateEx
{
	// Token: 0x06001790 RID: 6032 RVA: 0x0008AFC8 File Offset: 0x000891C8
	public static void BroadcastOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnPostNetworkUpdate> list = Pool.GetList<IOnPostNetworkUpdate>();
		go.GetComponentsInChildren<IOnPostNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnPostNetworkUpdate(entity);
		}
		Pool.FreeList<IOnPostNetworkUpdate>(ref list);
	}

	// Token: 0x06001791 RID: 6033 RVA: 0x0008B008 File Offset: 0x00089208
	public static void SendOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnPostNetworkUpdate> list = Pool.GetList<IOnPostNetworkUpdate>();
		go.GetComponents<IOnPostNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnPostNetworkUpdate(entity);
		}
		Pool.FreeList<IOnPostNetworkUpdate>(ref list);
	}
}
