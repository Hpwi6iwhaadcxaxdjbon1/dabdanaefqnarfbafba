using System;
using System.Collections.Generic;
using Network;

// Token: 0x020000B4 RID: 180
public class RepairBench : StorageContainer
{
	// Token: 0x04000659 RID: 1625
	public float maxConditionLostOnRepair = 0.2f;

	// Token: 0x0400065A RID: 1626
	public GameObjectRef skinchangeEffect;

	// Token: 0x060009D7 RID: 2519 RVA: 0x00052C2C File Offset: 0x00050E2C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RepairBench.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x00009C63 File Offset: 0x00007E63
	public void TryRepair()
	{
		base.ServerRPC("RepairItem");
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00009C70 File Offset: 0x00007E70
	public void ChangeSkinTo(int id)
	{
		base.ServerRPC<int>("ChangeSkin", id);
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x00009C7E File Offset: 0x00007E7E
	public float GetRepairFraction(Item itemToRepair)
	{
		return 1f - itemToRepair.condition / itemToRepair.maxCondition;
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x00009C93 File Offset: 0x00007E93
	public float RepairCostFraction(Item itemToRepair)
	{
		return this.GetRepairFraction(itemToRepair) * 0.2f;
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x00052C70 File Offset: 0x00050E70
	public void GetRepairCostList(ItemBlueprint bp, List<ItemAmount> allIngredients)
	{
		foreach (ItemAmount itemAmount in bp.ingredients)
		{
			allIngredients.Add(new ItemAmount(itemAmount.itemDef, itemAmount.amount));
		}
		foreach (ItemAmount itemAmount2 in bp.ingredients)
		{
			if (itemAmount2.itemDef.category == ItemCategory.Component && itemAmount2.itemDef.Blueprint != null)
			{
				bool flag = false;
				ItemAmount itemAmount3 = itemAmount2.itemDef.Blueprint.ingredients[0];
				foreach (ItemAmount itemAmount4 in allIngredients)
				{
					if (itemAmount4.itemDef == itemAmount3.itemDef)
					{
						itemAmount4.amount += itemAmount3.amount * itemAmount2.amount;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					allIngredients.Add(new ItemAmount(itemAmount3.itemDef, itemAmount3.amount * itemAmount2.amount));
				}
			}
		}
	}
}
