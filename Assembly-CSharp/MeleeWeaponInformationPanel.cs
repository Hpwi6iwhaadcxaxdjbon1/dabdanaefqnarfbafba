using System;
using Rust;
using UnityEngine;

// Token: 0x02000668 RID: 1640
public class MeleeWeaponInformationPanel : ItemInformationPanel
{
	// Token: 0x04002082 RID: 8322
	public ItemStatValue damageDisplay;

	// Token: 0x04002083 RID: 8323
	public ItemStatValue attackRateDisplay;

	// Token: 0x04002084 RID: 8324
	public ItemStatValue attackSizeDisplay;

	// Token: 0x04002085 RID: 8325
	public ItemStatValue attackRangeDisplay;

	// Token: 0x04002086 RID: 8326
	public ItemStatValue oreGatherDisplay;

	// Token: 0x04002087 RID: 8327
	public ItemStatValue treeGatherDisplay;

	// Token: 0x04002088 RID: 8328
	public ItemStatValue fleshGatherDisplay;

	// Token: 0x0600248B RID: 9355 RVA: 0x000C10A4 File Offset: 0x000BF2A4
	public override bool EligableForDisplay(ItemDefinition info)
	{
		ItemModEntity component = info.GetComponent<ItemModEntity>();
		return !(component == null) && !(component.entityPrefab.Get().GetComponent<BaseMelee>() == null);
	}

	// Token: 0x0600248C RID: 9356 RVA: 0x000C10E0 File Offset: 0x000BF2E0
	public override void SetupForItem(ItemDefinition info, Item item = null)
	{
		ItemModEntity component = info.GetComponent<ItemModEntity>();
		if (component == null)
		{
			return;
		}
		BaseMelee component2 = component.entityPrefab.Get().GetComponent<BaseMelee>();
		if (component2 == null)
		{
			return;
		}
		float num = 0f;
		foreach (DamageTypeEntry damageTypeEntry in component2.damageTypes)
		{
			num += damageTypeEntry.amount;
		}
		this.damageDisplay.SetValue(num, 0, "");
		this.attackRateDisplay.SetValue(60f / component2.repeatDelay, 0, "");
		this.attackSizeDisplay.SetValue(component2.attackRadius, 1, "");
		this.attackRangeDisplay.SetValue(component2.maxDistance, 1, "");
		this.oreGatherDisplay.gameObject.SetActive(component2.gathering.Ore.gatherDamage > 0f);
		this.treeGatherDisplay.gameObject.SetActive(component2.gathering.Tree.gatherDamage > 0f);
		this.fleshGatherDisplay.gameObject.SetActive(component2.gathering.Flesh.gatherDamage > 0f);
		this.oreGatherDisplay.SetValue(1f - component2.gathering.Ore.destroyFraction, 0, component2.gathering.Ore.gatherDamage.ToString("N0"));
		this.treeGatherDisplay.SetValue(1f - component2.gathering.Tree.destroyFraction, 0, component2.gathering.Tree.gatherDamage.ToString("N0"));
		this.fleshGatherDisplay.SetValue(1f - component2.gathering.Flesh.destroyFraction, 0, component2.gathering.Flesh.gatherDamage.ToString("N0"));
		this.oreGatherDisplay.gameObject.transform.parent.gameObject.SetActive(false);
		this.oreGatherDisplay.gameObject.transform.parent.gameObject.SetActive(true);
		Canvas.ForceUpdateCanvases();
	}
}
