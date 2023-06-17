using System;
using UnityEngine;

// Token: 0x020004F8 RID: 1272
public class TerrainFilter : PrefabAttribute
{
	// Token: 0x040019EA RID: 6634
	public SpawnFilter Filter;

	// Token: 0x06001D8C RID: 7564 RVA: 0x000A1470 File Offset: 0x0009F670
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawCube(base.transform.position + Vector3.up * 50f * 0.5f, new Vector3(0.5f, 50f, 0.5f));
		Gizmos.DrawSphere(base.transform.position + Vector3.up * 50f, 2f);
	}

	// Token: 0x06001D8D RID: 7565 RVA: 0x00017B7E File Offset: 0x00015D7E
	public bool Check(Vector3 pos)
	{
		return this.Filter.GetFactor(pos) > 0f;
	}

	// Token: 0x06001D8E RID: 7566 RVA: 0x00017B93 File Offset: 0x00015D93
	protected override Type GetIndexedType()
	{
		return typeof(TerrainFilter);
	}
}
