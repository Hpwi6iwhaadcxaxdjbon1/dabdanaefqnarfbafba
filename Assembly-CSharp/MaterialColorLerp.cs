using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000306 RID: 774
public class MaterialColorLerp : MonoBehaviour, IClientComponent
{
	// Token: 0x0400112A RID: 4394
	public Color startColor;

	// Token: 0x0400112B RID: 4395
	public Color endColor;

	// Token: 0x0400112C RID: 4396
	public Color currentColor;

	// Token: 0x0400112D RID: 4397
	public float delta;

	// Token: 0x0400112E RID: 4398
	private static MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x0400112F RID: 4399
	private bool initialized;

	// Token: 0x04001130 RID: 4400
	private float lerpStartTime;

	// Token: 0x04001131 RID: 4401
	private List<Renderer> cachedRenderers = new List<Renderer>();

	// Token: 0x06001448 RID: 5192 RVA: 0x000114B3 File Offset: 0x0000F6B3
	public void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06001449 RID: 5193 RVA: 0x000114BB File Offset: 0x0000F6BB
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.currentColor = this.startColor;
		this.UpdateMaterials(this.startColor);
		this.initialized = true;
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x000114E5 File Offset: 0x0000F6E5
	public void SetColorScale(float scale)
	{
		this.Init();
		this.delta = Mathf.Clamp01(scale);
		this.lerpStartTime = Time.realtimeSinceStartup;
		base.enabled = true;
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x0007DD40 File Offset: 0x0007BF40
	public void Update()
	{
		float value = Time.realtimeSinceStartup - this.lerpStartTime;
		Color color = Color.Lerp(this.startColor, this.endColor, this.delta);
		float t = Mathf.Clamp01(value);
		this.currentColor = Color.Lerp(this.currentColor, color, t);
		this.UpdateMaterials(this.currentColor);
		if (this.currentColor == color)
		{
			base.enabled = false;
		}
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x0001150B File Offset: 0x0000F70B
	public void RefreshRenderers()
	{
		this.cachedRenderers.Clear();
		base.GetComponentsInChildren<Renderer>(this.cachedRenderers);
	}

	// Token: 0x0600144D RID: 5197 RVA: 0x0007DDAC File Offset: 0x0007BFAC
	private void UpdateMaterials(Color final)
	{
		if (MaterialColorLerp.materialPropertyBlock == null)
		{
			MaterialColorLerp.materialPropertyBlock = new MaterialPropertyBlock();
		}
		else
		{
			MaterialColorLerp.materialPropertyBlock.Clear();
		}
		MaterialColorLerp.materialPropertyBlock.SetColor("_Color", final);
		foreach (Renderer renderer in this.cachedRenderers)
		{
			if (renderer != null)
			{
				renderer.SetPropertyBlock(MaterialColorLerp.materialPropertyBlock);
			}
		}
	}
}
