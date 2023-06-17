using System;
using UnityEngine;

// Token: 0x020004F9 RID: 1273
public static class TerrainFilterEx
{
	// Token: 0x06001D90 RID: 7568 RVA: 0x000A1508 File Offset: 0x0009F708
	public static bool ApplyTerrainFilters(this Transform transform, TerrainFilter[] filters, Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter globalFilter = null)
	{
		if (filters.Length == 0)
		{
			return true;
		}
		foreach (TerrainFilter terrainFilter in filters)
		{
			Vector3 vector = Vector3.Scale(terrainFilter.worldPosition, scale);
			vector = rot * vector;
			Vector3 vector2 = pos + vector;
			if (TerrainMeta.OutOfBounds(vector2))
			{
				return false;
			}
			if (globalFilter != null && globalFilter.GetFactor(vector2) == 0f)
			{
				return false;
			}
			if (!terrainFilter.Check(vector2))
			{
				return false;
			}
		}
		return true;
	}
}
