using System;

// Token: 0x02000450 RID: 1104
public class ItemModConsumeContents : ItemMod
{
	// Token: 0x04001716 RID: 5910
	public GameObjectRef consumeEffect;

	// Token: 0x06001A50 RID: 6736 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void CL_DoAction(Item item, BasePlayer player)
	{
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x00092B0C File Offset: 0x00090D0C
	public override bool CanDoAction(Item item, BasePlayer player)
	{
		if (!player.metabolism.CanConsume())
		{
			return false;
		}
		if (item.contents == null)
		{
			return false;
		}
		foreach (Item item2 in item.contents.itemList)
		{
			ItemModConsume component = item2.info.GetComponent<ItemModConsume>();
			if (!(component == null) && component.CanDoAction(item2, player))
			{
				return true;
			}
		}
		return false;
	}
}
