using System;
using UnityEngine;

// Token: 0x02000798 RID: 1944
[AddComponentMenu("Image Effects/FXAA")]
public class FXAA : FXAAPostEffectsBase, IImageEffect
{
	// Token: 0x040025C8 RID: 9672
	public Shader shader;

	// Token: 0x040025C9 RID: 9673
	private Material mat;

	// Token: 0x06002A38 RID: 10808 RVA: 0x00020C2D File Offset: 0x0001EE2D
	private void CreateMaterials()
	{
		if (this.mat == null)
		{
			this.mat = base.CheckShaderAndCreateMaterial(this.shader, this.mat);
		}
	}

	// Token: 0x06002A39 RID: 10809 RVA: 0x00020C55 File Offset: 0x0001EE55
	private void Start()
	{
		this.CreateMaterials();
		base.CheckSupport(false);
	}

	// Token: 0x06002A3A RID: 10810 RVA: 0x00020C65 File Offset: 0x0001EE65
	public bool IsActive()
	{
		return base.enabled;
	}

	// Token: 0x06002A3B RID: 10811 RVA: 0x000D7B88 File Offset: 0x000D5D88
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		float num = 1f / (float)Screen.width;
		float num2 = 1f / (float)Screen.height;
		this.mat.SetVector("_rcpFrame", new Vector4(num, num2, 0f, 0f));
		this.mat.SetVector("_rcpFrameOpt", new Vector4(num * 2f, num2 * 2f, num * 0.5f, num2 * 0.5f));
		Graphics.Blit(source, destination, this.mat);
	}
}
