using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000581 RID: 1409
[ExecuteInEditMode]
public class DeferredDecalRenderer : BaseCommandBuffer
{
	// Token: 0x04001C2A RID: 7210
	private static ListDictionary<InstancingKey, InstancingBuffer>[] DiffuseBuffer = new ListDictionary<InstancingKey, InstancingBuffer>[]
	{
		new ListDictionary<InstancingKey, InstancingBuffer>(8),
		new ListDictionary<InstancingKey, InstancingBuffer>(8)
	};

	// Token: 0x04001C2B RID: 7211
	private static ListDictionary<InstancingKey, InstancingBuffer>[] SpecularBuffer = new ListDictionary<InstancingKey, InstancingBuffer>[]
	{
		new ListDictionary<InstancingKey, InstancingBuffer>(8),
		new ListDictionary<InstancingKey, InstancingBuffer>(8)
	};

	// Token: 0x04001C2C RID: 7212
	private static ListDictionary<InstancingKey, InstancingBuffer>[] NormalsBuffer = new ListDictionary<InstancingKey, InstancingBuffer>[]
	{
		new ListDictionary<InstancingKey, InstancingBuffer>(8),
		new ListDictionary<InstancingKey, InstancingBuffer>(8)
	};

	// Token: 0x04001C2D RID: 7213
	private static ListDictionary<InstancingKey, InstancingBuffer>[] EmissionBuffer = new ListDictionary<InstancingKey, InstancingBuffer>[]
	{
		new ListDictionary<InstancingKey, InstancingBuffer>(8),
		new ListDictionary<InstancingKey, InstancingBuffer>(8)
	};

	// Token: 0x04001C2E RID: 7214
	private static ListDictionary<InstancingKey, InstancingBuffer>[] CombinedBuffer = new ListDictionary<InstancingKey, InstancingBuffer>[]
	{
		new ListDictionary<InstancingKey, InstancingBuffer>(8),
		new ListDictionary<InstancingKey, InstancingBuffer>(8)
	};

	// Token: 0x04001C2F RID: 7215
	private static MaterialPropertyBlock block;

	// Token: 0x06002036 RID: 8246 RVA: 0x000AF660 File Offset: 0x000AD860
	private void RefreshCommandBuffer(Camera camera)
	{
		CommandBuffer commandBuffer = base.GetCommandBuffer("Deferred Decals", camera, CameraEvent.BeforeLighting);
		if (DeferredDecalSystem.IsEmpty)
		{
			return;
		}
		int nameID = Shader.PropertyToID("_NormalsCopy");
		commandBuffer.GetTemporaryRT(nameID, -1, -1);
		commandBuffer.Blit(BuiltinRenderTextureType.GBuffer2, nameID);
		for (int i = 0; i < 2; i++)
		{
			this.Clear(DeferredDecalRenderer.DiffuseBuffer[i]);
			this.Clear(DeferredDecalRenderer.SpecularBuffer[i]);
			this.Clear(DeferredDecalRenderer.NormalsBuffer[i]);
			this.Clear(DeferredDecalRenderer.EmissionBuffer[i]);
			this.Clear(DeferredDecalRenderer.CombinedBuffer[i]);
			DeferredDecalRenderer.block = ((DeferredDecalRenderer.block != null) ? DeferredDecalRenderer.block : new MaterialPropertyBlock());
			DeferredDecalRenderer.block.CopySHCoefficientArraysFrom(MainCamera.LightProbe);
			this.Apply(commandBuffer, DeferredDecalSystem.DiffuseRenderTarget, DeferredDecalSystem.DiffusePass, DeferredDecalSystem.DiffuseDecals[i], DeferredDecalRenderer.DiffuseBuffer[i], DeferredDecalRenderer.block);
			this.Apply(commandBuffer, DeferredDecalSystem.SpecularRenderTarget, DeferredDecalSystem.SpecularPass, DeferredDecalSystem.SpecularDecals[i], DeferredDecalRenderer.SpecularBuffer[i], DeferredDecalRenderer.block);
			this.Apply(commandBuffer, DeferredDecalSystem.NormalsRenderTarget, DeferredDecalSystem.NormalsPass, DeferredDecalSystem.NormalsDecals[i], DeferredDecalRenderer.NormalsBuffer[i], DeferredDecalRenderer.block);
			this.Apply(commandBuffer, DeferredDecalSystem.CombinedRenderTarget, DeferredDecalSystem.CombinedPass, DeferredDecalSystem.CombinedDecals[i], DeferredDecalRenderer.CombinedBuffer[i], DeferredDecalRenderer.block);
			this.Apply(commandBuffer, DeferredDecalSystem.EmissionRenderTarget, DeferredDecalSystem.EmissionPass, DeferredDecalSystem.EmissionDecals[i], DeferredDecalRenderer.EmissionBuffer[i], DeferredDecalRenderer.block);
		}
		commandBuffer.ReleaseTemporaryRT(nameID);
	}

	// Token: 0x06002037 RID: 8247 RVA: 0x000AF7E0 File Offset: 0x000AD9E0
	private void Clear(ListDictionary<InstancingKey, InstancingBuffer> dict)
	{
		int count = dict.Values.Count;
		InstancingBuffer[] buffer = dict.Values.Buffer;
		for (int i = 0; i < count; i++)
		{
			buffer[i].Clear();
		}
	}

	// Token: 0x06002038 RID: 8248 RVA: 0x000AF81C File Offset: 0x000ADA1C
	private void Apply(CommandBuffer buf, RenderTargetIdentifier[] target, int pass, ListDictionary<InstancingKey, ListHashSet<DeferredDecal>> src, ListDictionary<InstancingKey, InstancingBuffer> dst, MaterialPropertyBlock block)
	{
		int count = src.Count;
		if (count == 0)
		{
			return;
		}
		InstancingKey[] buffer = src.Keys.Buffer;
		ListHashSet<DeferredDecal>[] buffer2 = src.Values.Buffer;
		for (int i = 0; i < count; i++)
		{
			this.Apply(buf, target, pass, buffer2[i], this.GetBuffer(dst, buffer[i]), block);
		}
	}

	// Token: 0x06002039 RID: 8249 RVA: 0x000AF878 File Offset: 0x000ADA78
	private void Apply(CommandBuffer buf, RenderTargetIdentifier[] target, int pass, ListHashSet<DeferredDecal> src, InstancingBuffer dst, MaterialPropertyBlock block)
	{
		int num = Mathf.Min(src.Values.Count, Decal.limit);
		if (num == 0)
		{
			return;
		}
		DeferredDecal[] buffer = src.Values.Buffer;
		buf.SetRenderTarget(target, BuiltinRenderTextureType.CameraTarget);
		for (int i = 0; i < num; i++)
		{
			dst.Add(buffer[i].matrix);
		}
		dst.Apply(buf, Decal.instancing, block);
	}

	// Token: 0x0600203A RID: 8250 RVA: 0x000AF8E4 File Offset: 0x000ADAE4
	private InstancingBuffer GetBuffer(ListDictionary<InstancingKey, InstancingBuffer> dict, InstancingKey key)
	{
		InstancingBuffer instancingBuffer;
		if (!dict.TryGetValue(key, ref instancingBuffer))
		{
			instancingBuffer = new InstancingBuffer(key, Decal.capacity);
			dict.Add(key, instancingBuffer);
		}
		return instancingBuffer;
	}

	// Token: 0x0600203B RID: 8251 RVA: 0x00019828 File Offset: 0x00017A28
	protected void OnPreRender()
	{
		if (Application.isPlaying && Decal.cache && !DeferredDecalSystem.IsDirty)
		{
			return;
		}
		this.RefreshCommandBuffer(Camera.current);
		DeferredDecalSystem.IsDirty = false;
	}
}
