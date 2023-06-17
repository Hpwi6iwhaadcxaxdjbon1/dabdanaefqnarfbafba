using System;
using UnityEngine;

// Token: 0x020002ED RID: 749
public class AIHelicopterAnimation : MonoBehaviour
{
	// Token: 0x040010A5 RID: 4261
	public PatrolHelicopterAI _ai;

	// Token: 0x040010A6 RID: 4262
	public float swayAmount = 1f;

	// Token: 0x040010A7 RID: 4263
	public float lastStrafeScalar;

	// Token: 0x040010A8 RID: 4264
	public float lastForwardBackScalar;

	// Token: 0x040010A9 RID: 4265
	public float degreeMax = 90f;

	// Token: 0x040010AA RID: 4266
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x040010AB RID: 4267
	public float oldMoveSpeed;

	// Token: 0x040010AC RID: 4268
	public float smoothRateOfChange;

	// Token: 0x040010AD RID: 4269
	public float flareAmount;
}
