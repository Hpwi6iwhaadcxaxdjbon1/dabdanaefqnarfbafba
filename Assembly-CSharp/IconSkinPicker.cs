using System;
using Facepunch.Steamworks;
using UnityEngine;

// Token: 0x02000206 RID: 518
public class IconSkinPicker : MonoBehaviour
{
	// Token: 0x04000CB5 RID: 3253
	public GameObjectRef pickerIcon;

	// Token: 0x04000CB6 RID: 3254
	public GameObject container;

	// Token: 0x04000CB7 RID: 3255
	public Action skinChangedEvent;

	// Token: 0x06000FEC RID: 4076 RVA: 0x0006BE08 File Offset: 0x0006A008
	internal void Refresh(ItemBlueprint blueprint)
	{
		if (this.container != null && this.container.transform.childCount > 0)
		{
			this.container.transform.RetireAllChildren(GameManager.client);
		}
		if (blueprint == null || !blueprint.targetItem.HasSkins)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		this.AddOption(blueprint.targetItem, 0, "Default", true);
		if (blueprint.targetItem.skins2 != null)
		{
			foreach (Inventory.Definition definition in blueprint.targetItem.skins2)
			{
				if (LocalPlayer.HasInventoryItem(definition.Id))
				{
					this.AddOption(blueprint.targetItem, definition.Id, definition.Name.ToUpper(), true);
				}
			}
		}
		ItemSkinDirectory.Skin[] skins2 = blueprint.targetItem.skins;
		for (int i = 0; i < skins2.Length; i++)
		{
			ItemSkin itemSkin = FileSystem.Load<ItemSkin>(skins2[i].name, false);
			if (!(itemSkin == null) && LocalPlayer.HasInventoryItem(itemSkin.id))
			{
				this.AddOption(blueprint.targetItem, itemSkin.id, itemSkin.displayName.translated, true);
			}
		}
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x0006BF4C File Offset: 0x0006A14C
	private void AddOption(ItemDefinition item, int skinid, string text, bool canUse)
	{
		GameObject gameObject = GameManager.client.CreatePrefab(this.pickerIcon.resourcePath, Vector3.zero, Quaternion.identity, true);
		gameObject.transform.SetParent(this.container.transform, false);
		IconSkin component = gameObject.GetComponent<IconSkin>();
		component.Setup(item, skinid, text, canUse);
		component.onChanged = new Action(this.OnSkinChanged);
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x0000E124 File Offset: 0x0000C324
	public virtual void OnSkinChanged()
	{
		if (SingletonComponent<SelectedBlueprint>.Instance)
		{
			SingletonComponent<SelectedBlueprint>.Instance.OnInventoryChanged();
		}
		if (this.skinChangedEvent == null)
		{
			return;
		}
		this.skinChangedEvent.Invoke();
	}
}
