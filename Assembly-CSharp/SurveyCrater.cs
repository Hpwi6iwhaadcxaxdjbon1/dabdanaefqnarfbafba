using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class SurveyCrater : BaseCombatEntity
{
	// Token: 0x04000304 RID: 772
	private Option __menuOption_Menu_AnalysisComplete;

	// Token: 0x04000305 RID: 773
	private ResourceDispenser resourceDispenser;

	// Token: 0x060005E2 RID: 1506 RVA: 0x000416F8 File Offset: 0x0003F8F8
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("SurveyCrater.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_AnalysisComplete", 0.1f))
			{
				if (this.Menu_PerformAnalysis_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_AnalysisComplete.show = true;
					this.__menuOption_Menu_AnalysisComplete.showDisabled = false;
					this.__menuOption_Menu_AnalysisComplete.longUseOnly = false;
					this.__menuOption_Menu_AnalysisComplete.order = 0;
					this.__menuOption_Menu_AnalysisComplete.time = 3f;
					this.__menuOption_Menu_AnalysisComplete.icon = "examine";
					this.__menuOption_Menu_AnalysisComplete.desc = "analyze_desc";
					this.__menuOption_Menu_AnalysisComplete.title = "analyze";
					if (this.__menuOption_Menu_AnalysisComplete.function == null)
					{
						this.__menuOption_Menu_AnalysisComplete.function = new Action<BasePlayer>(this.Menu_AnalysisComplete);
					}
					if (this.__menuOption_Menu_AnalysisComplete.timeStart == null)
					{
						this.__menuOption_Menu_AnalysisComplete.timeStart = new Action(this.Menu_Analysis_Start);
					}
					list.Add(this.__menuOption_Menu_AnalysisComplete);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x060005E3 RID: 1507 RVA: 0x0000732C File Offset: 0x0000552C
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_PerformAnalysis_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00041834 File Offset: 0x0003FA34
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SurveyCrater.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00004962 File Offset: 0x00002B62
	public override float BoundsPadding()
	{
		return 2f;
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00041878 File Offset: 0x0003FA78
	protected override void ClientInit(Entity info)
	{
		Vector3 vector;
		Vector3 toDirection;
		if (base.transform.GetGroundInfoTerrainOnly(out vector, out toDirection, 100f))
		{
			base.transform.Find("visuals").rotation = Quaternion.FromToRotation(Vector3.up, toDirection);
		}
		base.ClientInit(info);
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00007343 File Offset: 0x00005543
	[BaseEntity.Menu.Description("analyze_desc", "Perform a mineral analysis to see what is contained here")]
	[BaseEntity.Menu.ShowIf("Menu_PerformAnalysis_ShowIf")]
	[BaseEntity.Menu.Icon("examine")]
	[BaseEntity.Menu("analyze", "Perform Analysis", Time = 3f, OnStart = "Menu_Analysis_Start")]
	public void Menu_AnalysisComplete(BasePlayer player)
	{
		base.ServerRPC("AnalysisComplete");
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x00002ECE File Offset: 0x000010CE
	public void Menu_Analysis_Start()
	{
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x00002D44 File Offset: 0x00000F44
	public bool Menu_PerformAnalysis_ShowIf(BasePlayer player)
	{
		return true;
	}
}
