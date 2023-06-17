using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000768 RID: 1896
public class PrefabPool
{
	// Token: 0x04002469 RID: 9321
	public Stack<Poolable> stack = new Stack<Poolable>();

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x0600294C RID: 10572 RVA: 0x000202C2 File Offset: 0x0001E4C2
	public int Count
	{
		get
		{
			return this.stack.Count;
		}
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x000202CF File Offset: 0x0001E4CF
	public void Push(Poolable info)
	{
		this.stack.Push(info);
		info.EnterPool();
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x000D2D5C File Offset: 0x000D0F5C
	public void Push(GameObject instance)
	{
		Poolable component = instance.GetComponent<Poolable>();
		this.Push(component);
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x000D2D78 File Offset: 0x000D0F78
	public GameObject Pop(Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
	{
		while (this.stack.Count > 0)
		{
			Poolable poolable = this.stack.Pop();
			if (poolable)
			{
				poolable.transform.position = pos;
				poolable.transform.rotation = rot;
				poolable.LeavePool();
				return poolable.gameObject;
			}
		}
		return null;
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x000D2DD0 File Offset: 0x000D0FD0
	public void Clear()
	{
		foreach (Poolable poolable in this.stack)
		{
			if (poolable)
			{
				Object.Destroy(poolable);
			}
		}
		this.stack.Clear();
	}
}
