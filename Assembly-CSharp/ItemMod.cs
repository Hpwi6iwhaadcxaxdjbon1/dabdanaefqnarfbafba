using System;
using System.Collections.Generic;
using GameMenu;
using UnityEngine;

// Token: 0x02000442 RID: 1090
public class ItemMod : MonoBehaviour
{
	// Token: 0x040016F8 RID: 5880
	private ItemMod[] siblingMods;

	// Token: 0x06001A2E RID: 6702 RVA: 0x00015AA9 File Offset: 0x00013CA9
	public virtual void ModInit()
	{
		this.siblingMods = base.GetComponents<ItemMod>();
	}

	// Token: 0x06001A2F RID: 6703 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnChanged(Item item)
	{
	}

	// Token: 0x06001A30 RID: 6704 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void GetMenuOptions(Item item, List<Option> list, BasePlayer player)
	{
	}

	// Token: 0x06001A31 RID: 6705 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool BeltSelect(Item item, BasePlayer player)
	{
		return false;
	}

	// Token: 0x06001A32 RID: 6706 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void CL_DoAction(Item item, BasePlayer player)
	{
	}

	// Token: 0x06001A33 RID: 6707 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnObjectSetup(Item item, GameObject obj)
	{
	}

	// Token: 0x06001A34 RID: 6708 RVA: 0x00092988 File Offset: 0x00090B88
	public virtual bool CanDoAction(Item item, BasePlayer player)
	{
		ItemMod[] array = this.siblingMods;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].Passes(item))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001A35 RID: 6709 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool Passes(Item item)
	{
		return true;
	}

	// Token: 0x06001A36 RID: 6710 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnRemovedFromWorld(Item item)
	{
	}

	// Token: 0x06001A37 RID: 6711 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnMovedToWorld(Item item)
	{
	}
}
