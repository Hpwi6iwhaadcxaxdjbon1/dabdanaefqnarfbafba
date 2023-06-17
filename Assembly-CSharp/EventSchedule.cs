using System;
using UnityEngine;

// Token: 0x0200035D RID: 861
public class EventSchedule : BaseMonoBehaviour
{
	// Token: 0x04001325 RID: 4901
	[Tooltip("The minimum amount of hours between events")]
	public float minimumHoursBetween = 12f;

	// Token: 0x04001326 RID: 4902
	[Tooltip("The maximum amount of hours between events")]
	public float maxmumHoursBetween = 24f;
}
