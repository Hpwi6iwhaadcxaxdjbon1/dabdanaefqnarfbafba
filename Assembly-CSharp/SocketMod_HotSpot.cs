using System;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class SocketMod_HotSpot : SocketMod
{
	// Token: 0x04000BCE RID: 3022
	public float spotSize = 0.1f;

	// Token: 0x06000EB7 RID: 3767 RVA: 0x0000D4FC File Offset: 0x0000B6FC
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
		Gizmos.DrawSphere(Vector3.zero, this.spotSize);
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x000664A0 File Offset: 0x000646A0
	public override void ModifyPlacement(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		place.position = position;
	}
}
