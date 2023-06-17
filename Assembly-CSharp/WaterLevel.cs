using System;
using UnityEngine;

// Token: 0x02000411 RID: 1041
public static class WaterLevel
{
	// Token: 0x0600195C RID: 6492 RVA: 0x0008FA74 File Offset: 0x0008DC74
	public static float Factor(Bounds bounds)
	{
		float result;
		using (TimeWarning.New("WaterLevel.Factor", 0.1f))
		{
			if (bounds.size == Vector3.zero)
			{
				bounds.size = new Vector3(0.1f, 0.1f, 0.1f);
			}
			WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(bounds);
			result = (waterInfo.isValid ? Mathf.InverseLerp(bounds.min.y, bounds.max.y, waterInfo.surfaceLevel) : 0f);
		}
		return result;
	}

	// Token: 0x0600195D RID: 6493 RVA: 0x0008FB18 File Offset: 0x0008DD18
	public static bool Test(Vector3 pos)
	{
		bool isValid;
		using (TimeWarning.New("WaterLevel.Test", 0.1f))
		{
			isValid = WaterLevel.GetWaterInfo(pos).isValid;
		}
		return isValid;
	}

	// Token: 0x0600195E RID: 6494 RVA: 0x0008FB60 File Offset: 0x0008DD60
	public static float GetWaterDepth(Vector3 pos)
	{
		float currentDepth;
		using (TimeWarning.New("WaterLevel.GetWaterDepth", 0.1f))
		{
			currentDepth = WaterLevel.GetWaterInfo(pos).currentDepth;
		}
		return currentDepth;
	}

	// Token: 0x0600195F RID: 6495 RVA: 0x0008FBA8 File Offset: 0x0008DDA8
	public static float GetOverallWaterDepth(Vector3 pos)
	{
		float overallDepth;
		using (TimeWarning.New("WaterLevel.GetOverallWaterDepth", 0.1f))
		{
			overallDepth = WaterLevel.GetWaterInfo(pos).overallDepth;
		}
		return overallDepth;
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x0008FBF0 File Offset: 0x0008DDF0
	public static WaterLevel.WaterInfo GetBuoyancyWaterInfo(Vector3 pos, float normX, float normZ)
	{
		WaterLevel.WaterInfo result;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0.1f))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			float num = 0f;
			float num2 = TerrainMeta.WaterMap ? WaterSystem.GetHeight(pos, normX, normZ, out num) : 0f;
			if (pos.y > num2)
			{
				result = waterInfo;
			}
			else
			{
				bool flag = pos.y < num - 1f;
				if (flag)
				{
					num2 = 0f;
					if (pos.y > num2)
					{
						return waterInfo;
					}
				}
				int num3 = TerrainMeta.TopologyMap ? TerrainMeta.TopologyMap.GetTopologyFast(normX, normZ) : 0;
				if ((flag || (num3 & 246144) == 0) && WaterSystem.Collision && WaterSystem.Collision.GetIgnore(pos, 0.01f))
				{
					result = waterInfo;
				}
				else
				{
					waterInfo.isValid = true;
					waterInfo.currentDepth = Mathf.Max(0f, num2 - pos.y);
					waterInfo.overallDepth = Mathf.Max(0f, num2 - num);
					waterInfo.surfaceLevel = num2;
					result = waterInfo;
				}
			}
		}
		return result;
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x0008FD24 File Offset: 0x0008DF24
	public static WaterLevel.WaterInfo GetWaterInfo(Vector3 pos)
	{
		WaterLevel.WaterInfo result;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0.1f))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			float num = TerrainMeta.WaterMap ? TerrainMeta.WaterMap.GetHeight(pos) : 0f;
			if (pos.y > num)
			{
				result = waterInfo;
			}
			else
			{
				float num2 = TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(pos) : 0f;
				if (pos.y < num2 - 1f)
				{
					num = 0f;
					if (pos.y > num)
					{
						return waterInfo;
					}
				}
				if (WaterSystem.Collision && WaterSystem.Collision.GetIgnore(pos, 0.01f))
				{
					result = waterInfo;
				}
				else
				{
					waterInfo.isValid = true;
					waterInfo.currentDepth = Mathf.Max(0f, num - pos.y);
					waterInfo.overallDepth = Mathf.Max(0f, num - num2);
					waterInfo.surfaceLevel = num;
					result = waterInfo;
				}
			}
		}
		return result;
	}

	// Token: 0x06001962 RID: 6498 RVA: 0x0008FE3C File Offset: 0x0008E03C
	public static WaterLevel.WaterInfo GetWaterInfo(Bounds bounds)
	{
		WaterLevel.WaterInfo result;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0.1f))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			float num = TerrainMeta.WaterMap ? TerrainMeta.WaterMap.GetHeight(bounds.center) : 0f;
			if (bounds.min.y > num)
			{
				result = waterInfo;
			}
			else
			{
				float num2 = TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(bounds.center) : 0f;
				if (bounds.max.y < num2 - 1f)
				{
					num = 0f;
					if (bounds.min.y > num)
					{
						return waterInfo;
					}
				}
				if (WaterSystem.Collision && WaterSystem.Collision.GetIgnore(bounds))
				{
					result = waterInfo;
				}
				else
				{
					waterInfo.isValid = true;
					waterInfo.currentDepth = Mathf.Max(0f, num - bounds.min.y);
					waterInfo.overallDepth = Mathf.Max(0f, num - num2);
					waterInfo.surfaceLevel = num;
					result = waterInfo;
				}
			}
		}
		return result;
	}

	// Token: 0x02000412 RID: 1042
	public struct WaterInfo
	{
		// Token: 0x040015C9 RID: 5577
		public bool isValid;

		// Token: 0x040015CA RID: 5578
		public float currentDepth;

		// Token: 0x040015CB RID: 5579
		public float overallDepth;

		// Token: 0x040015CC RID: 5580
		public float surfaceLevel;
	}
}
