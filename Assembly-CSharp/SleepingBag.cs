using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class SleepingBag : DecayEntity
{
	// Token: 0x040002C9 RID: 713
	private Option __menuOption_Menu_AssignToFriend;

	// Token: 0x040002CA RID: 714
	private Option __menuOption_Menu_CloseDoor;

	// Token: 0x040002CB RID: 715
	private Option __menuOption_Menu_MakeBed;

	// Token: 0x040002CC RID: 716
	private Option __menuOption_Menu_MakePrivate;

	// Token: 0x040002CD RID: 717
	private Option __menuOption_Menu_MakePublic;

	// Token: 0x040002CE RID: 718
	[NonSerialized]
	public ulong deployerUserID;

	// Token: 0x040002CF RID: 719
	public GameObject renameDialog;

	// Token: 0x040002D0 RID: 720
	public GameObject assignDialog;

	// Token: 0x040002D1 RID: 721
	public float secondsBetweenReuses = 300f;

	// Token: 0x040002D2 RID: 722
	public string niceName = "Unnamed Bag";

	// Token: 0x040002D3 RID: 723
	public Vector3 spawnOffset = Vector3.zero;

	// Token: 0x040002D4 RID: 724
	public bool canBePublic;

	// Token: 0x06000598 RID: 1432 RVA: 0x00040228 File Offset: 0x0003E428
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("SleepingBag.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_AssignToFriend", 0.1f))
			{
				if (this.Menu_AssignToFriend_Test(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_AssignToFriend.show = true;
					this.__menuOption_Menu_AssignToFriend.showDisabled = false;
					this.__menuOption_Menu_AssignToFriend.longUseOnly = false;
					this.__menuOption_Menu_AssignToFriend.order = 50;
					this.__menuOption_Menu_AssignToFriend.icon = "friends_servers";
					this.__menuOption_Menu_AssignToFriend.desc = "assign_sleepingbag_desc";
					this.__menuOption_Menu_AssignToFriend.title = "assign_sleepingbag";
					if (this.__menuOption_Menu_AssignToFriend.function == null)
					{
						this.__menuOption_Menu_AssignToFriend.function = new Action<BasePlayer>(this.Menu_AssignToFriend);
					}
					list.Add(this.__menuOption_Menu_AssignToFriend);
				}
			}
			using (TimeWarning.New("Menu_CloseDoor", 0.1f))
			{
				this.__menuOption_Menu_CloseDoor.show = true;
				this.__menuOption_Menu_CloseDoor.showDisabled = false;
				this.__menuOption_Menu_CloseDoor.longUseOnly = false;
				this.__menuOption_Menu_CloseDoor.order = 0;
				this.__menuOption_Menu_CloseDoor.icon = "sleepingbag";
				this.__menuOption_Menu_CloseDoor.desc = "rename_sleepingbag_desc";
				this.__menuOption_Menu_CloseDoor.title = "rename_sleepingbag";
				if (this.__menuOption_Menu_CloseDoor.function == null)
				{
					this.__menuOption_Menu_CloseDoor.function = new Action<BasePlayer>(this.Menu_CloseDoor);
				}
				list.Add(this.__menuOption_Menu_CloseDoor);
			}
			using (TimeWarning.New("Menu_MakeBed", 0.1f))
			{
				if (this.Menu_MakeBed_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_MakeBed.show = true;
					this.__menuOption_Menu_MakeBed.showDisabled = false;
					this.__menuOption_Menu_MakeBed.longUseOnly = false;
					this.__menuOption_Menu_MakeBed.order = -10;
					this.__menuOption_Menu_MakeBed.icon = "sleeping";
					this.__menuOption_Menu_MakeBed.desc = "bag_makebed_desc";
					this.__menuOption_Menu_MakeBed.title = "bag_makebed";
					if (this.__menuOption_Menu_MakeBed.function == null)
					{
						this.__menuOption_Menu_MakeBed.function = new Action<BasePlayer>(this.Menu_MakeBed);
					}
					list.Add(this.__menuOption_Menu_MakeBed);
				}
			}
			using (TimeWarning.New("Menu_MakePrivate", 0.1f))
			{
				if (this.Menu_MakePrivate_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_MakePrivate.show = true;
					this.__menuOption_Menu_MakePrivate.showDisabled = false;
					this.__menuOption_Menu_MakePrivate.longUseOnly = false;
					this.__menuOption_Menu_MakePrivate.order = 100;
					this.__menuOption_Menu_MakePrivate.icon = "close";
					this.__menuOption_Menu_MakePrivate.desc = "bag_makeprivate_desc";
					this.__menuOption_Menu_MakePrivate.title = "bag_makeprivate";
					if (this.__menuOption_Menu_MakePrivate.function == null)
					{
						this.__menuOption_Menu_MakePrivate.function = new Action<BasePlayer>(this.Menu_MakePrivate);
					}
					list.Add(this.__menuOption_Menu_MakePrivate);
				}
			}
			using (TimeWarning.New("Menu_MakePublic", 0.1f))
			{
				if (this.Menu_MakePublic_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_MakePublic.show = true;
					this.__menuOption_Menu_MakePublic.showDisabled = false;
					this.__menuOption_Menu_MakePublic.longUseOnly = false;
					this.__menuOption_Menu_MakePublic.order = 100;
					this.__menuOption_Menu_MakePublic.icon = "upgrade";
					this.__menuOption_Menu_MakePublic.desc = "bag_makepublic_desc";
					this.__menuOption_Menu_MakePublic.title = "bag_makepublic";
					if (this.__menuOption_Menu_MakePublic.function == null)
					{
						this.__menuOption_Menu_MakePublic.function = new Action<BasePlayer>(this.Menu_MakePublic);
					}
					list.Add(this.__menuOption_Menu_MakePublic);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x06000599 RID: 1433 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool HasMenuOptions
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x0004068C File Offset: 0x0003E88C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SleepingBag.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x00005EBB File Offset: 0x000040BB
	public bool IsPublic()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved3);
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x000406D0 File Offset: 0x0003E8D0
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sleepingBag != null)
		{
			this.niceName = info.msg.sleepingBag.name;
			this.deployerUserID = info.msg.sleepingBag.deployerID;
		}
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x00006F3B File Offset: 0x0000513B
	public void ClientRename(string newName)
	{
		base.ServerRPC<string>("Rename", newName);
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x00006F49 File Offset: 0x00005149
	[BaseEntity.Menu.Description("rename_sleepingbag_desc", "Change the name of this sleeping bag so it's easily distinguishable")]
	[BaseEntity.Menu("rename_sleepingbag", "Rename Sleeping Bag")]
	[BaseEntity.Menu.Icon("sleepingbag")]
	public void Menu_CloseDoor(BasePlayer player)
	{
		RenameSleepingBag component = Object.Instantiate<GameObject>(this.renameDialog).GetComponent<RenameSleepingBag>();
		component.bag = this;
		component.OpenDialog();
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x00006F67 File Offset: 0x00005167
	[BaseEntity.Menu("assign_sleepingbag", "Give To Friend", Order = 50)]
	[BaseEntity.Menu.Description("assign_sleepingbag_desc", "Assign this sleeping bag to a friend. You won't own it anymore.")]
	[BaseEntity.Menu.ShowIf("Menu_AssignToFriend_Test")]
	[BaseEntity.Menu.Icon("friends_servers")]
	public void Menu_AssignToFriend(BasePlayer player)
	{
		PickAFriend component = Object.Instantiate<GameObject>(this.assignDialog).GetComponent<PickAFriend>();
		component.onSelected = delegate(ulong id)
		{
			base.ServerRPC<ulong>("AssignToFriend", id);
		};
		component.OpenDialog();
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x00006F90 File Offset: 0x00005190
	public bool Menu_AssignToFriend_Test(BasePlayer player)
	{
		return player.userID == this.deployerUserID;
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x00006FA0 File Offset: 0x000051A0
	[BaseEntity.Menu.ShowIf("Menu_MakePublic_ShowIf")]
	[BaseEntity.Menu("bag_makepublic", "Set Public", Order = 100)]
	[BaseEntity.Menu.Icon("upgrade")]
	[BaseEntity.Menu.Description("bag_makepublic_desc", "Allow anyone who approaches this bed to sleep in it and take it over.")]
	public void Menu_MakePublic(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_MakePublic", true);
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x00006FAE File Offset: 0x000051AE
	public bool Menu_MakePublic_ShowIf(BasePlayer player)
	{
		return this.canBePublic && !this.IsPublic() && player.userID == this.deployerUserID;
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x00006FD0 File Offset: 0x000051D0
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu.Description("bag_makeprivate_desc", "No one else can take over this bed or spawn in it.")]
	[BaseEntity.Menu("bag_makeprivate", "Set Private", Order = 100)]
	[BaseEntity.Menu.ShowIf("Menu_MakePrivate_ShowIf")]
	public void Menu_MakePrivate(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_MakePublic", false);
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x00006FDE File Offset: 0x000051DE
	public bool Menu_MakePrivate_ShowIf(BasePlayer player)
	{
		return this.canBePublic && this.IsPublic() && (player.userID == this.deployerUserID || player.CanBuild());
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x00007008 File Offset: 0x00005208
	[BaseEntity.Menu("bag_makebed", "Make Bed", Order = -10)]
	[BaseEntity.Menu.Icon("sleeping")]
	[BaseEntity.Menu.ShowIf("Menu_MakeBed_ShowIf")]
	[BaseEntity.Menu.Description("bag_makebed_desc", "Claim this bed for yourself")]
	public void Menu_MakeBed(BasePlayer player)
	{
		base.ServerRPC("RPC_MakeBed");
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x00007015 File Offset: 0x00005215
	public bool Menu_MakeBed_ShowIf(BasePlayer player)
	{
		return this.canBePublic && this.IsPublic() && player.userID != this.deployerUserID;
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x00040720 File Offset: 0x0003E920
	public override Info GetMenuInformation(GameObject primaryObject, BasePlayer player)
	{
		Info menuInformation = base.GetMenuInformation(primaryObject, player);
		menuInformation.entityName = this.niceName;
		return menuInformation;
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x0000703A File Offset: 0x0000523A
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && player.userID == this.deployerUserID;
	}
}
