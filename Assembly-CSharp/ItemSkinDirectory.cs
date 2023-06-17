using System;
using System.Linq;
using UnityEngine;

// Token: 0x020005DB RID: 1499
public class ItemSkinDirectory : ScriptableObject
{
	// Token: 0x04001E19 RID: 7705
	private static ItemSkinDirectory _Instance;

	// Token: 0x04001E1A RID: 7706
	public ItemSkinDirectory.Skin[] skins;

	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06002208 RID: 8712 RVA: 0x0001B038 File Offset: 0x00019238
	public static ItemSkinDirectory Instance
	{
		get
		{
			if (ItemSkinDirectory._Instance == null)
			{
				ItemSkinDirectory._Instance = FileSystem.Load<ItemSkinDirectory>("assets/skins.asset", true);
			}
			return ItemSkinDirectory._Instance;
		}
	}

	// Token: 0x06002209 RID: 8713 RVA: 0x000B7944 File Offset: 0x000B5B44
	public static ItemSkinDirectory.Skin[] ForItem(ItemDefinition item)
	{
		return Enumerable.ToArray<ItemSkinDirectory.Skin>(Enumerable.Where<ItemSkinDirectory.Skin>(ItemSkinDirectory.Instance.skins, (ItemSkinDirectory.Skin x) => x.isSkin && x.itemid == item.itemid));
	}

	// Token: 0x0600220A RID: 8714 RVA: 0x000B7980 File Offset: 0x000B5B80
	public static ItemSkinDirectory.Skin FindByInventoryDefinitionId(int id)
	{
		return Enumerable.FirstOrDefault<ItemSkinDirectory.Skin>(Enumerable.Where<ItemSkinDirectory.Skin>(ItemSkinDirectory.Instance.skins, (ItemSkinDirectory.Skin x) => x.id == id));
	}

	// Token: 0x020005DC RID: 1500
	[Serializable]
	public struct Skin
	{
		// Token: 0x04001E1B RID: 7707
		public int id;

		// Token: 0x04001E1C RID: 7708
		public int itemid;

		// Token: 0x04001E1D RID: 7709
		public string name;

		// Token: 0x04001E1E RID: 7710
		public bool isSkin;

		// Token: 0x04001E1F RID: 7711
		private SteamInventoryItem _invItem;

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x0600220C RID: 8716 RVA: 0x0001B05C File Offset: 0x0001925C
		public SteamInventoryItem invItem
		{
			get
			{
				if (this._invItem == null)
				{
					this._invItem = FileSystem.Load<SteamInventoryItem>(this.name, true);
				}
				return this._invItem;
			}
		}
	}
}
