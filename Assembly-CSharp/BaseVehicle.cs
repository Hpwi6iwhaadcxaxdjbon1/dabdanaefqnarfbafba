using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000332 RID: 818
public class BaseVehicle : BaseMountable
{
	// Token: 0x04001298 RID: 4760
	public GameObjectRef serverGibs;

	// Token: 0x04001299 RID: 4761
	[Tooltip("Allow players to mount other mountables/ladders from this vehicle")]
	public bool mountChaining = true;

	// Token: 0x0400129A RID: 4762
	public bool shouldShowHudHealth;

	// Token: 0x0400129B RID: 4763
	[Header("Mount Points")]
	public BaseVehicle.MountPointInfo[] mountPoints;

	// Token: 0x0400129C RID: 4764
	public const BaseEntity.Flags Flag_Headlights = BaseEntity.Flags.Reserved5;

	// Token: 0x0400129D RID: 4765
	public bool seatClipCheck;

	// Token: 0x060015B2 RID: 5554 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool DirectlyMountable()
	{
		return true;
	}

	// Token: 0x060015B3 RID: 5555 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void UpdatePlayerModel(BasePlayer player)
	{
	}

	// Token: 0x060015B4 RID: 5556 RVA: 0x0001257E File Offset: 0x0001077E
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.InitializeClientEffects();
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void InitializeClientEffects()
	{
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool SupportsChildDeployables()
	{
		return false;
	}

	// Token: 0x02000333 RID: 819
	[Serializable]
	public class MountPointInfo
	{
		// Token: 0x0400129E RID: 4766
		public Vector3 pos;

		// Token: 0x0400129F RID: 4767
		public Vector3 rot;

		// Token: 0x040012A0 RID: 4768
		public GameObjectRef prefab;

		// Token: 0x040012A1 RID: 4769
		public BaseMountable mountable;
	}
}
