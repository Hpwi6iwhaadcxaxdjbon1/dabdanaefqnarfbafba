using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200012A RID: 298
public class HumanNPC : NPCPlayer
{
	// Token: 0x04000875 RID: 2165
	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x04000876 RID: 2166
	public HumanNPC.SpeedType desiredSpeed = HumanNPC.SpeedType.SlowWalk;

	// Token: 0x04000877 RID: 2167
	[Header("Detection")]
	public float sightRange = 30f;

	// Token: 0x04000878 RID: 2168
	public float sightRangeLarge = 200f;

	// Token: 0x04000879 RID: 2169
	public float visionCone = -0.8f;

	// Token: 0x0400087A RID: 2170
	[Header("Damage")]
	public float aimConeScale = 2f;

	// Token: 0x0400087B RID: 2171
	private List<BaseCombatEntity> _targets = new List<BaseCombatEntity>();

	// Token: 0x0400087C RID: 2172
	public BaseCombatEntity currentTarget;

	// Token: 0x0400087D RID: 2173
	private HumanBrain _brain;

	// Token: 0x0400087E RID: 2174
	public float lastDismountTime;

	// Token: 0x06000BEB RID: 3051 RVA: 0x0000489D File Offset: 0x00002A9D
	public override float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x0000489D File Offset: 0x00002A9D
	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x0000489D File Offset: 0x00002A9D
	public override float MaxHealth()
	{
		return this.startHealth;
	}

	// Token: 0x0200012B RID: 299
	public enum SpeedType
	{
		// Token: 0x04000880 RID: 2176
		Crouch = 1,
		// Token: 0x04000881 RID: 2177
		SlowWalk,
		// Token: 0x04000882 RID: 2178
		Walk,
		// Token: 0x04000883 RID: 2179
		Sprint
	}
}
