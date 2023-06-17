using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000595 RID: 1429
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class DeferredMeshDecalRenderer : MonoBehaviour
{
	// Token: 0x04001C8E RID: 7310
	private Camera targetCamera;

	// Token: 0x04001C8F RID: 7311
	private const string commandBufferName = "DeferredMeshDecals";

	// Token: 0x04001C90 RID: 7312
	private CameraEvent commandBufferEvent = CameraEvent.AfterGBuffer;

	// Token: 0x04001C91 RID: 7313
	private CommandBuffer commandBuffer;

	// Token: 0x04001C92 RID: 7314
	private RenderTargetIdentifier[] targets = new RenderTargetIdentifier[4];

	// Token: 0x04001C93 RID: 7315
	private Material multiCopyMat;

	// Token: 0x04001C94 RID: 7316
	private MaterialPropertyBlock block;

	// Token: 0x04001C95 RID: 7317
	private static HashSet<DeferredMeshDecal> registered = new HashSet<DeferredMeshDecal>();

	// Token: 0x04001C96 RID: 7318
	private static HashSet<DeferredMeshDecal> visible = new HashSet<DeferredMeshDecal>();

	// Token: 0x04001C97 RID: 7319
	private static Dictionary<DeferredMeshDecal.InstanceData, SimpleList<Matrix4x4>> batches = new Dictionary<DeferredMeshDecal.InstanceData, SimpleList<Matrix4x4>>();

	// Token: 0x17000217 RID: 535
	// (get) Token: 0x060020A4 RID: 8356 RVA: 0x00019D8B File Offset: 0x00017F8B
	public Camera TargetCamera
	{
		get
		{
			if (this.targetCamera == null)
			{
				this.targetCamera = base.GetComponent<Camera>();
			}
			return this.targetCamera;
		}
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x00019DAD File Offset: 0x00017FAD
	public static void Register(DeferredMeshDecal decal)
	{
		DeferredMeshDecalRenderer.registered.Add(decal);
	}

	// Token: 0x060020A6 RID: 8358 RVA: 0x00019DBB File Offset: 0x00017FBB
	public static void Unregister(DeferredMeshDecal decal)
	{
		DeferredMeshDecalRenderer.registered.Remove(decal);
	}

	// Token: 0x060020A7 RID: 8359 RVA: 0x00019DC9 File Offset: 0x00017FC9
	private void OnEnable()
	{
		this.CreateCommandBuffer();
		this.CreateMaterials();
	}

	// Token: 0x060020A8 RID: 8360 RVA: 0x00019DD7 File Offset: 0x00017FD7
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.CleanupBatches();
		this.CleanupCommandBuffer();
		this.CleanupMaterials();
	}

	// Token: 0x060020A9 RID: 8361 RVA: 0x00019DF3 File Offset: 0x00017FF3
	private void CreateCommandBuffer()
	{
		this.CleanupCommandBuffer();
		this.commandBuffer = new CommandBuffer();
		this.commandBuffer.name = "DeferredMeshDecals";
		this.TargetCamera.AddCommandBuffer(this.commandBufferEvent, this.commandBuffer);
	}

	// Token: 0x060020AA RID: 8362 RVA: 0x000B157C File Offset: 0x000AF77C
	private void CleanupCommandBuffer()
	{
		CommandBuffer[] commandBuffers = this.TargetCamera.GetCommandBuffers(this.commandBufferEvent);
		for (int i = 0; i < commandBuffers.Length; i++)
		{
			if (commandBuffers[i].name == "DeferredMeshDecals")
			{
				this.TargetCamera.RemoveCommandBuffer(this.commandBufferEvent, commandBuffers[i]);
			}
		}
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x00019E2D File Offset: 0x0001802D
	private void CreateMaterials()
	{
		this.CleanupMaterials();
		this.multiCopyMat = new Material(Shader.Find("Hidden/MultiCopy"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	// Token: 0x060020AC RID: 8364 RVA: 0x00019E52 File Offset: 0x00018052
	private void CleanupMaterials()
	{
		if (this.multiCopyMat != null)
		{
			Object.DestroyImmediate(this.multiCopyMat);
			this.multiCopyMat = null;
		}
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x00019E74 File Offset: 0x00018074
	public static void BecameVisible(DeferredMeshDecal decal)
	{
		DeferredMeshDecalRenderer.visible.Add(decal);
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x00019E82 File Offset: 0x00018082
	public static void BecameInvisible(DeferredMeshDecal decal)
	{
		DeferredMeshDecalRenderer.visible.Remove(decal);
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x000B15D4 File Offset: 0x000AF7D4
	private void CleanupBatches()
	{
		foreach (KeyValuePair<DeferredMeshDecal.InstanceData, SimpleList<Matrix4x4>> keyValuePair in DeferredMeshDecalRenderer.batches)
		{
			SimpleList<Matrix4x4> value = keyValuePair.Value;
			Pool.Free<SimpleList<Matrix4x4>>(ref value);
		}
		DeferredMeshDecalRenderer.batches.Clear();
	}

	// Token: 0x060020B0 RID: 8368 RVA: 0x000B1638 File Offset: 0x000AF838
	private void UpdateBatches()
	{
		this.CleanupBatches();
		foreach (DeferredMeshDecal deferredMeshDecal in DeferredMeshDecalRenderer.visible)
		{
			if (deferredMeshDecal != null)
			{
				foreach (DeferredMeshDecal.InstanceData instanceData in deferredMeshDecal.GetInstanceData())
				{
					SimpleList<Matrix4x4> simpleList = null;
					DeferredMeshDecalRenderer.batches.TryGetValue(instanceData, ref simpleList);
					if (simpleList == null)
					{
						simpleList = Pool.Get<SimpleList<Matrix4x4>>();
						simpleList.Clear();
						DeferredMeshDecalRenderer.batches.Add(instanceData, simpleList);
					}
					simpleList.Add(instanceData.LocalToWorld);
				}
			}
		}
	}

	// Token: 0x060020B1 RID: 8369 RVA: 0x000B1710 File Offset: 0x000AF910
	public void FillCommandBuffer(CommandBuffer cb)
	{
		cb.Clear();
		if (DeferredMeshDecalRenderer.visible.Count > 0)
		{
			this.UpdateBatches();
			int nameID = Shader.PropertyToID("_GBuffer0Copy");
			int nameID2 = Shader.PropertyToID("_GBuffer1Copy");
			Shader.PropertyToID("_GBuffer2Copy");
			Shader.PropertyToID("_GBuffer3Copy");
			cb.GetTemporaryRT(nameID, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32);
			cb.GetTemporaryRT(nameID2, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32);
			cb.Blit(BuiltinRenderTextureType.GBuffer0, nameID);
			cb.Blit(BuiltinRenderTextureType.GBuffer1, nameID2);
			cb.SetGlobalTexture("_GBuffer0Copy", nameID);
			cb.SetGlobalTexture("_GBuffer1Copy", nameID2);
			this.targets[0] = BuiltinRenderTextureType.GBuffer0;
			this.targets[1] = BuiltinRenderTextureType.GBuffer1;
			this.targets[2] = BuiltinRenderTextureType.GBuffer2;
			this.targets[3] = BuiltinRenderTextureType.CameraTarget;
			cb.SetRenderTarget(this.targets, BuiltinRenderTextureType.CameraTarget);
			this.block = ((this.block != null) ? this.block : new MaterialPropertyBlock());
			this.block.CopySHCoefficientArraysFrom(MainCamera.LightProbe);
			foreach (KeyValuePair<DeferredMeshDecal.InstanceData, SimpleList<Matrix4x4>> keyValuePair in DeferredMeshDecalRenderer.batches)
			{
				if (keyValuePair.Key.Material.enableInstancing)
				{
					cb.DrawMeshInstanced(keyValuePair.Key.Mesh, keyValuePair.Key.SubmeshIndex, keyValuePair.Key.Material, 0, keyValuePair.Value.Array, keyValuePair.Value.Count, this.block);
				}
				else
				{
					for (int i = 0; i < keyValuePair.Value.Count; i++)
					{
						cb.DrawMesh(keyValuePair.Key.Mesh, keyValuePair.Value.Array[i], keyValuePair.Key.Material, keyValuePair.Key.SubmeshIndex, 0, this.block);
					}
				}
			}
			cb.ReleaseTemporaryRT(nameID);
			cb.ReleaseTemporaryRT(nameID2);
		}
	}

	// Token: 0x060020B2 RID: 8370 RVA: 0x00019E90 File Offset: 0x00018090
	private void OnPreRender()
	{
		if (this.TargetCamera != null && this.commandBuffer != null)
		{
			this.FillCommandBuffer(this.commandBuffer);
		}
	}
}
