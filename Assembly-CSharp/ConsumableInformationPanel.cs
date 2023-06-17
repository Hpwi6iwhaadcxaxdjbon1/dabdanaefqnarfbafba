using System;
using UnityEngine;

// Token: 0x02000651 RID: 1617
public class ConsumableInformationPanel : ItemInformationPanel
{
	// Token: 0x04002024 RID: 8228
	public ItemTextValue[] values;

	// Token: 0x06002412 RID: 9234 RVA: 0x0001C800 File Offset: 0x0001AA00
	private void Start()
	{
		this.values = base.GetComponentsInChildren<ItemTextValue>();
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x000BED38 File Offset: 0x000BCF38
	public override bool EligableForDisplay(ItemDefinition info)
	{
		if (info.GetComponent<ItemModConsumable>() == null)
		{
			return false;
		}
		ItemModEntity component = info.GetComponent<ItemModEntity>();
		if (component != null)
		{
			GameObject gameObject = component.entityPrefab.Get();
			if (gameObject && gameObject.GetComponent<MedicalTool>() == null)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002414 RID: 9236 RVA: 0x000BED8C File Offset: 0x000BCF8C
	public override void SetupForItem(ItemDefinition info, Item item = null)
	{
		ItemModConsumable component = info.GetComponent<ItemModConsumable>();
		ItemTextValue[] array = this.values;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		foreach (ItemModConsumable.ConsumableEffect consumableEffect in component.effects)
		{
			string text = consumableEffect.type.ToString();
			foreach (ItemTextValue itemTextValue in this.values)
			{
				if (itemTextValue.name == text)
				{
					itemTextValue.gameObject.SetActive(true);
					float num = consumableEffect.amount * ((item == null) ? 1f : ((component.conditionFractionToLose > 0f) ? component.conditionFractionToLose : item.conditionNormalized));
					bool flag = Mathf.Floor(num) != num;
					itemTextValue.SetValue(num, flag ? 1 : 0, "");
				}
			}
		}
	}
}
