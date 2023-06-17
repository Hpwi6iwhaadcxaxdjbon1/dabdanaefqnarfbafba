using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class WaterWell : LiquidContainer
{
	// Token: 0x0400031C RID: 796
	private Option __menuOption_Menu_Pump;

	// Token: 0x0400031D RID: 797
	public Animator animator;

	// Token: 0x0400031E RID: 798
	private const BaseEntity.Flags Pumping = BaseEntity.Flags.Reserved2;

	// Token: 0x0400031F RID: 799
	private const BaseEntity.Flags WaterFlow = BaseEntity.Flags.Reserved3;

	// Token: 0x04000320 RID: 800
	public float caloriesPerPump = 5f;

	// Token: 0x04000321 RID: 801
	public float pressurePerPump = 0.2f;

	// Token: 0x04000322 RID: 802
	public float pressureForProduction = 1f;

	// Token: 0x04000323 RID: 803
	public float currentPressure;

	// Token: 0x04000324 RID: 804
	public int waterPerPump = 50;

	// Token: 0x04000325 RID: 805
	public GameObject waterLevelObj;

	// Token: 0x04000326 RID: 806
	public float waterLevelObjFullOffset;

	// Token: 0x04000327 RID: 807
	private float cachedWaterLevel;

	// Token: 0x06000610 RID: 1552 RVA: 0x000426DC File Offset: 0x000408DC
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("WaterWell.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Pump", 0.1f))
			{
				if (this.Menu_Pump_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Pump.show = true;
					this.__menuOption_Menu_Pump.showDisabled = false;
					this.__menuOption_Menu_Pump.longUseOnly = false;
					this.__menuOption_Menu_Pump.order = -100;
					this.__menuOption_Menu_Pump.icon = "rotate";
					this.__menuOption_Menu_Pump.desc = "pmp_desc";
					this.__menuOption_Menu_Pump.title = "pump";
					if (this.__menuOption_Menu_Pump.function == null)
					{
						this.__menuOption_Menu_Pump.function = new Action<BasePlayer>(this.Menu_Pump);
					}
					list.Add(this.__menuOption_Menu_Pump);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x06000611 RID: 1553 RVA: 0x000074F8 File Offset: 0x000056F8
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Pump_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x000427E4 File Offset: 0x000409E4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WaterWell.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x0000750F File Offset: 0x0000570F
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.waterwell != null)
		{
			this.cachedWaterLevel = info.msg.waterwell.waterLevel;
		}
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x0000753B File Offset: 0x0000573B
	public float GetWaterAmount()
	{
		if (base.isClient)
		{
			return this.cachedWaterLevel;
		}
		return 0f;
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x00007551 File Offset: 0x00005751
	[BaseEntity.Menu.Icon("rotate")]
	[BaseEntity.Menu.ShowIf("Menu_Pump_ShowIf")]
	[BaseEntity.Menu("pump", "Pump", Order = -100)]
	[BaseEntity.Menu.Description("pmp_desc", "Pump the Well")]
	public void Menu_Pump(BasePlayer player)
	{
		base.ServerRPC("RPC_Pump");
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x00042828 File Offset: 0x00040A28
	public bool Menu_Pump_ShowIf(BasePlayer player)
	{
		return !base.HasFlag(BaseEntity.Flags.Reserved2) && player.lookingAtCollider.CompareTag("Usable Primary") && Vector3.Distance(player.transform.position, base.transform.position) <= 2f && player.metabolism.calories.value > this.caloriesPerPump;
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x0000755E File Offset: 0x0000575E
	public override bool ShouldShowLootMenus()
	{
		return base.ShouldShowLootMenus() && LocalPlayer.Entity.lookingAtEntity == this && LocalPlayer.Entity.lookingAtCollider.CompareTag("Usable Secondary");
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x00003066 File Offset: 0x00001266
	public override bool DrinkEligable(BasePlayer player)
	{
		return player.lookingAtCollider.CompareTag("Usable Secondary");
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x00042890 File Offset: 0x00040A90
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		float num = Mathf.Clamp01(this.GetWaterAmount() / 500f);
		this.waterLevelObj.transform.localPosition = Vector3.Lerp(this.waterLevelObj.transform.localPosition, new Vector3(0f, num * this.waterLevelObjFullOffset, 0f), Time.deltaTime * 2f);
	}
}
