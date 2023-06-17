using System;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class SocketHandle : PrefabAttribute
{
	// Token: 0x06000E99 RID: 3737 RVA: 0x0000D3C8 File Offset: 0x0000B5C8
	protected override Type GetIndexedType()
	{
		return typeof(SocketHandle);
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x00065C74 File Offset: 0x00063E74
	internal void AdjustTarget(ref Construction.Target target, float maxplaceDistance)
	{
		Vector3 worldPosition = this.worldPosition;
		Vector3 a = target.ray.origin + target.ray.direction * maxplaceDistance - worldPosition;
		target.ray.direction = (a - target.ray.origin).normalized;
	}
}
