using System;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class SocketMod_AreaCheck : SocketMod
{
	// Token: 0x04000BB9 RID: 3001
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 0.1f);

	// Token: 0x04000BBA RID: 3002
	public LayerMask layerMask;

	// Token: 0x04000BBB RID: 3003
	public bool wantsInside = true;

	// Token: 0x06000EA3 RID: 3747 RVA: 0x00065D2C File Offset: 0x00063F2C
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		bool flag = true;
		if (!this.wantsInside)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green : Color.red);
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x0000D440 File Offset: 0x0000B640
	public static bool IsInArea(Vector3 position, Quaternion rotation, Bounds bounds, LayerMask layerMask)
	{
		return GamePhysics.CheckOBB(new OBB(position, rotation, bounds), layerMask.value, 0);
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x00065D88 File Offset: 0x00063F88
	public bool DoCheck(Vector3 position, Quaternion rotation)
	{
		Vector3 position2 = position + rotation * this.worldPosition;
		Quaternion rotation2 = rotation * this.worldRotation;
		return SocketMod_AreaCheck.IsInArea(position2, rotation2, this.bounds, this.layerMask) == this.wantsInside;
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x00065DD0 File Offset: 0x00063FD0
	public override bool DoCheck(Construction.Placement place)
	{
		if (this.DoCheck(place.position, place.rotation))
		{
			return true;
		}
		Construction.lastPlacementError = "Failed Check: IsInArea (" + this.hierachyName + ")";
		if (Input.GetKey(KeyCode.KeypadDivide))
		{
			Vector3 vPos = place.position + place.rotation * this.worldPosition;
			DDraw.Sphere(vPos, 0.05f, Color.red, 0.1f, true);
			DDraw.Text(Construction.lastPlacementError, vPos, Color.white, 0.1f);
		}
		return false;
	}
}
