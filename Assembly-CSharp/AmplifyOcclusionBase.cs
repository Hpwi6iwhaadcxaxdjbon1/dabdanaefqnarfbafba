using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

// Token: 0x02000786 RID: 1926
[AddComponentMenu("")]
public class AmplifyOcclusionBase : MonoBehaviour
{
	// Token: 0x040024F2 RID: 9458
	[Header("Ambient Occlusion")]
	public AmplifyOcclusionBase.ApplicationMethod ApplyMethod;

	// Token: 0x040024F3 RID: 9459
	[Tooltip("Number of samples per pass.")]
	public AmplifyOcclusionBase.SampleCountLevel SampleCount = AmplifyOcclusionBase.SampleCountLevel.Medium;

	// Token: 0x040024F4 RID: 9460
	public AmplifyOcclusionBase.PerPixelNormalSource PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.Camera;

	// Token: 0x040024F5 RID: 9461
	[Tooltip("Final applied intensity of the occlusion effect.")]
	[Range(0f, 1f)]
	public float Intensity = 1f;

	// Token: 0x040024F6 RID: 9462
	public Color Tint = Color.black;

	// Token: 0x040024F7 RID: 9463
	[Range(0f, 32f)]
	[Tooltip("Radius spread of the occlusion.")]
	public float Radius = 2f;

	// Token: 0x040024F8 RID: 9464
	[Range(32f, 1024f)]
	[Tooltip("Max sampling range in pixels.")]
	[NonSerialized]
	public int PixelRadiusLimit = 512;

	// Token: 0x040024F9 RID: 9465
	[Tooltip("Occlusion contribution amount on relation to radius.")]
	[Range(0f, 2f)]
	[NonSerialized]
	public float RadiusIntensity = 1f;

	// Token: 0x040024FA RID: 9466
	[Range(0f, 16f)]
	[Tooltip("Power exponent attenuation of the occlusion.")]
	public float PowerExponent = 1.8f;

	// Token: 0x040024FB RID: 9467
	[Tooltip("Controls the initial occlusion contribution offset.")]
	[Range(0f, 0.99f)]
	public float Bias = 0.05f;

	// Token: 0x040024FC RID: 9468
	[Tooltip("Controls the thickness occlusion contribution.")]
	[Range(0f, 1f)]
	public float Thickness = 1f;

	// Token: 0x040024FD RID: 9469
	[Tooltip("Compute the Occlusion and Blur at half of the resolution.")]
	public bool Downsample = true;

	// Token: 0x040024FE RID: 9470
	[Header("Distance Fade")]
	[Tooltip("Control parameters at faraway.")]
	public bool FadeEnabled;

	// Token: 0x040024FF RID: 9471
	[Tooltip("Distance in Unity unities that start to fade.")]
	public float FadeStart = 100f;

	// Token: 0x04002500 RID: 9472
	[Tooltip("Length distance to performe the transition.")]
	public float FadeLength = 50f;

	// Token: 0x04002501 RID: 9473
	[Tooltip("Final Intensity parameter.")]
	[Range(0f, 1f)]
	public float FadeToIntensity;

	// Token: 0x04002502 RID: 9474
	public Color FadeToTint = Color.black;

	// Token: 0x04002503 RID: 9475
	[Range(0f, 32f)]
	[Tooltip("Final Radius parameter.")]
	public float FadeToRadius = 2f;

	// Token: 0x04002504 RID: 9476
	[Tooltip("Final PowerExponent parameter.")]
	[Range(0f, 16f)]
	public float FadeToPowerExponent = 1.8f;

	// Token: 0x04002505 RID: 9477
	[Range(0f, 1f)]
	[Tooltip("Final Thickness parameter.")]
	public float FadeToThickness = 1f;

	// Token: 0x04002506 RID: 9478
	[Header("Bilateral Blur")]
	public bool BlurEnabled = true;

	// Token: 0x04002507 RID: 9479
	[Tooltip("Radius in screen pixels.")]
	[Range(1f, 4f)]
	public int BlurRadius = 3;

	// Token: 0x04002508 RID: 9480
	[Tooltip("Number of times that the Blur will repeat.")]
	[Range(1f, 4f)]
	public int BlurPasses = 1;

	// Token: 0x04002509 RID: 9481
	[Range(0f, 20f)]
	[Tooltip("0 - Blured, 1 - Sharpened.")]
	public float BlurSharpness = 10f;

	// Token: 0x0400250A RID: 9482
	[Header("Temporal Filter")]
	[Tooltip("Accumulates the effect over the time.")]
	public bool FilterEnabled = true;

	// Token: 0x0400250B RID: 9483
	[Range(0f, 1f)]
	[Tooltip("Controls the accumulation decayment. 0 - Faster update, more flicker. 1 - Slow update (ghosting on moving objects), less flicker.")]
	public float FilterBlending = 0.5f;

	// Token: 0x0400250C RID: 9484
	[Tooltip("Controls the discard sensibility based on the motion of the scene and objects. 0 - Discard less, reuse more (more ghost effect). 1 - Discard more, reuse less (less ghost effect).")]
	[Range(0f, 1f)]
	public float FilterResponse = 0.5f;

	// Token: 0x0400250D RID: 9485
	[Tooltip("Enables directional variations.")]
	[NonSerialized]
	public bool TemporalDirections = true;

	// Token: 0x0400250E RID: 9486
	[Tooltip("Enables offset variations.")]
	[NonSerialized]
	public bool TemporalOffsets = true;

	// Token: 0x0400250F RID: 9487
	[Tooltip("Reduces ghosting effect near the objects's edges while moving.")]
	[NonSerialized]
	public bool TemporalDilation;

	// Token: 0x04002510 RID: 9488
	[Tooltip("Uses the object movement information for calc new areas of occlusion.")]
	[NonSerialized]
	public bool UseMotionVectors = true;

	// Token: 0x04002511 RID: 9489
	private AmplifyOcclusionBase.PerPixelNormalSource m_prevPerPixelNormals;

	// Token: 0x04002512 RID: 9490
	private AmplifyOcclusionBase.ApplicationMethod m_prevApplyMethod;

	// Token: 0x04002513 RID: 9491
	private bool m_prevDeferredReflections;

	// Token: 0x04002514 RID: 9492
	private AmplifyOcclusionBase.SampleCountLevel m_prevSampleCount;

	// Token: 0x04002515 RID: 9493
	private bool m_prevDownsample;

	// Token: 0x04002516 RID: 9494
	private bool m_prevBlurEnabled;

	// Token: 0x04002517 RID: 9495
	private int m_prevBlurRadius;

	// Token: 0x04002518 RID: 9496
	private int m_prevBlurPasses;

	// Token: 0x04002519 RID: 9497
	private Camera m_targetCamera;

	// Token: 0x0400251A RID: 9498
	private RenderTargetIdentifier[] applyDebugTargetsTemporal = new RenderTargetIdentifier[2];

	// Token: 0x0400251B RID: 9499
	private RenderTargetIdentifier[] applyPostEffectTargetsTemporal = new RenderTargetIdentifier[2];

	// Token: 0x0400251C RID: 9500
	private RenderTargetIdentifier[] applyDeferredTargets_Log_Temporal = new RenderTargetIdentifier[3];

	// Token: 0x0400251D RID: 9501
	private RenderTargetIdentifier[] applyDeferredTargetsTemporal = new RenderTargetIdentifier[3];

	// Token: 0x0400251E RID: 9502
	private AmplifyOcclusionBase.CmdBuffer m_commandBuffer_Occlusion;

	// Token: 0x0400251F RID: 9503
	private AmplifyOcclusionBase.CmdBuffer m_commandBuffer_Apply;

	// Token: 0x04002520 RID: 9504
	private static Mesh m_quadMesh = null;

	// Token: 0x04002521 RID: 9505
	private static Material m_occlusionMat = null;

	// Token: 0x04002522 RID: 9506
	private static Material m_blurMat = null;

	// Token: 0x04002523 RID: 9507
	private static Material m_applyOcclusionMat = null;

	// Token: 0x04002524 RID: 9508
	private RenderTextureFormat m_occlusionRTFormat = RenderTextureFormat.RGHalf;

	// Token: 0x04002525 RID: 9509
	private RenderTextureFormat m_accumTemporalRTFormat;

	// Token: 0x04002526 RID: 9510
	private RenderTextureFormat m_temporaryEmissionRTFormat;

	// Token: 0x04002527 RID: 9511
	private bool m_paramsChanged = true;

	// Token: 0x04002528 RID: 9512
	private RenderTexture m_occlusionDepthRT;

	// Token: 0x04002529 RID: 9513
	private RenderTexture[] m_temporalAccumRT;

	// Token: 0x0400252A RID: 9514
	private uint m_sampleStep;

	// Token: 0x0400252B RID: 9515
	private uint m_curStepIdx;

	// Token: 0x0400252C RID: 9516
	private static readonly uint m_maxSampleSteps = 6U;

	// Token: 0x0400252D RID: 9517
	private static readonly int PerPixelNormalSourceCount = 4;

	// Token: 0x0400252E RID: 9518
	private Matrix4x4 m_prevViewProjMatrixLeft = Matrix4x4.identity;

	// Token: 0x0400252F RID: 9519
	private Matrix4x4 m_prevInvViewProjMatrixLeft = Matrix4x4.identity;

	// Token: 0x04002530 RID: 9520
	private Matrix4x4 m_prevViewProjMatrixRight = Matrix4x4.identity;

	// Token: 0x04002531 RID: 9521
	private Matrix4x4 m_prevInvViewProjMatrixRight = Matrix4x4.identity;

	// Token: 0x04002532 RID: 9522
	private static readonly float[] m_temporalRotations = new float[]
	{
		0.16666667f,
		0.8333333f,
		0.5f,
		0.6666667f,
		0.33333334f,
		0f
	};

	// Token: 0x04002533 RID: 9523
	private static readonly float[] m_spatialOffsets = new float[]
	{
		0f,
		0.5f,
		0.25f,
		0.75f
	};

	// Token: 0x04002534 RID: 9524
	private readonly RenderTargetIdentifier[] m_applyDeferredTargets = new RenderTargetIdentifier[]
	{
		BuiltinRenderTextureType.GBuffer0,
		BuiltinRenderTextureType.CameraTarget
	};

	// Token: 0x04002535 RID: 9525
	private readonly RenderTargetIdentifier[] m_applyDeferredTargets_Log = new RenderTargetIdentifier[]
	{
		BuiltinRenderTextureType.GBuffer0,
		BuiltinRenderTextureType.GBuffer3
	};

	// Token: 0x04002536 RID: 9526
	private AmplifyOcclusionBase.TargetDesc m_target;

	// Token: 0x060029DE RID: 10718 RVA: 0x000D520C File Offset: 0x000D340C
	private void createCommandBuffer(ref AmplifyOcclusionBase.CmdBuffer aCmdBuffer, string aCmdBufferName, CameraEvent aCameraEvent)
	{
		if (aCmdBuffer.cmdBuffer != null)
		{
			this.cleanupCommandBuffer(ref aCmdBuffer);
		}
		aCmdBuffer.cmdBufferName = aCmdBufferName;
		aCmdBuffer.cmdBuffer = new CommandBuffer();
		aCmdBuffer.cmdBuffer.name = aCmdBufferName;
		aCmdBuffer.cmdBufferEvent = aCameraEvent;
		this.m_targetCamera.AddCommandBuffer(aCameraEvent, aCmdBuffer.cmdBuffer);
	}

	// Token: 0x060029DF RID: 10719 RVA: 0x000D5260 File Offset: 0x000D3460
	private void cleanupCommandBuffer(ref AmplifyOcclusionBase.CmdBuffer aCmdBuffer)
	{
		CommandBuffer[] commandBuffers = this.m_targetCamera.GetCommandBuffers(aCmdBuffer.cmdBufferEvent);
		for (int i = 0; i < commandBuffers.Length; i++)
		{
			if (commandBuffers[i].name == aCmdBuffer.cmdBufferName)
			{
				this.m_targetCamera.RemoveCommandBuffer(aCmdBuffer.cmdBufferEvent, commandBuffers[i]);
			}
		}
		aCmdBuffer.cmdBufferName = null;
		aCmdBuffer.cmdBufferEvent = CameraEvent.BeforeDepthTexture;
		aCmdBuffer.cmdBuffer = null;
	}

	// Token: 0x060029E0 RID: 10720 RVA: 0x000D52CC File Offset: 0x000D34CC
	private void createQuadMesh()
	{
		if (AmplifyOcclusionBase.m_quadMesh == null)
		{
			AmplifyOcclusionBase.m_quadMesh = new Mesh();
			AmplifyOcclusionBase.m_quadMesh.vertices = new Vector3[]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 1f, 0f),
				new Vector3(1f, 1f, 0f),
				new Vector3(1f, 0f, 0f)
			};
			AmplifyOcclusionBase.m_quadMesh.uv = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f)
			};
			AmplifyOcclusionBase.m_quadMesh.triangles = new int[]
			{
				0,
				1,
				2,
				0,
				2,
				3
			};
			AmplifyOcclusionBase.m_quadMesh.normals = new Vector3[0];
			AmplifyOcclusionBase.m_quadMesh.tangents = new Vector4[0];
			AmplifyOcclusionBase.m_quadMesh.colors32 = new Color32[0];
			AmplifyOcclusionBase.m_quadMesh.colors = new Color[0];
		}
	}

	// Token: 0x060029E1 RID: 10721 RVA: 0x0002082E File Offset: 0x0001EA2E
	private void PerformBlit(CommandBuffer cb, Material mat, int pass)
	{
		cb.DrawMesh(AmplifyOcclusionBase.m_quadMesh, Matrix4x4.identity, mat, 0, pass);
	}

	// Token: 0x060029E2 RID: 10722 RVA: 0x000D5434 File Offset: 0x000D3634
	private Material createMaterialWithShaderName(string aShaderName, bool aThroughErrorMsg)
	{
		Shader shader = Shader.Find(aShaderName);
		if (shader == null)
		{
			if (aThroughErrorMsg)
			{
				Debug.LogErrorFormat("[AmplifyOcclusion] Cannot find shader: \"{0}\" Please contact support@amplify.pt", new object[]
				{
					aShaderName
				});
			}
			return null;
		}
		return new Material(shader)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x060029E3 RID: 10723 RVA: 0x000D5478 File Offset: 0x000D3678
	private void checkMaterials(bool aThroughErrorMsg)
	{
		if (AmplifyOcclusionBase.m_occlusionMat == null)
		{
			AmplifyOcclusionBase.m_occlusionMat = this.createMaterialWithShaderName("Hidden/Amplify Occlusion/Occlusion", aThroughErrorMsg);
		}
		if (AmplifyOcclusionBase.m_blurMat == null)
		{
			AmplifyOcclusionBase.m_blurMat = this.createMaterialWithShaderName("Hidden/Amplify Occlusion/Blur", aThroughErrorMsg);
		}
		if (AmplifyOcclusionBase.m_applyOcclusionMat == null)
		{
			AmplifyOcclusionBase.m_applyOcclusionMat = this.createMaterialWithShaderName("Hidden/Amplify Occlusion/Apply", aThroughErrorMsg);
		}
	}

	// Token: 0x060029E4 RID: 10724 RVA: 0x000D54E0 File Offset: 0x000D36E0
	private bool checkRenderTextureFormats()
	{
		if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32) && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			this.m_occlusionRTFormat = RenderTextureFormat.RGHalf;
			if (!SystemInfo.SupportsRenderTextureFormat(this.m_occlusionRTFormat))
			{
				this.m_occlusionRTFormat = RenderTextureFormat.RGFloat;
				if (!SystemInfo.SupportsRenderTextureFormat(this.m_occlusionRTFormat))
				{
					this.m_occlusionRTFormat = RenderTextureFormat.ARGBHalf;
				}
			}
			this.m_accumTemporalRTFormat = RenderTextureFormat.ARGB32;
			return true;
		}
		return false;
	}

	// Token: 0x060029E5 RID: 10725 RVA: 0x00020843 File Offset: 0x0001EA43
	private void OnEnable()
	{
		if (!this.checkRenderTextureFormats())
		{
			Debug.LogError("[AmplifyOcclusion] Target platform does not meet the minimum requirements for this effect to work properly.");
			base.enabled = false;
			return;
		}
		this.m_targetCamera = base.GetComponent<Camera>();
		this.checkMaterials(false);
		this.createQuadMesh();
	}

	// Token: 0x060029E6 RID: 10726 RVA: 0x00020878 File Offset: 0x0001EA78
	private void Reset()
	{
		if (this.m_commandBuffer_Occlusion.cmdBuffer != null)
		{
			this.cleanupCommandBuffer(ref this.m_commandBuffer_Occlusion);
		}
		if (this.m_commandBuffer_Apply.cmdBuffer != null)
		{
			this.cleanupCommandBuffer(ref this.m_commandBuffer_Apply);
		}
		this.releaseRT();
	}

	// Token: 0x060029E7 RID: 10727 RVA: 0x000208B2 File Offset: 0x0001EAB2
	private void OnDisable()
	{
		this.Reset();
	}

	// Token: 0x060029E8 RID: 10728 RVA: 0x000D5538 File Offset: 0x000D3738
	private void releaseRT()
	{
		this.safeReleaseRT(ref this.m_occlusionDepthRT);
		this.m_occlusionDepthRT = null;
		if (this.m_temporalAccumRT != null && this.m_temporalAccumRT.Length != 0)
		{
			this.safeReleaseRT(ref this.m_temporalAccumRT[0]);
			this.safeReleaseRT(ref this.m_temporalAccumRT[1]);
		}
		this.m_temporalAccumRT = null;
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x000D5594 File Offset: 0x000D3794
	private bool checkParamsChanged()
	{
		if (this.m_occlusionDepthRT != null && (this.m_occlusionDepthRT.width != this.m_target.width || this.m_occlusionDepthRT.height != this.m_target.height || !this.m_occlusionDepthRT.IsCreated()))
		{
			this.releaseRT();
			this.m_paramsChanged = true;
		}
		if (this.m_temporalAccumRT != null && this.m_temporalAccumRT.Length != 2)
		{
			this.m_temporalAccumRT = null;
		}
		if (this.m_occlusionDepthRT == null)
		{
			this.m_occlusionDepthRT = this.safeAllocateRT("_AO_OcclusionDepthTexture", this.m_target.width, this.m_target.height, this.m_occlusionRTFormat, RenderTextureReadWrite.Linear, FilterMode.Point);
		}
		if (this.m_temporalAccumRT == null)
		{
			this.m_temporalAccumRT = new RenderTexture[2];
			this.m_temporalAccumRT[0] = this.safeAllocateRT("_AO_TemporalAccum_0", this.m_target.width, this.m_target.height, this.m_accumTemporalRTFormat, RenderTextureReadWrite.Linear, FilterMode.Bilinear);
			this.m_temporalAccumRT[1] = this.safeAllocateRT("_AO_TemporalAccum_1", this.m_target.width, this.m_target.height, this.m_accumTemporalRTFormat, RenderTextureReadWrite.Linear, FilterMode.Bilinear);
		}
		if (this.m_prevSampleCount != this.SampleCount || this.m_prevDownsample != this.Downsample || this.m_prevBlurEnabled != this.BlurEnabled || this.m_prevBlurPasses != this.BlurPasses || this.m_prevBlurRadius != this.BlurRadius)
		{
			this.m_paramsChanged = true;
		}
		return this.m_paramsChanged;
	}

	// Token: 0x060029EA RID: 10730 RVA: 0x000D571C File Offset: 0x000D391C
	private void updateParams()
	{
		this.m_prevSampleCount = this.SampleCount;
		this.m_prevDownsample = this.Downsample;
		this.m_prevBlurEnabled = this.BlurEnabled;
		this.m_prevBlurPasses = this.BlurPasses;
		this.m_prevBlurRadius = this.BlurRadius;
		this.m_paramsChanged = false;
	}

	// Token: 0x060029EB RID: 10731 RVA: 0x000D576C File Offset: 0x000D396C
	private void Update()
	{
		if (this.m_targetCamera.actualRenderingPath != RenderingPath.DeferredShading)
		{
			if (this.PerPixelNormals != AmplifyOcclusionBase.PerPixelNormalSource.None && this.PerPixelNormals != AmplifyOcclusionBase.PerPixelNormalSource.Camera)
			{
				this.m_paramsChanged = true;
				this.PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.Camera;
				Debug.LogWarning("[AmplifyOcclusion] GBuffer Normals only available in Camera Deferred Shading mode. Switched to Camera source.");
			}
			if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred)
			{
				this.m_paramsChanged = true;
				this.ApplyMethod = AmplifyOcclusionBase.ApplicationMethod.PostEffect;
				Debug.LogWarning("[AmplifyOcclusion] Deferred Method requires a Deferred Shading path. Switching to Post Effect Method.");
			}
		}
		else
		{
			if (this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.Camera)
			{
				this.m_paramsChanged = true;
				this.PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.GBuffer;
				Debug.LogWarning("[AmplifyOcclusion] Camera Normals not supported for Deferred Method. Switching to GBuffer Normals.");
			}
			if (this.UseMotionVectors && this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred)
			{
				this.m_paramsChanged = true;
				this.UseMotionVectors = false;
			}
			else if (!this.UseMotionVectors && this.ApplyMethod != AmplifyOcclusionBase.ApplicationMethod.Deferred)
			{
				this.m_paramsChanged = true;
				this.UseMotionVectors = true;
			}
		}
		if ((this.m_targetCamera.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None)
		{
			this.m_targetCamera.depthTextureMode |= DepthTextureMode.Depth;
		}
		if (this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.Camera && (this.m_targetCamera.depthTextureMode & DepthTextureMode.DepthNormals) == DepthTextureMode.None)
		{
			this.m_targetCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
		}
		if (this.UseMotionVectors && (this.m_targetCamera.depthTextureMode & DepthTextureMode.MotionVectors) == DepthTextureMode.None)
		{
			this.m_targetCamera.depthTextureMode |= DepthTextureMode.MotionVectors;
		}
	}

	// Token: 0x060029EC RID: 10732 RVA: 0x000D58AC File Offset: 0x000D3AAC
	private void OnPreRender()
	{
		this.checkMaterials(true);
		if (this.m_targetCamera != null)
		{
			bool flag = GraphicsSettings.GetShaderMode(BuiltinShaderType.DeferredReflections) > BuiltinShaderMode.Disabled;
			if (this.m_prevPerPixelNormals != this.PerPixelNormals || this.m_prevApplyMethod != this.ApplyMethod || this.m_prevDeferredReflections != flag || this.m_commandBuffer_Occlusion.cmdBuffer == null || this.m_commandBuffer_Apply.cmdBuffer == null)
			{
				CameraEvent aCameraEvent = CameraEvent.BeforeImageEffectsOpaque;
				if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred)
				{
					aCameraEvent = (flag ? CameraEvent.BeforeReflections : CameraEvent.BeforeLighting);
				}
				this.createCommandBuffer(ref this.m_commandBuffer_Occlusion, "AmplifyOcclusion_Compute", aCameraEvent);
				this.createCommandBuffer(ref this.m_commandBuffer_Apply, "AmplifyOcclusion_Apply", aCameraEvent);
				this.m_prevPerPixelNormals = this.PerPixelNormals;
				this.m_prevApplyMethod = this.ApplyMethod;
				this.m_prevDeferredReflections = flag;
				this.m_paramsChanged = true;
			}
			if (this.m_commandBuffer_Occlusion.cmdBuffer != null && this.m_commandBuffer_Apply.cmdBuffer != null)
			{
				this.m_curStepIdx = (this.m_sampleStep & 1U);
				this.UpdateGlobalShaderConstants();
				this.checkParamsChanged();
				this.UpdateGlobalShaderConstants_AmbientOcclusion();
				this.UpdateGlobalShaderConstants_Matrices();
				if (this.m_paramsChanged)
				{
					this.m_commandBuffer_Occlusion.cmdBuffer.Clear();
					this.commandBuffer_FillComputeOcclusion(this.m_commandBuffer_Occlusion.cmdBuffer);
				}
				this.m_commandBuffer_Apply.cmdBuffer.Clear();
				if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Debug)
				{
					this.commandBuffer_FillApplyDebug(this.m_commandBuffer_Apply.cmdBuffer);
				}
				else if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.PostEffect)
				{
					this.commandBuffer_FillApplyPostEffect(this.m_commandBuffer_Apply.cmdBuffer);
				}
				else
				{
					bool logTarget = !this.m_targetCamera.allowHDR;
					this.commandBuffer_FillApplyDeferred(this.m_commandBuffer_Apply.cmdBuffer, logTarget);
				}
				this.updateParams();
				this.m_sampleStep += 1U;
			}
		}
	}

	// Token: 0x060029ED RID: 10733 RVA: 0x000208BA File Offset: 0x0001EABA
	private void OnPostRender()
	{
		if (this.m_occlusionDepthRT != null)
		{
			this.m_occlusionDepthRT.MarkRestoreExpected();
		}
	}

	// Token: 0x060029EE RID: 10734 RVA: 0x000D5A68 File Offset: 0x000D3C68
	private int safeAllocateTemporaryRT(CommandBuffer cb, string propertyName, int width, int height, RenderTextureFormat format = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, FilterMode filterMode = FilterMode.Point)
	{
		int num = Shader.PropertyToID(propertyName);
		cb.GetTemporaryRT(num, width, height, 0, filterMode, format, readWrite);
		return num;
	}

	// Token: 0x060029EF RID: 10735 RVA: 0x000208D5 File Offset: 0x0001EAD5
	private void safeReleaseTemporaryRT(CommandBuffer cb, int id)
	{
		cb.ReleaseTemporaryRT(id);
	}

	// Token: 0x060029F0 RID: 10736 RVA: 0x000D5A90 File Offset: 0x000D3C90
	private RenderTexture safeAllocateRT(string name, int width, int height, RenderTextureFormat format, RenderTextureReadWrite readWrite, FilterMode filterMode = FilterMode.Point)
	{
		width = Mathf.Clamp(width, 1, 65536);
		height = Mathf.Clamp(height, 1, 65536);
		RenderTexture renderTexture = new RenderTexture(width, height, 0, format, readWrite);
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.name = name;
		renderTexture.filterMode = filterMode;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x000208DE File Offset: 0x0001EADE
	private void safeReleaseRT(ref RenderTexture rt)
	{
		if (rt != null)
		{
			RenderTexture.active = null;
			rt.Release();
			Object.DestroyImmediate(rt);
			rt = null;
		}
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x00020901 File Offset: 0x0001EB01
	private void BeginSample(CommandBuffer cb, string name)
	{
		cb.BeginSample(name);
	}

	// Token: 0x060029F3 RID: 10739 RVA: 0x0002090A File Offset: 0x0001EB0A
	private void EndSample(CommandBuffer cb, string name)
	{
		cb.EndSample(name);
	}

	// Token: 0x060029F4 RID: 10740 RVA: 0x000D5AEC File Offset: 0x000D3CEC
	private void commandBuffer_FillComputeOcclusion(CommandBuffer cb)
	{
		this.BeginSample(cb, "AO 1 - ComputeOcclusion");
		if (this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.GBuffer || this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.GBufferOctaEncoded)
		{
			cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_GBufferNormals, BuiltinRenderTextureType.GBuffer2);
		}
		Vector4 value = new Vector4(this.m_target.oneOverWidth, this.m_target.oneOverHeight, (float)this.m_target.width, (float)this.m_target.height);
		int pass = (int)(this.SampleCount * (AmplifyOcclusionBase.SampleCountLevel)AmplifyOcclusionBase.PerPixelNormalSourceCount + (int)this.PerPixelNormals);
		if (this.Downsample)
		{
			int num = this.safeAllocateTemporaryRT(cb, "_AO_SmallOcclusionTexture", this.m_target.width / 2, this.m_target.height / 2, this.m_occlusionRTFormat, RenderTextureReadWrite.Linear, FilterMode.Point);
			cb.SetGlobalVector(AmplifyOcclusionBase.PropertyID._AO_Target_TexelSize, new Vector4(1f / ((float)this.m_target.width / 2f), 1f / ((float)this.m_target.height / 2f), (float)this.m_target.width / 2f, (float)this.m_target.height / 2f));
			cb.SetRenderTarget(num);
			this.PerformBlit(cb, AmplifyOcclusionBase.m_occlusionMat, pass);
			cb.SetRenderTarget(null);
			this.EndSample(cb, "AO 1 - ComputeOcclusion");
			if (this.BlurEnabled)
			{
				this.commandBuffer_Blur(cb, num, this.m_target.width / 2, this.m_target.height / 2);
			}
			this.BeginSample(cb, "AO 2b - Combine");
			cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_Source, num);
			cb.SetGlobalVector(AmplifyOcclusionBase.PropertyID._AO_Target_TexelSize, value);
			cb.SetRenderTarget(this.m_occlusionDepthRT);
			this.PerformBlit(cb, AmplifyOcclusionBase.m_occlusionMat, 16);
			this.safeReleaseTemporaryRT(cb, num);
			cb.SetRenderTarget(null);
			this.EndSample(cb, "AO 2b - Combine");
			return;
		}
		cb.SetGlobalVector(AmplifyOcclusionBase.PropertyID._AO_Source_TexelSize, value);
		cb.SetGlobalVector(AmplifyOcclusionBase.PropertyID._AO_Target_TexelSize, value);
		cb.SetRenderTarget(this.m_occlusionDepthRT);
		this.PerformBlit(cb, AmplifyOcclusionBase.m_occlusionMat, pass);
		cb.SetRenderTarget(null);
		this.EndSample(cb, "AO 1 - ComputeOcclusion");
		if (this.BlurEnabled)
		{
			this.commandBuffer_Blur(cb, this.m_occlusionDepthRT, this.m_target.width, this.m_target.height);
		}
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x000D5D58 File Offset: 0x000D3F58
	private void commandBuffer_Blur(CommandBuffer cb, RenderTargetIdentifier aSourceRT, int aSourceWidth, int aSourceHeight)
	{
		this.BeginSample(cb, "AO 2 - Blur");
		int num = this.safeAllocateTemporaryRT(cb, "_AO_BlurTmp", aSourceWidth, aSourceHeight, this.m_occlusionRTFormat, RenderTextureReadWrite.Linear, FilterMode.Point);
		for (int i = 0; i < this.BlurPasses; i++)
		{
			cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_Source, aSourceRT);
			int pass = (this.BlurRadius - 1) * 2;
			cb.SetRenderTarget(num);
			this.PerformBlit(cb, AmplifyOcclusionBase.m_blurMat, pass);
			cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_Source, num);
			int pass2 = 1 + (this.BlurRadius - 1) * 2;
			cb.SetRenderTarget(aSourceRT);
			this.PerformBlit(cb, AmplifyOcclusionBase.m_blurMat, pass2);
		}
		this.safeReleaseTemporaryRT(cb, num);
		cb.SetRenderTarget(null);
		this.EndSample(cb, "AO 2 - Blur");
	}

	// Token: 0x060029F6 RID: 10742 RVA: 0x00020913 File Offset: 0x0001EB13
	private int getTemporalPass()
	{
		return (this.UseMotionVectors ? 2 : 0) | (this.TemporalDilation ? 1 : 0);
	}

	// Token: 0x060029F7 RID: 10743 RVA: 0x000D5E1C File Offset: 0x000D401C
	private void commandBuffer_TemporalFilter(CommandBuffer cb)
	{
		float num = Mathf.Lerp(0.65f, 0.86f, this.FilterBlending);
		num = ((this.SampleCount == AmplifyOcclusionBase.SampleCountLevel.High) ? (num * 0.975f) : num);
		num = ((this.SampleCount == AmplifyOcclusionBase.SampleCountLevel.VeryHigh) ? (num * 0.95f) : num);
		cb.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_TemporalCurveAdj, num);
		cb.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_TemporalMotionSensibility, this.FilterResponse * this.FilterResponse + 0.01f);
		cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_CurrOcclusionDepth, this.m_occlusionDepthRT);
		cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_TemporalAccumm, this.m_temporalAccumRT[(int)(1U - this.m_curStepIdx)]);
		if (this.TemporalDirections)
		{
			cb.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_TemporalDirections, AmplifyOcclusionBase.m_temporalRotations[(int)(this.m_sampleStep % AmplifyOcclusionBase.m_maxSampleSteps)]);
		}
		else
		{
			cb.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_TemporalDirections, 0f);
		}
		if (this.TemporalOffsets)
		{
			cb.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_TemporalOffsets, AmplifyOcclusionBase.m_spatialOffsets[Mathf.FloorToInt(this.m_sampleStep * 1f / AmplifyOcclusionBase.m_maxSampleSteps) & 3]);
			return;
		}
		cb.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_TemporalOffsets, 0f);
	}

	// Token: 0x060029F8 RID: 10744 RVA: 0x000D5F44 File Offset: 0x000D4144
	private void commandBuffer_FillApplyDeferred(CommandBuffer cb, bool logTarget)
	{
		this.BeginSample(cb, "AO 3 - ApplyDeferred");
		if (!logTarget)
		{
			if (this.FilterEnabled)
			{
				this.commandBuffer_TemporalFilter(cb);
				this.applyDeferredTargetsTemporal[0] = this.m_applyDeferredTargets[0];
				this.applyDeferredTargetsTemporal[1] = this.m_applyDeferredTargets[1];
				this.applyDeferredTargetsTemporal[2] = new RenderTargetIdentifier(this.m_temporalAccumRT[(int)this.m_curStepIdx]);
				cb.SetRenderTarget(this.applyDeferredTargetsTemporal, this.applyDeferredTargetsTemporal[0]);
				this.PerformBlit(cb, AmplifyOcclusionBase.m_applyOcclusionMat, 6 + this.getTemporalPass());
			}
			else
			{
				cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_OcclusionTexture, this.m_occlusionDepthRT);
				cb.SetRenderTarget(this.m_applyDeferredTargets, this.m_applyDeferredTargets[0]);
				this.PerformBlit(cb, AmplifyOcclusionBase.m_applyOcclusionMat, 5);
			}
		}
		else
		{
			int num = this.safeAllocateTemporaryRT(cb, "_AO_tmpAlbedo", this.m_target.fullWidth, this.m_target.fullHeight, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, FilterMode.Point);
			int num2 = this.safeAllocateTemporaryRT(cb, "_AO_tmpEmission", this.m_target.fullWidth, this.m_target.fullHeight, this.m_temporaryEmissionRTFormat, RenderTextureReadWrite.Default, FilterMode.Point);
			cb.Blit(BuiltinRenderTextureType.GBuffer0, num);
			cb.Blit(BuiltinRenderTextureType.GBuffer3, num2);
			cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_GBufferAlbedo, num);
			cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_GBufferEmission, num2);
			if (this.FilterEnabled)
			{
				this.commandBuffer_TemporalFilter(cb);
				this.applyDeferredTargets_Log_Temporal[0] = this.m_applyDeferredTargets_Log[0];
				this.applyDeferredTargets_Log_Temporal[1] = this.m_applyDeferredTargets_Log[1];
				this.applyDeferredTargets_Log_Temporal[2] = new RenderTargetIdentifier(this.m_temporalAccumRT[(int)this.m_curStepIdx]);
				cb.SetRenderTarget(this.applyDeferredTargets_Log_Temporal, this.applyDeferredTargets_Log_Temporal[0]);
				this.PerformBlit(cb, AmplifyOcclusionBase.m_applyOcclusionMat, 11 + this.getTemporalPass());
			}
			else
			{
				cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_OcclusionTexture, this.m_occlusionDepthRT);
				cb.SetRenderTarget(this.m_applyDeferredTargets_Log, this.m_applyDeferredTargets_Log[0]);
				this.PerformBlit(cb, AmplifyOcclusionBase.m_applyOcclusionMat, 10);
			}
			this.safeReleaseTemporaryRT(cb, num);
			this.safeReleaseTemporaryRT(cb, num2);
		}
		cb.SetRenderTarget(null);
		this.EndSample(cb, "AO 3 - ApplyDeferred");
	}

	// Token: 0x060029F9 RID: 10745 RVA: 0x000D61C0 File Offset: 0x000D43C0
	private void commandBuffer_FillApplyPostEffect(CommandBuffer cb)
	{
		this.BeginSample(cb, "AO 3 - ApplyPostEffect");
		if (this.FilterEnabled)
		{
			this.commandBuffer_TemporalFilter(cb);
			this.applyPostEffectTargetsTemporal[0] = BuiltinRenderTextureType.CameraTarget;
			this.applyPostEffectTargetsTemporal[1] = new RenderTargetIdentifier(this.m_temporalAccumRT[(int)this.m_curStepIdx]);
			cb.SetRenderTarget(this.applyPostEffectTargetsTemporal, this.applyPostEffectTargetsTemporal[0]);
			this.PerformBlit(cb, AmplifyOcclusionBase.m_applyOcclusionMat, 16 + this.getTemporalPass());
		}
		else
		{
			cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_OcclusionTexture, this.m_occlusionDepthRT);
			cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
			this.PerformBlit(cb, AmplifyOcclusionBase.m_applyOcclusionMat, 15);
		}
		cb.SetRenderTarget(null);
		this.EndSample(cb, "AO 3 - ApplyPostEffect");
	}

	// Token: 0x060029FA RID: 10746 RVA: 0x000D6290 File Offset: 0x000D4490
	private void commandBuffer_FillApplyDebug(CommandBuffer cb)
	{
		this.BeginSample(cb, "AO 3 - ApplyDebug");
		if (this.FilterEnabled)
		{
			this.commandBuffer_TemporalFilter(cb);
			this.applyDebugTargetsTemporal[0] = BuiltinRenderTextureType.CameraTarget;
			this.applyDebugTargetsTemporal[1] = new RenderTargetIdentifier(this.m_temporalAccumRT[(int)this.m_curStepIdx]);
			cb.SetRenderTarget(this.applyDebugTargetsTemporal, this.applyDebugTargetsTemporal[0]);
			this.PerformBlit(cb, AmplifyOcclusionBase.m_applyOcclusionMat, 1 + this.getTemporalPass());
		}
		else
		{
			cb.SetGlobalTexture(AmplifyOcclusionBase.PropertyID._AO_OcclusionTexture, this.m_occlusionDepthRT);
			cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
			this.PerformBlit(cb, AmplifyOcclusionBase.m_applyOcclusionMat, 0);
		}
		cb.SetRenderTarget(null);
		this.EndSample(cb, "AO 3 - ApplyDebug");
	}

	// Token: 0x060029FB RID: 10747 RVA: 0x000D6360 File Offset: 0x000D4560
	private bool isStereoSinglePassEnabled()
	{
		return this.m_targetCamera.stereoEnabled && XRSettings.eyeTextureDesc.vrUsage == VRTextureUsage.TwoEyes;
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x000D638C File Offset: 0x000D458C
	private void UpdateGlobalShaderConstants()
	{
		if (XRSettings.enabled)
		{
			this.m_target.fullWidth = (int)((float)XRSettings.eyeTextureDesc.width * XRSettings.eyeTextureResolutionScale);
			this.m_target.fullHeight = (int)((float)XRSettings.eyeTextureDesc.height * XRSettings.eyeTextureResolutionScale);
		}
		else
		{
			this.m_target.fullWidth = this.m_targetCamera.pixelWidth;
			this.m_target.fullHeight = this.m_targetCamera.pixelHeight;
		}
		this.m_target.width = this.m_target.fullWidth;
		this.m_target.height = this.m_target.fullHeight;
		this.m_target.oneOverWidth = 1f / (float)this.m_target.width;
		this.m_target.oneOverHeight = 1f / (float)this.m_target.height;
		float num = this.m_targetCamera.fieldOfView * 0.017453292f;
		float num2 = 1f / Mathf.Tan(num * 0.5f);
		Vector2 vector = new Vector2(num2 * ((float)this.m_target.height / (float)this.m_target.width), num2);
		Vector2 vector2 = new Vector2(1f / vector.x, 1f / vector.y);
		Shader.SetGlobalVector(AmplifyOcclusionBase.PropertyID._AO_UVToView, new Vector4(2f * vector2.x, 2f * vector2.y, -1f * vector2.x, -1f * vector2.y));
		float num3;
		if (this.m_targetCamera.orthographic)
		{
			num3 = (float)this.m_target.height / this.m_targetCamera.orthographicSize;
		}
		else
		{
			num3 = (float)this.m_target.height / (Mathf.Tan(num * 0.5f) * 2f);
		}
		if (this.Downsample)
		{
			num3 = num3 * 0.5f * 0.5f;
		}
		else
		{
			num3 *= 0.5f;
		}
		Shader.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_HalfProjScale, num3);
		if (this.FadeEnabled)
		{
			this.FadeStart = Mathf.Max(0f, this.FadeStart);
			this.FadeLength = Mathf.Max(0.01f, this.FadeLength);
			float y = 1f / this.FadeLength;
			Shader.SetGlobalVector(AmplifyOcclusionBase.PropertyID._AO_FadeParams, new Vector2(this.FadeStart, y));
			float num4 = 1f - this.FadeToThickness;
			Shader.SetGlobalVector(AmplifyOcclusionBase.PropertyID._AO_FadeValues, new Vector4(this.FadeToIntensity, this.FadeToRadius, this.FadeToPowerExponent, (1f - num4 * num4) * 0.98f));
			Shader.SetGlobalColor(AmplifyOcclusionBase.PropertyID._AO_FadeToTint, new Color(this.FadeToTint.r, this.FadeToTint.g, this.FadeToTint.b, 0f));
			return;
		}
		Shader.SetGlobalVector(AmplifyOcclusionBase.PropertyID._AO_FadeParams, new Vector2(0f, 0f));
	}

	// Token: 0x060029FD RID: 10749 RVA: 0x000D668C File Offset: 0x000D488C
	private void UpdateGlobalShaderConstants_AmbientOcclusion()
	{
		Shader.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_Radius, this.Radius);
		Shader.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_PixelRadiusLimit, (float)this.PixelRadiusLimit);
		Shader.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_RadiusIntensity, this.RadiusIntensity);
		Shader.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_PowExponent, this.PowerExponent);
		Shader.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_Bias, this.Bias * this.Bias);
		Shader.SetGlobalColor(AmplifyOcclusionBase.PropertyID._AO_Levels, new Color(this.Tint.r, this.Tint.g, this.Tint.b, this.Intensity));
		float num = 1f - this.Thickness;
		Shader.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_ThicknessDecay, (1f - num * num) * 0.98f);
		if (this.BlurEnabled)
		{
			Shader.SetGlobalFloat(AmplifyOcclusionBase.PropertyID._AO_BlurSharpness, this.BlurSharpness * 100f);
		}
	}

	// Token: 0x060029FE RID: 10750 RVA: 0x000D676C File Offset: 0x000D496C
	private void UpdateGlobalShaderConstants_Matrices()
	{
		if (this.isStereoSinglePassEnabled())
		{
			Matrix4x4 stereoViewMatrix = this.m_targetCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
			Matrix4x4 stereoViewMatrix2 = this.m_targetCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
			Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_CameraViewLeft, stereoViewMatrix);
			Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_CameraViewRight, stereoViewMatrix2);
			Matrix4x4 stereoProjectionMatrix = this.m_targetCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
			Matrix4x4 stereoProjectionMatrix2 = this.m_targetCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
			Matrix4x4 gpuprojectionMatrix = GL.GetGPUProjectionMatrix(stereoProjectionMatrix, false);
			Matrix4x4 gpuprojectionMatrix2 = GL.GetGPUProjectionMatrix(stereoProjectionMatrix2, false);
			Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_ProjMatrixLeft, gpuprojectionMatrix);
			Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_ProjMatrixRight, gpuprojectionMatrix2);
			if (this.FilterEnabled)
			{
				Matrix4x4 matrix4x = gpuprojectionMatrix * stereoViewMatrix;
				Matrix4x4 matrix4x2 = gpuprojectionMatrix2 * stereoViewMatrix2;
				Matrix4x4 matrix4x3 = Matrix4x4.Inverse(matrix4x);
				Matrix4x4 matrix4x4 = Matrix4x4.Inverse(matrix4x2);
				Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_InvViewProjMatrixLeft, matrix4x3);
				Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_PrevViewProjMatrixLeft, this.m_prevViewProjMatrixLeft);
				Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_PrevInvViewProjMatrixLeft, this.m_prevInvViewProjMatrixLeft);
				Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_InvViewProjMatrixRight, matrix4x4);
				Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_PrevViewProjMatrixRight, this.m_prevViewProjMatrixRight);
				Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_PrevInvViewProjMatrixRight, this.m_prevInvViewProjMatrixRight);
				this.m_prevViewProjMatrixLeft = matrix4x;
				this.m_prevInvViewProjMatrixLeft = matrix4x3;
				this.m_prevViewProjMatrixRight = matrix4x2;
				this.m_prevInvViewProjMatrixRight = matrix4x4;
				return;
			}
		}
		else
		{
			Matrix4x4 worldToCameraMatrix = this.m_targetCamera.worldToCameraMatrix;
			Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_CameraViewLeft, worldToCameraMatrix);
			if (this.FilterEnabled)
			{
				Matrix4x4 matrix4x5 = GL.GetGPUProjectionMatrix(this.m_targetCamera.projectionMatrix, false) * worldToCameraMatrix;
				Matrix4x4 matrix4x6 = Matrix4x4.Inverse(matrix4x5);
				Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_InvViewProjMatrixLeft, matrix4x6);
				Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_PrevViewProjMatrixLeft, this.m_prevViewProjMatrixLeft);
				Shader.SetGlobalMatrix(AmplifyOcclusionBase.PropertyID._AO_PrevInvViewProjMatrixLeft, this.m_prevInvViewProjMatrixLeft);
				this.m_prevViewProjMatrixLeft = matrix4x5;
				this.m_prevInvViewProjMatrixLeft = matrix4x6;
			}
		}
	}

	// Token: 0x02000787 RID: 1927
	public enum ApplicationMethod
	{
		// Token: 0x04002538 RID: 9528
		PostEffect,
		// Token: 0x04002539 RID: 9529
		Deferred,
		// Token: 0x0400253A RID: 9530
		Debug
	}

	// Token: 0x02000788 RID: 1928
	public enum PerPixelNormalSource
	{
		// Token: 0x0400253C RID: 9532
		None,
		// Token: 0x0400253D RID: 9533
		Camera,
		// Token: 0x0400253E RID: 9534
		GBuffer,
		// Token: 0x0400253F RID: 9535
		GBufferOctaEncoded
	}

	// Token: 0x02000789 RID: 1929
	public enum SampleCountLevel
	{
		// Token: 0x04002541 RID: 9537
		Low,
		// Token: 0x04002542 RID: 9538
		Medium,
		// Token: 0x04002543 RID: 9539
		High,
		// Token: 0x04002544 RID: 9540
		VeryHigh
	}

	// Token: 0x0200078A RID: 1930
	private struct CmdBuffer
	{
		// Token: 0x04002545 RID: 9541
		public CommandBuffer cmdBuffer;

		// Token: 0x04002546 RID: 9542
		public CameraEvent cmdBufferEvent;

		// Token: 0x04002547 RID: 9543
		public string cmdBufferName;
	}

	// Token: 0x0200078B RID: 1931
	private struct TargetDesc
	{
		// Token: 0x04002548 RID: 9544
		public int fullWidth;

		// Token: 0x04002549 RID: 9545
		public int fullHeight;

		// Token: 0x0400254A RID: 9546
		public RenderTextureFormat format;

		// Token: 0x0400254B RID: 9547
		public int width;

		// Token: 0x0400254C RID: 9548
		public int height;

		// Token: 0x0400254D RID: 9549
		public float oneOverWidth;

		// Token: 0x0400254E RID: 9550
		public float oneOverHeight;
	}

	// Token: 0x0200078C RID: 1932
	private static class ShaderPass
	{
		// Token: 0x0400254F RID: 9551
		public const int CombineDownsampledOcclusionDepth = 16;

		// Token: 0x04002550 RID: 9552
		public const int CombineEmission = 17;

		// Token: 0x04002551 RID: 9553
		public const int CombineEmissionLog = 18;

		// Token: 0x04002552 RID: 9554
		public const int BlurHorizontal1 = 0;

		// Token: 0x04002553 RID: 9555
		public const int BlurVertical1 = 1;

		// Token: 0x04002554 RID: 9556
		public const int BlurHorizontal2 = 2;

		// Token: 0x04002555 RID: 9557
		public const int BlurVertical2 = 3;

		// Token: 0x04002556 RID: 9558
		public const int BlurHorizontal3 = 4;

		// Token: 0x04002557 RID: 9559
		public const int BlurVertical3 = 5;

		// Token: 0x04002558 RID: 9560
		public const int BlurHorizontal4 = 6;

		// Token: 0x04002559 RID: 9561
		public const int BlurVertical4 = 7;

		// Token: 0x0400255A RID: 9562
		public const int ApplyDebug = 0;

		// Token: 0x0400255B RID: 9563
		public const int ApplyDebugTemporal = 1;

		// Token: 0x0400255C RID: 9564
		public const int ApplyDeferred = 5;

		// Token: 0x0400255D RID: 9565
		public const int ApplyDeferredTemporal = 6;

		// Token: 0x0400255E RID: 9566
		public const int ApplyDeferredLog = 10;

		// Token: 0x0400255F RID: 9567
		public const int ApplyDeferredLogTemporal = 11;

		// Token: 0x04002560 RID: 9568
		public const int ApplyPostEffect = 15;

		// Token: 0x04002561 RID: 9569
		public const int ApplyPostEffectTemporal = 16;

		// Token: 0x04002562 RID: 9570
		public const int ApplyPostEffectTemporalMultiply = 20;

		// Token: 0x04002563 RID: 9571
		public const int OcclusionLow_None = 0;

		// Token: 0x04002564 RID: 9572
		public const int OcclusionLow_Camera = 1;

		// Token: 0x04002565 RID: 9573
		public const int OcclusionLow_GBuffer = 2;

		// Token: 0x04002566 RID: 9574
		public const int OcclusionLow_GBufferOctaEncoded = 3;
	}

	// Token: 0x0200078D RID: 1933
	private static class PropertyID
	{
		// Token: 0x04002567 RID: 9575
		public static readonly int _AO_Radius = Shader.PropertyToID("_AO_Radius");

		// Token: 0x04002568 RID: 9576
		public static readonly int _AO_PixelRadiusLimit = Shader.PropertyToID("_AO_PixelRadiusLimit");

		// Token: 0x04002569 RID: 9577
		public static readonly int _AO_RadiusIntensity = Shader.PropertyToID("_AO_RadiusIntensity");

		// Token: 0x0400256A RID: 9578
		public static readonly int _AO_PowExponent = Shader.PropertyToID("_AO_PowExponent");

		// Token: 0x0400256B RID: 9579
		public static readonly int _AO_Bias = Shader.PropertyToID("_AO_Bias");

		// Token: 0x0400256C RID: 9580
		public static readonly int _AO_Levels = Shader.PropertyToID("_AO_Levels");

		// Token: 0x0400256D RID: 9581
		public static readonly int _AO_ThicknessDecay = Shader.PropertyToID("_AO_ThicknessDecay");

		// Token: 0x0400256E RID: 9582
		public static readonly int _AO_BlurSharpness = Shader.PropertyToID("_AO_BlurSharpness");

		// Token: 0x0400256F RID: 9583
		public static readonly int _AO_CameraViewLeft = Shader.PropertyToID("_AO_CameraViewLeft");

		// Token: 0x04002570 RID: 9584
		public static readonly int _AO_CameraViewRight = Shader.PropertyToID("_AO_CameraViewRight");

		// Token: 0x04002571 RID: 9585
		public static readonly int _AO_ProjMatrixLeft = Shader.PropertyToID("_AO_ProjMatrixLeft");

		// Token: 0x04002572 RID: 9586
		public static readonly int _AO_ProjMatrixRight = Shader.PropertyToID("_AO_ProjMatrixRight");

		// Token: 0x04002573 RID: 9587
		public static readonly int _AO_InvViewProjMatrixLeft = Shader.PropertyToID("_AO_InvViewProjMatrixLeft");

		// Token: 0x04002574 RID: 9588
		public static readonly int _AO_PrevViewProjMatrixLeft = Shader.PropertyToID("_AO_PrevViewProjMatrixLeft");

		// Token: 0x04002575 RID: 9589
		public static readonly int _AO_PrevInvViewProjMatrixLeft = Shader.PropertyToID("_AO_PrevInvViewProjMatrixLeft");

		// Token: 0x04002576 RID: 9590
		public static readonly int _AO_InvViewProjMatrixRight = Shader.PropertyToID("_AO_InvViewProjMatrixRight");

		// Token: 0x04002577 RID: 9591
		public static readonly int _AO_PrevViewProjMatrixRight = Shader.PropertyToID("_AO_PrevViewProjMatrixRight");

		// Token: 0x04002578 RID: 9592
		public static readonly int _AO_PrevInvViewProjMatrixRight = Shader.PropertyToID("_AO_PrevInvViewProjMatrixRight");

		// Token: 0x04002579 RID: 9593
		public static readonly int _AO_GBufferNormals = Shader.PropertyToID("_AO_GBufferNormals");

		// Token: 0x0400257A RID: 9594
		public static readonly int _AO_Target_TexelSize = Shader.PropertyToID("_AO_Target_TexelSize");

		// Token: 0x0400257B RID: 9595
		public static readonly int _AO_TemporalCurveAdj = Shader.PropertyToID("_AO_TemporalCurveAdj");

		// Token: 0x0400257C RID: 9596
		public static readonly int _AO_TemporalMotionSensibility = Shader.PropertyToID("_AO_TemporalMotionSensibility");

		// Token: 0x0400257D RID: 9597
		public static readonly int _AO_CurrOcclusionDepth = Shader.PropertyToID("_AO_CurrOcclusionDepth");

		// Token: 0x0400257E RID: 9598
		public static readonly int _AO_TemporalAccumm = Shader.PropertyToID("_AO_TemporalAccumm");

		// Token: 0x0400257F RID: 9599
		public static readonly int _AO_TemporalDirections = Shader.PropertyToID("_AO_TemporalDirections");

		// Token: 0x04002580 RID: 9600
		public static readonly int _AO_TemporalOffsets = Shader.PropertyToID("_AO_TemporalOffsets");

		// Token: 0x04002581 RID: 9601
		public static readonly int _AO_OcclusionTexture = Shader.PropertyToID("_AO_OcclusionTexture");

		// Token: 0x04002582 RID: 9602
		public static readonly int _AO_GBufferAlbedo = Shader.PropertyToID("_AO_GBufferAlbedo");

		// Token: 0x04002583 RID: 9603
		public static readonly int _AO_GBufferEmission = Shader.PropertyToID("_AO_GBufferEmission");

		// Token: 0x04002584 RID: 9604
		public static readonly int _AO_UVToView = Shader.PropertyToID("_AO_UVToView");

		// Token: 0x04002585 RID: 9605
		public static readonly int _AO_HalfProjScale = Shader.PropertyToID("_AO_HalfProjScale");

		// Token: 0x04002586 RID: 9606
		public static readonly int _AO_FadeParams = Shader.PropertyToID("_AO_FadeParams");

		// Token: 0x04002587 RID: 9607
		public static readonly int _AO_FadeValues = Shader.PropertyToID("_AO_FadeValues");

		// Token: 0x04002588 RID: 9608
		public static readonly int _AO_FadeToTint = Shader.PropertyToID("_AO_FadeToTint");

		// Token: 0x04002589 RID: 9609
		public static readonly int _AO_Source_TexelSize = Shader.PropertyToID("_AO_Source_TexelSize");

		// Token: 0x0400258A RID: 9610
		public static readonly int _AO_Source = Shader.PropertyToID("_AO_Source");
	}
}
