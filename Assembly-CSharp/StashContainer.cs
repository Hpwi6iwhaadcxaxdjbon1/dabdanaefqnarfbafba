using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class StashContainer : StorageContainer
{
	// Token: 0x040002EB RID: 747
	private Option __menuOption_Menu_HideStash;

	// Token: 0x040002EC RID: 748
	public Transform visuals;

	// Token: 0x040002ED RID: 749
	public float burriedOffset;

	// Token: 0x040002EE RID: 750
	public float raisedOffset;

	// Token: 0x040002EF RID: 751
	public GameObjectRef buryEffect;

	// Token: 0x040002F0 RID: 752
	public float uncoverRange = 3f;

	// Token: 0x060005C6 RID: 1478 RVA: 0x00041018 File Offset: 0x0003F218
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("StashContainer.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_HideStash", 0.1f))
			{
				if (this.Menu_HideStash_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_HideStash.show = true;
					this.__menuOption_Menu_HideStash.showDisabled = false;
					this.__menuOption_Menu_HideStash.longUseOnly = false;
					this.__menuOption_Menu_HideStash.order = 100;
					this.__menuOption_Menu_HideStash.icon = "drop";
					this.__menuOption_Menu_HideStash.desc = "hide_stash_Desc";
					this.__menuOption_Menu_HideStash.title = "hide_stash";
					if (this.__menuOption_Menu_HideStash.function == null)
					{
						this.__menuOption_Menu_HideStash.function = new Action<BasePlayer>(this.Menu_HideStash);
					}
					list.Add(this.__menuOption_Menu_HideStash);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x060005C7 RID: 1479 RVA: 0x000071F6 File Offset: 0x000053F6
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_HideStash_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x00041120 File Offset: 0x0003F320
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StashContainer.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x00041164 File Offset: 0x0003F364
	public bool PlayerInRange(BasePlayer ply)
	{
		if (Vector3.Distance(base.transform.position, ply.transform.position) <= this.uncoverRange)
		{
			Vector3 normalized = (base.transform.position - ply.eyes.position).normalized;
			if (Vector3.Dot(ply.eyes.BodyForward(), normalized) > 0.95f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x0000720D File Offset: 0x0000540D
	public override bool ShouldShowLootMenus()
	{
		return base.ShouldShowLootMenus() && !this.IsHidden();
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x000411D4 File Offset: 0x0003F3D4
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.visuals.transform.localPosition = new Vector3(this.visuals.transform.localPosition.x, this.raisedOffset, this.visuals.transform.localPosition.z);
		base.InvokeRepeating(new Action(this.TryUnhide), Random.Range(0f, 1f), 3f);
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x00041254 File Offset: 0x0003F454
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		bool flag = this.IsHidden();
		base.Load(info);
		if (flag != this.IsHidden())
		{
			Effect.client.Run(this.buryEffect.resourcePath, base.transform.position, base.transform.up, default(Vector3));
		}
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x00007222 File Offset: 0x00005422
	[BaseEntity.Menu("hide_stash", "Hide", Order = 100)]
	[BaseEntity.Menu.ShowIf("Menu_HideStash_ShowIf")]
	[BaseEntity.Menu.Icon("drop")]
	[BaseEntity.Menu.Description("hide_stash_Desc", "Hide the stash underground")]
	public void Menu_HideStash(BasePlayer player)
	{
		base.ServerRPC("RPC_HideStash");
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x0000722F File Offset: 0x0000542F
	public bool Menu_HideStash_ShowIf(BasePlayer player)
	{
		return this.ShouldShowLootMenus();
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x00007237 File Offset: 0x00005437
	public void TryUnhide()
	{
		if (!LocalPlayer.Entity)
		{
			return;
		}
		if (this.IsHidden() && this.PlayerInRange(LocalPlayer.Entity))
		{
			base.ServerRPC("RPC_WantsUnhide");
		}
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x000412A8 File Offset: 0x0003F4A8
	public void Update()
	{
		if (base.isClient)
		{
			float num = this.IsHidden() ? this.burriedOffset : this.raisedOffset;
			if (this.visuals.transform.localPosition.y != num)
			{
				this.visuals.transform.localPosition = Vector3.Lerp(this.visuals.transform.localPosition, new Vector3(this.visuals.transform.localPosition.x, num, this.visuals.transform.localPosition.z), Time.deltaTime);
			}
		}
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x00002D37 File Offset: 0x00000F37
	public bool IsHidden()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5);
	}

	// Token: 0x0200004F RID: 79
	public static class StashContainerFlags
	{
		// Token: 0x040002F1 RID: 753
		public const BaseEntity.Flags Hidden = BaseEntity.Flags.Reserved5;
	}
}
