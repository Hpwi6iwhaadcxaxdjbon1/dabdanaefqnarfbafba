using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200049A RID: 1178
[CreateAssetMenu(menuName = "Rust/Terrain Config")]
public class TerrainConfig : ScriptableObject
{
	// Token: 0x0400184A RID: 6218
	public bool CastShadows = true;

	// Token: 0x0400184B RID: 6219
	public LayerMask GroundMask = 0;

	// Token: 0x0400184C RID: 6220
	public LayerMask WaterMask = 0;

	// Token: 0x0400184D RID: 6221
	public PhysicMaterial GenericMaterial;

	// Token: 0x0400184E RID: 6222
	public Material Material;

	// Token: 0x0400184F RID: 6223
	public Texture[] AlbedoArrays = new Texture[3];

	// Token: 0x04001850 RID: 6224
	public Texture[] NormalArrays = new Texture[3];

	// Token: 0x04001851 RID: 6225
	public float HeightMapErrorMin = 5f;

	// Token: 0x04001852 RID: 6226
	public float HeightMapErrorMax = 100f;

	// Token: 0x04001853 RID: 6227
	public float BaseMapDistanceMin = 100f;

	// Token: 0x04001854 RID: 6228
	public float BaseMapDistanceMax = 500f;

	// Token: 0x04001855 RID: 6229
	public float ShaderLodMin = 100f;

	// Token: 0x04001856 RID: 6230
	public float ShaderLodMax = 600f;

	// Token: 0x04001857 RID: 6231
	public TerrainConfig.SplatType[] Splats = new TerrainConfig.SplatType[8];

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06001B51 RID: 6993 RVA: 0x00016778 File Offset: 0x00014978
	public Texture AlbedoArray
	{
		get
		{
			return this.AlbedoArrays[Mathf.Clamp(QualitySettings.masterTextureLimit, 0, 2)];
		}
	}

	// Token: 0x17000181 RID: 385
	// (get) Token: 0x06001B52 RID: 6994 RVA: 0x0001678D File Offset: 0x0001498D
	public Texture NormalArray
	{
		get
		{
			return this.NormalArrays[Mathf.Clamp(QualitySettings.masterTextureLimit, 0, 2)];
		}
	}

	// Token: 0x06001B53 RID: 6995 RVA: 0x00097B64 File Offset: 0x00095D64
	public PhysicMaterial[] GetPhysicMaterials()
	{
		PhysicMaterial[] array = new PhysicMaterial[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].Material;
		}
		return array;
	}

	// Token: 0x06001B54 RID: 6996 RVA: 0x00097BA4 File Offset: 0x00095DA4
	public Color[] GetAridColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].AridColor;
		}
		return array;
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x00097BE8 File Offset: 0x00095DE8
	public Color[] GetTemperateColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].TemperateColor;
		}
		return array;
	}

	// Token: 0x06001B56 RID: 6998 RVA: 0x00097C2C File Offset: 0x00095E2C
	public Color[] GetTundraColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].TundraColor;
		}
		return array;
	}

	// Token: 0x06001B57 RID: 6999 RVA: 0x00097C70 File Offset: 0x00095E70
	public Color[] GetArcticColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].ArcticColor;
		}
		return array;
	}

	// Token: 0x06001B58 RID: 7000 RVA: 0x00097CB4 File Offset: 0x00095EB4
	public float[] GetSplatTiling()
	{
		float[] array = new float[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].SplatTiling;
		}
		return array;
	}

	// Token: 0x06001B59 RID: 7001 RVA: 0x00097CF4 File Offset: 0x00095EF4
	public float GetMaxSplatTiling()
	{
		float num = float.MinValue;
		for (int i = 0; i < this.Splats.Length; i++)
		{
			if (this.Splats[i].SplatTiling > num)
			{
				num = this.Splats[i].SplatTiling;
			}
		}
		return num;
	}

	// Token: 0x06001B5A RID: 7002 RVA: 0x00097D3C File Offset: 0x00095F3C
	public float GetMinSplatTiling()
	{
		float num = float.MaxValue;
		for (int i = 0; i < this.Splats.Length; i++)
		{
			if (this.Splats[i].SplatTiling < num)
			{
				num = this.Splats[i].SplatTiling;
			}
		}
		return num;
	}

	// Token: 0x06001B5B RID: 7003 RVA: 0x00097D84 File Offset: 0x00095F84
	public Vector3[] GetPackedUVMIX()
	{
		Vector3[] array = new Vector3[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = new Vector3(this.Splats[i].UVMIXMult, this.Splats[i].UVMIXStart, this.Splats[i].UVMIXDist);
		}
		return array;
	}

	// Token: 0x0200049B RID: 1179
	[Serializable]
	public class SplatType
	{
		// Token: 0x04001858 RID: 6232
		public string Name = "";

		// Token: 0x04001859 RID: 6233
		[FormerlySerializedAs("WarmColor")]
		public Color AridColor = Color.white;

		// Token: 0x0400185A RID: 6234
		[FormerlySerializedAs("Color")]
		public Color TemperateColor = Color.white;

		// Token: 0x0400185B RID: 6235
		[FormerlySerializedAs("ColdColor")]
		public Color TundraColor = Color.white;

		// Token: 0x0400185C RID: 6236
		[FormerlySerializedAs("ColdColor")]
		public Color ArcticColor = Color.white;

		// Token: 0x0400185D RID: 6237
		public PhysicMaterial Material;

		// Token: 0x0400185E RID: 6238
		public float SplatTiling = 5f;

		// Token: 0x0400185F RID: 6239
		[Range(0f, 1f)]
		public float UVMIXMult = 0.15f;

		// Token: 0x04001860 RID: 6240
		public float UVMIXStart;

		// Token: 0x04001861 RID: 6241
		public float UVMIXDist = 100f;
	}
}
