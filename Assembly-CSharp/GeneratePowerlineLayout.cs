using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000506 RID: 1286
public class GeneratePowerlineLayout : ProceduralComponent
{
	// Token: 0x040019FE RID: 6654
	public const float Width = 10f;

	// Token: 0x040019FF RID: 6655
	private const int MaxDepth = 100000;

	// Token: 0x06001DAF RID: 7599 RVA: 0x000A19D0 File Offset: 0x0009FBD0
	public override void Process(uint seed)
	{
		List<PathList> list = new List<PathList>();
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		List<MonumentInfo> monuments = TerrainMeta.Path.Monuments;
		if (monuments.Count == 0)
		{
			return;
		}
		int num = Mathf.NextPowerOfTwo((int)(World.Size / 10f));
		int[,] array = new int[num, num];
		float radius = 5f;
		for (int i = 0; i < num; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float normX = ((float)j + 0.5f) / (float)num;
				float slope = heightMap.GetSlope(normX, normZ);
				int topology = topologyMap.GetTopology(normX, normZ, radius);
				int num2 = 2295174;
				int num3 = 55296;
				int num4 = 512;
				if ((topology & num2) != 0)
				{
					array[i, j] = int.MaxValue;
				}
				else if ((topology & num3) != 0)
				{
					array[i, j] = 2500;
				}
				else if ((topology & num4) != 0)
				{
					array[i, j] = 1000;
				}
				else
				{
					array[i, j] = 1 + (int)(slope * slope * 10f);
				}
			}
		}
		PathFinder pathFinder = new PathFinder(array, true);
		List<GeneratePowerlineLayout.PathSegment> list2 = new List<GeneratePowerlineLayout.PathSegment>();
		List<GeneratePowerlineLayout.PathNode> list3 = new List<GeneratePowerlineLayout.PathNode>();
		List<GeneratePowerlineLayout.PathNode> list4 = new List<GeneratePowerlineLayout.PathNode>();
		List<PathFinder.Point> list5 = new List<PathFinder.Point>();
		List<PathFinder.Point> list6 = new List<PathFinder.Point>();
		List<PathFinder.Point> list7 = new List<PathFinder.Point>();
		using (List<MonumentInfo>.Enumerator enumerator = monuments.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MonumentInfo monumentInfo = enumerator.Current;
				bool flag = list3.Count == 0;
				foreach (TerrainPathConnect terrainPathConnect in monumentInfo.GetTargets(InfrastructureType.Power))
				{
					PathFinder.Point point = terrainPathConnect.GetPoint(num);
					PathFinder.Node node = pathFinder.FindClosestWalkable(point, 100000);
					if (node != null)
					{
						GeneratePowerlineLayout.PathNode pathNode = new GeneratePowerlineLayout.PathNode();
						pathNode.monument = monumentInfo;
						pathNode.node = node;
						if (flag)
						{
							list3.Add(pathNode);
						}
						else
						{
							list4.Add(pathNode);
						}
					}
				}
			}
			goto IL_3D3;
		}
		IL_220:
		list6.Clear();
		list7.Clear();
		list6.AddRange(Enumerable.Select<GeneratePowerlineLayout.PathNode, PathFinder.Point>(list3, (GeneratePowerlineLayout.PathNode x) => x.node.point));
		list6.AddRange(list5);
		list7.AddRange(Enumerable.Select<GeneratePowerlineLayout.PathNode, PathFinder.Point>(list4, (GeneratePowerlineLayout.PathNode x) => x.node.point));
		PathFinder.Node node2 = pathFinder.FindPathUndirected(list6, list7, 100000);
		if (node2 == null)
		{
			GeneratePowerlineLayout.PathNode copy = list4[0];
			list3.AddRange(Enumerable.Where<GeneratePowerlineLayout.PathNode>(list4, (GeneratePowerlineLayout.PathNode x) => x.monument == copy.monument));
			list4.RemoveAll((GeneratePowerlineLayout.PathNode x) => x.monument == copy.monument);
		}
		else
		{
			GeneratePowerlineLayout.PathSegment segment = new GeneratePowerlineLayout.PathSegment();
			for (PathFinder.Node node3 = node2; node3 != null; node3 = node3.next)
			{
				if (node3 == node2)
				{
					segment.start = node3;
				}
				if (node3.next == null)
				{
					segment.end = node3;
				}
			}
			list2.Add(segment);
			GeneratePowerlineLayout.PathNode copy = list4.Find((GeneratePowerlineLayout.PathNode x) => x.node.point == segment.start.point || x.node.point == segment.end.point);
			list3.AddRange(Enumerable.Where<GeneratePowerlineLayout.PathNode>(list4, (GeneratePowerlineLayout.PathNode x) => x.monument == copy.monument));
			list4.RemoveAll((GeneratePowerlineLayout.PathNode x) => x.monument == copy.monument);
			int num5 = 1;
			for (PathFinder.Node node4 = node2; node4 != null; node4 = node4.next)
			{
				if (num5 % 8 == 0)
				{
					list5.Add(node4.point);
				}
				num5++;
			}
		}
		IL_3D3:
		if (list4.Count == 0)
		{
			List<Vector3> list8 = new List<Vector3>();
			foreach (GeneratePowerlineLayout.PathSegment pathSegment in list2)
			{
				for (PathFinder.Node node5 = pathSegment.start; node5 != null; node5 = node5.next)
				{
					float num6 = ((float)node5.point.x + 0.5f) / (float)num;
					float num7 = ((float)node5.point.y + 0.5f) / (float)num;
					float height = heightMap.GetHeight01(num6, num7);
					list8.Add(TerrainMeta.Denormalize(new Vector3(num6, height, num7)));
				}
				if (list8.Count != 0)
				{
					if (list8.Count >= 8)
					{
						list.Add(new PathList("Powerline " + list.Count, list8.ToArray())
						{
							Start = true,
							End = true
						});
					}
					list8.Clear();
				}
			}
			TerrainMeta.Path.Powerlines.AddRange(list);
			return;
		}
		goto IL_220;
	}

	// Token: 0x02000507 RID: 1287
	private class PathNode
	{
		// Token: 0x04001A00 RID: 6656
		public MonumentInfo monument;

		// Token: 0x04001A01 RID: 6657
		public PathFinder.Node node;
	}

	// Token: 0x02000508 RID: 1288
	private class PathSegment
	{
		// Token: 0x04001A02 RID: 6658
		public PathFinder.Node start;

		// Token: 0x04001A03 RID: 6659
		public PathFinder.Node end;
	}
}
