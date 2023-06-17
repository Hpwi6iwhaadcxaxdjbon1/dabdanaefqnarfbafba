using System;
using UnityEngine;

// Token: 0x020001A7 RID: 423
public class DecayPoint : PrefabAttribute
{
	// Token: 0x04000B81 RID: 2945
	[Tooltip("If this point is occupied this will take this % off the power of the decay")]
	public float protection = 0.25f;

	// Token: 0x04000B82 RID: 2946
	public Socket_Base socket;

	// Token: 0x06000E57 RID: 3671 RVA: 0x0000D26C File Offset: 0x0000B46C
	public bool IsOccupied(BaseEntity entity)
	{
		return entity.IsOccupied(this.socket);
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x0000D27A File Offset: 0x0000B47A
	protected override Type GetIndexedType()
	{
		return typeof(DecayPoint);
	}
}
