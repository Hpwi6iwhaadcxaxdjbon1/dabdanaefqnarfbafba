using System;
using System.Collections.Generic;
using ConVar;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000033 RID: 51
public class HackableLockedCrate : LootContainer
{
	// Token: 0x040001DA RID: 474
	private Option __menuOption_Menu_Hack;

	// Token: 0x040001DB RID: 475
	public const BaseEntity.Flags Flag_Hacking = BaseEntity.Flags.Reserved1;

	// Token: 0x040001DC RID: 476
	public const BaseEntity.Flags Flag_FullyHacked = BaseEntity.Flags.Reserved2;

	// Token: 0x040001DD RID: 477
	public Text timerText;

	// Token: 0x040001DE RID: 478
	[ServerVar(Help = "How many seconds for the crate to unlock")]
	public static float requiredHackSeconds = 900f;

	// Token: 0x040001DF RID: 479
	[ServerVar(Help = "How many seconds until the crate is destroyed without any hack attempts")]
	public static float decaySeconds = 7200f;

	// Token: 0x040001E0 RID: 480
	public SoundPlayer hackProgressBeep;

	// Token: 0x040001E1 RID: 481
	private float hackSeconds;

	// Token: 0x040001E2 RID: 482
	public GameObjectRef shockEffect;

	// Token: 0x040001E3 RID: 483
	public GameObjectRef mapMarkerEntityPrefab;

	// Token: 0x040001E4 RID: 484
	public GameObjectRef landEffect;

	// Token: 0x040001E5 RID: 485
	private int beepSeconds;

	// Token: 0x06000458 RID: 1112 RVA: 0x0003B298 File Offset: 0x00039498
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("HackableLockedCrate.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Hack", 0.1f))
			{
				if (this.Menu_Hack_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Hack.show = true;
					this.__menuOption_Menu_Hack.showDisabled = false;
					this.__menuOption_Menu_Hack.longUseOnly = false;
					this.__menuOption_Menu_Hack.order = -100;
					this.__menuOption_Menu_Hack.icon = "upgrade";
					this.__menuOption_Menu_Hack.desc = "hack_desc";
					this.__menuOption_Menu_Hack.title = "hack";
					if (this.__menuOption_Menu_Hack.function == null)
					{
						this.__menuOption_Menu_Hack.function = new Action<BasePlayer>(this.Menu_Hack);
					}
					list.Add(this.__menuOption_Menu_Hack);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x06000459 RID: 1113 RVA: 0x00005DAF File Offset: 0x00003FAF
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Hack_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x0003B3A0 File Offset: 0x000395A0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HackableLockedCrate.OnRpcMessage", 0.1f))
		{
			if (rpc == 2961367905U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: UpdateHackProgress ");
				}
				using (TimeWarning.New("UpdateHackProgress", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							int totalSecondsComplete = msg.read.Int32();
							int totalSecondsRequired = msg.read.Int32();
							this.UpdateHackProgress(totalSecondsComplete, totalSecondsRequired);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in UpdateHackProgress", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00004723 File Offset: 0x00002923
	public bool IsBeingHacked()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x00005DC6 File Offset: 0x00003FC6
	public bool IsFullyHacked()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x00005DD3 File Offset: 0x00003FD3
	public override void DestroyShared()
	{
		base.DestroyShared();
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x00005DDB File Offset: 0x00003FDB
	[BaseEntity.RPC_Client]
	public void UpdateHackProgress(int totalSecondsComplete, int totalSecondsRequired)
	{
		this.beepSeconds++;
		if (this.beepSeconds > 3)
		{
			this.hackProgressBeep.PlayOneshot();
			this.beepSeconds = 0;
		}
		this.hackSeconds = (float)totalSecondsComplete;
		HackableLockedCrate.requiredHackSeconds = (float)totalSecondsRequired;
		this.HackScreenUpdate();
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x00005E1B File Offset: 0x0000401B
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0003B4A8 File Offset: 0x000396A8
	public void HackScreenUpdate()
	{
		float num = Mathf.Clamp((float)Mathf.CeilToInt(HackableLockedCrate.requiredHackSeconds - this.hackSeconds), 0f, float.PositiveInfinity);
		int num2 = Mathf.FloorToInt(num / 60f);
		int num3 = Mathf.FloorToInt(num - (float)(num2 * 60));
		this.timerText.text = string.Concat(new object[]
		{
			num2,
			":",
			(num3 < 10) ? "0" : "",
			num3
		});
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x00005E23 File Offset: 0x00004023
	public override bool ShouldShowLootMenus()
	{
		return base.ShouldShowLootMenus() && this.IsFullyHacked() && LocalPlayer.Entity != null;
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00005E42 File Offset: 0x00004042
	[BaseEntity.Menu.ShowIf("Menu_Hack_ShowIf")]
	[BaseEntity.Menu.Icon("upgrade")]
	[BaseEntity.Menu.Description("hack_desc", "Begin attempting to break the code")]
	[BaseEntity.Menu("hack", "Begin Hacking", Order = -100)]
	public void Menu_Hack(BasePlayer player)
	{
		base.ServerRPC("RPC_Hack");
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00005E4F File Offset: 0x0000404F
	public bool Menu_Hack_ShowIf(BasePlayer player)
	{
		return !this.IsBeingHacked() && player.lookingAtCollider != null && player.lookingAtCollider.CompareTag("Usable Secondary");
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x00004F34 File Offset: 0x00003134
	public override float GetExtrapolationTime()
	{
		return Mathf.Clamp(Lerp.extrapolation, 0f, 0.1f);
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00005E79 File Offset: 0x00004079
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		if (this.IsFullyHacked())
		{
			this.hackSeconds = 10f;
			HackableLockedCrate.requiredHackSeconds = 10f;
			this.HackScreenUpdate();
		}
	}
}
