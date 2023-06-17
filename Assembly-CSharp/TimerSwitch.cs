using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class TimerSwitch : IOEntity
{
	// Token: 0x04000306 RID: 774
	private Option __menuOption_Menu_TurnOn;

	// Token: 0x04000307 RID: 775
	public float timerLength = 10f;

	// Token: 0x04000308 RID: 776
	public Transform timerDrum;

	// Token: 0x04000309 RID: 777
	private float timePassed = -1f;

	// Token: 0x0400030A RID: 778
	private float oldTimeFraction;

	// Token: 0x060005EB RID: 1515 RVA: 0x000418C4 File Offset: 0x0003FAC4
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("TimerSwitch.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_TurnOn", 0.1f))
			{
				if (this.Menu_Activate_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_TurnOn.show = true;
					this.__menuOption_Menu_TurnOn.showDisabled = false;
					this.__menuOption_Menu_TurnOn.longUseOnly = false;
					this.__menuOption_Menu_TurnOn.order = 0;
					this.__menuOption_Menu_TurnOn.icon = "rotate";
					this.__menuOption_Menu_TurnOn.desc = "activate_switch_desc";
					this.__menuOption_Menu_TurnOn.title = "activate";
					if (this.__menuOption_Menu_TurnOn.function == null)
					{
						this.__menuOption_Menu_TurnOn.function = new Action<BasePlayer>(this.Menu_TurnOn);
					}
					list.Add(this.__menuOption_Menu_TurnOn);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060005EC RID: 1516 RVA: 0x00007350 File Offset: 0x00005550
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Activate_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x000419CC File Offset: 0x0003FBCC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TimerSwitch.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00041A10 File Offset: 0x0003FC10
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.timerLength = info.msg.ioEntity.genericFloat2;
			this.timePassed = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x00041A60 File Offset: 0x0003FC60
	public void Update()
	{
		float num = Mathf.Clamp01(this.timePassed / this.timerLength);
		if (num < this.oldTimeFraction)
		{
			this.oldTimeFraction = num;
		}
		else
		{
			this.oldTimeFraction = Mathf.Lerp(this.oldTimeFraction, num, Time.deltaTime * 10f);
		}
		Quaternion localRotation = Quaternion.AngleAxis(360f * this.oldTimeFraction, Vector3.forward);
		this.timerDrum.localRotation = localRotation;
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x00007367 File Offset: 0x00005567
	[BaseEntity.Menu.Description("activate_switch_desc", "Activate the timer")]
	[BaseEntity.Menu.Icon("rotate")]
	[BaseEntity.Menu.ShowIf("Menu_Activate_ShowIf")]
	[BaseEntity.Menu("activate", "Activate")]
	public void Menu_TurnOn(BasePlayer player)
	{
		base.ServerRPC("SVSwitch");
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x00005A75 File Offset: 0x00003C75
	public bool Menu_Activate_ShowIf(BasePlayer player)
	{
		return !base.IsOn() && base.IsPowered();
	}
}
