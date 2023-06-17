using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000514 RID: 1300
public class GenerateRoadLayout : ProceduralComponent
{
	// Token: 0x04001A1B RID: 6683
	public const float Width = 10f;

	// Token: 0x04001A1C RID: 6684
	public const float InnerPadding = 1f;

	// Token: 0x04001A1D RID: 6685
	public const float OuterPadding = 1f;

	// Token: 0x04001A1E RID: 6686
	public const float InnerFade = 1f;

	// Token: 0x04001A1F RID: 6687
	public const float OuterFade = 8f;

	// Token: 0x04001A20 RID: 6688
	public const float RandomScale = 0.75f;

	// Token: 0x04001A21 RID: 6689
	public const float MeshOffset = --0f;

	// Token: 0x04001A22 RID: 6690
	public const float TerrainOffset = -0.5f;

	// Token: 0x04001A23 RID: 6691
	private const int MaxDepth = 100000;

	// Token: 0x06001DCE RID: 7630 RVA: 0x000A2754 File Offset: 0x000A0954
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
				int num2 = SeedRandom.Range(ref seed, 100, 500);
				float slope = heightMap.GetSlope(normX, normZ);
				int topology = topologyMap.GetTopology(normX, normZ, radius);
				int num3 = 2295686;
				int num4 = 49152;
				if (slope > 20f || (topology & num3) != 0)
				{
					array[i, j] = int.MaxValue;
				}
				else if ((topology & num4) != 0)
				{
					array[i, j] = 2500;
				}
				else
				{
					array[i, j] = 1 + (int)(slope * slope * 10f) + num2;
				}
			}
		}
		PathFinder pathFinder = new PathFinder(array, true);
		List<GenerateRoadLayout.PathSegment> list2 = new List<GenerateRoadLayout.PathSegment>();
		List<GenerateRoadLayout.PathNode> list3 = new List<GenerateRoadLayout.PathNode>();
		List<GenerateRoadLayout.PathNode> list4 = new List<GenerateRoadLayout.PathNode>();
		List<PathFinder.Point> list5 = new List<PathFinder.Point>();
		List<PathFinder.Point> list6 = new List<PathFinder.Point>();
		List<PathFinder.Point> list7 = new List<PathFinder.Point>();
		using (List<MonumentInfo>.Enumerator enumerator = monuments.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MonumentInfo monumentInfo = enumerator.Current;
				bool flag = list3.Count == 0;
				foreach (TerrainPathConnect terrainPathConnect in monumentInfo.GetTargets(InfrastructureType.Road))
				{
					PathFinder.Point point = terrainPathConnect.GetPoint(num);
					PathFinder.Node node = pathFinder.FindClosestWalkable(point, 100000);
					if (node != null)
					{
						GenerateRoadLayout.PathNode pathNode = new GenerateRoadLayout.PathNode();
						pathNode.monument = monumentInfo;
						pathNode.target = terrainPathConnect;
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
			goto IL_3DC;
		}
		IL_229:
		list6.Clear();
		list7.Clear();
		list6.AddRange(Enumerable.Select<GenerateRoadLayout.PathNode, PathFinder.Point>(list3, (GenerateRoadLayout.PathNode x) => x.node.point));
		list6.AddRange(list5);
		list7.AddRange(Enumerable.Select<GenerateRoadLayout.PathNode, PathFinder.Point>(list4, (GenerateRoadLayout.PathNode x) => x.node.point));
		PathFinder.Node node2 = pathFinder.FindPathUndirected(list6, list7, 100000);
		if (node2 == null)
		{
			GenerateRoadLayout.PathNode copy = list4[0];
			list3.AddRange(Enumerable.Where<GenerateRoadLayout.PathNode>(list4, (GenerateRoadLayout.PathNode x) => x.monument == copy.monument));
			list4.RemoveAll((GenerateRoadLayout.PathNode x) => x.monument == copy.monument);
		}
		else
		{
			GenerateRoadLayout.PathSegment segment = new GenerateRoadLayout.PathSegment();
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
			GenerateRoadLayout.PathNode copy = list4.Find((GenerateRoadLayout.PathNode x) => x.node.point == segment.start.point || x.node.point == segment.end.point);
			list3.AddRange(Enumerable.Where<GenerateRoadLayout.PathNode>(list4, (GenerateRoadLayout.PathNode x) => x.monument == copy.monument));
			list4.RemoveAll((GenerateRoadLayout.PathNode x) => x.monument == copy.monument);
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
		IL_3DC:
		if (list4.Count == 0)
		{
			using (List<GenerateRoadLayout.PathNode>.Enumerator enumerator3 = list3.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					GenerateRoadLayout.PathNode target = enumerator3.Current;
					GenerateRoadLayout.PathSegment pathSegment = list2.Find((GenerateRoadLayout.PathSegment x) => x.start.point == target.node.point || x.end.point == target.node.point);
					if (pathSegment != null)
					{
						if (pathSegment.start.point == target.node.point)
						{
							PathFinder.Node node5 = target.node;
							PathFinder.Node start = pathFinder.Reverse(target.node);
							node5.next = pathSegment.start;
							pathSegment.start = start;
							pathSegment.origin = target.target;
						}
						else if (pathSegment.end.point == target.node.point)
						{
							pathSegment.end.next = target.node;
							pathSegment.end = pathFinder.FindEnd(target.node);
							pathSegment.target = target.target;
						}
					}
				}
			}
			List<Vector3> list8 = new List<Vector3>();
			foreach (GenerateRoadLayout.PathSegment pathSegment2 in list2)
			{
				bool start2 = false;
				bool end = false;
				for (PathFinder.Node node6 = pathSegment2.start; node6 != null; node6 = node6.next)
				{
					float normX2 = ((float)node6.point.x + 0.5f) / (float)num;
					float normZ2 = ((float)node6.point.y + 0.5f) / (float)num;
					if (pathSegment2.start == node6 && pathSegment2.origin != null)
					{
						start2 = true;
						normX2 = TerrainMeta.NormalizeX(pathSegment2.origin.transform.position.x);
						normZ2 = TerrainMeta.NormalizeZ(pathSegment2.origin.transform.position.z);
					}
					else if (pathSegment2.end == node6 && pathSegment2.target != null)
					{
						end = true;
						normX2 = TerrainMeta.NormalizeX(pathSegment2.target.transform.position.x);
						normZ2 = TerrainMeta.NormalizeZ(pathSegment2.target.transform.position.z);
					}
					float x2 = TerrainMeta.DenormalizeX(normX2);
					float z = TerrainMeta.DenormalizeZ(normZ2);
					float y = Mathf.Max(heightMap.GetHeight(normX2, normZ2), 1f);
					list8.Add(new Vector3(x2, y, z));
				}
				if (list8.Count != 0)
				{
					if (list8.Count >= 2)
					{
						list.Add(new PathList("Road " + list.Count, list8.ToArray())
						{
							Width = 10f,
							InnerPadding = 1f,
							OuterPadding = 1f,
							InnerFade = 1f,
							OuterFade = 8f,
							RandomScale = 0.75f,
							MeshOffset = --0f,
							TerrainOffset = -0.5f,
							Topology = 2048,
							Splat = 128,
							Start = start2,
							End = end
						});
					}
					list8.Clear();
				}
			}
			foreach (PathList pathList in list)
			{
				pathList.Path.Smoothen(2);
			}
			TerrainMeta.Path.Roads.AddRange(list);
			return;
		}
		goto IL_229;
	}

	// Token: 0x02000515 RID: 1301
	private class PathNode
	{
		// Token: 0x04001A24 RID: 6692
		public MonumentInfo monument;

		// Token: 0x04001A25 RID: 6693
		public TerrainPathConnect target;

		// Token: 0x04001A26 RID: 6694
		public PathFinder.Node node;
	}

	// Token: 0x02000516 RID: 1302
	private class PathSegment
	{
		// Token: 0x04001A27 RID: 6695
		public PathFinder.Node start;

		// Token: 0x04001A28 RID: 6696
		public PathFinder.Node end;

		// Token: 0x04001A29 RID: 6697
		public TerrainPathConnect origin;

		// Token: 0x04001A2A RID: 6698
		public TerrainPathConnect target;
	}
}
