using System;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public class SocketMod_InWater : SocketMod
{
	// Token: 0x04000BCF RID: 3023
	public bool wantsInWater = true;

	// Token: 0x06000EBA RID: 3770 RVA: 0x0000D54F File Offset: 0x0000B74F
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x000664D4 File Offset: 0x000646D4
	public override bool DoCheck(Construction.Placement place)
	{
		if (WaterLevel.Test(place.position + place.rotation * this.worldPosition) == this.wantsInWater)
		{
			return true;
		}
		Construction.lastPlacementError = "Failed Check: InWater (" + this.hierachyName + ")";
		return false;
	}
}
