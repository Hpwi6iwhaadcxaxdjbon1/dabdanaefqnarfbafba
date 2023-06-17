using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000046 RID: 70
public class RFReceiver : IOEntity, IRFObject
{
	// Token: 0x040002AC RID: 684
	private Option __menuOption_Menu_SetFreqency;

	// Token: 0x040002AD RID: 685
	public int frequency;

	// Token: 0x040002AE RID: 686
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x0600055D RID: 1373 RVA: 0x0003F154 File Offset: 0x0003D354
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("RFReceiver.GetMenuOptions", 0.1f))
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

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x0600055E RID: 1374 RVA: 0x00006C35 File Offset: 0x00004E35
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_SetFrequency_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x0003F260 File Offset: 0x0003D460
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RFReceiver.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x00006C4C File Offset: 0x00004E4C
	public int GetFrequency()
	{
		return this.frequency;
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x00006C54 File Offset: 0x00004E54
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.frequency = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0003F2A4 File Offset: 0x0003D4A4
	[BaseEntity.Menu("setfrequency", "Set Frequency", Order = 400)]
	[BaseEntity.Menu.Description("setfreq_desc", "Configure which frequenc to listen to")]
	[BaseEntity.Menu.ShowIf("Menu_SetFrequency_ShowIf")]
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

	// Token: 0x06000563 RID: 1379 RVA: 0x00005C94 File Offset: 0x00003E94
	public bool Menu_SetFrequency_ShowIf(BasePlayer player)
	{
		return LocalPlayer.Entity.CanBuild();
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x00006C27 File Offset: 0x00004E27
	public void ClientSetFrequency(int newFreq)
	{
		base.ServerRPC<int>("ServerSetFrequency", newFreq);
	}
}
