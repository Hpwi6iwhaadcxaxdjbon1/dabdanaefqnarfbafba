using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000045 RID: 69
public class RFBroadcaster : IOEntity, IRFObject
{
	// Token: 0x040002A8 RID: 680
	private Option __menuOption_Menu_SetFreqency;

	// Token: 0x040002A9 RID: 681
	public int frequency;

	// Token: 0x040002AA RID: 682
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x040002AB RID: 683
	public const BaseEntity.Flags Flag_Broadcasting = BaseEntity.Flags.Reserved3;

	// Token: 0x06000554 RID: 1364 RVA: 0x0003EFB4 File Offset: 0x0003D1B4
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("RFBroadcaster.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_SetFreqency", 0.1f))
			{
				if (this.Menu_SetFrequency_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_SetFreqency.show = true;
					this.__menuOption_Menu_SetFreqency.showDisabled = false;
					this.__menuOption_Menu_SetFreqency.longUseOnly = false;
					this.__menuOption_Menu_SetFreqency.order = 400;
					this.__menuOption_Menu_SetFreqency.icon = "broadcast";
					this.__menuOption_Menu_SetFreqency.desc = "setfreq_desc";
					this.__menuOption_Menu_SetFreqency.title = "setfrequency";
					if (this.__menuOption_Menu_SetFreqency.function == null)
					{
						this.__menuOption_Menu_SetFreqency.function = new Action<BasePlayer>(this.Menu_SetFreqency);
					}
					list.Add(this.__menuOption_Menu_SetFreqency);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x06000555 RID: 1365 RVA: 0x00006BDC File Offset: 0x00004DDC
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_SetFrequency_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0003F0C0 File Offset: 0x0003D2C0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RFBroadcaster.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x00006BF3 File Offset: 0x00004DF3
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x00006BFB File Offset: 0x00004DFB
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0003F104 File Offset: 0x0003D304
	[BaseEntity.Menu.ShowIf("Menu_SetFrequency_ShowIf")]
	[BaseEntity.Menu.Description("setfreq_desc", "Configure which frequenc to listen to")]
	[BaseEntity.Menu("setfrequency", "Set Frequency", Order = 400)]
	[BaseEntity.Menu.Icon("broadcast")]
	public void Menu_SetFreqency(BasePlayer player)
	{
		if (LocalPlayer.Entity == null || !LocalPlayer.Entity.CanBuild())
		{
			return;
		}
		FrequencyConfig component = GameManager.client.CreatePrefab(this.frequencyPanelPrefab.resourcePath, true).GetComponent<FrequencyConfig>();
		component.SetRFObj(this);
		component.OpenDialog();
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x00005C94 File Offset: 0x00003E94
	public bool Menu_SetFrequency_ShowIf(BasePlayer player)
	{
		return LocalPlayer.Entity.CanBuild();
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x00006C27 File Offset: 0x00004E27
	public void ClientSetFrequency(int newFreq)
	{
		base.ServerRPC<int>("ServerSetFrequency", newFreq);
	}
}
