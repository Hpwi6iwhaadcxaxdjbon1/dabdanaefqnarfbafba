using System;
using System.Collections.Generic;
using ConVar;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class CodeLock : BaseLock
{
	// Token: 0x040001A3 RID: 419
	private Option __menuOption_Menu_ChangeGuestCode;

	// Token: 0x040001A4 RID: 420
	private Option __menuOption_Menu_ChangeLockCode;

	// Token: 0x040001A5 RID: 421
	private Option __menuOption_Menu_Lock;

	// Token: 0x040001A6 RID: 422
	private Option __menuOption_Menu_Unlock;

	// Token: 0x040001A7 RID: 423
	public GameObjectRef keyEnterDialog;

	// Token: 0x040001A8 RID: 424
	public GameObjectRef effectUnlocked;

	// Token: 0x040001A9 RID: 425
	public GameObjectRef effectLocked;

	// Token: 0x040001AA RID: 426
	public GameObjectRef effectDenied;

	// Token: 0x040001AB RID: 427
	public GameObjectRef effectCodeChanged;

	// Token: 0x040001AC RID: 428
	public GameObjectRef effectShock;

	// Token: 0x040001AD RID: 429
	private bool hasCode;

	// Token: 0x060003E4 RID: 996 RVA: 0x00039684 File Offset: 0x00037884
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("CodeLock.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_ChangeGuestCode", 0.1f))
			{
				if (this.Menu_ChangeGuestCode_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_ChangeGuestCode.show = true;
					this.__menuOption_Menu_ChangeGuestCode.showDisabled = false;
					this.__menuOption_Menu_ChangeGuestCode.longUseOnly = false;
					this.__menuOption_Menu_ChangeGuestCode.order = 2;
					this.__menuOption_Menu_ChangeGuestCode.icon = "friends_servers";
					this.__menuOption_Menu_ChangeGuestCode.desc = "codelock_change_guestcode";
					this.__menuOption_Menu_ChangeGuestCode.title = "change_lock_guest_code";
					if (this.__menuOption_Menu_ChangeGuestCode.function == null)
					{
						this.__menuOption_Menu_ChangeGuestCode.function = new Action<BasePlayer>(this.Menu_ChangeGuestCode);
					}
					list.Add(this.__menuOption_Menu_ChangeGuestCode);
				}
			}
			using (TimeWarning.New("Menu_ChangeLockCode", 0.1f))
			{
				if (this.Menu_ChangeLockCode_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_ChangeLockCode.show = true;
					this.__menuOption_Menu_ChangeLockCode.showDisabled = false;
					this.__menuOption_Menu_ChangeLockCode.longUseOnly = false;
					this.__menuOption_Menu_ChangeLockCode.order = 1;
					this.__menuOption_Menu_ChangeLockCode.icon = "change_code";
					this.__menuOption_Menu_ChangeLockCode.desc = "codelock_change";
					this.__menuOption_Menu_ChangeLockCode.title = "change_lock_code";
					if (this.__menuOption_Menu_ChangeLockCode.function == null)
					{
						this.__menuOption_Menu_ChangeLockCode.function = new Action<BasePlayer>(this.Menu_ChangeLockCode);
					}
					list.Add(this.__menuOption_Menu_ChangeLockCode);
				}
			}
			using (TimeWarning.New("Menu_Lock", 0.1f))
			{
				if (this.Menu_Lock_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Lock.show = true;
					this.__menuOption_Menu_Lock.showDisabled = false;
					this.__menuOption_Menu_Lock.longUseOnly = false;
					this.__menuOption_Menu_Lock.order = 1;
					this.__menuOption_Menu_Lock.icon = "lock";
					this.__menuOption_Menu_Lock.desc = "codelock_lock_desc";
					this.__menuOption_Menu_Lock.title = "lock";
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
					this.__menuOption_Menu_Unlock.desc = "codelock_unlock_desc";
					this.__menuOption_Menu_Unlock.title = "unlock_with_code";
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

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x060003E5 RID: 997 RVA: 0x00039A20 File Offset: 0x00037C20
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_ChangeGuestCode_ShowIf(LocalPlayer.Entity) || this.Menu_ChangeLockCode_ShowIf(LocalPlayer.Entity) || this.Menu_Lock_ShowIf(LocalPlayer.Entity) || this.Menu_Unlock_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x00039A70 File Offset: 0x00037C70
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CodeLock.OnRpcMessage", 0.1f))
		{
			if (rpc == 1519812707U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: EnterUnlockCode ");
				}
				using (TimeWarning.New("EnterUnlockCode", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.EnterUnlockCode(rpc2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in EnterUnlockCode", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x00005883 File Offset: 0x00003A83
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.codeLock != null)
		{
			this.hasCode = info.msg.codeLock.hasCode;
		}
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x000058AF File Offset: 0x00003AAF
	[BaseEntity.Menu("unlock_with_code", "Unlock with Code", Order = 1)]
	[BaseEntity.Menu.ShowIf("Menu_Unlock_ShowIf")]
	[BaseEntity.Menu.Icon("unlock")]
	[BaseEntity.Menu.Description("codelock_unlock_desc", "This lock has a pass code which you must enter it to unlock.")]
	public void Menu_Unlock(BasePlayer player)
	{
		base.ServerRPC("TryUnlock");
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x000058BC File Offset: 0x00003ABC
	public bool Menu_Unlock_ShowIf(BasePlayer player)
	{
		return base.IsLocked();
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x000058C4 File Offset: 0x00003AC4
	[BaseEntity.Menu.Icon("lock")]
	[BaseEntity.Menu("lock", "Lock", Order = 1)]
	[BaseEntity.Menu.Description("codelock_lock_desc", "Lock with the existing pass code")]
	[BaseEntity.Menu.ShowIf("Menu_Lock_ShowIf")]
	public void Menu_Lock(BasePlayer player)
	{
		base.ServerRPC("TryLock");
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x000058D1 File Offset: 0x00003AD1
	public bool Menu_Lock_ShowIf(BasePlayer player)
	{
		return this.hasCode && !base.IsLocked();
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x00039B8C File Offset: 0x00037D8C
	[BaseEntity.Menu.ShowIf("Menu_ChangeLockCode_ShowIf")]
	[BaseEntity.Menu.Icon("change_code")]
	[BaseEntity.Menu.Description("codelock_change", "Change the code to something else")]
	[BaseEntity.Menu("change_lock_code", "Change Lock Code", Order = 1)]
	public void Menu_ChangeLockCode(BasePlayer player)
	{
		KeyCodeEntry component = GameManager.client.CreatePrefab(this.keyEnterDialog.resourcePath, true).GetComponent<KeyCodeEntry>();
		component.SetUsingGuestCode(false);
		component.onCodeEntered = (Action<string>)Delegate.Combine(component.onCodeEntered, delegate(string str)
		{
			base.ServerRPC<string, bool>("RPC_ChangeCode", str, false);
		});
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x0000494A File Offset: 0x00002B4A
	public bool Menu_ChangeLockCode_ShowIf(BasePlayer player)
	{
		return !base.IsLocked();
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x00039BDC File Offset: 0x00037DDC
	[BaseEntity.Menu.Icon("friends_servers")]
	[BaseEntity.Menu.ShowIf("Menu_ChangeGuestCode_ShowIf")]
	[BaseEntity.Menu.Description("codelock_change_guestcode", "Change the Guest Code")]
	[BaseEntity.Menu("change_lock_guest_code", "Change Guest Code", Order = 2)]
	public void Menu_ChangeGuestCode(BasePlayer player)
	{
		KeyCodeEntry component = GameManager.client.CreatePrefab(this.keyEnterDialog.resourcePath, true).GetComponent<KeyCodeEntry>();
		component.SetUsingGuestCode(true);
		component.onCodeEntered = (Action<string>)Delegate.Combine(component.onCodeEntered, delegate(string str)
		{
			base.ServerRPC<string, bool>("RPC_ChangeCode", str, true);
		});
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x000058E6 File Offset: 0x00003AE6
	public bool Menu_ChangeGuestCode_ShowIf(BasePlayer player)
	{
		return !base.IsLocked() && this.hasCode;
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x000058F8 File Offset: 0x00003AF8
	[BaseEntity.RPC_Client]
	private void EnterUnlockCode(BaseEntity.RPCMessage rpc)
	{
		KeyCodeEntry component = GameManager.client.CreatePrefab(this.keyEnterDialog.resourcePath, true).GetComponent<KeyCodeEntry>();
		component.onCodeEntered = (Action<string>)Delegate.Combine(component.onCodeEntered, delegate(string str)
		{
			base.ServerRPC<string>("UnlockWithCode", str);
		});
	}
}
