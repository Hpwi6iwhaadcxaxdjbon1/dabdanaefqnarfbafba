using System;
using UnityEngine;

// Token: 0x02000425 RID: 1061
[ExecuteInEditMode]
public class MaterialOverlay : MonoBehaviour
{
	// Token: 0x0400161C RID: 5660
	public Material material;

	// Token: 0x0600199A RID: 6554 RVA: 0x000909F4 File Offset: 0x0008EBF4
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.material)
		{
			Graphics.Blit(source, destination);
			return;
		}
		for (int i = 0; i < this.material.passCount; i++)
		{
			Graphics.Blit(source, destination, this.material, i);
		}
	}
}
