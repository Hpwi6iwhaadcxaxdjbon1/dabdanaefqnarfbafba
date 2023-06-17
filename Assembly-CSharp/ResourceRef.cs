using System;
using UnityEngine;

// Token: 0x0200076E RID: 1902
[Serializable]
public class ResourceRef<T> where T : Object
{
	// Token: 0x04002477 RID: 9335
	public string guid;

	// Token: 0x04002478 RID: 9336
	private Object _cachedObject;

	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x0600296A RID: 10602 RVA: 0x00020384 File Offset: 0x0001E584
	public bool isValid
	{
		get
		{
			return !string.IsNullOrEmpty(this.guid);
		}
	}

	// Token: 0x0600296B RID: 10603 RVA: 0x00020394 File Offset: 0x0001E594
	public T Get()
	{
		if (this._cachedObject == null)
		{
			this._cachedObject = GameManifest.GUIDToObject(this.guid);
		}
		return this._cachedObject as T;
	}

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x0600296C RID: 10604 RVA: 0x000203C5 File Offset: 0x0001E5C5
	public string resourcePath
	{
		get
		{
			return GameManifest.GUIDToPath(this.guid);
		}
	}

	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x0600296D RID: 10605 RVA: 0x000203D2 File Offset: 0x0001E5D2
	public uint resourceID
	{
		get
		{
			return StringPool.Get(this.resourcePath);
		}
	}
}
