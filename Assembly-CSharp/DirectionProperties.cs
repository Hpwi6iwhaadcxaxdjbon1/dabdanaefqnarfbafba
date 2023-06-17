using System;
using ConVar;
using UnityEngine;

// Token: 0x020001A8 RID: 424
public class DirectionProperties : PrefabAttribute
{
	// Token: 0x04000B83 RID: 2947
	private const float radius = 200f;

	// Token: 0x04000B84 RID: 2948
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	// Token: 0x04000B85 RID: 2949
	public ProtectionProperties extraProtection;

	// Token: 0x06000E5A RID: 3674 RVA: 0x0000D299 File Offset: 0x0000B499
	protected override Type GetIndexedType()
	{
		return typeof(DirectionProperties);
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x00064D30 File Offset: 0x00062F30
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		GizmosUtil.DrawSemiCircle(200f);
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x00064D7C File Offset: 0x00062F7C
	public bool IsWeakspot(Transform tx, HitInfo info)
	{
		if (this.bounds.size == Vector3.zero)
		{
			return false;
		}
		Vector3 vector = tx.worldToLocalMatrix.MultiplyPoint3x4(info.PointStart);
		float num = Vector3Ex.DotDegrees(this.worldForward, vector);
		OBB obb;
		obb..ctor(tx.position + tx.rotation * (this.worldRotation * this.bounds.center + this.worldPosition), this.bounds.size, tx.rotation * this.worldRotation);
		bool flag = num > 100f && obb.Contains(info.HitPositionWorld);
		if (ConVar.Vis.weakspots)
		{
			UnityEngine.DDraw.Line(info.PointStart, info.HitPositionWorld, flag ? Color.red : Color.green, 600f, true, true);
			UnityEngine.DDraw.Text("x", info.HitPositionWorld, Color.white, 600f);
		}
		return flag;
	}
}
