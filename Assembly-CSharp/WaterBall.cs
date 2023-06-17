using System;
using UnityEngine;

// Token: 0x02000300 RID: 768
public class WaterBall : BaseEntity
{
	// Token: 0x04001109 RID: 4361
	public ItemDefinition liquidType;

	// Token: 0x0400110A RID: 4362
	public int waterAmount;

	// Token: 0x0400110B RID: 4363
	public GameObjectRef waterExplosion;

	// Token: 0x0400110C RID: 4364
	public Rigidbody myRigidBody;
}
