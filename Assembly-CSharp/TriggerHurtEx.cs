using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020005C0 RID: 1472
public class TriggerHurtEx : TriggerBase, IServerComponent
{
	// Token: 0x04001D99 RID: 7577
	public float repeatRate = 0.1f;

	// Token: 0x04001D9A RID: 7578
	[Header("On Enter")]
	public List<DamageTypeEntry> damageOnEnter;

	// Token: 0x04001D9B RID: 7579
	public GameObjectRef effectOnEnter;

	// Token: 0x04001D9C RID: 7580
	public TriggerHurtEx.HurtType hurtTypeOnEnter;

	// Token: 0x04001D9D RID: 7581
	[Header("On Timer (damage per second)")]
	public List<DamageTypeEntry> damageOnTimer;

	// Token: 0x04001D9E RID: 7582
	public GameObjectRef effectOnTimer;

	// Token: 0x04001D9F RID: 7583
	public TriggerHurtEx.HurtType hurtTypeOnTimer;

	// Token: 0x04001DA0 RID: 7584
	[Header("On Move (damage per meter)")]
	public List<DamageTypeEntry> damageOnMove;

	// Token: 0x04001DA1 RID: 7585
	public GameObjectRef effectOnMove;

	// Token: 0x04001DA2 RID: 7586
	public TriggerHurtEx.HurtType hurtTypeOnMove;

	// Token: 0x04001DA3 RID: 7587
	[Header("On Leave")]
	public List<DamageTypeEntry> damageOnLeave;

	// Token: 0x04001DA4 RID: 7588
	public GameObjectRef effectOnLeave;

	// Token: 0x04001DA5 RID: 7589
	public TriggerHurtEx.HurtType hurtTypeOnLeave;

	// Token: 0x04001DA6 RID: 7590
	public bool damageEnabled = true;

	// Token: 0x020005C1 RID: 1473
	public enum HurtType
	{
		// Token: 0x04001DA8 RID: 7592
		Simple,
		// Token: 0x04001DA9 RID: 7593
		IncludeBleedingAndScreenShake
	}
}
