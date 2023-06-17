using System;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class HideIfAiming : MonoBehaviour, IEffect
{
	// Token: 0x0400083E RID: 2110
	public ParticleSystem[] systems;

	// Token: 0x06000BC5 RID: 3013 RVA: 0x0005A670 File Offset: 0x00058870
	public virtual void SetupEffect(Effect effect)
	{
		if (BaseViewModel.ActiveModel == null)
		{
			return;
		}
		BaseEntity baseEntity = base.transform.parent.gameObject.ToBaseEntity();
		if (baseEntity == null)
		{
			return;
		}
		BaseEntity parentEntity = baseEntity.GetParentEntity();
		if (parentEntity == null)
		{
			return;
		}
		BasePlayer basePlayer = parentEntity.ToPlayer();
		if (basePlayer == null)
		{
			return;
		}
		if (!basePlayer.IsLocalPlayer())
		{
			return;
		}
		if (!basePlayer.InFirstPersonMode())
		{
			return;
		}
		bool flag = BaseViewModel.ActiveModel.ironSights && BaseViewModel.ActiveModel.ironSights.Enabled;
		ParticleSystem[] array = this.systems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = !flag;
		}
	}
}
