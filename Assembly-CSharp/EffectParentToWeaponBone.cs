using System;
using UnityEngine;

// Token: 0x02000278 RID: 632
public class EffectParentToWeaponBone : MonoBehaviour, IEffect
{
	// Token: 0x04000EBF RID: 3775
	public string boneName;

	// Token: 0x06001234 RID: 4660 RVA: 0x00077900 File Offset: 0x00075B00
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
		this.PositionOnObject(base.transform.parent.gameObject);
	}

	// Token: 0x06001235 RID: 4661 RVA: 0x00077950 File Offset: 0x00075B50
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
		this.PositionOnObject(BaseViewModel.ActiveModel.gameObject);
		return true;
	}

	// Token: 0x06001236 RID: 4662 RVA: 0x000779D8 File Offset: 0x00075BD8
	private void PositionOnObject(GameObject wm)
	{
		Transform parent = wm.transform;
		Model component = wm.GetComponent<Model>();
		if (component != null)
		{
			parent = component.FindBone(this.boneName);
		}
		base.transform.parent = parent;
		base.gameObject.Identity();
	}
}
