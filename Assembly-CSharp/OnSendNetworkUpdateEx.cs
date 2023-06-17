using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020003A8 RID: 936
public static class OnSendNetworkUpdateEx
{
	// Token: 0x06001793 RID: 6035 RVA: 0x0008B048 File Offset: 0x00089248
	public static void BroadcastOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnSendNetworkUpdate> list = Pool.GetList<IOnSendNetworkUpdate>();
		go.GetComponentsInChildren<IOnSendNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnSendNetworkUpdate(entity);
		}
		Pool.FreeList<IOnSendNetworkUpdate>(ref list);
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x0008B088 File Offset: 0x00089288
	public static void SendOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnSendNetworkUpdate> list = Pool.GetList<IOnSendNetworkUpdate>();
		go.GetComponents<IOnSendNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnSendNetworkUpdate(entity);
		}
		Pool.FreeList<IOnSendNetworkUpdate>(ref list);
	}
}
