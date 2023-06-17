using System;
using System.Collections.Generic;
using Rust;

// Token: 0x02000247 RID: 583
public class DirectionalDamageTrigger : TriggerBase
{
	// Token: 0x04000E42 RID: 3650
	public float repeatRate = 1f;

	// Token: 0x04000E43 RID: 3651
	public List<DamageTypeEntry> damageType;

	// Token: 0x04000E44 RID: 3652
	public GameObjectRef attackEffect;
}
