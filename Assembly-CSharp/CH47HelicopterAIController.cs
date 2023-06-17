using System;
using UnityEngine;

// Token: 0x02000344 RID: 836
public class CH47HelicopterAIController : CH47Helicopter
{
	// Token: 0x040012E7 RID: 4839
	public GameObjectRef scientistPrefab;

	// Token: 0x040012E8 RID: 4840
	public GameObjectRef dismountablePrefab;

	// Token: 0x040012E9 RID: 4841
	public float maxTiltAngle = 0.3f;

	// Token: 0x040012EA RID: 4842
	public float AiAltitudeForce = 10000f;

	// Token: 0x040012EB RID: 4843
	public GameObjectRef lockedCratePrefab;

	// Token: 0x040012EC RID: 4844
	public const BaseEntity.Flags Flag_Damaged = BaseEntity.Flags.Reserved7;

	// Token: 0x040012ED RID: 4845
	public const BaseEntity.Flags Flag_NearDeath = BaseEntity.Flags.OnFire;

	// Token: 0x040012EE RID: 4846
	public const BaseEntity.Flags Flag_DropDoorOpen = BaseEntity.Flags.Reserved8;

	// Token: 0x040012EF RID: 4847
	public GameObject triggerHurt;

	// Token: 0x060015EA RID: 5610 RVA: 0x00012828 File Offset: 0x00010A28
	public override bool MountMenuVisible()
	{
		return base.MountMenuVisible() && LocalPlayer.Entity != null && LocalPlayer.Entity.IsAdmin;
	}

	// Token: 0x060015EB RID: 5611 RVA: 0x00005DD3 File Offset: 0x00003FD3
	public override void DestroyShared()
	{
		base.DestroyShared();
	}
}
