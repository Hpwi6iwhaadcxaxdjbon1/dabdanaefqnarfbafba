using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class CustomTimerSwitch : TimerSwitch
{
	// Token: 0x040001B1 RID: 433
	private Option __menuOption_Menu_SetTime;

	// Token: 0x040001B2 RID: 434
	public GameObjectRef timerPanelPrefab;

	// Token: 0x06000404 RID: 1028 RVA: 0x00039F38 File Offset: 0x00038138
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("CustomTimerSwitch.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_SetTime", 0.1f))
			{
				if (this.Menu_SetTime_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_SetTime.show = true;
					this.__menuOption_Menu_SetTime.showDisabled = false;
					this.__menuOption_Menu_SetTime.longUseOnly = false;
					this.__menuOption_Menu_SetTime.order = 400;
					this.__menuOption_Menu_SetTime.icon = "stopwatch";
					this.__menuOption_Menu_SetTime.desc = "settime_desc";
					this.__menuOption_Menu_SetTime.title = "settime";
					if (this.__menuOption_Menu_SetTime.function == null)
					{
						this.__menuOption_Menu_SetTime.function = new Action<BasePlayer>(this.Menu_SetTime);
					}
					list.Add(this.__menuOption_Menu_SetTime);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x06000405 RID: 1029 RVA: 0x00005A2A File Offset: 0x00003C2A
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_SetTime_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x0003A044 File Offset: 0x00038244
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CustomTimerSwitch.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x0000599F File Offset: 0x00003B9F
	public bool CanPlayerAdmin(BasePlayer player)
	{
		return player != null && player.CanBuild() && !base.IsOn();
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00005A41 File Offset: 0x00003C41
	public void SendNewTime(int seconds)
	{
		if (!this.CanPlayerAdmin(LocalPlayer.Entity))
		{
			return;
		}
		Debug.Log("client sending new time");
		seconds = Mathf.Clamp(seconds, 1, 1000000000);
		base.ServerRPC<int>("SERVER_SetTime", seconds);
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x0003A088 File Offset: 0x00038288
	[BaseEntity.Menu.Description("settime_desc", "Set how long the timer should be active for")]
	[BaseEntity.Menu("settime", "Set Time", Order = 400)]
	[BaseEntity.Menu.ShowIf("Menu_SetTime_ShowIf")]
	[BaseEntity.Menu.Icon("stopwatch")]
	public void Menu_SetTime(BasePlayer player)
	{
		if (LocalPlayer.Entity == null || !LocalPlayer.Entity.CanBuild())
		{
			return;
		}
		TimerConfig component = GameManager.client.CreatePrefab(this.timerPanelPrefab.resourcePath, true).GetComponent<TimerConfig>();
		component.SetTimerSwitch(this);
		component.OpenDialog();
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00005A75 File Offset: 0x00003C75
	public bool Menu_SetTime_ShowIf(BasePlayer player)
	{
		return !base.IsOn() && base.IsPowered();
	}
}
