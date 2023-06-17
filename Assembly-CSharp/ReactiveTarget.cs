using System;
using System.Collections.Generic;
using ConVar;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class ReactiveTarget : DecayEntity
{
	// Token: 0x04000297 RID: 663
	private Option __menuOption_Lower_Target;

	// Token: 0x04000298 RID: 664
	private Option __menuOption_Reset_target;

	// Token: 0x04000299 RID: 665
	public Animator myAnimator;

	// Token: 0x0400029A RID: 666
	public GameObjectRef bullseyeEffect;

	// Token: 0x0400029B RID: 667
	public GameObjectRef knockdownEffect;

	// Token: 0x0400029C RID: 668
	private float lastToggleTime = float.NegativeInfinity;

	// Token: 0x06000530 RID: 1328 RVA: 0x0003E78C File Offset: 0x0003C98C
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("ReactiveTarget.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Lower_Target", 0.1f))
			{
				if (this.Menu_Lower_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Lower_Target.show = true;
					this.__menuOption_Lower_Target.showDisabled = false;
					this.__menuOption_Lower_Target.longUseOnly = false;
					this.__menuOption_Lower_Target.order = 0;
					this.__menuOption_Lower_Target.icon = "drop";
					this.__menuOption_Lower_Target.desc = "lower_target_Desc";
					this.__menuOption_Lower_Target.title = "lower_target";
					if (this.__menuOption_Lower_Target.function == null)
					{
						this.__menuOption_Lower_Target.function = new Action<BasePlayer>(this.Lower_Target);
					}
					list.Add(this.__menuOption_Lower_Target);
				}
			}
			using (TimeWarning.New("Reset_target", 0.1f))
			{
				if (this.Menu_Reset_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Reset_target.show = true;
					this.__menuOption_Reset_target.showDisabled = false;
					this.__menuOption_Reset_target.longUseOnly = false;
					this.__menuOption_Reset_target.order = 0;
					this.__menuOption_Reset_target.icon = "rotate";
					this.__menuOption_Reset_target.desc = "reset_target_Desc";
					this.__menuOption_Reset_target.title = "reset_target";
					if (this.__menuOption_Reset_target.function == null)
					{
						this.__menuOption_Reset_target.function = new Action<BasePlayer>(this.Reset_target);
					}
					list.Add(this.__menuOption_Reset_target);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x06000531 RID: 1329 RVA: 0x00006973 File Offset: 0x00004B73
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Lower_ShowIf(LocalPlayer.Entity) || this.Menu_Reset_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0003E980 File Offset: 0x0003CB80
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ReactiveTarget.OnRpcMessage", 0.1f))
		{
			if (rpc == 2157068783U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: HitEffect ");
				}
				using (TimeWarning.New("HitEffect", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.HitEffect(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in HitEffect", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0003EA9C File Offset: 0x0003CC9C
	public void OnHitShared(HitInfo info)
	{
		if (this.IsKnockedDown())
		{
			return;
		}
		bool flag = info.HitBone == StringPool.Get("target_collider");
		bool flag2 = info.HitBone == StringPool.Get("target_collider_bullseye");
		if (!flag && !flag2)
		{
			return;
		}
		if (base.isClient)
		{
			this.myAnimator.SetTrigger("hit");
		}
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x00005D1E File Offset: 0x00003F1E
	public bool IsKnockedDown()
	{
		return !base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x00006999 File Offset: 0x00004B99
	public override void OnAttacked(HitInfo info)
	{
		this.OnHitShared(info);
		base.OnAttacked(info);
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x000069A9 File Offset: 0x00004BA9
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && this.CanToggle();
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x000069BC File Offset: 0x00004BBC
	public bool CanToggle()
	{
		return UnityEngine.Time.realtimeSinceStartup > this.lastToggleTime + 1f;
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x000069D1 File Offset: 0x00004BD1
	[BaseEntity.RPC_Client]
	public void HitEffect(BaseEntity.RPCMessage msg)
	{
		if (!LocalPlayer.Entity)
		{
			return;
		}
		if (msg.read.UInt32() == LocalPlayer.Entity.net.ID)
		{
			return;
		}
		this.myAnimator.SetTrigger("hit");
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x0003EAF8 File Offset: 0x0003CCF8
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		bool flag = base.HasFlag(BaseEntity.Flags.On);
		base.Load(info);
		if (base.isClient)
		{
			bool flag2 = base.HasFlag(BaseEntity.Flags.On);
			if (flag != flag2)
			{
				this.lastToggleTime = UnityEngine.Time.realtimeSinceStartup;
				this.UpdateAnimationParameters(flag2);
				if (flag2)
				{
					base.gameObject.BroadcastDecalRecycle();
				}
			}
		}
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x00006A0D File Offset: 0x00004C0D
	public override void ClientOnEnable()
	{
		base.ClientOnEnable();
		this.UpdateAnimationParameters(base.HasFlag(BaseEntity.Flags.On));
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x00006A22 File Offset: 0x00004C22
	private void UpdateAnimationParameters(bool isOn)
	{
		if (!this.myAnimator)
		{
			return;
		}
		if (!this.myAnimator.isInitialized)
		{
			return;
		}
		this.myAnimator.SetBool("knocked_down", !isOn);
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x00006A54 File Offset: 0x00004C54
	[BaseEntity.Menu("reset_target", "Reset")]
	[BaseEntity.Menu.Icon("rotate")]
	[BaseEntity.Menu.ShowIf("Menu_Reset_ShowIf")]
	[BaseEntity.Menu.Description("reset_target_Desc", "Reset the target")]
	public void Reset_target(BasePlayer player)
	{
		base.ServerRPC("RPC_Reset");
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x00006A61 File Offset: 0x00004C61
	public bool Menu_Reset_ShowIf(BasePlayer player)
	{
		return !base.HasFlag(BaseEntity.Flags.On) && this.CanToggle();
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x00006A74 File Offset: 0x00004C74
	[BaseEntity.Menu.Icon("drop")]
	[BaseEntity.Menu.Description("lower_target_Desc", "Lower the target")]
	[BaseEntity.Menu("lower_target", "Lower")]
	[BaseEntity.Menu.ShowIf("Menu_Lower_ShowIf")]
	public void Lower_Target(BasePlayer player)
	{
		base.ServerRPC("RPC_Lower");
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x00006A81 File Offset: 0x00004C81
	public bool Menu_Lower_ShowIf(BasePlayer player)
	{
		return base.HasFlag(BaseEntity.Flags.On) && this.CanToggle();
	}
}
