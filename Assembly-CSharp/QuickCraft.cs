using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x0200066D RID: 1645
public class QuickCraft : MonoBehaviour, IInventoryChanged
{
	// Token: 0x04002098 RID: 8344
	public GameObjectRef craftButton;

	// Token: 0x04002099 RID: 8345
	public GameObject empty;

	// Token: 0x0400209A RID: 8346
	private bool isDirty;

	// Token: 0x0400209B RID: 8347
	private int lastHash;

	// Token: 0x0400209C RID: 8348
	private const int buttonCount = 18;

	// Token: 0x0600249F RID: 9375 RVA: 0x0001CD97 File Offset: 0x0001AF97
	private void Awake()
	{
		base.transform.DestroyAllChildren(false);
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x0001CDA5 File Offset: 0x0001AFA5
	private void OnEnable()
	{
		base.StartCoroutine(this.WatchForChanges());
		GlobalMessages.onInventoryChanged.Add(this);
	}

	// Token: 0x060024A1 RID: 9377 RVA: 0x0001C384 File Offset: 0x0001A584
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		GlobalMessages.onInventoryChanged.Remove(this);
	}

	// Token: 0x060024A2 RID: 9378 RVA: 0x0001CDBF File Offset: 0x0001AFBF
	public void OnInventoryChanged()
	{
		this.Dirty();
	}

	// Token: 0x060024A3 RID: 9379 RVA: 0x0001CDC7 File Offset: 0x0001AFC7
	private void Dirty()
	{
		this.isDirty = true;
	}

	// Token: 0x060024A4 RID: 9380 RVA: 0x0001CDD0 File Offset: 0x0001AFD0
	private IEnumerator WatchForChanges()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForSeconds(0.5f);
			if (!UIInventory.isOpen)
			{
				yield return CoroutineEx.waitForSeconds(0.3f);
			}
			else if (!this.isDirty)
			{
				yield return CoroutineEx.waitForSeconds(1f);
			}
			else
			{
				this.Rebuild();
			}
		}
		yield break;
	}

	// Token: 0x060024A5 RID: 9381 RVA: 0x000C1A80 File Offset: 0x000BFC80
	private void Rebuild()
	{
		if (ItemManager.bpList == null)
		{
			return;
		}
		this.isDirty = false;
		List<ItemBlueprint> list = Pool.GetList<ItemBlueprint>();
		foreach (ItemBlueprint itemBlueprint in ItemManager.bpList)
		{
			if (LocalPlayer.HasCraftLevel(itemBlueprint.workbenchLevelRequired) && LocalPlayer.HasUnlocked(itemBlueprint.targetItem) && LocalPlayer.HasItems(itemBlueprint.ingredients, 1))
			{
				list.Add(itemBlueprint);
			}
		}
		list.Sort(LocalPlayer.ItemBlueprintDescendingOrder);
		if (list.Count > 18)
		{
			list.RemoveRange(18, list.Count - 18);
		}
		int num = 32;
		foreach (ItemBlueprint itemBlueprint2 in list)
		{
			num += itemBlueprint2.GetHashCode();
		}
		if (num == this.lastHash)
		{
			Pool.FreeList<ItemBlueprint>(ref list);
			return;
		}
		this.lastHash = num;
		base.transform.RetireAllChildren(GameManager.client);
		this.empty.SetActive(list.Count == 0);
		foreach (ItemBlueprint item in list)
		{
			GameManager.client.CreatePrefab(this.craftButton.resourcePath, base.transform, true).GetComponent<QuickCraftButton>().Setup(item);
		}
		Pool.FreeList<ItemBlueprint>(ref list);
	}
}
