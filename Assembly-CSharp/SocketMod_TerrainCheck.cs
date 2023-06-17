using System;
using UnityEngine;

// Token: 0x020001C5 RID: 453
public class SocketMod_TerrainCheck : SocketMod
{
	// Token: 0x04000BD7 RID: 3031
	public bool wantsInTerrain = true;

	// Token: 0x06000EC3 RID: 3779 RVA: 0x00066838 File Offset: 0x00064A38
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		bool flag = SocketMod_TerrainCheck.IsInTerrain(base.transform.position);
		if (!this.wantsInTerrain)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green : Color.red);
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x00066898 File Offset: 0x00064A98
	public static bool IsInTerrain(Vector3 vPoint)
	{
		if (TerrainMeta.OutOfBounds(vPoint))
		{
			return false;
		}
		if (!TerrainMeta.Collision || !TerrainMeta.Collision.GetIgnore(vPoint, 0.01f))
		{
			foreach (Terrain terrain in Terrain.activeTerrains)
			{
				if (terrain.SampleHeight(vPoint) + terrain.transform.position.y > vPoint.y)
				{
					return true;
				}
			}
		}
		return Physics.Raycast(new Ray(vPoint + Vector3.up * 3f, Vector3.down), 3f, 65536);
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x0006693C File Offset: 0x00064B3C
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		if (SocketMod_TerrainCheck.IsInTerrain(vector) == this.wantsInTerrain)
		{
			return true;
		}
		Construction.lastPlacementError = this.fullName + ": not in terrain";
		if (Input.GetKeyDown(KeyCode.KeypadDivide))
		{
			DDraw.Sphere(vector, 0.1f, Color.red, 20f, true);
			DDraw.Text(Construction.lastPlacementError, vector, Color.white, 20f);
		}
		return false;
	}
}
