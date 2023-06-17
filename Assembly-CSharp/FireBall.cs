using System;
using UnityEngine;

// Token: 0x020002D0 RID: 720
public class FireBall : BaseEntity, ISplashable
{
	// Token: 0x04000FFC RID: 4092
	public float lifeTimeMin = 20f;

	// Token: 0x04000FFD RID: 4093
	public float lifeTimeMax = 40f;

	// Token: 0x04000FFE RID: 4094
	public ParticleSystem[] movementSystems;

	// Token: 0x04000FFF RID: 4095
	public ParticleSystem[] restingSystems;

	// Token: 0x04001000 RID: 4096
	[NonSerialized]
	public float generation;

	// Token: 0x04001001 RID: 4097
	public GameObjectRef spreadSubEntity;

	// Token: 0x04001002 RID: 4098
	public float tickRate = 0.5f;

	// Token: 0x04001003 RID: 4099
	public float damagePerSecond = 2f;

	// Token: 0x04001004 RID: 4100
	public float radius = 0.5f;

	// Token: 0x04001005 RID: 4101
	public int waterToExtinguish = 200;

	// Token: 0x04001006 RID: 4102
	public bool canMerge;

	// Token: 0x04001007 RID: 4103
	public LayerMask AttackLayers = 1219701521;

	// Token: 0x04001008 RID: 4104
	private bool wasResting = true;

	// Token: 0x060013AF RID: 5039 RVA: 0x00007B4D File Offset: 0x00005D4D
	public bool IsResting()
	{
		return base.HasFlag(BaseEntity.Flags.OnFire);
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x0007C218 File Offset: 0x0007A418
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!base.isServer)
		{
			if (this.IsResting() != this.wasResting && this.restingSystems.Length != 0)
			{
				foreach (ParticleSystem particleSystem in this.movementSystems)
				{
					if (!(particleSystem == null))
					{
						particleSystem.enableEmission = !this.IsResting();
					}
				}
				foreach (ParticleSystem particleSystem2 in this.restingSystems)
				{
					if (!(particleSystem2 == null))
					{
						particleSystem2.enableEmission = this.IsResting();
					}
				}
			}
			this.wasResting = this.IsResting();
		}
	}
}
