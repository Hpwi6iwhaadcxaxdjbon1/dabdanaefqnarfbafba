using System;

// Token: 0x0200044B RID: 1099
public class ItemModConditionInWater : ItemMod
{
	// Token: 0x04001707 RID: 5895
	public bool requiredState;

	// Token: 0x06001A48 RID: 6728 RVA: 0x00092ADC File Offset: 0x00090CDC
	public override bool Passes(Item item)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		return !(ownerPlayer == null) && ownerPlayer.IsHeadUnderwater() == this.requiredState;
	}
}
