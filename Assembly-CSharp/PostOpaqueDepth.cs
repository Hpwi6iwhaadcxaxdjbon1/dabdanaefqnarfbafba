using System;
using Rust;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020005AA RID: 1450
[ExecuteInEditMode]
[RequireComponent(typeof(CommandBufferManager))]
public class PostOpaqueDepth : MonoBehaviour
{
	// Token: 0x04001D09 RID: 7433
	public RenderTexture postOpaqueDepth;

	// Token: 0x04001D0A RID: 7434
	private Camera camera;

	// Token: 0x04001D0B RID: 7435
	private CommandBufferManager commandBufferManager;

	// Token: 0x04001D0C RID: 7436
	private CommandBufferDesc commandBufferDesc;

	// Token: 0x04001D0D RID: 7437
	private Material copyDepthMat;

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x06002145 RID: 8517 RVA: 0x0001A707 File Offset: 0x00018907
	public RenderTexture PostOpaque
	{
		get
		{
			return this.postOpaqueDepth;
		}
	}

	// Token: 0x17000234 RID: 564
	// (get) Token: 0x06002146 RID: 8518 RVA: 0x0001A70F File Offset: 0x0001890F
	public Material CopyDepthMat
	{
		get
		{
			return this.copyDepthMat;
		}
	}

	// Token: 0x06002147 RID: 8519 RVA: 0x0001A717 File Offset: 0x00018917
	private void OnEnable()
	{
		this.camera = base.GetComponent<Camera>();
		this.CreateMaterial();
		this.CheckBindRenderTextures();
		this.CheckCommandBuffer();
	}

	// Token: 0x06002148 RID: 8520 RVA: 0x0001A738 File Offset: 0x00018938
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.CleanupCommandBuffer();
		this.DestroyMaterial();
		this.DestroyRenderTextures();
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x000B3590 File Offset: 0x000B1790
	private void CheckCommandBuffer()
	{
		if (this.commandBufferManager == null)
		{
			this.commandBufferManager = base.GetComponent<CommandBufferManager>();
		}
		if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
		{
			this.commandBufferDesc = ((this.commandBufferDesc == null) ? new CommandBufferDesc(CameraEvent.BeforeLighting, 200, new CommandBufferDesc.FillCommandBuffer(this.FillCommandBuffer)) : this.commandBufferDesc);
			this.commandBufferManager.AddCommands(this.commandBufferDesc);
		}
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x0001A754 File Offset: 0x00018954
	private void CleanupCommandBuffer()
	{
		if (this.commandBufferManager != null && this.commandBufferManager.IsReady)
		{
			this.commandBufferManager.RemoveCommands(this.commandBufferDesc);
			this.commandBufferManager = null;
		}
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x0001A789 File Offset: 0x00018989
	private void CreateMaterial()
	{
		this.copyDepthMat = new Material(Shader.Find("Hidden/CopyCameraDepth"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	// Token: 0x0600214C RID: 8524 RVA: 0x0001A7A8 File Offset: 0x000189A8
	private void DestroyMaterial()
	{
		if (this.copyDepthMat != null)
		{
			Object.DestroyImmediate(this.copyDepthMat);
			this.copyDepthMat = null;
		}
	}

	// Token: 0x0600214D RID: 8525 RVA: 0x0001A7CA File Offset: 0x000189CA
	private void DestroyRenderTextures()
	{
		if (this.postOpaqueDepth != null)
		{
			this.postOpaqueDepth.Release();
			Object.DestroyImmediate(this.postOpaqueDepth);
			this.postOpaqueDepth = null;
		}
	}

	// Token: 0x0600214E RID: 8526 RVA: 0x000B3610 File Offset: 0x000B1810
	private bool CheckBindRenderTextures()
	{
		int num = Mathf.Clamp(this.camera.pixelWidth, 1, 65536);
		int num2 = Mathf.Clamp(this.camera.pixelHeight, 1, 65536);
		bool result = false;
		if (num > 0 && num2 > 0)
		{
			if (this.postOpaqueDepth == null || num != this.postOpaqueDepth.width || num2 != this.postOpaqueDepth.height)
			{
				if (this.postOpaqueDepth != null)
				{
					this.DestroyRenderTextures();
				}
				this.postOpaqueDepth = new RenderTexture(num, num2, 24, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear)
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				this.postOpaqueDepth.name = "PostOpaqueDepthRT";
				this.postOpaqueDepth.filterMode = FilterMode.Point;
				this.postOpaqueDepth.wrapMode = TextureWrapMode.Clamp;
				this.postOpaqueDepth.isPowerOfTwo = false;
				this.postOpaqueDepth.autoGenerateMips = false;
				this.postOpaqueDepth.Create();
			}
			result = true;
		}
		return result;
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x0001A7F7 File Offset: 0x000189F7
	private void FillCommandBuffer(CommandBuffer cb)
	{
		cb.Blit(null, new RenderTargetIdentifier(this.postOpaqueDepth), this.copyDepthMat);
		cb.SetGlobalTexture("_PostOpaqueCameraDepthTexture", this.postOpaqueDepth);
	}

	// Token: 0x06002150 RID: 8528 RVA: 0x0001A827 File Offset: 0x00018A27
	public void Update()
	{
		this.CheckBindRenderTextures();
		this.CheckCommandBuffer();
	}
}
