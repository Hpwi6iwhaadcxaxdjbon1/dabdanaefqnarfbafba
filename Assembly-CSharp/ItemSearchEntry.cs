using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000E2 RID: 226
public class ItemSearchEntry : MonoBehaviour
{
	// Token: 0x0400070A RID: 1802
	public Button button;

	// Token: 0x0400070B RID: 1803
	public Text text;

	// Token: 0x0400070C RID: 1804
	public RawImage image;

	// Token: 0x0400070D RID: 1805
	public RawImage bpImage;

	// Token: 0x0400070E RID: 1806
	private ItemDefinition itemInfo;

	// Token: 0x0400070F RID: 1807
	private AddSellOrderManager manager;

	// Token: 0x06000AB4 RID: 2740 RVA: 0x00055D88 File Offset: 0x00053F88
	public void Setup(ItemDefinition info, AddSellOrderManager mgr)
	{
		this.itemInfo = info;
		this.text.text = info.displayName.translated;
		this.image.texture = info.iconSprite.texture;
		this.bpImage.enabled = (info.Blueprint != null && info.Blueprint.isResearchable && info.Blueprint.userCraftable && !info.Blueprint.defaultBlueprint);
		this.manager = mgr;
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0000A81D File Offset: 0x00008A1D
	public void Clicked()
	{
		this.manager.ItemSelectionMade(this.itemInfo, false);
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0000A831 File Offset: 0x00008A31
	public void ClickedBP()
	{
		this.manager.ItemSelectionMade(this.itemInfo, true);
	}
}
