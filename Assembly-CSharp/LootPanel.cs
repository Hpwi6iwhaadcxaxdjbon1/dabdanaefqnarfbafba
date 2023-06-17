using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000662 RID: 1634
public class LootPanel : MonoBehaviour
{
	// Token: 0x04002070 RID: 8304
	public Text Title;

	// Token: 0x1700025F RID: 607
	// (get) Token: 0x06002465 RID: 9317 RVA: 0x000C0964 File Offset: 0x000BEB64
	public ItemContainer Container_00
	{
		get
		{
			if (LocalPlayer.Entity && LocalPlayer.Entity.inventory.loot.IsLooting() && LocalPlayer.Entity.inventory.loot.containers.Count > 0)
			{
				return LocalPlayer.Entity.inventory.loot.containers[0];
			}
			return null;
		}
	}

	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06002466 RID: 9318 RVA: 0x000C09CC File Offset: 0x000BEBCC
	public ItemContainer Container_01
	{
		get
		{
			if (LocalPlayer.Entity && LocalPlayer.Entity.inventory.loot.IsLooting() && LocalPlayer.Entity.inventory.loot.containers.Count > 1)
			{
				return LocalPlayer.Entity.inventory.loot.containers[1];
			}
			return null;
		}
	}

	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06002467 RID: 9319 RVA: 0x000C0A34 File Offset: 0x000BEC34
	public ItemContainer Container_02
	{
		get
		{
			if (LocalPlayer.Entity && LocalPlayer.Entity.inventory.loot.IsLooting() && LocalPlayer.Entity.inventory.loot.containers.Count > 2)
			{
				return LocalPlayer.Entity.inventory.loot.containers[2];
			}
			return null;
		}
	}

	// Token: 0x06002468 RID: 9320 RVA: 0x0001CB4D File Offset: 0x0001AD4D
	public T GetContainerEntity<T>() where T : BaseEntity
	{
		return this.GetContainerEntity() as T;
	}

	// Token: 0x06002469 RID: 9321 RVA: 0x0001CB5F File Offset: 0x0001AD5F
	public BaseEntity GetContainerEntity()
	{
		if (LocalPlayer.Entity && LocalPlayer.Entity.inventory.loot.IsLooting())
		{
			return LocalPlayer.Entity.inventory.loot.GetClientEntity();
		}
		return null;
	}

	// Token: 0x0600246A RID: 9322 RVA: 0x0001CB99 File Offset: 0x0001AD99
	public virtual void Update()
	{
		if (this.Title)
		{
			this.Title.text = this.lootingEntityName;
		}
	}

	// Token: 0x17000262 RID: 610
	// (get) Token: 0x0600246B RID: 9323 RVA: 0x000C0A9C File Offset: 0x000BEC9C
	public string lootingEntityName
	{
		get
		{
			BaseEntity containerEntity = this.GetContainerEntity();
			if (!containerEntity.IsValid())
			{
				return "Loot";
			}
			BasePlayer basePlayer = containerEntity as BasePlayer;
			if (basePlayer != null)
			{
				return basePlayer.displayName;
			}
			LootableCorpse lootableCorpse = containerEntity as LootableCorpse;
			if (lootableCorpse != null)
			{
				return lootableCorpse.playerName;
			}
			DroppedItemContainer droppedItemContainer = containerEntity as DroppedItemContainer;
			if (droppedItemContainer != null)
			{
				return droppedItemContainer.playerName;
			}
			PrefabInformation prefabInformation = PrefabAttribute.client.Find<PrefabInformation>(containerEntity.prefabID);
			if (prefabInformation)
			{
				return prefabInformation.title.translated;
			}
			containerEntity as LootContainer != null;
			return "Loot";
		}
	}
}
