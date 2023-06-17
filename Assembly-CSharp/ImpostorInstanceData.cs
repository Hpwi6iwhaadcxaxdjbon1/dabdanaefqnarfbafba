using System;
using UnityEngine;

// Token: 0x0200059E RID: 1438
public class ImpostorInstanceData
{
	// Token: 0x04001CB7 RID: 7351
	public ImpostorBatch Batch;

	// Token: 0x04001CB8 RID: 7352
	public int BatchIndex;

	// Token: 0x04001CB9 RID: 7353
	private int hash;

	// Token: 0x17000219 RID: 537
	// (get) Token: 0x060020D4 RID: 8404 RVA: 0x0001A0E2 File Offset: 0x000182E2
	// (set) Token: 0x060020D3 RID: 8403 RVA: 0x0001A0D9 File Offset: 0x000182D9
	public Renderer Renderer { get; private set; }

	// Token: 0x1700021A RID: 538
	// (get) Token: 0x060020D6 RID: 8406 RVA: 0x0001A0F3 File Offset: 0x000182F3
	// (set) Token: 0x060020D5 RID: 8405 RVA: 0x0001A0EA File Offset: 0x000182EA
	public Mesh Mesh { get; private set; }

	// Token: 0x1700021B RID: 539
	// (get) Token: 0x060020D8 RID: 8408 RVA: 0x0001A104 File Offset: 0x00018304
	// (set) Token: 0x060020D7 RID: 8407 RVA: 0x0001A0FB File Offset: 0x000182FB
	public Material Material { get; private set; }

	// Token: 0x060020D9 RID: 8409 RVA: 0x0001A10C File Offset: 0x0001830C
	public ImpostorInstanceData(Renderer renderer, Mesh mesh, Material material)
	{
		this.Renderer = renderer;
		this.Mesh = mesh;
		this.Material = material;
		this.hash = this.GenerateHashCode();
		this.Update();
	}

	// Token: 0x060020DA RID: 8410 RVA: 0x0001A13B File Offset: 0x0001833B
	private int GenerateHashCode()
	{
		return (17 * 31 + this.Material.GetHashCode()) * 31 + this.Mesh.GetHashCode();
	}

	// Token: 0x060020DB RID: 8411 RVA: 0x000B1C68 File Offset: 0x000AFE68
	public override bool Equals(object obj)
	{
		ImpostorInstanceData impostorInstanceData = obj as ImpostorInstanceData;
		return impostorInstanceData.Material == this.Material && impostorInstanceData.Mesh == this.Mesh;
	}

	// Token: 0x060020DC RID: 8412 RVA: 0x0001A15D File Offset: 0x0001835D
	public override int GetHashCode()
	{
		return this.hash;
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x000B1CA4 File Offset: 0x000AFEA4
	public Vector4 PositionAndScale()
	{
		Transform transform = this.Renderer.transform;
		Vector3 position = transform.position;
		Vector3 lossyScale = transform.lossyScale;
		float w = this.Renderer.enabled ? lossyScale.x : (-lossyScale.x);
		return new Vector4(position.x, position.y, position.z, w);
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x0001A165 File Offset: 0x00018365
	public void Update()
	{
		if (this.Batch != null)
		{
			this.Batch.Positions[this.BatchIndex] = this.PositionAndScale();
		}
	}
}
