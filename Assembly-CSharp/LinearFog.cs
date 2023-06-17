using System;
using UnityEngine;

// Token: 0x02000424 RID: 1060
[ExecuteInEditMode]
public class LinearFog : MonoBehaviour
{
	// Token: 0x04001616 RID: 5654
	public Material fogMaterial;

	// Token: 0x04001617 RID: 5655
	public Color fogColor = Color.white;

	// Token: 0x04001618 RID: 5656
	public float fogStart;

	// Token: 0x04001619 RID: 5657
	public float fogRange = 1f;

	// Token: 0x0400161A RID: 5658
	public float fogDensity = 1f;

	// Token: 0x0400161B RID: 5659
	public bool fogSky;

	// Token: 0x06001998 RID: 6552 RVA: 0x00090920 File Offset: 0x0008EB20
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.fogMaterial)
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.fogMaterial.SetColor("_FogColor", this.fogColor);
		this.fogMaterial.SetFloat("_Start", this.fogStart);
		this.fogMaterial.SetFloat("_Range", this.fogRange);
		this.fogMaterial.SetFloat("_Density", this.fogDensity);
		if (this.fogSky)
		{
			this.fogMaterial.SetFloat("_CutOff", 2f);
		}
		else
		{
			this.fogMaterial.SetFloat("_CutOff", 1f);
		}
		for (int i = 0; i < this.fogMaterial.passCount; i++)
		{
			Graphics.Blit(source, destination, this.fogMaterial, i);
		}
	}
}
