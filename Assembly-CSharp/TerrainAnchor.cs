using System;
using UnityEngine;

// Token: 0x020004B8 RID: 1208
public class TerrainAnchor : PrefabAttribute
{
	// Token: 0x040018D0 RID: 6352
	public float Extents = 1f;

	// Token: 0x040018D1 RID: 6353
	public float Offset;

	// Token: 0x06001BEB RID: 7147 RVA: 0x0009A1C0 File Offset: 0x000983C0
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawLine(base.transform.position + Vector3.up * this.Offset - Vector3.up * this.Extents, base.transform.position + Vector3.up * this.Offset + Vector3.up * this.Extents);
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x0009A25C File Offset: 0x0009845C
	public void Apply(out float height, out float min, out float max, Vector3 pos)
	{
		float extents = this.Extents;
		float offset = this.Offset;
		height = TerrainMeta.HeightMap.GetHeight(pos);
		min = height - offset - extents;
		max = height - offset + extents;
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x00016DB7 File Offset: 0x00014FB7
	protected override Type GetIndexedType()
	{
		return typeof(TerrainAnchor);
	}
}
