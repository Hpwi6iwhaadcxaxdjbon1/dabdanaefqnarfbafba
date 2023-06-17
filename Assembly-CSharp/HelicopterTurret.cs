using System;
using UnityEngine;

// Token: 0x020002EF RID: 751
public class HelicopterTurret : MonoBehaviour
{
	// Token: 0x040010B1 RID: 4273
	public PatrolHelicopterAI _heliAI;

	// Token: 0x040010B2 RID: 4274
	public float fireRate = 0.125f;

	// Token: 0x040010B3 RID: 4275
	public float burstLength = 3f;

	// Token: 0x040010B4 RID: 4276
	public float timeBetweenBursts = 3f;

	// Token: 0x040010B5 RID: 4277
	public float maxTargetRange = 300f;

	// Token: 0x040010B6 RID: 4278
	public float loseTargetAfter = 5f;

	// Token: 0x040010B7 RID: 4279
	public Transform gun_yaw;

	// Token: 0x040010B8 RID: 4280
	public Transform gun_pitch;

	// Token: 0x040010B9 RID: 4281
	public Transform muzzleTransform;

	// Token: 0x040010BA RID: 4282
	public bool left;

	// Token: 0x040010BB RID: 4283
	public BaseCombatEntity _target;
}
