using System;

// Token: 0x020000DB RID: 219
public class NPCVendingMachine : VendingMachine
{
	// Token: 0x040006F3 RID: 1779
	public NPCVendingOrder vendingOrders;

	// Token: 0x06000A88 RID: 2696 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool ShouldShowLootMenus()
	{
		return false;
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool ShouldShowAdminPanel()
	{
		return false;
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool CanPlayerAdmin(BasePlayer player)
	{
		return false;
	}
}
