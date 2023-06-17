using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020005B1 RID: 1457
public static class CommandBufferEx
{
	// Token: 0x0600218F RID: 8591 RVA: 0x000B5E80 File Offset: 0x000B4080
	public static void BlitArray(this CommandBuffer cb, Mesh blitMesh, RenderTargetIdentifier source, Material mat, int slice, int pass = 0)
	{
		cb.SetGlobalTexture("_Source", source);
		cb.SetGlobalFloat("_SourceMip", 0f);
		if (slice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)slice);
			cb.SetGlobalInt("_TargetSlice", slice);
		}
		cb.DrawMesh(blitMesh, Matrix4x4.identity, mat, 0, pass);
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x000B5EDC File Offset: 0x000B40DC
	public static void BlitArray(this CommandBuffer cb, Mesh blitMesh, RenderTargetIdentifier source, Texture target, Material mat, int slice, int pass = 0)
	{
		cb.SetRenderTarget(target, 0, CubemapFace.PositiveX, -1);
		cb.SetGlobalTexture("_Source", source);
		cb.SetGlobalFloat("_SourceMip", 0f);
		if (slice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)slice);
			cb.SetGlobalInt("_TargetSlice", slice);
		}
		cb.DrawMesh(blitMesh, Matrix4x4.identity, mat, 0, pass);
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x000B5F48 File Offset: 0x000B4148
	public static void BlitArrayMip(this CommandBuffer cb, Mesh blitMesh, Texture source, int sourceMip, int sourceSlice, Texture target, int targetMip, int targetSlice, Material mat, int pass = 0)
	{
		int num = source.width >> sourceMip;
		int num2 = source.height >> sourceMip;
		Vector4 value = new Vector4(1f / (float)num, 1f / (float)num2, (float)num, (float)num2);
		int num3 = target.width >> targetMip;
		int num4 = target.height >> targetMip;
		Vector4 value2 = new Vector4(1f / (float)num3, 1f / (float)num4, (float)num3, (float)num4);
		cb.SetGlobalTexture("_Source", source);
		cb.SetGlobalVector("_Source_TexelSize", value);
		cb.SetGlobalVector("_Target_TexelSize", value2);
		cb.SetGlobalFloat("_SourceMip", (float)sourceMip);
		if (sourceSlice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)sourceSlice);
			cb.SetGlobalInt("_TargetSlice", targetSlice);
		}
		cb.SetRenderTarget(target, targetMip, CubemapFace.PositiveX, -1);
		cb.DrawMesh(blitMesh, Matrix4x4.identity, mat, 0, pass);
	}

	// Token: 0x06002192 RID: 8594 RVA: 0x000B603C File Offset: 0x000B423C
	public static void BlitMip(this CommandBuffer cb, Mesh blitMesh, Texture source, Texture target, int mip, int slice, Material mat, int pass = 0)
	{
		cb.BlitArrayMip(blitMesh, source, mip, slice, target, mip, slice, mat, pass);
	}
}
