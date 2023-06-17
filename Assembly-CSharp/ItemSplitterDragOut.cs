using System;
using UnityEngine;

// Token: 0x0200065E RID: 1630
public class ItemSplitterDragOut : MonoBehaviour, IDraggable
{
	// Token: 0x04002062 RID: 8290
	public ItemSplitter rootSplitter;

	// Token: 0x06002459 RID: 9305 RVA: 0x0001CB08 File Offset: 0x0001AD08
	public object GetDragData()
	{
		return this.rootSplitter.dragInfo;
	}

	// Token: 0x0600245A RID: 9306 RVA: 0x0001C971 File Offset: 0x0001AB71
	public string GetDragType()
	{
		return "item";
	}

	// Token: 0x0600245B RID: 9307 RVA: 0x0001CB15 File Offset: 0x0001AD15
	public Sprite GetDragSprite()
	{
		if (SelectedItem.item == null)
		{
			return null;
		}
		return SelectedItem.item.info.iconSprite;
	}
}
