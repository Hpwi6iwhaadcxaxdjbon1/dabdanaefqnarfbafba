using System;
using UnityEngine;

// Token: 0x0200027A RID: 634
public class EmissionToggle : MonoBehaviour, IClientComponent
{
	// Token: 0x04000EC7 RID: 3783
	private Color emissionColor;

	// Token: 0x04000EC8 RID: 3784
	public Renderer[] targetRenderers;

	// Token: 0x04000EC9 RID: 3785
	public int materialIndex = -1;

	// Token: 0x04000ECA RID: 3786
	private static MaterialPropertyBlock block;

	// Token: 0x0600123B RID: 4667 RVA: 0x00077B60 File Offset: 0x00075D60
	public void OnEnable()
	{
		if (this.targetRenderers == null || this.targetRenderers.Length == 0 || this.targetRenderers[0] == null)
		{
			return;
		}
		if (this.materialIndex != -1)
		{
			this.emissionColor = this.targetRenderers[0].sharedMaterials[this.materialIndex].GetColor("_EmissionColor");
		}
		else
		{
			this.emissionColor = this.targetRenderers[0].sharedMaterial.GetColor("_EmissionColor");
		}
		if (EmissionToggle.block == null)
		{
			EmissionToggle.block = new MaterialPropertyBlock();
		}
	}

	// Token: 0x0600123C RID: 4668 RVA: 0x00077BEC File Offset: 0x00075DEC
	public void SetEmissionEnabled(bool on)
	{
		if (EmissionToggle.block == null)
		{
			EmissionToggle.block = new MaterialPropertyBlock();
		}
		EmissionToggle.block.Clear();
		EmissionToggle.block.SetColor("_EmissionColor", on ? this.emissionColor : Color.black);
		Renderer[] array = this.targetRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetPropertyBlock(EmissionToggle.block);
		}
	}
}
