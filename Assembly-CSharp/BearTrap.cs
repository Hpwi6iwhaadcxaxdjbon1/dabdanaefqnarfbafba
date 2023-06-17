using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class BearTrap : BaseTrap
{
	// Token: 0x04000195 RID: 405
	private Option __menuOption_Arm_Beartrap;

	// Token: 0x04000196 RID: 406
	protected Animator animator;

	// Token: 0x04000197 RID: 407
	private bool initialized;

	// Token: 0x060003B8 RID: 952 RVA: 0x00038D20 File Offset: 0x00036F20
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("BearTrap.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Arm_Beartrap", 0.1f))
			{
				if (this.Menu_Arm_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Arm_Beartrap.show = true;
					this.__menuOption_Arm_Beartrap.showDisabled = false;
					this.__menuOption_Arm_Beartrap.longUseOnly = false;
					this.__menuOption_Arm_Beartrap.order = 0;
					this.__menuOption_Arm_Beartrap.icon = "rotate";
					this.__menuOption_Arm_Beartrap.desc = "arm_beartrap_desc";
					this.__menuOption_Arm_Beartrap.title = "arm_beartrap";
					if (this.__menuOption_Arm_Beartrap.function == null)
					{
						this.__menuOption_Arm_Beartrap.function = new Action<BasePlayer>(this.Arm_Beartrap);
					}
					list.Add(this.__menuOption_Arm_Beartrap);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060003B9 RID: 953 RVA: 0x0000568C File Offset: 0x0000388C
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Arm_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060003BA RID: 954 RVA: 0x00038E28 File Offset: 0x00037028
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BearTrap.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003BB RID: 955 RVA: 0x000056A3 File Offset: 0x000038A3
	public bool Armed()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x060003BC RID: 956 RVA: 0x000056AC File Offset: 0x000038AC
	public override void InitShared()
	{
		this.animator = base.GetComponent<Animator>();
		base.InitShared();
	}

	// Token: 0x060003BD RID: 957 RVA: 0x000056C0 File Offset: 0x000038C0
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !this.Armed() && player.CanBuild();
	}

	// Token: 0x060003BE RID: 958 RVA: 0x000056DB File Offset: 0x000038DB
	[BaseEntity.Menu("arm_beartrap", "Arm")]
	[BaseEntity.Menu.ShowIf("Menu_Arm_ShowIf")]
	[BaseEntity.Menu.Icon("rotate")]
	[BaseEntity.Menu.Description("arm_beartrap_desc", "Arm the trap")]
	public void Arm_Beartrap(BasePlayer player)
	{
		base.ServerRPC("RPC_Arm");
	}

	// Token: 0x060003BF RID: 959 RVA: 0x000056E8 File Offset: 0x000038E8
	public bool Menu_Arm_ShowIf(BasePlayer player)
	{
		return !this.Armed();
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x00038E6C File Offset: 0x0003706C
	public override void ClientOnEnable()
	{
		base.ClientOnEnable();
		if (!this.initialized)
		{
			this.animator.SetBool("armed", this.Armed());
			this.animator.Play(this.Armed() ? "armed" : "fired");
			this.initialized = true;
		}
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x000056F3 File Offset: 0x000038F3
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!base.isServer && this.animator.isInitialized)
		{
			this.animator.SetBool("armed", this.Armed());
		}
	}
}
