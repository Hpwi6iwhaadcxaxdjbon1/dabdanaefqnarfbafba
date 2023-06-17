using System;

// Token: 0x0200000C RID: 12
public class TriggerVehiclePush : TriggerBase, IServerComponent
{
	// Token: 0x0400005A RID: 90
	public BaseEntity thisEntity;

	// Token: 0x0400005B RID: 91
	public float maxPushVelocity = 10f;

	// Token: 0x0400005C RID: 92
	public float minRadius;

	// Token: 0x0400005D RID: 93
	public float maxRadius;
}
