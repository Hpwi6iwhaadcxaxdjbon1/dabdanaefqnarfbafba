using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000647 RID: 1607
public class CraftingQueue : SingletonComponent<CraftingQueue>
{
	// Token: 0x04001FF3 RID: 8179
	public GameObject queueContainer;

	// Token: 0x04001FF4 RID: 8180
	public GameObject queueItemPrefab;

	// Token: 0x04001FF5 RID: 8181
	private static Dictionary<int, int> CraftingItems = new Dictionary<int, int>();

	// Token: 0x04001FF6 RID: 8182
	public static bool isCrafting;

	// Token: 0x060023CC RID: 9164 RVA: 0x0001C492 File Offset: 0x0001A692
	protected override void Awake()
	{
		base.Awake();
		this.queueContainer.transform.DestroyAllChildren(true);
		this.UpdateVisibility();
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x000BDDDC File Offset: 0x000BBFDC
	public void ClientConnected()
	{
		CraftingQueueIcon[] componentsInChildren = this.queueContainer.GetComponentsInChildren<CraftingQueueIcon>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i]);
		}
		CraftingQueue.CraftingItems.Clear();
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x000BDE18 File Offset: 0x000BC018
	internal static void TaskStarted(int taskid, float time)
	{
		IEnumerable<CraftingQueueIcon> componentsInChildren = SingletonComponent<CraftingQueue>.Instance.queueContainer.GetComponentsInChildren<CraftingQueueIcon>(true);
		Func<CraftingQueueIcon, bool> <>9__0;
		Func<CraftingQueueIcon, bool> func;
		if ((func = <>9__0) == null)
		{
			func = (<>9__0 = ((CraftingQueueIcon x) => x.taskid == taskid));
		}
		foreach (CraftingQueueIcon craftingQueueIcon in Enumerable.Where<CraftingQueueIcon>(componentsInChildren, func))
		{
			craftingQueueIcon.OnTaskStart(time);
		}
		SingletonComponent<CraftingQueue>.Instance.UpdateVisibility();
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x0001C4B1 File Offset: 0x0001A6B1
	internal static int Count(ItemBlueprint bp)
	{
		if (!CraftingQueue.CraftingItems.ContainsKey(bp.targetItem.itemid))
		{
			return 0;
		}
		return CraftingQueue.CraftingItems[bp.targetItem.itemid];
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x000BDEA8 File Offset: 0x000BC0A8
	internal static void TaskAdd(int taskid, int itemid, int amount, int skinid)
	{
		if (!CraftingQueue.CraftingItems.ContainsKey(itemid))
		{
			CraftingQueue.CraftingItems.Add(itemid, 0);
		}
		CraftingQueue.CraftingItems[itemid] = CraftingQueue.CraftingItems[itemid] + 1;
		GameObject gameObject = Object.Instantiate<GameObject>(SingletonComponent<CraftingQueue>.Instance.queueItemPrefab);
		gameObject.transform.SetParent(SingletonComponent<CraftingQueue>.Instance.queueContainer.transform, false);
		gameObject.GetComponent<CraftingQueueIcon>().Init(taskid, itemid, amount, skinid);
		SingletonComponent<CraftingQueue>.Instance.UpdateVisibility();
	}

	// Token: 0x060023D1 RID: 9169 RVA: 0x000BDF28 File Offset: 0x000BC128
	internal static void TaskFinished(int taskid, bool success, int amountleft)
	{
		if (SingletonComponent<CraftingQueue>.Instance == null)
		{
			return;
		}
		if (SingletonComponent<CraftingQueue>.Instance.queueContainer == null)
		{
			return;
		}
		IEnumerable<CraftingQueueIcon> componentsInChildren = SingletonComponent<CraftingQueue>.Instance.queueContainer.GetComponentsInChildren<CraftingQueueIcon>(true);
		Func<CraftingQueueIcon, bool> <>9__0;
		Func<CraftingQueueIcon, bool> func;
		if ((func = <>9__0) == null)
		{
			func = (<>9__0 = ((CraftingQueueIcon x) => x.taskid == taskid));
		}
		foreach (CraftingQueueIcon craftingQueueIcon in Enumerable.Where<CraftingQueueIcon>(componentsInChildren, func))
		{
			if (CraftingQueue.CraftingItems.ContainsKey(craftingQueueIcon.item.itemid))
			{
				CraftingQueue.CraftingItems[craftingQueueIcon.item.itemid] = CraftingQueue.CraftingItems[craftingQueueIcon.item.itemid] - 1;
			}
			craftingQueueIcon.Finished(success, amountleft);
			LocalPlayer.AddCraftCount(craftingQueueIcon.item.Blueprint);
		}
		SingletonComponent<CraftingQueue>.Instance.UpdateVisibility();
	}

	// Token: 0x060023D2 RID: 9170 RVA: 0x0001C4E1 File Offset: 0x0001A6E1
	public void UpdateVisibility()
	{
		CraftingQueue.isCrafting = (Enumerable.Count<CraftingQueueIcon>(this.queueContainer.GetComponentsInChildren<CraftingQueueIcon>(true)) > 0);
	}

	// Token: 0x060023D3 RID: 9171 RVA: 0x0001C4FC File Offset: 0x0001A6FC
	public CraftingQueueIcon GetActive()
	{
		return Enumerable.FirstOrDefault<CraftingQueueIcon>(SingletonComponent<CraftingQueue>.Instance.queueContainer.GetComponentsInChildren<CraftingQueueIcon>(true));
	}
}
