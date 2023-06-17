using System;
using System.Collections.Generic;
using GameMenu;
using Network;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class DudTimedExplosive : TimedExplosive
{
	// Token: 0x040001C6 RID: 454
	private Option __menuOption_Menu_Pickup;

	// Token: 0x040001C7 RID: 455
	public GameObjectRef fizzleEffect;

	// Token: 0x040001C8 RID: 456
	public GameObject wickSpark;

	// Token: 0x040001C9 RID: 457
	public AudioSource wickSound;

	// Token: 0x040001CA RID: 458
	public float dudChance = 0.3f;

	// Token: 0x040001CB RID: 459
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemToGive;

	// Token: 0x040001CC RID: 460
	[NonSerialized]
	private float explodeTime;

	// Token: 0x0600042C RID: 1068 RVA: 0x0003A884 File Offset: 0x00038A84
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("DudTimedExplosive.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Pickup", 0.1f))
			{
				if (this.Menu_PickupDud_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Pickup.show = true;
					this.__menuOption_Menu_Pickup.showDisabled = false;
					this.__menuOption_Menu_Pickup.longUseOnly = false;
					this.__menuOption_Menu_Pickup.order = 0;
					this.__menuOption_Menu_Pickup.icon = "player_loot";
					this.__menuOption_Menu_Pickup.desc = "pickup_dud_desc";
					this.__menuOption_Menu_Pickup.title = "pickup_dud";
					if (this.__menuOption_Menu_Pickup.function == null)
					{
						this.__menuOption_Menu_Pickup.function = new Action<BasePlayer>(this.Menu_Pickup);
					}
					list.Add(this.__menuOption_Menu_Pickup);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x0600042D RID: 1069 RVA: 0x00005C0F File Offset: 0x00003E0F
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_PickupDud_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x0003A98C File Offset: 0x00038B8C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DudTimedExplosive.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x000056A3 File Offset: 0x000038A3
	private bool IsWickBurning()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x0003A9D0 File Offset: 0x00038BD0
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		bool flag = this.IsWickBurning();
		base.Load(info);
		bool flag2 = this.IsWickBurning();
		if (info.msg.dudExplosive != null)
		{
			this.explodeTime = Time.realtimeSinceStartup + info.msg.dudExplosive.fuseTimeLeft;
		}
		if (base.isClient && flag != flag2)
		{
			this.wickSpark.SetActive(flag2);
		}
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x0003AA34 File Offset: 0x00038C34
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		if (!this.IsWickBurning())
		{
			if (this.wickSound.isPlaying)
			{
				this.wickSound.Stop();
			}
			return;
		}
		float num = MainCamera.Distance(base.transform.position);
		if (num > this.wickSound.maxDistance && this.wickSound.isPlaying)
		{
			this.wickSound.Stop();
		}
		if (num < this.wickSound.maxDistance && !this.wickSound.isPlaying)
		{
			this.wickSound.Play();
		}
		float num2 = Mathf.Clamp(this.explodeTime - Time.realtimeSinceStartup, 0f, 3f);
		float num3 = (1f - num2 / 3f) * 1f;
		this.wickSound.pitch = 1f + num3;
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00005C26 File Offset: 0x00003E26
	[BaseEntity.Menu.Description("pickup_dud_desc", "Pick up this dud")]
	[BaseEntity.Menu.ShowIf("Menu_PickupDud_ShowIf")]
	[BaseEntity.Menu("pickup_dud", "Pick up")]
	[BaseEntity.Menu.Icon("player_loot")]
	public void Menu_Pickup(BasePlayer player)
	{
		base.ServerRPC("RPC_Pickup");
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x00005C33 File Offset: 0x00003E33
	public bool Menu_PickupDud_ShowIf(BasePlayer player)
	{
		return !this.IsWickBurning();
	}
}
