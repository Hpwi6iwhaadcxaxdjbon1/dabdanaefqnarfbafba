using System;
using System.Collections.Generic;
using GameMenu;

// Token: 0x02000117 RID: 279
public class ItemModRFListener : ItemMod
{
	// Token: 0x0400081B RID: 2075
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x0400081C RID: 2076
	public GameObjectRef entityPrefab;

	// Token: 0x0400081D RID: 2077
	private uint pendingPagerID;

	// Token: 0x06000B95 RID: 2965 RVA: 0x00059934 File Offset: 0x00057B34
	public PagerEntity GetPagerEnt(Item item, bool isServer = true)
	{
		BaseNetworkable baseNetworkable = null;
		if (item.instanceData == null)
		{
			return null;
		}
		if (!isServer)
		{
			baseNetworkable = BaseNetworkable.clientEntities.Find(item.instanceData.subEntity);
		}
		if (baseNetworkable)
		{
			return baseNetworkable.GetComponent<PagerEntity>();
		}
		return null;
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x00059978 File Offset: 0x00057B78
	public override void GetMenuOptions(Item item, List<Option> list, BasePlayer player)
	{
		PagerEntity pagerEnt = this.GetPagerEnt(item, false);
		if (pagerEnt == null)
		{
			return;
		}
		if (pagerEnt.HasFlag(PagerEntity.Flag_Silent))
		{
			list.Add(new Option
			{
				icon = "circle_open",
				title = "silent_off",
				desc = "silent_off_desc",
				command = "silentoff",
				order = 1
			});
		}
		else
		{
			list.Add(new Option
			{
				icon = "circle_closed",
				title = "silent_on",
				desc = "silent_on_desc",
				command = "silenton",
				order = 2
			});
		}
		if (pagerEnt.HasFlag(BaseEntity.Flags.On))
		{
			list.Add(new Option
			{
				icon = "close",
				title = "stop",
				desc = "stop_desc",
				command = "stop",
				order = 0
			});
		}
		if (item.GetOwnerPlayer() == player)
		{
			Option option = default(Option);
			option.title = "setfreq";
			option.desc = "setfreq_desc";
			option.icon = "broadcast";
			option.order = 10;
			option.show = true;
			option.showDisabled = false;
			option.function = new Action<BasePlayer>(this.ConfigureClicked);
			this.pendingPagerID = pagerEnt.net.ID;
			list.Add(option);
		}
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x00059B04 File Offset: 0x00057D04
	public void ConfigureClicked(BasePlayer player)
	{
		if (this.pendingPagerID == 0U)
		{
			return;
		}
		BaseNetworkable baseNetworkable = BaseNetworkable.clientEntities.Find(this.pendingPagerID);
		if (baseNetworkable)
		{
			PagerEntity component = baseNetworkable.GetComponent<PagerEntity>();
			if (component)
			{
				FrequencyConfig component2 = GameManager.client.CreatePrefab(this.frequencyPanelPrefab.resourcePath, true).GetComponent<FrequencyConfig>();
				component2.SetRFObj(component);
				component2.OpenDialog();
			}
		}
		this.pendingPagerID = 0U;
	}
}
