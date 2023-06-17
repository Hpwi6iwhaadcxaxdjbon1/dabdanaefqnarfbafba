using System;
using UnityEngine;

// Token: 0x020002FB RID: 763
public class SupplySignal : TimedExplosive
{
	// Token: 0x040010FC RID: 4348
	public GameObjectRef smokeEffectPrefab;

	// Token: 0x040010FD RID: 4349
	public GameObjectRef EntityToCreate;

	// Token: 0x040010FE RID: 4350
	[NonSerialized]
	public GameObject smokeEffect;

	// Token: 0x06001423 RID: 5155 RVA: 0x0007D768 File Offset: 0x0007B968
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isServer)
		{
			return;
		}
		if (base.HasFlag(BaseEntity.Flags.On))
		{
			if (!this.smokeEffect)
			{
				this.smokeEffect = EffectLibrary.CreateEffect(this.smokeEffectPrefab.resourcePath, base.transform, base.transform.position, base.transform.rotation);
				this.smokeEffect.transform.localRotation = Quaternion.LookRotation(Vector3.up);
				return;
			}
		}
		else
		{
			base.gameObject.BroadcastOnParentDestroying();
			this.smokeEffect = null;
		}
	}
}
