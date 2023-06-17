using System;
using UnityEngine;

// Token: 0x02000654 RID: 1620
public class ItemDropCommand : MonoBehaviour
{
	// Token: 0x04002032 RID: 8242
	public string command = "drop";

	// Token: 0x0600241C RID: 9244 RVA: 0x000BF4B0 File Offset: 0x000BD6B0
	private void OnDroppedValue(ItemIcon.DragInfo dropInfo)
	{
		if (dropInfo.item == null)
		{
			return;
		}
		if (this.command == "drop" && !dropInfo.canDrop)
		{
			return;
		}
		if (dropInfo.item.info.inventoryDropSound != null)
		{
			SoundManager.PlayOneshot(dropInfo.item.info.inventoryDropSound, null, true, default(Vector3));
		}
		LocalPlayer.ItemCommandEx<int>(dropInfo.item.uid, this.command, dropInfo.amount);
	}
}
