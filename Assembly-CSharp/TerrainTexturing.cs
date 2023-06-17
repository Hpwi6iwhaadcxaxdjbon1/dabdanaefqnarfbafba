using System;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020004F1 RID: 1265
[ExecuteInEditMode]
public class TerrainTexturing : TerrainExtension
{
	// Token: 0x040019C1 RID: 6593
	private const int coarseHeightDownscale = 1;

	// Token: 0x040019C2 RID: 6594
	public bool debugFoliageDisplacement;

	// Token: 0x040019C3 RID: 6595
	private const int CoarseSlopeBlurPasses = 4;

	// Token: 0x040019C4 RID: 6596
	private const float CoarseSlopeBlurRadius = 1f;

	// Token: 0x040019C5 RID: 6597
	private bool initialized;

	// Token: 0x040019C6 RID: 6598
	private TextureCacheState pyramidCacheState = TextureCacheState.Initializing;

	// Token: 0x040019C7 RID: 6599
	private RenderTexture diffuseBasePyramid;

	// Token: 0x040019C8 RID: 6600
	private RenderTexture normalBasePyramid;

	// Token: 0x040019C9 RID: 6601
	private const int MaxBasePyramidSize = 4096;

	// Token: 0x040019CA RID: 6602
	private TextureCacheState coarseHeightSlopeCacheState = TextureCacheState.Initializing;

	// Token: 0x040019CB RID: 6603
	private RenderTexture coarseHeightSlopeMap;

	// Token: 0x040019CC RID: 6604
	private int prevCoarseHeightDownscale;

	// Token: 0x040019CD RID: 6605
	private bool prevDebugFoliageDisplacement;

	// Token: 0x040019CE RID: 6606
	private int prevQuality = -1;

	// Token: 0x040019CF RID: 6607
	private bool triggerUpdateGlobalParams;

	// Token: 0x040019D0 RID: 6608
	private string[,] layerShaderPropNames;

	// Token: 0x040019D1 RID: 6609
	private static TerrainTexturing instance;

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06001D63 RID: 7523 RVA: 0x00017961 File Offset: 0x00015B61
	public RenderTexture DiffuseBasePyramid
	{
		get
		{
			return this.diffuseBasePyramid;
		}
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06001D64 RID: 7524 RVA: 0x00017969 File Offset: 0x00015B69
	public RenderTexture NormalBasePyramid
	{
		get
		{
			return this.normalBasePyramid;
		}
	}

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06001D65 RID: 7525 RVA: 0x00017971 File Offset: 0x00015B71
	public RenderTexture CoarseHeightSlopeMap
	{
		get
		{
			return this.coarseHeightSlopeMap;
		}
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06001D66 RID: 7526 RVA: 0x00017979 File Offset: 0x00015B79
	public static TerrainTexturing Instance
	{
		get
		{
			return TerrainTexturing.instance;
		}
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x00017980 File Offset: 0x00015B80
	private void CheckInstance()
	{
		TerrainTexturing.instance = ((TerrainTexturing.instance != null) ? TerrainTexturing.instance : this);
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x0001799C File Offset: 0x00015B9C
	private void Awake()
	{
		this.CheckInstance();
	}

	// Token: 0x06001D69 RID: 7529 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void Setup()
	{
	}

	// Token: 0x06001D6A RID: 7530 RVA: 0x000A03D8 File Offset: 0x0009E5D8
	public override void PostSetup()
	{
		TerrainMeta component = base.GetComponent<TerrainMeta>();
		if (component == null || component.config == null)
		{
			Debug.LogError("[TerrainTexturing] Missing TerrainMeta or TerrainConfig not assigned.");
			return;
		}
		this.Shutdown();
		if (this.pyramidCacheState != TextureCacheState.Skipped)
		{
			this.InitializeBasePyramid();
		}
		this.InitializeCoarseHeightSlopeMap();
		this.UpdateGlobalShaderConstants();
		this.UpdateTerrainToGlobal();
		if (this.terrain.reflectionProbeUsage == ReflectionProbeUsage.Off)
		{
			this.terrain.reflectionProbeUsage = ReflectionProbeUsage.Simple;
		}
		this.prevCoarseHeightDownscale = 1;
		this.prevDebugFoliageDisplacement = this.debugFoliageDisplacement;
		this.prevQuality = ConVar.Graphics.quality;
		this.initialized = true;
	}

	// Token: 0x06001D6B RID: 7531 RVA: 0x0001799C File Offset: 0x00015B9C
	private void OnEnable()
	{
		this.CheckInstance();
	}

	// Token: 0x06001D6C RID: 7532 RVA: 0x000179A4 File Offset: 0x00015BA4
	private void Shutdown()
	{
		this.ReleaseBasePyramid();
		this.ReleaseCoarseHeightSlopeMap();
		this.initialized = false;
	}

	// Token: 0x06001D6D RID: 7533 RVA: 0x000179B9 File Offset: 0x00015BB9
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.Shutdown();
	}

	// Token: 0x06001D6E RID: 7534 RVA: 0x000179C9 File Offset: 0x00015BC9
	private Material CreateMaterial(string name)
	{
		return new Material(Shader.Find(name))
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x06001D6F RID: 7535 RVA: 0x000179DE File Offset: 0x00015BDE
	private void DestroyMaterial(ref Material mat)
	{
		if (mat != null)
		{
			Object.DestroyImmediate(mat);
			mat = null;
		}
	}

	// Token: 0x06001D70 RID: 7536 RVA: 0x000179F4 File Offset: 0x00015BF4
	private void DestroyRT(ref RenderTexture rtex)
	{
		if (rtex != null)
		{
			RenderTexture.active = null;
			Object.DestroyImmediate(rtex);
			rtex = null;
		}
	}

	// Token: 0x06001D71 RID: 7537 RVA: 0x00017A10 File Offset: 0x00015C10
	private Texture2D CreateTex(string name, int width, int height, TextureFormat format, bool mips, bool linear)
	{
		return new Texture2D(width, height, format, mips, linear)
		{
			hideFlags = HideFlags.DontSave,
			name = name,
			anisoLevel = 1,
			filterMode = FilterMode.Trilinear,
			wrapMode = TextureWrapMode.Clamp
		};
	}

	// Token: 0x06001D72 RID: 7538 RVA: 0x000A0474 File Offset: 0x0009E674
	private RenderTexture CreateBasePyramidRT(string name, int size, bool linear)
	{
		RenderTextureReadWrite readWrite = linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB;
		RenderTexture renderTexture = new RenderTexture(size, size, 0, RenderTextureFormat.ARGB32, readWrite);
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.name = name;
		renderTexture.anisoLevel = 4;
		renderTexture.filterMode = FilterMode.Trilinear;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.isPowerOfTwo = true;
		renderTexture.dimension = TextureDimension.Tex2D;
		renderTexture.useMipMap = true;
		renderTexture.autoGenerateMips = true;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x06001D73 RID: 7539 RVA: 0x000179DE File Offset: 0x00015BDE
	private void DestroyTex(ref Texture2D tex)
	{
		if (tex != null)
		{
			Object.DestroyImmediate(tex);
			tex = null;
		}
	}

	// Token: 0x06001D74 RID: 7540 RVA: 0x000A04DC File Offset: 0x0009E6DC
	private void WarmupStrings()
	{
		if (this.layerShaderPropNames == null)
		{
			this.layerShaderPropNames = new string[9, 8];
			for (int i = 0; i < 8; i++)
			{
				this.layerShaderPropNames[0, i] = "Splat" + i + "_UVMIX";
				this.layerShaderPropNames[1, i] = "Splat" + i + "_AridColor";
				this.layerShaderPropNames[2, i] = "Splat" + i + "_TemperateColor";
				this.layerShaderPropNames[3, i] = "Splat" + i + "_TundraColor";
				this.layerShaderPropNames[4, i] = "Splat" + i + "_ArcticColor";
				this.layerShaderPropNames[5, i] = "_Layer" + i + "_Specularity";
				this.layerShaderPropNames[6, i] = "_Layer" + i + "_Smoothness";
				this.layerShaderPropNames[7, i] = "_Layer" + i + "_Factor";
				this.layerShaderPropNames[8, i] = "_Layer" + i + "_Falloff";
			}
		}
	}

	// Token: 0x06001D75 RID: 7541 RVA: 0x00017A43 File Offset: 0x00015C43
	private void InitializeBasePyramid()
	{
		this.diffuseBasePyramid = this.CreateBasePyramidRT("Terrain-DiffuseBase", 4096, false);
		this.normalBasePyramid = this.CreateBasePyramidRT("Terrain-NormalBase", 4096, false);
		this.pyramidCacheState = TextureCacheState.Uncached;
	}

	// Token: 0x06001D76 RID: 7542 RVA: 0x000A0648 File Offset: 0x0009E848
	private void InitializeCoarseHeightSlopeMap()
	{
		int num = Mathf.ClosestPowerOfTwo(this.terrain.terrainData.heightmapResolution) >> 1;
		this.prevCoarseHeightDownscale = 1;
		this.coarseHeightSlopeMap = new RenderTexture(num, num, 0, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear);
		this.coarseHeightSlopeMap.name = "Terrain_HeightSlope";
		this.coarseHeightSlopeMap.filterMode = FilterMode.Trilinear;
		this.coarseHeightSlopeMap.wrapMode = TextureWrapMode.Clamp;
		this.coarseHeightSlopeMap.useMipMap = true;
		this.coarseHeightSlopeMap.autoGenerateMips = true;
		this.coarseHeightSlopeMap.Create();
		this.coarseHeightSlopeCacheState = TextureCacheState.Uncached;
	}

	// Token: 0x06001D77 RID: 7543 RVA: 0x00017A7A File Offset: 0x00015C7A
	private void ReleaseBasePyramid()
	{
		RenderTexture.active = null;
		this.DestroyRT(ref this.diffuseBasePyramid);
		this.DestroyRT(ref this.normalBasePyramid);
	}

	// Token: 0x06001D78 RID: 7544 RVA: 0x00017A9A File Offset: 0x00015C9A
	private void ReleaseCoarseHeightSlopeMap()
	{
		this.DestroyRT(ref this.coarseHeightSlopeMap);
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x000A06D8 File Offset: 0x0009E8D8
	private void CacheBasePyramid()
	{
		Material material = this.CreateMaterial("Hidden/Terrain/RenderBase");
		this.UpdateGlobalShaderConstants();
		this.UpdateTerrainToGlobal();
		float num = 0.00024414062f;
		material.SetVector("_viewport", new Vector4(1f, -1f, 0f, 1f));
		material.SetVector("_offsets", new Vector4(-0.5f * num, 0.5f * num, 0f, 0f));
		GL.sRGBWrite = true;
		UnityEngine.Graphics.Blit(null, this.diffuseBasePyramid, material, 0);
		GL.sRGBWrite = false;
		UnityEngine.Graphics.Blit(null, this.normalBasePyramid, material, 1);
		this.DestroyMaterial(ref material);
		this.pyramidCacheState = TextureCacheState.CachedRaw;
	}

	// Token: 0x06001D7A RID: 7546 RVA: 0x000A0788 File Offset: 0x0009E988
	private void CacheCoarseHeightSlopeMap()
	{
		TerrainHeightMap component = base.GetComponent<TerrainHeightMap>();
		if (component == null)
		{
			return;
		}
		int srcsize = component.HeightTexture.width;
		int dstsize = this.coarseHeightSlopeMap.width;
		int num = Mathf.ClosestPowerOfTwo(srcsize);
		Color[] pixels = new Color[dstsize * dstsize];
		Texture2D texture2D = this.CreateTex("Terrain_HeightSlope", dstsize, dstsize, TextureFormat.RGFloat, false, true);
		int block = num / dstsize;
		float num2 = 1f / (float)(block * block);
		float hoffset = this.terrain.transform.position.y;
		float hscale = this.terrain.terrainData.size.y * num2 * 0.25f * BitUtility.Short2Float(1);
		float hscale_s = num2 * BitUtility.Short2Float(1) * 0.375f;
		float ny = TerrainMeta.Size.x / TerrainMeta.Size.y / (float)srcsize;
		if (Application.isEditor && !Application.isPlaying)
		{
			TextureData textureData = new TextureData(component.HeightTexture);
			Color32[] heightColors = textureData.colors;
			Parallel.For(0, dstsize, delegate(int dz)
			{
				int num4 = dz * block;
				int num5 = dz * dstsize;
				int j = 0;
				int num6 = 0;
				while (j < dstsize)
				{
					float num7 = 0f;
					float num8 = 0f;
					float num9 = 0f;
					float num10 = 0f;
					int num11 = num4 * srcsize + num6;
					int num12 = num11 + 1;
					int num13 = num11 + srcsize;
					int num14 = num12 + srcsize;
					for (int k = 0; k < block; k++)
					{
						for (int l = 0; l < block; l++)
						{
							Color32 color = heightColors[num11 + l];
							Color32 color2 = heightColors[num12 + l];
							Color32 color3 = heightColors[num13 + l];
							Color32 color4 = heightColors[num14 + l];
							num7 += (float)((int)color.b << 8 | (int)color.r);
							num8 += (float)((int)color2.b << 8 | (int)color2.r);
							num9 += (float)((int)color3.b << 8 | (int)color3.r);
							num10 += (float)((int)color4.b << 8 | (int)color4.r);
						}
						num11 += srcsize;
						num12 += srcsize;
						num13 += srcsize;
						num14 += srcsize;
					}
					float x = (num8 + num10 - num7 - num9) * hscale_s;
					float z = (num9 + num10 - num7 - num8) * hscale_s;
					Vector3 normalized = new Vector3(x, ny, z).normalized;
					float r = (num7 + num8 + num9 + num10) * hscale + hoffset;
					float g = normalized.x * normalized.x + normalized.z * normalized.z;
					pixels[num5 + j] = new Color(r, g, 0f, 0f);
					j++;
					num6 += block;
				}
			});
		}
		else
		{
			int heightres = component.res;
			short[] heights = component.src;
			Parallel.For(0, dstsize, delegate(int dz)
			{
				int num4 = dz * block;
				int num5 = dz * dstsize;
				int j = 0;
				int num6 = 0;
				while (j < dstsize)
				{
					float num7 = 0f;
					float num8 = 0f;
					float num9 = 0f;
					float num10 = 0f;
					int num11 = num6;
					int num12 = num4;
					int num13 = num6 + 1;
					int num14 = num4 + 1;
					for (int k = 0; k < block; k++)
					{
						for (int l = 0; l < block; l++)
						{
							num7 += (float)heights[(num12 + k) * heightres + (num11 + l)];
							num8 += (float)heights[(num12 + k) * heightres + (num13 + l)];
							num9 += (float)heights[(num14 + k) * heightres + (num11 + l)];
							num10 += (float)heights[(num14 + k) * heightres + (num13 + l)];
						}
					}
					float x = (num8 + num10 - num7 - num9) * hscale_s;
					float z = (num9 + num10 - num7 - num8) * hscale_s;
					Vector3 normalized = new Vector3(x, ny, z).normalized;
					float r = (num7 + num8 + num9 + num10) * hscale + hoffset;
					float g = normalized.x * normalized.x + normalized.z * normalized.z;
					pixels[num5 + j] = new Color(r, g, 0f, 0f);
					j++;
					num6 += block;
				}
			});
		}
		texture2D.SetPixels(pixels);
		texture2D.Apply(false);
		UnityEngine.Graphics.Blit(texture2D, this.coarseHeightSlopeMap);
		RenderTexture temporary = RenderTexture.GetTemporary(dstsize, dstsize, 0, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear);
		RenderTexture temporary2 = RenderTexture.GetTemporary(dstsize, dstsize, 0, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear);
		Material material = new Material(Shader.Find("Hidden/Rust/SeparableBlur"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		UnityEngine.Graphics.Blit(texture2D, temporary);
		float num3 = 1f / (float)dstsize;
		for (int i = 0; i < 4; i++)
		{
			material.SetVector("offsets", new Vector4(num3, 0f, 0f, 0f));
			UnityEngine.Graphics.Blit(temporary, temporary2, material, 2);
			material.SetVector("offsets", new Vector4(0f, num3, 0f, 0f));
			UnityEngine.Graphics.Blit(temporary2, temporary, material, 2);
		}
		UnityEngine.Graphics.Blit(temporary, this.coarseHeightSlopeMap);
		Object.DestroyImmediate(material);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		this.DestroyTex(ref texture2D);
		this.coarseHeightSlopeCacheState = TextureCacheState.CachedRaw;
	}

	// Token: 0x06001D7B RID: 7547 RVA: 0x00017AA8 File Offset: 0x00015CA8
	private void UpdateBasePyramidShaderConstants()
	{
		Shader.SetGlobalTexture("Terrain_DiffuseBasePyramid", this.diffuseBasePyramid);
		Shader.SetGlobalTexture("Terrain_NormalBasePyramid", this.normalBasePyramid);
	}

	// Token: 0x06001D7C RID: 7548 RVA: 0x000A0A6C File Offset: 0x0009EC6C
	private void UpdateTerrainShaderConstants()
	{
		if (Application.isPlaying)
		{
			TerrainSplatMap component = base.GetComponent<TerrainSplatMap>();
			Shader.SetGlobalTexture("Terrain_Control0", component.SplatTexture0);
			Shader.SetGlobalTexture("Terrain_Control1", component.SplatTexture1);
		}
		Shader.SetGlobalTexture("Terrain_AlbedoArray", this.config.AlbedoArray);
		Shader.SetGlobalTexture("Terrain_NormalArray", this.config.NormalArray);
		Shader.SetGlobalTexture("Terrain_HeightSlope", this.coarseHeightSlopeMap);
		Vector3 position = this.terrain.GetPosition();
		Vector3 size = this.terrain.terrainData.size;
		Shader.SetGlobalVector("Terrain_Position", position);
		Shader.SetGlobalVector("Terrain_Size", size);
		Shader.SetGlobalVector("Terrain_RcpSize", Vector3Ex.Inverse(size));
		Vector2 v = new Vector2(1f / this.config.GetMinSplatTiling(), 1f / this.config.GetMinSplatTiling());
		float[] splatTiling = this.config.GetSplatTiling();
		Vector3[] packedUVMIX = this.config.GetPackedUVMIX();
		Vector4 value = new Vector4(1f / splatTiling[0], 1f / splatTiling[1], 1f / splatTiling[2], 1f / splatTiling[3]);
		Vector4 value2 = new Vector4(1f / splatTiling[4], 1f / splatTiling[5], 1f / splatTiling[6], 1f / splatTiling[7]);
		Shader.SetGlobalVector("Terrain_TexelSize", v);
		Shader.SetGlobalVector("Terrain_TexelSize0", value);
		Shader.SetGlobalVector("Terrain_TexelSize1", value2);
		Color[] aridColors = this.config.GetAridColors();
		Color[] temperateColors = this.config.GetTemperateColors();
		Color[] tundraColors = this.config.GetTundraColors();
		Color[] arcticColors = this.config.GetArcticColors();
		float num = packedUVMIX[0].x;
		float num2 = packedUVMIX[0].y;
		float num3 = packedUVMIX[0].z;
		this.WarmupStrings();
		for (int i = 0; i < 8; i++)
		{
			Shader.SetGlobalVector(this.layerShaderPropNames[0, i], new Vector3(packedUVMIX[i].x, packedUVMIX[i].y, 1f / packedUVMIX[i].z));
			Shader.SetGlobalColor(this.layerShaderPropNames[1, i], aridColors[i].linear);
			Shader.SetGlobalColor(this.layerShaderPropNames[2, i], temperateColors[i].linear);
			Shader.SetGlobalColor(this.layerShaderPropNames[3, i], tundraColors[i].linear);
			Shader.SetGlobalColor(this.layerShaderPropNames[4, i], arcticColors[i].linear);
			num = Mathf.Max(num, packedUVMIX[i].x);
			num2 = Mathf.Min(num2, packedUVMIX[i].y);
			num3 = Mathf.Max(num3, packedUVMIX[i].z);
		}
		Shader.SetGlobalFloat("_UVMIXMult", num);
		Shader.SetGlobalFloat("_UVMIXStart", num2);
		Shader.SetGlobalFloat("_UVMIXDist", num3);
	}

	// Token: 0x06001D7D RID: 7549 RVA: 0x00017ACA File Offset: 0x00015CCA
	private void UpdateFoliageDisplaceShaderConstants()
	{
		KeywordUtil.EnsureKeywordState("_FOLIAGEDISPLACE_DEBUG", this.debugFoliageDisplacement);
	}

	// Token: 0x06001D7E RID: 7550 RVA: 0x00017ADC File Offset: 0x00015CDC
	private void UpdateGlobalShaderConstants()
	{
		this.UpdateBasePyramidShaderConstants();
		this.UpdateTerrainShaderConstants();
		this.UpdateFoliageDisplaceShaderConstants();
	}

	// Token: 0x06001D7F RID: 7551 RVA: 0x000A0DB0 File Offset: 0x0009EFB0
	private void UpdateTerrainToGlobal()
	{
		Material material = this.config.Material;
		Shader.SetGlobalFloat("_TerrainParallax", material.GetFloat("_TerrainParallax"));
		Shader.SetGlobalTexture("Terrain_PotatoDetailTexture", material.GetTexture("_PotatoDetailTexture"));
		Shader.SetGlobalFloat("Terrain_PotatoDetailWorldUVScale", material.GetFloat("_PotatoDetailWorldUVScale"));
		for (int i = 0; i < 8; i++)
		{
			string name = this.layerShaderPropNames[5, i];
			string name2 = this.layerShaderPropNames[6, i];
			string name3 = this.layerShaderPropNames[7, i];
			string name4 = this.layerShaderPropNames[8, i];
			Shader.SetGlobalFloat(name, material.GetFloat(name));
			Shader.SetGlobalFloat(name2, material.GetFloat(name2));
			Shader.SetGlobalFloat(name3, material.GetFloat(name3));
			Shader.SetGlobalFloat(name4, material.GetFloat(name4));
		}
		Shader.SetGlobalFloat("Terrain_ShoreWetnessLayer_Range", material.GetFloat("_ShoreWetnessLayer_Range"));
		Shader.SetGlobalFloat("Terrain_ShoreWetnessLayer_BlendFactor", material.GetFloat("_ShoreWetnessLayer_BlendFactor"));
		Shader.SetGlobalFloat("Terrain_ShoreWetnessLayer_BlendFalloff", material.GetFloat("_ShoreWetnessLayer_BlendFalloff"));
		Shader.SetGlobalFloat("Terrain_ShoreWetnessLayer_WetAlbedoScale", material.GetFloat("_ShoreWetnessLayer_WetAlbedoScale"));
		Shader.SetGlobalFloat("Terrain_ShoreWetnessLayer_WetSmoothness", material.GetFloat("_ShoreWetnessLayer_WetSmoothness"));
	}

	// Token: 0x06001D80 RID: 7552 RVA: 0x00017AF0 File Offset: 0x00015CF0
	private bool CheckLostPyramidData()
	{
		return (this.diffuseBasePyramid != null && !this.diffuseBasePyramid.IsCreated()) || (this.normalBasePyramid != null && !this.normalBasePyramid.IsCreated());
	}

	// Token: 0x06001D81 RID: 7553 RVA: 0x00017B2D File Offset: 0x00015D2D
	private bool CheckRebuildCoarseHeightSlopeData()
	{
		return this.prevCoarseHeightDownscale != 1 || (this.coarseHeightSlopeMap != null && !this.coarseHeightSlopeMap.IsCreated());
	}

	// Token: 0x06001D82 RID: 7554 RVA: 0x00017B58 File Offset: 0x00015D58
	public void TriggerUpdateGlobalParams()
	{
		this.triggerUpdateGlobalParams = true;
	}

	// Token: 0x06001D83 RID: 7555 RVA: 0x000A0EF0 File Offset: 0x0009F0F0
	private void Update()
	{
		if (!this.initialized)
		{
			return;
		}
		bool flag = this.prevQuality != ConVar.Graphics.quality;
		if (this.triggerUpdateGlobalParams || flag || (Application.isEditor && !Application.isPlaying))
		{
			this.UpdateGlobalShaderConstants();
			this.UpdateTerrainToGlobal();
			this.triggerUpdateGlobalParams = false;
			this.prevQuality = ConVar.Graphics.quality;
		}
		if (this.pyramidCacheState != TextureCacheState.Skipped && this.pyramidCacheState == TextureCacheState.Uncached)
		{
			this.CacheBasePyramid();
		}
		if (this.prevDebugFoliageDisplacement != this.debugFoliageDisplacement)
		{
			this.prevDebugFoliageDisplacement = this.debugFoliageDisplacement;
			this.triggerUpdateGlobalParams = true;
		}
		if (this.coarseHeightSlopeCacheState == TextureCacheState.CachedRaw && this.CheckRebuildCoarseHeightSlopeData())
		{
			Debug.Log("[TerrainTexturing] Lost data detected. Rebuilding coarse height data.");
			this.ReleaseCoarseHeightSlopeMap();
			this.InitializeCoarseHeightSlopeMap();
			this.triggerUpdateGlobalParams = true;
		}
		if (this.coarseHeightSlopeCacheState == TextureCacheState.Uncached)
		{
			this.CacheCoarseHeightSlopeMap();
		}
		if (this.pyramidCacheState == TextureCacheState.CachedRaw && this.CheckLostPyramidData())
		{
			Debug.Log("[TerrainTexturing] Lost data detected. Rebuilding base pyramid.");
			this.ReleaseBasePyramid();
			this.InitializeBasePyramid();
			this.triggerUpdateGlobalParams = true;
		}
	}

	// Token: 0x020004F2 RID: 1266
	private enum LayerShaderProp
	{
		// Token: 0x040019D3 RID: 6611
		UVMIX,
		// Token: 0x040019D4 RID: 6612
		AridColor,
		// Token: 0x040019D5 RID: 6613
		TemperateColor,
		// Token: 0x040019D6 RID: 6614
		TundraColor,
		// Token: 0x040019D7 RID: 6615
		ArcticColor,
		// Token: 0x040019D8 RID: 6616
		Specularity,
		// Token: 0x040019D9 RID: 6617
		Smoothness,
		// Token: 0x040019DA RID: 6618
		Factor,
		// Token: 0x040019DB RID: 6619
		Falloff,
		// Token: 0x040019DC RID: 6620
		COUNT
	}
}
