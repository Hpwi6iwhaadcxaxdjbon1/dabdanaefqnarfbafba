using System;
using UnityEngine;

// Token: 0x020002A1 RID: 673
public class GrenadeWeapon : ThrownWeapon
{
	// Token: 0x04000F8D RID: 3981
	[NonSerialized]
	private bool drop;

	// Token: 0x060012EA RID: 4842 RVA: 0x0007A77C File Offset: 0x0007897C
	public override void OnInput()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY))
		{
			this.viewModel.SetBool("attack_hold", true);
			ownerPlayer.modelState.aiming = true;
			this.drop = false;
			return;
		}
		if (ownerPlayer.input.state.IsDown(BUTTON.FIRE_SECONDARY))
		{
			this.viewModel.SetBool("attack_hold", true);
			ownerPlayer.modelState.aiming = true;
			this.drop = true;
			return;
		}
		this.viewModel.SetBool("attack_hold", false);
		ownerPlayer.modelState.aiming = false;
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x0007A830 File Offset: 0x00078A30
	public override void OnViewmodelEvent(string name)
	{
		base.OnViewmodelEvent(name);
		if (!(name == "release"))
		{
			name == "armed";
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (this.drop)
		{
			base.ServerRPC<Vector3, Vector3>("DoDrop", ownerPlayer.eyes.position, ownerPlayer.eyes.BodyForward());
		}
		else
		{
			float arg = 1f;
			base.ServerRPC<Vector3, Vector3, float>("DoThrow", ownerPlayer.eyes.position, ownerPlayer.eyes.BodyForward(), arg);
		}
		ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Attack, "");
	}
}
