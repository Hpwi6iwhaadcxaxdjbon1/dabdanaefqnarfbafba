using System;
using UnityEngine;

// Token: 0x0200073B RID: 1851
public static class GameObjectUtil
{
	// Token: 0x0600284C RID: 10316 RVA: 0x000CF698 File Offset: 0x000CD898
	public static void GlobalBroadcast(string messageName, object param = null)
	{
		Transform[] rootObjects = TransformUtil.GetRootObjects();
		for (int i = 0; i < rootObjects.Length; i++)
		{
			rootObjects[i].BroadcastMessage(messageName, param, SendMessageOptions.DontRequireReceiver);
		}
	}
}
