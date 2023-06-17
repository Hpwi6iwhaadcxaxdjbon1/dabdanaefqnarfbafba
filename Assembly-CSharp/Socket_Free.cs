using System;
using UnityEngine;

// Token: 0x020001B4 RID: 436
public class Socket_Free : Socket_Base
{
	// Token: 0x04000BAB RID: 2987
	public Vector3 idealPlacementNormal = Vector3.up;

	// Token: 0x04000BAC RID: 2988
	public bool useTargetNormal = true;

	// Token: 0x06000E89 RID: 3721 RVA: 0x000656D4 File Offset: 0x000638D4
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 1f);
		GizmosUtil.DrawWireCircleZ(Vector3.forward * 0f, 0.2f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x0000D397 File Offset: 0x0000B597
	public override bool TestTarget(Construction.Target target)
	{
		return target.onTerrain;
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x00065744 File Offset: 0x00063944
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion rotation = Quaternion.identity;
		if (this.useTargetNormal)
		{
			Vector3 vector = (target.position - target.ray.origin).normalized;
			float t = Mathf.Abs(Vector3.Dot(vector, target.normal));
			vector = Vector3.Lerp(vector, this.idealPlacementNormal, t);
			rotation = Quaternion.LookRotation(target.normal, vector) * Quaternion.Inverse(this.rotation) * Quaternion.Euler(target.rotation);
		}
		else
		{
			Vector3 normalized = (target.position - target.ray.origin).normalized;
			normalized.y = 0f;
			rotation = Quaternion.LookRotation(normalized, this.idealPlacementNormal) * Quaternion.Euler(target.rotation);
		}
		Vector3 vector2 = target.position;
		vector2 -= rotation * this.position;
		return new Construction.Placement
		{
			rotation = rotation,
			position = vector2
		};
	}
}
