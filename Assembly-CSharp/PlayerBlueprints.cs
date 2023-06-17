using System;
using System.Linq;
using Facepunch.Steamworks;
using ProtoBuf;
using UnityEngine.Assertions;

// Token: 0x0200030E RID: 782
public class PlayerBlueprints : EntityComponent<BasePlayer>
{
	// Token: 0x0400115A RID: 4442
	public SteamInventory steamInventory;

	// Token: 0x0400115B RID: 4443
	[NonSerialized]
	private int[] craftableItems;

	// Token: 0x06001469 RID: 5225 RVA: 0x00011607 File Offset: 0x0000F807
	public void ClientInit()
	{
		Assert.IsTrue(base.baseEntity.IsLocalPlayer(), "PlayerBlueprints.ClientInit should only be called for the local player!");
		UIBlueprints.Refresh();
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x00011623 File Offset: 0x0000F823
	public void ClientUpdate(PersistantPlayer info)
	{
		if (this.craftableItems != null && this.craftableItems.Length == Enumerable.Count<int>(info.unlockedItems))
		{
			return;
		}
		this.craftableItems = info.unlockedItems.ToArray();
		UIBlueprints.Refresh();
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x0007E638 File Offset: 0x0007C838
	public bool HasUnlocked(ItemDefinition targetItem)
	{
		if (!targetItem.Blueprint || !targetItem.Blueprint.NeedsSteamItem)
		{
			int[] defaultBlueprints = ItemManager.defaultBlueprints;
			for (int i = 0; i < defaultBlueprints.Length; i++)
			{
				if (defaultBlueprints[i] == targetItem.itemid)
				{
					return true;
				}
			}
			return base.baseEntity.isClient && this.craftableItems != null && Enumerable.Contains<int>(this.craftableItems, targetItem.itemid);
		}
		if (targetItem.steamItem != null && !this.steamInventory.HasItem(targetItem.steamItem.id))
		{
			return false;
		}
		if (targetItem.steamItem == null)
		{
			bool flag = false;
			foreach (ItemSkinDirectory.Skin skin in targetItem.skins)
			{
				if (this.steamInventory.HasItem(skin.id))
				{
					flag = true;
					break;
				}
			}
			if (!flag && targetItem.skins2 != null)
			{
				foreach (Inventory.Definition definition in targetItem.skins2)
				{
					if (this.steamInventory.HasItem(definition.Id))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x0007E764 File Offset: 0x0007C964
	public bool CanCraft(int itemid, int skinItemId)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemid);
		return !(itemDefinition == null) && (skinItemId == 0 || this.steamInventory.HasItem(skinItemId)) && LocalPlayer.GetCraftLevel() >= (float)itemDefinition.Blueprint.workbenchLevelRequired && this.HasUnlocked(itemDefinition);
	}
}
