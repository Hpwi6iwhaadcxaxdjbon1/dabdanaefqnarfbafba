using System;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class Scientist : NPCPlayerApex
{
	// Token: 0x04000816 RID: 2070
	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x06000B87 RID: 2951 RVA: 0x0000B07A File Offset: 0x0000927A
	public override string Categorize()
	{
		return "scientist";
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x0000B081 File Offset: 0x00009281
	public override float StartHealth()
	{
		return Random.Range(this.startHealth, this.startHealth);
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x0000489D File Offset: 0x00002A9D
	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x0000489D File Offset: 0x00002A9D
	public override float MaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x0000B094 File Offset: 0x00009294
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this._displayName = string.Format("Scientist {0}", (int)((this.net != null) ? this.net.ID : ((uint)"scientist".GetHashCode())));
	}
}
