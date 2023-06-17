using System;
using UnityEngine;

// Token: 0x02000573 RID: 1395
public static class WaterCheckEx
{
	// Token: 0x06001FE4 RID: 8164 RVA: 0x000ADDEC File Offset: 0x000ABFEC
	public static bool ApplyWaterChecks(this Transform transform, WaterCheck[] anchors, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		foreach (WaterCheck waterCheck in anchors)
		{
			Vector3 vector = Vector3.Scale(waterCheck.worldPosition, scale);
			if (waterCheck.Rotate)
			{
				vector = rot * vector;
			}
			Vector3 pos2 = pos + vector;
			if (!waterCheck.Check(pos2))
			{
				return false;
			}
		}
		return true;
	}
}
