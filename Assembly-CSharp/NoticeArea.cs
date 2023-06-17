using System;
using UnityEngine;

// Token: 0x02000669 RID: 1641
public class NoticeArea : SingletonComponent<NoticeArea>
{
	// Token: 0x04002089 RID: 8329
	public GameObject itemPickupPrefab;

	// Token: 0x0400208A RID: 8330
	public GameObject itemDroppedPrefab;

	// Token: 0x0600248E RID: 9358 RVA: 0x000C1334 File Offset: 0x000BF534
	public static void ItemPickUp(ItemDefinition def, int amount, string nameOverride)
	{
		if (SingletonComponent<NoticeArea>.Instance == null)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>((amount > 0) ? SingletonComponent<NoticeArea>.Instance.itemPickupPrefab : SingletonComponent<NoticeArea>.Instance.itemDroppedPrefab);
		if (gameObject == null)
		{
			return;
		}
		gameObject.transform.SetParent(SingletonComponent<NoticeArea>.Instance.transform, false);
		ItemPickupNotice component = gameObject.GetComponent<ItemPickupNotice>();
		if (component == null)
		{
			return;
		}
		component.itemInfo = def;
		component.amount = amount;
		if (!string.IsNullOrEmpty(nameOverride))
		{
			component.Text.text = nameOverride;
		}
	}
}
