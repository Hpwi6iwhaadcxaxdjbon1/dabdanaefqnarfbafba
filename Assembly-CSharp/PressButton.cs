using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000040 RID: 64
public class PressButton : IOEntity
{
	// Token: 0x0400028D RID: 653
	private Option __menuOption_Menu_Press;

	// Token: 0x0400028E RID: 654
	public float pressDuration = 5f;

	// Token: 0x0600051C RID: 1308 RVA: 0x0003E38C File Offset: 0x0003C58C
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("PressButton.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Press", 0.1f))
			{
				if (this.Menu_Press_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Press.show = true;
					this.__menuOption_Menu_Press.showDisabled = false;
					this.__menuOption_Menu_Press.longUseOnly = false;
					this.__menuOption_Menu_Press.order = 0;
					this.__menuOption_Menu_Press.icon = "press";
					this.__menuOption_Menu_Press.desc = "press_button_desc";
					this.__menuOption_Menu_Press.title = "press";
					if (this.__menuOption_Menu_Press.function == null)
					{
						this.__menuOption_Menu_Press.function = new Action<BasePlayer>(this.Menu_Press);
					}
					list.Add(this.__menuOption_Menu_Press);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x0600051D RID: 1309 RVA: 0x000068A7 File Offset: 0x00004AA7
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Press_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x0003E494 File Offset: 0x0003C694
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PressButton.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x000068BE File Offset: 0x00004ABE
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.pressDuration = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x000068EA File Offset: 0x00004AEA
	[BaseEntity.Menu.ShowIf("Menu_Press_ShowIf")]
	[BaseEntity.Menu.Icon("press")]
	[BaseEntity.Menu.Description("press_button_desc", "Press")]
	[BaseEntity.Menu("press", "Press")]
	public void Menu_Press(BasePlayer player)
	{
		base.ServerRPC("Press");
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x00005862 File Offset: 0x00003A62
	public bool Menu_Press_ShowIf(BasePlayer player)
	{
		return !base.IsOn();
	}
}
