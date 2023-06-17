using System;

// Token: 0x02000653 RID: 1619
public class GunModInformationPanel : ItemInformationPanel
{
	// Token: 0x0400202C RID: 8236
	public ItemTextValue fireRateDisplay;

	// Token: 0x0400202D RID: 8237
	public ItemTextValue velocityDisplay;

	// Token: 0x0400202E RID: 8238
	public ItemTextValue damageDisplay;

	// Token: 0x0400202F RID: 8239
	public ItemTextValue accuracyDisplay;

	// Token: 0x04002030 RID: 8240
	public ItemTextValue recoilDisplay;

	// Token: 0x04002031 RID: 8241
	public ItemTextValue zoomDisplay;

	// Token: 0x06002419 RID: 9241 RVA: 0x000BF218 File Offset: 0x000BD418
	public override bool EligableForDisplay(ItemDefinition info)
	{
		ItemModEntity component = info.GetComponent<ItemModEntity>();
		return !(component == null) && !(component.entityPrefab.Get().GetComponent<ProjectileWeaponMod>() == null);
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x000BF254 File Offset: 0x000BD454
	public override void SetupForItem(ItemDefinition info, Item item = null)
	{
		ItemModEntity component = info.GetComponent<ItemModEntity>();
		if (component == null)
		{
			return;
		}
		ProjectileWeaponMod component2 = component.entityPrefab.Get().GetComponent<ProjectileWeaponMod>();
		if (component2 == null)
		{
			return;
		}
		this.fireRateDisplay.SetValue((1f - component2.repeatDelay.scalar) * 100f, 0, "");
		this.velocityDisplay.SetValue((component2.projectileVelocity.scalar - 1f) * 100f, 0, "");
		this.damageDisplay.SetValue((component2.projectileDamage.scalar - 1f) * 100f, 0, "");
		this.recoilDisplay.SetValue((component2.recoil.scalar - 1f) * 100f, 0, "");
		this.zoomDisplay.SetValue(component2.zoomAmountDisplayOnly, 1, "");
		float num = 0f;
		float num2 = 0f;
		if (component2.hipAimCone.enabled)
		{
			num -= component2.hipAimCone.offset * 15f;
			num2 += (1f - component2.hipAimCone.scalar) * 100f;
		}
		if (component2.sightAimCone.enabled)
		{
			num -= component2.sightAimCone.offset * 15f;
			num2 += (1f - component2.sightAimCone.scalar) * 100f;
		}
		num += num2 / 2f;
		if (component2.aimsway.enabled)
		{
			num += (1f - component2.aimsway.scalar) * 15f;
		}
		this.accuracyDisplay.SetValue(num, 0, "");
		this.accuracyDisplay.gameObject.SetActive(num != 0f);
		this.fireRateDisplay.gameObject.SetActive(component2.repeatDelay.enabled);
		this.velocityDisplay.gameObject.SetActive(component2.projectileVelocity.enabled);
		this.damageDisplay.gameObject.SetActive(component2.projectileDamage.enabled);
		this.recoilDisplay.gameObject.SetActive(component2.recoil.enabled);
		this.zoomDisplay.gameObject.SetActive(component2.zoomAmountDisplayOnly != 0f);
	}
}
