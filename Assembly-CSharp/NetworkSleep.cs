using System;
using UnityEngine;

// Token: 0x02000716 RID: 1814
public class NetworkSleep : MonoBehaviour
{
	// Token: 0x0400239D RID: 9117
	public static int totalBehavioursDisabled;

	// Token: 0x0400239E RID: 9118
	public static int totalCollidersDisabled;

	// Token: 0x0400239F RID: 9119
	public Behaviour[] behaviours;

	// Token: 0x040023A0 RID: 9120
	public Collider[] colliders;

	// Token: 0x040023A1 RID: 9121
	internal int BehavioursDisabled;

	// Token: 0x040023A2 RID: 9122
	internal int CollidersDisabled;
}
