using System;
using UnityEngine;

// Token: 0x020001BA RID: 442
public class SocketMod_AngleCheck : SocketMod
{
	// Token: 0x04000BB6 RID: 2998
	public bool wantsAngle = true;

	// Token: 0x04000BB7 RID: 2999
	public Vector3 worldNormal = Vector3.up;

	// Token: 0x04000BB8 RID: 3000
	public float withinDegrees = 45f;

	// Token: 0x06000EA0 RID: 3744 RVA: 0x0000D3E0 File Offset: 0x0000B5E0
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawFrustum(Vector3.zero, this.withinDegrees, 1f, 0f, 1f);
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x00065CD4 File Offset: 0x00063ED4
	public override bool DoCheck(Construction.Placement place)
	{
		if (Vector3Ex.DotDegrees(this.worldNormal, place.rotation * Vector3.up) < this.withinDegrees)
		{
			return true;
		}
		Construction.lastPlacementError = "Failed Check: AngleCheck (" + this.hierachyName + ")";
		Input.GetKey(KeyCode.KeypadDivide);
		return false;
	}
}
