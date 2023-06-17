using System;
using UnityEngine;

// Token: 0x020004BC RID: 1212
public class TerrainCheck : PrefabAttribute
{
	// Token: 0x040018DD RID: 6365
	public bool Rotate = true;

	// Token: 0x040018DE RID: 6366
	public float Extents = 1f;

	// Token: 0x06001BF3 RID: 7155 RVA: 0x0009A3C8 File Offset: 0x000985C8
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawLine(base.transform.position - Vector3.up * this.Extents, base.transform.position + Vector3.up * this.Extents);
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x0009A438 File Offset: 0x00098638
	public bool Check(Vector3 pos)
	{
		float extents = this.Extents;
		float height = TerrainMeta.HeightMap.GetHeight(pos);
		float num = pos.y - extents;
		float num2 = pos.y + extents;
		return num <= height && num2 >= height;
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x00016E25 File Offset: 0x00015025
	protected override Type GetIndexedType()
	{
		return typeof(TerrainCheck);
	}
}
