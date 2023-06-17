using System;
using UnityEngine;

// Token: 0x020004A2 RID: 1186
public abstract class DecorComponent : PrefabAttribute
{
	// Token: 0x06001BA6 RID: 7078
	public abstract void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale);

	// Token: 0x06001BA7 RID: 7079 RVA: 0x00016A94 File Offset: 0x00014C94
	protected override Type GetIndexedType()
	{
		return typeof(DecorComponent);
	}
}
