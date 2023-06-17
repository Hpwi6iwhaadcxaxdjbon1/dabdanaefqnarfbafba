using System;
using System.Collections.Generic;
using ConVar;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class SpookySpeaker : BaseCombatEntity
{
	// Token: 0x040002E6 RID: 742
	private Option __menuOption_Menu_SoundOff;

	// Token: 0x040002E7 RID: 743
	private Option __menuOption_Menu_SoundOn;

	// Token: 0x040002E8 RID: 744
	public SoundPlayer soundPlayer;

	// Token: 0x040002E9 RID: 745
	public float soundSpacing = 12f;

	// Token: 0x040002EA RID: 746
	public float soundSpacingRand = 5f;

	// Token: 0x060005BD RID: 1469 RVA: 0x00040D40 File Offset: 0x0003EF40
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("SpookySpeaker.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_SoundOff", 0.1f))
			{
				if (this.Menu_SoundOff_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_SoundOff.show = true;
					this.__menuOption_Menu_SoundOff.showDisabled = false;
					this.__menuOption_Menu_SoundOff.longUseOnly = false;
					this.__menuOption_Menu_SoundOff.order = -100;
					this.__menuOption_Menu_SoundOff.icon = "close";
					this.__menuOption_Menu_SoundOff.desc = "sound_off_desc";
					this.__menuOption_Menu_SoundOff.title = "sound_off";
					if (this.__menuOption_Menu_SoundOff.function == null)
					{
						this.__menuOption_Menu_SoundOff.function = new Action<BasePlayer>(this.Menu_SoundOff);
					}
					list.Add(this.__menuOption_Menu_SoundOff);
				}
			}
			using (TimeWarning.New("Menu_SoundOn", 0.1f))
			{
				if (this.Menu_SoundOn_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_SoundOn.show = true;
					this.__menuOption_Menu_SoundOn.showDisabled = false;
					this.__menuOption_Menu_SoundOn.longUseOnly = false;
					this.__menuOption_Menu_SoundOn.order = -100;
					this.__menuOption_Menu_SoundOn.icon = "power";
					this.__menuOption_Menu_SoundOn.desc = "sound_on_desc";
					this.__menuOption_Menu_SoundOn.title = "sound_on";
					if (this.__menuOption_Menu_SoundOn.function == null)
					{
						this.__menuOption_Menu_SoundOn.function = new Action<BasePlayer>(this.Menu_SoundOn);
					}
					list.Add(this.__menuOption_Menu_SoundOn);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x060005BE RID: 1470 RVA: 0x00007188 File Offset: 0x00005388
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_SoundOff_ShowIf(LocalPlayer.Entity) || this.Menu_SoundOn_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x00040F34 File Offset: 0x0003F134
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SpookySpeaker.OnRpcMessage", 0.1f))
		{
			if (rpc == 2884648246U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: PlaySpookySound ");
				}
				using (TimeWarning.New("PlaySpookySound", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							this.PlaySpookySound();
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in PlaySpookySound", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x000071AE File Offset: 0x000053AE
	[BaseEntity.RPC_Client]
	public void PlaySpookySound()
	{
		SoundManager.PlayOneshot(this.soundPlayer.soundDefinition, base.gameObject, false, this.soundPlayer.transform.position);
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x00005854 File Offset: 0x00003A54
	[BaseEntity.Menu.Description("sound_on_desc", "Begin playing spooky sounds periodically")]
	[BaseEntity.Menu.ShowIf("Menu_SoundOn_ShowIf")]
	[BaseEntity.Menu.Icon("power")]
	[BaseEntity.Menu("sound_on", "Turn On", Order = -100)]
	public void Menu_SoundOn(BasePlayer player)
	{
		base.ServerRPC<bool>("SetWantsOn", true);
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00002E2B File Offset: 0x0000102B
	public bool Menu_SoundOn_ShowIf(BasePlayer player)
	{
		return !base.IsOn() && player.CanBuild();
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x0000586D File Offset: 0x00003A6D
	[BaseEntity.Menu.Description("sound_off_desc", "Stop playing spooky sounds")]
	[BaseEntity.Menu("sound_off", "Turn Off", Order = -100)]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu.ShowIf("Menu_SoundOff_ShowIf")]
	public void Menu_SoundOff(BasePlayer player)
	{
		base.ServerRPC<bool>("SetWantsOn", false);
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00002E4B File Offset: 0x0000104B
	public bool Menu_SoundOff_ShowIf(BasePlayer player)
	{
		return base.IsOn() && player.CanBuild();
	}
}
