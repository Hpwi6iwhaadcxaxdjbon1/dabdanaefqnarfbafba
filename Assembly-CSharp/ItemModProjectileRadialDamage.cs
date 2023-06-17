using System;
using Rust;

// Token: 0x0200045D RID: 1117
public class ItemModProjectileRadialDamage : ItemModProjectileMod
{
	// Token: 0x04001745 RID: 5957
	public float radius = 0.5f;

	// Token: 0x04001746 RID: 5958
	public DamageTypeEntry damage;

	// Token: 0x04001747 RID: 5959
	public GameObjectRef effect;

	// Token: 0x04001748 RID: 5960
	public bool ignoreHitObject = true;
}
