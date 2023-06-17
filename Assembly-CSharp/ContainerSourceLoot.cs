using System;

// Token: 0x02000619 RID: 1561
public class ContainerSourceLoot : ItemContainerSource
{
	// Token: 0x04001F27 RID: 7975
	public int container;

	// Token: 0x060022F9 RID: 8953 RVA: 0x0001BB81 File Offset: 0x00019D81
	public override ItemContainer GetItemContainer()
	{
		return LocalPlayer.GetLootContainer(this.container);
	}
}
