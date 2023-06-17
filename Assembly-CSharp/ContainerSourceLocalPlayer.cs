using System;

// Token: 0x02000618 RID: 1560
public class ContainerSourceLocalPlayer : ItemContainerSource
{
	// Token: 0x04001F26 RID: 7974
	public PlayerInventory.Type type;

	// Token: 0x060022F7 RID: 8951 RVA: 0x0001BB6C File Offset: 0x00019D6C
	public override ItemContainer GetItemContainer()
	{
		return LocalPlayer.GetContainer(this.type);
	}
}
