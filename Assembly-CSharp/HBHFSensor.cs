using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000034 RID: 52
public class HBHFSensor : BaseDetector
{
	// Token: 0x040001E6 RID: 486
	private Option __menuOption_Menu_ExcludeAuth;

	// Token: 0x040001E7 RID: 487
	private Option __menuOption_Menu_ExcludeOthers;

	// Token: 0x040001E8 RID: 488
	private Option __menuOption_Menu_IncludeAuth;

	// Token: 0x040001E9 RID: 489
	private Option __menuOption_Menu_IncludeOthers;

	// Token: 0x040001EA RID: 490
	public GameObjectRef detectUp;

	// Token: 0x040001EB RID: 491
	public GameObjectRef detectDown;

	// Token: 0x040001EC RID: 492
	public const BaseEntity.Flags Flag_IncludeOthers = BaseEntity.Flags.Reserved2;

	// Token: 0x040001ED RID: 493
	public const BaseEntity.Flags Flag_IncludeAuthed = BaseEntity.Flags.Reserved3;

	// Token: 0x06000468 RID: 1128 RVA: 0x0003B534 File Offset: 0x00039734
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("HBHFSensor.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_ExcludeAuth", 0.1f))
			{
				if (this.Menu_ExcludeAuth_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_ExcludeAuth.show = true;
					this.__menuOption_Menu_ExcludeAuth.showDisabled = false;
					this.__menuOption_Menu_ExcludeAuth.longUseOnly = false;
					this.__menuOption_Menu_ExcludeAuth.order = 0;
					this.__menuOption_Menu_ExcludeAuth.icon = "close";
					this.__menuOption_Menu_ExcludeAuth.desc = "exclude_auth_desc";
					this.__menuOption_Menu_ExcludeAuth.title = "exclude_auth";
					if (this.__menuOption_Menu_ExcludeAuth.function == null)
					{
						this.__menuOption_Menu_ExcludeAuth.function = new Action<BasePlayer>(this.Menu_ExcludeAuth);
					}
					list.Add(this.__menuOption_Menu_ExcludeAuth);
				}
			}
			using (TimeWarning.New("Menu_ExcludeOthers", 0.1f))
			{
				if (this.Menu_ExcludeOthers_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_ExcludeOthers.show = true;
					this.__menuOption_Menu_ExcludeOthers.showDisabled = false;
					this.__menuOption_Menu_ExcludeOthers.longUseOnly = false;
					this.__menuOption_Menu_ExcludeOthers.order = 0;
					this.__menuOption_Menu_ExcludeOthers.icon = "close";
					this.__menuOption_Menu_ExcludeOthers.desc = "exclude_others_desc";
					this.__menuOption_Menu_ExcludeOthers.title = "exclude_others";
					if (this.__menuOption_Menu_ExcludeOthers.function == null)
					{
						this.__menuOption_Menu_ExcludeOthers.function = new Action<BasePlayer>(this.Menu_ExcludeOthers);
					}
					list.Add(this.__menuOption_Menu_ExcludeOthers);
				}
			}
			using (TimeWarning.New("Menu_IncludeAuth", 0.1f))
			{
				if (this.Menu_IncludeAuth_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_IncludeAuth.show = true;
					this.__menuOption_Menu_IncludeAuth.showDisabled = false;
					this.__menuOption_Menu_IncludeAuth.longUseOnly = false;
					this.__menuOption_Menu_IncludeAuth.order = 0;
					this.__menuOption_Menu_IncludeAuth.icon = "add";
					this.__menuOption_Menu_IncludeAuth.desc = "include_auth_desc";
					this.__menuOption_Menu_IncludeAuth.title = "include_auth";
					if (this.__menuOption_Menu_IncludeAuth.function == null)
					{
						this.__menuOption_Menu_IncludeAuth.function = new Action<BasePlayer>(this.Menu_IncludeAuth);
					}
					list.Add(this.__menuOption_Menu_IncludeAuth);
				}
			}
			using (TimeWarning.New("Menu_IncludeOthers", 0.1f))
			{
				if (this.Menu_IncludeOthers_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_IncludeOthers.show = true;
					this.__menuOption_Menu_IncludeOthers.showDisabled = false;
					this.__menuOption_Menu_IncludeOthers.longUseOnly = false;
					this.__menuOption_Menu_IncludeOthers.order = 0;
					this.__menuOption_Menu_IncludeOthers.icon = "add";
					this.__menuOption_Menu_IncludeOthers.desc = "include_others_desc";
					this.__menuOption_Menu_IncludeOthers.title = "include_others";
					if (this.__menuOption_Menu_IncludeOthers.function == null)
					{
						this.__menuOption_Menu_IncludeOthers.function = new Action<BasePlayer>(this.Menu_IncludeOthers);
					}
					list.Add(this.__menuOption_Menu_IncludeOthers);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000469 RID: 1129 RVA: 0x0003B8D0 File Offset: 0x00039AD0
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_ExcludeAuth_ShowIf(LocalPlayer.Entity) || this.Menu_ExcludeOthers_ShowIf(LocalPlayer.Entity) || this.Menu_IncludeAuth_ShowIf(LocalPlayer.Entity) || this.Menu_IncludeOthers_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0003B920 File Offset: 0x00039B20
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HBHFSensor.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00005EBB File Offset: 0x000040BB
	public bool ShouldIncludeAuthorized()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved3);
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x00005DC6 File Offset: 0x00003FC6
	public bool ShouldIncludeOthers()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x00005EC8 File Offset: 0x000040C8
	[BaseEntity.Menu.Description("include_auth_desc", "Include building authorized players")]
	[BaseEntity.Menu("include_auth", "Include Authorized")]
	[BaseEntity.Menu.Icon("add")]
	[BaseEntity.Menu.ShowIf("Menu_IncludeAuth_ShowIf")]
	public void Menu_IncludeAuth(BasePlayer player)
	{
		base.ServerRPC<bool>("SetIncludeAuth", true);
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x00005ED6 File Offset: 0x000040D6
	public bool Menu_IncludeAuth_ShowIf(BasePlayer player)
	{
		return base.IsPowered() && player.CanBuild() && !this.ShouldIncludeAuthorized() && player.IsHoldingEntity<Hammer>();
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x00005EF8 File Offset: 0x000040F8
	[BaseEntity.Menu("exclude_auth", "Exclude Authorized")]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu.Description("exclude_auth_desc", "Exclude building authorized players")]
	[BaseEntity.Menu.ShowIf("Menu_ExcludeAuth_ShowIf")]
	public void Menu_ExcludeAuth(BasePlayer player)
	{
		base.ServerRPC<bool>("SetIncludeAuth", false);
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x00005F06 File Offset: 0x00004106
	public bool Menu_ExcludeAuth_ShowIf(BasePlayer player)
	{
		return base.IsPowered() && player.CanBuild() && this.ShouldIncludeAuthorized() && player.IsHoldingEntity<Hammer>();
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x00005F28 File Offset: 0x00004128
	[BaseEntity.Menu.ShowIf("Menu_IncludeOthers_ShowIf")]
	[BaseEntity.Menu.Icon("add")]
	[BaseEntity.Menu.Description("include_others_desc", "Include non building authed players")]
	[BaseEntity.Menu("include_others", "Include Others")]
	public void Menu_IncludeOthers(BasePlayer player)
	{
		base.ServerRPC<bool>("SetIncludeOthers", true);
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00005F36 File Offset: 0x00004136
	public bool Menu_IncludeOthers_ShowIf(BasePlayer player)
	{
		return base.IsPowered() && player.CanBuild() && !this.ShouldIncludeOthers() && player.IsHoldingEntity<Hammer>();
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x00005F58 File Offset: 0x00004158
	[BaseEntity.Menu.ShowIf("Menu_ExcludeOthers_ShowIf")]
	[BaseEntity.Menu("exclude_others", "Exclude Others")]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu.Description("exclude_others_desc", "Exclude non building authed players")]
	public void Menu_ExcludeOthers(BasePlayer player)
	{
		base.ServerRPC<bool>("SetIncludeOthers", false);
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00005F66 File Offset: 0x00004166
	public bool Menu_ExcludeOthers_ShowIf(BasePlayer player)
	{
		return base.IsPowered() && player.CanBuild() && this.ShouldIncludeOthers() && player.IsHoldingEntity<Hammer>();
	}
}
