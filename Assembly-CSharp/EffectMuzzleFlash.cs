using System;
using UnityEngine;

// Token: 0x02000276 RID: 630
public class EffectMuzzleFlash : MonoBehaviour, IEffect
{
	// Token: 0x0600122E RID: 4654 RVA: 0x000777A4 File Offset: 0x000759A4
	public virtual void SetupEffect(Effect effect)
	{
		if (base.transform.parent == null)
		{
			GameManager.Destroy(base.gameObject, 0f);
			return;
		}
		if (this.ApplyToViewModel())
		{
			return;
		}
		BaseProjectile baseProjectile = base.gameObject.ToBaseEntity() as BaseProjectile;
		if (baseProjectile != null)
		{
			this.PositionOnObject(baseProjectile.MuzzlePoint);
		}
	}

	// Token: 0x0600122F RID: 4655 RVA: 0x00077804 File Offset: 0x00075A04
	private bool ApplyToViewModel()
	{
		if (BaseViewModel.ActiveModel == null)
		{
			return false;
		}
		BaseEntity baseEntity = base.transform.parent.gameObject.ToBaseEntity();
		if (baseEntity == null)
		{
			return false;
		}
		BaseEntity parentEntity = baseEntity.GetParentEntity();
		if (parentEntity == null)
		{
			return false;
		}
		BasePlayer basePlayer = parentEntity.ToPlayer();
		if (basePlayer == null)
		{
			return false;
		}
		if (!basePlayer.IsLocalPlayer())
		{
			return false;
		}
		if (!basePlayer.InFirstPersonMode())
		{
			return false;
		}
		this.PositionOnObject(BaseViewModel.ActiveModel.MuzzlePoint);
		return true;
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x0000FBE8 File Offset: 0x0000DDE8
	private void PositionOnObject(Transform muzzle)
	{
		base.transform.parent = muzzle;
		base.gameObject.Identity();
	}
}
