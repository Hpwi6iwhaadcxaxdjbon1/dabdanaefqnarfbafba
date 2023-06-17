using System;

// Token: 0x020002D2 RID: 722
public class FlameExplosive : TimedExplosive
{
	// Token: 0x0400100A RID: 4106
	public GameObjectRef createOnExplode;

	// Token: 0x0400100B RID: 4107
	public float numToCreate = 10f;

	// Token: 0x0400100C RID: 4108
	public float minVelocity = 2f;

	// Token: 0x0400100D RID: 4109
	public float maxVelocity = 5f;

	// Token: 0x0400100E RID: 4110
	public float spreadAngle = 90f;
}
