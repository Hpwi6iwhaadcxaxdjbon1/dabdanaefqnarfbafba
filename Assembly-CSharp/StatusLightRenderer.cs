using System;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class StatusLightRenderer : MonoBehaviour, IClientComponent
{
	// Token: 0x04000DC4 RID: 3524
	public Material offMaterial;

	// Token: 0x04000DC5 RID: 3525
	public Material onMaterial;

	// Token: 0x04000DC6 RID: 3526
	private MaterialPropertyBlock propertyBlock;

	// Token: 0x04000DC7 RID: 3527
	private Renderer targetRenderer;

	// Token: 0x04000DC8 RID: 3528
	private Color lightColor;

	// Token: 0x04000DC9 RID: 3529
	private Light targetLight;

	// Token: 0x04000DCA RID: 3530
	private int colorID;

	// Token: 0x04000DCB RID: 3531
	private int emissionID;

	// Token: 0x060010D6 RID: 4310 RVA: 0x00071B4C File Offset: 0x0006FD4C
	protected void Awake()
	{
		this.propertyBlock = new MaterialPropertyBlock();
		this.targetRenderer = base.GetComponent<Renderer>();
		this.targetLight = base.GetComponent<Light>();
		this.colorID = Shader.PropertyToID("_Color");
		this.emissionID = Shader.PropertyToID("_EmissionColor");
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x00071B9C File Offset: 0x0006FD9C
	public void SetOff()
	{
		if (this.targetRenderer)
		{
			this.targetRenderer.sharedMaterial = this.offMaterial;
			this.targetRenderer.SetPropertyBlock(null);
		}
		if (this.targetLight)
		{
			this.targetLight.color = Color.clear;
		}
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x00071BF0 File Offset: 0x0006FDF0
	public void SetOn()
	{
		if (this.targetRenderer)
		{
			this.targetRenderer.sharedMaterial = this.onMaterial;
			this.targetRenderer.SetPropertyBlock(this.propertyBlock);
		}
		if (this.targetLight)
		{
			this.targetLight.color = this.lightColor;
		}
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x00071C4C File Offset: 0x0006FE4C
	public void SetRed()
	{
		this.propertyBlock.Clear();
		this.propertyBlock.SetColor(this.colorID, this.GetColor(197, 46, 0, byte.MaxValue));
		this.propertyBlock.SetColor(this.emissionID, this.GetColor(191, 0, 2, byte.MaxValue, 2.916925f));
		this.lightColor = this.GetColor(byte.MaxValue, 111, 102, byte.MaxValue);
		this.SetOn();
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x00071CD0 File Offset: 0x0006FED0
	public void SetGreen()
	{
		this.propertyBlock.Clear();
		this.propertyBlock.SetColor(this.colorID, this.GetColor(19, 191, 13, byte.MaxValue));
		this.propertyBlock.SetColor(this.emissionID, this.GetColor(19, 191, 13, byte.MaxValue, 2.5f));
		this.lightColor = this.GetColor(156, byte.MaxValue, 102, byte.MaxValue);
		this.SetOn();
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x0000EC0F File Offset: 0x0000CE0F
	private Color GetColor(byte r, byte g, byte b, byte a)
	{
		return new Color32(r, g, b, a);
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x0000EC20 File Offset: 0x0000CE20
	private Color GetColor(byte r, byte g, byte b, byte a, float intensity)
	{
		return new Color32(r, g, b, a) * intensity;
	}
}
