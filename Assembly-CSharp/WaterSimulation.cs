using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000566 RID: 1382
[Serializable]
public class WaterSimulation
{
	// Token: 0x04001B91 RID: 7057
	private const int PerlinSize = 64;

	// Token: 0x04001B92 RID: 7058
	private const int MaxSolverResolution = 512;

	// Token: 0x04001B93 RID: 7059
	private WaterSystem water;

	// Token: 0x04001B94 RID: 7060
	private Camera camera;

	// Token: 0x04001B95 RID: 7061
	private int solverResolution = 64;

	// Token: 0x04001B96 RID: 7062
	private float solverSizeInWorld = 18f;

	// Token: 0x04001B97 RID: 7063
	private float gravity = 9.81f;

	// Token: 0x04001B98 RID: 7064
	private float amplitude = 0.0001f;

	// Token: 0x04001B99 RID: 7065
	private int solverButterflyCount;

	// Token: 0x04001B9A RID: 7066
	private Vector2 windDirection;

	// Token: 0x04001B9B RID: 7067
	private float windMagnitude;

	// Token: 0x04001B9C RID: 7068
	private Texture2D perlinNoiseMap;

	// Token: 0x04001B9D RID: 7069
	private RenderTexture displacementMap;

	// Token: 0x04001B9E RID: 7070
	private Vector4 displacementMapTexelSize;

	// Token: 0x04001B9F RID: 7071
	private RenderTexture normalFoldMap;

	// Token: 0x04001BA0 RID: 7072
	private Material computeNormalFoldMat;

	// Token: 0x04001BA1 RID: 7073
	private Material simulationMat;

	// Token: 0x04001BA2 RID: 7074
	private Texture2D hTilde0Map;

	// Token: 0x04001BA3 RID: 7075
	private Texture2D dispersionMap;

	// Token: 0x04001BA4 RID: 7076
	private Texture2D butterflyMap;

	// Token: 0x04001BA5 RID: 7077
	private RenderTexture hTilde_hMap;

	// Token: 0x04001BA6 RID: 7078
	private RenderTexture hTilde_dxdzMap;

	// Token: 0x04001BA7 RID: 7079
	private CommandBufferManager commandBufferManager;

	// Token: 0x04001BA8 RID: 7080
	private CommandBufferDesc commandBufferDesc;

	// Token: 0x04001BA9 RID: 7081
	private bool playing;

	// Token: 0x04001BAA RID: 7082
	private bool initialized;

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x06001F6E RID: 8046 RVA: 0x00018D19 File Offset: 0x00016F19
	public Texture2D PerlinNoiseMap
	{
		get
		{
			return this.perlinNoiseMap;
		}
	}

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x06001F6F RID: 8047 RVA: 0x00018D21 File Offset: 0x00016F21
	public Texture DisplacementMap
	{
		get
		{
			return this.displacementMap;
		}
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x06001F70 RID: 8048 RVA: 0x00018D29 File Offset: 0x00016F29
	public Vector4 DisplacementMapTexelSize
	{
		get
		{
			return this.displacementMapTexelSize;
		}
	}

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x06001F71 RID: 8049 RVA: 0x00018D31 File Offset: 0x00016F31
	public RenderTexture NormalFoldMap
	{
		get
		{
			return this.normalFoldMap;
		}
	}

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x06001F72 RID: 8050 RVA: 0x00018D39 File Offset: 0x00016F39
	public bool IsPlaying
	{
		get
		{
			return this.playing;
		}
	}

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x06001F73 RID: 8051 RVA: 0x00018D41 File Offset: 0x00016F41
	public bool IsInitialized
	{
		get
		{
			return this.initialized;
		}
	}

	// Token: 0x06001F74 RID: 8052 RVA: 0x000ABD50 File Offset: 0x000A9F50
	public void Initialize(WaterSystem water, WaterRuntime runtime)
	{
		if (!Mathf.IsPowerOfTwo(this.solverResolution))
		{
			Debug.LogWarning("[Water] Simulation solver resolution much be a power-of-two.");
		}
		if (this.solverResolution > 512)
		{
			Debug.LogWarning("[Water] Simulation has a max solver resolution of " + 512);
		}
		this.water = water;
		this.camera = runtime.Camera;
		this.UpdateSimulationParams();
		this.InitializeMaterials();
		this.InitializeTextures();
		this.InitializeFFT();
		this.BindMaterialProperties();
		this.LoadPerlinNoiseTexture();
		this.CheckCommandBuffer();
		this.playing = false;
		this.initialized = true;
	}

	// Token: 0x06001F75 RID: 8053 RVA: 0x00018D49 File Offset: 0x00016F49
	public void Destroy()
	{
		if (this.initialized)
		{
			this.CleanupCommandBuffer();
			this.DestroyFFT();
			this.DestroyTextures();
			this.DestroyMaterials();
			this.playing = false;
			this.initialized = false;
		}
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x000ABDE8 File Offset: 0x000A9FE8
	private void CheckCommandBuffer()
	{
		if (this.initialized && this.camera != null)
		{
			if (this.commandBufferManager == null)
			{
				this.commandBufferManager = this.camera.GetComponent<CommandBufferManager>();
			}
			if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
			{
				this.commandBufferDesc = ((this.commandBufferDesc == null) ? new CommandBufferDesc(CameraEvent.BeforeGBuffer, 0, new CommandBufferDesc.FillCommandBuffer(this.FillCommandBuffer)) : this.commandBufferDesc);
				this.commandBufferManager.AddCommands(this.commandBufferDesc);
			}
		}
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x00018D79 File Offset: 0x00016F79
	private void CleanupCommandBuffer()
	{
		if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
		{
			this.commandBufferManager.RemoveCommands(this.commandBufferDesc);
			this.commandBufferManager = null;
		}
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x00018DAE File Offset: 0x00016FAE
	private void FillCommandBuffer(CommandBuffer cb)
	{
		if (this.playing)
		{
			this.Disperse(cb);
			this.Transform(cb, this.hTilde_hMap);
			this.Transform(cb, this.hTilde_dxdzMap);
			this.UpdateTextures(cb);
		}
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x000ABE84 File Offset: 0x000AA084
	private void SafeDestroy<T>(ref T obj) where T : Object
	{
		if (obj != null)
		{
			if (obj.GetType() == typeof(RenderTexture))
			{
				(obj as RenderTexture).Release();
			}
			Object.DestroyImmediate(obj);
			obj = default(T);
		}
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x00018DE0 File Offset: 0x00016FE0
	private void InitializeMaterials()
	{
		this.computeNormalFoldMat = new Material(Shader.Find("Hidden/Water/ComputeNormalFold"))
		{
			hideFlags = HideFlags.DontSave
		};
		this.simulationMat = new Material(Shader.Find("Hidden/Water/Simulation"))
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x00018E1C File Offset: 0x0001701C
	private void DestroyMaterials()
	{
		this.SafeDestroy<Material>(ref this.computeNormalFoldMat);
		this.SafeDestroy<Material>(ref this.simulationMat);
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x000ABEF0 File Offset: 0x000AA0F0
	private int ReverseBits(int value, int bitCount)
	{
		int num = 0;
		for (int i = 0; i < bitCount; i++)
		{
			num = (num << 1) + (value & 1);
			value >>= 1;
		}
		return num;
	}

	// Token: 0x06001F7D RID: 8061 RVA: 0x000ABF1C File Offset: 0x000AA11C
	private WaterSimulation.Butterfly[] CreateButterflyTable(int res, int numButterflies)
	{
		WaterSimulation.Butterfly[] array = new WaterSimulation.Butterfly[res * numButterflies];
		for (int i = 0; i < numButterflies; i++)
		{
			int num = 1 << numButterflies - 1 - i;
			int num2 = 1 << i;
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < num2; k++)
				{
					int num3;
					int num4;
					int i2;
					int j2;
					if (i == 0)
					{
						num3 = j * num2 * 2 + k;
						num4 = j * num2 * 2 + num2 + k;
						i2 = this.ReverseBits(num3, numButterflies);
						j2 = this.ReverseBits(num4, numButterflies);
					}
					else
					{
						num3 = j * num2 * 2 + k;
						num4 = j * num2 * 2 + num2 + k;
						i2 = num3;
						j2 = num4;
					}
					float num5 = Mathf.Cos(6.2831855f * (float)(k * num) / (float)res);
					float num6 = Mathf.Sin(6.2831855f * (float)(k * num) / (float)res);
					int num7 = num3 + i * res;
					int num8 = num4 + i * res;
					array[num7].i = i2;
					array[num7].j = j2;
					array[num7].wr = num5;
					array[num7].wi = num6;
					array[num8].i = i2;
					array[num8].j = j2;
					array[num8].wr = -num5;
					array[num8].wi = -num6;
				}
			}
		}
		return array;
	}

	// Token: 0x06001F7E RID: 8062 RVA: 0x000AC08C File Offset: 0x000AA28C
	private RenderTexture CreateRenderTexture(string name, int width, int height, RenderTextureFormat format, TextureWrapMode wrap, FilterMode filter)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, format, RenderTextureReadWrite.Linear);
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.name = name;
		renderTexture.wrapMode = wrap;
		renderTexture.filterMode = filter;
		renderTexture.anisoLevel = ((filter == FilterMode.Trilinear) ? 4 : 1);
		renderTexture.useMipMap = (filter == FilterMode.Trilinear);
		renderTexture.autoGenerateMips = (filter == FilterMode.Trilinear);
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x06001F7F RID: 8063 RVA: 0x00018E36 File Offset: 0x00017036
	private Texture2D CreateTexture(string name, int width, int height, TextureFormat format, TextureWrapMode wrap, FilterMode filter)
	{
		return new Texture2D(width, height, format, filter == FilterMode.Trilinear, true)
		{
			hideFlags = HideFlags.DontSave,
			name = name,
			wrapMode = wrap,
			filterMode = filter,
			anisoLevel = ((filter == FilterMode.Trilinear) ? 4 : 1)
		};
	}

	// Token: 0x06001F80 RID: 8064 RVA: 0x000AC0F8 File Offset: 0x000AA2F8
	private void InitializeTextures()
	{
		this.displacementMap = this.CreateRenderTexture("Water.DispMapGPU", this.solverResolution, this.solverResolution, RenderTextureFormat.ARGBHalf, TextureWrapMode.Repeat, FilterMode.Trilinear);
		this.displacementMapTexelSize = new Vector4(1f / (float)this.solverResolution, 1f / (float)this.solverResolution, (float)this.solverResolution, (float)this.solverResolution);
		RenderTextureFormat format = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB2101010) ? RenderTextureFormat.ARGB2101010 : RenderTextureFormat.ARGBHalf;
		this.normalFoldMap = this.CreateRenderTexture("Water.NormalFoldMap", this.solverResolution, this.solverResolution, format, TextureWrapMode.Repeat, FilterMode.Trilinear);
		this.perlinNoiseMap = this.CreateTexture("Water.PerlinMap", 64, 64, TextureFormat.RGBAHalf, TextureWrapMode.Repeat, FilterMode.Trilinear);
		this.hTilde0Map = this.CreateTexture("Water.hTilde0Map", this.solverResolution, this.solverResolution, TextureFormat.RGBAHalf, TextureWrapMode.Clamp, FilterMode.Point);
		this.dispersionMap = this.CreateTexture("Water.Dispersion", this.solverResolution, this.solverResolution, TextureFormat.RFloat, TextureWrapMode.Clamp, FilterMode.Point);
		this.butterflyMap = this.CreateTexture("Water.ButterflyMap", this.solverResolution, this.solverButterflyCount, TextureFormat.RGBAHalf, TextureWrapMode.Clamp, FilterMode.Point);
		this.hTilde_hMap = this.CreateRenderTexture("Water.hTilde_h", this.solverResolution, this.solverResolution, RenderTextureFormat.RGHalf, TextureWrapMode.Clamp, FilterMode.Point);
		this.hTilde_dxdzMap = this.CreateRenderTexture("Water.hTilde_dxdz", this.solverResolution, this.solverResolution, RenderTextureFormat.ARGBHalf, TextureWrapMode.Clamp, FilterMode.Point);
	}

	// Token: 0x06001F81 RID: 8065 RVA: 0x000AC244 File Offset: 0x000AA444
	private void DestroyTextures()
	{
		RenderTexture.active = null;
		this.SafeDestroy<RenderTexture>(ref this.displacementMap);
		this.SafeDestroy<RenderTexture>(ref this.normalFoldMap);
		this.SafeDestroy<Texture2D>(ref this.perlinNoiseMap);
		this.SafeDestroy<Texture2D>(ref this.hTilde0Map);
		this.SafeDestroy<Texture2D>(ref this.dispersionMap);
		this.SafeDestroy<Texture2D>(ref this.butterflyMap);
		this.SafeDestroy<RenderTexture>(ref this.hTilde_hMap);
		this.SafeDestroy<RenderTexture>(ref this.hTilde_dxdzMap);
	}

	// Token: 0x06001F82 RID: 8066 RVA: 0x000AC2B8 File Offset: 0x000AA4B8
	private void InitializeFFT()
	{
		Color[] array = new Color[this.solverResolution * this.solverResolution];
		Color[] array2 = new Color[this.solverResolution * this.solverResolution];
		Color[] array3 = new Color[this.solverResolution * this.solverButterflyCount];
		WaterSimulation.Butterfly[] array4 = this.CreateButterflyTable(this.solverResolution, this.solverButterflyCount);
		Random.State state = Random.state;
		Random.InitState(49649);
		int i = 0;
		int num = 0;
		while (i < this.solverResolution)
		{
			int j = 0;
			while (j < this.solverResolution)
			{
				Vector2 vector = this.hTilde_0(j, i);
				Vector2 vector2 = this.hTilde_0(-j, -i);
				float r = this.Dispersion(j, i);
				array[num] = new Color(vector.x, vector.y, vector2.x, -vector2.y);
				array2[num] = new Color(r, 0f, 0f, 0f);
				j++;
				num++;
			}
			i++;
		}
		int k = 0;
		int num2 = 0;
		while (k < this.solverButterflyCount)
		{
			int l = 0;
			while (l < this.solverResolution)
			{
				float r2 = ((float)array4[num2].i + 0.5f) / (float)this.solverResolution;
				float g = ((float)array4[num2].j + 0.5f) / (float)this.solverResolution;
				float wr = array4[num2].wr;
				float wi = array4[num2].wi;
				array3[num2] = new Color(r2, g, wr, wi);
				l++;
				num2++;
			}
			k++;
		}
		Random.state = state;
		this.hTilde0Map.SetPixels(array);
		this.hTilde0Map.Apply(false);
		this.dispersionMap.SetPixels(array2);
		this.dispersionMap.Apply(false);
		this.butterflyMap.SetPixels(array3);
		this.butterflyMap.Apply(false);
	}

	// Token: 0x06001F83 RID: 8067 RVA: 0x00002ECE File Offset: 0x000010CE
	private void DestroyFFT()
	{
	}

	// Token: 0x06001F84 RID: 8068 RVA: 0x000AC4C8 File Offset: 0x000AA6C8
	private void BindMaterialProperties()
	{
		this.simulationMat.SetTexture("_hTilde0Map", this.hTilde0Map);
		this.simulationMat.SetTexture("_DispersionMap", this.dispersionMap);
		this.simulationMat.SetTexture("_ButterflyMap", this.butterflyMap);
		this.simulationMat.SetVector("_SolverResolution", new Vector2((float)this.solverResolution, 1f / (float)this.solverResolution));
		this.simulationMat.SetVector("_SolverSizeInWorld", new Vector2(this.solverSizeInWorld, 1f / this.solverSizeInWorld));
		this.simulationMat.SetTexture("_hTilde_hMap", this.hTilde_hMap);
		this.simulationMat.SetTexture("_hTilde_dxdzMap", this.hTilde_dxdzMap);
	}

	// Token: 0x06001F85 RID: 8069 RVA: 0x000AC5A0 File Offset: 0x000AA7A0
	private void LoadPerlinNoiseTexture()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("PerlinNoise");
		MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		binaryReader.ReadBytes(128);
		Texture2D texture2D = this.CreateTexture("tempTex", 64, 64, TextureFormat.RGBAFloat, TextureWrapMode.Repeat, FilterMode.Trilinear);
		Color[] array = new Color[4096];
		for (int i = 63; i >= 0; i--)
		{
			int num = i * 64;
			int j = 0;
			while (j < 64)
			{
				array[num].r = Mathf.HalfToFloat(binaryReader.ReadUInt16());
				array[num].g = Mathf.HalfToFloat(binaryReader.ReadUInt16());
				array[num].b = Mathf.HalfToFloat(binaryReader.ReadUInt16());
				array[num].a = Mathf.HalfToFloat(binaryReader.ReadUInt16());
				j++;
				num++;
			}
		}
		texture2D.SetPixels(array);
		texture2D.Apply();
		int k = 64;
		int num2 = 0;
		while (k > 0)
		{
			this.perlinNoiseMap.SetPixels(texture2D.GetPixels(num2), num2);
			k >>= 1;
			num2++;
		}
		this.perlinNoiseMap.Apply(false);
		memoryStream.Close();
		binaryReader.Close();
		this.SafeDestroy<Texture2D>(ref texture2D);
		Resources.UnloadAsset(textAsset);
	}

	// Token: 0x06001F86 RID: 8070 RVA: 0x000AC6F4 File Offset: 0x000AA8F4
	public bool CheckLostData()
	{
		return (this.displacementMap != null && !this.displacementMap.IsCreated()) || (this.normalFoldMap != null && !this.normalFoldMap.IsCreated()) || (this.hTilde_hMap != null && !this.hTilde_hMap.IsCreated()) || (this.hTilde_dxdzMap != null && !this.hTilde_dxdzMap.IsCreated());
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x000AC778 File Offset: 0x000AA978
	private void UpdateSimulationParams()
	{
		int num = Mathf.Clamp(Mathf.NextPowerOfTwo(this.water.Simulation.SolverResolution), 2, 512);
		if (num != this.solverResolution)
		{
			this.DestroyTextures();
			this.DestroyFFT();
			this.solverResolution = num;
			this.InitializeTextures();
			this.InitializeFFT();
		}
		this.solverSizeInWorld = Mathf.Max(this.solverSizeInWorld, 1f);
		this.gravity = this.water.Simulation.Gravity;
		this.amplitude = this.water.Simulation.Amplitude;
		this.solverButterflyCount = (int)(Mathf.Log((float)this.solverResolution) / Mathf.Log(2f));
		Vector2 vector = new Vector2(this.water.Simulation.Wind.x, this.water.Simulation.Wind.z);
		this.windDirection = vector.normalized;
		this.windMagnitude = vector.magnitude;
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x00018E75 File Offset: 0x00017075
	public void Update()
	{
		if (this.initialized)
		{
			this.UpdateSimulationParams();
			this.BindMaterialProperties();
			this.CheckCommandBuffer();
		}
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x00018E91 File Offset: 0x00017091
	public void Play()
	{
		this.playing = true;
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x00018E9A File Offset: 0x0001709A
	public void Stop()
	{
		this.playing = false;
	}

	// Token: 0x06001F8B RID: 8075 RVA: 0x000AC87C File Offset: 0x000AAA7C
	private float Dispersion(int n_prime, int m_prime)
	{
		float num = 0.03141593f;
		float num2 = 3.1415927f * (float)(2 * n_prime - this.solverResolution) / this.solverSizeInWorld;
		float num3 = 3.1415927f * (float)(2 * m_prime - this.solverResolution) / this.solverSizeInWorld;
		return Mathf.Floor(Mathf.Sqrt(this.gravity * Mathf.Sqrt(num2 * num2 + num3 * num3)) / num) * num;
	}

	// Token: 0x06001F8C RID: 8076 RVA: 0x000AC8E4 File Offset: 0x000AAAE4
	private float Phillips(int n_prime, int m_prime)
	{
		Vector2 vector = new Vector2(3.1415927f * (float)(2 * n_prime - this.solverResolution) / this.solverSizeInWorld, 3.1415927f * (float)(2 * m_prime - this.solverResolution) / this.solverSizeInWorld);
		float magnitude = vector.magnitude;
		if (magnitude < 1E-06f)
		{
			return 0f;
		}
		float num = magnitude * magnitude;
		float num2 = num * num;
		float num3 = Vector2.Dot(vector.normalized, this.windDirection);
		float num4 = num3 * num3;
		float num5 = this.windMagnitude;
		float num6 = num5 * num5 / this.gravity;
		float num7 = num6 * num6;
		float num8 = 0.001f;
		float num9 = num7 * num8 * num8;
		return this.amplitude * Mathf.Exp(-1f / (num * num7)) / num2 * num4 * Mathf.Exp(-num * num9);
	}

	// Token: 0x06001F8D RID: 8077 RVA: 0x000AC9A8 File Offset: 0x000AABA8
	private float Gauss()
	{
		float num = Random.value;
		float value = Random.value;
		num = ((num >= 1E-06f) ? num : 1E-06f);
		return Mathf.Sqrt(-2f * Mathf.Log(num)) * Mathf.Cos(6.2831855f * value);
	}

	// Token: 0x06001F8E RID: 8078 RVA: 0x00018EA3 File Offset: 0x000170A3
	private Vector2 hTilde_0(int n_prime, int m_prime)
	{
		return new Vector2(this.Gauss(), this.Gauss()) * Mathf.Sqrt(this.Phillips(n_prime, m_prime) / 2f);
	}

	// Token: 0x06001F8F RID: 8079 RVA: 0x00018ECE File Offset: 0x000170CE
	private void Disperse(CommandBuffer cb)
	{
		cb.Blit(null, this.hTilde_hMap, this.simulationMat, 0);
		cb.Blit(null, this.hTilde_dxdzMap, this.simulationMat, 1);
	}

	// Token: 0x06001F90 RID: 8080 RVA: 0x000AC9F0 File Offset: 0x000AABF0
	private void BlitOverride(CommandBuffer cb, RenderTexture source, RenderTargetIdentifier destination, Material material, int pass)
	{
		if (source != null)
		{
			cb.SetGlobalTexture("_Source", source);
			cb.SetGlobalVector("_Source_TexelSize", new Vector4(1f / (float)source.width, 1f / (float)source.height, (float)source.width, (float)source.height));
		}
		if (source != null)
		{
			cb.Blit(new RenderTargetIdentifier(source), destination, material, pass);
			return;
		}
		cb.Blit(null, destination, material, pass);
	}

	// Token: 0x06001F91 RID: 8081 RVA: 0x000ACA78 File Offset: 0x000AAC78
	private void Transform(CommandBuffer cb, RenderTexture data)
	{
		int num = this.water.Simulation.SolverResolution;
		int num2 = Shader.PropertyToID("_pingBuffer");
		int num3 = Shader.PropertyToID("_pongBuffer");
		cb.GetTemporaryRT(num2, num, num, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
		cb.GetTemporaryRT(num3, num, num, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
		bool flag = true;
		int num4 = this.solverButterflyCount;
		for (int i = 0; i < num4; i++)
		{
			RenderTargetIdentifier dest = flag ? num2 : num3;
			RenderTargetIdentifier renderTargetIdentifier = flag ? num3 : num2;
			flag = !flag;
			renderTargetIdentifier = ((i == 0) ? new RenderTargetIdentifier(data) : renderTargetIdentifier);
			cb.SetGlobalTexture("_Source", renderTargetIdentifier);
			cb.SetGlobalVector("_Source_TexelSize", new Vector4(1f / (float)num, 1f / (float)num, (float)num, (float)num));
			cb.SetGlobalFloat("_ButterflyPass", ((float)i + 0.5f) / (float)num4);
			cb.Blit(renderTargetIdentifier, dest, this.simulationMat, 2);
		}
		for (int j = 0; j < num4; j++)
		{
			RenderTargetIdentifier renderTargetIdentifier2 = flag ? num2 : num3;
			RenderTargetIdentifier renderTargetIdentifier3 = flag ? num3 : num2;
			flag = !flag;
			renderTargetIdentifier2 = ((j + 1 == num4) ? new RenderTargetIdentifier(data) : renderTargetIdentifier2);
			cb.SetGlobalTexture("_Source", renderTargetIdentifier3);
			cb.SetGlobalVector("_Source_TexelSize", new Vector4(1f / (float)num, 1f / (float)num, (float)num, (float)num));
			cb.SetGlobalFloat("_ButterflyPass", ((float)j + 0.5f) / (float)num4);
			cb.Blit(renderTargetIdentifier3, renderTargetIdentifier2, this.simulationMat, 3);
		}
		cb.ReleaseTemporaryRT(num2);
		cb.ReleaseTemporaryRT(num3);
	}

	// Token: 0x06001F92 RID: 8082 RVA: 0x00018F02 File Offset: 0x00017102
	private void UpdateTextures(CommandBuffer cb)
	{
		this.BlitOverride(cb, null, this.displacementMap, this.simulationMat, 4);
		this.BlitOverride(cb, this.displacementMap, this.normalFoldMap, this.computeNormalFoldMat, 0);
	}

	// Token: 0x06001F93 RID: 8083 RVA: 0x000ACC28 File Offset: 0x000AAE28
	public void ShowDebug()
	{
		float num = 0f;
		if (this.displacementMap != null)
		{
			GUI.DrawTexture(new Rect(0f, num, 256f, 256f), this.displacementMap, 0, false);
		}
		if (this.normalFoldMap != null)
		{
			GUI.DrawTexture(new Rect(256f, num, 256f, 256f), this.normalFoldMap, 0, false);
		}
		num += 256f;
		if (this.hTilde_hMap != null)
		{
			GUI.DrawTexture(new Rect(0f, num, 256f, 256f), this.hTilde_hMap, 0, false);
		}
		if (this.hTilde_dxdzMap != null)
		{
			GUI.DrawTexture(new Rect(256f, num, 256f, 256f), this.hTilde_dxdzMap, 0, false);
		}
		num += 256f;
		if (this.hTilde0Map != null)
		{
			GUI.DrawTexture(new Rect(0f, num, 256f, 256f), this.hTilde0Map, 0, false);
		}
		if (this.dispersionMap != null)
		{
			GUI.DrawTexture(new Rect(256f, num, 256f, 256f), this.dispersionMap, 0, false);
		}
		if (this.butterflyMap != null)
		{
			GUI.DrawTexture(new Rect(512f, num, 256f, 256f), this.butterflyMap, 0, false);
		}
		num += 256f;
	}

	// Token: 0x02000567 RID: 1383
	public struct Butterfly
	{
		// Token: 0x04001BAB RID: 7083
		public int i;

		// Token: 0x04001BAC RID: 7084
		public int j;

		// Token: 0x04001BAD RID: 7085
		public float wr;

		// Token: 0x04001BAE RID: 7086
		public float wi;
	}
}
