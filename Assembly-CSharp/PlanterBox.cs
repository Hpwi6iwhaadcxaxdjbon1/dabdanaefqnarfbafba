using System;
using UnityEngine;

// Token: 0x020002F6 RID: 758
public class PlanterBox : BaseCombatEntity, ISplashable
{
	// Token: 0x040010EE RID: 4334
	public int soilSaturation;

	// Token: 0x040010EF RID: 4335
	public int soilSaturationMax = 8000;

	// Token: 0x040010F0 RID: 4336
	public MeshRenderer soilRenderer;

	// Token: 0x040010F1 RID: 4337
	private MaterialPropertyBlock block;

	// Token: 0x0600140F RID: 5135 RVA: 0x000111E7 File Offset: 0x0000F3E7
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource != null)
		{
			this.soilSaturation = info.msg.resource.stage;
		}
		if (base.isClient)
		{
			this.UpdateMaterialSettings();
		}
	}

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06001410 RID: 5136 RVA: 0x00011221 File Offset: 0x0000F421
	public float soilSaturationFraction
	{
		get
		{
			return (float)this.soilSaturation / (float)this.soilSaturationMax;
		}
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x00006425 File Offset: 0x00004625
	public override bool DisplayHealthInfo(BasePlayer player)
	{
		return this.ShowHealthInfo && base.healthFraction <= 0.75f && base.healthFraction > 0f;
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x00011232 File Offset: 0x0000F432
	public void SetupMaterialBlock()
	{
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x0007D4E4 File Offset: 0x0007B6E4
	public void UpdateMaterialSettings()
	{
		this.SetupMaterialBlock();
		Color value = Color.white * 0.25f + Color.white * 0.75f * (1f - this.soilSaturationFraction);
		value.a = 1f;
		this.block.Clear();
		this.block.SetColor("_Color", value);
		this.soilRenderer.SetPropertyBlock(this.block);
	}
}
