using System;
using UnityEngine;

// Token: 0x02000767 RID: 1895
public static class PoolableEx
{
	// Token: 0x0600294A RID: 10570 RVA: 0x000D2D34 File Offset: 0x000D0F34
	public static bool SupportsPooling(this GameObject gameObject)
	{
		Poolable component = gameObject.GetComponent<Poolable>();
		return component != null && component.prefabID > 0U;
	}

	// Token: 0x0600294B RID: 10571 RVA: 0x000202A4 File Offset: 0x0001E4A4
	public static void AwakeFromInstantiate(this GameObject gameObject)
	{
		if (gameObject.activeSelf)
		{
			gameObject.GetComponent<Poolable>().SetBehaviourEnabled(true);
			return;
		}
		gameObject.SetActive(true);
	}
}
