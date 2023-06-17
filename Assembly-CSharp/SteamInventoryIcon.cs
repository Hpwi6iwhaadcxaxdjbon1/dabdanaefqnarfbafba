using System;
using Facepunch.Extend;
using Facepunch.Steamworks;
using Rust.Workshop;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006A1 RID: 1697
public class SteamInventoryIcon : MonoBehaviour
{
	// Token: 0x17000277 RID: 631
	// (get) Token: 0x060025D2 RID: 9682 RVA: 0x0001D85E File Offset: 0x0001BA5E
	// (set) Token: 0x060025D3 RID: 9683 RVA: 0x0001D866 File Offset: 0x0001BA66
	private Workshop.Item WorkshopItem { get; set; }

	// Token: 0x060025D4 RID: 9684 RVA: 0x000C6E24 File Offset: 0x000C5024
	public void Setup(Inventory.Item item, ItemSkinDirectory.Skin skin)
	{
		TransformEx.FindChildRecursive(base.transform, "ItemIcon").GetComponent<Image>().sprite = skin.invItem.icon;
		TransformEx.FindChildRecursive(base.transform, "ItemName").GetComponent<Text>().text = skin.invItem.displayName.english;
	}

	// Token: 0x060025D5 RID: 9685 RVA: 0x000C6E84 File Offset: 0x000C5084
	internal void Setup(Inventory.Item item)
	{
		if (item.Definition.GetProperty<ulong>("workshopdownload") == 0UL)
		{
			return;
		}
		ApprovedSkinInfo approvedSkinInfo = Approved.FindByInventoryId((ulong)((long)item.DefinitionId));
		if (approvedSkinInfo == null)
		{
			return;
		}
		TransformEx.FindChildRecursive(base.transform, "ItemName").GetComponent<Text>().text = approvedSkinInfo.Name;
		TransformEx.FindChildRecursive(base.transform, "ItemIcon").GetComponent<Image>().sprite = WorkshopIconLoader.Find(approvedSkinInfo.WorkshopdId, null, null);
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x0001D86F File Offset: 0x0001BA6F
	public void OnClicked()
	{
		Application.OpenURL("http://steamcommunity.com/id/garry/inventory/#252490");
	}
}
