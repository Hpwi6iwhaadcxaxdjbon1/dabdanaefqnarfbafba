using System;
using UnityEngine;

// Token: 0x0200054E RID: 1358
public static class TerrainPlacementEx
{
	// Token: 0x06001E6D RID: 7789 RVA: 0x000A79D4 File Offset: 0x000A5BD4
	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (placements.Length == 0)
		{
			return;
		}
		Matrix4x4 localToWorld = Matrix4x4.TRS(pos, rot, scale);
		Matrix4x4 inverse = localToWorld.inverse;
		for (int i = 0; i < placements.Length; i++)
		{
			placements[i].Apply(localToWorld, inverse);
		}
	}

	// Token: 0x06001E6E RID: 7790 RVA: 0x00018100 File Offset: 0x00016300
	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements)
	{
		transform.ApplyTerrainPlacements(placements, transform.position, transform.rotation, transform.lossyScale);
	}
}
