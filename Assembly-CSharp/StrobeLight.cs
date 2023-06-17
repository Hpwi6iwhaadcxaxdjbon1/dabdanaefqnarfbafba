using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class StrobeLight : BaseCombatEntity
{
	// Token: 0x04000016 RID: 22
	public float frequency;

	// Token: 0x04000017 RID: 23
	public MeshRenderer lightMesh;

	// Token: 0x04000018 RID: 24
	public Light strobeLight;

	// Token: 0x04000019 RID: 25
	private float speedSlow = 10f;

	// Token: 0x0400001A RID: 26
	private float speedMed = 20f;

	// Token: 0x0400001B RID: 27
	private float speedFast = 40f;

	// Token: 0x0400001C RID: 28
	public float burnRate = 10f;

	// Token: 0x0400001D RID: 29
	public float lifeTimeSeconds = 21600f;

	// Token: 0x0400001E RID: 30
	public const BaseEntity.Flags Flag_Slow = BaseEntity.Flags.Reserved6;

	// Token: 0x0400001F RID: 31
	public const BaseEntity.Flags Flag_Med = BaseEntity.Flags.Reserved7;

	// Token: 0x04000020 RID: 32
	public const BaseEntity.Flags Flag_Fast = BaseEntity.Flags.Reserved8;

	// Token: 0x04000021 RID: 33
	[ClientVar(Saved = true)]
	public static bool forceoff;

	// Token: 0x04000022 RID: 34
	private float lastStrobeTime;

	// Token: 0x04000023 RID: 35
	private Option __menuOption_Menu_StrobeFast;

	// Token: 0x04000024 RID: 36
	private Option __menuOption_Menu_StrobeMed;

	// Token: 0x04000025 RID: 37
	private Option __menuOption_Menu_StrobeSlow;

	// Token: 0x04000026 RID: 38
	private Option __menuOption_Menu_TurnOff;

	// Token: 0x04000027 RID: 39
	private Option __menuOption_Menu_TurnOn;

	// Token: 0x06000024 RID: 36 RVA: 0x00026E90 File Offset: 0x00025090
	public float GetFrequency()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved6))
		{
			return this.speedSlow;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved7))
		{
			return this.speedMed;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved8))
		{
			return this.speedFast;
		}
		return this.speedSlow;
	}

	// Token: 0x06000025 RID: 37 RVA: 0x00026EE0 File Offset: 0x000250E0
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		if (StrobeLight.forceoff)
		{
			this.ClientSetLights(false);
			return;
		}
		if (!base.IsOn())
		{
			this.ClientSetLights(false);
			return;
		}
		float num = 1000f / this.GetFrequency() / 1000f;
		if (Time.time > this.lastStrobeTime + num)
		{
			this.Toggle();
			this.lastStrobeTime = Time.time;
		}
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00026F48 File Offset: 0x00025148
	public void ClientSetLights(bool wantsOn)
	{
		if (wantsOn == this.strobeLight.enabled)
		{
			return;
		}
		this.strobeLight.enabled = wantsOn;
		Vector3 position = this.strobeLight.transform.position;
		this.strobeLight.transform.Translate((float)(wantsOn ? 1 : -1) * 1E-05f, 0f, 0f);
		this.lightMesh.enabled = wantsOn;
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00026FB8 File Offset: 0x000251B8
	public void Toggle()
	{
		bool wantsOn = !this.strobeLight.enabled;
		this.ClientSetLights(wantsOn);
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00002E1D File Offset: 0x0000101D
	[BaseEntity.Menu.Description("turnon_strobe_desc", "Activate the strobe light")]
	[BaseEntity.Menu("strobe_on", "Turn On", Order = -90)]
	[BaseEntity.Menu.Icon("power")]
	[BaseEntity.Menu.ShowIf("Menu_StrobeOn_ShowIf")]
	public void Menu_TurnOn(BasePlayer player)
	{
		base.ServerRPC<bool>("SetStrobe", true);
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00002E2B File Offset: 0x0000102B
	public bool Menu_StrobeOn_ShowIf(BasePlayer player)
	{
		return !base.IsOn() && player.CanBuild();
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00002E3D File Offset: 0x0000103D
	[BaseEntity.Menu.Description("turnof_strobe_desc", "Turn the strobe light off")]
	[BaseEntity.Menu("strobe_off", "Turn Off", Order = -90)]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu.ShowIf("Menu_StrobeOff_ShowIf")]
	public void Menu_TurnOff(BasePlayer player)
	{
		base.ServerRPC<bool>("SetStrobe", false);
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002E4B File Offset: 0x0000104B
	public bool Menu_StrobeOff_ShowIf(BasePlayer player)
	{
		return base.IsOn() && player.CanBuild();
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002E5D File Offset: 0x0000105D
	[BaseEntity.Menu.Icon("gear")]
	[BaseEntity.Menu("strobe_slow", "10hz")]
	[BaseEntity.Menu.Description("slow_strobe_desc", "Set the strobe light to 10hz")]
	[BaseEntity.Menu.ShowIf("Menu_StrobeSlow_ShowIf")]
	public void Menu_StrobeSlow(BasePlayer player)
	{
		base.ServerRPC<int>("SetStrobeSpeed", 1);
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00002E6B File Offset: 0x0000106B
	public bool Menu_StrobeSlow_ShowIf(BasePlayer player)
	{
		return player.CanBuild();
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002E73 File Offset: 0x00001073
	[BaseEntity.Menu.Icon("gear")]
	[BaseEntity.Menu.Description("med_strobe_desc", "Set the strobe light to 20hz")]
	[BaseEntity.Menu("strobe_med", "20hz")]
	[BaseEntity.Menu.ShowIf("Menu_StrobeMed_ShowIf")]
	public void Menu_StrobeMed(BasePlayer player)
	{
		base.ServerRPC<int>("SetStrobeSpeed", 2);
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00002E6B File Offset: 0x0000106B
	public bool Menu_StrobeMed_ShowIf(BasePlayer player)
	{
		return player.CanBuild();
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00002E81 File Offset: 0x00001081
	[BaseEntity.Menu.Icon("gear")]
	[BaseEntity.Menu.ShowIf("Menu_StrobeFast_ShowIf")]
	[BaseEntity.Menu("strobe_fast", "40hz")]
	[BaseEntity.Menu.Description("fast_strobe_desc", "Set the strobe light to 40hz")]
	public void Menu_StrobeFast(BasePlayer player)
	{
		base.ServerRPC<int>("SetStrobeSpeed", 3);
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00002E6B File Offset: 0x0000106B
	public bool Menu_StrobeFast_ShowIf(BasePlayer player)
	{
		return player.CanBuild();
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00026FDC File Offset: 0x000251DC
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("StrobeLight.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_StrobeFast", 0.1f))
			{
				if (this.Menu_StrobeFast_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_StrobeFast.show = true;
					this.__menuOption_Menu_StrobeFast.showDisabled = false;
					this.__menuOption_Menu_StrobeFast.longUseOnly = false;
					this.__menuOption_Menu_StrobeFast.order = 0;
					this.__menuOption_Menu_StrobeFast.icon = "gear";
					this.__menuOption_Menu_StrobeFast.desc = "fast_strobe_desc";
					this.__menuOption_Menu_StrobeFast.title = "strobe_fast";
					if (this.__menuOption_Menu_StrobeFast.function == null)
					{
						this.__menuOption_Menu_StrobeFast.function = new Action<BasePlayer>(this.Menu_StrobeFast);
					}
					list.Add(this.__menuOption_Menu_StrobeFast);
				}
			}
			using (TimeWarning.New("Menu_StrobeMed", 0.1f))
			{
				if (this.Menu_StrobeMed_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_StrobeMed.show = true;
					this.__menuOption_Menu_StrobeMed.showDisabled = false;
					this.__menuOption_Menu_StrobeMed.longUseOnly = false;
					this.__menuOption_Menu_StrobeMed.order = 0;
					this.__menuOption_Menu_StrobeMed.icon = "gear";
					this.__menuOption_Menu_StrobeMed.desc = "med_strobe_desc";
					this.__menuOption_Menu_StrobeMed.title = "strobe_med";
					if (this.__menuOption_Menu_StrobeMed.function == null)
					{
						this.__menuOption_Menu_StrobeMed.function = new Action<BasePlayer>(this.Menu_StrobeMed);
					}
					list.Add(this.__menuOption_Menu_StrobeMed);
				}
			}
			using (TimeWarning.New("Menu_StrobeSlow", 0.1f))
			{
				if (this.Menu_StrobeSlow_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_StrobeSlow.show = true;
					this.__menuOption_Menu_StrobeSlow.showDisabled = false;
					this.__menuOption_Menu_StrobeSlow.longUseOnly = false;
					this.__menuOption_Menu_StrobeSlow.order = 0;
					this.__menuOption_Menu_StrobeSlow.icon = "gear";
					this.__menuOption_Menu_StrobeSlow.desc = "slow_strobe_desc";
					this.__menuOption_Menu_StrobeSlow.title = "strobe_slow";
					if (this.__menuOption_Menu_StrobeSlow.function == null)
					{
						this.__menuOption_Menu_StrobeSlow.function = new Action<BasePlayer>(this.Menu_StrobeSlow);
					}
					list.Add(this.__menuOption_Menu_StrobeSlow);
				}
			}
			using (TimeWarning.New("Menu_TurnOff", 0.1f))
			{
				if (this.Menu_StrobeOff_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_TurnOff.show = true;
					this.__menuOption_Menu_TurnOff.showDisabled = false;
					this.__menuOption_Menu_TurnOff.longUseOnly = false;
					this.__menuOption_Menu_TurnOff.order = -90;
					this.__menuOption_Menu_TurnOff.icon = "close";
					this.__menuOption_Menu_TurnOff.desc = "turnof_strobe_desc";
					this.__menuOption_Menu_TurnOff.title = "strobe_off";
					if (this.__menuOption_Menu_TurnOff.function == null)
					{
						this.__menuOption_Menu_TurnOff.function = new Action<BasePlayer>(this.Menu_TurnOff);
					}
					list.Add(this.__menuOption_Menu_TurnOff);
				}
			}
			using (TimeWarning.New("Menu_TurnOn", 0.1f))
			{
				if (this.Menu_StrobeOn_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_TurnOn.show = true;
					this.__menuOption_Menu_TurnOn.showDisabled = false;
					this.__menuOption_Menu_TurnOn.longUseOnly = false;
					this.__menuOption_Menu_TurnOn.order = -90;
					this.__menuOption_Menu_TurnOn.icon = "power";
					this.__menuOption_Menu_TurnOn.desc = "turnon_strobe_desc";
					this.__menuOption_Menu_TurnOn.title = "strobe_on";
					if (this.__menuOption_Menu_TurnOn.function == null)
					{
						this.__menuOption_Menu_TurnOn.function = new Action<BasePlayer>(this.Menu_TurnOn);
					}
					list.Add(this.__menuOption_Menu_TurnOn);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000033 RID: 51 RVA: 0x0002744C File Offset: 0x0002564C
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_StrobeFast_ShowIf(LocalPlayer.Entity) || this.Menu_StrobeMed_ShowIf(LocalPlayer.Entity) || this.Menu_StrobeSlow_ShowIf(LocalPlayer.Entity) || this.Menu_StrobeOff_ShowIf(LocalPlayer.Entity) || this.Menu_StrobeOn_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000034 RID: 52 RVA: 0x000274AC File Offset: 0x000256AC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StrobeLight.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}
}
