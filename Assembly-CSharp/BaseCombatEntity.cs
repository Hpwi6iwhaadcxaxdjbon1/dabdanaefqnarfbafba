using System;
using System.Collections.Generic;
using ConVar;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class BaseCombatEntity : BaseEntity
{
	// Token: 0x040000ED RID: 237
	private Option __menuOption_Menu_Pickup;

	// Token: 0x040000EE RID: 238
	[Header("BaseCombatEntity")]
	public SkeletonProperties skeletonProperties;

	// Token: 0x040000EF RID: 239
	public ProtectionProperties baseProtection;

	// Token: 0x040000F0 RID: 240
	public float startHealth;

	// Token: 0x040000F1 RID: 241
	public BaseCombatEntity.Pickup pickup;

	// Token: 0x040000F2 RID: 242
	public BaseCombatEntity.Repair repair;

	// Token: 0x040000F3 RID: 243
	public bool ShowHealthInfo = true;

	// Token: 0x040000F4 RID: 244
	public BaseCombatEntity.LifeState lifestate;

	// Token: 0x040000F5 RID: 245
	public bool sendsHitNotification;

	// Token: 0x040000F6 RID: 246
	public bool sendsMeleeHitNotification = true;

	// Token: 0x040000F7 RID: 247
	public bool markAttackerHostile = true;

	// Token: 0x040000F8 RID: 248
	private float _health;

	// Token: 0x040000F9 RID: 249
	private float _maxHealth = 100f;

	// Token: 0x040000FA RID: 250
	protected float deathTime;

	// Token: 0x040000FB RID: 251
	private int lastNotifyFrame;

	// Token: 0x06000279 RID: 633 RVA: 0x00032408 File Offset: 0x00030608
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("BaseCombatEntity.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Menu_Pickup", 0.1f))
			{
				if (this.Menu_Pickup_If(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_Pickup.show = true;
					this.__menuOption_Menu_Pickup.showDisabled = false;
					this.__menuOption_Menu_Pickup.longUseOnly = false;
					this.__menuOption_Menu_Pickup.order = 100;
					this.__menuOption_Menu_Pickup.icon = "pickup";
					this.__menuOption_Menu_Pickup.desc = "pickup.desc";
					this.__menuOption_Menu_Pickup.title = "pickup";
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

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x0600027A RID: 634 RVA: 0x00004745 File Offset: 0x00002945
	public override bool HasMenuOptions
	{
		get
		{
			return this.Menu_Pickup_If(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600027B RID: 635 RVA: 0x00032510 File Offset: 0x00030710
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseCombatEntity.OnRpcMessage", 0.1f))
		{
			if (rpc == 3063539021U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: HitNotify ");
				}
				using (TimeWarning.New("HitNotify", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.HitNotify(rpc2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in HitNotify", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600027C RID: 636 RVA: 0x0003262C File Offset: 0x0003082C
	[BaseEntity.RPC_Client]
	public void HitNotify(BaseEntity.RPCMessage rpc)
	{
		if ((rpc.read.Bit() && hitnotify.notification_level == 1) || hitnotify.notification_level <= 0)
		{
			return;
		}
		Effect.client.Run("assets/bundled/prefabs/fx/hit_notify.prefab", default(Vector3), default(Vector3), default(Vector3));
	}

	// Token: 0x0600027D RID: 637 RVA: 0x0000475C File Offset: 0x0000295C
	protected override void ClientInit(Entity info)
	{
		this._maxHealth = this.StartHealth();
		base.ClientInit(info);
	}

	// Token: 0x0600027E RID: 638 RVA: 0x00004771 File Offset: 0x00002971
	public virtual bool DisplayHealthInfo(BasePlayer player)
	{
		return this.ShowHealthInfo;
	}

	// Token: 0x0600027F RID: 639 RVA: 0x00004779 File Offset: 0x00002979
	internal override Transform GetEyeTransform()
	{
		if (this.ragdoll && this.IsDead())
		{
			return this.ragdoll.eyeTransform;
		}
		return base.GetEyeTransform();
	}

	// Token: 0x06000280 RID: 640 RVA: 0x000047A2 File Offset: 0x000029A2
	protected virtual void OnLifeStateChanged()
	{
		if (this.lifestate == BaseCombatEntity.LifeState.Dead && !base.JustCreated)
		{
			this.deathTime = UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x06000281 RID: 641 RVA: 0x000047C0 File Offset: 0x000029C0
	public virtual bool IsDead()
	{
		return this.lifestate == BaseCombatEntity.LifeState.Dead;
	}

	// Token: 0x06000282 RID: 642 RVA: 0x000047CB File Offset: 0x000029CB
	public virtual bool IsAlive()
	{
		return this.lifestate == BaseCombatEntity.LifeState.Alive;
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000283 RID: 643 RVA: 0x000047D6 File Offset: 0x000029D6
	public float SecondsSinceDeath
	{
		get
		{
			return UnityEngine.Time.realtimeSinceStartup - this.deathTime;
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x06000284 RID: 644 RVA: 0x000047E4 File Offset: 0x000029E4
	// (set) Token: 0x06000285 RID: 645 RVA: 0x000047F3 File Offset: 0x000029F3
	public float healthFraction
	{
		get
		{
			return this.Health() / this.MaxHealth();
		}
		set
		{
			this.health = this.MaxHealth() * value;
		}
	}

	// Token: 0x06000286 RID: 646 RVA: 0x00004803 File Offset: 0x00002A03
	public override void ResetState()
	{
		base.ResetState();
		this._health = this._maxHealth;
	}

	// Token: 0x06000287 RID: 647 RVA: 0x00004817 File Offset: 0x00002A17
	public override void DestroyShared()
	{
		base.DestroyShared();
	}

	// Token: 0x06000288 RID: 648 RVA: 0x0000481F File Offset: 0x00002A1F
	public virtual float GetThreatLevel()
	{
		return 0f;
	}

	// Token: 0x06000289 RID: 649 RVA: 0x00004826 File Offset: 0x00002A26
	public override float PenetrationResistance(HitInfo info)
	{
		if (!this.baseProtection)
		{
			return 1f;
		}
		return this.baseProtection.density;
	}

	// Token: 0x0600028A RID: 650 RVA: 0x00004846 File Offset: 0x00002A46
	public virtual void ScaleDamage(HitInfo info)
	{
		if (info.UseProtection && this.baseProtection != null)
		{
			this.baseProtection.Scale(info.damageTypes, 1f);
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x0003267C File Offset: 0x0003087C
	public HitArea SkeletonLookup(uint boneID)
	{
		if (this.skeletonProperties == null)
		{
			return (HitArea)(-1);
		}
		SkeletonProperties.BoneProperty boneProperty = this.skeletonProperties.FindBone(boneID);
		if (boneProperty == null)
		{
			return (HitArea)(-1);
		}
		return boneProperty.area;
	}

	// Token: 0x0600028C RID: 652 RVA: 0x000326B4 File Offset: 0x000308B4
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		BaseCombatEntity.LifeState lifeState = this.lifestate;
		if (info.msg.baseCombat != null)
		{
			this.lifestate = (BaseCombatEntity.LifeState)info.msg.baseCombat.state;
			this._health = info.msg.baseCombat.health;
		}
		base.Load(info);
		if (base.isClient && lifeState != this.lifestate)
		{
			this.OnLifeStateChanged();
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x0600028D RID: 653 RVA: 0x00004874 File Offset: 0x00002A74
	// (set) Token: 0x0600028E RID: 654 RVA: 0x0000487C File Offset: 0x00002A7C
	public float health
	{
		get
		{
			return this._health;
		}
		set
		{
			this._health = Mathf.Clamp(value, 0f, this.MaxHealth());
		}
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00004874 File Offset: 0x00002A74
	public override float Health()
	{
		return this._health;
	}

	// Token: 0x06000290 RID: 656 RVA: 0x00004895 File Offset: 0x00002A95
	public override float MaxHealth()
	{
		return this._maxHealth;
	}

	// Token: 0x06000291 RID: 657 RVA: 0x0000489D File Offset: 0x00002A9D
	public virtual float StartHealth()
	{
		return this.startHealth;
	}

	// Token: 0x06000292 RID: 658 RVA: 0x000048A5 File Offset: 0x00002AA5
	public virtual float StartMaxHealth()
	{
		return this.StartHealth();
	}

	// Token: 0x06000293 RID: 659 RVA: 0x00032720 File Offset: 0x00030920
	public void DoHitNotify(HitInfo info)
	{
		using (TimeWarning.New("DoHitNotify", 0.1f))
		{
			if (this.sendsHitNotification && !(info.Initiator == null) && info.Initiator is BasePlayer && !info.isHeadshot && !(this == info.Initiator))
			{
				if (UnityEngine.Time.frameCount != this.lastNotifyFrame)
				{
					this.lastNotifyFrame = UnityEngine.Time.frameCount;
					bool flag = info.Weapon is BaseMelee;
					if (base.isClient && hitnotify.notification_level == 1 && info.Initiator == LocalPlayer.Entity && (!flag || this.sendsMeleeHitNotification))
					{
						Effect.client.Run("assets/bundled/prefabs/fx/hit_notify.prefab", default(Vector3), default(Vector3), default(Vector3));
					}
				}
			}
		}
	}

	// Token: 0x06000294 RID: 660 RVA: 0x00032814 File Offset: 0x00030A14
	public override void OnAttacked(HitInfo info)
	{
		using (TimeWarning.New("BaseCombatEntity.OnAttacked", 0.1f))
		{
			if (!this.IsDead())
			{
				this.DoHitNotify(info);
			}
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000295 RID: 661 RVA: 0x000048AD File Offset: 0x00002AAD
	public virtual bool CanPickup(BasePlayer player)
	{
		return this.pickup.enabled && (!this.pickup.requireBuildingPrivilege || (player.CanBuild() && (!this.pickup.requireHammer || player.IsHoldingEntity<Hammer>())));
	}

	// Token: 0x06000296 RID: 662 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnPickedUp(Item createdItem, BasePlayer player)
	{
	}

	// Token: 0x06000297 RID: 663 RVA: 0x000048EC File Offset: 0x00002AEC
	[BaseEntity.Menu("pickup", "Pickup", Order = 100)]
	[BaseEntity.Menu.Icon("pickup")]
	[BaseEntity.Menu.Description("pickup.desc", "Pick up this item and put it in your inventory")]
	[BaseEntity.Menu.ShowIf("Menu_Pickup_If")]
	public void Menu_Pickup(BasePlayer player)
	{
		base.ServerRPC("RPC_PickupStart");
	}

	// Token: 0x06000298 RID: 664 RVA: 0x000048F9 File Offset: 0x00002AF9
	public bool Menu_Pickup_If(BasePlayer player)
	{
		return this.CanPickup(player);
	}

	// Token: 0x06000299 RID: 665 RVA: 0x00032864 File Offset: 0x00030A64
	public virtual List<ItemAmount> BuildCost()
	{
		if (this.repair.itemTarget == null)
		{
			return null;
		}
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(this.repair.itemTarget);
		if (itemBlueprint == null)
		{
			return null;
		}
		return itemBlueprint.ingredients;
	}

	// Token: 0x02000018 RID: 24
	public enum LifeState
	{
		// Token: 0x040000FD RID: 253
		Alive,
		// Token: 0x040000FE RID: 254
		Dead
	}

	// Token: 0x02000019 RID: 25
	[Serializable]
	public struct Pickup
	{
		// Token: 0x040000FF RID: 255
		public bool enabled;

		// Token: 0x04000100 RID: 256
		[ItemSelector(ItemCategory.All)]
		public ItemDefinition itemTarget;

		// Token: 0x04000101 RID: 257
		public int itemCount;

		// Token: 0x04000102 RID: 258
		[Tooltip("Should we set the condition of the item based on the health of the picked up entity")]
		public bool setConditionFromHealth;

		// Token: 0x04000103 RID: 259
		[Tooltip("How much to reduce the item condition when picking up")]
		public float subtractCondition;

		// Token: 0x04000104 RID: 260
		[Tooltip("Must have building access to pick up")]
		public bool requireBuildingPrivilege;

		// Token: 0x04000105 RID: 261
		[Tooltip("Must have hammer equipped to pick up")]
		public bool requireHammer;

		// Token: 0x04000106 RID: 262
		[Tooltip("Inventory Must be empty (if applicable) to be picked up")]
		public bool requireEmptyInv;
	}

	// Token: 0x0200001A RID: 26
	[Serializable]
	public struct Repair
	{
		// Token: 0x04000107 RID: 263
		public bool enabled;

		// Token: 0x04000108 RID: 264
		[ItemSelector(ItemCategory.All)]
		public ItemDefinition itemTarget;
	}
}
