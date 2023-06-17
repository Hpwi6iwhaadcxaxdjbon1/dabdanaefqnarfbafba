using System;
using UnityEngine;

// Token: 0x02000542 RID: 1346
public class Monument : TerrainPlacement
{
	// Token: 0x04001A94 RID: 6804
	public float Radius;

	// Token: 0x04001A95 RID: 6805
	public float Fade = 10f;

	// Token: 0x06001E3B RID: 7739 RVA: 0x000A5BA0 File Offset: 0x000A3DA0
	protected void OnDrawGizmosSelected()
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.Radius);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.Radius - this.Fade);
	}

	// Token: 0x06001E3C RID: 7740 RVA: 0x000A5C1C File Offset: 0x000A3E1C
	protected override void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool useBlendMap = this.blendmap != null;
		Vector3 position = localToWorld.MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(this.heightmap);
		TextureData blenddata = useBlendMap ? new TextureData(this.blendmap) : default(TextureData);
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.HeightMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			float normZ = TerrainMeta.HeightMap.Coordinate(z);
			float normX = TerrainMeta.HeightMap.Coordinate(x);
			Vector3 vector = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector2 = worldToLocal.MultiplyPoint3x4(vector) - this.offset;
			float num;
			if (useBlendMap)
			{
				num = blenddata.GetInterpolatedVector((vector2.x + this.extents.x) / this.size.x, (vector2.z + this.extents.z) / this.size.z).w;
			}
			else
			{
				float num2 = Noise.Billow(vector.x, vector.z, 4, 0.005f, 0.25f * this.Fade, 2f, 0.5f);
				num = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade + num2, Vector3Ex.Magnitude2D(vector2));
			}
			if (num == 0f)
			{
				return;
			}
			float num3 = TerrainMeta.NormalizeY(position.y + this.offset.y + heightdata.GetInterpolatedHalf((vector2.x + this.extents.x) / this.size.x, (vector2.z + this.extents.z) / this.size.z) * this.size.y);
			num3 = Mathf.SmoothStep(TerrainMeta.HeightMap.GetHeight01(x, z), num3, num);
			TerrainMeta.HeightMap.SetHeight(x, z, num3);
		});
	}

	// Token: 0x06001E3D RID: 7741 RVA: 0x000A5D7C File Offset: 0x000A3F7C
	protected override void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool should0 = base.ShouldSplat(1);
		bool should1 = base.ShouldSplat(2);
		bool should2 = base.ShouldSplat(4);
		bool should3 = base.ShouldSplat(8);
		bool should4 = base.ShouldSplat(16);
		bool should5 = base.ShouldSplat(32);
		bool should6 = base.ShouldSplat(64);
		bool should7 = base.ShouldSplat(128);
		if (!should0 && !should1 && !should2 && !should3 && !should4 && !should5 && !should6 && !should7)
		{
			return;
		}
		TextureData splat0data = new TextureData(this.splatmap0);
		TextureData splat1data = new TextureData(this.splatmap1);
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.SplatMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			GenerateCliffSplat.Process(x, z);
			float normZ = TerrainMeta.SplatMap.Coordinate(z);
			float normX = TerrainMeta.SplatMap.Coordinate(x);
			Vector3 vector = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector2 = worldToLocal.MultiplyPoint3x4(vector) - this.offset;
			float num = Noise.Billow(vector.x, vector.z, 4, 0.005f, 0.25f * this.Fade, 2f, 0.5f);
			float num2 = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade + num, Vector3Ex.Magnitude2D(vector2));
			if (num2 == 0f)
			{
				return;
			}
			Vector4 interpolatedVector = splat0data.GetInterpolatedVector((vector2.x + this.extents.x) / this.size.x, (vector2.z + this.extents.z) / this.size.z);
			Vector4 interpolatedVector2 = splat1data.GetInterpolatedVector((vector2.x + this.extents.x) / this.size.x, (vector2.z + this.extents.z) / this.size.z);
			if (!should0)
			{
				interpolatedVector.x = 0f;
			}
			if (!should1)
			{
				interpolatedVector.y = 0f;
			}
			if (!should2)
			{
				interpolatedVector.z = 0f;
			}
			if (!should3)
			{
				interpolatedVector.w = 0f;
			}
			if (!should4)
			{
				interpolatedVector2.x = 0f;
			}
			if (!should5)
			{
				interpolatedVector2.y = 0f;
			}
			if (!should6)
			{
				interpolatedVector2.z = 0f;
			}
			if (!should7)
			{
				interpolatedVector2.w = 0f;
			}
			TerrainMeta.SplatMap.SetSplatRaw(x, z, interpolatedVector, interpolatedVector2, num2);
		});
	}

	// Token: 0x06001E3E RID: 7742 RVA: 0x000A5F54 File Offset: 0x000A4154
	protected override void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		TextureData alphadata = new TextureData(this.alphamap);
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.AlphaMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			float normZ = TerrainMeta.AlphaMap.Coordinate(z);
			float normX = TerrainMeta.AlphaMap.Coordinate(x);
			Vector3 vector = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector2 = worldToLocal.MultiplyPoint3x4(vector) - this.offset;
			float num = Noise.Billow(vector.x, vector.z, 4, 0.005f, 0.25f * this.Fade, 2f, 0.5f);
			float num2 = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade + num, Vector3Ex.Magnitude2D(vector2));
			if (num2 == 0f)
			{
				return;
			}
			float w = alphadata.GetInterpolatedVector((vector2.x + this.extents.x) / this.size.x, (vector2.z + this.extents.z) / this.size.z).w;
			TerrainMeta.AlphaMap.SetAlpha(x, z, w, num2);
		});
	}

	// Token: 0x06001E3F RID: 7743 RVA: 0x000A6068 File Offset: 0x000A4268
	protected override void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool should0 = base.ShouldBiome(1);
		bool should1 = base.ShouldBiome(2);
		bool should2 = base.ShouldBiome(4);
		bool should3 = base.ShouldBiome(8);
		if (!should0 && !should1 && !should2 && !should3)
		{
			return;
		}
		TextureData biomedata = new TextureData(this.biomemap);
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.BiomeMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			float normZ = TerrainMeta.BiomeMap.Coordinate(z);
			float normX = TerrainMeta.BiomeMap.Coordinate(x);
			Vector3 vector = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector2 = worldToLocal.MultiplyPoint3x4(vector) - this.offset;
			float num = Noise.Billow(vector.x, vector.z, 4, 0.005f, 0.25f * this.Fade, 2f, 0.5f);
			float num2 = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade + num, Vector3Ex.Magnitude2D(vector2));
			if (num2 == 0f)
			{
				return;
			}
			Vector4 interpolatedVector = biomedata.GetInterpolatedVector((vector2.x + this.extents.x) / this.size.x, (vector2.z + this.extents.z) / this.size.z);
			if (!should0)
			{
				interpolatedVector.x = 0f;
			}
			if (!should1)
			{
				interpolatedVector.y = 0f;
			}
			if (!should2)
			{
				interpolatedVector.z = 0f;
			}
			if (!should3)
			{
				interpolatedVector.w = 0f;
			}
			TerrainMeta.BiomeMap.SetBiomeRaw(x, z, interpolatedVector, num2);
		});
	}

	// Token: 0x06001E40 RID: 7744 RVA: 0x000A61D4 File Offset: 0x000A43D4
	protected override void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		TextureData topologydata = new TextureData(this.topologymap);
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.TopologyMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			GenerateCliffTopology.Process(x, z);
			float normZ = TerrainMeta.TopologyMap.Coordinate(z);
			float normX = TerrainMeta.TopologyMap.Coordinate(x);
			Vector3 point = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector = worldToLocal.MultiplyPoint3x4(point) - this.offset;
			int interpolatedInt = topologydata.GetInterpolatedInt((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z);
			if (this.ShouldTopology(interpolatedInt))
			{
				TerrainMeta.TopologyMap.AddTopology(x, z, interpolatedInt & this.TopologyMask);
			}
		});
	}

	// Token: 0x06001E41 RID: 7745 RVA: 0x00002ECE File Offset: 0x000010CE
	protected override void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}
}
