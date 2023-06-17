using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200065B RID: 1627
public class ItemPanel : SingletonComponent<ItemPanel>
{
	// Token: 0x0400205A RID: 8282
	private ItemIcon item;

	// Token: 0x0600244D RID: 9293 RVA: 0x0001CA5A File Offset: 0x0001AC5A
	private void Start()
	{
		this.OnChanged();
	}

	// Token: 0x0600244E RID: 9294 RVA: 0x0001CA62 File Offset: 0x0001AC62
	public void Update()
	{
		if (this.item != SelectedItem.selectedItem)
		{
			this.item = SelectedItem.selectedItem;
			this.OnChanged();
		}
	}

	// Token: 0x0600244F RID: 9295 RVA: 0x000C05F8 File Offset: 0x000BE7F8
	private void OnChanged()
	{
		base.transform.DestroyAllChildren(false);
		if (this.item == null || this.item.item == null || this.item.item.info.panel == null)
		{
			base.GetComponent<LayoutElement>().ignoreLayout = true;
			base.GetComponent<CanvasGroup>().alpha = 0f;
			base.GetComponent<CanvasGroup>().interactable = false;
			return;
		}
		base.GetComponent<LayoutElement>().ignoreLayout = false;
		base.GetComponent<CanvasGroup>().alpha = 1f;
		base.GetComponent<CanvasGroup>().interactable = true;
		GameObject gameObject = base.gameObject.InstantiateChild(this.item.item.info.panel);
		if (gameObject != null)
		{
			gameObject.BroadcastMessage("OnItem", this.item.item);
		}
	}
}
