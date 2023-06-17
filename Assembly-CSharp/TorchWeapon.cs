using System;
using GameTips;
using Network;
using Rust;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class TorchWeapon : BaseMelee
{
	// Token: 0x0400067D RID: 1661
	[NonSerialized]
	public float fuelTickAmount = 0.16666667f;

	// Token: 0x0400067E RID: 1662
	[Header("TorchWeapon")]
	public AnimatorOverrideController LitHoldAnimationOverride;

	// Token: 0x06000A0C RID: 2572 RVA: 0x0005378C File Offset: 0x0005198C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TorchWeapon.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x00009EEA File Offset: 0x000080EA
	public override void GetAttackStats(HitInfo info)
	{
		base.GetAttackStats(info);
		if (base.HasFlag(BaseEntity.Flags.On))
		{
			info.damageTypes.Add(DamageType.Heat, 1f);
		}
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x000537D0 File Offset: 0x000519D0
	public override void OnInput()
	{
		base.OnInput();
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_SECONDARY) && !ownerPlayer.IsHeadUnderwater())
		{
			this.ToggleOn();
		}
		else if (base.IsOn() && ownerPlayer.IsHeadUnderwater())
		{
			this.viewModel.SetBool("wants_on", false);
		}
		if (this.viewModel)
		{
			this.viewModel.SetBool("is_on", base.IsOn());
		}
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00009F0D File Offset: 0x0000810D
	private void ToggleOn()
	{
		if (this.viewModel)
		{
			this.viewModel.SetBool("wants_on", !base.IsOn());
		}
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x00009F35 File Offset: 0x00008135
	public override void OnViewmodelEvent(string name)
	{
		base.OnViewmodelEvent(name);
		if (name == "ignite")
		{
			base.ServerRPC("Ignite");
			TipEquipTorch.TorchLit();
		}
		if (name == "extinguish")
		{
			base.ServerRPC("Extinguish");
		}
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x00009F73 File Offset: 0x00008173
	public override AnimatorOverrideController GetHoldAnimations()
	{
		if (base.IsOn())
		{
			return this.LitHoldAnimationOverride;
		}
		return base.GetHoldAnimations();
	}
}
