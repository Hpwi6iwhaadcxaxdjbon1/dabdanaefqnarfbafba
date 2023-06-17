using System;
using UnityEngine;

// Token: 0x02000280 RID: 640
public class MaterialSwap : MonoBehaviour, IClientComponent
{
	// Token: 0x04000EF3 RID: 3827
	public int materialIndex;

	// Token: 0x04000EF4 RID: 3828
	public Renderer myRenderer;

	// Token: 0x04000EF5 RID: 3829
	public Material OverrideMaterial;

	// Token: 0x04000EF6 RID: 3830
	private bool initialized;

	// Token: 0x04000EF7 RID: 3831
	private Material[] originalMaterials;

	// Token: 0x06001257 RID: 4695 RVA: 0x00078668 File Offset: 0x00076868
	public void SetOverrideEnabled(bool on)
	{
		if (!this.initialized)
		{
			this.initialized = true;
			if (this.myRenderer == null)
			{
				this.myRenderer = base.GetComponent<Renderer>();
			}
			this.originalMaterials = this.myRenderer.sharedMaterials;
		}
		Material[] sharedMaterials = this.myRenderer.sharedMaterials;
		sharedMaterials[this.materialIndex] = (on ? this.OverrideMaterial : this.originalMaterials[this.materialIndex]);
		this.myRenderer.materials = sharedMaterials;
	}
}
