using System;
using UnityEngine;

// Token: 0x020004BA RID: 1210
public static class TerrainAnchorEx
{
	// Token: 0x06001BEF RID: 7151 RVA: 0x00016DD6 File Offset: 0x00014FD6
	public static bool ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors, ref Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		return transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, TerrainAnchorMode.MinimizeError, filter);
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x0009A298 File Offset: 0x00098498
	public static bool ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors, ref Vector3 pos, Quaternion rot, Vector3 scale, TerrainAnchorMode mode, SpawnFilter filter = null)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		float num = 0f;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		foreach (TerrainAnchor terrainAnchor in anchors)
		{
			Vector3 vector = Vector3.Scale(terrainAnchor.worldPosition, scale);
			vector = rot * vector;
			Vector3 vector2 = pos + vector;
			if (TerrainMeta.OutOfBounds(vector2))
			{
				return false;
			}
			if (filter != null && filter.GetFactor(vector2) == 0f)
			{
				return false;
			}
			float num4;
			float num5;
			float num6;
			terrainAnchor.Apply(out num4, out num5, out num6, vector2);
			num += num4 - vector.y;
			num2 = Mathf.Max(num2, num5 - vector.y);
			num3 = Mathf.Min(num3, num6 - vector.y);
			if (num3 < num2)
			{
				return false;
			}
		}
		if (mode == TerrainAnchorMode.MinimizeError)
		{
			pos.y = Mathf.Clamp(num / (float)anchors.Length, num2, num3);
		}
		else
		{
			pos.y = Mathf.Clamp(pos.y, num2, num3);
		}
		return true;
	}

	// Token: 0x06001BF1 RID: 7153 RVA: 0x0009A394 File Offset: 0x00098594
	public static void ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors)
	{
		Vector3 position = transform.position;
		transform.ApplyTerrainAnchors(anchors, ref position, transform.rotation, transform.lossyScale, null);
		transform.position = position;
	}
}
