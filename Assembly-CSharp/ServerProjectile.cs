using System;
using UnityEngine;

// Token: 0x02000323 RID: 803
public class ServerProjectile : EntityComponent<BaseEntity>, IServerComponent
{
	// Token: 0x04001250 RID: 4688
	public Vector3 initialVelocity;

	// Token: 0x04001251 RID: 4689
	public float drag;

	// Token: 0x04001252 RID: 4690
	public float gravityModifier = 1f;

	// Token: 0x04001253 RID: 4691
	public float speed = 15f;

	// Token: 0x04001254 RID: 4692
	public float scanRange;

	// Token: 0x04001255 RID: 4693
	public Vector3 swimScale;

	// Token: 0x04001256 RID: 4694
	public Vector3 swimSpeed;

	// Token: 0x04001257 RID: 4695
	public float radius;
}
