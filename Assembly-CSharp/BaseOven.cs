using System;
using System.Collections.Generic;
using GameMenu;
using Network;

// Token: 0x0200001D RID: 29
public class BaseOven : StorageContainer, ISplashable
{
	// Token: 0x0400011A RID: 282
	private Option __menuOption_SwitchOff;

	// Token: 0x0400011B RID: 283
	private Option __menuOption_SwitchOn;

	// Token: 0x0400011C RID: 284
	public BaseOven.TemperatureType temperature;

	// Token: 0x0400011D RID: 285
	public BaseEntity.Menu.Option switchOnMenu;

	// Token: 0x0400011E RID: 286
	public BaseEntity.Menu.Option switchOffMenu;

	// Token: 0x0400011F RID: 287
	public ItemAmount[] startupContents;

	// Token: 0x04000120 RID: 288
	public bool allowByproductCreation = true;

	// Token: 0x04000121 RID: 289
	public ItemDefinition fuelType;

	// Token: 0x04000122 RID: 290
	public bool canModFire;

	// Token: 0x04000123 RID: 291
	public bool disabledBySplash = true;

	// Token: 0x060002C0 RID: 704 RVA: 0x00032F30 File Offset: 0x00031130
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("BaseOven.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("SwitchOff", 0.1f))
			{
				if (this.SwitchOff_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_SwitchOff.show = true;
					this.__menuOption_SwitchOff.showDisabled = false;
					this.__menuOption_SwitchOff.longUseOnly = false;
					this.__menuOption_SwitchOff.order = 20;
					this.__menuOption_SwitchOff.copyOptionsFrom = this.switchOffMenu;
					if (this.__menuOption_SwitchOff.function == null)
					{
						this.__menuOption_SwitchOff.function = new Action<BasePlayer>(this.SwitchOff);
					}
					list.Add(this.__menuOption_SwitchOff);
				}
			}
			using (TimeWarning.New("SwitchOn", 0.1f))
			{
				if (this.SwitchOn_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_SwitchOn.show = true;
					this.__menuOption_SwitchOn.showDisabled = false;
					this.__menuOption_SwitchOn.longUseOnly = false;
					this.__menuOption_SwitchOn.order = 20;
					this.__menuOption_SwitchOn.copyOptionsFrom = this.switchOnMenu;
					if (this.__menuOption_SwitchOn.function == null)
					{
						this.__menuOption_SwitchOn.function = new Action<BasePlayer>(this.SwitchOn);
					}
					list.Add(this.__menuOption_SwitchOn);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x060002C1 RID: 705 RVA: 0x00004A8C File Offset: 0x00002C8C
	public override bool HasMenuOptions
	{
		get
		{
			return this.SwitchOff_ShowIf(LocalPlayer.Entity) || this.SwitchOn_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x000330E0 File Offset: 0x000312E0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseOven.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00004AB2 File Offset: 0x00002CB2
	public override bool HasSlot(BaseEntity.Slot slot)
	{
		return (this.canModFire && slot == BaseEntity.Slot.FireMod) || base.HasSlot(slot);
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00004AC9 File Offset: 0x00002CC9
	[BaseEntity.Menu.ShowIf("SwitchOn_ShowIf")]
	[BaseEntity.Menu(UseVariable = "switchOnMenu", Order = 20)]
	public void SwitchOn(BasePlayer player)
	{
		base.ServerRPC<bool>("SVSwitch", true);
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00004AD7 File Offset: 0x00002CD7
	public bool SwitchOn_ShowIf(BasePlayer player)
	{
		return !base.IsOn() && (!this.needsBuildingPrivilegeToUse || LocalPlayer.Entity.CanBuild());
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x00004AF7 File Offset: 0x00002CF7
	[BaseEntity.Menu(UseVariable = "switchOffMenu", Order = 20)]
	[BaseEntity.Menu.ShowIf("SwitchOff_ShowIf")]
	public void SwitchOff(BasePlayer player)
	{
		base.ServerRPC<bool>("SVSwitch", false);
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x00004B05 File Offset: 0x00002D05
	public bool SwitchOff_ShowIf(BasePlayer player)
	{
		return base.IsOn() && (!this.needsBuildingPrivilegeToUse || LocalPlayer.Entity.CanBuild());
	}

	// Token: 0x0200001E RID: 30
	public enum TemperatureType
	{
		// Token: 0x04000125 RID: 293
		Normal,
		// Token: 0x04000126 RID: 294
		Warming,
		// Token: 0x04000127 RID: 295
		Cooking,
		// Token: 0x04000128 RID: 296
		Smelting,
		// Token: 0x04000129 RID: 297
		Fractioning
	}
}
