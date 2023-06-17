using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200003F RID: 63
public class PowerCounter : IOEntity
{
	// Token: 0x0400027F RID: 639
	private Option __menuOption_Menu_SetTarget;

	// Token: 0x04000280 RID: 640
	private Option __menuOption_Menu_ShowCounter;

	// Token: 0x04000281 RID: 641
	private Option __menuOption_Menu_ShowPassthrough;

	// Token: 0x04000282 RID: 642
	private int counterNumber;

	// Token: 0x04000283 RID: 643
	private int targetCounterNumber = 10;

	// Token: 0x04000284 RID: 644
	public CanvasGroup screenAlpha;

	// Token: 0x04000285 RID: 645
	public Text screenText;

	// Token: 0x04000286 RID: 646
	public const BaseEntity.Flags Flag_ShowPassthrough = BaseEntity.Flags.Reserved2;

	// Token: 0x04000287 RID: 647
	public GameObjectRef counterConfigPanel;

	// Token: 0x04000288 RID: 648
	public Color passthroughColor;

	// Token: 0x04000289 RID: 649
	public Color counterColor;

	// Token: 0x0400028A RID: 650
	private int client_counterNumber = -1;

	// Token: 0x0400028B RID: 651
	private float nextScreenVisTime = -1f;

	// Token: 0x0400028C RID: 652
	private int pendingNumberChange = -1;

	// Token: 0x06000505 RID: 1285 RVA: 0x0003DED8 File Offset: 0x0003C0D8
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("PowerCounter.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_SetTarget", 0.1f))
			{
				if (this.Menu_SetTarget_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_SetTarget.show = true;
					this.__menuOption_Menu_SetTarget.showDisabled = false;
					this.__menuOption_Menu_SetTarget.longUseOnly = false;
					this.__menuOption_Menu_SetTarget.order = -400;
					this.__menuOption_Menu_SetTarget.icon = "gear";
					this.__menuOption_Menu_SetTarget.desc = "settime_desc";
					this.__menuOption_Menu_SetTarget.title = "settarget";
					if (this.__menuOption_Menu_SetTarget.function == null)
					{
						this.__menuOption_Menu_SetTarget.function = new Action<BasePlayer>(this.Menu_SetTarget);
					}
					list.Add(this.__menuOption_Menu_SetTarget);
				}
			}
			using (TimeWarning.New("Menu_ShowCounter", 0.1f))
			{
				if (this.Menu_Counter_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_ShowCounter.show = true;
					this.__menuOption_Menu_ShowCounter.showDisabled = false;
					this.__menuOption_Menu_ShowCounter.longUseOnly = false;
					this.__menuOption_Menu_ShowCounter.order = 400;
					this.__menuOption_Menu_ShowCounter.icon = "stopwatch";
					this.__menuOption_Menu_ShowCounter.desc = "displaycounter_desc";
					this.__menuOption_Menu_ShowCounter.title = "displaycounter";
					if (this.__menuOption_Menu_ShowCounter.function == null)
					{
						this.__menuOption_Menu_ShowCounter.function = new Action<BasePlayer>(this.Menu_ShowCounter);
					}
					list.Add(this.__menuOption_Menu_ShowCounter);
				}
			}
			using (TimeWarning.New("Menu_ShowPassthrough", 0.1f))
			{
				if (this.Menu_Passthrough_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_ShowPassthrough.show = true;
					this.__menuOption_Menu_ShowPassthrough.showDisabled = false;
					this.__menuOption_Menu_ShowPassthrough.longUseOnly = false;
					this.__menuOption_Menu_ShowPassthrough.order = 400;
					this.__menuOption_Menu_ShowPassthrough.icon = "upgrade";
					this.__menuOption_Menu_ShowPassthrough.desc = "displaypassthrough_desc";
					this.__menuOption_Menu_ShowPassthrough.title = "displaypassthrough";
					if (this.__menuOption_Menu_ShowPassthrough.function == null)
					{
						this.__menuOption_Menu_ShowPassthrough.function = new Action<BasePlayer>(this.Menu_ShowPassthrough);
					}
					list.Add(this.__menuOption_Menu_ShowPassthrough);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x06000506 RID: 1286 RVA: 0x000066E8 File Offset: 0x000048E8
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_SetTarget_ShowIf(LocalPlayer.Entity) || this.Menu_Counter_ShowIf(LocalPlayer.Entity) || this.Menu_Passthrough_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x0003E1AC File Offset: 0x0003C3AC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PowerCounter.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x00005DC6 File Offset: 0x00003FC6
	public bool DisplayPassthrough()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x0000671D File Offset: 0x0000491D
	public bool DisplayCounter()
	{
		return !this.DisplayPassthrough();
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x00006728 File Offset: 0x00004928
	public bool CanPlayerAdmin(BasePlayer player)
	{
		return player != null && player.CanBuild();
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x0000673B File Offset: 0x0000493B
	public int GetTarget()
	{
		return this.targetCounterNumber;
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x0003E1F0 File Offset: 0x0003C3F0
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.client_counterNumber = info.msg.ioEntity.genericInt1;
			this.client_powerout = info.msg.ioEntity.genericInt2;
			this.targetCounterNumber = info.msg.ioEntity.genericInt3;
		}
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00006743 File Offset: 0x00004943
	public void UpdateNumber(int newNumber)
	{
		if (this.counterNumber == newNumber && this.GetColor() != this.screenText.color)
		{
			return;
		}
		this.pendingNumberChange = newNumber;
		this.nextScreenVisTime = Time.time + 0.2f;
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0003E254 File Offset: 0x0003C454
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		if (this.pendingNumberChange != this.counterNumber && this.screenAlpha.alpha == 0f)
		{
			this.counterNumber = this.pendingNumberChange;
			this.DelayedNumberChange();
		}
		float num = (this.nextScreenVisTime > 0f && this.nextScreenVisTime <= Time.time) ? 1f : 0f;
		if (this.screenAlpha.alpha != num)
		{
			this.screenAlpha.alpha = Mathf.MoveTowards(this.screenAlpha.alpha, num, Time.deltaTime * 10f);
		}
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0000677F File Offset: 0x0000497F
	public void DelayedNumberChange()
	{
		if (this.counterNumber == -1)
		{
			this.screenText.text = "-";
		}
		else
		{
			this.screenText.text = this.counterNumber.ToString();
		}
		this.UpdateColor();
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x000067B8 File Offset: 0x000049B8
	public Color GetColor()
	{
		if (!this.DisplayPassthrough())
		{
			return this.counterColor;
		}
		return this.passthroughColor;
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0003E2F8 File Offset: 0x0003C4F8
	public void UpdateColor()
	{
		Color color = this.DisplayPassthrough() ? this.passthroughColor : this.counterColor;
		if (this.screenText.color != color)
		{
			this.screenText.color = color;
		}
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x000067CF File Offset: 0x000049CF
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.UpdateColor();
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x000067DE File Offset: 0x000049DE
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		if (!base.IsPowered())
		{
			this.nextScreenVisTime = -1f;
			return;
		}
		if (this.DisplayPassthrough())
		{
			this.UpdateNumber(this.client_powerout + 1);
			return;
		}
		this.UpdateNumber(this.client_counterNumber);
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0000681D File Offset: 0x00004A1D
	[BaseEntity.Menu.Description("displaypassthrough_desc", "Show how much energy is flowing to us instead of the counter")]
	[BaseEntity.Menu.ShowIf("Menu_Passthrough_ShowIf")]
	[BaseEntity.Menu("displaypassthrough", "Show Passthrough", Order = 400)]
	[BaseEntity.Menu.Icon("upgrade")]
	public void Menu_ShowPassthrough(BasePlayer player)
	{
		base.ServerRPC<bool>("ToggleDisplayMode", true);
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x0000671D File Offset: 0x0000491D
	public bool Menu_Passthrough_ShowIf(BasePlayer player)
	{
		return !this.DisplayPassthrough();
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x0000682B File Offset: 0x00004A2B
	[BaseEntity.Menu.ShowIf("Menu_Counter_ShowIf")]
	[BaseEntity.Menu("displaycounter", "Show Counter", Order = 400)]
	[BaseEntity.Menu.Icon("stopwatch")]
	[BaseEntity.Menu.Description("displaycounter_desc", "Show the counter")]
	public void Menu_ShowCounter(BasePlayer player)
	{
		base.ServerRPC<bool>("ToggleDisplayMode", false);
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x00006839 File Offset: 0x00004A39
	public bool Menu_Counter_ShowIf(BasePlayer player)
	{
		return this.DisplayPassthrough();
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x00006841 File Offset: 0x00004A41
	public void SendNewTarget(int newTarget)
	{
		if (!this.CanPlayerAdmin(LocalPlayer.Entity))
		{
			return;
		}
		newTarget = Mathf.Clamp(newTarget, 1, 100);
		base.ServerRPC<int>("SERVER_SetTarget", newTarget);
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x0003E33C File Offset: 0x0003C53C
	[BaseEntity.Menu.ShowIf("Menu_SetTarget_ShowIf")]
	[BaseEntity.Menu.Icon("gear")]
	[BaseEntity.Menu.Description("settime_desc", "Set how long the timer should be active for")]
	[BaseEntity.Menu("settarget", "Set Target", Order = -400)]
	public void Menu_SetTarget(BasePlayer player)
	{
		if (LocalPlayer.Entity == null || !LocalPlayer.Entity.CanBuild())
		{
			return;
		}
		CounterConfig component = GameManager.client.CreatePrefab(this.counterConfigPanel.resourcePath, true).GetComponent<CounterConfig>();
		component.SetCounter(this);
		component.OpenDialog();
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x00006868 File Offset: 0x00004A68
	public bool Menu_SetTarget_ShowIf(BasePlayer player)
	{
		return this.CanPlayerAdmin(player) && !this.DisplayPassthrough();
	}
}
