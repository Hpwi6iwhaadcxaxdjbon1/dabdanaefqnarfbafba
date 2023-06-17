using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using GameMenu;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class AutoTurret : StorageContainer
{
	// Token: 0x040000C4 RID: 196
	private Option __menuOption_MenuAuthorize;

	// Token: 0x040000C5 RID: 197
	private Option __menuOption_MenuClearList;

	// Token: 0x040000C6 RID: 198
	private Option __menuOption_MenuDeauthorize;

	// Token: 0x040000C7 RID: 199
	private Option __menuOption_MenuTurretAttackAll;

	// Token: 0x040000C8 RID: 200
	private Option __menuOption_MenuTurretDisable;

	// Token: 0x040000C9 RID: 201
	private Option __menuOption_MenuTurretEnable;

	// Token: 0x040000CA RID: 202
	private Option __menuOption_MenuTurretPeacekeeper;

	// Token: 0x040000CB RID: 203
	private Option __menuOption_MenuTurretRotate;

	// Token: 0x040000CC RID: 204
	public GameObjectRef gun_fire_effect;

	// Token: 0x040000CD RID: 205
	public GameObjectRef bulletEffect;

	// Token: 0x040000CE RID: 206
	public float bulletSpeed = 200f;

	// Token: 0x040000CF RID: 207
	public AmbienceEmitter ambienceEmitter;

	// Token: 0x040000D0 RID: 208
	private SoundModulation.Modulator turnSoundModulator;

	// Token: 0x040000D1 RID: 209
	private Sound turnLoop;

	// Token: 0x040000D2 RID: 210
	private float nextFocusSound;

	// Token: 0x040000D3 RID: 211
	private bool wasTurning;

	// Token: 0x040000D4 RID: 212
	private Quaternion lastYaw = Quaternion.identity;

	// Token: 0x040000D5 RID: 213
	public BaseCombatEntity target;

	// Token: 0x040000D6 RID: 214
	public Transform eyePos;

	// Token: 0x040000D7 RID: 215
	public Transform muzzlePos;

	// Token: 0x040000D8 RID: 216
	public Vector3 aimDir;

	// Token: 0x040000D9 RID: 217
	public Transform gun_yaw;

	// Token: 0x040000DA RID: 218
	public Transform gun_pitch;

	// Token: 0x040000DB RID: 219
	public float sightRange = 30f;

	// Token: 0x040000DC RID: 220
	public SoundDefinition turnLoopDef;

	// Token: 0x040000DD RID: 221
	public SoundDefinition movementChangeDef;

	// Token: 0x040000DE RID: 222
	public SoundDefinition ambientLoopDef;

	// Token: 0x040000DF RID: 223
	public SoundDefinition focusCameraDef;

	// Token: 0x040000E0 RID: 224
	public float focusSoundFreqMin = 2.5f;

	// Token: 0x040000E1 RID: 225
	public float focusSoundFreqMax = 7f;

	// Token: 0x040000E2 RID: 226
	public GameObjectRef peacekeeperToggleSound;

	// Token: 0x040000E3 RID: 227
	public GameObjectRef onlineSound;

	// Token: 0x040000E4 RID: 228
	public GameObjectRef offlineSound;

	// Token: 0x040000E5 RID: 229
	public GameObjectRef targetAcquiredEffect;

	// Token: 0x040000E6 RID: 230
	public GameObjectRef targetLostEffect;

	// Token: 0x040000E7 RID: 231
	public float aimCone;

	// Token: 0x040000E8 RID: 232
	private List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	// Token: 0x040000E9 RID: 233
	public ItemDefinition ammoType;

	// Token: 0x040000EA RID: 234
	public TargetTrigger targetTrigger;

	// Token: 0x0600024B RID: 587 RVA: 0x000314F4 File Offset: 0x0002F6F4
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("AutoTurret.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("MenuAuthorize", 0.1f))
			{
				if (this.MenuAuthorize_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuAuthorize.show = true;
					this.__menuOption_MenuAuthorize.showDisabled = false;
					this.__menuOption_MenuAuthorize.longUseOnly = false;
					this.__menuOption_MenuAuthorize.order = -10;
					this.__menuOption_MenuAuthorize.icon = "authorize";
					this.__menuOption_MenuAuthorize.desc = "autoturret_authorize_desc";
					this.__menuOption_MenuAuthorize.title = "autoturret_authorize";
					if (this.__menuOption_MenuAuthorize.function == null)
					{
						this.__menuOption_MenuAuthorize.function = new Action<BasePlayer>(this.MenuAuthorize);
					}
					list.Add(this.__menuOption_MenuAuthorize);
				}
			}
			using (TimeWarning.New("MenuClearList", 0.1f))
			{
				if (this.MenuClearList_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuClearList.show = true;
					this.__menuOption_MenuClearList.showDisabled = false;
					this.__menuOption_MenuClearList.longUseOnly = false;
					this.__menuOption_MenuClearList.order = 100;
					this.__menuOption_MenuClearList.icon = "clear_list";
					this.__menuOption_MenuClearList.desc = "autoturret_clear_desc";
					this.__menuOption_MenuClearList.title = "autoturret_clear";
					if (this.__menuOption_MenuClearList.function == null)
					{
						this.__menuOption_MenuClearList.function = new Action<BasePlayer>(this.MenuClearList);
					}
					list.Add(this.__menuOption_MenuClearList);
				}
			}
			using (TimeWarning.New("MenuDeauthorize", 0.1f))
			{
				if (this.MenuDeauthorize_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuDeauthorize.show = true;
					this.__menuOption_MenuDeauthorize.showDisabled = false;
					this.__menuOption_MenuDeauthorize.longUseOnly = false;
					this.__menuOption_MenuDeauthorize.order = 200;
					this.__menuOption_MenuDeauthorize.icon = "deauthorize";
					this.__menuOption_MenuDeauthorize.desc = "autoturret_deauthorize_desc";
					this.__menuOption_MenuDeauthorize.title = "autoturret_deauthorize";
					if (this.__menuOption_MenuDeauthorize.function == null)
					{
						this.__menuOption_MenuDeauthorize.function = new Action<BasePlayer>(this.MenuDeauthorize);
					}
					list.Add(this.__menuOption_MenuDeauthorize);
				}
			}
			using (TimeWarning.New("MenuTurretAttackAll", 0.1f))
			{
				if (this.MenuTurretAttackAll_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuTurretAttackAll.show = true;
					this.__menuOption_MenuTurretAttackAll.showDisabled = false;
					this.__menuOption_MenuTurretAttackAll.longUseOnly = false;
					this.__menuOption_MenuTurretAttackAll.order = 20;
					this.__menuOption_MenuTurretAttackAll.icon = "target";
					this.__menuOption_MenuTurretAttackAll.desc = "turret_attackall_desc";
					this.__menuOption_MenuTurretAttackAll.title = "turret_attackall";
					if (this.__menuOption_MenuTurretAttackAll.function == null)
					{
						this.__menuOption_MenuTurretAttackAll.function = new Action<BasePlayer>(this.MenuTurretAttackAll);
					}
					list.Add(this.__menuOption_MenuTurretAttackAll);
				}
			}
			using (TimeWarning.New("MenuTurretDisable", 0.1f))
			{
				if (this.MenuTurretDisable_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuTurretDisable.show = true;
					this.__menuOption_MenuTurretDisable.showDisabled = false;
					this.__menuOption_MenuTurretDisable.longUseOnly = false;
					this.__menuOption_MenuTurretDisable.order = 1;
					this.__menuOption_MenuTurretDisable.icon = "close";
					this.__menuOption_MenuTurretDisable.desc = "turret_disable_desc";
					this.__menuOption_MenuTurretDisable.title = "turret_disable";
					if (this.__menuOption_MenuTurretDisable.function == null)
					{
						this.__menuOption_MenuTurretDisable.function = new Action<BasePlayer>(this.MenuTurretDisable);
					}
					list.Add(this.__menuOption_MenuTurretDisable);
				}
			}
			using (TimeWarning.New("MenuTurretEnable", 0.1f))
			{
				if (this.MenuTurretEnable_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuTurretEnable.show = true;
					this.__menuOption_MenuTurretEnable.showDisabled = false;
					this.__menuOption_MenuTurretEnable.longUseOnly = false;
					this.__menuOption_MenuTurretEnable.order = 1;
					this.__menuOption_MenuTurretEnable.icon = "power";
					this.__menuOption_MenuTurretEnable.desc = "turret_enable_desc";
					this.__menuOption_MenuTurretEnable.title = "turret_enable";
					if (this.__menuOption_MenuTurretEnable.function == null)
					{
						this.__menuOption_MenuTurretEnable.function = new Action<BasePlayer>(this.MenuTurretEnable);
					}
					list.Add(this.__menuOption_MenuTurretEnable);
				}
			}
			using (TimeWarning.New("MenuTurretPeacekeeper", 0.1f))
			{
				if (this.MenuTurretPeacekeeper_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuTurretPeacekeeper.show = true;
					this.__menuOption_MenuTurretPeacekeeper.showDisabled = false;
					this.__menuOption_MenuTurretPeacekeeper.longUseOnly = false;
					this.__menuOption_MenuTurretPeacekeeper.order = 10;
					this.__menuOption_MenuTurretPeacekeeper.icon = "peace";
					this.__menuOption_MenuTurretPeacekeeper.desc = "turret_peacekeeper_desc";
					this.__menuOption_MenuTurretPeacekeeper.title = "turret_peacekeeper";
					if (this.__menuOption_MenuTurretPeacekeeper.function == null)
					{
						this.__menuOption_MenuTurretPeacekeeper.function = new Action<BasePlayer>(this.MenuTurretPeacekeeper);
					}
					list.Add(this.__menuOption_MenuTurretPeacekeeper);
				}
			}
			using (TimeWarning.New("MenuTurretRotate", 0.1f))
			{
				if (this.MenuTurretRotate_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_MenuTurretRotate.show = true;
					this.__menuOption_MenuTurretRotate.showDisabled = false;
					this.__menuOption_MenuTurretRotate.longUseOnly = false;
					this.__menuOption_MenuTurretRotate.order = 30;
					this.__menuOption_MenuTurretRotate.icon = "rotate";
					this.__menuOption_MenuTurretRotate.desc = "turret_rotate_desc";
					this.__menuOption_MenuTurretRotate.title = "turret_rotate";
					if (this.__menuOption_MenuTurretRotate.function == null)
					{
						this.__menuOption_MenuTurretRotate.function = new Action<BasePlayer>(this.MenuTurretRotate);
					}
					list.Add(this.__menuOption_MenuTurretRotate);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x0600024C RID: 588 RVA: 0x00031BE8 File Offset: 0x0002FDE8
	public override bool HasMenuOptions
	{
		get
		{
			return this.MenuAuthorize_ShowIf(LocalPlayer.Entity) || this.MenuClearList_ShowIf(LocalPlayer.Entity) || this.MenuDeauthorize_ShowIf(LocalPlayer.Entity) || this.MenuTurretAttackAll_ShowIf(LocalPlayer.Entity) || this.MenuTurretDisable_ShowIf(LocalPlayer.Entity) || this.MenuTurretEnable_ShowIf(LocalPlayer.Entity) || this.MenuTurretPeacekeeper_ShowIf(LocalPlayer.Entity) || this.MenuTurretRotate_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x0600024D RID: 589 RVA: 0x00031C74 File Offset: 0x0002FE74
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("AutoTurret.OnRpcMessage", 0.1f))
		{
			if (rpc == 1127653975U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_FireGun ");
				}
				using (TimeWarning.New("CLIENT_FireGun", 0.1f))
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
							this.CLIENT_FireGun(rpc2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_FireGun", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
			if (rpc == 1671178114U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_ReceiveAimDir ");
				}
				using (TimeWarning.New("CLIENT_ReceiveAimDir", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rpc3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CLIENT_ReceiveAimDir(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_ReceiveAimDir", true);
						Debug.LogException(exception2);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600024E RID: 590 RVA: 0x000044F5 File Offset: 0x000026F5
	public override void ResetState()
	{
		base.ResetState();
		this.nextFocusSound = 0f;
		this.wasTurning = false;
		this.lastYaw = Quaternion.identity;
	}

	// Token: 0x0600024F RID: 591 RVA: 0x0000451A File Offset: 0x0000271A
	public override bool ShouldShowLootMenus()
	{
		return this.IsOffline() && base.ShouldShowLootMenus();
	}

	// Token: 0x06000250 RID: 592 RVA: 0x0000452C File Offset: 0x0000272C
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.InitializeClientsideEffects();
	}

	// Token: 0x06000251 RID: 593 RVA: 0x00002ECE File Offset: 0x000010CE
	public void InitializeClientsideEffects()
	{
	}

	// Token: 0x06000252 RID: 594 RVA: 0x0000453B File Offset: 0x0000273B
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		if (this.turnLoop)
		{
			this.turnLoop.StopAndRecycle(0f);
			this.turnLoop = null;
			this.turnSoundModulator = null;
		}
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00031EBC File Offset: 0x000300BC
	public void ClientTick()
	{
		this.UpdateAiming();
		bool didTurn = this.gun_yaw.transform.rotation != this.lastYaw;
		this.lastYaw = this.gun_yaw.transform.rotation;
		this.UpdateSounds(didTurn);
		this.wasTurning = didTurn;
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00031F10 File Offset: 0x00030110
	private void UpdateSounds(bool didTurn)
	{
		this.ambienceEmitter.enabled = this.IsOnline();
		if (this.ambienceEmitter != null && this.ambienceEmitter.active)
		{
			if (didTurn != this.wasTurning)
			{
				SoundManager.PlayOneshot(this.movementChangeDef, base.gameObject, false, default(Vector3));
			}
			else if (!this.wasTurning && this.IsOnline() && UnityEngine.Time.time > this.nextFocusSound)
			{
				SoundManager.PlayOneshot(this.focusCameraDef, base.gameObject, false, default(Vector3));
				this.nextFocusSound = UnityEngine.Time.time + Random.Range(this.focusSoundFreqMin, this.focusSoundFreqMax);
			}
			if (didTurn && this.turnLoop == null)
			{
				this.turnLoop = SoundManager.RequestSoundInstance(this.turnLoopDef, base.gameObject, default(Vector3), false);
				if (this.turnLoop)
				{
					this.turnSoundModulator = this.turnLoop.modulation.CreateModulator(SoundModulation.Parameter.Gain);
					this.turnSoundModulator.value = 0.001f;
					this.turnLoop.Play();
				}
			}
		}
		if (this.turnLoop != null && this.turnSoundModulator != null)
		{
			float b = didTurn ? 1f : 0f;
			this.turnSoundModulator.value = Mathf.Lerp(this.turnSoundModulator.value, b, UnityEngine.Time.deltaTime * 4f);
			if (this.turnSoundModulator.value < 0.001f && this.turnLoop.isAudioSourcePlaying)
			{
				this.turnLoop.StopAndRecycle(0f);
				this.turnLoop = null;
				this.turnSoundModulator = null;
			}
		}
	}

	// Token: 0x06000255 RID: 597 RVA: 0x0000456E File Offset: 0x0000276E
	[BaseEntity.RPC_Client]
	public void CLIENT_ReceiveAimDir(BaseEntity.RPCMessage rpc)
	{
		this.aimDir = rpc.read.Vector3();
	}

	// Token: 0x06000256 RID: 598 RVA: 0x000320C8 File Offset: 0x000302C8
	[BaseEntity.RPC_Client]
	public void CLIENT_FireGun(BaseEntity.RPCMessage rpc)
	{
		uint i = rpc.read.UInt32();
		Transform transform = this.model.FindBone(StringPool.Get(i));
		Vector3 a = rpc.read.Vector3();
		Vector3 position = transform.position;
		Vector3 normalized = (a - position).normalized;
		Effect.client.Run(this.gun_fire_effect.resourcePath, this, StringPool.Get(transform.gameObject.name), Vector3.zero, Vector3.zero);
		GameObject gameObject = GameManager.client.CreatePrefab(this.bulletEffect.resourcePath, position + normalized * 1f, Quaternion.LookRotation(normalized), false);
		if (gameObject == null)
		{
			return;
		}
		Projectile component = gameObject.GetComponent<Projectile>();
		if (component)
		{
			component.clientsideEffect = true;
			component.owner = null;
			component.seed = 0;
			component.InitializeVelocity(normalized * this.bulletSpeed);
		}
		gameObject.SetActive(true);
	}

	// Token: 0x06000257 RID: 599 RVA: 0x00004581 File Offset: 0x00002781
	[BaseEntity.Menu.ShowIf("MenuTurretEnable_ShowIf")]
	[BaseEntity.Menu.Icon("power")]
	[BaseEntity.Menu("turret_enable", "Turn On", Order = 1)]
	[BaseEntity.Menu.Description("turret_enable_desc", "Turn the AutoTurret On.")]
	public void MenuTurretEnable(BasePlayer player)
	{
		base.ServerRPC("SERVER_TurnOn");
	}

	// Token: 0x06000258 RID: 600 RVA: 0x0000458E File Offset: 0x0000278E
	public bool MenuTurretEnable_ShowIf(BasePlayer player)
	{
		return this.CanChangeSettings(player);
	}

	// Token: 0x06000259 RID: 601 RVA: 0x00004597 File Offset: 0x00002797
	[BaseEntity.Menu("turret_disable", "Turn Off", Order = 1)]
	[BaseEntity.Menu.Description("turret_disable_desc", "Turn the AutoTurret Off.")]
	[BaseEntity.Menu.Icon("close")]
	[BaseEntity.Menu.ShowIf("MenuTurretDisable_ShowIf")]
	public void MenuTurretDisable(BasePlayer player)
	{
		base.ServerRPC("SERVER_TurnOff");
	}

	// Token: 0x0600025A RID: 602 RVA: 0x000045A4 File Offset: 0x000027A4
	public bool MenuTurretDisable_ShowIf(BasePlayer player)
	{
		return this.IsOnline() && this.IsAuthed(player);
	}

	// Token: 0x0600025B RID: 603 RVA: 0x000045B7 File Offset: 0x000027B7
	[BaseEntity.Menu.ShowIf("MenuAuthorize_ShowIf")]
	[BaseEntity.Menu.Icon("authorize")]
	[BaseEntity.Menu("autoturret_authorize", "Authorize", Order = -10)]
	[BaseEntity.Menu.Description("autoturret_authorize_desc", "Add yourself to this turret's friendly list so you are not targeted")]
	public void MenuAuthorize(BasePlayer player)
	{
		base.ServerRPC("AddSelfAuthorize");
	}

	// Token: 0x0600025C RID: 604 RVA: 0x000045C4 File Offset: 0x000027C4
	public bool MenuAuthorize_ShowIf(BasePlayer player)
	{
		return !this.IsAuthed(player) && this.IsOffline();
	}

	// Token: 0x0600025D RID: 605 RVA: 0x000045D7 File Offset: 0x000027D7
	[BaseEntity.Menu.ShowIf("MenuDeauthorize_ShowIf")]
	[BaseEntity.Menu.Icon("deauthorize")]
	[BaseEntity.Menu("autoturret_deauthorize", "Deauthorize", Order = 200)]
	[BaseEntity.Menu.Description("autoturret_deauthorize_desc", "Remove yourself from this turret's friendly list. Warning : You will be targeted!")]
	public void MenuDeauthorize(BasePlayer player)
	{
		base.ServerRPC("RemoveSelfAuthorize");
	}

	// Token: 0x0600025E RID: 606 RVA: 0x0000458E File Offset: 0x0000278E
	public bool MenuDeauthorize_ShowIf(BasePlayer player)
	{
		return this.CanChangeSettings(player);
	}

	// Token: 0x0600025F RID: 607 RVA: 0x000045E4 File Offset: 0x000027E4
	[BaseEntity.Menu.ShowIf("MenuClearList_ShowIf")]
	[BaseEntity.Menu.Icon("clear_list")]
	[BaseEntity.Menu.Description("autoturret_clear_desc", "Clear the authorized list")]
	[BaseEntity.Menu("autoturret_clear", "Clear Authorized List", Order = 100)]
	public void MenuClearList(BasePlayer player)
	{
		base.ServerRPC("ClearList");
	}

	// Token: 0x06000260 RID: 608 RVA: 0x0000458E File Offset: 0x0000278E
	public bool MenuClearList_ShowIf(BasePlayer player)
	{
		return this.CanChangeSettings(player);
	}

	// Token: 0x06000261 RID: 609 RVA: 0x000045F1 File Offset: 0x000027F1
	[BaseEntity.Menu.ShowIf("MenuTurretRotate_ShowIf")]
	[BaseEntity.Menu.Description("turret_rotate_desc", "Rotate the turret aiming position.")]
	[BaseEntity.Menu.Icon("rotate")]
	[BaseEntity.Menu("turret_rotate", "Rotate", Order = 30)]
	public void MenuTurretRotate(BasePlayer player)
	{
		base.ServerRPC("FlipAim");
	}

	// Token: 0x06000262 RID: 610 RVA: 0x0000458E File Offset: 0x0000278E
	public bool MenuTurretRotate_ShowIf(BasePlayer player)
	{
		return this.CanChangeSettings(player);
	}

	// Token: 0x06000263 RID: 611 RVA: 0x000045FE File Offset: 0x000027FE
	[BaseEntity.Menu.Icon("peace")]
	[BaseEntity.Menu.ShowIf("MenuTurretPeacekeeper_ShowIf")]
	[BaseEntity.Menu.Description("turret_peacekeeper_desc", "Enable Peacekeeper mode and only target armed or hostile players")]
	[BaseEntity.Menu("turret_peacekeeper", "Peacekeeper mode", Order = 10)]
	public void MenuTurretPeacekeeper(BasePlayer player)
	{
		base.ServerRPC("SERVER_Peacekeeper");
	}

	// Token: 0x06000264 RID: 612 RVA: 0x0000460B File Offset: 0x0000280B
	public bool MenuTurretPeacekeeper_ShowIf(BasePlayer player)
	{
		return this.CanChangeSettings(player) && !base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000265 RID: 613 RVA: 0x00004626 File Offset: 0x00002826
	[BaseEntity.Menu.Description("turret_attackall_desc", "Target everyone who is not authorized")]
	[BaseEntity.Menu.ShowIf("MenuTurretAttackAll_ShowIf")]
	[BaseEntity.Menu.Icon("target")]
	[BaseEntity.Menu("turret_attackall", "Attack All", Order = 20)]
	public void MenuTurretAttackAll(BasePlayer player)
	{
		base.ServerRPC("SERVER_AttackAll");
	}

	// Token: 0x06000266 RID: 614 RVA: 0x00004633 File Offset: 0x00002833
	public bool MenuTurretAttackAll_ShowIf(BasePlayer player)
	{
		return this.CanChangeSettings(player) && base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000267 RID: 615 RVA: 0x0000464B File Offset: 0x0000284B
	public bool IsOnline()
	{
		return base.IsOn();
	}

	// Token: 0x06000268 RID: 616 RVA: 0x00004653 File Offset: 0x00002853
	public bool IsOffline()
	{
		return !this.IsOnline();
	}

	// Token: 0x06000269 RID: 617 RVA: 0x0000465E File Offset: 0x0000285E
	public virtual Transform GetCenterMuzzle()
	{
		return this.muzzlePos;
	}

	// Token: 0x0600026A RID: 618 RVA: 0x000321C0 File Offset: 0x000303C0
	public float AngleToTarget(BaseCombatEntity potentialtarget)
	{
		Transform centerMuzzle = this.GetCenterMuzzle();
		Vector3 a = this.AimOffset(potentialtarget);
		Vector3 position = centerMuzzle.position;
		Vector3 normalized = (a - position).normalized;
		return Vector3.Angle(centerMuzzle.forward, normalized);
	}

	// Token: 0x0600026B RID: 619 RVA: 0x00004666 File Offset: 0x00002866
	public virtual bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return this.AngleToTarget(potentialtarget) <= 90f;
	}

	// Token: 0x0600026C RID: 620 RVA: 0x00004679 File Offset: 0x00002879
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && this.IsOffline() && this.IsAuthed(player);
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00004695 File Offset: 0x00002895
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.autoturret != null)
		{
			this.authorizedPlayers = info.msg.autoturret.users;
			info.msg.autoturret.users = null;
		}
	}

	// Token: 0x0600026E RID: 622 RVA: 0x000046D2 File Offset: 0x000028D2
	public void Update()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (base.isClient)
		{
			this.ClientTick();
		}
	}

	// Token: 0x0600026F RID: 623 RVA: 0x00032200 File Offset: 0x00030400
	public Vector3 AimOffset(BaseCombatEntity aimat)
	{
		BasePlayer basePlayer = aimat as BasePlayer;
		if (!(basePlayer != null))
		{
			return aimat.transform.position + new Vector3(0f, 0.3f, 0f);
		}
		if (basePlayer.IsSleeping())
		{
			return basePlayer.transform.position + Vector3.up * 0.1f;
		}
		return basePlayer.eyes.position;
	}

	// Token: 0x06000270 RID: 624 RVA: 0x000046EB File Offset: 0x000028EB
	public float GetAimSpeed()
	{
		if (base.isClient)
		{
			return 5f;
		}
		return 1f;
	}

	// Token: 0x06000271 RID: 625 RVA: 0x00032278 File Offset: 0x00030478
	public void UpdateAiming()
	{
		if (this.aimDir == Vector3.zero)
		{
			return;
		}
		float num = base.isServer ? 16f : 5f;
		Quaternion quaternion = Quaternion.LookRotation(this.aimDir);
		Quaternion quaternion2 = Quaternion.Euler(0f, quaternion.eulerAngles.y, 0f);
		Quaternion quaternion3 = Quaternion.Euler(quaternion.eulerAngles.x, 0f, 0f);
		if (this.gun_yaw.transform.rotation != quaternion2)
		{
			this.gun_yaw.transform.rotation = Quaternion.Lerp(this.gun_yaw.transform.rotation, quaternion2, UnityEngine.Time.deltaTime * num);
		}
		if (this.gun_pitch.transform.localRotation != quaternion3)
		{
			this.gun_pitch.transform.localRotation = Quaternion.Lerp(this.gun_pitch.transform.localRotation, quaternion3, UnityEngine.Time.deltaTime * num);
		}
	}

	// Token: 0x06000272 RID: 626 RVA: 0x0003237C File Offset: 0x0003057C
	public bool IsAuthed(BasePlayer player)
	{
		return Enumerable.Any<PlayerNameID>(this.authorizedPlayers, (PlayerNameID x) => x.userid == player.userID);
	}

	// Token: 0x06000273 RID: 627 RVA: 0x00004700 File Offset: 0x00002900
	public bool AnyAuthed()
	{
		return this.authorizedPlayers.Count > 0;
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00004710 File Offset: 0x00002910
	public virtual bool CanChangeSettings(BasePlayer player)
	{
		return this.IsAuthed(player) && this.IsOffline();
	}

	// Token: 0x06000275 RID: 629 RVA: 0x00004723 File Offset: 0x00002923
	public bool PeacekeeperMode()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x02000015 RID: 21
	public static class TurretFlags
	{
		// Token: 0x040000EB RID: 235
		public const BaseEntity.Flags Peacekeeper = BaseEntity.Flags.Reserved1;
	}
}
