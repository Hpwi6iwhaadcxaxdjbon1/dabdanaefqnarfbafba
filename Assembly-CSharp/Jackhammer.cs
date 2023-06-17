using System;
using Network;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class Jackhammer : BaseMelee
{
	// Token: 0x0400060D RID: 1549
	private bool lastEngineStatus;

	// Token: 0x0400060E RID: 1550
	private float nextReleaseTime;

	// Token: 0x0600094F RID: 2383 RVA: 0x0004FF04 File Offset: 0x0004E104
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Jackhammer.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x00002D44 File Offset: 0x00000F44
	public bool HasAmmo()
	{
		return true;
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x00009788 File Offset: 0x00007988
	public override void OnDeploy()
	{
		base.OnDeploy();
		this.lastEngineStatus = false;
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0004FF48 File Offset: 0x0004E148
	public override void OnInput()
	{
		base.ProcessInputTime();
		if (!this.IsFullyDeployed())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		ownerPlayer.modelState.aiming = false;
		bool flag = ownerPlayer.CanAttack();
		bool flag2 = ownerPlayer.input.state.IsDown(BUTTON.FIRE_PRIMARY);
		bool flag3 = flag2 || Time.realtimeSinceStartup < this.nextReleaseTime;
		bool flag4 = flag && flag3 && this.HasAmmo();
		if (flag2)
		{
			this.nextReleaseTime = Time.realtimeSinceStartup + 0.2f;
		}
		ownerPlayer.modelState.aiming = flag4;
		if (!base.HasAttackCooldown() && flag4)
		{
			base.StartAttackCooldown(this.repeatDelay);
			this.DoAttack();
		}
		if (this.viewModel.instance)
		{
			this.viewModel.instance.SetBool("attacking", flag4);
		}
		if (this.lastEngineStatus != flag4)
		{
			base.ServerRPC<bool>("Server_SetEngineStatus", flag4);
		}
		this.lastEngineStatus = flag4;
		if (flag4)
		{
			ownerPlayer.BlockSprint(0.25f);
		}
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void DoViewmodelImpact()
	{
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x00009797 File Offset: 0x00007997
	protected override void DoAttack()
	{
		base.DoAttack();
	}
}
