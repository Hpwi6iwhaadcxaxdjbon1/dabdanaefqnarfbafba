using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020002FD RID: 765
public class VisualStorageContainer : LootContainer
{
	// Token: 0x04001102 RID: 4354
	public VisualStorageContainerNode[] displayNodes;

	// Token: 0x04001103 RID: 4355
	public VisualStorageContainer.DisplayModel[] displayModels;

	// Token: 0x04001104 RID: 4356
	public Transform nodeParent;

	// Token: 0x04001105 RID: 4357
	public GameObject defaultDisplayModel;

	// Token: 0x06001428 RID: 5160 RVA: 0x0007D8B4 File Offset: 0x0007BAB4
	public void ClearRigidBodies()
	{
		if (this.displayModels == null)
		{
			return;
		}
		foreach (VisualStorageContainer.DisplayModel displayModel in this.displayModels)
		{
			if (displayModel != null)
			{
				Object.Destroy(displayModel.displayModel.GetComponentInChildren<Rigidbody>());
			}
		}
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x0007D8F8 File Offset: 0x0007BAF8
	public void SetItemsVisible(bool vis)
	{
		if (this.displayModels == null)
		{
			return;
		}
		foreach (VisualStorageContainer.DisplayModel displayModel in this.displayModels)
		{
			if (displayModel != null)
			{
				LODGroup componentInChildren = displayModel.displayModel.GetComponentInChildren<LODGroup>();
				if (componentInChildren)
				{
					componentInChildren.localReferencePoint = (vis ? Vector3.zero : new Vector3(10000f, 10000f, 10000f));
				}
				else
				{
					Debug.Log("VisualStorageContainer item missing LODGroup" + displayModel.displayModel.gameObject.name);
				}
			}
		}
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x00011348 File Offset: 0x0000F548
	public void ItemUpdateComplete()
	{
		this.ClearRigidBodies();
		this.SetItemsVisible(true);
	}

	// Token: 0x0600142B RID: 5163 RVA: 0x0007D984 File Offset: 0x0007BB84
	public void UpdateVisibleItems(ItemContainer msg)
	{
		for (int i = 0; i < this.displayModels.Length; i++)
		{
			VisualStorageContainer.DisplayModel displayModel = this.displayModels[i];
			if (displayModel != null)
			{
				Object.Destroy(displayModel.displayModel);
				this.displayModels[i] = null;
			}
		}
		if (msg == null)
		{
			return;
		}
		foreach (Item item in msg.contents)
		{
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(item.itemid);
			GameObject gameObject;
			if (itemDefinition.worldModelPrefab != null && itemDefinition.worldModelPrefab.isValid)
			{
				gameObject = itemDefinition.worldModelPrefab.Instantiate(null);
			}
			else
			{
				gameObject = Object.Instantiate<GameObject>(this.defaultDisplayModel);
			}
			if (gameObject)
			{
				gameObject.transform.position = this.displayNodes[item.slot].transform.position + new Vector3(0f, 0.25f, 0f);
				gameObject.transform.rotation = this.displayNodes[item.slot].transform.rotation;
				Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
				rigidbody.mass = 1f;
				rigidbody.drag = 0.1f;
				rigidbody.angularDrag = 0.1f;
				rigidbody.interpolation = 1;
				rigidbody.constraints = 10;
				this.displayModels[item.slot].displayModel = gameObject;
				this.displayModels[item.slot].slot = item.slot;
				this.displayModels[item.slot].def = itemDefinition;
				gameObject.SetActive(true);
			}
		}
		this.SetItemsVisible(false);
		base.CancelInvoke(new Action(this.ItemUpdateComplete));
		base.Invoke(new Action(this.ItemUpdateComplete), 1f);
	}

	// Token: 0x020002FE RID: 766
	[Serializable]
	public class DisplayModel
	{
		// Token: 0x04001106 RID: 4358
		public GameObject displayModel;

		// Token: 0x04001107 RID: 4359
		public ItemDefinition def;

		// Token: 0x04001108 RID: 4360
		public int slot;
	}
}
