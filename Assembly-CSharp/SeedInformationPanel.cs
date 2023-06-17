using System;

// Token: 0x02000672 RID: 1650
public class SeedInformationPanel : ItemInformationPanel
{
	// Token: 0x040020BE RID: 8382
	public ItemTextValue durationDisplay;

	// Token: 0x040020BF RID: 8383
	public ItemTextValue waterRequiredDisplay;

	// Token: 0x040020C0 RID: 8384
	public ItemTextValue yieldDisplay;

	// Token: 0x040020C1 RID: 8385
	public ItemTextValue maxHarvestsDisplay;

	// Token: 0x060024C6 RID: 9414 RVA: 0x000C25E4 File Offset: 0x000C07E4
	public override bool EligableForDisplay(ItemDefinition info)
	{
		ItemModDeployable component = info.GetComponent<ItemModDeployable>();
		return !(component == null) && !(component.entityPrefab.Get().GetComponent<PlantEntity>() == null);
	}

	// Token: 0x060024C7 RID: 9415 RVA: 0x000C2620 File Offset: 0x000C0820
	public override void SetupForItem(ItemDefinition info, Item item = null)
	{
		ItemModDeployable component = info.GetComponent<ItemModDeployable>();
		if (component == null)
		{
			return;
		}
		PlantEntity component2 = component.entityPrefab.Get().GetComponent<PlantEntity>();
		if (component2 == null)
		{
			return;
		}
		int num;
		if (item == null || item.instanceData == null || item.instanceData.dataInt == 0)
		{
			num = -1;
		}
		else
		{
			num = item.instanceData.dataInt;
		}
		float num2 = 1f + (float)num / 10000f * 3f;
		this.durationDisplay.SetValue(0f, 0, (num <= 0) ? "Unknown" : ((component2.plantProperty.waterConsumptionLifetime / num2).ToString("N1") + " Min."));
		this.waterRequiredDisplay.SetValue((float)component2.plantProperty.lifetimeWaterConsumption, 0, "");
		int pickupAmount = component2.plantProperty.pickupAmount;
		this.yieldDisplay.SetValue(0f, 0, string.Concat(new object[]
		{
			pickupAmount,
			"-",
			pickupAmount + component2.plantProperty.waterYieldBonus * pickupAmount,
			" ",
			component2.plantProperty.pickupItem.displayName.translated
		}));
		this.maxHarvestsDisplay.SetValue((float)component2.plantProperty.maxHarvests, 0, "");
	}
}
