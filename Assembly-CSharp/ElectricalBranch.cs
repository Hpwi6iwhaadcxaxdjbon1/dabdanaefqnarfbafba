using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x0200002F RID: 47
public class ElectricalBranch : IOEntity
{
	// Token: 0x040001CD RID: 461
	private Option __menuOption_Menu_SetBranch;

	// Token: 0x040001CE RID: 462
	public int branchAmount = 2;

	// Token: 0x040001CF RID: 463
	public GameObjectRef branchPanelPrefab;

	// Token: 0x06000435 RID: 1077 RVA: 0x0003AB08 File Offset: 0x00038D08
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("ElectricalBranch.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_SetBranch", 0.1f))
			{
				if (this.Menu_SetBranch_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_SetBranch.show = true;
					this.__menuOption_Menu_SetBranch.showDisabled = false;
					this.__menuOption_Menu_SetBranch.longUseOnly = false;
					this.__menuOption_Menu_SetBranch.order = 400;
					this.__menuOption_Menu_SetBranch.icon = "electric";
					this.__menuOption_Menu_SetBranch.desc = "setbranch_desc";
					this.__menuOption_Menu_SetBranch.title = "setbranch";
					if (this.__menuOption_Menu_SetBranch.function == null)
					{
						this.__menuOption_Menu_SetBranch.function = new Action<BasePlayer>(this.Menu_SetBranch);
					}
					list.Add(this.__menuOption_Menu_SetBranch);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x06000436 RID: 1078 RVA: 0x00005C51 File Offset: 0x00003E51
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_SetBranch_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x0003AC14 File Offset: 0x00038E14
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ElectricalBranch.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x00005C68 File Offset: 0x00003E68
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.branchAmount = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x0003AC58 File Offset: 0x00038E58
	[BaseEntity.Menu.ShowIf("Menu_SetBranch_ShowIf")]
	[BaseEntity.Menu.Description("setbranch_desc", "Configure how much power to branch off")]
	[BaseEntity.Menu.Icon("electric")]
	[BaseEntity.Menu("setbranch", "Configure", Order = 400)]
	public void Menu_SetBranch(BasePlayer player)
	{
		if (LocalPlayer.Entity == null || !LocalPlayer.Entity.CanBuild())
		{
			return;
		}
		BranchConfig component = GameManager.client.CreatePrefab(this.branchPanelPrefab.resourcePath, true).GetComponent<BranchConfig>();
		component.SetBranch(this);
		component.OpenDialog();
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x00005C94 File Offset: 0x00003E94
	public bool Menu_SetBranch_ShowIf(BasePlayer player)
	{
		return LocalPlayer.Entity.CanBuild();
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x00005CA0 File Offset: 0x00003EA0
	public void ClientChangePower(int newAmount)
	{
		base.ServerRPC<int>("SetBranchOffPower", newAmount);
	}
}
