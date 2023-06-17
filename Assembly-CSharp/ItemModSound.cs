using System;

// Token: 0x02000462 RID: 1122
public class ItemModSound : ItemMod
{
	// Token: 0x0400175B RID: 5979
	public GameObjectRef effect = new GameObjectRef();

	// Token: 0x0400175C RID: 5980
	public ItemModSound.Type actionType;

	// Token: 0x02000463 RID: 1123
	public enum Type
	{
		// Token: 0x0400175E RID: 5982
		OnAttachToWeapon
	}
}
