using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000588 RID: 1416
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CommandBufferManager))]
public class DeferredExtension : MonoBehaviour
{
	// Token: 0x04001C50 RID: 7248
	public ExtendGBufferParams extendGBuffer = ExtendGBufferParams.Default;

	// Token: 0x04001C51 RID: 7249
	public SubsurfaceScatteringParams subsurfaceScattering = SubsurfaceScatteringParams.Default;

	// Token: 0x04001C52 RID: 7250
	public Texture2D blueNoise;

	// Token: 0x04001C53 RID: 7251
	public float depthScale = 100f;

	// Token: 0x04001C54 RID: 7252
	public bool debug;

	// Token: 0x04001C55 RID: 7253
	private Camera _targetCamera;

	// Token: 0x04001C56 RID: 7254
	private CommandBufferManager commandBufferManager;

	// Token: 0x04001C57 RID: 7255
	private CommandBufferDesc extendGBufferCBDesc;

	// Token: 0x04001C58 RID: 7256
	private CommandBufferDesc postSubsurfaceCBDesc;

	// Token: 0x04001C59 RID: 7257
	private Material postSubsurfaceMat;

	// Token: 0x04001C5A RID: 7258
	private int frameIndexMod8;

	// Token: 0x04001C5B RID: 7259
	private PostProcessLayer post;

	// Token: 0x04001C5C RID: 7260
	private int gbufferWidth;

	// Token: 0x04001C5D RID: 7261
	private int gbufferHeight;

	// Token: 0x04001C5E RID: 7262
	private RenderTexture gbufferTexture4;

	// Token: 0x04001C5F RID: 7263
	private RenderTexture gbufferTexture5;

	// Token: 0x04001C60 RID: 7264
	private RenderTargetIdentifier[] targets = new RenderTargetIdentifier[2];

	// Token: 0x04001C61 RID: 7265
	private static HashSet<DeferredExtensionMesh> registeredMeshes = new HashSet<DeferredExtensionMesh>();

	// Token: 0x04001C62 RID: 7266
	private static HashSet<DeferredExtensionMesh> visibleMeshes = new HashSet<DeferredExtensionMesh>();

	// Token: 0x1700020B RID: 523
	// (get) Token: 0x0600204F RID: 8271 RVA: 0x000B0198 File Offset: 0x000AE398
	private Camera targetCamera
	{
		get
		{
			return this._targetCamera = ((this._targetCamera != null) ? this._targetCamera : base.GetComponent<Camera>());
		}
	}

	// Token: 0x06002050 RID: 8272 RVA: 0x0001989C File Offset: 0x00017A9C
	public static void Register(DeferredExtensionMesh mesh)
	{
		DeferredExtension.registeredMeshes.Add(mesh);
	}

	// Token: 0x06002051 RID: 8273 RVA: 0x000198AA File Offset: 0x00017AAA
	public static void Unregister(DeferredExtensionMesh mesh)
	{
		DeferredExtension.registeredMeshes.Remove(mesh);
	}

	// Token: 0x06002052 RID: 8274 RVA: 0x000198B8 File Offset: 0x00017AB8
	private void Awake()
	{
		this.post = base.GetComponent<PostProcessLayer>();
		this.frameIndexMod8 = 0;
	}

	// Token: 0x06002053 RID: 8275 RVA: 0x000198CD File Offset: 0x00017ACD
	private void OnEnable()
	{
		this.CheckCommandBuffers();
		this.CreateMaterials();
		this.targetCamera.depthTextureMode |= DepthTextureMode.Depth;
	}

	// Token: 0x06002054 RID: 8276 RVA: 0x000198EE File Offset: 0x00017AEE
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.CleanupCommandBuffers();
		this.CleanupMaterials();
		this.CleanupSystemTextures();
		Shader.DisableKeyword("POSTPROCESS_SCATTERING");
	}

	// Token: 0x06002055 RID: 8277 RVA: 0x000B01CC File Offset: 0x000AE3CC
	private void CheckCommandBuffers()
	{
		if (this.commandBufferManager == null)
		{
			this.commandBufferManager = base.GetComponent<CommandBufferManager>();
		}
		if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
		{
			this.extendGBufferCBDesc = ((this.extendGBufferCBDesc == null) ? new CommandBufferDesc(CameraEvent.AfterGBuffer, 0, new CommandBufferDesc.FillCommandBuffer(this.FillCommandBuffer_ExtendGBuffer)) : this.extendGBufferCBDesc);
			this.postSubsurfaceCBDesc = ((this.postSubsurfaceCBDesc == null) ? new CommandBufferDesc(CameraEvent.BeforeImageEffectsOpaque, 0, new CommandBufferDesc.FillCommandBuffer(this.FillCommandBuffer_PostSubsurface)) : this.postSubsurfaceCBDesc);
			this.commandBufferManager.AddCommands(this.extendGBufferCBDesc);
			this.commandBufferManager.AddCommands(this.postSubsurfaceCBDesc);
		}
	}

	// Token: 0x06002056 RID: 8278 RVA: 0x000B0288 File Offset: 0x000AE488
	private void CleanupCommandBuffers()
	{
		if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
		{
			this.commandBufferManager.RemoveCommands(this.extendGBufferCBDesc);
			this.commandBufferManager.RemoveCommands(this.postSubsurfaceCBDesc);
			this.commandBufferManager = null;
		}
	}

	// Token: 0x06002057 RID: 8279 RVA: 0x00019914 File Offset: 0x00017B14
	private void CreateMaterials()
	{
		this.CleanupMaterials();
		this.postSubsurfaceMat = new Material(Shader.Find("Hidden/PostProcessSubsurface"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	// Token: 0x06002058 RID: 8280 RVA: 0x00019939 File Offset: 0x00017B39
	private void CleanupMaterials()
	{
		if (this.postSubsurfaceMat != null)
		{
			Object.DestroyImmediate(this.postSubsurfaceMat);
			this.postSubsurfaceMat = null;
		}
	}

	// Token: 0x06002059 RID: 8281 RVA: 0x000B02DC File Offset: 0x000AE4DC
	private void BindSystemTextures()
	{
		if (this.gbufferTexture4 != null)
		{
			Shader.SetGlobalTexture(this.gbufferTexture4.name, this.gbufferTexture4);
		}
		if (this.gbufferTexture5 != null)
		{
			Shader.SetGlobalTexture(this.gbufferTexture5.name, this.gbufferTexture5);
		}
	}

	// Token: 0x0600205A RID: 8282 RVA: 0x0001995B File Offset: 0x00017B5B
	private void SafeCleanupTexture<T>(ref T tex) where T : Texture
	{
		if (tex != null)
		{
			Object.DestroyImmediate(tex);
			tex = default(T);
		}
	}

	// Token: 0x0600205B RID: 8283 RVA: 0x00019987 File Offset: 0x00017B87
	private void CleanupSystemTextures()
	{
		this.SafeCleanupTexture<RenderTexture>(ref this.gbufferTexture4);
		this.SafeCleanupTexture<RenderTexture>(ref this.gbufferTexture5);
	}

	// Token: 0x0600205C RID: 8284 RVA: 0x000B0334 File Offset: 0x000AE534
	private void CheckSystemTextures()
	{
		if ((this.extendGBuffer.enabled && this.targetCamera.pixelWidth != this.gbufferWidth) || this.targetCamera.pixelHeight != this.gbufferHeight)
		{
			this.SafeCleanupTexture<RenderTexture>(ref this.gbufferTexture4);
			this.SafeCleanupTexture<RenderTexture>(ref this.gbufferTexture5);
			this.gbufferWidth = this.targetCamera.pixelWidth;
			this.gbufferHeight = this.targetCamera.pixelHeight;
			this.gbufferTexture4 = new RenderTexture(this.gbufferWidth, this.gbufferHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			this.gbufferTexture4.name = "_GBufferExtendedTexture4";
			this.gbufferTexture4.Create();
			this.gbufferTexture5 = new RenderTexture(this.gbufferWidth, this.gbufferHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			this.gbufferTexture5.name = "_GBufferExtendedTexture5";
			this.gbufferTexture5.Create();
			this.BindSystemTextures();
		}
	}

	// Token: 0x0600205D RID: 8285 RVA: 0x000199A1 File Offset: 0x00017BA1
	public static void BecameVisible(DeferredExtensionMesh mesh)
	{
		DeferredExtension.visibleMeshes.Add(mesh);
	}

	// Token: 0x0600205E RID: 8286 RVA: 0x000199AF File Offset: 0x00017BAF
	public static void BecameInvisible(DeferredExtensionMesh mesh)
	{
		DeferredExtension.visibleMeshes.Remove(mesh);
	}

	// Token: 0x0600205F RID: 8287 RVA: 0x000B0424 File Offset: 0x000AE624
	public void FillCommandBuffer_ExtendGBuffer(CommandBuffer cb)
	{
		if (base.enabled && this.extendGBuffer.enabled && DeferredExtension.visibleMeshes.Count > 0)
		{
			this.targets[0] = new RenderTargetIdentifier(this.gbufferTexture4);
			this.targets[1] = new RenderTargetIdentifier(this.gbufferTexture5);
			cb.SetRenderTarget(this.targets, BuiltinRenderTextureType.CameraTarget);
			foreach (DeferredExtensionMesh deferredExtensionMesh in DeferredExtension.visibleMeshes)
			{
				if (deferredExtensionMesh != null)
				{
					deferredExtensionMesh.AddToCommandBuffer(cb);
				}
			}
		}
	}

	// Token: 0x06002060 RID: 8288 RVA: 0x000B04C4 File Offset: 0x000AE6C4
	public void FillCommandBuffer_PostSubsurface(CommandBuffer cb)
	{
		if (base.enabled && this.subsurfaceScattering.enabled && SubsurfaceProfile.Texture != null)
		{
			int pixelWidth = this.targetCamera.pixelWidth;
			int pixelHeight = this.targetCamera.pixelHeight;
			int nameID = Shader.PropertyToID("_SubsurfaceFinal");
			cb.GetTemporaryRT(nameID, pixelWidth, pixelHeight, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
			int pass = this.subsurfaceScattering.halfResolution ? 1 : 0;
			int pass2 = this.subsurfaceScattering.halfResolution ? 5 : 4;
			int width = this.subsurfaceScattering.halfResolution ? (pixelWidth / 2) : pixelWidth;
			int height = this.subsurfaceScattering.halfResolution ? (pixelHeight / 2) : pixelHeight;
			int nameID2 = Shader.PropertyToID("_SubsurfaceSetupId");
			cb.GetTemporaryRT(nameID2, width, height, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
			cb.SetGlobalTexture("_CameraGBufferTexture0", BuiltinRenderTextureType.GBuffer0);
			cb.SetGlobalTexture("_CameraGBufferTexture1", BuiltinRenderTextureType.GBuffer1);
			cb.SetGlobalTexture("_CameraGBufferTexture2", BuiltinRenderTextureType.GBuffer2);
			cb.SetGlobalTexture("_CameraGBufferTexture3", BuiltinRenderTextureType.CameraTarget);
			cb.SetGlobalVector("_CameraTarget_TexelSize", new Vector4(1f / (float)pixelWidth, 1f / (float)pixelHeight, (float)pixelWidth, (float)pixelHeight));
			cb.Blit(BuiltinRenderTextureType.CameraTarget, nameID2, this.postSubsurfaceMat, pass);
			int nameID3 = Shader.PropertyToID("_SubsurfaceX");
			cb.GetTemporaryRT(nameID3, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf);
			float num = this.targetCamera.projectionMatrix.m00 * this.subsurfaceScattering.radiusScale;
			float x = num / 3f * 0.5f;
			cb.SetGlobalTexture("_SubsurfaceProfileTexture", SubsurfaceProfile.Texture);
			cb.SetGlobalVector("_SubsurfaceProfileParams", new Vector3(x, num, this.depthScale));
			cb.Blit(nameID2, nameID3, this.postSubsurfaceMat, 2);
			cb.ReleaseTemporaryRT(nameID2);
			int nameID4 = Shader.PropertyToID("_SubsurfaceY");
			cb.GetTemporaryRT(nameID4, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf);
			cb.Blit(nameID3, nameID4, this.postSubsurfaceMat, 3);
			cb.ReleaseTemporaryRT(nameID3);
			cb.Blit(nameID4, nameID, this.postSubsurfaceMat, pass2);
			cb.ReleaseTemporaryRT(nameID4);
			cb.Blit(nameID, BuiltinRenderTextureType.CameraTarget);
			cb.ReleaseTemporaryRT(nameID);
			return;
		}
		this.CheckShaderKeywords();
	}

	// Token: 0x06002061 RID: 8289 RVA: 0x000B073C File Offset: 0x000AE93C
	private void CheckConsoleVars()
	{
		this.subsurfaceScattering.enabled = SSS.enabled;
		this.subsurfaceScattering.quality = (SubsurfaceScatteringParams.Quality)SSS.quality;
		this.subsurfaceScattering.halfResolution = SSS.halfres;
		this.subsurfaceScattering.radiusScale = SSS.scale;
	}

	// Token: 0x06002062 RID: 8290 RVA: 0x000B078C File Offset: 0x000AE98C
	private void CheckShaderKeywords()
	{
		KeywordUtil.EnsureKeywordState("POSTPROCESS_SCATTERING", this.subsurfaceScattering.enabled);
		KeywordUtil.EnsureKeywordState(this.postSubsurfaceMat, "SSS_QUALITY_MEDIUM", this.subsurfaceScattering.quality == SubsurfaceScatteringParams.Quality.Medium);
		KeywordUtil.EnsureKeywordState(this.postSubsurfaceMat, "SSS_QUALITY_HIGH", this.subsurfaceScattering.quality == SubsurfaceScatteringParams.Quality.High);
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x000B07EC File Offset: 0x000AE9EC
	private void SetShaderGlobals()
	{
		if (this.targetCamera != null)
		{
			Matrix4x4 worldToCameraMatrix = this.targetCamera.worldToCameraMatrix;
			Matrix4x4 gpuprojectionMatrix = GL.GetGPUProjectionMatrix(this.targetCamera.projectionMatrix, false);
			Matrix4x4 value = gpuprojectionMatrix * worldToCameraMatrix;
			Shader.SetGlobalMatrix("global_ViewProjMatrix", value);
			Shader.SetGlobalMatrix("global_ProjMatrix", gpuprojectionMatrix);
			Shader.SetGlobalMatrix("global_InvProjMatrix", gpuprojectionMatrix.inverse);
		}
		Vector4 zero = Vector4.zero;
		this.frameIndexMod8 = (this.frameIndexMod8 + 1) % 8;
		if (this.post != null)
		{
			zero = new Vector4((float)this.post.temporalAntialiasing.sampleIndex, (float)this.post.temporalAntialiasing.sampleCount, 0f, 0f);
		}
		else
		{
			zero = Vector4.zero;
		}
		Shader.SetGlobalFloat("global_DepthScale", this.depthScale);
		Shader.SetGlobalFloat("global_FrameIndexMod8", (float)this.frameIndexMod8);
		Shader.SetGlobalVector("global_TemporalAAParams", zero);
		if (this.blueNoise != null)
		{
			Shader.SetGlobalTexture("global_BlueNoise", this.blueNoise);
			Shader.SetGlobalFloat("global_BlueNoiseRcpSize", 1f / (float)this.blueNoise.width);
		}
	}

	// Token: 0x06002064 RID: 8292 RVA: 0x000199BD File Offset: 0x00017BBD
	private void Update()
	{
		this.CheckCommandBuffers();
		this.CheckSystemTextures();
		this.CheckConsoleVars();
		this.CheckShaderKeywords();
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x000199D7 File Offset: 0x00017BD7
	private void OnPreRender()
	{
		this.SetShaderGlobals();
	}

	// Token: 0x06002066 RID: 8294 RVA: 0x000199DF File Offset: 0x00017BDF
	public static float LightIntensityScale(float intensity)
	{
		return Mathf.GammaToLinearSpace(intensity) * 3.1415927f;
	}

	// Token: 0x02000589 RID: 1417
	private enum SubsurfacePass
	{
		// Token: 0x04001C64 RID: 7268
		SetupFullRes,
		// Token: 0x04001C65 RID: 7269
		SetupHalfRes,
		// Token: 0x04001C66 RID: 7270
		Direction0,
		// Token: 0x04001C67 RID: 7271
		Direction1,
		// Token: 0x04001C68 RID: 7272
		RecombineFullRes,
		// Token: 0x04001C69 RID: 7273
		RecombineHalfRes,
		// Token: 0x04001C6A RID: 7274
		RecombinePassthrough
	}
}
