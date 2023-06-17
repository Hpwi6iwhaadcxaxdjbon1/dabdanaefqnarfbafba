using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class Recycler : StorageContainer
{
	// Token: 0x0400029D RID: 669
	private Option __menuOption_Menu_TurnOff;

	// Token: 0x0400029E RID: 670
	private Option __menuOption_Menu_TurnOn;

	// Token: 0x0400029F RID: 671
	public float recycleEfficiency = 0.5f;

	// Token: 0x040002A0 RID: 672
	public SoundDefinition grindingLoopDef;

	// Token: 0x040002A1 RID: 673
	public GameObjectRef startSound;

	// Token: 0x040002A2 RID: 674
	public GameObjectRef stopSound;

	// Token: 0x040002A3 RID: 675
	private bool lastFrameOn;

	// Token: 0x040002A4 RID: 676
	private SoundModulation.Modulator grindingSoundModulator;

	// Token: 0x040002A5 RID: 677
	private Sound grindingLoop;

	// Token: 0x06000541 RID: 1345 RVA: 0x0003EB48 File Offset: 0x0003CD48
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("Recycler.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_TurnOff", 0.1f))
			{
				if (this.Menu_TurnOff_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_TurnOff.show = true;
					this.__menuOption_Menu_TurnOff.showDisabled = false;
					this.__menuOption_Menu_TurnOff.longUseOnly = false;
					this.__menuOption_Menu_TurnOff.order = 0;
					this.__menuOption_Menu_TurnOff.icon = "close";
					this.__menuOption_Menu_TurnOff.desc = "turn_off_recycler_desc";
					this.__menuOption_Menu_TurnOff.title = "turn_off";
					if (this.__menuOption_Menu_TurnOff.function == null)
					{
						this.__menuOption_Menu_TurnOff.function = new Action<BasePlayer>(this.Menu_TurnOff);
					}
					list.Add(this.__menuOption_Menu_TurnOff);
				}
			}
			using (TimeWarning.New("Menu_TurnOn", 0.1f))
			{
				if (this.Menu_TurnOn_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_TurnOn.show = true;
					this.__menuOption_Menu_TurnOn.showDisabled = false;
					this.__menuOption_Menu_TurnOn.longUseOnly = false;
					this.__menuOption_Menu_TurnOn.order = 0;
					this.__menuOption_Menu_TurnOn.icon = "power";
					this.__menuOption_Menu_TurnOn.desc = "turn_on_recycler_desc";
					this.__menuOption_Menu_TurnOn.title = "turn_on";
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

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x06000542 RID: 1346 RVA: 0x00006AA7 File Offset: 0x00004CA7
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_TurnOff_ShowIf(LocalPlayer.Entity) || this.Menu_TurnOn_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x0003ED3C File Offset: 0x0003CF3C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Recycler.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x00006ACD File Offset: 0x00004CCD
	public override void ResetState()
	{
		base.ResetState();
		this.lastFrameOn = false;
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x00004AC9 File Offset: 0x00002CC9
	[BaseEntity.Menu.ShowIf("Menu_TurnOn_ShowIf")]
	[BaseEntity.Menu("turn_on", "Turn On")]
	[BaseEntity.Menu.Description("turn_on_recycler_desc", "Begin Recycling")]
	[BaseEntity.Menu.Icon("power")]
	public void Menu_TurnOn(BasePlayer player)
	{
		base.ServerRPC<bool>("SVSwitch", true);
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x00006ADC File Offset: 0x00004CDC
	public bool Menu_TurnOn_ShowIf(BasePlayer player)
	{
		return !base.IsOn() && (!this.onlyOneUser || player.inventory.loot.entitySource == this);
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x00004AF7 File Offset: 0x00002CF7
	[BaseEntity.Menu("turn_off", "Turn Off")]
	[BaseEntity.Menu.Description("turn_off_recycler_desc", "Stop Recycling")]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu.ShowIf("Menu_TurnOff_ShowIf")]
	public void Menu_TurnOff(BasePlayer player)
	{
		base.ServerRPC<bool>("SVSwitch", false);
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x00006B08 File Offset: 0x00004D08
	public bool Menu_TurnOff_ShowIf(BasePlayer player)
	{
		return base.IsOn() && (!this.onlyOneUser || player.inventory.loot.entitySource == this);
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x00006B34 File Offset: 0x00004D34
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.InitializeClientsideEffects();
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0003ED80 File Offset: 0x0003CF80
	public void InitializeClientsideEffects()
	{
		if (this.grindingLoop == null)
		{
			this.grindingLoop = SoundManager.RequestSoundInstance(this.grindingLoopDef, base.gameObject, default(Vector3), false);
			this.grindingLoop.Play();
			this.grindingSoundModulator = this.grindingLoop.modulation.CreateModulator(SoundModulation.Parameter.Gain);
			this.grindingSoundModulator.value = 0f;
		}
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x00006B43 File Offset: 0x00004D43
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		if (this.grindingLoop)
		{
			this.grindingLoop.StopAndRecycle(0f);
			this.grindingLoop = null;
			this.grindingSoundModulator = null;
		}
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0003EDF0 File Offset: 0x0003CFF0
	public void Update()
	{
		if (this.grindingSoundModulator != null)
		{
			if (base.IsOn() && !this.lastFrameOn)
			{
				this.grindingLoop.Play();
			}
			this.grindingSoundModulator.value = Mathf.Lerp(this.grindingSoundModulator.value, base.IsOn() ? 1f : 0f, Time.deltaTime * 5f);
			this.lastFrameOn = base.IsOn();
		}
	}
}
