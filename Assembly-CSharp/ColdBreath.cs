using System;
using Rust;
using UnityEngine;

// Token: 0x0200026D RID: 621
public class ColdBreath : BaseMonoBehaviour
{
	// Token: 0x04000EAF RID: 3759
	public GameObjectRef effect;

	// Token: 0x04000EB0 RID: 3760
	private BasePlayer player;

	// Token: 0x04000EB1 RID: 3761
	private Transform jawBone;

	// Token: 0x06001207 RID: 4615 RVA: 0x000768CC File Offset: 0x00074ACC
	protected void OnEnable()
	{
		this.player = (base.gameObject.ToBaseEntity() as BasePlayer);
		PlayerModel component = base.gameObject.GetComponent<PlayerModel>();
		if (component)
		{
			this.jawBone = component.FindBone("jaw");
		}
		base.InvokeRepeating(new Action(this.Breathe), 3f, 3f);
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x0000FA61 File Offset: 0x0000DC61
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.Breathe));
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x00076930 File Offset: 0x00074B30
	private void Breathe()
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		if (LocalPlayer.Entity.metabolism.temperature.value > 5f)
		{
			return;
		}
		if (this.player.IsSwimming())
		{
			return;
		}
		if (LocalPlayer.Entity == this.player && this.player.currentViewMode == BasePlayer.CameraMode.FirstPerson)
		{
			return;
		}
		if (this.player.IsDead())
		{
			return;
		}
		if (MainCamera.Distance(this.player) > 20f)
		{
			return;
		}
		EffectLibrary.CreateEffect(this.effect.resourcePath, this.jawBone.position, this.jawBone.rotation);
	}
}
