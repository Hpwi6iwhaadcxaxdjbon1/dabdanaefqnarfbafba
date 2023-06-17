using System;
using UnityEngine;

// Token: 0x0200053D RID: 1341
public abstract class TerrainModifier : PrefabAttribute
{
	// Token: 0x04001A8E RID: 6798
	public float Opacity = 1f;

	// Token: 0x04001A8F RID: 6799
	public float Radius;

	// Token: 0x04001A90 RID: 6800
	public float Fade;

	// Token: 0x06001E2E RID: 7726 RVA: 0x000A5AD0 File Offset: 0x000A3CD0
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, this.Opacity);
		GizmosUtil.DrawWireCircleY(base.transform.position, base.transform.lossyScale.y * this.Radius);
	}

	// Token: 0x06001E2F RID: 7727 RVA: 0x000A5B24 File Offset: 0x000A3D24
	public void Apply(Vector3 pos, float scale)
	{
		float opacity = this.Opacity;
		float radius = scale * this.Radius;
		float fade = scale * this.Fade;
		this.Apply(pos, opacity, radius, fade);
	}

	// Token: 0x06001E30 RID: 7728
	protected abstract void Apply(Vector3 position, float opacity, float radius, float fade);

	// Token: 0x06001E31 RID: 7729 RVA: 0x00017F20 File Offset: 0x00016120
	protected override Type GetIndexedType()
	{
		return typeof(TerrainModifier);
	}
}
