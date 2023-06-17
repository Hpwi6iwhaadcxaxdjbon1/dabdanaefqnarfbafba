using System;
using UnityEngine;

// Token: 0x0200044E RID: 1102
[RequireComponent(typeof(ItemModConsumable))]
public class ItemModConsume : ItemMod
{
	// Token: 0x0400170F RID: 5903
	public GameObjectRef consumeEffect;

	// Token: 0x04001710 RID: 5904
	public string eatGesture = "eat_2hand";

	// Token: 0x04001711 RID: 5905
	[Tooltip("Items that are given on consumption of this item")]
	public ItemAmountRandom[] product;

	// Token: 0x04001712 RID: 5906
	public ItemModConsumable primaryConsumable;

	// Token: 0x06001A4C RID: 6732 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void CL_DoAction(Item item, BasePlayer player)
	{
	}

	// Token: 0x06001A4D RID: 6733 RVA: 0x00015B9D File Offset: 0x00013D9D
	public override bool CanDoAction(Item item, BasePlayer player)
	{
		return player.metabolism.CanConsume();
	}
}
