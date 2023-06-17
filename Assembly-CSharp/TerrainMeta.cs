using System;
using UnityEngine;

// Token: 0x020004E8 RID: 1256
[ExecuteInEditMode]
public class TerrainMeta : MonoBehaviour
{
	// Token: 0x04001962 RID: 6498
	public Terrain terrain;

	// Token: 0x04001963 RID: 6499
	public TerrainConfig config;

	// Token: 0x04001964 RID: 6500
	public TerrainMeta.PaintMode paint;

	// Token: 0x04001965 RID: 6501
	[HideInInspector]
	public TerrainMeta.PaintMode currentPaintMode;

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x06001D10 RID: 7440 RVA: 0x00017631 File Offset: 0x00015831
	// (set) Token: 0x06001D11 RID: 7441 RVA: 0x00017638 File Offset: 0x00015838
	public static TerrainConfig Config { get; private set; }

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x06001D12 RID: 7442 RVA: 0x00017640 File Offset: 0x00015840
	// (set) Token: 0x06001D13 RID: 7443 RVA: 0x00017647 File Offset: 0x00015847
	public static Terrain Terrain { get; private set; }

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x06001D14 RID: 7444 RVA: 0x0001764F File Offset: 0x0001584F
	// (set) Token: 0x06001D15 RID: 7445 RVA: 0x00017656 File Offset: 0x00015856
	public static Transform Transform { get; private set; }

	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06001D16 RID: 7446 RVA: 0x0001765E File Offset: 0x0001585E
	// (set) Token: 0x06001D17 RID: 7447 RVA: 0x00017665 File Offset: 0x00015865
	public static Vector3 Position { get; private set; }

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x06001D18 RID: 7448 RVA: 0x0001766D File Offset: 0x0001586D
	// (set) Token: 0x06001D19 RID: 7449 RVA: 0x00017674 File Offset: 0x00015874
	public static Vector3 Size { get; private set; }

	// Token: 0x17000198 RID: 408
	// (get) Token: 0x06001D1A RID: 7450 RVA: 0x0001767C File Offset: 0x0001587C
	public static Vector3 Center
	{
		get
		{
			return TerrainMeta.Position + TerrainMeta.Size * 0.5f;
		}
	}

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x06001D1B RID: 7451 RVA: 0x00017697 File Offset: 0x00015897
	// (set) Token: 0x06001D1C RID: 7452 RVA: 0x0001769E File Offset: 0x0001589E
	public static Vector3 OneOverSize { get; private set; }

	// Token: 0x1700019A RID: 410
	// (get) Token: 0x06001D1D RID: 7453 RVA: 0x000176A6 File Offset: 0x000158A6
	// (set) Token: 0x06001D1E RID: 7454 RVA: 0x000176AD File Offset: 0x000158AD
	public static Vector3 HighestPoint { get; set; }

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x06001D1F RID: 7455 RVA: 0x000176B5 File Offset: 0x000158B5
	// (set) Token: 0x06001D20 RID: 7456 RVA: 0x000176BC File Offset: 0x000158BC
	public static Vector3 LowestPoint { get; set; }

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x06001D21 RID: 7457 RVA: 0x000176C4 File Offset: 0x000158C4
	// (set) Token: 0x06001D22 RID: 7458 RVA: 0x000176CB File Offset: 0x000158CB
	public static float LootAxisAngle { get; private set; }

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x06001D23 RID: 7459 RVA: 0x000176D3 File Offset: 0x000158D3
	// (set) Token: 0x06001D24 RID: 7460 RVA: 0x000176DA File Offset: 0x000158DA
	public static float BiomeAxisAngle { get; private set; }

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x06001D25 RID: 7461 RVA: 0x000176E2 File Offset: 0x000158E2
	// (set) Token: 0x06001D26 RID: 7462 RVA: 0x000176E9 File Offset: 0x000158E9
	public static TerrainData Data { get; private set; }

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x06001D27 RID: 7463 RVA: 0x000176F1 File Offset: 0x000158F1
	// (set) Token: 0x06001D28 RID: 7464 RVA: 0x000176F8 File Offset: 0x000158F8
	public static TerrainCollider Collider { get; private set; }

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x06001D29 RID: 7465 RVA: 0x00017700 File Offset: 0x00015900
	// (set) Token: 0x06001D2A RID: 7466 RVA: 0x00017707 File Offset: 0x00015907
	public static TerrainCollision Collision { get; private set; }

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x06001D2B RID: 7467 RVA: 0x0001770F File Offset: 0x0001590F
	// (set) Token: 0x06001D2C RID: 7468 RVA: 0x00017716 File Offset: 0x00015916
	public static TerrainPhysics Physics { get; private set; }

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x06001D2D RID: 7469 RVA: 0x0001771E File Offset: 0x0001591E
	// (set) Token: 0x06001D2E RID: 7470 RVA: 0x00017725 File Offset: 0x00015925
	public static TerrainColors Colors { get; private set; }

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x06001D2F RID: 7471 RVA: 0x0001772D File Offset: 0x0001592D
	// (set) Token: 0x06001D30 RID: 7472 RVA: 0x00017734 File Offset: 0x00015934
	public static TerrainQuality Quality { get; private set; }

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x06001D31 RID: 7473 RVA: 0x0001773C File Offset: 0x0001593C
	// (set) Token: 0x06001D32 RID: 7474 RVA: 0x00017743 File Offset: 0x00015943
	public static TerrainPath Path { get; private set; }

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x06001D33 RID: 7475 RVA: 0x0001774B File Offset: 0x0001594B
	// (set) Token: 0x06001D34 RID: 7476 RVA: 0x00017752 File Offset: 0x00015952
	public static TerrainBiomeMap BiomeMap { get; private set; }

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x06001D35 RID: 7477 RVA: 0x0001775A File Offset: 0x0001595A
	// (set) Token: 0x06001D36 RID: 7478 RVA: 0x00017761 File Offset: 0x00015961
	public static TerrainAlphaMap AlphaMap { get; private set; }

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x06001D37 RID: 7479 RVA: 0x00017769 File Offset: 0x00015969
	// (set) Token: 0x06001D38 RID: 7480 RVA: 0x00017770 File Offset: 0x00015970
	public static TerrainBlendMap BlendMap { get; private set; }

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x06001D39 RID: 7481 RVA: 0x00017778 File Offset: 0x00015978
	// (set) Token: 0x06001D3A RID: 7482 RVA: 0x0001777F File Offset: 0x0001597F
	public static TerrainHeightMap HeightMap { get; private set; }

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x06001D3B RID: 7483 RVA: 0x00017787 File Offset: 0x00015987
	// (set) Token: 0x06001D3C RID: 7484 RVA: 0x0001778E File Offset: 0x0001598E
	public static TerrainSplatMap SplatMap { get; private set; }

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x06001D3D RID: 7485 RVA: 0x00017796 File Offset: 0x00015996
	// (set) Token: 0x06001D3E RID: 7486 RVA: 0x0001779D File Offset: 0x0001599D
	public static TerrainTopologyMap TopologyMap { get; private set; }

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06001D3F RID: 7487 RVA: 0x000177A5 File Offset: 0x000159A5
	// (set) Token: 0x06001D40 RID: 7488 RVA: 0x000177AC File Offset: 0x000159AC
	public static TerrainWaterMap WaterMap { get; private set; }

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06001D41 RID: 7489 RVA: 0x000177B4 File Offset: 0x000159B4
	// (set) Token: 0x06001D42 RID: 7490 RVA: 0x000177BB File Offset: 0x000159BB
	public static TerrainTexturing Texturing { get; private set; }

	// Token: 0x06001D43 RID: 7491 RVA: 0x0009F644 File Offset: 0x0009D844
	public static bool OutOfBounds(Vector3 worldPos)
	{
		return worldPos.x < TerrainMeta.Position.x || worldPos.z < TerrainMeta.Position.z || worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x || worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z;
	}

	// Token: 0x06001D44 RID: 7492 RVA: 0x0009F6B8 File Offset: 0x0009D8B8
	public static bool OutOfMargin(Vector3 worldPos)
	{
		return worldPos.x < TerrainMeta.Position.x - TerrainMeta.Size.x || worldPos.z < TerrainMeta.Position.z - TerrainMeta.Size.z || worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x + TerrainMeta.Size.x || worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z + TerrainMeta.Size.z;
	}

	// Token: 0x06001D45 RID: 7493 RVA: 0x0009F758 File Offset: 0x0009D958
	public static Vector3 Normalize(Vector3 worldPos)
	{
		float x = (worldPos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		float y = (worldPos.y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y;
		float z = (worldPos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return new Vector3(x, y, z);
	}

	// Token: 0x06001D46 RID: 7494 RVA: 0x000177C3 File Offset: 0x000159C3
	public static float NormalizeX(float x)
	{
		return (x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
	}

	// Token: 0x06001D47 RID: 7495 RVA: 0x000177DC File Offset: 0x000159DC
	public static float NormalizeY(float y)
	{
		return (y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y;
	}

	// Token: 0x06001D48 RID: 7496 RVA: 0x000177F5 File Offset: 0x000159F5
	public static float NormalizeZ(float z)
	{
		return (z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
	}

	// Token: 0x06001D49 RID: 7497 RVA: 0x0009F7C4 File Offset: 0x0009D9C4
	public static Vector3 Denormalize(Vector3 normPos)
	{
		float x = TerrainMeta.Position.x + normPos.x * TerrainMeta.Size.x;
		float y = TerrainMeta.Position.y + normPos.y * TerrainMeta.Size.y;
		float z = TerrainMeta.Position.z + normPos.z * TerrainMeta.Size.z;
		return new Vector3(x, y, z);
	}

	// Token: 0x06001D4A RID: 7498 RVA: 0x0001780E File Offset: 0x00015A0E
	public static float DenormalizeX(float normX)
	{
		return TerrainMeta.Position.x + normX * TerrainMeta.Size.x;
	}

	// Token: 0x06001D4B RID: 7499 RVA: 0x00017827 File Offset: 0x00015A27
	public static float DenormalizeY(float normY)
	{
		return TerrainMeta.Position.y + normY * TerrainMeta.Size.y;
	}

	// Token: 0x06001D4C RID: 7500 RVA: 0x00017840 File Offset: 0x00015A40
	public static float DenormalizeZ(float normZ)
	{
		return TerrainMeta.Position.z + normZ * TerrainMeta.Size.z;
	}

	// Token: 0x06001D4D RID: 7501 RVA: 0x00017859 File Offset: 0x00015A59
	protected void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Shader.DisableKeyword("TERRAIN_PAINTING");
	}

	// Token: 0x06001D4E RID: 7502 RVA: 0x0009F830 File Offset: 0x0009DA30
	public void Init(Terrain terrainOverride = null, TerrainConfig configOverride = null)
	{
		if (terrainOverride != null)
		{
			this.terrain = terrainOverride;
		}
		if (configOverride != null)
		{
			this.config = configOverride;
		}
		TerrainMeta.Terrain = this.terrain;
		TerrainMeta.Config = this.config;
		TerrainMeta.Transform = this.terrain.transform;
		TerrainMeta.Data = this.terrain.terrainData;
		TerrainMeta.Size = this.terrain.terrainData.size;
		TerrainMeta.OneOverSize = Vector3Ex.Inverse(TerrainMeta.Size);
		TerrainMeta.Position = this.terrain.GetPosition();
		TerrainMeta.Collider = this.terrain.GetComponent<TerrainCollider>();
		TerrainMeta.Collision = this.terrain.GetComponent<TerrainCollision>();
		TerrainMeta.Physics = this.terrain.GetComponent<TerrainPhysics>();
		TerrainMeta.Colors = this.terrain.GetComponent<TerrainColors>();
		TerrainMeta.Quality = this.terrain.GetComponent<TerrainQuality>();
		TerrainMeta.Path = this.terrain.GetComponent<TerrainPath>();
		TerrainMeta.BiomeMap = this.terrain.GetComponent<TerrainBiomeMap>();
		TerrainMeta.AlphaMap = this.terrain.GetComponent<TerrainAlphaMap>();
		TerrainMeta.BlendMap = this.terrain.GetComponent<TerrainBlendMap>();
		TerrainMeta.HeightMap = this.terrain.GetComponent<TerrainHeightMap>();
		TerrainMeta.SplatMap = this.terrain.GetComponent<TerrainSplatMap>();
		TerrainMeta.TopologyMap = this.terrain.GetComponent<TerrainTopologyMap>();
		TerrainMeta.WaterMap = this.terrain.GetComponent<TerrainWaterMap>();
		TerrainMeta.Texturing = this.terrain.GetComponent<TerrainTexturing>();
		TerrainMeta.HighestPoint = new Vector3(TerrainMeta.Position.x, TerrainMeta.Position.y + TerrainMeta.Size.y, TerrainMeta.Position.z);
		TerrainMeta.LowestPoint = new Vector3(TerrainMeta.Position.x, TerrainMeta.Position.y, TerrainMeta.Position.z);
		TerrainExtension[] components = base.GetComponents<TerrainExtension>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Init(this.terrain, this.config);
		}
		uint seed = World.Seed;
		int num = SeedRandom.Range(ref seed, 0, 4) * 90;
		int num2 = SeedRandom.Range(ref seed, -45, 46);
		int num3 = SeedRandom.Sign(ref seed);
		TerrainMeta.LootAxisAngle = (float)num;
		TerrainMeta.BiomeAxisAngle = (float)(num + num2 + num3 * 90);
	}

	// Token: 0x06001D4F RID: 7503 RVA: 0x0001786D File Offset: 0x00015A6D
	public static void InitNoTerrain()
	{
		TerrainMeta.Size = new Vector3(4096f, 4096f, 4096f);
	}

	// Token: 0x06001D50 RID: 7504 RVA: 0x0009FA6C File Offset: 0x0009DC6C
	public void SetupComponents()
	{
		foreach (TerrainExtension terrainExtension in base.GetComponents<TerrainExtension>())
		{
			terrainExtension.Setup();
			terrainExtension.isInitialized = true;
		}
	}

	// Token: 0x06001D51 RID: 7505 RVA: 0x0009FAA0 File Offset: 0x0009DCA0
	public void PostSetupComponents()
	{
		TerrainExtension[] components = base.GetComponents<TerrainExtension>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].PostSetup();
		}
	}

	// Token: 0x06001D52 RID: 7506 RVA: 0x0009FACC File Offset: 0x0009DCCC
	public void BindShaderProperties()
	{
		if (this.config)
		{
			Shader.SetGlobalTexture("Terrain_AlbedoArray", this.config.AlbedoArray);
			Shader.SetGlobalTexture("Terrain_NormalArray", this.config.NormalArray);
			Shader.SetGlobalVector("Terrain_TexelSize", new Vector2(1f / this.config.GetMinSplatTiling(), 1f / this.config.GetMinSplatTiling()));
			Shader.SetGlobalVector("Terrain_TexelSize0", new Vector4(1f / this.config.Splats[0].SplatTiling, 1f / this.config.Splats[1].SplatTiling, 1f / this.config.Splats[2].SplatTiling, 1f / this.config.Splats[3].SplatTiling));
			Shader.SetGlobalVector("Terrain_TexelSize1", new Vector4(1f / this.config.Splats[4].SplatTiling, 1f / this.config.Splats[5].SplatTiling, 1f / this.config.Splats[6].SplatTiling, 1f / this.config.Splats[7].SplatTiling));
			Shader.SetGlobalVector("Splat0_UVMIX", new Vector3(this.config.Splats[0].UVMIXMult, this.config.Splats[0].UVMIXStart, 1f / this.config.Splats[0].UVMIXDist));
			Shader.SetGlobalVector("Splat1_UVMIX", new Vector3(this.config.Splats[1].UVMIXMult, this.config.Splats[1].UVMIXStart, 1f / this.config.Splats[1].UVMIXDist));
			Shader.SetGlobalVector("Splat2_UVMIX", new Vector3(this.config.Splats[2].UVMIXMult, this.config.Splats[2].UVMIXStart, 1f / this.config.Splats[2].UVMIXDist));
			Shader.SetGlobalVector("Splat3_UVMIX", new Vector3(this.config.Splats[3].UVMIXMult, this.config.Splats[3].UVMIXStart, 1f / this.config.Splats[3].UVMIXDist));
			Shader.SetGlobalVector("Splat4_UVMIX", new Vector3(this.config.Splats[4].UVMIXMult, this.config.Splats[4].UVMIXStart, 1f / this.config.Splats[4].UVMIXDist));
			Shader.SetGlobalVector("Splat5_UVMIX", new Vector3(this.config.Splats[5].UVMIXMult, this.config.Splats[5].UVMIXStart, 1f / this.config.Splats[5].UVMIXDist));
			Shader.SetGlobalVector("Splat6_UVMIX", new Vector3(this.config.Splats[6].UVMIXMult, this.config.Splats[6].UVMIXStart, 1f / this.config.Splats[6].UVMIXDist));
			Shader.SetGlobalVector("Splat7_UVMIX", new Vector3(this.config.Splats[7].UVMIXMult, this.config.Splats[7].UVMIXStart, 1f / this.config.Splats[7].UVMIXDist));
		}
		if (TerrainMeta.HeightMap)
		{
			Shader.SetGlobalTexture("Terrain_Normal", TerrainMeta.HeightMap.NormalTexture);
		}
		if (TerrainMeta.AlphaMap)
		{
			Shader.SetGlobalTexture("Terrain_Alpha", TerrainMeta.AlphaMap.AlphaTexture);
		}
		if (TerrainMeta.BiomeMap)
		{
			Shader.SetGlobalTexture("Terrain_Biome", TerrainMeta.BiomeMap.BiomeTexture);
		}
		if (TerrainMeta.SplatMap)
		{
			Shader.SetGlobalTexture("Terrain_Control0", TerrainMeta.SplatMap.SplatTexture0);
			Shader.SetGlobalTexture("Terrain_Control1", TerrainMeta.SplatMap.SplatTexture1);
		}
		TerrainMeta.WaterMap;
		if (this.terrain)
		{
			Shader.SetGlobalVector("Terrain_Position", TerrainMeta.Position);
			Shader.SetGlobalVector("Terrain_Size", TerrainMeta.Size);
			Shader.SetGlobalVector("Terrain_RcpSize", TerrainMeta.OneOverSize);
			if (this.terrain.materialTemplate)
			{
				if (this.terrain.materialTemplate.IsKeywordEnabled("_TERRAIN_BLEND_LINEAR"))
				{
					this.terrain.materialTemplate.DisableKeyword("_TERRAIN_BLEND_LINEAR");
				}
				if (this.terrain.materialTemplate.IsKeywordEnabled("_TERRAIN_VERTEX_NORMALS"))
				{
					this.terrain.materialTemplate.DisableKeyword("_TERRAIN_VERTEX_NORMALS");
				}
			}
		}
	}

	// Token: 0x020004E9 RID: 1257
	public enum PaintMode
	{
		// Token: 0x04001980 RID: 6528
		None,
		// Token: 0x04001981 RID: 6529
		Splats,
		// Token: 0x04001982 RID: 6530
		Biomes,
		// Token: 0x04001983 RID: 6531
		Alpha,
		// Token: 0x04001984 RID: 6532
		Blend,
		// Token: 0x04001985 RID: 6533
		Field,
		// Token: 0x04001986 RID: 6534
		Cliff,
		// Token: 0x04001987 RID: 6535
		Summit,
		// Token: 0x04001988 RID: 6536
		Beachside,
		// Token: 0x04001989 RID: 6537
		Beach,
		// Token: 0x0400198A RID: 6538
		Forest,
		// Token: 0x0400198B RID: 6539
		Forestside,
		// Token: 0x0400198C RID: 6540
		Ocean,
		// Token: 0x0400198D RID: 6541
		Oceanside,
		// Token: 0x0400198E RID: 6542
		Decor,
		// Token: 0x0400198F RID: 6543
		Monument,
		// Token: 0x04001990 RID: 6544
		Road,
		// Token: 0x04001991 RID: 6545
		Roadside,
		// Token: 0x04001992 RID: 6546
		Bridge,
		// Token: 0x04001993 RID: 6547
		River,
		// Token: 0x04001994 RID: 6548
		Riverside,
		// Token: 0x04001995 RID: 6549
		Lake,
		// Token: 0x04001996 RID: 6550
		Lakeside,
		// Token: 0x04001997 RID: 6551
		Offshore,
		// Token: 0x04001998 RID: 6552
		Powerline,
		// Token: 0x04001999 RID: 6553
		Runway,
		// Token: 0x0400199A RID: 6554
		Building,
		// Token: 0x0400199B RID: 6555
		Cliffside,
		// Token: 0x0400199C RID: 6556
		Mountain,
		// Token: 0x0400199D RID: 6557
		Clutter,
		// Token: 0x0400199E RID: 6558
		Alt,
		// Token: 0x0400199F RID: 6559
		Tier0,
		// Token: 0x040019A0 RID: 6560
		Tier1,
		// Token: 0x040019A1 RID: 6561
		Tier2,
		// Token: 0x040019A2 RID: 6562
		Mainland,
		// Token: 0x040019A3 RID: 6563
		Hilltop
	}
}
