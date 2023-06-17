using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200050E RID: 1294
public class GenerateRiverLayout : ProceduralComponent
{
	// Token: 0x04001A0A RID: 6666
	public const float Width = 24f;

	// Token: 0x04001A0B RID: 6667
	public const float InnerPadding = 0.5f;

	// Token: 0x04001A0C RID: 6668
	public const float OuterPadding = 0.5f;

	// Token: 0x04001A0D RID: 6669
	public const float InnerFade = 8f;

	// Token: 0x04001A0E RID: 6670
	public const float OuterFade = 16f;

	// Token: 0x04001A0F RID: 6671
	public const float RandomScale = 0.75f;

	// Token: 0x04001A10 RID: 6672
	public const float MeshOffset = -0.4f;

	// Token: 0x04001A11 RID: 6673
	public const float TerrainOffset = -2f;

	// Token: 0x06001DC1 RID: 7617 RVA: 0x000A1F9C File Offset: 0x000A019C
	public override void Process(uint seed)
	{
		List<PathList> list = new List<PathList>();
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		List<Vector3> list2 = new List<Vector3>();
		for (float num = TerrainMeta.Position.z; num < TerrainMeta.Position.z + TerrainMeta.Size.z; num += 50f)
		{
			for (float num2 = TerrainMeta.Position.x; num2 < TerrainMeta.Position.x + TerrainMeta.Size.x; num2 += 50f)
			{
				Vector3 vector = new Vector3(num2, 0f, num);
				float num3 = vector.y = heightMap.GetHeight(vector);
				if (vector.y > 5f)
				{
					Vector3 normal = heightMap.GetNormal(vector);
					if (normal.y > 0.01f)
					{
						Vector2 normalized = new Vector2(normal.x, normal.z).normalized;
						list2.Add(vector);
						float radius = 12f;
						int num4 = 12;
						int i = 0;
						while (i < 10000)
						{
							vector.x += normalized.x;
							vector.z += normalized.y;
							if (heightMap.GetSlope(vector) > 30f)
							{
								break;
							}
							float height = heightMap.GetHeight(vector);
							if (height > num3 + 10f)
							{
								break;
							}
							vector.y = Mathf.Min(height, num3);
							list2.Add(vector);
							int topology = topologyMap.GetTopology(vector, radius);
							int topology2 = topologyMap.GetTopology(vector);
							int num5 = 2694148;
							int num6 = 128;
							if ((topology & num5) != 0)
							{
								break;
							}
							if ((topology2 & num6) != 0 && --num4 <= 0)
							{
								if (list2.Count >= 300)
								{
									list.Add(new PathList("River " + list.Count, list2.ToArray())
									{
										Width = 24f,
										InnerPadding = 0.5f,
										OuterPadding = 0.5f,
										InnerFade = 8f,
										OuterFade = 16f,
										RandomScale = 0.75f,
										MeshOffset = -0.4f,
										TerrainOffset = -2f,
										Topology = 16384,
										Splat = 64,
										Start = true,
										End = true
									});
									break;
								}
								break;
							}
							else
							{
								normal = heightMap.GetNormal(vector);
								normalized = new Vector2(normalized.x + 0.15f * normal.x, normalized.y + 0.15f * normal.z).normalized;
								num3 = vector.y;
								i++;
							}
						}
						list2.Clear();
					}
				}
			}
		}
		list.Sort((PathList a, PathList b) => b.Path.Points.Length.CompareTo(a.Path.Points.Length));
		int num7 = Mathf.RoundToInt(10f * TerrainMeta.Size.x * TerrainMeta.Size.z * 1E-06f);
		int num8 = Mathf.NextPowerOfTwo((int)(World.Size / 24f));
		bool[,] array = new bool[num8, num8];
		for (int j = 0; j < list.Count; j++)
		{
			if (j >= num7)
			{
				ListEx.RemoveUnordered<PathList>(list, j--);
			}
			else
			{
				PathList pathList = list[j];
				for (int k = 0; k < j; k++)
				{
					if (Vector3.Distance(list[k].Path.GetStartPoint(), pathList.Path.GetStartPoint()) < 100f)
					{
						ListEx.RemoveUnordered<PathList>(list, j--);
					}
				}
				int num9 = -1;
				int num10 = -1;
				for (int l = 0; l < pathList.Path.Points.Length; l++)
				{
					Vector3 vector2 = pathList.Path.Points[l];
					int num11 = Mathf.Clamp((int)(TerrainMeta.NormalizeX(vector2.x) * (float)num8), 0, num8 - 1);
					int num12 = Mathf.Clamp((int)(TerrainMeta.NormalizeZ(vector2.z) * (float)num8), 0, num8 - 1);
					if (num9 != num11 || num10 != num12)
					{
						if (array[num12, num11])
						{
							ListEx.RemoveUnordered<PathList>(list, j--);
							break;
						}
						num9 = num11;
						num10 = num12;
						array[num12, num11] = true;
					}
				}
			}
		}
		TerrainMeta.Path.Rivers.AddRange(list);
	}
}
