using System;

// Token: 0x020000D7 RID: 215
public class InvisibleVendingMachine : NPCVendingMachine
{
	// Token: 0x040006E8 RID: 1768
	public GameObjectRef buyEffect;

	// Token: 0x040006E9 RID: 1769
	public NPCVendingOrderManifest vmoManifest;

	// Token: 0x06000A82 RID: 2690 RVA: 0x0000A59B File Offset: 0x0000879B
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}
}
