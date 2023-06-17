using System;
using System.Collections.Generic;
using GameMenu;
using UnityEngine;

// Token: 0x0200045A RID: 1114
public class ItemModMenuOption : ItemMod
{
	// Token: 0x04001737 RID: 5943
	public string commandName;

	// Token: 0x04001738 RID: 5944
	public ItemMod actionTarget;

	// Token: 0x04001739 RID: 5945
	public BaseEntity.Menu.Option option;

	// Token: 0x0400173A RID: 5946
	[Tooltip("If true, this is the command that will run when an item is 'selected' on the toolbar")]
	public bool isPrimaryOption = true;

	// Token: 0x06001A5F RID: 6751 RVA: 0x00092B9C File Offset: 0x00090D9C
	public override void GetMenuOptions(Item item, List<Option> list, BasePlayer player)
	{
		if (!this.actionTarget.CanDoAction(item, player))
		{
			return;
		}
		list.Add(new Option
		{
			iconSprite = this.option.icon,
			title = this.option.name.token,
			desc = this.option.description.token,
			command = this.commandName,
			order = this.option.order,
			show = true,
			showDisabled = false
		});
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x00015D1D File Offset: 0x00013F1D
	public override bool BeltSelect(Item item, BasePlayer player)
	{
		if (!this.isPrimaryOption)
		{
			return false;
		}
		if (!this.actionTarget.CanDoAction(item, player))
		{
			return false;
		}
		this.actionTarget.CL_DoAction(item, player);
		LocalPlayer.ItemCommand(item.uid, this.commandName);
		return true;
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x00092C38 File Offset: 0x00090E38
	private void OnValidate()
	{
		if (this.actionTarget == null)
		{
			Debug.LogWarning("ItemModMenuOption: actionTarget is null!", base.gameObject);
		}
		if (string.IsNullOrEmpty(this.commandName))
		{
			Debug.LogWarning("ItemModMenuOption: commandName can't be empty!", base.gameObject);
		}
		if (this.option.icon == null)
		{
			Debug.LogWarning("No icon set for ItemModMenuOption " + base.gameObject.name, base.gameObject);
		}
	}
}
