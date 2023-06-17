using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200056A RID: 1386
[ExecuteInEditMode]
public class WaterSystem : MonoBehaviour
{
	// Token: 0x04001BB7 RID: 7095
	public WaterQuality Quality = WaterQuality.High;

	// Token: 0x04001BB8 RID: 7096
	public bool ShowDebug;

	// Token: 0x04001BB9 RID: 7097
	public bool ShowGizmos;

	// Token: 0x04001BBA RID: 7098
	public bool ProgressTime = true;

	// Token: 0x04001BBB RID: 7099
	public WaterSystem.SimulationSettings Simulation = new WaterSystem.SimulationSettings();

	// Token: 0x04001BBC RID: 7100
	public WaterSystem.RenderingSettings Rendering = new WaterSystem.RenderingSettings();

	// Token: 0x04001BBD RID: 7101
	[HideInInspector]
	public WaterGerstner.Wave[] GerstnerWaves;

	// Token: 0x04001BC3 RID: 7107
	public static readonly int[] QualityToMaxVertices = new int[]
	{
		32000,
		128000,
		200000
	};

	// Token: 0x04001BC4 RID: 7108
	private WaterQuality prevQuality = WaterQuality.High;

	// Token: 0x04001BC5 RID: 7109
	private ReflectionProbeEx reflectionProbe;

	// Token: 0x04001BC6 RID: 7110
	private float reflectionProbeUpdateTime;

	// Token: 0x04001BC7 RID: 7111
	private bool reflectionProbeReady;

	// Token: 0x04001BC8 RID: 7112
	private Texture2D defaultHeightSlopeMap;

	// Token: 0x04001BC9 RID: 7113
	private bool hasValidCausticsAnims;

	// Token: 0x04001BCA RID: 7114
	private List<WaterRuntime> runtimeCleanup = new List<WaterRuntime>();

	// Token: 0x04001BCB RID: 7115
	private ComputeBuffer gerstnerWaveBuffer;

	// Token: 0x04001BCF RID: 7119
	private static WaterSystem instance;

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x06001F96 RID: 8086 RVA: 0x00018F77 File Offset: 0x00017177
	// (set) Token: 0x06001F95 RID: 8085 RVA: 0x00018F6E File Offset: 0x0001716E
	public bool IsInitialized { get; private set; }

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x06001F97 RID: 8087 RVA: 0x00018F7F File Offset: 0x0001717F
	// (set) Token: 0x06001F98 RID: 8088 RVA: 0x00018F86 File Offset: 0x00017186
	public static WaterCollision Collision { get; private set; }

	// Token: 0x170001FA RID: 506
	// (get) Token: 0x06001F9A RID: 8090 RVA: 0x00018F96 File Offset: 0x00017196
	// (set) Token: 0x06001F99 RID: 8089 RVA: 0x00018F8E File Offset: 0x0001718E
	public static WaterDynamics Dynamics { get; private set; }

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x06001F9C RID: 8092 RVA: 0x00018FA5 File Offset: 0x000171A5
	// (set) Token: 0x06001F9B RID: 8091 RVA: 0x00018F9D File Offset: 0x0001719D
	public static WaterBody Ocean { get; private set; } = null;

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x06001F9E RID: 8094 RVA: 0x00018FB4 File Offset: 0x000171B4
	// (set) Token: 0x06001F9D RID: 8093 RVA: 0x00018FAC File Offset: 0x000171AC
	public static HashSet<WaterBody> WaterBodies { get; private set; } = new HashSet<WaterBody>();

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x06001F9F RID: 8095 RVA: 0x00018FBB File Offset: 0x000171BB
	public bool HasCaustics
	{
		get
		{
			return this.Quality >= WaterQuality.Medium && this.hasValidCausticsAnims;
		}
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x06001FA1 RID: 8097 RVA: 0x00018FD6 File Offset: 0x000171D6
	// (set) Token: 0x06001FA0 RID: 8096 RVA: 0x00018FCE File Offset: 0x000171CE
	public static Dictionary<WaterCamera, WaterRuntime> Runtimes { get; private set; } = new Dictionary<WaterCamera, WaterRuntime>();

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x06001FA3 RID: 8099 RVA: 0x00018FE5 File Offset: 0x000171E5
	// (set) Token: 0x06001FA2 RID: 8098 RVA: 0x00018FDD File Offset: 0x000171DD
	public static HashSet<WaterDepthMask> DepthMasks { get; private set; } = new HashSet<WaterDepthMask>();

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x06001FA4 RID: 8100 RVA: 0x00018FEC File Offset: 0x000171EC
	// (set) Token: 0x06001FA5 RID: 8101 RVA: 0x00018FF3 File Offset: 0x000171F3
	public static float WaveTime { get; private set; } = 0f;

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x06001FA6 RID: 8102 RVA: 0x00018FFB File Offset: 0x000171FB
	public static WaterSystem Instance
	{
		get
		{
			return WaterSystem.instance;
		}
	}

	// Token: 0x06001FA7 RID: 8103 RVA: 0x000ACDA4 File Offset: 0x000AAFA4
	private void CheckInstance()
	{
		WaterSystem.instance = ((WaterSystem.instance != null) ? WaterSystem.instance : this);
		WaterSystem.Collision = ((WaterSystem.Collision != null) ? WaterSystem.Collision : base.GetComponent<WaterCollision>());
		WaterSystem.Dynamics = ((WaterSystem.Dynamics != null) ? WaterSystem.Dynamics : base.GetComponent<WaterDynamics>());
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x00019002 File Offset: 0x00017202
	public void Awake()
	{
		this.CheckInstance();
	}

	// Token: 0x06001FA9 RID: 8105 RVA: 0x000ACE0C File Offset: 0x000AB00C
	public static float GetHeight(Vector3 pos)
	{
		float num;
		return WaterSystem.GetHeight(pos, out num);
	}

	// Token: 0x06001FAA RID: 8106 RVA: 0x000ACE24 File Offset: 0x000AB024
	public static float GetHeight(Vector3 pos, out float terrainHeight)
	{
		float normX = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		float normZ = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return WaterSystem.GetHeight(pos, normX, normZ, out terrainHeight);
	}

	// Token: 0x06001FAB RID: 8107 RVA: 0x000ACE74 File Offset: 0x000AB074
	public static float GetHeight(Vector3 pos, float normX, float normZ, out float terrainHeight)
	{
		float result = (TerrainMeta.WaterMap != null) ? TerrainMeta.WaterMap.GetHeightFast(normX, normZ) : 0f;
		float num = (WaterSystem.Instance != null) ? WaterSystem.Ocean.Transform.position.y : 0f;
		terrainHeight = ((TerrainMeta.HeightMap != null) ? TerrainMeta.HeightMap.GetHeight(pos) : 0f);
		if (WaterSystem.instance != null && WaterSystem.instance.GerstnerWaves != null)
		{
			float num2 = Mathf.Clamp01(Mathf.Abs(num - terrainHeight) * 0.1f);
			result = WaterGerstner.SampleHeight(WaterSystem.instance.GerstnerWaves, pos) * num2;
		}
		return result;
	}

	// Token: 0x06001FAC RID: 8108 RVA: 0x000ACF30 File Offset: 0x000AB130
	public static Vector3 GetNormal(Vector3 pos)
	{
		return ((TerrainMeta.WaterMap != null) ? TerrainMeta.WaterMap.GetNormal(pos) : Vector3.up).normalized;
	}

	// Token: 0x06001FAD RID: 8109 RVA: 0x0001900A File Offset: 0x0001720A
	public void GenerateWaves()
	{
		this.GerstnerWaves = WaterGerstner.SetupWaves(this.Simulation.Wind, this.Simulation.GerstnerWaves);
	}

	// Token: 0x06001FAE RID: 8110 RVA: 0x000ACF64 File Offset: 0x000AB164
	public static void RegisterBody(WaterBody body)
	{
		if (body.Type == WaterBodyType.Ocean)
		{
			if (WaterSystem.Ocean == null)
			{
				WaterSystem.Ocean = body;
			}
			else if (WaterSystem.Ocean != body)
			{
				Debug.LogWarning("[Water] Ocean body is already registered. Ignoring call because only one is allowed.");
				return;
			}
		}
		WaterSystem.WaterBodies.Add(body);
	}

	// Token: 0x06001FAF RID: 8111 RVA: 0x0001902D File Offset: 0x0001722D
	public static void UnregisterBody(WaterBody body)
	{
		WaterSystem.WaterBodies.Remove(body);
	}

	// Token: 0x06001FB0 RID: 8112 RVA: 0x0001903B File Offset: 0x0001723B
	private void UpdateWaveTime()
	{
		WaterSystem.WaveTime = (this.ProgressTime ? (UnityEngine.Time.realtimeSinceStartup - (EnvSync.EngineTimeClient - EnvSync.EngineTimeServer)) : WaterSystem.WaveTime);
	}

	// Token: 0x06001FB1 RID: 8113 RVA: 0x00019062 File Offset: 0x00017262
	private void Update()
	{
		this.UpdateWaveTime();
		this.UpdateClient();
	}

	// Token: 0x06001FB2 RID: 8114 RVA: 0x000ACFB4 File Offset: 0x000AB1B4
	private static void ShutdownRuntimes()
	{
		foreach (KeyValuePair<WaterCamera, WaterRuntime> keyValuePair in WaterSystem.Runtimes)
		{
			keyValuePair.Value.Shutdown();
		}
	}

	// Token: 0x06001FB3 RID: 8115 RVA: 0x00019070 File Offset: 0x00017270
	public static void Clear()
	{
		WaterSystem.ShutdownRuntimes();
		WaterSystem.Runtimes.Clear();
		WaterSystem.WaterBodies.Clear();
		WaterSystem.DepthMasks.Clear();
	}

	// Token: 0x06001FB4 RID: 8116 RVA: 0x00019095 File Offset: 0x00017295
	public static void RegisterCamera(WaterCamera camera)
	{
		WaterSystem.Runtimes.Add(camera, new WaterRuntime(camera));
	}

	// Token: 0x06001FB5 RID: 8117 RVA: 0x000190A8 File Offset: 0x000172A8
	public static void UnregisterCamera(WaterCamera camera)
	{
		WaterSystem.Runtimes.Remove(camera);
	}

	// Token: 0x06001FB6 RID: 8118 RVA: 0x000190B6 File Offset: 0x000172B6
	public static void RegisterDepthMask(WaterDepthMask mask)
	{
		WaterSystem.DepthMasks.Add(mask);
	}

	// Token: 0x06001FB7 RID: 8119 RVA: 0x000190C4 File Offset: 0x000172C4
	public static void UnregisterDepthMask(WaterDepthMask mask)
	{
		WaterSystem.DepthMasks.Remove(mask);
	}

	// Token: 0x06001FB8 RID: 8120 RVA: 0x000AD00C File Offset: 0x000AB20C
	private void CreateReflectionProbe()
	{
		this.DestroyReflectionProbe();
		if (this.reflectionProbe == null)
		{
			GameObject gameObject = new GameObject("Reflection")
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			this.reflectionProbe = gameObject.AddComponent<ReflectionProbeEx>();
			this.reflectionProbe.transform.parent = base.transform;
			this.reflectionProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
			this.reflectionProbe.timeSlicing = this.Rendering.SkyReflections.TimeSlicing;
			this.reflectionProbe.resolution = 256;
			this.reflectionProbe.hdr = true;
			this.reflectionProbe.shadowDistance = 0f;
			this.reflectionProbe.clearFlags = ReflectionProbeClearFlags.SolidColor;
			this.reflectionProbe.background = Color.black;
			this.reflectionProbe.nearClip = 25f;
			this.reflectionProbe.farClip = 2500f;
		}
		this.reflectionProbe.ClearRenderList();
		this.reflectionProbeReady = false;
	}

	// Token: 0x06001FB9 RID: 8121 RVA: 0x000190D2 File Offset: 0x000172D2
	private void DestroyReflectionProbe()
	{
		if (this.reflectionProbe != null)
		{
			Object.DestroyImmediate(this.reflectionProbe.gameObject);
			this.reflectionProbe = null;
		}
	}

	// Token: 0x06001FBA RID: 8122 RVA: 0x000AD108 File Offset: 0x000AB308
	private void CheckReflectionProbe()
	{
		Camera camera = MainCamera.isValid ? MainCamera.mainCamera : Camera.main;
		if (!this.reflectionProbeReady && TOD_Sky.Instance != null && TOD_Sky.Instance.Components != null && camera != null)
		{
			this.reflectionProbe.AddToRenderList(TOD_Sky.Instance.Components.Stars.GetComponent<Renderer>(), false);
			this.reflectionProbe.AddToRenderList(TOD_Sky.Instance.Components.Moon.GetComponent<Renderer>(), false);
			this.reflectionProbe.AddToRenderList(TOD_Sky.Instance.Components.Sun.GetComponent<Renderer>(), false);
			this.reflectionProbe.AddToRenderList(TOD_Sky.Instance.Components.Atmosphere.GetComponent<Renderer>(), true);
			this.reflectionProbe.AddToRenderList(TOD_Sky.Instance.Components.Clouds.GetComponent<Renderer>(), false);
			foreach (Renderer renderer in TOD_Sky.Instance.Components.Billboards.GetComponentsInChildren<Renderer>())
			{
				this.reflectionProbe.AddToRenderList(renderer, false);
			}
			this.reflectionProbe.attachToTarget = camera.transform;
			this.reflectionProbe.RenderProbe();
			this.reflectionProbeReady = true;
		}
	}

	// Token: 0x06001FBB RID: 8123 RVA: 0x000AD25C File Offset: 0x000AB45C
	private void CreateDefaultHeightSlopeMap()
	{
		if (this.defaultHeightSlopeMap == null)
		{
			Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGHalf, false, true);
			texture2D.name = "Default_HeightSlope";
			texture2D.filterMode = FilterMode.Trilinear;
			texture2D.wrapMode = TextureWrapMode.Clamp;
			texture2D.SetPixel(0, 0, new Color(-10000f, 0f, 0f));
			texture2D.Apply();
			Shader.SetGlobalTexture("Terrain_HeightSlope", texture2D);
			this.defaultHeightSlopeMap = texture2D;
		}
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x000190F9 File Offset: 0x000172F9
	private void DestroyDefaultHeightSlopeMap()
	{
		if (this.defaultHeightSlopeMap != null)
		{
			Object.DestroyImmediate(this.defaultHeightSlopeMap);
			this.defaultHeightSlopeMap = null;
		}
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x000AD2D0 File Offset: 0x000AB4D0
	private void OnEnable()
	{
		this.prevQuality = this.Quality;
		this.CheckInstance();
		this.CreateReflectionProbe();
		this.CreateDefaultHeightSlopeMap();
		Texture2D[] framesDeep = this.Rendering.CausticsAnimation.FramesDeep;
		Texture2D[] framesShallow = this.Rendering.CausticsAnimation.FramesShallow;
		this.hasValidCausticsAnims = (framesDeep != null && framesDeep.Length != 0 && framesShallow != null && framesShallow.Length != 0);
		if (!this.hasValidCausticsAnims)
		{
			Debug.LogWarning("[Water] Missing caustics animations. Disabling caustics on all quality modes.");
		}
		this.IsInitialized = true;
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x0001911B File Offset: 0x0001731B
	private void OnDisable()
	{
		if (Application.isPlaying && Application.isQuitting)
		{
			return;
		}
		WaterSystem.ShutdownRuntimes();
		this.DestroyReflectionProbe();
		this.DestroyDefaultHeightSlopeMap();
		this.DestroyComputeBuffer(ref this.gerstnerWaveBuffer);
		this.IsInitialized = false;
	}

	// Token: 0x06001FBF RID: 8127 RVA: 0x000AD354 File Offset: 0x000AB554
	public void UpdateClient()
	{
		this.Quality = (WaterQuality)Water.quality;
		this.CheckInstance();
		this.CheckReflectionProbe();
		if (this.UpdateQuality() && this.IsInitialized)
		{
			foreach (WaterBody waterBody in WaterSystem.WaterBodies)
			{
				if (waterBody.Type == WaterBodyType.Ocean && waterBody.Renderer.enabled)
				{
					waterBody.Renderer.enabled = false;
				}
			}
			this.UpdateReflectionProbe();
			this.UpdateKeywords();
			this.UpdateGlobalShaderProperties();
			this.UpdateWaves();
			this.UpdateRuntimes();
		}
	}

	// Token: 0x06001FC0 RID: 8128 RVA: 0x00019150 File Offset: 0x00017350
	private void DestroyComputeBuffer(ref ComputeBuffer buffer)
	{
		if (buffer != null)
		{
			buffer.Release();
			buffer = null;
		}
	}

	// Token: 0x06001FC1 RID: 8129 RVA: 0x000AD408 File Offset: 0x000AB608
	private void UpdateGerstnerWaveBuffer(WaterGerstner.Wave[] waves)
	{
		if (this.gerstnerWaveBuffer == null || this.gerstnerWaveBuffer.count != waves.Length)
		{
			this.DestroyComputeBuffer(ref this.gerstnerWaveBuffer);
			this.gerstnerWaveBuffer = new ComputeBuffer(waves.Length, Marshal.SizeOf(typeof(WaterGerstner.Wave)));
		}
		this.gerstnerWaveBuffer.SetData(waves);
	}

	// Token: 0x06001FC2 RID: 8130 RVA: 0x00019160 File Offset: 0x00017360
	private void UpdateWaveBuffer<T>(ref ComputeBuffer buffer, T[] waves)
	{
		if (buffer == null || buffer.count != waves.Length)
		{
			this.DestroyComputeBuffer(ref buffer);
			buffer = new ComputeBuffer(waves.Length, Marshal.SizeOf(typeof(T)));
		}
		buffer.SetData(waves);
	}

	// Token: 0x06001FC3 RID: 8131 RVA: 0x000AD464 File Offset: 0x000AB664
	private void UpdateWaves()
	{
		if (this.GerstnerWaves == null || this.GerstnerWaves.Length == 0)
		{
			this.GenerateWaves();
		}
		this.UpdateWaveBuffer<WaterGerstner.Wave>(ref this.gerstnerWaveBuffer, this.GerstnerWaves);
		Shader.SetGlobalBuffer("_WaveData", this.gerstnerWaveBuffer);
		Shader.SetGlobalInt("_WaveCount", this.GerstnerWaves.Length);
		Shader.SetGlobalFloat("_WaveTime", WaterSystem.WaveTime);
	}

	// Token: 0x06001FC4 RID: 8132 RVA: 0x000AD4CC File Offset: 0x000AB6CC
	private void UpdateRuntimes()
	{
		this.runtimeCleanup.Clear();
		foreach (KeyValuePair<WaterCamera, WaterRuntime> keyValuePair in WaterSystem.Runtimes)
		{
			WaterRuntime value = keyValuePair.Value;
			if (value != null)
			{
				if (value.WaterCamera != null)
				{
					if (!value.IsInitialized)
					{
						value.Initialize(this);
					}
					if (value.Simulation.CheckLostData() || value.Rendering.CheckLostData())
					{
						value.Shutdown();
						value.Initialize(this);
					}
					value.Update();
				}
				else
				{
					value.Shutdown();
					this.runtimeCleanup.Add(value);
				}
			}
		}
		foreach (WaterRuntime waterRuntime in this.runtimeCleanup)
		{
			WaterSystem.Runtimes.Remove(waterRuntime.WaterCamera);
		}
	}

	// Token: 0x06001FC5 RID: 8133 RVA: 0x000AD5DC File Offset: 0x000AB7DC
	public void UpdateMaterial(Material material)
	{
		if (WaterSystem.Ocean != null && material == WaterSystem.Ocean.Material)
		{
			foreach (KeyValuePair<WaterCamera, WaterRuntime> keyValuePair in WaterSystem.Runtimes)
			{
				keyValuePair.Value.Rendering.UpdateUnderwaterMaterial(WaterSystem.Ocean.Material);
			}
		}
	}

	// Token: 0x06001FC6 RID: 8134 RVA: 0x000AD664 File Offset: 0x000AB864
	public void ClearUnderwaterScatterCoefficientOverride()
	{
		foreach (KeyValuePair<WaterCamera, WaterRuntime> keyValuePair in WaterSystem.Runtimes)
		{
			keyValuePair.Value.Rendering.ClearUnderwaterScatterCoefficientOverride();
		}
	}

	// Token: 0x06001FC7 RID: 8135 RVA: 0x000AD6C0 File Offset: 0x000AB8C0
	public void SetUnderwaterScatterCoefficientOverride(float scatterCoefficient)
	{
		foreach (KeyValuePair<WaterCamera, WaterRuntime> keyValuePair in WaterSystem.Runtimes)
		{
			keyValuePair.Value.Rendering.SetUnderwaterScatterCoefficientOverride(scatterCoefficient);
		}
	}

	// Token: 0x06001FC8 RID: 8136 RVA: 0x000AD720 File Offset: 0x000AB920
	private void UpdateReflectionProbe()
	{
		if (this.reflectionProbe.IsFinishedRendering() && this.reflectionProbeUpdateTime > this.Rendering.SkyReflections.ProbeUpdateInterval)
		{
			this.reflectionProbe.timeSlicing = this.Rendering.SkyReflections.TimeSlicing;
			this.reflectionProbe.RenderProbe();
			this.reflectionProbeUpdateTime = 0f;
		}
		this.reflectionProbeUpdateTime += UnityEngine.Time.deltaTime;
	}

	// Token: 0x06001FC9 RID: 8137 RVA: 0x0001919A File Offset: 0x0001739A
	public void ResetVisibility()
	{
		this.ToggleVisibility(255);
		WaterVisibilityTrigger.Reset();
	}

	// Token: 0x06001FCA RID: 8138 RVA: 0x000AD798 File Offset: 0x000AB998
	public void ToggleVisibility(int mask)
	{
		foreach (KeyValuePair<WaterCamera, WaterRuntime> keyValuePair in WaterSystem.Runtimes)
		{
			keyValuePair.Value.SetVisibilityMask(mask);
		}
		foreach (WaterBody waterBody in WaterSystem.WaterBodies)
		{
			if (waterBody.Type != WaterBodyType.Ocean)
			{
				waterBody.Renderer.enabled = ((mask & (int)waterBody.Type) != 0);
			}
			for (int i = 0; i < waterBody.Triggers.Length; i++)
			{
				if (waterBody.Triggers[i] != null)
				{
					waterBody.Triggers[i].enabled = ((mask & (int)waterBody.Type) != 0);
				}
			}
		}
	}

	// Token: 0x06001FCB RID: 8139 RVA: 0x000AD88C File Offset: 0x000ABA8C
	private void UpdateGlobalShaderProperties()
	{
		if (TOD_Sky.Instance != null)
		{
			Vector4 vector = TOD_Sky.Instance.AmbientColor.linear;
			Vector4 vector2 = TOD_Sky.Instance.LightColor.linear * TOD_Sky.Instance.LightIntensity;
			Vector4 value = vector2 + vector;
			Shader.SetGlobalVector("_Water_AmbientLighting", vector);
			Shader.SetGlobalVector("_Water_PrimaryLighting", vector2);
			Shader.SetGlobalVector("_Water_CombinedLighting", value);
		}
		WaterSystem.RenderingSettings.Caustics causticsAnimation = this.Rendering.CausticsAnimation;
		WaterSystem.RenderingSettings.SSR screenSpaceReflections = this.Rendering.ScreenSpaceReflections;
		float thicknessMin = this.Rendering.ScreenSpaceReflections.ThicknessMin;
		float thicknessMax = screenSpaceReflections.ThicknessMax;
		float thicknessStartDist = screenSpaceReflections.ThicknessStartDist;
		float w = 1f / (screenSpaceReflections.ThicknessEndDist - screenSpaceReflections.ThicknessStartDist);
		Shader.SetGlobalFloat("_WaterSSR_FresnelCutoff", screenSpaceReflections.FresnelCutoff);
		Shader.SetGlobalVector("_WaterSSR_ThicknessParams", new Vector4(thicknessMin, thicknessMax, thicknessStartDist, w));
		if (this.HasCaustics && TOD_Sky.Instance != null)
		{
			int num = Mathf.Min(causticsAnimation.FramesShallow.Length, causticsAnimation.FramesDeep.Length);
			float num2 = UnityEngine.Time.time * causticsAnimation.FrameRate;
			int num3 = Mathf.FloorToInt(num2 + 0f) % num;
			int num4 = Mathf.FloorToInt(num2 + 1f) % num;
			float value2 = num2 - (float)((int)num2);
			Shader.SetGlobalTexture("_WaterCaustics_Shallow_Texture0", causticsAnimation.FramesShallow[num3]);
			Shader.SetGlobalTexture("_WaterCaustics_Shallow_Texture1", causticsAnimation.FramesShallow[num4]);
			Shader.SetGlobalTexture("_WaterCaustics_Deep_Texture0", causticsAnimation.FramesDeep[num3]);
			Shader.SetGlobalTexture("_WaterCaustics_Deep_Texture1", causticsAnimation.FramesDeep[num4]);
			Shader.SetGlobalFloat("_WaterCaustics_Blend01", value2);
			Shader.SetGlobalVector("_WaterCaustics_LightDirection", -TOD_Sky.Instance.LightDirection);
		}
		if (this.reflectionProbe != null)
		{
			Shader.SetGlobalTexture("_Water_ReflProbe", this.reflectionProbe.Texture);
			Shader.SetGlobalVector("_Water_ReflProbe_HDR", new Vector2(Mathf.GammaToLinearSpace(1f), 1f));
		}
		if (WaterSystem.Ocean != null)
		{
			Shader.SetGlobalColor("_Water_OceanColor", WaterSystem.Ocean.Material.GetColor("_Color").linear);
			Shader.SetGlobalColor("_Water_OceanWaterColor", WaterSystem.Ocean.Material.GetColor("_WaterColor").linear);
			Shader.SetGlobalFloat("_Water_OceanSmoothness", WaterSystem.Ocean.Material.GetFloat("_Smoothness"));
			Shader.SetGlobalVector("_Water_OceanColorExtinction", WaterSystem.Ocean.Material.GetVector("_ColorExtinction"));
			Shader.SetGlobalFloat("_Water_OceanScatterCoefficient", WaterSystem.Ocean.Material.GetFloat("_ScatterCoefficient"));
			Shader.SetGlobalFloat("_OceanWaterLevel", WaterSystem.Ocean.Transform.position.y);
			Shader.SetGlobalFloat("_TessWorldUVScale", WaterSystem.Ocean.Material.GetFloat("_TessWorldUVScale"));
			Shader.SetGlobalFloat("_TessNormalScale", WaterSystem.Ocean.Material.GetFloat("_TessNormalScale"));
			Shader.SetGlobalFloat("_TessDispScale", WaterSystem.Ocean.Material.GetFloat("_TessDispScale"));
		}
		else
		{
			Shader.SetGlobalFloat("_OceanWaterLevel", -10000f);
		}
		Texture texture = (TerrainTexturing.Instance != null) ? TerrainTexturing.Instance.CoarseHeightSlopeMap : null;
		Shader.SetGlobalTexture("Terrain_HeightSlope", (texture != null) ? texture : this.defaultHeightSlopeMap);
	}

	// Token: 0x06001FCC RID: 8140 RVA: 0x000191AC File Offset: 0x000173AC
	private bool UpdateQuality()
	{
		this.Quality = (WaterQuality)Water.quality;
		if (this.Quality != this.prevQuality)
		{
			base.enabled = false;
			base.enabled = true;
			return false;
		}
		return true;
	}

	// Token: 0x06001FCD RID: 8141 RVA: 0x000191D8 File Offset: 0x000173D8
	private void UpdateKeywords()
	{
		KeywordUtil.EnsureKeywordState("_WATER_QUALITY_HIGH", this.Quality == WaterQuality.High);
		KeywordUtil.EnsureKeywordState("_WATER_QUALITY_MEDIUM", this.Quality == WaterQuality.Medium);
		KeywordUtil.EnsureKeywordState("_WATER_REFLECTIONS", Water.reflections > 0);
	}

	// Token: 0x0200056B RID: 1387
	public struct WaveSample
	{
		// Token: 0x04001BD0 RID: 7120
		public Vector3 position;

		// Token: 0x04001BD1 RID: 7121
		public Vector3 normal;
	}

	// Token: 0x0200056C RID: 1388
	[Serializable]
	public class SimulationSettings
	{
		// Token: 0x04001BD2 RID: 7122
		public Vector3 Wind = new Vector3(3f, 0f, 3f);

		// Token: 0x04001BD3 RID: 7123
		public int SolverResolution = 64;

		// Token: 0x04001BD4 RID: 7124
		public float SolverSizeInWorld = 18f;

		// Token: 0x04001BD5 RID: 7125
		public float Gravity = 9.81f;

		// Token: 0x04001BD6 RID: 7126
		public float Amplitude = 0.0001f;

		// Token: 0x04001BD7 RID: 7127
		public TextAsset PerlinNoiseData;

		// Token: 0x04001BD8 RID: 7128
		public WaterGerstner.WaveSettings GerstnerWaves;
	}

	// Token: 0x0200056D RID: 1389
	[Serializable]
	public class RenderingSettings
	{
		// Token: 0x04001BD9 RID: 7129
		public float MaxDisplacementDistance = 50f;

		// Token: 0x04001BDA RID: 7130
		public WaterSystem.RenderingSettings.SkyProbe SkyReflections;

		// Token: 0x04001BDB RID: 7131
		public WaterSystem.RenderingSettings.SSR ScreenSpaceReflections;

		// Token: 0x04001BDC RID: 7132
		public WaterSystem.RenderingSettings.Caustics CausticsAnimation;

		// Token: 0x0200056E RID: 1390
		[Serializable]
		public class SkyProbe
		{
			// Token: 0x04001BDD RID: 7133
			public float ProbeUpdateInterval = 1f;

			// Token: 0x04001BDE RID: 7134
			public bool TimeSlicing = true;
		}

		// Token: 0x0200056F RID: 1391
		[Serializable]
		public class SSR
		{
			// Token: 0x04001BDF RID: 7135
			public float FresnelCutoff = 0.02f;

			// Token: 0x04001BE0 RID: 7136
			public float ThicknessMin = 1f;

			// Token: 0x04001BE1 RID: 7137
			public float ThicknessMax = 20f;

			// Token: 0x04001BE2 RID: 7138
			public float ThicknessStartDist = 40f;

			// Token: 0x04001BE3 RID: 7139
			public float ThicknessEndDist = 100f;
		}

		// Token: 0x02000570 RID: 1392
		[Serializable]
		public class Caustics
		{
			// Token: 0x04001BE4 RID: 7140
			public float FrameRate = 15f;

			// Token: 0x04001BE5 RID: 7141
			public Texture2D[] FramesShallow = new Texture2D[0];

			// Token: 0x04001BE6 RID: 7142
			public Texture2D[] FramesDeep = new Texture2D[0];
		}
	}
}
