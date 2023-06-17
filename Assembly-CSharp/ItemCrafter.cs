using System;
using System.Collections.Generic;

// Token: 0x0200042B RID: 1067
public class ItemCrafter : EntityComponent<BasePlayer>
{
	// Token: 0x04001654 RID: 5716
	public List<ItemContainer> containers = new List<ItemContainer>();

	// Token: 0x04001655 RID: 5717
	public Queue<ItemCraftTask> queue = new Queue<ItemCraftTask>();

	// Token: 0x04001656 RID: 5718
	public int taskUID;

	// Token: 0x060019AB RID: 6571 RVA: 0x000153F3 File Offset: 0x000135F3
	public void AddContainer(ItemContainer container)
	{
		this.containers.Add(container);
	}

	// Token: 0x060019AC RID: 6572 RVA: 0x00091010 File Offset: 0x0008F210
	public static float GetScaledDuration(ItemBlueprint bp, float workbenchLevel)
	{
		float num = workbenchLevel - (float)bp.workbenchLevelRequired;
		if (num == 1f)
		{
			return bp.time * 0.5f;
		}
		if (num >= 2f)
		{
			return bp.time * 0.25f;
		}
		return bp.time;
	}

	// Token: 0x060019AD RID: 6573 RVA: 0x00091058 File Offset: 0x0008F258
	private bool DoesHaveUsableItem(int item, int iAmount)
	{
		int num = 0;
		foreach (ItemContainer itemContainer in this.containers)
		{
			num += itemContainer.GetAmount(item, true);
		}
		return num >= iAmount;
	}

	// Token: 0x060019AE RID: 6574 RVA: 0x000910B8 File Offset: 0x0008F2B8
	public bool CanCraft(ItemBlueprint bp, int amount = 1)
	{
		float num = (float)amount / (float)bp.targetItem.craftingStackable;
		foreach (ItemCraftTask itemCraftTask in this.queue)
		{
			if (!itemCraftTask.cancelled)
			{
				num += (float)itemCraftTask.amount / (float)itemCraftTask.blueprint.targetItem.craftingStackable;
			}
		}
		if (num > 8f)
		{
			return false;
		}
		if (amount < 1 || amount > bp.targetItem.craftingStackable)
		{
			return false;
		}
		foreach (ItemAmount itemAmount in bp.ingredients)
		{
			if (!this.DoesHaveUsableItem(itemAmount.itemid, (int)itemAmount.amount * amount))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060019AF RID: 6575 RVA: 0x000911B4 File Offset: 0x0008F3B4
	public bool CanCraft(ItemDefinition def, int amount = 1)
	{
		ItemBlueprint component = def.GetComponent<ItemBlueprint>();
		return this.CanCraft(component, amount);
	}
}
