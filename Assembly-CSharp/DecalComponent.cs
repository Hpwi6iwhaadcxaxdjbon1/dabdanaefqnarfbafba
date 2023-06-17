using System;
using UnityEngine;

// Token: 0x020001F8 RID: 504
public abstract class DecalComponent : PrefabAttribute
{
	// Token: 0x06000FB1 RID: 4017
	public abstract void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale);

	// Token: 0x06000FB2 RID: 4018 RVA: 0x0000DEED File Offset: 0x0000C0ED
	protected override Type GetIndexedType()
	{
		return typeof(DecalComponent);
	}
}
