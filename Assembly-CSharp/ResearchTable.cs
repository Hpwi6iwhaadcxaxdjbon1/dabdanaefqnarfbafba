using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x020000B5 RID: 181
public class ResearchTable : StorageContainer
{
	// Token: 0x0400065B RID: 1627
	[NonSerialized]
	public float researchFinishedTime;

	// Token: 0x0400065C RID: 1628
	public float researchCostFraction = 1f;

	// Token: 0x0400065D RID: 1629
	public float researchDuration = 10f;

	// Token: 0x0400065E RID: 1630
	public int requiredPaper = 10;

	// Token: 0x0400065F RID: 1631
	public GameObjectRef researchStartEffect;

	// Token: 0x04000660 RID: 1632
	public GameObjectRef researchFailEffect;

	// Token: 0x04000661 RID: 1633
	public GameObjectRef researchSuccessEffect;

	// Token: 0x04000662 RID: 1634
	public ItemDefinition researchResource;

	// Token: 0x060009DE RID: 2526 RVA: 0x00052DE8 File Offset: 0x00050FE8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ResearchTable.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x00009CB5 File Offset: 0x00007EB5
	public override void ResetState()
	{
		base.ResetState();
		this.researchFinishedTime = 0f;
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x00009CC8 File Offset: 0x00007EC8
	public void TryResearch()
	{
		base.ServerRPC("DoResearch");
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x000056A3 File Offset: 0x000038A3
	public bool IsResearching()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x00009CD5 File Offset: 0x00007ED5
	public int RarityMultiplier(Rarity rarity)
	{
		if (rarity == 1)
		{
			return 20;
		}
		if (rarity == 2)
		{
			return 15;
		}
		if (rarity == 3)
		{
			return 10;
		}
		return 5;
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x00052E2C File Offset: 0x0005102C
	public int GetBlueprintStacksize(Item sourceItem)
	{
		int result = this.RarityMultiplier(sourceItem.info.rarity);
		if (sourceItem.info.category == ItemCategory.Ammunition)
		{
			result = Mathf.FloorToInt((float)sourceItem.info.stackable / (float)sourceItem.info.Blueprint.amountToCreate) * 2;
		}
		return result;
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x00052E80 File Offset: 0x00051080
	public int ScrapForResearch(Item item)
	{
		int result = 0;
		if (item.info.rarity == 1)
		{
			result = 20;
		}
		if (item.info.rarity == 2)
		{
			result = 75;
		}
		if (item.info.rarity == 3)
		{
			result = 250;
		}
		if (item.info.rarity == 4 || item.info.rarity == null)
		{
			result = 750;
		}
		return result;
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x00052EE8 File Offset: 0x000510E8
	public bool IsItemResearchable(Item item)
	{
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(item.info);
		return !(itemBlueprint == null) && itemBlueprint.isResearchable && !itemBlueprint.defaultBlueprint;
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x00009CED File Offset: 0x00007EED
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.researchTable != null)
		{
			this.researchFinishedTime = Time.realtimeSinceStartup + info.msg.researchTable.researchTimeLeft;
		}
	}
}
