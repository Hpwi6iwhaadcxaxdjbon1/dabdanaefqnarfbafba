using System;
using Rust;

// Token: 0x02000407 RID: 1031
public class TriggerHurtNotChild : TriggerBase
{
	// Token: 0x040015B6 RID: 5558
	public float DamagePerSecond = 1f;

	// Token: 0x040015B7 RID: 5559
	public float DamageTickRate = 4f;

	// Token: 0x040015B8 RID: 5560
	public DamageType damageType;

	// Token: 0x040015B9 RID: 5561
	public BaseEntity SourceEntity;
}
