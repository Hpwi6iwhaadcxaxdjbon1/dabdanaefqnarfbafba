using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020003BA RID: 954
public struct RendererKey : IEquatable<RendererKey>
{
	// Token: 0x040014A8 RID: 5288
	public Material material;

	// Token: 0x040014A9 RID: 5289
	public ShadowCastingMode shadows;

	// Token: 0x040014AA RID: 5290
	public int layer;

	// Token: 0x060017F6 RID: 6134 RVA: 0x00013F5B File Offset: 0x0001215B
	public RendererKey(Material material, ShadowCastingMode shadows, int layer)
	{
		this.material = material;
		this.shadows = shadows;
		this.layer = layer;
	}

	// Token: 0x060017F7 RID: 6135 RVA: 0x00013F72 File Offset: 0x00012172
	public RendererKey(MeshRenderer renderer)
	{
		this.material = renderer.sharedMaterial;
		this.shadows = renderer.shadowCastingMode;
		this.layer = renderer.gameObject.layer;
	}

	// Token: 0x060017F8 RID: 6136 RVA: 0x00013F9D File Offset: 0x0001219D
	public RendererKey(RendererBatch batch)
	{
		this.material = batch.BatchRenderer.sharedMaterial;
		this.shadows = batch.BatchRenderer.shadowCastingMode;
		this.layer = batch.BatchRenderer.gameObject.layer;
	}

	// Token: 0x060017F9 RID: 6137 RVA: 0x0008C1E4 File Offset: 0x0008A3E4
	public override int GetHashCode()
	{
		int hashCode = this.material.GetHashCode();
		int num = (int)this.shadows;
		return hashCode ^ num.GetHashCode() ^ this.layer.GetHashCode();
	}

	// Token: 0x060017FA RID: 6138 RVA: 0x00013FD7 File Offset: 0x000121D7
	public override bool Equals(object other)
	{
		return other is RendererKey && this.Equals((RendererKey)other);
	}

	// Token: 0x060017FB RID: 6139 RVA: 0x00013FEF File Offset: 0x000121EF
	public bool Equals(RendererKey other)
	{
		return this.material == other.material && this.shadows == other.shadows && this.layer == other.layer;
	}
}
