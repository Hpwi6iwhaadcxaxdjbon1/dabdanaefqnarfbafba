using System;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class NPCMurderer : NPCPlayerApex
{
	// Token: 0x040007F7 RID: 2039
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x06000B79 RID: 2937 RVA: 0x0000AFBB File Offset: 0x000091BB
	public override string Categorize()
	{
		return "murderer";
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x0000AFC2 File Offset: 0x000091C2
	public override float StartHealth()
	{
		return Random.Range(100f, 100f);
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x00004EC7 File Offset: 0x000030C7
	public override float StartMaxHealth()
	{
		return 100f;
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x00004EC7 File Offset: 0x000030C7
	public override float MaxHealth()
	{
		return 100f;
	}
}
