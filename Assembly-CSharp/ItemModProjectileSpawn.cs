using System;

// Token: 0x0200045E RID: 1118
public class ItemModProjectileSpawn : ItemModProjectile
{
	// Token: 0x04001749 RID: 5961
	public float createOnImpactChance;

	// Token: 0x0400174A RID: 5962
	public GameObjectRef createOnImpact = new GameObjectRef();

	// Token: 0x0400174B RID: 5963
	public float spreadAngle = 30f;

	// Token: 0x0400174C RID: 5964
	public float spreadVelocityMin = 1f;

	// Token: 0x0400174D RID: 5965
	public float spreadVelocityMax = 3f;

	// Token: 0x0400174E RID: 5966
	public int numToCreateChances = 1;
}
