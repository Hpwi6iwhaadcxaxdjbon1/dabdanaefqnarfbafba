using System;
using UnityEngine;

// Token: 0x02000279 RID: 633
public class EmissionScaledByLight : MonoBehaviour, IClientComponent
{
	// Token: 0x04000EC0 RID: 3776
	private Color emissionColor;

	// Token: 0x04000EC1 RID: 3777
	public Renderer[] targetRenderers;

	// Token: 0x04000EC2 RID: 3778
	public int materialIndex = -1;

	// Token: 0x04000EC3 RID: 3779
	private static MaterialPropertyBlock block;

	// Token: 0x04000EC4 RID: 3780
	public Light lightToFollow;

	// Token: 0x04000EC5 RID: 3781
	public float maxEmissionValue = 3f;

	// Token: 0x04000EC6 RID: 3782
	private int index = -1;

	// Token: 0x06001238 RID: 4664 RVA: 0x00077A20 File Offset: 0x00075C20
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
		if (EmissionScaledByLight.block == null)
		{
			EmissionScaledByLight.block = new MaterialPropertyBlock();
		}
		this.index = Shader.PropertyToID("_EmissionColor");
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x00077ABC File Offset: 0x00075CBC
	public void Update()
	{
		if (EmissionScaledByLight.block == null)
		{
			EmissionScaledByLight.block = new MaterialPropertyBlock();
		}
		float b = (!this.lightToFollow.enabled || !this.lightToFollow.gameObject.activeSelf) ? 0f : Mathf.Clamp01(this.lightToFollow.intensity / this.maxEmissionValue);
		EmissionScaledByLight.block.Clear();
		EmissionScaledByLight.block.SetColor(this.index, this.emissionColor * b);
		Renderer[] array = this.targetRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetPropertyBlock(EmissionScaledByLight.block);
		}
	}
}
