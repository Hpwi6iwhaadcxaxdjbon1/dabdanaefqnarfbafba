using System;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class MedicalTool : AttackEntity
{
	// Token: 0x0400061E RID: 1566
	public float healDurationSelf = 4f;

	// Token: 0x0400061F RID: 1567
	public float healDurationOther = 4f;

	// Token: 0x04000620 RID: 1568
	public float maxDistanceOther = 2f;

	// Token: 0x04000621 RID: 1569
	public bool canUseOnOther = true;

	// Token: 0x04000622 RID: 1570
	public bool canRevive = true;

	// Token: 0x04000623 RID: 1571
	private BasePlayer useTarget;

	// Token: 0x04000624 RID: 1572
	private float resetTime;

	// Token: 0x06000971 RID: 2417 RVA: 0x00050858 File Offset: 0x0004EA58
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MedicalTool.OnRpcMessage", 0.1f))
		{
			if (rpc == 3916310150U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: Reset ");
				}
				using (TimeWarning.New("Reset", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							this.Reset();
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in Reset", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x0005093C File Offset: 0x0004EB3C
	public BasePlayer GetTarget()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		HitTest hitTest = new HitTest();
		hitTest.AttackRay = ownerPlayer.eyes.BodyRay();
		HitTest hitTest2 = hitTest;
		hitTest2.AttackRay.origin = hitTest2.AttackRay.origin + hitTest.AttackRay.direction * -0.1f;
		hitTest.MaxDistance = this.maxDistanceOther + 0.1f;
		hitTest.ignoreEntity = ownerPlayer;
		hitTest.Radius = 0f;
		hitTest.Forgiveness = 0.4f;
		hitTest.type = HitTest.Type.MeleeAttack;
		if (!GameTrace.Trace(hitTest, 1269916417))
		{
			return null;
		}
		BasePlayer basePlayer = hitTest.HitEntity as BasePlayer;
		if (basePlayer && basePlayer.IsAlive())
		{
			return hitTest.HitEntity as BasePlayer;
		}
		return null;
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x00050300 File Offset: 0x0004E500
	public bool AnyPressed()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		return ownerPlayer && (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY) || ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_SECONDARY));
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x00050A0C File Offset: 0x0004EC0C
	public override void OnInput()
	{
		BasePlayer target = this.GetTarget();
		base.OnInput();
		if (this.viewModel == null)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (this.resetTime != 0f)
		{
			if (this.resetTime < UnityEngine.Time.realtimeSinceStartup)
			{
				this.Reset();
			}
			if ((this.useTarget != null && this.useTarget != target) || ownerPlayer.IsSwimming())
			{
				this.viewModel.CrossFade("idle", 0.1f);
				this.Reset();
				return;
			}
		}
		else if (!ownerPlayer.IsSwimming())
		{
			if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_PRIMARY))
			{
				this.viewModel.Play("use_self");
				ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Attack, "");
				this.resetTime = UnityEngine.Time.realtimeSinceStartup + this.healDurationSelf;
			}
			if (ownerPlayer.input.state.WasJustPressed(BUTTON.FIRE_SECONDARY) && this.canUseOnOther && target)
			{
				this.useTarget = target;
				this.viewModel.Play("use_other");
				ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Reload, "");
				this.resetTime = UnityEngine.Time.realtimeSinceStartup + this.healDurationOther;
			}
		}
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x00050B54 File Offset: 0x0004ED54
	public override void OnViewmodelEvent(string name)
	{
		base.OnViewmodelEvent(name);
		if (name == "used_self")
		{
			base.ServerRPC("UseSelf");
		}
		if (name == "used_other")
		{
			BasePlayer target = this.GetTarget();
			if (target && target == this.useTarget)
			{
				base.ServerRPC<uint>("UseOther", target.net.ID);
			}
			this.useTarget = null;
		}
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x0000982B File Offset: 0x00007A2B
	[BaseEntity.RPC_Client]
	public void Reset()
	{
		this.resetTime = 0f;
		this.useTarget = null;
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x0000983F File Offset: 0x00007A3F
	public override void OnDeploy()
	{
		base.OnDeploy();
		this.Reset();
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x0000984D File Offset: 0x00007A4D
	public override void OnHolstered()
	{
		base.OnHolstered();
		this.Reset();
	}
}
