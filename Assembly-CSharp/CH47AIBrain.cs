using System;

// Token: 0x02000341 RID: 833
public class CH47AIBrain : BaseAIBrain<CH47HelicopterAIController>
{
	// Token: 0x040012DD RID: 4829
	public const int CH47State_Idle = 1;

	// Token: 0x040012DE RID: 4830
	public const int CH47State_Patrol = 2;

	// Token: 0x040012DF RID: 4831
	public const int CH47State_Land = 3;

	// Token: 0x040012E0 RID: 4832
	public const int CH47State_Dropoff = 4;

	// Token: 0x040012E1 RID: 4833
	public const int CH47State_Orbit = 5;

	// Token: 0x040012E2 RID: 4834
	public const int CH47State_Retreat = 6;

	// Token: 0x040012E3 RID: 4835
	public const int CH47State_Egress = 7;
}
