using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020001F3 RID: 499
public class MeshRendererInfo : ComponentInfo<MeshRenderer>
{
	// Token: 0x04000C8F RID: 3215
	public ShadowCastingMode shadows;

	// Token: 0x04000C90 RID: 3216
	public Material material;

	// Token: 0x04000C91 RID: 3217
	public Mesh mesh;

	// Token: 0x06000FA0 RID: 4000 RVA: 0x0006AFA8 File Offset: 0x000691A8
	public override void Reset()
	{
		this.component.shadowCastingMode = this.shadows;
		if (this.material)
		{
			this.component.sharedMaterial = this.material;
		}
		this.component.GetComponent<MeshFilter>().sharedMesh = this.mesh;
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x0000DE5B File Offset: 0x0000C05B
	public override void Setup()
	{
		this.shadows = this.component.shadowCastingMode;
		this.material = this.component.sharedMaterial;
		this.mesh = this.component.GetComponent<MeshFilter>().sharedMesh;
	}
}
