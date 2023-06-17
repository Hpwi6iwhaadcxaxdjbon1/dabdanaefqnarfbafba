using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class SearchLight : StorageContainer
{
	// Token: 0x040002B2 RID: 690
	private Option __menuOption_StopUseLight;

	// Token: 0x040002B3 RID: 691
	private Option __menuOption_SwitchOff;

	// Token: 0x040002B4 RID: 692
	private Option __menuOption_SwitchOn;

	// Token: 0x040002B5 RID: 693
	private Option __menuOption_UseLight;

	// Token: 0x040002B6 RID: 694
	public GameObject pitchObject;

	// Token: 0x040002B7 RID: 695
	public GameObject yawObject;

	// Token: 0x040002B8 RID: 696
	public GameObject eyePoint;

	// Token: 0x040002B9 RID: 697
	public GameObject lightEffect;

	// Token: 0x040002BA RID: 698
	public SoundPlayer turnLoop;

	// Token: 0x040002BB RID: 699
	public ItemDefinition fuelType;

	// Token: 0x040002BC RID: 700
	private Vector3 aimDir = Vector3.zero;

	// Token: 0x040002BD RID: 701
	private bool wasMoving;

	// Token: 0x06000571 RID: 1393 RVA: 0x0003F538 File Offset: 0x0003D738
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("SearchLight.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("StopUseLight", 0.1f))
			{
				if (this.Menu_StopUseLight_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_StopUseLight.show = true;
					this.__menuOption_StopUseLight.showDisabled = false;
					this.__menuOption_StopUseLight.longUseOnly = false;
					this.__menuOption_StopUseLight.order = -100;
					this.__menuOption_StopUseLight.icon = "exit";
					this.__menuOption_StopUseLight.desc = "stopuselight_desc";
					this.__menuOption_StopUseLight.title = "stopuselight";
					if (this.__menuOption_StopUseLight.function == null)
					{
						this.__menuOption_StopUseLight.function = new Action<BasePlayer>(this.StopUseLight);
					}
					list.Add(this.__menuOption_StopUseLight);
				}
			}
			using (TimeWarning.New("SwitchOff", 0.1f))
			{
				if (this.Menu_SwitchOff_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_SwitchOff.show = true;
					this.__menuOption_SwitchOff.showDisabled = false;
					this.__menuOption_SwitchOff.longUseOnly = false;
					this.__menuOption_SwitchOff.order = -90;
					this.__menuOption_SwitchOff.icon = "close";
					this.__menuOption_SwitchOff.desc = "switchoff_desc";
					this.__menuOption_SwitchOff.title = "switchoff";
					if (this.__menuOption_SwitchOff.function == null)
					{
						this.__menuOption_SwitchOff.function = new Action<BasePlayer>(this.SwitchOff);
					}
					list.Add(this.__menuOption_SwitchOff);
				}
			}
			using (TimeWarning.New("SwitchOn", 0.1f))
			{
				if (this.Menu_SwitchOn_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_SwitchOn.show = true;
					this.__menuOption_SwitchOn.showDisabled = false;
					this.__menuOption_SwitchOn.longUseOnly = false;
					this.__menuOption_SwitchOn.order = -90;
					this.__menuOption_SwitchOn.icon = "power";
					this.__menuOption_SwitchOn.desc = "switchon_desc";
					this.__menuOption_SwitchOn.title = "switchon";
					if (this.__menuOption_SwitchOn.function == null)
					{
						this.__menuOption_SwitchOn.function = new Action<BasePlayer>(this.SwitchOn);
					}
					list.Add(this.__menuOption_SwitchOn);
				}
			}
			using (TimeWarning.New("UseLight", 0.1f))
			{
				if (this.Menu_UseLight_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_UseLight.show = true;
					this.__menuOption_UseLight.showDisabled = false;
					this.__menuOption_UseLight.longUseOnly = false;
					this.__menuOption_UseLight.order = -100;
					this.__menuOption_UseLight.icon = "gear";
					this.__menuOption_UseLight.desc = "uselight_desc";
					this.__menuOption_UseLight.title = "uselight";
					if (this.__menuOption_UseLight.function == null)
					{
						this.__menuOption_UseLight.function = new Action<BasePlayer>(this.UseLight);
					}
					list.Add(this.__menuOption_UseLight);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000572 RID: 1394 RVA: 0x0003F8D8 File Offset: 0x0003DAD8
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_StopUseLight_ShowIf(LocalPlayer.Entity) || this.Menu_SwitchOff_ShowIf(LocalPlayer.Entity) || this.Menu_SwitchOn_ShowIf(LocalPlayer.Entity) || this.Menu_UseLight_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0003F928 File Offset: 0x0003DB28
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SearchLight.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x00006CF6 File Offset: 0x00004EF6
	public override void ResetState()
	{
		this.aimDir = Vector3.zero;
		this.wasMoving = false;
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x00006D0A File Offset: 0x00004F0A
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.autoturret != null)
		{
			this.aimDir = info.msg.autoturret.aimDir;
		}
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x00006D36 File Offset: 0x00004F36
	public void Update()
	{
		if (base.isClient)
		{
			this.UpdateAimpoint();
		}
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0003F96C File Offset: 0x0003DB6C
	private void UpdateAimpoint()
	{
		if (this.aimDir == Vector3.zero)
		{
			return;
		}
		Quaternion quaternion = Quaternion.LookRotation(this.aimDir);
		Quaternion quaternion2 = Quaternion.Euler(0f, quaternion.eulerAngles.y, 0f);
		Quaternion quaternion3 = Quaternion.Euler(quaternion.eulerAngles.x, 0f, 0f);
		bool flag = false;
		if (this.yawObject.transform.rotation != quaternion2)
		{
			flag = (Quaternion.Angle(this.yawObject.transform.rotation, quaternion2) > 0.5f);
			this.yawObject.transform.rotation = Quaternion.Lerp(this.yawObject.transform.rotation, quaternion2, Time.deltaTime * 8f);
		}
		if (this.pitchObject.transform.localRotation != quaternion3)
		{
			this.pitchObject.transform.localRotation = Quaternion.Lerp(this.pitchObject.transform.localRotation, quaternion3, Time.deltaTime * 8f);
		}
		if (flag != this.wasMoving)
		{
			if (flag)
			{
				this.turnLoop.FadeInAndPlay(0.5f);
			}
			else
			{
				this.turnLoop.FadeOutAndRecycle(0.5f);
			}
		}
		this.wasMoving = flag;
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x00006D46 File Offset: 0x00004F46
	[BaseEntity.Menu.Description("switchon_desc", "Turn the light on")]
	[BaseEntity.Menu("switchon", "Switch On", Order = -90)]
	[BaseEntity.Menu.ShowIf("Menu_SwitchOn_ShowIf")]
	[BaseEntity.Menu.Icon("power")]
	public void SwitchOn(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_Switch", true);
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x00006D54 File Offset: 0x00004F54
	public bool Menu_SwitchOn_ShowIf(BasePlayer player)
	{
		return !base.HasFlag(BaseEntity.Flags.On) && (!this.needsBuildingPrivilegeToUse || LocalPlayer.Entity.CanBuild());
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x00006D75 File Offset: 0x00004F75
	[BaseEntity.Menu.ShowIf("Menu_SwitchOff_ShowIf")]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu("switchoff", "Switch Off", Order = -90)]
	[BaseEntity.Menu.Description("switchoff_desc", "Turn the light off")]
	public void SwitchOff(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_Switch", false);
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x00006D83 File Offset: 0x00004F83
	public bool Menu_SwitchOff_ShowIf(BasePlayer player)
	{
		return base.HasFlag(BaseEntity.Flags.On) && (!this.needsBuildingPrivilegeToUse || LocalPlayer.Entity.CanBuild());
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x00006DA4 File Offset: 0x00004FA4
	[BaseEntity.Menu("uselight", "Use", Order = -100)]
	[BaseEntity.Menu.ShowIf("Menu_UseLight_ShowIf")]
	[BaseEntity.Menu.Description("uselight_desc", "Control the aim direction of the light")]
	[BaseEntity.Menu.Icon("gear")]
	public void UseLight(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_UseLight", true);
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x00006DB2 File Offset: 0x00004FB2
	public bool Menu_UseLight_ShowIf(BasePlayer player)
	{
		return !base.HasFlag(BaseEntity.Flags.Reserved5) && (!this.needsBuildingPrivilegeToUse || LocalPlayer.Entity.CanBuild());
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x00006DD7 File Offset: 0x00004FD7
	[BaseEntity.Menu.Description("stopuselight_desc", "Stop controlling the light")]
	[BaseEntity.Menu("stopuselight", "Stop Using", Order = -100)]
	[BaseEntity.Menu.Icon("exit")]
	[BaseEntity.Menu.ShowIf("Menu_StopUseLight_ShowIf")]
	public void StopUseLight(BasePlayer player)
	{
		base.ServerRPC<bool>("RPC_UseLight", false);
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x00006DE5 File Offset: 0x00004FE5
	public bool Menu_StopUseLight_ShowIf(BasePlayer player)
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5) && (!this.needsBuildingPrivilegeToUse || LocalPlayer.Entity.CanBuild());
	}

	// Token: 0x02000049 RID: 73
	public static class SearchLightFlags
	{
		// Token: 0x040002BE RID: 702
		public const BaseEntity.Flags PlayerUsing = BaseEntity.Flags.Reserved5;
	}
}
