using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000039 RID: 57
public class Lift : AnimatedBuildingBlock
{
	// Token: 0x04000223 RID: 547
	private Option __menuOption_Menu_UseLift;

	// Token: 0x04000224 RID: 548
	public GameObjectRef triggerPrefab;

	// Token: 0x04000225 RID: 549
	public string triggerBone;

	// Token: 0x04000226 RID: 550
	public float resetDelay = 5f;

	// Token: 0x060004A7 RID: 1191 RVA: 0x0003C620 File Offset: 0x0003A820
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("Lift.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_UseLift", 0.1f))
			{
				if (this.Menu_UseLift_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_UseLift.show = true;
					this.__menuOption_Menu_UseLift.showDisabled = false;
					this.__menuOption_Menu_UseLift.longUseOnly = false;
					this.__menuOption_Menu_UseLift.order = 0;
					this.__menuOption_Menu_UseLift.icon = "open_door";
					this.__menuOption_Menu_UseLift.desc = "use_lift_desc";
					this.__menuOption_Menu_UseLift.title = "use_lift";
					if (this.__menuOption_Menu_UseLift.function == null)
					{
						this.__menuOption_Menu_UseLift.function = new Action<BasePlayer>(this.Menu_UseLift);
					}
					list.Add(this.__menuOption_Menu_UseLift);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x060004A8 RID: 1192 RVA: 0x0000622F File Offset: 0x0000442F
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_UseLift_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0003C728 File Offset: 0x0003A928
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Lift.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x00006246 File Offset: 0x00004446
	[BaseEntity.Menu.Icon("open_door")]
	[BaseEntity.Menu("use_lift", "Use Lift")]
	[BaseEntity.Menu.Description("use_lift_desc", "Activate the lift")]
	[BaseEntity.Menu.ShowIf("Menu_UseLift_ShowIf")]
	public void Menu_UseLift(BasePlayer player)
	{
		base.ServerRPC("RPC_UseLift");
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00006253 File Offset: 0x00004453
	public bool Menu_UseLift_ShowIf(BasePlayer player)
	{
		return !base.IsOpen();
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool NeedsCrosshair()
	{
		return true;
	}
}
