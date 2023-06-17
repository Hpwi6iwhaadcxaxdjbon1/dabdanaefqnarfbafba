using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x02000037 RID: 55
public class KeyLock : BaseLock
{
	// Token: 0x04000216 RID: 534
	private Option __menuOption_Menu_CreateKey;

	// Token: 0x04000217 RID: 535
	private Option __menuOption_Menu_Lock;

	// Token: 0x04000218 RID: 536
	private Option __menuOption_Menu_Unlock;

	// Token: 0x04000219 RID: 537
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition keyItemType;

	// Token: 0x0400021A RID: 538
	private int keyCode;

	// Token: 0x06000491 RID: 1169 RVA: 0x0003C054 File Offset: 0x0003A254
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("KeyLock.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_CreateKey", 0.1f))
			{
				this.__menuOption_Menu_CreateKey.show = true;
				this.__menuOption_Menu_CreateKey.showDisabled = false;
				this.__menuOption_Menu_CreateKey.longUseOnly = false;
				this.__menuOption_Menu_CreateKey.order = 2;
				this.__menuOption_Menu_CreateKey.icon = "key";
				this.__menuOption_Menu_CreateKey.desc = "keylock_create_key";
				this.__menuOption_Menu_CreateKey.title = "create_key";
				if (this.__menuOption_Menu_CreateKey.function == null)
				{
					this.__menuOption_Menu_CreateKey.function = new Action<BasePlayer>(this.Menu_CreateKey);
				}
				this.Menu_CreateKey_Proxy(ref this.__menuOption_Menu_CreateKey);
				list.Add(this.__menuOption_Menu_CreateKey);
			}
			using (TimeWarning.New("Menu_Lock", 0.1f))
			{
				if (this.Menu_Lock_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Lock.show = true;
					this.__menuOption_Menu_Lock.showDisabled = false;
					this.__menuOption_Menu_Lock.longUseOnly = false;
					this.__menuOption_Menu_Lock.order = 1;
					this.__menuOption_Menu_Lock.icon = "unlock";
					this.__menuOption_Menu_Lock.desc = "keylock_lock_desc";
					this.__menuOption_Menu_Lock.title = "lock_with_key";
					if (this.__menuOption_Menu_Lock.function == null)
					{
						this.__menuOption_Menu_Lock.function = new Action<BasePlayer>(this.Menu_Lock);
					}
					list.Add(this.__menuOption_Menu_Lock);
				}
			}
			using (TimeWarning.New("Menu_Unlock", 0.1f))
			{
				if (this.Menu_Unlock_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Unlock.show = true;
					this.__menuOption_Menu_Unlock.showDisabled = false;
					this.__menuOption_Menu_Unlock.longUseOnly = false;
					this.__menuOption_Menu_Unlock.order = 1;
					this.__menuOption_Menu_Unlock.icon = "unlock";
					this.__menuOption_Menu_Unlock.desc = "keylock_unlock_desc";
					this.__menuOption_Menu_Unlock.title = "unlock_with_key";
					if (this.__menuOption_Menu_Unlock.function == null)
					{
						this.__menuOption_Menu_Unlock.function = new Action<BasePlayer>(this.Menu_Unlock);
					}
					list.Add(this.__menuOption_Menu_Unlock);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x06000492 RID: 1170 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool HasMenuOptions
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x0003C318 File Offset: 0x0003A518
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("KeyLock.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0003C35C File Offset: 0x0003A55C
	public override bool HasLockPermission(BasePlayer player)
	{
		if (player.IsDead())
		{
			return false;
		}
		if (player.userID == base.OwnerID)
		{
			return true;
		}
		foreach (Item key in player.inventory.FindItemIDs(this.keyItemType.itemid))
		{
			if (this.CanKeyUnlockUs(key))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x0000612C File Offset: 0x0000432C
	private bool CanKeyUnlockUs(Item key)
	{
		return key.instanceData != null && key.instanceData.dataInt == this.keyCode;
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x0000614E File Offset: 0x0000434E
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.keyLock != null)
		{
			this.keyCode = info.msg.keyLock.code;
		}
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x0000617A File Offset: 0x0000437A
	[BaseEntity.Menu.Icon("unlock")]
	[BaseEntity.Menu("unlock_with_key", "Unlock", Order = 1)]
	[BaseEntity.Menu.Description("keylock_unlock_desc", "Unlock this door")]
	[BaseEntity.Menu.ShowIf("Menu_Unlock_ShowIf")]
	public void Menu_Unlock(BasePlayer player)
	{
		base.ServerRPC("RPC_Unlock");
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x00006187 File Offset: 0x00004387
	public bool Menu_Unlock_ShowIf(BasePlayer player)
	{
		return this.HasLockPermission(player) && base.IsLocked();
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x0000619A File Offset: 0x0000439A
	[BaseEntity.Menu.Description("keylock_lock_desc", "Lock this door")]
	[BaseEntity.Menu("lock_with_key", "Lock", Order = 1)]
	[BaseEntity.Menu.Icon("unlock")]
	[BaseEntity.Menu.ShowIf("Menu_Lock_ShowIf")]
	public void Menu_Lock(BasePlayer player)
	{
		base.ServerRPC("RPC_Lock");
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x000061A7 File Offset: 0x000043A7
	public bool Menu_Lock_ShowIf(BasePlayer player)
	{
		return this.HasLockPermission(player) && !base.IsLocked();
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x000061BD File Offset: 0x000043BD
	[BaseEntity.Menu.Icon("key")]
	[BaseEntity.Menu.Description("keylock_create_key", "Craft a key for this lock to allow friends to enter")]
	[BaseEntity.Menu("create_key", "Create a Key", ProxyFunction = "Menu_CreateKey_Proxy", Order = 2)]
	public void Menu_CreateKey(BasePlayer player)
	{
		base.ServerRPC("RPC_CreateKey");
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0003C3E4 File Offset: 0x0003A5E4
	public void Menu_CreateKey_Proxy(ref Option option)
	{
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(ItemManager.FindItemDefinition(this.keyItemType.itemid));
		option.show = (!base.IsLocked() || this.HasLockPermission(LocalPlayer.Entity));
		option.showDisabled = !LocalPlayer.HasItems(itemBlueprint.ingredients, 1);
		option.requirements = LocalPlayer.BuildItemRequiredString(itemBlueprint.ingredients);
	}
}
