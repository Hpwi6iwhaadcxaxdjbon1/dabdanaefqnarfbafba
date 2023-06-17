using System;

// Token: 0x02000129 RID: 297
public class HumanBrain : BaseAIBrain<HumanNPC>
{
	// Token: 0x0400086B RID: 2155
	public const int HumanState_Idle = 1;

	// Token: 0x0400086C RID: 2156
	public const int HumanState_Flee = 2;

	// Token: 0x0400086D RID: 2157
	public const int HumanState_Cover = 3;

	// Token: 0x0400086E RID: 2158
	public const int HumanState_Patrol = 4;

	// Token: 0x0400086F RID: 2159
	public const int HumanState_Roam = 5;

	// Token: 0x04000870 RID: 2160
	public const int HumanState_Chase = 6;

	// Token: 0x04000871 RID: 2161
	public const int HumanState_Exfil = 7;

	// Token: 0x04000872 RID: 2162
	public const int HumanState_Mounted = 8;

	// Token: 0x04000873 RID: 2163
	public const int HumanState_Combat = 9;

	// Token: 0x04000874 RID: 2164
	public const int HumanState_Traverse = 10;
}
