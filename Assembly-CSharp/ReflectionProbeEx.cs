using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020005AB RID: 1451
[ExecuteInEditMode]
public class ReflectionProbeEx : MonoBehaviour
{
	// Token: 0x04001D0E RID: 7438
	private static readonly uint[,] qualitySamples = new uint[,]
	{
		{
			6U,
			12U,
			16U,
			16U
		},
		{
			8U,
			16U,
			24U,
			32U
		},
		{
			16U,
			16U,
			32U,
			64U
		},
		{
			32U,
			32U,
			64U,
			128U
		},
		{
			64U,
			64U,
			128U,
			128U
		}
	};

	// Token: 0x04001D0F RID: 7439
	private static Vector4[] sampleData = new Vector4[128];

	// Token: 0x04001D10 RID: 7440
	public ReflectionProbeRefreshMode refreshMode = ReflectionProbeRefreshMode.EveryFrame;

	// Token: 0x04001D11 RID: 7441
	public bool timeSlicing;

	// Token: 0x04001D12 RID: 7442
	public int resolution = 128;

	// Token: 0x04001D13 RID: 7443
	[InspectorName("HDR")]
	public bool hdr = true;

	// Token: 0x04001D14 RID: 7444
	public float shadowDistance;

	// Token: 0x04001D15 RID: 7445
	public ReflectionProbeClearFlags clearFlags = ReflectionProbeClearFlags.Skybox;

	// Token: 0x04001D16 RID: 7446
	public Color background = new Color(0.192f, 0.301f, 0.474f);

	// Token: 0x04001D17 RID: 7447
	public float nearClip = 0.3f;

	// Token: 0x04001D18 RID: 7448
	public float farClip = 1000f;

	// Token: 0x04001D19 RID: 7449
	public Transform attachToTarget;

	// Token: 0x04001D1A RID: 7450
	public Light directionalLight;

	// Token: 0x04001D1B RID: 7451
	public float textureMipBias = 2f;

	// Token: 0x04001D1C RID: 7452
	public bool highPrecision;

	// Token: 0x04001D1D RID: 7453
	public ReflectionProbeEx.ConvolutionQuality convolutionQuality;

	// Token: 0x04001D1E RID: 7454
	public List<ReflectionProbeEx.RenderListEntry> staticRenderList = new List<ReflectionProbeEx.RenderListEntry>();

	// Token: 0x04001D1F RID: 7455
	public Cubemap reflectionCubemap;

	// Token: 0x04001D20 RID: 7456
	public float reflectionIntensity = 1f;

	// Token: 0x04001D21 RID: 7457
	private List<ReflectionProbeEx.RenderListEntry> dynamicRenderList = new List<ReflectionProbeEx.RenderListEntry>();

	// Token: 0x04001D22 RID: 7458
	private ReflectionProbe probe;

	// Token: 0x04001D23 RID: 7459
	private RenderTexture probeTexture;

	// Token: 0x04001D24 RID: 7460
	private int probeResolution = 256;

	// Token: 0x04001D25 RID: 7461
	private bool probeHdr;

	// Token: 0x04001D26 RID: 7462
	private float probeShadowDistance;

	// Token: 0x04001D27 RID: 7463
	private float probeNearClip = 1f;

	// Token: 0x04001D28 RID: 7464
	private float probeFarClip = 1000f;

	// Token: 0x04001D29 RID: 7465
	private const int probeDepth = 24;

	// Token: 0x04001D2A RID: 7466
	private bool probeHighPrecision;

	// Token: 0x04001D2B RID: 7467
	private ReflectionProbeEx.TimeSlicingState timeSlicedRenderState = ReflectionProbeEx.TimeSlicingState.Finished;

	// Token: 0x04001D2C RID: 7468
	private bool scriptingRenderQueued;

	// Token: 0x04001D2D RID: 7469
	private Matrix4x4 faceProjMatrix = Matrix4x4.identity;

	// Token: 0x04001D2E RID: 7470
	private Matrix4x4 faceProjInvMatrix = Matrix4x4.identity;

	// Token: 0x04001D2F RID: 7471
	private int prevFrame;

	// Token: 0x04001D30 RID: 7472
	private ReflectionProbeRefreshMode savedProbeRefresh;

	// Token: 0x04001D31 RID: 7473
	private ReflectionProbeMode savedProbeMode;

	// Token: 0x04001D32 RID: 7474
	private Texture savedProbeCustomTexture;

	// Token: 0x04001D33 RID: 7475
	private Mesh blitMesh;

	// Token: 0x04001D34 RID: 7476
	private Mesh skyboxMesh;

	// Token: 0x04001D35 RID: 7477
	private static float[] octaVerts = new float[]
	{
		0f,
		1f,
		0f,
		0f,
		0f,
		-1f,
		1f,
		0f,
		0f,
		0f,
		1f,
		0f,
		1f,
		0f,
		0f,
		0f,
		0f,
		1f,
		0f,
		1f,
		0f,
		0f,
		0f,
		1f,
		-1f,
		0f,
		0f,
		0f,
		1f,
		0f,
		-1f,
		0f,
		0f,
		0f,
		0f,
		-1f,
		0f,
		-1f,
		0f,
		1f,
		0f,
		0f,
		0f,
		0f,
		-1f,
		0f,
		-1f,
		0f,
		0f,
		0f,
		1f,
		1f,
		0f,
		0f,
		0f,
		-1f,
		0f,
		-1f,
		0f,
		0f,
		0f,
		0f,
		1f,
		0f,
		-1f,
		0f,
		0f,
		0f,
		-1f,
		-1f,
		0f,
		0f
	};

	// Token: 0x04001D36 RID: 7478
	private static readonly ReflectionProbeEx.CubemapFaceMatrices[] cubemapFaceMatrices = new ReflectionProbeEx.CubemapFaceMatrices[]
	{
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f))
	};

	// Token: 0x04001D37 RID: 7479
	private static readonly ReflectionProbeEx.CubemapFaceMatrices[] cubemapFaceMatricesD3D11 = new ReflectionProbeEx.CubemapFaceMatrices[]
	{
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f), new Vector3(-1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, -1f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f))
	};

	// Token: 0x04001D38 RID: 7480
	private static readonly ReflectionProbeEx.CubemapFaceMatrices[] shadowCubemapFaceMatrices = new ReflectionProbeEx.CubemapFaceMatrices[]
	{
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f)),
		new ReflectionProbeEx.CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f))
	};

	// Token: 0x04001D39 RID: 7481
	private ReflectionProbeEx.CubemapFaceMatrices[] platformCubemapFaceMatrices;

	// Token: 0x04001D3A RID: 7482
	private static readonly HashSet<string> supportedShaderNames;

	// Token: 0x04001D3B RID: 7483
	private static Dictionary<Shader, Shader> supportedShaders;

	// Token: 0x04001D3C RID: 7484
	private static Dictionary<Material, Material> matchingMaterials;

	// Token: 0x04001D3D RID: 7485
	private RenderTexture probeTempTexture;

	// Token: 0x04001D3E RID: 7486
	private RenderTexture probeShadowTexture;

	// Token: 0x04001D3F RID: 7487
	private RenderTexture arrayTexture;

	// Token: 0x04001D40 RID: 7488
	private RenderTexture arrayTempTexture;

	// Token: 0x04001D41 RID: 7489
	private RenderTexture arrayDepthTexture;

	// Token: 0x04001D42 RID: 7490
	private int mipmapCount;

	// Token: 0x04001D43 RID: 7491
	private Material blitMaterial;

	// Token: 0x04001D44 RID: 7492
	private Material filterMaterial;

	// Token: 0x04001D45 RID: 7493
	private Material shadowMaterial;

	// Token: 0x04001D46 RID: 7494
	private CommandBuffer forwardCB;

	// Token: 0x04001D47 RID: 7495
	private CommandBuffer shadowCB;

	// Token: 0x04001D48 RID: 7496
	private Matrix4x4[] viewProjMatrixArray = new Matrix4x4[6];

	// Token: 0x04001D49 RID: 7497
	private Matrix4x4[] objectToWorldArray = new Matrix4x4[6];

	// Token: 0x04001D4A RID: 7498
	private Matrix4x4[] cameraToWorldArray = new Matrix4x4[6];

	// Token: 0x04001D4B RID: 7499
	private bool useGeometryShader = true;

	// Token: 0x04001D4C RID: 7500
	private int PassCount = 1;

	// Token: 0x04001D4D RID: 7501
	private static readonly int[] tab32;

	// Token: 0x06002152 RID: 8530 RVA: 0x000B3704 File Offset: 0x000B1904
	private Vector2 Hammersley(uint index, uint numSamples)
	{
		float x = index / numSamples;
		float y = this.ReverseBits(index) * 2.3283064E-10f;
		return new Vector2(x, y);
	}

	// Token: 0x06002153 RID: 8531 RVA: 0x000B3730 File Offset: 0x000B1930
	private float D_GGX(float roughness, float NdotH)
	{
		float num = roughness * roughness;
		float num2 = num * num;
		float num3 = (NdotH * num2 - NdotH) * NdotH + 1f;
		return num2 / (3.1415927f * num3 * num3);
	}

	// Token: 0x06002154 RID: 8532 RVA: 0x000B375C File Offset: 0x000B195C
	private Vector3 ImportanceSampleGGX(Vector2 Xi, float roughness)
	{
		float num = roughness * roughness;
		float num2 = num * num;
		float f = 6.2831855f * Xi.x;
		float num3 = Mathf.Sqrt((1f - Xi.y) / (1f + (num2 - 1f) * Xi.y));
		float num4 = Mathf.Sqrt(1f - num3 * num3);
		Vector3 result;
		result.x = num4 * Mathf.Cos(f);
		result.y = num4 * Mathf.Sin(f);
		result.z = num3;
		return result;
	}

	// Token: 0x06002155 RID: 8533 RVA: 0x000B37DC File Offset: 0x000B19DC
	private void ApplyConvolution(Texture target)
	{
		float num = (float)(this.probeResolution * this.probeResolution);
		for (int i = 1; i < this.mipmapCount; i++)
		{
			uint num2 = ReflectionProbeEx.qualitySamples[(int)this.convolutionQuality, Mathf.Min(i - 1, 3)];
			int num3 = 0;
			float roughness = (float)i / (float)this.mipmapCount;
			Vector3 vector = new Vector3(0f, 0f, 1f);
			for (uint num4 = 0U; num4 < num2; num4 += 1U)
			{
				Vector2 xi = this.Hammersley(num4, num2);
				Vector3 vector2 = this.ImportanceSampleGGX(xi, roughness);
				Vector3 vector3 = 2f * Vector3.Dot(vector, vector2) * vector2 - vector;
				float num5 = Mathf.Clamp01(Vector3.Dot(vector, vector3));
				if (num5 > 0.001f)
				{
					float num6 = Mathf.Clamp01(Vector3.Dot(vector, vector2));
					float num7 = this.D_GGX(roughness, num6) * num6 / (4f * num6);
					float num8 = 1f / (num2 * num7);
					float num9 = 12.566371f / (6f * num);
					float num10 = 1f;
					float w = Mathf.Max(0.5f * Mathf.Log(num8 / num9, 2f) + num10, 0f);
					float d = num5 - Mathf.Floor(num5);
					vector3 = vector3.normalized * d;
					ReflectionProbeEx.sampleData[num3] = new Vector4(vector3.x, vector3.y, vector3.z, w);
					num3++;
				}
			}
			this.forwardCB.SetGlobalVectorArray("_SampleData", ReflectionProbeEx.sampleData);
			this.forwardCB.SetGlobalFloat("_SampleCount", (float)num3);
			this.forwardCB.SetRenderTarget(target, i, CubemapFace.PositiveX, -1);
			for (int j = 0; j < this.PassCount; j++)
			{
				int slice = this.useGeometryShader ? -1 : j;
				this.forwardCB.BlitArray(this.blitMesh, this.probeTempTexture, this.filterMaterial, slice, 0);
			}
		}
	}

	// Token: 0x17000235 RID: 565
	// (get) Token: 0x06002156 RID: 8534 RVA: 0x0001A836 File Offset: 0x00018A36
	public RenderTexture Texture
	{
		get
		{
			return this.probeTexture;
		}
	}

	// Token: 0x06002157 RID: 8535 RVA: 0x0001A83E File Offset: 0x00018A3E
	public void ClearRenderList()
	{
		this.dynamicRenderList.Clear();
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x0001A84B File Offset: 0x00018A4B
	public void AddToRenderList(Renderer renderer, bool alwaysEnabled = false)
	{
		this.dynamicRenderList.Add(new ReflectionProbeEx.RenderListEntry(renderer, alwaysEnabled));
		this.RegisterMaterialReplacement(renderer.sharedMaterial);
	}

	// Token: 0x06002159 RID: 8537 RVA: 0x0001A86C File Offset: 0x00018A6C
	private void Awake()
	{
		this.probe = base.GetComponent<ReflectionProbe>();
	}

	// Token: 0x0600215A RID: 8538 RVA: 0x000B39F0 File Offset: 0x000B1BF0
	private void OnEnable()
	{
		if (!this.InitializeCubemapFaceMatrices())
		{
			base.enabled = false;
			return;
		}
		this.UpdateProperties();
		this.CreateMeshes();
		this.CreateTextures();
		this.CreateMaterials();
		this.CreateCommandBuffers();
		this.AttachToLight();
		this.ModifyProbeProperties();
		if (this.refreshMode == ReflectionProbeRefreshMode.OnAwake)
		{
			this.RenderProbe();
		}
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(this.OnCameraPreRender));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(this.OnCameraPreRender));
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x000B3A88 File Offset: 0x000B1C88
	private void OnDisable()
	{
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(this.OnCameraPreRender));
		this.DetachFromLight();
		this.DestroyMeshes();
		this.DestroyTextures();
		this.DestroyMaterials();
		this.DestroyCommandBuffers();
		this.RestoreProbeSettings();
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x000B3ADC File Offset: 0x000B1CDC
	private void OnCameraPreRender(Camera cam)
	{
		if (cam == MainCamera.mainCamera && this.prevFrame != Time.frameCount)
		{
			this.prevFrame = Time.frameCount;
			if (this.attachToTarget != null)
			{
				base.transform.position = this.attachToTarget.transform.position;
			}
			if (this.UpdateProperties())
			{
				this.DestroyTextures();
				this.CreateTextures();
				if (this.probe != null)
				{
					this.probe.customBakedTexture = this.probeTexture;
				}
			}
			this.ClearCommandBuffers();
			if (this.refreshMode == ReflectionProbeRefreshMode.EveryFrame && (!this.timeSlicing || (this.timeSlicing && this.timeSlicedRenderState == ReflectionProbeEx.TimeSlicingState.Finished)))
			{
				this.scriptingRenderQueued = true;
				this.timeSlicedRenderState = (this.timeSlicing ? ReflectionProbeEx.TimeSlicingState.Shadow : ReflectionProbeEx.TimeSlicingState.Finished);
			}
			if (this.scriptingRenderQueued)
			{
				if (this.timeSlicing || this.timeSlicedRenderState != ReflectionProbeEx.TimeSlicingState.Finished)
				{
					if (this.timeSlicedRenderState != ReflectionProbeEx.TimeSlicingState.Finished)
					{
						this.PrepareTimeSlicedRender(this.timeSlicedRenderState);
						this.timeSlicedRenderState++;
						this.ExecuteRender();
					}
					if (this.timeSlicedRenderState == ReflectionProbeEx.TimeSlicingState.Finished)
					{
						this.scriptingRenderQueued = false;
						return;
					}
				}
				else
				{
					this.PrepareFullRender();
					this.ExecuteRender();
					this.scriptingRenderQueued = false;
				}
			}
		}
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x0001A87A File Offset: 0x00018A7A
	public void RenderProbe()
	{
		this.scriptingRenderQueued = true;
		this.timeSlicedRenderState = (this.timeSlicing ? ReflectionProbeEx.TimeSlicingState.Shadow : ReflectionProbeEx.TimeSlicingState.Finished);
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x0001A895 File Offset: 0x00018A95
	public bool IsFinishedRendering()
	{
		return !this.timeSlicing || this.timeSlicedRenderState == ReflectionProbeEx.TimeSlicingState.Finished;
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x000B3C1C File Offset: 0x000B1E1C
	private void ModifyProbeProperties()
	{
		if (this.probe != null)
		{
			this.savedProbeRefresh = this.probe.refreshMode;
			this.savedProbeMode = this.probe.mode;
			this.savedProbeCustomTexture = this.probe.customBakedTexture;
			this.probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
			this.probe.mode = ReflectionProbeMode.Custom;
			this.probe.customBakedTexture = this.probeTexture;
		}
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x000B3C94 File Offset: 0x000B1E94
	private void RestoreProbeSettings()
	{
		if (this.probe != null)
		{
			this.probe.refreshMode = this.savedProbeRefresh;
			this.probe.mode = this.savedProbeMode;
			this.probe.customBakedTexture = this.savedProbeCustomTexture;
		}
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x000B3CE4 File Offset: 0x000B1EE4
	private bool UpdateProperties()
	{
		if (this.probe != null)
		{
			this.resolution = this.probe.resolution;
			this.hdr = this.probe.hdr;
			this.shadowDistance = this.probe.shadowDistance;
			this.clearFlags = this.probe.clearFlags;
			this.background = this.probe.backgroundColor;
			this.nearClip = this.probe.nearClipPlane;
			this.farClip = this.probe.farClipPlane;
		}
		if (this.resolution != this.probeResolution || this.hdr != this.probeHdr || this.nearClip != this.probeNearClip || this.farClip != this.probeFarClip || this.highPrecision != this.probeHighPrecision || this.shadowDistance != this.probeShadowDistance)
		{
			this.probeResolution = this.resolution;
			this.probeHdr = this.hdr;
			this.probeShadowDistance = this.shadowDistance;
			this.probeNearClip = this.nearClip;
			this.probeFarClip = this.farClip;
			this.probeHighPrecision = this.highPrecision;
			this.faceProjMatrix = Matrix4x4.Perspective(90f, 1f, this.probeNearClip, this.probeFarClip);
			this.faceProjInvMatrix = this.faceProjMatrix.inverse;
			return true;
		}
		return false;
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x0001A8AA File Offset: 0x00018AAA
	private void CreateMeshes()
	{
		if (this.blitMesh == null)
		{
			this.blitMesh = ReflectionProbeEx.CreateBlitMesh();
		}
		if (this.skyboxMesh == null)
		{
			this.skyboxMesh = ReflectionProbeEx.CreateSkyboxMesh();
		}
	}

	// Token: 0x06002163 RID: 8547 RVA: 0x000B3E48 File Offset: 0x000B2048
	private void DestroyMeshes()
	{
		if (this.blitMesh != null)
		{
			Object.DestroyImmediate(this.blitMesh);
			this.blitMesh = null;
		}
		if (this.skyboxMesh != null)
		{
			Object.DestroyImmediate(this.skyboxMesh);
			this.skyboxMesh = null;
		}
	}

	// Token: 0x06002164 RID: 8548 RVA: 0x000B3E98 File Offset: 0x000B2098
	private static Mesh CreateBlitMesh()
	{
		return new Mesh
		{
			vertices = new Vector3[]
			{
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f, 1f, 0f),
				new Vector3(1f, 1f, 0f),
				new Vector3(1f, -1f, 0f)
			},
			uv = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f)
			},
			triangles = new int[]
			{
				0,
				1,
				2,
				0,
				2,
				3
			}
		};
	}

	// Token: 0x06002165 RID: 8549 RVA: 0x000B3FA0 File Offset: 0x000B21A0
	private static ReflectionProbeEx.CubemapSkyboxVertex SubDivVert(ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2)
	{
		Vector3 a = new Vector3(v1.x, v1.y, v1.z);
		Vector3 b = new Vector3(v2.x, v2.y, v2.z);
		Vector3 vector = Vector3.Normalize(Vector3.Lerp(a, b, 0.5f));
		ReflectionProbeEx.CubemapSkyboxVertex result;
		result.x = (result.tu = vector.x);
		result.y = (result.tv = vector.y);
		result.z = (result.tw = vector.z);
		result.color = Color.white;
		return result;
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x000B4040 File Offset: 0x000B2240
	private static void Subdivide(List<ReflectionProbeEx.CubemapSkyboxVertex> destArray, ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2, ReflectionProbeEx.CubemapSkyboxVertex v3)
	{
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex = ReflectionProbeEx.SubDivVert(v1, v2);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex2 = ReflectionProbeEx.SubDivVert(v2, v3);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex3 = ReflectionProbeEx.SubDivVert(v1, v3);
		destArray.Add(v1);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(v2);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(v3);
		destArray.Add(cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex2);
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x000B40BC File Offset: 0x000B22BC
	private static void SubdivideYOnly(List<ReflectionProbeEx.CubemapSkyboxVertex> destArray, ReflectionProbeEx.CubemapSkyboxVertex v1, ReflectionProbeEx.CubemapSkyboxVertex v2, ReflectionProbeEx.CubemapSkyboxVertex v3)
	{
		float num = Mathf.Abs(v2.y - v1.y);
		float num2 = Mathf.Abs(v2.y - v3.y);
		float num3 = Mathf.Abs(v3.y - v1.y);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex;
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex2;
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex3;
		if (num < num2 && num < num3)
		{
			cubemapSkyboxVertex = v3;
			cubemapSkyboxVertex2 = v1;
			cubemapSkyboxVertex3 = v2;
		}
		else if (num2 < num && num2 < num3)
		{
			cubemapSkyboxVertex = v1;
			cubemapSkyboxVertex2 = v2;
			cubemapSkyboxVertex3 = v3;
		}
		else
		{
			cubemapSkyboxVertex = v2;
			cubemapSkyboxVertex2 = v3;
			cubemapSkyboxVertex3 = v1;
		}
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex4 = ReflectionProbeEx.SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex2);
		ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex5 = ReflectionProbeEx.SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(cubemapSkyboxVertex4);
		destArray.Add(cubemapSkyboxVertex5);
		Vector3 vector = new Vector3(cubemapSkyboxVertex5.x - cubemapSkyboxVertex2.x, cubemapSkyboxVertex5.y - cubemapSkyboxVertex2.y, cubemapSkyboxVertex5.z - cubemapSkyboxVertex2.z);
		Vector3 vector2 = new Vector3(cubemapSkyboxVertex4.x - cubemapSkyboxVertex3.x, cubemapSkyboxVertex4.y - cubemapSkyboxVertex3.y, cubemapSkyboxVertex4.z - cubemapSkyboxVertex3.z);
		if (vector.x * vector.x + vector.y * vector.y + vector.z * vector.z > vector2.x * vector2.x + vector2.y * vector2.y + vector2.z * vector2.z)
		{
			destArray.Add(cubemapSkyboxVertex4);
			destArray.Add(cubemapSkyboxVertex2);
			destArray.Add(cubemapSkyboxVertex3);
			destArray.Add(cubemapSkyboxVertex5);
			destArray.Add(cubemapSkyboxVertex4);
			destArray.Add(cubemapSkyboxVertex3);
			return;
		}
		destArray.Add(cubemapSkyboxVertex5);
		destArray.Add(cubemapSkyboxVertex4);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex5);
		destArray.Add(cubemapSkyboxVertex2);
		destArray.Add(cubemapSkyboxVertex3);
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x000B4284 File Offset: 0x000B2484
	private static Mesh CreateSkyboxMesh()
	{
		List<ReflectionProbeEx.CubemapSkyboxVertex> list = new List<ReflectionProbeEx.CubemapSkyboxVertex>();
		for (int i = 0; i < 24; i++)
		{
			ReflectionProbeEx.CubemapSkyboxVertex cubemapSkyboxVertex = default(ReflectionProbeEx.CubemapSkyboxVertex);
			Vector3 vector = Vector3.Normalize(new Vector3(ReflectionProbeEx.octaVerts[i * 3], ReflectionProbeEx.octaVerts[i * 3 + 1], ReflectionProbeEx.octaVerts[i * 3 + 2]));
			cubemapSkyboxVertex.x = (cubemapSkyboxVertex.tu = vector.x);
			cubemapSkyboxVertex.y = (cubemapSkyboxVertex.tv = vector.y);
			cubemapSkyboxVertex.z = (cubemapSkyboxVertex.tw = vector.z);
			cubemapSkyboxVertex.color = Color.white;
			list.Add(cubemapSkyboxVertex);
		}
		for (int j = 0; j < 3; j++)
		{
			List<ReflectionProbeEx.CubemapSkyboxVertex> list2 = new List<ReflectionProbeEx.CubemapSkyboxVertex>(list.Count);
			list2.AddRange(list);
			int count = list2.Count;
			list.Clear();
			list.Capacity = count * 4;
			for (int k = 0; k < count; k += 3)
			{
				ReflectionProbeEx.Subdivide(list, list2[k], list2[k + 1], list2[k + 2]);
			}
		}
		for (int l = 0; l < 2; l++)
		{
			List<ReflectionProbeEx.CubemapSkyboxVertex> list3 = new List<ReflectionProbeEx.CubemapSkyboxVertex>(list.Count);
			list3.AddRange(list);
			int count2 = list3.Count;
			float num = Mathf.Pow(0.5f, (float)l + 1f);
			list.Clear();
			list.Capacity = count2 * 4;
			for (int m = 0; m < count2; m += 3)
			{
				if (Mathf.Max(Mathf.Max(Mathf.Abs(list3[m].y), Mathf.Abs(list3[m + 1].y)), Mathf.Abs(list3[m + 2].y)) > num)
				{
					list.Add(list3[m]);
					list.Add(list3[m + 1]);
					list.Add(list3[m + 2]);
				}
				else
				{
					ReflectionProbeEx.SubdivideYOnly(list, list3[m], list3[m + 1], list3[m + 2]);
				}
			}
		}
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[list.Count];
		Vector2[] array2 = new Vector2[list.Count];
		int[] array3 = new int[list.Count];
		for (int n = 0; n < list.Count; n++)
		{
			array[n] = new Vector3(list[n].x, list[n].y, list[n].z);
			array2[n] = new Vector3(list[n].tu, list[n].tv);
			array3[n] = n;
		}
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		return mesh;
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x000B458C File Offset: 0x000B278C
	private bool InitializeCubemapFaceMatrices()
	{
		GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
		if (graphicsDeviceType != GraphicsDeviceType.Direct3D11)
		{
			switch (graphicsDeviceType)
			{
			case GraphicsDeviceType.Metal:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
				goto IL_75;
			case GraphicsDeviceType.OpenGLCore:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatrices;
				goto IL_75;
			case GraphicsDeviceType.Direct3D12:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
				goto IL_75;
			case GraphicsDeviceType.Vulkan:
				this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
				goto IL_75;
			}
			this.platformCubemapFaceMatrices = null;
		}
		else
		{
			this.platformCubemapFaceMatrices = ReflectionProbeEx.cubemapFaceMatricesD3D11;
		}
		IL_75:
		if (this.platformCubemapFaceMatrices == null)
		{
			Debug.LogError("[ReflectionProbeEx] Initialization failed. No cubemap ortho basis defined for " + SystemInfo.graphicsDeviceType);
			return false;
		}
		return true;
	}

	// Token: 0x0600216A RID: 8554 RVA: 0x000B4634 File Offset: 0x000B2834
	private void CreateTextures()
	{
		RenderTextureFormat format = RenderTextureFormat.ARGB32;
		RenderTextureReadWrite readWrite = RenderTextureReadWrite.sRGB;
		if (this.probeHdr)
		{
			if (this.probeHighPrecision)
			{
				format = RenderTextureFormat.ARGBHalf;
			}
			else
			{
				RenderTextureFormat renderTextureFormat = RenderTextureFormat.RGB111110Float;
				format = (SystemInfo.SupportsRenderTextureFormat(renderTextureFormat) ? renderTextureFormat : RenderTextureFormat.ARGBHalf);
			}
			readWrite = RenderTextureReadWrite.Linear;
		}
		int size = (this.directionalLight != null) ? this.probeResolution : 4;
		this.SafeCreateCubeRT(ref this.probeTexture, "Probex", this.probeResolution, 0, true, TextureDimension.Cube, FilterMode.Trilinear, format, readWrite);
		this.SafeCreateCubeRT(ref this.probeTempTexture, "ProbexTemp", this.probeResolution, 0, true, TextureDimension.Cube, FilterMode.Trilinear, format, readWrite);
		this.SafeCreateCubeRT(ref this.arrayTexture, "ProbexArray", this.probeResolution, 0, true, TextureDimension.Tex2DArray, FilterMode.Point, format, readWrite);
		this.SafeCreateCubeRT(ref this.arrayTempTexture, "ProbexArrayTemp", this.probeResolution, 0, true, TextureDimension.Tex2DArray, FilterMode.Point, format, readWrite);
		this.SafeCreateCubeRT(ref this.arrayDepthTexture, "ProbexArrayDepth", this.probeResolution, 16, false, TextureDimension.Tex2DArray, FilterMode.Point, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
		if (this.shadowDistance > 0f)
		{
			this.SafeCreateCubeRT(ref this.probeShadowTexture, "ProbexShadow", size, 0, false, TextureDimension.Cube, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			Graphics.SetRenderTarget(this.probeShadowTexture, 0, CubemapFace.PositiveX, -1);
			GL.Clear(false, true, Color.white);
		}
		else
		{
			this.probeShadowTexture = null;
		}
		this.mipmapCount = this.FastLog2(this.probeResolution) + 1;
		if (this.reflectionCubemap == null)
		{
			this.FindEnvironmentReflection();
		}
	}

	// Token: 0x0600216B RID: 8555 RVA: 0x000B4788 File Offset: 0x000B2988
	private void DestroyTextures()
	{
		RenderTexture.active = null;
		this.SafeDestroy<RenderTexture>(ref this.probeTexture);
		this.SafeDestroy<RenderTexture>(ref this.probeTempTexture);
		this.SafeDestroy<RenderTexture>(ref this.probeShadowTexture);
		this.SafeDestroy<RenderTexture>(ref this.arrayTexture);
		this.SafeDestroy<RenderTexture>(ref this.arrayTempTexture);
		this.SafeDestroy<RenderTexture>(ref this.arrayDepthTexture);
	}

	// Token: 0x0600216C RID: 8556 RVA: 0x000B47E4 File Offset: 0x000B29E4
	private Material RegisterMaterialReplacement(Material material)
	{
		Shader shader = material.shader;
		Shader shader2 = null;
		if (!ReflectionProbeEx.supportedShaders.TryGetValue(shader, ref shader2))
		{
			string name = shader.name;
			if (ReflectionProbeEx.supportedShaderNames.Contains(name))
			{
				string text = "Hidden/ReflectionProbeEx/" + name;
				shader2 = Shader.Find(text);
				if (shader2 != null)
				{
					ReflectionProbeEx.supportedShaders.Add(shader, shader2);
				}
				else
				{
					Debug.LogError("[ReflectionProbeEx] Replacement shader " + text + " not found.");
				}
			}
			else
			{
				Debug.LogError("[ReflectionProbeEx] Shader " + name + " not supported.");
			}
		}
		Material material2 = null;
		if (shader2 != null)
		{
			bool flag = ReflectionProbeEx.matchingMaterials.TryGetValue(material, ref material2);
			bool flag2 = flag && material2 == null;
			if (!flag || flag2)
			{
				if (flag2)
				{
					ReflectionProbeEx.matchingMaterials.Remove(material);
				}
				material2 = new Material(shader2);
				material2.CopyPropertiesFromMaterial(material);
				ReflectionProbeEx.matchingMaterials.Add(material, material2);
			}
		}
		return material2;
	}

	// Token: 0x0600216D RID: 8557 RVA: 0x000B48D0 File Offset: 0x000B2AD0
	private Material FindMaterialReplacement(Material material)
	{
		Material result = null;
		if (material != null && !ReflectionProbeEx.matchingMaterials.TryGetValue(material, ref result))
		{
			result = this.RegisterMaterialReplacement(material);
		}
		return result;
	}

	// Token: 0x0600216E RID: 8558 RVA: 0x000B4900 File Offset: 0x000B2B00
	private void CreateMaterials()
	{
		this.SafeCreateMaterial(ref this.blitMaterial, "Hidden/ReflectionProbeEx/SinglePassBlit");
		this.SafeCreateMaterial(ref this.filterMaterial, "Hidden/ReflectionProbeEx/SinglePassFilterEnvMap");
		this.SafeCreateMaterial(ref this.shadowMaterial, "Hidden/ReflectionProbeEx/SinglePassShadowMask");
		if (RenderSettings.skybox != null)
		{
			this.RegisterMaterialReplacement(RenderSettings.skybox);
		}
		foreach (ReflectionProbeEx.RenderListEntry renderListEntry in this.staticRenderList)
		{
			if (renderListEntry.renderer != null && renderListEntry.renderer.sharedMaterial != null)
			{
				this.RegisterMaterialReplacement(renderListEntry.renderer.sharedMaterial);
			}
		}
		this.useGeometryShader = (this.blitMaterial.GetTag("GeometryShaderPath", false, "").ToUpper() == "TRUE");
		this.PassCount = (this.useGeometryShader ? 1 : 6);
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x0001A8DE File Offset: 0x00018ADE
	private void DestroyMaterials()
	{
		this.SafeDestroy<Material>(ref this.blitMaterial);
		this.SafeDestroy<Material>(ref this.filterMaterial);
		this.SafeDestroy<Material>(ref this.shadowMaterial);
	}

	// Token: 0x06002170 RID: 8560 RVA: 0x0001A904 File Offset: 0x00018B04
	private void CreateCommandBuffers()
	{
		this.SafeCreateCB(ref this.forwardCB, "ReflProbeEx-Forward");
		this.SafeCreateCB(ref this.shadowCB, "ReflProbeEx-Shadow");
	}

	// Token: 0x06002171 RID: 8561 RVA: 0x0001A928 File Offset: 0x00018B28
	private void DestroyCommandBuffers()
	{
		this.SafeDispose<CommandBuffer>(ref this.forwardCB);
		this.SafeDispose<CommandBuffer>(ref this.shadowCB);
	}

	// Token: 0x06002172 RID: 8562 RVA: 0x0001A942 File Offset: 0x00018B42
	private void ClearCommandBuffers()
	{
		this.forwardCB.Clear();
		this.shadowCB.Clear();
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x0001A95A File Offset: 0x00018B5A
	private void AttachToLight()
	{
		if (this.directionalLight != null && this.probeShadowTexture != null)
		{
			this.DetachFromLight();
			this.directionalLight.AddCommandBuffer(LightEvent.AfterScreenspaceMask, this.shadowCB);
		}
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x0001A990 File Offset: 0x00018B90
	private void DetachFromLight()
	{
		if (this.directionalLight != null && this.probeShadowTexture != null)
		{
			this.directionalLight.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, this.shadowCB);
		}
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x000B4A08 File Offset: 0x000B2C08
	private void FindEnvironmentReflection()
	{
		if (RenderSettings.defaultReflectionMode == DefaultReflectionMode.Custom)
		{
			this.reflectionCubemap = RenderSettings.customReflection;
			this.reflectionIntensity = RenderSettings.reflectionIntensity;
			return;
		}
		int defaultReflectionResolution = RenderSettings.defaultReflectionResolution;
		TextureFormat textureFormat = TextureFormat.BC6H;
		foreach (Cubemap cubemap in Resources.FindObjectsOfTypeAll<Cubemap>())
		{
			if (cubemap.width == defaultReflectionResolution && cubemap.height == defaultReflectionResolution && cubemap.format == textureFormat && cubemap.mipmapCount > 1)
			{
				this.reflectionCubemap = cubemap;
				this.reflectionIntensity = RenderSettings.reflectionIntensity;
			}
		}
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x0001A9C0 File Offset: 0x00018BC0
	private void BindGlobalProperties()
	{
		this.forwardCB.SetGlobalVector("_WorldSpaceCameraPos", base.transform.position);
		this.forwardCB.SetGlobalFloat("_TextureMipBias", this.textureMipBias);
	}

	// Token: 0x06002177 RID: 8567 RVA: 0x000B4A94 File Offset: 0x000B2C94
	private void BindGlobalLight()
	{
		Vector3 v = -Vector3.forward;
		Vector4 value = Vector4.zero;
		if (this.directionalLight != null && this.directionalLight.isActiveAndEnabled)
		{
			v = -this.directionalLight.transform.forward;
			value = (this.directionalLight.color * this.directionalLight.intensity).linear;
		}
		float num = (float)this.probeResolution;
		float num2 = 1f + 1f / (float)this.probeResolution;
		this.forwardCB.SetGlobalVector("_WorldSpaceLightPos0", v);
		this.forwardCB.SetGlobalVector("_LightColor0", value);
		this.forwardCB.SetGlobalVector("_ScreenParams", new Vector4(num, num, num2, num2));
		this.forwardCB.SetGlobalMatrix("glstate_matrix_projection", GL.GetGPUProjectionMatrix(this.faceProjMatrix, false));
		if (this.probeShadowTexture != null)
		{
			this.forwardCB.SetGlobalTexture("_ShadowMask", this.probeShadowTexture);
		}
	}

	// Token: 0x06002178 RID: 8568 RVA: 0x000B4BB0 File Offset: 0x000B2DB0
	private void BindGlobalAmbient()
	{
		SphericalHarmonicsL2 sphericalHarmonicsL = RenderSettings.ambientProbe;
		if (RenderSettings.ambientMode == AmbientMode.Flat)
		{
			sphericalHarmonicsL = default(SphericalHarmonicsL2);
			sphericalHarmonicsL.AddAmbientLight(RenderSettings.ambientLight.linear * RenderSettings.ambientIntensity);
		}
		else if (RenderSettings.ambientMode == AmbientMode.Trilight)
		{
			Color a = RenderSettings.ambientSkyColor.linear * RenderSettings.ambientIntensity;
			Color color = RenderSettings.ambientEquatorColor.linear * RenderSettings.ambientIntensity;
			Color a2 = RenderSettings.ambientGroundColor.linear * RenderSettings.ambientIntensity;
			sphericalHarmonicsL = default(SphericalHarmonicsL2);
			sphericalHarmonicsL.AddAmbientLight(color);
			sphericalHarmonicsL.AddDirectionalLight(Vector3.up, a - color, 0.5f);
			sphericalHarmonicsL.AddDirectionalLight(Vector3.down, a2 - color, 0.5f);
		}
		this.forwardCB.SetGlobalVector("unity_SHAr", new Vector4(sphericalHarmonicsL[0, 3], sphericalHarmonicsL[0, 1], sphericalHarmonicsL[0, 2], sphericalHarmonicsL[0, 0] - sphericalHarmonicsL[0, 6]));
		this.forwardCB.SetGlobalVector("unity_SHAg", new Vector4(sphericalHarmonicsL[1, 3], sphericalHarmonicsL[1, 1], sphericalHarmonicsL[1, 2], sphericalHarmonicsL[1, 0] - sphericalHarmonicsL[1, 6]));
		this.forwardCB.SetGlobalVector("unity_SHAb", new Vector4(sphericalHarmonicsL[2, 3], sphericalHarmonicsL[2, 1], sphericalHarmonicsL[2, 2], sphericalHarmonicsL[2, 0] - sphericalHarmonicsL[2, 6]));
		this.forwardCB.SetGlobalVector("unity_SHBr", new Vector4(sphericalHarmonicsL[0, 4], sphericalHarmonicsL[0, 5], sphericalHarmonicsL[0, 6] * 3f, sphericalHarmonicsL[0, 7]));
		this.forwardCB.SetGlobalVector("unity_SHBg", new Vector4(sphericalHarmonicsL[1, 4], sphericalHarmonicsL[1, 5], sphericalHarmonicsL[1, 6] * 3f, sphericalHarmonicsL[1, 7]));
		this.forwardCB.SetGlobalVector("unity_SHBb", new Vector4(sphericalHarmonicsL[2, 4], sphericalHarmonicsL[2, 5], sphericalHarmonicsL[2, 6] * 3f, sphericalHarmonicsL[2, 7]));
		this.forwardCB.SetGlobalVector("unity_SHC", new Vector4(sphericalHarmonicsL[0, 8], sphericalHarmonicsL[1, 8], sphericalHarmonicsL[2, 8], 1f));
	}

	// Token: 0x06002179 RID: 8569 RVA: 0x000B4E48 File Offset: 0x000B3048
	private void BindGlobalReflection()
	{
		Texture customReflection = this.reflectionCubemap;
		float value = this.reflectionIntensity;
		if (RenderSettings.customReflection != null)
		{
			customReflection = RenderSettings.customReflection;
			value = RenderSettings.reflectionIntensity;
		}
		Vector2 v = new Vector2(Mathf.GammaToLinearSpace(value), 1f);
		this.forwardCB.SetGlobalTexture("unity_SpecCube0", customReflection);
		this.forwardCB.SetGlobalTexture("unity_SpecCube1", customReflection);
		this.forwardCB.SetGlobalVector("unity_SpecCube0_HDR", v);
		this.forwardCB.SetGlobalVector("unity_SpecCube1_HDR", v);
	}

	// Token: 0x0600217A RID: 8570 RVA: 0x000B4EE8 File Offset: 0x000B30E8
	private void RenderObjects()
	{
		Matrix4x4 gpuprojectionMatrix = GL.GetGPUProjectionMatrix(this.faceProjMatrix, false);
		for (int i = 0; i < 6; i++)
		{
			this.viewProjMatrixArray[i] = gpuprojectionMatrix * this.platformCubemapFaceMatrices[i].worldToView * base.transform.worldToLocalMatrix;
		}
		this.forwardCB.SetGlobalMatrixArray("_ViewProjMatrixArray", this.viewProjMatrixArray);
		this.forwardCB.SetRenderTarget(this.arrayTexture, this.arrayDepthTexture, 0, CubemapFace.PositiveX, -1);
		for (int j = 0; j < this.PassCount; j++)
		{
			this.forwardCB.SetGlobalInt("_TargetSlice", j);
			foreach (ReflectionProbeEx.RenderListEntry renderListEntry in this.staticRenderList)
			{
				Renderer renderer = renderListEntry.renderer;
				if (renderer != null && (renderer.enabled || renderListEntry.alwaysEnabled) && renderer.gameObject.activeInHierarchy)
				{
					Material material = this.FindMaterialReplacement(renderer.sharedMaterial);
					if (material != null)
					{
						this.forwardCB.DrawMesh(renderer.GetComponent<MeshFilter>().sharedMesh, renderer.transform.localToWorldMatrix, material, 0, 0);
					}
				}
			}
			foreach (ReflectionProbeEx.RenderListEntry renderListEntry2 in this.dynamicRenderList)
			{
				Renderer renderer2 = renderListEntry2.renderer;
				if (renderer2 != null && (renderer2.enabled || renderListEntry2.alwaysEnabled) && renderer2.gameObject.activeInHierarchy)
				{
					Material material2 = this.FindMaterialReplacement(renderer2.sharedMaterial);
					if (material2 != null)
					{
						this.forwardCB.DrawMesh(renderer2.GetComponent<MeshFilter>().sharedMesh, renderer2.transform.localToWorldMatrix, material2, 0, 0);
					}
				}
			}
		}
	}

	// Token: 0x0600217B RID: 8571 RVA: 0x000B510C File Offset: 0x000B330C
	private void RenderSkybox()
	{
		float num = 1E-06f;
		float num2 = this.probeNearClip * 0.01f;
		float num3 = this.probeFarClip * 10f;
		Matrix4x4 gpuprojectionMatrix = this.faceProjMatrix;
		gpuprojectionMatrix[2, 2] = -1f + num;
		gpuprojectionMatrix[2, 3] = (-2f + num) * num2;
		gpuprojectionMatrix[3, 2] = -1f;
		gpuprojectionMatrix = GL.GetGPUProjectionMatrix(gpuprojectionMatrix, false);
		for (int i = 0; i < 6; i++)
		{
			this.viewProjMatrixArray[i] = gpuprojectionMatrix * this.platformCubemapFaceMatrices[i].worldToView;
		}
		this.forwardCB.SetGlobalMatrixArray("_ViewProjMatrixArray", this.viewProjMatrixArray);
		Matrix4x4 matrix = Matrix4x4.TRS(base.transform.position, Quaternion.identity, new Vector3(num3, num3, num3));
		Material material = this.FindMaterialReplacement(RenderSettings.skybox);
		if (material != null)
		{
			this.forwardCB.SetRenderTarget(this.arrayTexture, this.arrayDepthTexture, 0, CubemapFace.PositiveX, -1);
			for (int j = 0; j < this.PassCount; j++)
			{
				this.forwardCB.SetGlobalInt("_TargetSlice", j);
				this.forwardCB.DrawMesh(this.skyboxMesh, matrix, material, 0, 0);
			}
		}
	}

	// Token: 0x0600217C RID: 8572 RVA: 0x000B525C File Offset: 0x000B345C
	private void IssueRender()
	{
		this.forwardCB.SetRenderTarget(this.arrayTexture, this.arrayDepthTexture, 0, CubemapFace.PositiveX, -1);
		this.forwardCB.ClearRenderTarget(true, this.clearFlags == ReflectionProbeClearFlags.SolidColor, this.background);
		this.BindGlobalProperties();
		this.BindGlobalLight();
		this.BindGlobalAmbient();
		this.BindGlobalReflection();
		if (this.clearFlags == ReflectionProbeClearFlags.Skybox)
		{
			this.RenderSkybox();
		}
		this.RenderObjects();
	}

	// Token: 0x0600217D RID: 8573 RVA: 0x000B52D8 File Offset: 0x000B34D8
	private void IssueGenerateMips()
	{
		for (int i = 0; i < this.PassCount; i++)
		{
			int slice = this.useGeometryShader ? -1 : i;
			this.forwardCB.BlitMip(this.blitMesh, this.arrayTexture, this.probeTempTexture, 0, slice, this.blitMaterial, 0);
			this.forwardCB.BlitMip(this.blitMesh, this.arrayTexture, this.probeTexture, 0, slice, this.blitMaterial, 0);
		}
		this.forwardCB.GenerateMips(this.probeTempTexture);
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x000B5360 File Offset: 0x000B3560
	private void IssueConvolution()
	{
		for (int i = 0; i < 6; i++)
		{
			this.viewProjMatrixArray[i] = this.faceProjMatrix * ReflectionProbeEx.cubemapFaceMatrices[i].worldToView;
			this.objectToWorldArray[i] = ReflectionProbeEx.cubemapFaceMatrices[i].viewToWorld;
		}
		this.forwardCB.SetGlobalMatrixArray("_ViewProjMatrixArray", this.viewProjMatrixArray);
		this.forwardCB.SetGlobalMatrixArray("_ObjectToWorldArray", this.objectToWorldArray);
		this.ApplyConvolution(this.probeTexture);
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x000B53F4 File Offset: 0x000B35F4
	private void IssueShadow()
	{
		for (int i = 0; i < 6; i++)
		{
			this.cameraToWorldArray[i] = base.transform.localToWorldMatrix * ReflectionProbeEx.shadowCubemapFaceMatrices[i].worldToView;
		}
		this.shadowCB.SetGlobalMatrix("unity_CameraInvProjection", this.faceProjInvMatrix);
		this.shadowCB.SetGlobalMatrixArray("_CameraToWorldArray", this.cameraToWorldArray);
		this.shadowCB.SetGlobalTexture("_ArrayDepthTexture", this.arrayDepthTexture);
		for (int j = 0; j < this.PassCount; j++)
		{
			int slice = this.useGeometryShader ? -1 : j;
			this.shadowCB.BlitArray(this.blitMesh, BuiltinRenderTextureType.CurrentActive, this.probeShadowTexture, this.shadowMaterial, slice, 0);
		}
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x000B54C4 File Offset: 0x000B36C4
	private void PrepareFullRender()
	{
		if (this.directionalLight != null && this.directionalLight.shadows != LightShadows.None && this.probeShadowTexture != null)
		{
			this.IssueShadow();
		}
		this.IssueRender();
		this.IssueGenerateMips();
		this.IssueConvolution();
	}

	// Token: 0x06002181 RID: 8577 RVA: 0x000B5514 File Offset: 0x000B3714
	private void PrepareTimeSlicedRender(ReflectionProbeEx.TimeSlicingState state)
	{
		if (state == ReflectionProbeEx.TimeSlicingState.Shadow)
		{
			if (this.directionalLight != null && this.directionalLight.shadows != LightShadows.None && this.probeShadowTexture != null)
			{
				this.IssueShadow();
				return;
			}
			state++;
		}
		switch (state)
		{
		case ReflectionProbeEx.TimeSlicingState.Render:
			this.IssueRender();
			return;
		case ReflectionProbeEx.TimeSlicingState.GenerateMips:
			this.IssueGenerateMips();
			return;
		case ReflectionProbeEx.TimeSlicingState.Convolution:
			this.IssueConvolution();
			return;
		default:
			return;
		}
	}

	// Token: 0x06002182 RID: 8578 RVA: 0x0001A9F8 File Offset: 0x00018BF8
	private void ExecuteRender()
	{
		if (this.forwardCB != null)
		{
			bool sRGBWrite = GL.sRGBWrite;
			GL.sRGBWrite = !this.probeHdr;
			Graphics.ExecuteCommandBuffer(this.forwardCB);
			GL.sRGBWrite = sRGBWrite;
		}
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x0001AA28 File Offset: 0x00018C28
	private int FastLog2(int value)
	{
		value |= value >> 1;
		value |= value >> 2;
		value |= value >> 4;
		value |= value >> 8;
		value |= value >> 16;
		return ReflectionProbeEx.tab32[(int)((uint)((long)value * 130329821L) >> 27)];
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x000B5584 File Offset: 0x000B3784
	private uint ReverseBits(uint bits)
	{
		bits = (bits << 16 | bits >> 16);
		bits = ((bits & 16711935U) << 8 | (bits & 4278255360U) >> 8);
		bits = ((bits & 252645135U) << 4 | (bits & 4042322160U) >> 4);
		bits = ((bits & 858993459U) << 2 | (bits & 3435973836U) >> 2);
		bits = ((bits & 1431655765U) << 1 | (bits & 2863311530U) >> 1);
		return bits;
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x0001AA61 File Offset: 0x00018C61
	private void SafeCreateMaterial(ref Material mat, Shader shader)
	{
		if (mat == null)
		{
			mat = new Material(shader);
		}
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x0001AA75 File Offset: 0x00018C75
	private void SafeCreateMaterial(ref Material mat, string shaderName)
	{
		if (mat == null)
		{
			this.SafeCreateMaterial(ref mat, Shader.Find(shaderName));
		}
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x000B55F4 File Offset: 0x000B37F4
	private void SafeCreateCubeRT(ref RenderTexture rt, string name, int size, int depth, bool mips, TextureDimension dim, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Linear)
	{
		if (rt == null || !rt.IsCreated())
		{
			this.SafeDestroy<RenderTexture>(ref rt);
			rt = new RenderTexture(size, size, depth, format, readWrite)
			{
				hideFlags = HideFlags.DontSave
			};
			rt.name = name;
			rt.dimension = dim;
			if (dim == TextureDimension.Tex2DArray)
			{
				rt.volumeDepth = 6;
			}
			rt.useMipMap = mips;
			rt.autoGenerateMips = false;
			rt.filterMode = filter;
			rt.anisoLevel = 0;
			rt.Create();
		}
	}

	// Token: 0x06002188 RID: 8584 RVA: 0x0001AA8E File Offset: 0x00018C8E
	private void SafeCreateCB(ref CommandBuffer cb, string name)
	{
		if (cb == null)
		{
			cb = new CommandBuffer();
			cb.name = name;
		}
	}

	// Token: 0x06002189 RID: 8585 RVA: 0x0001995B File Offset: 0x00017B5B
	private void SafeDestroy<T>(ref T obj) where T : Object
	{
		if (obj != null)
		{
			Object.DestroyImmediate(obj);
			obj = default(T);
		}
	}

	// Token: 0x0600218A RID: 8586 RVA: 0x0001AAA3 File Offset: 0x00018CA3
	private void SafeDispose<T>(ref T obj) where T : IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
			obj = default(T);
		}
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x000B577C File Offset: 0x000B397C
	// Note: this type is marked as 'beforefieldinit'.
	static ReflectionProbeEx()
	{
		HashSet<string> hashSet = new HashSet<string>();
		hashSet.Add("Skybox/Procedural");
		hashSet.Add("Time of Day/Atmosphere");
		hashSet.Add("Time of Day/Cloud Billboard Far");
		hashSet.Add("Time of Day/Cloud Layer");
		hashSet.Add("Time of Day/Moon");
		hashSet.Add("Time of Day/Skybox");
		hashSet.Add("Time of Day/Stars");
		hashSet.Add("Time of Day/Sun");
		hashSet.Add("Standard");
		ReflectionProbeEx.supportedShaderNames = hashSet;
		ReflectionProbeEx.supportedShaders = new Dictionary<Shader, Shader>();
		ReflectionProbeEx.matchingMaterials = new Dictionary<Material, Material>();
		ReflectionProbeEx.tab32 = new int[]
		{
			0,
			9,
			1,
			10,
			13,
			21,
			2,
			29,
			11,
			14,
			16,
			18,
			22,
			25,
			3,
			30,
			8,
			12,
			20,
			28,
			15,
			17,
			24,
			7,
			19,
			27,
			23,
			6,
			26,
			5,
			4,
			31
		};
	}

	// Token: 0x020005AC RID: 1452
	[Serializable]
	public enum ConvolutionQuality
	{
		// Token: 0x04001D4F RID: 7503
		Lowest,
		// Token: 0x04001D50 RID: 7504
		Low,
		// Token: 0x04001D51 RID: 7505
		Medium,
		// Token: 0x04001D52 RID: 7506
		High,
		// Token: 0x04001D53 RID: 7507
		VeryHigh
	}

	// Token: 0x020005AD RID: 1453
	[Serializable]
	public struct RenderListEntry
	{
		// Token: 0x04001D54 RID: 7508
		public Renderer renderer;

		// Token: 0x04001D55 RID: 7509
		public bool alwaysEnabled;

		// Token: 0x0600218D RID: 8589 RVA: 0x0001AAC5 File Offset: 0x00018CC5
		public RenderListEntry(Renderer renderer, bool alwaysEnabled)
		{
			this.renderer = renderer;
			this.alwaysEnabled = alwaysEnabled;
		}
	}

	// Token: 0x020005AE RID: 1454
	private enum TimeSlicingState
	{
		// Token: 0x04001D57 RID: 7511
		Shadow,
		// Token: 0x04001D58 RID: 7512
		Render,
		// Token: 0x04001D59 RID: 7513
		GenerateMips,
		// Token: 0x04001D5A RID: 7514
		Convolution,
		// Token: 0x04001D5B RID: 7515
		Finished
	}

	// Token: 0x020005AF RID: 1455
	private struct CubemapSkyboxVertex
	{
		// Token: 0x04001D5C RID: 7516
		public float x;

		// Token: 0x04001D5D RID: 7517
		public float y;

		// Token: 0x04001D5E RID: 7518
		public float z;

		// Token: 0x04001D5F RID: 7519
		public Color color;

		// Token: 0x04001D60 RID: 7520
		public float tu;

		// Token: 0x04001D61 RID: 7521
		public float tv;

		// Token: 0x04001D62 RID: 7522
		public float tw;
	}

	// Token: 0x020005B0 RID: 1456
	private struct CubemapFaceMatrices
	{
		// Token: 0x04001D63 RID: 7523
		public Matrix4x4 worldToView;

		// Token: 0x04001D64 RID: 7524
		public Matrix4x4 viewToWorld;

		// Token: 0x0600218E RID: 8590 RVA: 0x000B5D98 File Offset: 0x000B3F98
		public CubemapFaceMatrices(Vector3 x, Vector3 y, Vector3 z)
		{
			this.worldToView = Matrix4x4.identity;
			this.worldToView[0, 0] = x[0];
			this.worldToView[0, 1] = x[1];
			this.worldToView[0, 2] = x[2];
			this.worldToView[1, 0] = y[0];
			this.worldToView[1, 1] = y[1];
			this.worldToView[1, 2] = y[2];
			this.worldToView[2, 0] = z[0];
			this.worldToView[2, 1] = z[1];
			this.worldToView[2, 2] = z[2];
			this.viewToWorld = this.worldToView.inverse;
		}
	}
}
