using System;
using UnityEngine;

// Token: 0x020004AA RID: 1194
public class DecorSwim : DecorComponent
{
	// Token: 0x06001BB6 RID: 7094 RVA: 0x00016B97 File Offset: 0x00014D97
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		pos.y = TerrainMeta.WaterMap.GetHeight(pos);
	}
}
