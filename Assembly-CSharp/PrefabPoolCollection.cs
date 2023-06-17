using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000769 RID: 1897
public class PrefabPoolCollection
{
	// Token: 0x0400246A RID: 9322
	public Dictionary<uint, PrefabPool> storage = new Dictionary<uint, PrefabPool>();

	// Token: 0x06002952 RID: 10578 RVA: 0x000D2E38 File Offset: 0x000D1038
	public void Push(GameObject instance)
	{
		Poolable component = instance.GetComponent<Poolable>();
		PrefabPool prefabPool;
		if (!this.storage.TryGetValue(component.prefabID, ref prefabPool))
		{
			prefabPool = new PrefabPool();
			this.storage.Add(component.prefabID, prefabPool);
		}
		prefabPool.Push(component);
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x000D2E80 File Offset: 0x000D1080
	public GameObject Pop(uint id, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
	{
		PrefabPool prefabPool;
		if (this.storage.TryGetValue(id, ref prefabPool))
		{
			return prefabPool.Pop(pos, rot);
		}
		return null;
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x000D2EA8 File Offset: 0x000D10A8
	public void Clear()
	{
		foreach (KeyValuePair<uint, PrefabPool> keyValuePair in this.storage)
		{
			keyValuePair.Value.Clear();
		}
		this.storage.Clear();
	}
}
