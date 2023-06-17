using System;
using UnityEngine;

// Token: 0x020002EB RID: 747
public class M2BradleyPhysics : MonoBehaviour
{
	// Token: 0x0400109C RID: 4252
	private m2bradleyAnimator m2Animator;

	// Token: 0x0400109D RID: 4253
	public WheelCollider[] Wheels;

	// Token: 0x0400109E RID: 4254
	public WheelCollider[] TurningWheels;

	// Token: 0x0400109F RID: 4255
	public Rigidbody mainRigidbody;

	// Token: 0x040010A0 RID: 4256
	public Transform[] waypoints;

	// Token: 0x040010A1 RID: 4257
	private Vector3 currentWaypoint;

	// Token: 0x040010A2 RID: 4258
	private Vector3 nextWaypoint;
}
