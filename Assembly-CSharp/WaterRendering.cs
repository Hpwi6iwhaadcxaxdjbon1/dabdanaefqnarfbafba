using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000562 RID: 1378
[Serializable]
public class WaterRendering
{
	// Token: 0x04001B61 RID: 7009
	private WaterSystem water;

	// Token: 0x04001B62 RID: 7010
	private WaterSimulation simulation;

	// Token: 0x04001B63 RID: 7011
	private Camera camera;

	// Token: 0x04001B64 RID: 7012
	private PostOpaqueDepth postOpaqueDepth;

	// Token: 0x04001B65 RID: 7013
	private bool initialized;

	// Token: 0x04001B66 RID: 7014
	private WaterRadialMesh radialMesh = new WaterRadialMesh();

	// Token: 0x04001B67 RID: 7015
	private WaterRendering.RenderState state;

	// Token: 0x04001B68 RID: 7016
	private const int MaxCullingVolumes = 4;

	// Token: 0x04001B69 RID: 7017
	private const float MaxCullingVolumeDistanceToCamera = 500f;

	// Token: 0x04001B6A RID: 7018
	private const float MaxCullingVolumeSqrDistanceToCamera = 250000f;

	// Token: 0x04001B6B RID: 7019
	private static Vector4[] cullingVolumeArray = new Vector4[12];

	// Token: 0x04001B6C RID: 7020
	private static Plane[] frustumPlanes = new Plane[6];

	// Token: 0x04001B6D RID: 7021
	private Material reflectionMat;

	// Token: 0x04001B6E RID: 7022
	private Material underwaterMat;

	// Token: 0x04001B6F RID: 7023
	private Material multiCopyMat;

	// Token: 0x04001B70 RID: 7024
	private RenderTexture surfaceTex;

	// Token: 0x04001B71 RID: 7025
	private RenderTexture preFogBackgroundTex;

	// Token: 0x04001B72 RID: 7026
	private RenderTexture ssrReflectionTex;

	// Token: 0x04001B73 RID: 7027
	private UnityEngine.Mesh overlayMesh;

	// Token: 0x04001B74 RID: 7028
	private float underwaterScatterCoefficientOverride = -1f;

	// Token: 0x04001B75 RID: 7029
	private MaterialPropertyBlock underwaterBlock;

	// Token: 0x04001B76 RID: 7030
	private CommandBufferManager commandBufferManager;

	// Token: 0x04001B77 RID: 7031
	private CommandBufferDesc setGlobalsCBDesc;

	// Token: 0x04001B78 RID: 7032
	private CommandBufferDesc waterMaskCBDesc;

	// Token: 0x04001B79 RID: 7033
	private CommandBufferDesc waterDepthCBDesc;

	// Token: 0x04001B7A RID: 7034
	private CommandBufferDesc preFogCBDesc;

	// Token: 0x04001B7B RID: 7035
	private CommandBufferDesc postFogCBDesc;

	// Token: 0x04001B7C RID: 7036
	private RenderTargetIdentifier[] targets = new RenderTargetIdentifier[2];

	// Token: 0x04001B7D RID: 7037
	private bool initializedMaterials;

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x06001F31 RID: 7985 RVA: 0x000189F1 File Offset: 0x00016BF1
	public bool IsInitialized
	{
		get
		{
			return this.initialized;
		}
	}

	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x06001F32 RID: 7986 RVA: 0x000189F9 File Offset: 0x00016BF9
	public Material ReflectionMaterial
	{
		get
		{
			return this.reflectionMat;
		}
	}

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x06001F33 RID: 7987 RVA: 0x00018A01 File Offset: 0x00016C01
	public Material UnderwaterMaterial
	{
		get
		{
			return this.underwaterMat;
		}
	}

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x06001F34 RID: 7988 RVA: 0x00018A09 File Offset: 0x00016C09
	public MaterialPropertyBlock UnderwaterBlock
	{
		get
		{
			this.CheckAndClearUnderwaterBlock();
			return this.underwaterBlock;
		}
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x000AAAE8 File Offset: 0x000A8CE8
	public void Initialize(WaterSystem water, WaterRuntime runtime, int maxVertexCount)
	{
		this.water = water;
		this.simulation = runtime.Simulation;
		this.camera = runtime.Camera;
		this.postOpaqueDepth = runtime.PostOpaqueDepth;
		this.radialMesh.Initialize(maxVertexCount);
		this.reflectionMat = new Material(Shader.Find("Hidden/Water/Reflection"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.underwaterMat = new Material(Shader.Find("Hidden/Water/Underwater"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.multiCopyMat = new Material(Shader.Find("Hidden/MultiCopy"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.CheckRenderTextures();
		this.CheckCommandBuffer();
		this.CheckOverlayMesh();
		this.initializedMaterials = false;
		this.initialized = true;
	}

	// Token: 0x06001F36 RID: 7990 RVA: 0x000AABA4 File Offset: 0x000A8DA4
	public void Destroy()
	{
		if (this.initialized)
		{
			this.CleanupCommandBuffer();
			this.CleanupRenderTextures();
			this.CleanupOverlayMesh();
			this.radialMesh.Destroy();
			Object.DestroyImmediate(this.reflectionMat);
			Object.DestroyImmediate(this.underwaterMat);
			Object.DestroyImmediate(this.multiCopyMat);
			this.initializedMaterials = false;
			this.initialized = false;
		}
	}

	// Token: 0x06001F37 RID: 7991 RVA: 0x000AAC08 File Offset: 0x000A8E08
	private void CheckCommandBuffer()
	{
		if (this.commandBufferManager == null)
		{
			this.commandBufferManager = this.camera.GetComponent<CommandBufferManager>();
		}
		if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
		{
			this.setGlobalsCBDesc = ((this.setGlobalsCBDesc == null) ? new CommandBufferDesc(CameraEvent.AfterGBuffer, 299, new CommandBufferDesc.FillCommandBuffer(this.FillCommandBuffer_SetGlobals)) : this.setGlobalsCBDesc);
			this.waterMaskCBDesc = ((this.waterMaskCBDesc == null) ? new CommandBufferDesc(CameraEvent.AfterGBuffer, 300, new CommandBufferDesc.FillCommandBuffer(this.FillCommandBuffer_Mask)) : this.waterMaskCBDesc);
			this.waterDepthCBDesc = ((this.waterDepthCBDesc == null) ? new CommandBufferDesc(CameraEvent.BeforeLighting, 300, new CommandBufferDesc.FillCommandBuffer(this.FillCommandBuffer_Depth)) : this.waterDepthCBDesc);
			this.preFogCBDesc = ((this.preFogCBDesc == null) ? new CommandBufferDesc(CameraEvent.BeforeImageEffectsOpaque, 300, new CommandBufferDesc.FillCommandBuffer(this.FillCommandBuffer_PreFog)) : this.preFogCBDesc);
			this.postFogCBDesc = ((this.postFogCBDesc == null) ? new CommandBufferDesc(CameraEvent.AfterImageEffectsOpaque, 300, new CommandBufferDesc.FillCommandBuffer(this.FillCommandBuffer_PostFog)) : this.postFogCBDesc);
			this.commandBufferManager.AddCommands(this.setGlobalsCBDesc);
			this.commandBufferManager.AddCommands(this.waterMaskCBDesc);
			this.commandBufferManager.AddCommands(this.waterDepthCBDesc);
			this.commandBufferManager.AddCommands(this.preFogCBDesc);
			this.commandBufferManager.AddCommands(this.postFogCBDesc);
		}
	}

	// Token: 0x06001F38 RID: 7992 RVA: 0x000AAD90 File Offset: 0x000A8F90
	private void CleanupCommandBuffer()
	{
		if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
		{
			this.commandBufferManager.RemoveCommands(this.setGlobalsCBDesc);
			this.commandBufferManager.RemoveCommands(this.waterMaskCBDesc);
			this.commandBufferManager.RemoveCommands(this.waterDepthCBDesc);
			this.commandBufferManager.RemoveCommands(this.preFogCBDesc);
			this.commandBufferManager.RemoveCommands(this.postFogCBDesc);
			this.commandBufferManager = null;
		}
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x00018A17 File Offset: 0x00016C17
	public bool CheckLostData()
	{
		return (this.surfaceTex != null && !this.surfaceTex.IsCreated()) || (this.ssrReflectionTex != null && !this.ssrReflectionTex.IsCreated());
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x000AAE14 File Offset: 0x000A9014
	private static void CheckCreateRenderTexture(ref RenderTexture rt, string name, int width, int height, RenderTextureFormat format, bool linear, FilterMode filter = FilterMode.Point)
	{
		if (rt == null || rt.width != width || rt.height != height)
		{
			WaterRendering.SafeDestroyRenderTexture(ref rt);
			rt = new RenderTexture(width, height, 0, format, linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB)
			{
				hideFlags = HideFlags.DontSave
			};
			rt.name = name;
			rt.wrapMode = TextureWrapMode.Clamp;
			rt.filterMode = filter;
			rt.isPowerOfTwo = false;
			rt.autoGenerateMips = false;
			rt.Create();
		}
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x00018A54 File Offset: 0x00016C54
	private static void SafeDestroyRenderTexture(ref RenderTexture rt)
	{
		if (rt != null)
		{
			rt.Release();
			Object.DestroyImmediate(rt);
			rt = null;
		}
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x000AAE94 File Offset: 0x000A9094
	private void CheckRenderTextures()
	{
		int width = Mathf.Clamp(this.camera.pixelWidth, 1, 65536);
		int height = Mathf.Clamp(this.camera.pixelHeight, 1, 65536);
		WaterRendering.CheckCreateRenderTexture(ref this.surfaceTex, "Water-SurfaceTex", width, height, RenderTextureFormat.ARGB2101010, true, FilterMode.Point);
		WaterRendering.CheckCreateRenderTexture(ref this.preFogBackgroundTex, "Water-PreFogBackgroundTex", width, height, RenderTextureFormat.ARGB32, false, FilterMode.Point);
		if (this.state.reflections > 0)
		{
			WaterRendering.CheckCreateRenderTexture(ref this.ssrReflectionTex, "Water-SSRReflectionTex", width, height, RenderTextureFormat.ARGB32, false, FilterMode.Point);
			return;
		}
		WaterRendering.SafeDestroyRenderTexture(ref this.ssrReflectionTex);
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x00018A71 File Offset: 0x00016C71
	private void CleanupRenderTextures()
	{
		RenderTexture.active = null;
		WaterRendering.SafeDestroyRenderTexture(ref this.surfaceTex);
		WaterRendering.SafeDestroyRenderTexture(ref this.preFogBackgroundTex);
		WaterRendering.SafeDestroyRenderTexture(ref this.ssrReflectionTex);
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x000AAF28 File Offset: 0x000A9128
	private void CheckOverlayMesh()
	{
		this.overlayMesh = new UnityEngine.Mesh();
		this.overlayMesh.vertices = new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(1f, 1f, 0f),
			new Vector3(1f, 0f, 0f)
		};
		this.overlayMesh.uv = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		this.overlayMesh.triangles = new int[]
		{
			0,
			1,
			2,
			0,
			2,
			3
		};
		this.overlayMesh.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x00018A9A File Offset: 0x00016C9A
	private void CleanupOverlayMesh()
	{
		if (this.overlayMesh != null)
		{
			Object.DestroyImmediate(this.overlayMesh);
			this.overlayMesh = null;
		}
	}

	// Token: 0x06001F40 RID: 8000 RVA: 0x00018ABC File Offset: 0x00016CBC
	private bool IsVisibleToCamera()
	{
		return this.state.camera != null && (this.state.camera.cullingMask & 1 << this.state.layer) != 0;
	}

	// Token: 0x06001F41 RID: 8001 RVA: 0x000AB06C File Offset: 0x000A926C
	private void FillCommandBuffer_SetGlobals(CommandBuffer cb)
	{
		int num = Mathf.Clamp(this.camera.pixelWidth, 1, 65536);
		int num2 = Mathf.Clamp(this.camera.pixelHeight, 1, 65536);
		Matrix4x4 lhs = Matrix4x4.Scale(new Vector3((float)num, (float)num2, 1f)) * Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0f), Quaternion.identity, new Vector3(0.5f, 0.5f, 1f));
		Matrix4x4 worldToCameraMatrix = this.camera.worldToCameraMatrix;
		Matrix4x4 value = GL.GetGPUProjectionMatrix(this.camera.projectionMatrix, true) * worldToCameraMatrix;
		Matrix4x4 value2 = lhs * GL.GetGPUProjectionMatrix(this.camera.projectionMatrix, false);
		cb.SetGlobalMatrix("_Water_CameraView", worldToCameraMatrix);
		cb.SetGlobalMatrix("_Water_CameraViewProj", value);
		cb.SetGlobalMatrix("_WaterSSR_CameraProj", value2);
		if (Camera.current != MainCamera.mainCamera)
		{
			Matrix4x4 value3;
			Matrix4x4 value4;
			MainCamera.ComputeCameraFrustumCorners(this.camera, out value3, out value4);
			cb.SetGlobalMatrix("_FrustumNearCorners", value3);
			cb.SetGlobalMatrix("_FrustumRayCorners", value4);
		}
		cb.SetGlobalTexture("_WaterSurfaceTexture", this.surfaceTex);
		cb.SetGlobalTexture("_WaterPreFogBackgroundTexture", this.preFogBackgroundTex);
		if (this.state.reflections > 0)
		{
			WaterRendering.CheckCreateRenderTexture(ref this.ssrReflectionTex, "Water-SSRReflectionTex", num, num2, RenderTextureFormat.ARGB32, false, FilterMode.Point);
			cb.SetGlobalTexture("_WaterSSR_WorldNormTexture", this.surfaceTex);
			cb.SetGlobalTexture("_WaterSSR_ReflectionTexture", this.ssrReflectionTex);
			cb.SetGlobalVector("_WaterSSR_SourceTexture_TexelSize", new Vector4(1f / (float)num, 1f / (float)num2, (float)num, (float)num2));
		}
		if (this.state.simulation)
		{
			cb.SetGlobalTexture("_Water_NoiseMap", this.simulation.PerlinNoiseMap);
			cb.SetGlobalTexture("_Water_NormalFoldMap", this.simulation.NormalFoldMap);
			cb.SetGlobalVector("_Water_NormalFoldMap_TexelSize", this.simulation.DisplacementMapTexelSize);
		}
		if (this.state.displacement)
		{
			cb.SetGlobalTexture("_Water_DisplacementMap", this.simulation.DisplacementMap);
			cb.SetGlobalVector("_Water_DisplacementMap_TexelSize", this.simulation.DisplacementMapTexelSize);
		}
		int num3 = Mathf.Min(this.state.cullingVolumes.Count, 4);
		for (int i = 0; i < num3; i++)
		{
			WaterCullingVolume waterCullingVolume = this.state.cullingVolumes[i];
			WaterRendering.cullingVolumeArray[i * 3] = waterCullingVolume.WorldToLocal[0];
			WaterRendering.cullingVolumeArray[i * 3 + 1] = waterCullingVolume.WorldToLocal[1];
			WaterRendering.cullingVolumeArray[i * 3 + 2] = waterCullingVolume.WorldToLocal[2];
		}
		cb.SetGlobalVectorArray("_Water_CullingVolumeArray", WaterRendering.cullingVolumeArray);
		cb.SetGlobalInt("_Water_CullingVolumeCount", num3);
		cb.SetGlobalVector("_Water_WindDirection", this.water.Simulation.Wind.normalized);
	}

	// Token: 0x06001F42 RID: 8002 RVA: 0x000AB398 File Offset: 0x000A9598
	private void FillCommandBuffer_Mask(CommandBuffer cb)
	{
		RenderTargetIdentifier depth = new RenderTargetIdentifier(this.postOpaqueDepth.PostOpaque);
		cb.SetRenderTarget(this.surfaceTex, depth);
		cb.ClearRenderTarget(true, true, new Color(0f, 0f, 1f, 0f), 1f);
		foreach (WaterDepthMask waterDepthMask in WaterSystem.DepthMasks)
		{
			cb.DrawMesh(waterDepthMask.Mesh, waterDepthMask.transform.localToWorldMatrix, waterDepthMask.Material, 0, 1);
		}
	}

	// Token: 0x06001F43 RID: 8003 RVA: 0x000AB44C File Offset: 0x000A964C
	private void FillCommandBuffer_Depth(CommandBuffer cb)
	{
		if (this.IsVisibleToCamera() && this.state.visibilityMask > 0)
		{
			RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(this.postOpaqueDepth.PostOpaque);
			this.targets[0] = renderTargetIdentifier;
			this.targets[1] = this.surfaceTex;
			cb.SetRenderTarget(this.targets, renderTargetIdentifier);
			foreach (WaterBody waterBody in WaterSystem.WaterBodies)
			{
				if ((this.state.visibilityMask & (int)waterBody.Type) != 0)
				{
					int shaderPass = (this.state.displacement && waterBody.DepthDisplacementPass >= 0) ? waterBody.DepthDisplacementPass : waterBody.DepthPass;
					if (waterBody.Type == WaterBodyType.Ocean)
					{
						Matrix4x4 matrix = this.radialMesh.ComputeLocalToWorldMatrix(this.state.camera, waterBody.Transform.position.y);
						for (int i = 0; i < this.radialMesh.Meshes.Length; i++)
						{
							cb.DrawMesh(this.radialMesh.Meshes[i], matrix, waterBody.Material, 0, shaderPass);
						}
					}
					else
					{
						cb.DrawMesh(waterBody.SharedMesh, waterBody.Transform.localToWorldMatrix, waterBody.Material, 0, waterBody.DepthPass);
					}
				}
			}
			if (WaterSystem.Ocean != null)
			{
				if (!this.state.caustics)
				{
					cb.Blit(null, BuiltinRenderTextureType.GBuffer0, WaterSystem.Ocean.Material, WaterSystem.Ocean.OcclusionPass);
					return;
				}
				this.targets[0] = BuiltinRenderTextureType.GBuffer0;
				this.targets[1] = BuiltinRenderTextureType.CameraTarget;
				cb.SetRenderTarget(this.targets, BuiltinRenderTextureType.GBuffer0);
				cb.DrawMesh(this.overlayMesh, Matrix4x4.identity, WaterSystem.Ocean.Material, 0, WaterSystem.Ocean.OcclusionCausticsPass);
			}
		}
	}

	// Token: 0x06001F44 RID: 8004 RVA: 0x00018AF7 File Offset: 0x00016CF7
	private void FillCommandBuffer_PreFog(CommandBuffer cb)
	{
		if (this.IsVisibleToCamera() && this.state.visibilityMask > 0)
		{
			cb.Blit(BuiltinRenderTextureType.CameraTarget, this.preFogBackgroundTex, this.multiCopyMat, 1);
		}
	}

	// Token: 0x06001F45 RID: 8005 RVA: 0x000AB668 File Offset: 0x000A9868
	private void FillCommandBuffer_PostFog(CommandBuffer cb)
	{
		if (this.IsVisibleToCamera() && this.state.visibilityMask > 0 && this.state.reflections > 0)
		{
			int nameID = Shader.PropertyToID("_BackgroundColorTexture");
			cb.GetTemporaryRT(nameID, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32);
			cb.Blit(BuiltinRenderTextureType.CameraTarget, nameID, this.multiCopyMat, 1);
			cb.SetGlobalTexture("_BackgroundColorTexture", nameID);
			cb.Blit(null, this.ssrReflectionTex, this.reflectionMat, this.state.reflections);
			cb.ReleaseTemporaryRT(nameID);
		}
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x00018B2D File Offset: 0x00016D2D
	private void CheckAndClearUnderwaterBlock()
	{
		this.underwaterBlock = ((this.underwaterBlock != null) ? this.underwaterBlock : new MaterialPropertyBlock());
		this.underwaterBlock.Clear();
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x00018B55 File Offset: 0x00016D55
	public void ClearUnderwaterScatterCoefficientOverride()
	{
		this.underwaterScatterCoefficientOverride = -1f;
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x00018B62 File Offset: 0x00016D62
	public void SetUnderwaterScatterCoefficientOverride(float scatterCoefficient)
	{
		this.underwaterScatterCoefficientOverride = scatterCoefficient;
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x00018B6B File Offset: 0x00016D6B
	public void UpdateUnderwaterMaterial(Material reference)
	{
		this.underwaterMat.CopyPropertiesFromMaterial(reference);
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x000AB708 File Offset: 0x000A9908
	public static List<WaterCullingVolume> FindAndSortVisibleCullingVolumes(Camera camera, List<WaterCullingVolume> list)
	{
		if (list == null)
		{
			list = new List<WaterCullingVolume>();
		}
		else
		{
			list.Clear();
		}
		Vector3 position = camera.transform.position;
		CameraUtil.ExtractPlanes(camera, ref WaterRendering.frustumPlanes);
		foreach (WaterCullingVolume waterCullingVolume in WaterCullingVolume.Volumes)
		{
			if (Vector3.SqrMagnitude(waterCullingVolume.WorldBounds.center - position) < 250000f && waterCullingVolume != null && waterCullingVolume.UpdateVisibility(WaterRendering.frustumPlanes, position))
			{
				list.Add(waterCullingVolume);
			}
		}
		list.Sort((WaterCullingVolume x, WaterCullingVolume y) => x.DistanceToCamera.CompareTo(y.DistanceToCamera));
		return list;
	}

	// Token: 0x06001F4B RID: 8011 RVA: 0x000AB7E0 File Offset: 0x000A99E0
	public void PrepareRender(int visibilityMask)
	{
		if (this.initialized)
		{
			this.CheckRenderTextures();
			this.CheckCommandBuffer();
			CameraUtil.ExtractPlanes(this.camera, ref WaterRendering.frustumPlanes);
			this.state.camera = this.camera;
			this.state.layer = this.water.gameObject.layer;
			this.state.simulation = (this.water.Quality >= WaterQuality.Medium);
			this.state.displacement = (this.water.Quality >= WaterQuality.Medium);
			this.state.reflections = Water.reflections;
			this.state.caustics = this.water.HasCaustics;
			this.state.visibilityMask = visibilityMask;
			this.state.cullingVolumes = WaterRendering.FindAndSortVisibleCullingVolumes(this.state.camera, this.state.cullingVolumes);
			if (!this.initializedMaterials && WaterSystem.Ocean != null)
			{
				this.UpdateUnderwaterMaterial(WaterSystem.Ocean.Material);
				this.initializedMaterials = true;
			}
		}
	}

	// Token: 0x06001F4C RID: 8012 RVA: 0x000AB8FC File Offset: 0x000A9AFC
	private bool IssueRender(bool debug, bool depthOnly)
	{
		if (!this.initialized || this.state.camera == null)
		{
			return false;
		}
		int num = 0;
		if (!debug && this.IsVisibleToCamera() && this.state.visibilityMask > 0)
		{
			foreach (WaterBody waterBody in WaterSystem.WaterBodies)
			{
				if ((this.state.visibilityMask & (int)waterBody.Type) != 0)
				{
					if (waterBody.Type == WaterBodyType.Ocean)
					{
						Matrix4x4 matrix = this.radialMesh.ComputeLocalToWorldMatrix(this.state.camera, waterBody.Transform.position.y);
						for (int i = 0; i < this.radialMesh.Meshes.Length; i++)
						{
							UnityEngine.Graphics.DrawMesh(this.radialMesh.Meshes[i], matrix, waterBody.Material, waterBody.gameObject.layer, this.state.camera, 0, null, false, false);
							num++;
						}
					}
					else
					{
						num += (waterBody.Renderer.isVisible ? 1 : 0);
					}
				}
			}
			if ((WaterSystem.Collision != null && MainCamera.mainCamera != null && MainCamera.isWaterVisible) || num > 0)
			{
				this.CheckAndClearUnderwaterBlock();
				if (this.underwaterScatterCoefficientOverride > 0f)
				{
					this.underwaterBlock.SetFloat("_ScatterCoefficient", this.underwaterScatterCoefficientOverride);
				}
				UnityEngine.Graphics.DrawMesh(this.overlayMesh, Matrix4x4.identity, this.underwaterMat, 0, this.state.camera, 0, this.underwaterBlock);
			}
		}
		return num > 0;
	}

	// Token: 0x06001F4D RID: 8013 RVA: 0x00018B79 File Offset: 0x00016D79
	public bool IssueRender(bool debug)
	{
		return this.IssueRender(debug, false);
	}

	// Token: 0x06001F4E RID: 8014 RVA: 0x000ABAC0 File Offset: 0x000A9CC0
	public void ShowDebug()
	{
		if (this.initialized)
		{
			if (this.surfaceTex != null)
			{
				GUI.DrawTexture(new Rect((float)(Screen.width - 512), 0f, 512f, 512f), this.surfaceTex, 0, false);
			}
			if (this.ssrReflectionTex != null)
			{
				GUI.DrawTexture(new Rect((float)(Screen.width - 512), 512f, 512f, 512f), this.ssrReflectionTex, 0, false);
			}
		}
	}

	// Token: 0x02000563 RID: 1379
	private struct RenderState
	{
		// Token: 0x04001B7E RID: 7038
		public Camera camera;

		// Token: 0x04001B7F RID: 7039
		public int layer;

		// Token: 0x04001B80 RID: 7040
		public bool simulation;

		// Token: 0x04001B81 RID: 7041
		public bool displacement;

		// Token: 0x04001B82 RID: 7042
		public int reflections;

		// Token: 0x04001B83 RID: 7043
		public bool caustics;

		// Token: 0x04001B84 RID: 7044
		public int visibilityMask;

		// Token: 0x04001B85 RID: 7045
		public List<WaterCullingVolume> cullingVolumes;
	}
}
