using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200048B RID: 1163
public class PathList
{
	// Token: 0x040017EE RID: 6126
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x040017EF RID: 6127
	private static Quaternion rot180 = Quaternion.Euler(0f, 180f, 0f);

	// Token: 0x040017F0 RID: 6128
	public string Name;

	// Token: 0x040017F1 RID: 6129
	public PathInterpolator Path;

	// Token: 0x040017F2 RID: 6130
	public bool Spline;

	// Token: 0x040017F3 RID: 6131
	public bool Start;

	// Token: 0x040017F4 RID: 6132
	public bool End;

	// Token: 0x040017F5 RID: 6133
	public float Width;

	// Token: 0x040017F6 RID: 6134
	public float InnerPadding;

	// Token: 0x040017F7 RID: 6135
	public float OuterPadding;

	// Token: 0x040017F8 RID: 6136
	public float InnerFade;

	// Token: 0x040017F9 RID: 6137
	public float OuterFade;

	// Token: 0x040017FA RID: 6138
	public float RandomScale;

	// Token: 0x040017FB RID: 6139
	public float MeshOffset;

	// Token: 0x040017FC RID: 6140
	public float TerrainOffset;

	// Token: 0x040017FD RID: 6141
	public int Topology;

	// Token: 0x040017FE RID: 6142
	public int Splat;

	// Token: 0x040017FF RID: 6143
	public const float StepSize = 1f;

	// Token: 0x04001800 RID: 6144
	public const float MeshStepSize = 8f;

	// Token: 0x04001801 RID: 6145
	public const float MeshNormalSmoothing = 0.1f;

	// Token: 0x04001802 RID: 6146
	public const int SubMeshVerts = 100;

	// Token: 0x04001803 RID: 6147
	private static float[] placements = new float[]
	{
		default(float),
		-1f,
		1f
	};

	// Token: 0x06001B2C RID: 6956 RVA: 0x00016676 File Offset: 0x00014876
	public PathList(string name, Vector3[] points)
	{
		this.Name = name;
		this.Path = new PathInterpolator(points);
	}

	// Token: 0x06001B2D RID: 6957 RVA: 0x00095EF0 File Offset: 0x000940F0
	private void SpawnObjectsNeighborAligned(ref uint seed, Prefab[] prefabs, List<Vector3> positions, SpawnFilter filter = null)
	{
		if (positions.Count < 2)
		{
			return;
		}
		for (int i = 0; i < positions.Count; i++)
		{
			int num = Mathf.Max(i - 1, 0);
			int num2 = Mathf.Min(i + 1, positions.Count - 1);
			Vector3 position = positions[i];
			Quaternion rotation = Quaternion.LookRotation(Vector3Ex.XZ3D(positions[num2] - positions[num]));
			this.SpawnObject(ref seed, prefabs, position, rotation, filter);
		}
	}

	// Token: 0x06001B2E RID: 6958 RVA: 0x00095F68 File Offset: 0x00094168
	private bool SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		Prefab random = prefabs.GetRandom(ref seed);
		Vector3 vector = position;
		Quaternion quaternion = rotation;
		Vector3 localScale = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref vector, ref quaternion, ref localScale);
		if (!random.ApplyTerrainAnchors(ref vector, quaternion, localScale, filter))
		{
			return false;
		}
		random.ApplyTerrainPlacements(vector, quaternion, localScale);
		random.ApplyTerrainModifiers(vector, quaternion, localScale);
		World.AddPrefab(this.Name, random.ID, vector, quaternion, localScale);
		return true;
	}

	// Token: 0x06001B2F RID: 6959 RVA: 0x00095FD8 File Offset: 0x000941D8
	private bool CheckObjects(Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		foreach (Prefab prefab in prefabs)
		{
			Vector3 vector = position;
			Vector3 localScale = prefab.Object.transform.localScale;
			if (!prefab.ApplyTerrainAnchors(ref vector, rotation, localScale, filter))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001B30 RID: 6960 RVA: 0x0009601C File Offset: 0x0009421C
	private void SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 pos, Vector3 dir, PathList.BasicObject obj)
	{
		if (!obj.AlignToNormal)
		{
			dir = Vector3Ex.XZ3D(dir).normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 a = (this.Width * 0.5f + obj.Offset) * (PathList.rot90 * dir);
		for (int i = 0; i < PathList.placements.Length; i++)
		{
			if ((obj.Placement != PathList.Placement.Center || i == 0) && (obj.Placement != PathList.Placement.Side || i != 0))
			{
				Vector3 vector = pos + PathList.placements[i] * a;
				if (obj.HeightToTerrain)
				{
					vector.y = TerrainMeta.HeightMap.GetHeight(vector);
				}
				if (filter.Test(vector))
				{
					Quaternion rotation = (i == 2) ? Quaternion.LookRotation(PathList.rot180 * dir) : Quaternion.LookRotation(dir);
					if (this.SpawnObject(ref seed, prefabs, vector, rotation, filter))
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06001B31 RID: 6961 RVA: 0x0009610C File Offset: 0x0009430C
	private bool CheckObjects(Prefab[] prefabs, Vector3 pos, Vector3 dir, PathList.BasicObject obj)
	{
		if (!obj.AlignToNormal)
		{
			dir = Vector3Ex.XZ3D(dir).normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 a = (this.Width * 0.5f + obj.Offset) * (PathList.rot90 * dir);
		for (int i = 0; i < PathList.placements.Length; i++)
		{
			if ((obj.Placement != PathList.Placement.Center || i == 0) && (obj.Placement != PathList.Placement.Side || i != 0))
			{
				Vector3 vector = pos + PathList.placements[i] * a;
				if (obj.HeightToTerrain)
				{
					vector.y = TerrainMeta.HeightMap.GetHeight(vector);
				}
				if (filter.Test(vector))
				{
					Quaternion rotation = (i == 2) ? Quaternion.LookRotation(PathList.rot180 * dir) : Quaternion.LookRotation(dir);
					if (this.CheckObjects(prefabs, vector, rotation, filter))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06001B32 RID: 6962 RVA: 0x000961FC File Offset: 0x000943FC
	public void SpawnSide(ref uint seed, PathList.SideObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		PathList.Side side = obj.Side;
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float num = this.Width * 0.5f + obj.Offset;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		float[] array2 = new float[]
		{
			-num,
			num
		};
		int num2 = 0;
		Vector3 b = this.Path.GetStartPoint();
		List<Vector3> list = new List<Vector3>();
		float num3 = distance * 0.25f;
		float num4 = distance * 0.5f;
		float num5 = this.Path.StartOffset + num4;
		float num6 = this.Path.Length - this.Path.EndOffset - num4;
		for (float num7 = num5; num7 <= num6; num7 += num3)
		{
			Vector3 vector = this.Spline ? this.Path.GetPointCubicHermite(num7) : this.Path.GetPoint(num7);
			if ((vector - b).magnitude >= distance)
			{
				Vector3 tangent = this.Path.GetTangent(num7);
				Vector3 vector2 = PathList.rot90 * tangent;
				for (int i = 0; i < array2.Length; i++)
				{
					int num8 = (num2 + i) % array2.Length;
					if ((side != PathList.Side.Left || num8 == 0) && (side != PathList.Side.Right || num8 == 1))
					{
						float num9 = array2[num8];
						Vector3 vector3 = vector;
						vector3.x += vector2.x * num9;
						vector3.z += vector2.z * num9;
						float normX = TerrainMeta.NormalizeX(vector3.x);
						float normZ = TerrainMeta.NormalizeZ(vector3.z);
						if (filter.GetFactor(normX, normZ) >= SeedRandom.Value(ref seed))
						{
							if (density >= SeedRandom.Value(ref seed))
							{
								vector3.y = heightMap.GetHeight(normX, normZ);
								if (obj.Alignment == PathList.Alignment.None)
								{
									if (!this.SpawnObject(ref seed, array, vector3, Quaternion.identity, filter))
									{
										goto IL_279;
									}
								}
								else if (obj.Alignment == PathList.Alignment.Forward)
								{
									if (!this.SpawnObject(ref seed, array, vector3, Quaternion.LookRotation(tangent * num9), filter))
									{
										goto IL_279;
									}
								}
								else if (obj.Alignment == PathList.Alignment.Inward)
								{
									if (!this.SpawnObject(ref seed, array, vector3, Quaternion.LookRotation(-vector2 * num9), filter))
									{
										goto IL_279;
									}
								}
								else
								{
									list.Add(vector3);
								}
							}
							num2 = num8;
							b = vector;
							if (side == PathList.Side.Any)
							{
								break;
							}
						}
					}
					IL_279:;
				}
			}
		}
		if (list.Count > 0)
		{
			this.SpawnObjectsNeighborAligned(ref seed, array, list, filter);
		}
	}

	// Token: 0x06001B33 RID: 6963 RVA: 0x000964B8 File Offset: 0x000946B8
	public void SpawnAlong(ref uint seed, PathList.PathObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float dithering = obj.Dithering;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 b = this.Path.GetStartPoint();
		List<Vector3> list = new List<Vector3>();
		float num = distance * 0.25f;
		float num2 = distance * 0.5f;
		float num3 = this.Path.StartOffset + num2;
		float num4 = this.Path.Length - this.Path.EndOffset - num2;
		for (float num5 = num3; num5 <= num4; num5 += num)
		{
			Vector3 vector = this.Spline ? this.Path.GetPointCubicHermite(num5) : this.Path.GetPoint(num5);
			if ((vector - b).magnitude >= distance)
			{
				Vector3 tangent = this.Path.GetTangent(num5);
				Vector3 forward = PathList.rot90 * tangent;
				Vector3 vector2 = vector;
				vector2.x += SeedRandom.Range(ref seed, -dithering, dithering);
				vector2.z += SeedRandom.Range(ref seed, -dithering, dithering);
				float normX = TerrainMeta.NormalizeX(vector2.x);
				float normZ = TerrainMeta.NormalizeZ(vector2.z);
				if (filter.GetFactor(normX, normZ) >= SeedRandom.Value(ref seed))
				{
					if (density >= SeedRandom.Value(ref seed))
					{
						vector2.y = heightMap.GetHeight(normX, normZ);
						if (obj.Alignment == PathList.Alignment.None)
						{
							if (!this.SpawnObject(ref seed, array, vector2, Quaternion.identity, filter))
							{
								goto IL_1FD;
							}
						}
						else if (obj.Alignment == PathList.Alignment.Forward)
						{
							if (!this.SpawnObject(ref seed, array, vector2, Quaternion.LookRotation(tangent), filter))
							{
								goto IL_1FD;
							}
						}
						else if (obj.Alignment == PathList.Alignment.Inward)
						{
							if (!this.SpawnObject(ref seed, array, vector2, Quaternion.LookRotation(forward), filter))
							{
								goto IL_1FD;
							}
						}
						else
						{
							list.Add(vector2);
						}
					}
					b = vector;
				}
			}
			IL_1FD:;
		}
		if (list.Count > 0)
		{
			this.SpawnObjectsNeighborAligned(ref seed, array, list, filter);
		}
	}

	// Token: 0x06001B34 RID: 6964 RVA: 0x000966E8 File Offset: 0x000948E8
	public void SpawnBridge(ref uint seed, PathList.BridgeObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 a = this.Path.GetEndPoint() - startPoint;
		float magnitude = a.magnitude;
		Vector3 vector = a / magnitude;
		float num = magnitude / obj.Distance;
		int num2 = Mathf.RoundToInt(num);
		float num3 = 0.5f * (num - (float)num2);
		Vector3 vector2 = obj.Distance * vector;
		Vector3 vector3 = startPoint + (0.5f + num3) * vector2;
		Quaternion rotation = Quaternion.LookRotation(vector);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		for (int i = 0; i < num2; i++)
		{
			float num4 = Mathf.Max(heightMap.GetHeight(vector3), waterMap.GetHeight(vector3)) - 1f;
			if (vector3.y > num4)
			{
				this.SpawnObject(ref seed, array, vector3, rotation, null);
			}
			vector3 += vector2;
		}
	}

	// Token: 0x06001B35 RID: 6965 RVA: 0x0009681C File Offset: 0x00094A1C
	public void SpawnStart(ref uint seed, PathList.BasicObject obj)
	{
		if (!this.Start)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 startTangent = this.Path.GetStartTangent();
		this.SpawnObject(ref seed, array, startPoint, startTangent, obj);
	}

	// Token: 0x06001B36 RID: 6966 RVA: 0x0009689C File Offset: 0x00094A9C
	public void SpawnEnd(ref uint seed, PathList.BasicObject obj)
	{
		if (!this.End)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 dir = -this.Path.GetEndTangent();
		this.SpawnObject(ref seed, array, endPoint, dir, obj);
	}

	// Token: 0x06001B37 RID: 6967 RVA: 0x00096920 File Offset: 0x00094B20
	public void TrimStart(PathList.BasicObject obj)
	{
		if (!this.Start)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3[] points = this.Path.Points;
		Vector3[] tangents = this.Path.Tangents;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 pos = points[this.Path.MinIndex + i];
			Vector3 dir = tangents[this.Path.MinIndex + i];
			if (this.CheckObjects(array, pos, dir, obj))
			{
				this.Path.MinIndex += i;
				return;
			}
		}
	}

	// Token: 0x06001B38 RID: 6968 RVA: 0x000969F8 File Offset: 0x00094BF8
	public void TrimEnd(PathList.BasicObject obj)
	{
		if (!this.End)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3[] points = this.Path.Points;
		Vector3[] tangents = this.Path.Tangents;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 pos = points[this.Path.MaxIndex - i];
			Vector3 dir = -tangents[this.Path.MaxIndex - i];
			if (this.CheckObjects(array, pos, dir, obj))
			{
				this.Path.MaxIndex -= i;
				return;
			}
		}
	}

	// Token: 0x06001B39 RID: 6969 RVA: 0x00016691 File Offset: 0x00014891
	public void ResetTrims()
	{
		this.Path.MinIndex = this.Path.DefaultMinIndex;
		this.Path.MaxIndex = this.Path.DefaultMaxIndex;
	}

	// Token: 0x06001B3A RID: 6970 RVA: 0x00096AD4 File Offset: 0x00094CD4
	public void AdjustTerrainHeight()
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float outerFade = this.OuterFade;
		float innerFade = this.InnerFade;
		float offset01 = this.TerrainOffset * TerrainMeta.OneOverSize.y;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 a = PathList.rot90 * vector;
		Vector3 v = startPoint - a * (num2 + outerPadding + outerFade);
		Vector3 v2 = startPoint + a * (num2 + outerPadding + outerFade);
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector2 = this.Spline ? this.Path.GetPointCubicHermite(num4) : this.Path.GetPoint(num4);
			float a2 = Vector3Ex.Magnitude2D(startPoint - vector2);
			float b = Vector3Ex.Magnitude2D(endPoint - vector2);
			float opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(a2, b));
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector2.x, vector2.z, 2, 0.005f, 1f, 2f, 0.5f));
			vector = Vector3Ex.XZ3D(this.Path.GetTangent(num4)).normalized;
			a = PathList.rot90 * vector;
			Ray ray = new Ray(vector2, vector);
			Vector3 vector3 = vector2 - a * (radius + outerPadding + outerFade);
			Vector3 vector4 = vector2 + a * (radius + outerPadding + outerFade);
			float yn = TerrainMeta.NormalizeY(vector2.y);
			heightmap.ForEach(v, v2, vector3, vector4, delegate(int x, int z)
			{
				float num5 = heightmap.Coordinate(x);
				float num6 = heightmap.Coordinate(z);
				if ((topomap.GetTopology(num5, num6) & this.Topology) != 0)
				{
					return;
				}
				Vector3 vector5 = TerrainMeta.Denormalize(new Vector3(num5, yn, num6));
				Vector3 vector6 = ray.ClosestPoint(vector5);
				float value = Vector3Ex.Magnitude2D(vector5 - vector6);
				float num7 = Mathf.InverseLerp(radius + outerPadding + outerFade, radius + outerPadding, value);
				float num8 = Mathf.InverseLerp(radius - innerPadding, radius - innerPadding - innerFade, value);
				float num9 = TerrainMeta.NormalizeY(vector6.y);
				num7 = Mathf.SmoothStep(0f, 1f, num7);
				num8 = Mathf.SmoothStep(0f, 1f, num8);
				heightmap.SetHeight(x, z, num9 + offset01 * num8, opacity * num7);
			});
			v = vector3;
			v2 = vector4;
		}
	}

	// Token: 0x06001B3B RID: 6971 RVA: 0x00096D88 File Offset: 0x00094F88
	public void AdjustTerrainTexture()
	{
		if (this.Splat == 0)
		{
			return;
		}
		TerrainSplatMap splatmap = TerrainMeta.SplatMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 a = PathList.rot90 * vector;
		Vector3 v = startPoint - a * (num2 + outerPadding);
		Vector3 v2 = startPoint + a * (num2 + outerPadding);
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector2 = this.Spline ? this.Path.GetPointCubicHermite(num4) : this.Path.GetPoint(num4);
			float a2 = Vector3Ex.Magnitude2D(startPoint - vector2);
			float b = Vector3Ex.Magnitude2D(endPoint - vector2);
			float opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(a2, b));
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector2.x, vector2.z, 2, 0.005f, 1f, 2f, 0.5f));
			vector = Vector3Ex.XZ3D(this.Path.GetTangent(num4)).normalized;
			a = PathList.rot90 * vector;
			Ray ray = new Ray(vector2, vector);
			Vector3 vector3 = vector2 - a * (radius + outerPadding);
			Vector3 vector4 = vector2 + a * (radius + outerPadding);
			float yn = TerrainMeta.NormalizeY(vector2.y);
			splatmap.ForEach(v, v2, vector3, vector4, delegate(int x, int z)
			{
				float x2 = splatmap.Coordinate(x);
				float z2 = splatmap.Coordinate(z);
				Vector3 vector5 = TerrainMeta.Denormalize(new Vector3(x2, yn, z2));
				Vector3 b2 = ray.ClosestPoint(vector5);
				float value = Vector3Ex.Magnitude2D(vector5 - b2);
				float num5 = Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, value);
				splatmap.SetSplat(x, z, this.Splat, num5 * opacity);
			});
			v = vector3;
			v2 = vector4;
		}
	}

	// Token: 0x06001B3C RID: 6972 RVA: 0x00096FE4 File Offset: 0x000951E4
	public void AdjustTerrainTopology()
	{
		if (this.Topology == 0)
		{
			return;
		}
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 a = PathList.rot90 * vector;
		Vector3 v = startPoint - a * (num2 + outerPadding);
		Vector3 v2 = startPoint + a * (num2 + outerPadding);
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector2 = this.Spline ? this.Path.GetPointCubicHermite(num4) : this.Path.GetPoint(num4);
			float a2 = Vector3Ex.Magnitude2D(startPoint - vector2);
			float b = Vector3Ex.Magnitude2D(endPoint - vector2);
			float opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(a2, b));
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector2.x, vector2.z, 2, 0.005f, 1f, 2f, 0.5f));
			vector = Vector3Ex.XZ3D(this.Path.GetTangent(num4)).normalized;
			a = PathList.rot90 * vector;
			Ray ray = new Ray(vector2, vector);
			Vector3 vector3 = vector2 - a * (radius + outerPadding);
			Vector3 vector4 = vector2 + a * (radius + outerPadding);
			float yn = TerrainMeta.NormalizeY(vector2.y);
			topomap.ForEach(v, v2, vector3, vector4, delegate(int x, int z)
			{
				float x2 = topomap.Coordinate(x);
				float z2 = topomap.Coordinate(z);
				Vector3 vector5 = TerrainMeta.Denormalize(new Vector3(x2, yn, z2));
				Vector3 b2 = ray.ClosestPoint(vector5);
				float value = Vector3Ex.Magnitude2D(vector5 - b2);
				if (Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, value) * opacity > 0.3f)
				{
					topomap.SetTopology(x, z, this.Topology);
				}
			});
			v = vector3;
			v2 = vector4;
		}
	}

	// Token: 0x06001B3D RID: 6973 RVA: 0x00097240 File Offset: 0x00095440
	public List<Mesh> CreateMesh()
	{
		List<Mesh> list = new List<Mesh>();
		float num = 8f;
		float num2 = 64f;
		float randomScale = this.RandomScale;
		float meshOffset = this.MeshOffset;
		float num3 = this.Width * 0.5f;
		int num4 = (int)(this.Path.Length / num) * 2;
		int num5 = (int)(this.Path.Length / num) * 3;
		List<Vector3> list2 = new List<Vector3>(num4);
		List<Color> list3 = new List<Color>(num4);
		List<Vector2> list4 = new List<Vector2>(num4);
		List<Vector3> list5 = new List<Vector3>(num4);
		List<Vector4> list6 = new List<Vector4>(num4);
		List<int> list7 = new List<int>(num5);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector2 vector = new Vector2(0f, 0f);
		Vector2 vector2 = new Vector2(1f, 0f);
		Vector3 vector3 = Vector3.zero;
		Vector3 vector4 = Vector3.zero;
		Vector3 b = Vector3.zero;
		Vector3 rhs = Vector3.zero;
		int num6 = -1;
		int num7 = -1;
		float num8 = this.Path.Length + num;
		for (float num9 = 0f; num9 < num8; num9 += num)
		{
			Vector3 vector5 = this.Spline ? this.Path.GetPointCubicHermite(num9) : this.Path.GetPoint(num9);
			float num10 = Mathf.Lerp(num3, num3 * randomScale, Noise.Billow(vector5.x, vector5.z, 2, 0.005f, 1f, 2f, 0.5f));
			Vector3 tangent = this.Path.GetTangent(num9);
			Vector3 normalized = Vector3Ex.XZ3D(tangent).normalized;
			Vector3 vector6 = PathList.rot90 * normalized;
			Vector4 vector7 = new Vector4(vector6.x, vector6.y, vector6.z, 1f);
			Vector3 vector8 = Vector3.Slerp(Vector3.Cross(tangent, vector6), Vector3.up, 0.1f);
			Vector3 vector9 = new Vector3(vector5.x - vector6.x * num10, 0f, vector5.z - vector6.z * num10);
			vector9.y = Mathf.Min(vector5.y, heightMap.GetHeight(vector9)) + meshOffset;
			Vector3 vector10 = new Vector3(vector5.x + vector6.x * num10, 0f, vector5.z + vector6.z * num10);
			vector10.y = Mathf.Min(vector5.y, heightMap.GetHeight(vector10)) + meshOffset;
			if (num9 != 0f)
			{
				float num11 = Vector3Ex.Magnitude2D(vector5 - b) / (2f * num10);
				vector.y += num11;
				vector2.y += num11;
				if (Vector3.Dot(Vector3Ex.XZ3D(vector9 - vector3), rhs) <= 0f)
				{
					vector9 = vector3;
				}
				if (Vector3.Dot(Vector3Ex.XZ3D(vector10 - vector4), rhs) <= 0f)
				{
					vector10 = vector4;
				}
			}
			Color color = (num9 > 0f && num9 + num < num8) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f);
			list4.Add(vector);
			list3.Add(color);
			list2.Add(vector9);
			list5.Add(vector8);
			list6.Add(vector7);
			int num12 = list2.Count - 1;
			if (num6 != -1 && num7 != -1)
			{
				list7.Add(num12);
				list7.Add(num7);
				list7.Add(num6);
			}
			num6 = num12;
			vector3 = vector9;
			list4.Add(vector2);
			list3.Add(color);
			list2.Add(vector10);
			list5.Add(vector8);
			list6.Add(vector7);
			int num13 = list2.Count - 1;
			if (num6 != -1 && num7 != -1)
			{
				list7.Add(num13);
				list7.Add(num7);
				list7.Add(num6);
			}
			num7 = num13;
			vector4 = vector10;
			b = vector5;
			rhs = normalized;
			if (list2.Count >= 100 && this.Path.Length - num9 > num2)
			{
				Mesh mesh = new Mesh();
				mesh.SetVertices(list2);
				mesh.SetColors(list3);
				mesh.SetUVs(0, list4);
				mesh.SetTriangles(list7, 0);
				mesh.SetNormals(list5);
				mesh.SetTangents(list6);
				list.Add(mesh);
				list2.Clear();
				list3.Clear();
				list4.Clear();
				list5.Clear();
				list6.Clear();
				list7.Clear();
				num6 = -1;
				num7 = -1;
				num9 -= num;
			}
		}
		if (list7.Count > 0)
		{
			Mesh mesh2 = new Mesh();
			mesh2.SetVertices(list2);
			mesh2.SetColors(list3);
			mesh2.SetUVs(0, list4);
			mesh2.SetTriangles(list7, 0);
			mesh2.SetNormals(list5);
			mesh2.SetTangents(list6);
			list.Add(mesh2);
		}
		return list;
	}

	// Token: 0x0200048C RID: 1164
	public enum Side
	{
		// Token: 0x04001805 RID: 6149
		Both,
		// Token: 0x04001806 RID: 6150
		Left,
		// Token: 0x04001807 RID: 6151
		Right,
		// Token: 0x04001808 RID: 6152
		Any
	}

	// Token: 0x0200048D RID: 1165
	public enum Placement
	{
		// Token: 0x0400180A RID: 6154
		Center,
		// Token: 0x0400180B RID: 6155
		Side
	}

	// Token: 0x0200048E RID: 1166
	public enum Alignment
	{
		// Token: 0x0400180D RID: 6157
		None,
		// Token: 0x0400180E RID: 6158
		Neighbor,
		// Token: 0x0400180F RID: 6159
		Forward,
		// Token: 0x04001810 RID: 6160
		Inward
	}

	// Token: 0x0200048F RID: 1167
	[Serializable]
	public class BasicObject
	{
		// Token: 0x04001811 RID: 6161
		public string Folder;

		// Token: 0x04001812 RID: 6162
		public SpawnFilter Filter;

		// Token: 0x04001813 RID: 6163
		public PathList.Placement Placement;

		// Token: 0x04001814 RID: 6164
		public bool AlignToNormal = true;

		// Token: 0x04001815 RID: 6165
		public bool HeightToTerrain = true;

		// Token: 0x04001816 RID: 6166
		public float Offset;
	}

	// Token: 0x02000490 RID: 1168
	[Serializable]
	public class SideObject
	{
		// Token: 0x04001817 RID: 6167
		public string Folder;

		// Token: 0x04001818 RID: 6168
		public SpawnFilter Filter;

		// Token: 0x04001819 RID: 6169
		public PathList.Side Side;

		// Token: 0x0400181A RID: 6170
		public PathList.Alignment Alignment;

		// Token: 0x0400181B RID: 6171
		public float Density = 1f;

		// Token: 0x0400181C RID: 6172
		public float Distance = 25f;

		// Token: 0x0400181D RID: 6173
		public float Offset = 2f;
	}

	// Token: 0x02000491 RID: 1169
	[Serializable]
	public class PathObject
	{
		// Token: 0x0400181E RID: 6174
		public string Folder;

		// Token: 0x0400181F RID: 6175
		public SpawnFilter Filter;

		// Token: 0x04001820 RID: 6176
		public PathList.Alignment Alignment;

		// Token: 0x04001821 RID: 6177
		public float Density = 1f;

		// Token: 0x04001822 RID: 6178
		public float Distance = 5f;

		// Token: 0x04001823 RID: 6179
		public float Dithering = 5f;
	}

	// Token: 0x02000492 RID: 1170
	[Serializable]
	public class BridgeObject
	{
		// Token: 0x04001824 RID: 6180
		public string Folder;

		// Token: 0x04001825 RID: 6181
		public float Distance = 10f;
	}
}
