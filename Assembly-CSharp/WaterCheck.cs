using System;
using UnityEngine;

// Token: 0x02000572 RID: 1394
public class WaterCheck : PrefabAttribute
{
	// Token: 0x04001BEA RID: 7146
	public bool Rotate = true;

	// Token: 0x06001FE0 RID: 8160 RVA: 0x000193B1 File Offset: 0x000175B1
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 0f, 0.5f, 1f);
		Gizmos.DrawSphere(base.transform.position, 1f);
	}

	// Token: 0x06001FE1 RID: 8161 RVA: 0x000193E6 File Offset: 0x000175E6
	public bool Check(Vector3 pos)
	{
		return pos.y <= TerrainMeta.WaterMap.GetHeight(pos);
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x000193FE File Offset: 0x000175FE
	protected override Type GetIndexedType()
	{
		return typeof(WaterCheck);
	}
}
