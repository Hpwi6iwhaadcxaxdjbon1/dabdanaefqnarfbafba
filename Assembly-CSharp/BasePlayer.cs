using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Facepunch.Steamworks;
using GameMenu;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200001F RID: 31
public class BasePlayer : BaseCombatEntity
{
	// Token: 0x0400012A RID: 298
	private Option __menuOption_Climb;

	// Token: 0x0400012B RID: 299
	private Option __menuOption_Drink;

	// Token: 0x0400012C RID: 300
	private Option __menuOption_InviteToTeam;

	// Token: 0x0400012D RID: 301
	private Option __menuOption_Menu_AssistPlayer;

	// Token: 0x0400012E RID: 302
	private Option __menuOption_Menu_LootPlayer;

	// Token: 0x0400012F RID: 303
	private Option __menuOption_Promote;

	// Token: 0x04000130 RID: 304
	private Option __menuOption_SaltWater;

	// Token: 0x04000131 RID: 305
	[Header("BasePlayer")]
	public GameObjectRef fallDamageEffect;

	// Token: 0x04000132 RID: 306
	public GameObjectRef drownEffect;

	// Token: 0x04000133 RID: 307
	[InspectorFlags]
	public BasePlayer.PlayerFlags playerFlags;

	// Token: 0x04000134 RID: 308
	[NonSerialized]
	public PlayerEyes eyes;

	// Token: 0x04000135 RID: 309
	[NonSerialized]
	public PlayerInventory inventory;

	// Token: 0x04000136 RID: 310
	[NonSerialized]
	public PlayerBlueprints blueprints;

	// Token: 0x04000137 RID: 311
	[NonSerialized]
	public PlayerMetabolism metabolism;

	// Token: 0x04000138 RID: 312
	[NonSerialized]
	public PlayerInput input;

	// Token: 0x04000139 RID: 313
	[NonSerialized]
	public BaseMovement movement;

	// Token: 0x0400013A RID: 314
	[NonSerialized]
	public BaseCollision collision;

	// Token: 0x0400013B RID: 315
	public PlayerBelt Belt;

	// Token: 0x0400013C RID: 316
	[NonSerialized]
	private Collider triggerCollider;

	// Token: 0x0400013D RID: 317
	[NonSerialized]
	private Rigidbody physicsRigidbody;

	// Token: 0x0400013E RID: 318
	[NonSerialized]
	public ulong userID;

	// Token: 0x0400013F RID: 319
	[NonSerialized]
	public string UserIDString;

	// Token: 0x04000140 RID: 320
	protected string _displayName;

	// Token: 0x04000141 RID: 321
	private ProtectionProperties cachedProtection;

	// Token: 0x04000142 RID: 322
	private const int displayNameMaxLength = 32;

	// Token: 0x04000143 RID: 323
	public static bool oldCameraFix = false;

	// Token: 0x04000144 RID: 324
	private float lastHeadshotSoundTime;

	// Token: 0x04000145 RID: 325
	public bool clothingBlocksAiming;

	// Token: 0x04000146 RID: 326
	public float clothingMoveSpeedReduction;

	// Token: 0x04000147 RID: 327
	public float clothingWaterSpeedBonus;

	// Token: 0x04000148 RID: 328
	public float clothingAccuracyBonus;

	// Token: 0x04000149 RID: 329
	public bool equippingBlocked;

	// Token: 0x0400014A RID: 330
	[NonSerialized]
	public PlayerModel playerModel;

	// Token: 0x0400014B RID: 331
	[NonSerialized]
	public bool Frozen;

	// Token: 0x0400014C RID: 332
	[NonSerialized]
	public PlayerVoiceRecorder voiceRecorder;

	// Token: 0x0400014D RID: 333
	[NonSerialized]
	public PlayerVoiceSpeaker voiceSpeaker;

	// Token: 0x0400014E RID: 334
	[NonSerialized]
	public GameObject lookingAt;

	// Token: 0x0400014F RID: 335
	[NonSerialized]
	public BaseEntity lookingAtEntity;

	// Token: 0x04000150 RID: 336
	[NonSerialized]
	public Collider lookingAtCollider;

	// Token: 0x04000151 RID: 337
	[NonSerialized]
	public Vector3 lookingAtPoint;

	// Token: 0x04000152 RID: 338
	private const string playerModelPrefab = "assets/prefabs/player/player_model.prefab";

	// Token: 0x04000153 RID: 339
	private const string playerCollisionPrefab = "assets/prefabs/player/player_collision.prefab";

	// Token: 0x04000154 RID: 340
	private float wakeTime;

	// Token: 0x04000155 RID: 341
	private bool needsClothesRebuild;

	// Token: 0x04000156 RID: 342
	private bool wasSleeping;

	// Token: 0x04000157 RID: 343
	private uint lastClothesHash = uint.MaxValue;

	// Token: 0x04000158 RID: 344
	private static ListDictionary<ulong, BasePlayer> visiblePlayerList = new ListDictionary<ulong, BasePlayer>(8);

	// Token: 0x04000159 RID: 345
	public static int craftMode = 0;

	// Token: 0x0400015A RID: 346
	[ClientVar(ClientAdmin = true)]
	public static string lootPanelOverride = "";

	// Token: 0x0400015B RID: 347
	[NonSerialized]
	public BasePlayer.CameraMode currentViewMode;

	// Token: 0x0400015C RID: 348
	[NonSerialized]
	public BasePlayer.CameraMode selectedViewMode;

	// Token: 0x0400015D RID: 349
	private Vector3 lastRevivePoint;

	// Token: 0x0400015E RID: 350
	private Vector3 lastReviveDirection;

	// Token: 0x0400015F RID: 351
	private float nextTopologyTestTime;

	// Token: 0x04000160 RID: 352
	private float usePressTime;

	// Token: 0x04000161 RID: 353
	private float useHeldTime;

	// Token: 0x04000162 RID: 354
	private HitTest lookingAtTest;

	// Token: 0x04000163 RID: 355
	public static float lastDeathTimeClient = -1f;

	// Token: 0x04000164 RID: 356
	private const float drinkRange = 1.5f;

	// Token: 0x04000165 RID: 357
	private const float drinkMovementSpeed = 0.1f;

	// Token: 0x04000166 RID: 358
	public ulong currentTeam;

	// Token: 0x04000167 RID: 359
	public PlayerTeam clientTeam;

	// Token: 0x04000168 RID: 360
	private uint clActiveItem;

	// Token: 0x04000169 RID: 361
	[NonSerialized]
	public ModelState modelState = new ModelState
	{
		onground = true
	};

	// Token: 0x0400016A RID: 362
	[NonSerialized]
	private EntityRef mounted;

	// Token: 0x0400016B RID: 363
	private float nextSeatSwapTime;

	// Token: 0x0400016C RID: 364
	private float cachedBuildingPrivilegeTime;

	// Token: 0x0400016D RID: 365
	private BuildingPrivlidge cachedBuildingPrivilege;

	// Token: 0x0400016E RID: 366
	private int maxProjectileID;

	// Token: 0x0400016F RID: 367
	private float lastUpdateTime = float.NegativeInfinity;

	// Token: 0x04000170 RID: 368
	private float cachedThreatLevel;

	// Token: 0x04000171 RID: 369
	private float lastSentTickTime = float.NegativeInfinity;

	// Token: 0x04000172 RID: 370
	private PlayerTick lastSentTick;

	// Token: 0x04000173 RID: 371
	private float nextVisThink;

	// Token: 0x04000174 RID: 372
	private float lastTimeSeen;

	// Token: 0x04000175 RID: 373
	private bool debugPrevVisible = true;

	// Token: 0x060002C9 RID: 713 RVA: 0x00033124 File Offset: 0x00031324
	public override void GetMenuOptions(List<Option> list)
	{
		using (TimeWarning.New("BasePlayer.GetMenuOptions", 0.1f))
		{
			using (TimeWarning.New("Climb", 0.1f))
			{
				if (this.Climb_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Climb.show = true;
					this.__menuOption_Climb.showDisabled = false;
					this.__menuOption_Climb.longUseOnly = false;
					this.__menuOption_Climb.order = 0;
					this.__menuOption_Climb.icon = "upgrade";
					this.__menuOption_Climb.desc = "climb_desc";
					this.__menuOption_Climb.title = "climb";
					if (this.__menuOption_Climb.function == null)
					{
						this.__menuOption_Climb.function = new Action<BasePlayer>(this.Climb);
					}
					list.Add(this.__menuOption_Climb);
				}
			}
			using (TimeWarning.New("Drink", 0.1f))
			{
				if (this.Drink_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Drink.show = true;
					this.__menuOption_Drink.showDisabled = false;
					this.__menuOption_Drink.longUseOnly = false;
					this.__menuOption_Drink.order = 0;
					this.__menuOption_Drink.icon = "cup_water";
					this.__menuOption_Drink.desc = "drink_desc";
					this.__menuOption_Drink.title = "drink";
					if (this.__menuOption_Drink.function == null)
					{
						this.__menuOption_Drink.function = new Action<BasePlayer>(this.Drink);
					}
					list.Add(this.__menuOption_Drink);
				}
			}
			using (TimeWarning.New("InviteToTeam", 0.1f))
			{
				if (this.Invite_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_InviteToTeam.show = true;
					this.__menuOption_InviteToTeam.showDisabled = false;
					this.__menuOption_InviteToTeam.longUseOnly = false;
					this.__menuOption_InviteToTeam.order = 100;
					this.__menuOption_InviteToTeam.icon = "add";
					this.__menuOption_InviteToTeam.desc = "invite_team_sesc";
					this.__menuOption_InviteToTeam.title = "inviteToTeam";
					if (this.__menuOption_InviteToTeam.function == null)
					{
						this.__menuOption_InviteToTeam.function = new Action<BasePlayer>(this.InviteToTeam);
					}
					list.Add(this.__menuOption_InviteToTeam);
				}
			}
			using (TimeWarning.New("Menu_AssistPlayer", 0.1f))
			{
				if (this.Menu_AssistPlayer_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_AssistPlayer.show = true;
					this.__menuOption_Menu_AssistPlayer.showDisabled = false;
					this.__menuOption_Menu_AssistPlayer.longUseOnly = false;
					this.__menuOption_Menu_AssistPlayer.order = 10;
					this.__menuOption_Menu_AssistPlayer.time = 6f;
					this.__menuOption_Menu_AssistPlayer.icon = "player_assist";
					this.__menuOption_Menu_AssistPlayer.desc = "help_player_desc";
					this.__menuOption_Menu_AssistPlayer.title = "help_player";
					if (this.__menuOption_Menu_AssistPlayer.function == null)
					{
						this.__menuOption_Menu_AssistPlayer.function = new Action<BasePlayer>(this.Menu_AssistPlayer);
					}
					if (this.__menuOption_Menu_AssistPlayer.timeStart == null)
					{
						this.__menuOption_Menu_AssistPlayer.timeStart = new Action(this.Menu_AssistPlayer_TimeStart);
					}
					list.Add(this.__menuOption_Menu_AssistPlayer);
				}
			}
			using (TimeWarning.New("Menu_LootPlayer", 0.1f))
			{
				if (this.Menu_LootPlayer_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Menu_LootPlayer.show = true;
					this.__menuOption_Menu_LootPlayer.showDisabled = false;
					this.__menuOption_Menu_LootPlayer.longUseOnly = false;
					this.__menuOption_Menu_LootPlayer.order = 0;
					this.__menuOption_Menu_LootPlayer.icon = "player_loot";
					this.__menuOption_Menu_LootPlayer.desc = "help_player_desc";
					this.__menuOption_Menu_LootPlayer.title = "loot";
					if (this.__menuOption_Menu_LootPlayer.function == null)
					{
						this.__menuOption_Menu_LootPlayer.function = new Action<BasePlayer>(this.Menu_LootPlayer);
					}
					list.Add(this.__menuOption_Menu_LootPlayer);
				}
			}
			using (TimeWarning.New("Promote", 0.1f))
			{
				if (this.Promote_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_Promote.show = true;
					this.__menuOption_Promote.showDisabled = false;
					this.__menuOption_Promote.longUseOnly = false;
					this.__menuOption_Promote.order = 120;
					this.__menuOption_Promote.time = 3f;
					this.__menuOption_Promote.icon = "upgrade";
					this.__menuOption_Promote.desc = "promote_desc";
					this.__menuOption_Promote.title = "promote";
					if (this.__menuOption_Promote.function == null)
					{
						this.__menuOption_Promote.function = new Action<BasePlayer>(this.Promote);
					}
					if (this.__menuOption_Promote.timeStart == null)
					{
						this.__menuOption_Promote.timeStart = new Action(this.Menu_Promote_Start);
					}
					list.Add(this.__menuOption_Promote);
				}
			}
			using (TimeWarning.New("SaltWater", 0.1f))
			{
				if (this.SaltWater_ShowIf(LocalPlayer.Entity))
				{
					this.__menuOption_SaltWater.show = true;
					this.__menuOption_SaltWater.showDisabled = false;
					this.__menuOption_SaltWater.longUseOnly = false;
					this.__menuOption_SaltWater.order = 0;
					this.__menuOption_SaltWater.icon = "close";
					this.__menuOption_SaltWater.desc = "saltwater_desc";
					this.__menuOption_SaltWater.title = "saltwater";
					if (this.__menuOption_SaltWater.function == null)
					{
						this.__menuOption_SaltWater.function = new Action<BasePlayer>(this.SaltWater);
					}
					list.Add(this.__menuOption_SaltWater);
				}
			}
		}
		base.GetMenuOptions(list);
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060002CA RID: 714 RVA: 0x000337A8 File Offset: 0x000319A8
	public override bool HasMenuOptions
	{
		get
		{
			return this.Climb_ShowIf(LocalPlayer.Entity) || this.Drink_ShowIf(LocalPlayer.Entity) || this.Invite_ShowIf(LocalPlayer.Entity) || this.Menu_AssistPlayer_ShowIf(LocalPlayer.Entity) || this.Menu_LootPlayer_ShowIf(LocalPlayer.Entity) || this.Promote_ShowIf(LocalPlayer.Entity) || this.SaltWater_ShowIf(LocalPlayer.Entity) || base.HasMenuOptions;
		}
	}

	// Token: 0x060002CB RID: 715 RVA: 0x00033824 File Offset: 0x00031A24
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BasePlayer.OnRpcMessage", 0.1f))
		{
			if (rpc == 3534739241U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_ClearTeam ");
				}
				using (TimeWarning.New("CLIENT_ClearTeam", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CLIENT_ClearTeam(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_ClearTeam", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
			if (rpc == 744853362U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_PendingInvite ");
				}
				using (TimeWarning.New("CLIENT_PendingInvite", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CLIENT_PendingInvite(msg3);
						}
					}
					catch (Exception exception2)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_PendingInvite", true);
						Debug.LogException(exception2);
					}
				}
				return true;
			}
			if (rpc == 1029908264U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CLIENT_ReceiveTeamInfo ");
				}
				using (TimeWarning.New("CLIENT_ReceiveTeamInfo", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CLIENT_ReceiveTeamInfo(msg4);
						}
					}
					catch (Exception exception3)
					{
						Net.cl.Disconnect("RPC Error in CLIENT_ReceiveTeamInfo", true);
						Debug.LogException(exception3);
					}
				}
				return true;
			}
			if (rpc == 1588800225U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CraftMode ");
				}
				using (TimeWarning.New("CraftMode", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg5 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CraftMode(msg5);
						}
					}
					catch (Exception exception4)
					{
						Net.cl.Disconnect("RPC Error in CraftMode", true);
						Debug.LogException(exception4);
					}
				}
				return true;
			}
			if (rpc == 397787795U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: DirectionalDamage ");
				}
				using (TimeWarning.New("DirectionalDamage", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							Vector3 position = msg.read.Vector3();
							int damageType = msg.read.Int32();
							this.DirectionalDamage(position, damageType);
						}
					}
					catch (Exception exception5)
					{
						Net.cl.Disconnect("RPC Error in DirectionalDamage", true);
						Debug.LogException(exception5);
					}
				}
				return true;
			}
			if (rpc == 2811987493U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: FinishLoading ");
				}
				using (TimeWarning.New("FinishLoading", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							this.FinishLoading();
						}
					}
					catch (Exception exception6)
					{
						Net.cl.Disconnect("RPC Error in FinishLoading", true);
						Debug.LogException(exception6);
					}
				}
				return true;
			}
			if (rpc == 3278437942U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: ForcePositionTo ");
				}
				using (TimeWarning.New("ForcePositionTo", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							Vector3 position2 = msg.read.Vector3();
							this.ForcePositionTo(position2);
						}
					}
					catch (Exception exception7)
					{
						Net.cl.Disconnect("RPC Error in ForcePositionTo", true);
						Debug.LogException(exception7);
					}
				}
				return true;
			}
			if (rpc == 616869704U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: ForcePositionToParentOffset ");
				}
				using (TimeWarning.New("ForcePositionToParentOffset", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							Vector3 position3 = msg.read.Vector3();
							uint entID = msg.read.UInt32();
							this.ForcePositionToParentOffset(position3, entID);
						}
					}
					catch (Exception exception8)
					{
						Net.cl.Disconnect("RPC Error in ForcePositionToParentOffset", true);
						Debug.LogException(exception8);
					}
				}
				return true;
			}
			if (rpc == 1156888408U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: GetPerformanceReport ");
				}
				using (TimeWarning.New("GetPerformanceReport", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg6 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.GetPerformanceReport(msg6);
						}
					}
					catch (Exception exception9)
					{
						Net.cl.Disconnect("RPC Error in GetPerformanceReport", true);
						Debug.LogException(exception9);
					}
				}
				return true;
			}
			if (rpc == 1710591064U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: OnDied ");
				}
				using (TimeWarning.New("OnDied", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg7 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.OnDied(msg7);
						}
					}
					catch (Exception exception10)
					{
						Net.cl.Disconnect("RPC Error in OnDied", true);
						Debug.LogException(exception10);
					}
				}
				return true;
			}
			if (rpc == 1779218792U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: OnModelState ");
				}
				using (TimeWarning.New("OnModelState", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage data = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.OnModelState(data);
						}
					}
					catch (Exception exception11)
					{
						Net.cl.Disconnect("RPC Error in OnModelState", true);
						Debug.LogException(exception11);
					}
				}
				return true;
			}
			if (rpc == 1760224946U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: OnRespawnInformation ");
				}
				using (TimeWarning.New("OnRespawnInformation", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg8 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.OnRespawnInformation(msg8);
						}
					}
					catch (Exception exception12)
					{
						Net.cl.Disconnect("RPC Error in OnRespawnInformation", true);
						Debug.LogException(exception12);
					}
				}
				return true;
			}
			if (rpc == 2532400422U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: RecieveAchievement ");
				}
				using (TimeWarning.New("RecieveAchievement", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							string name = msg.read.String();
							this.RecieveAchievement(name);
						}
					}
					catch (Exception exception13)
					{
						Net.cl.Disconnect("RPC Error in RecieveAchievement", true);
						Debug.LogException(exception13);
					}
				}
				return true;
			}
			if (rpc == 3394540410U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: RPC_OpenLootPanel ");
				}
				using (TimeWarning.New("RPC_OpenLootPanel", 0.1f))
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
							this.RPC_OpenLootPanel(rpc2);
						}
					}
					catch (Exception exception14)
					{
						Net.cl.Disconnect("RPC Error in RPC_OpenLootPanel", true);
						Debug.LogException(exception14);
					}
				}
				return true;
			}
			if (rpc == 2932979989U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: SetHostileLength ");
				}
				using (TimeWarning.New("SetHostileLength", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage hostileLength = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetHostileLength(hostileLength);
						}
					}
					catch (Exception exception15)
					{
						Net.cl.Disconnect("RPC Error in SetHostileLength", true);
						Debug.LogException(exception15);
					}
				}
				return true;
			}
			if (rpc == 1029673592U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: SetWeaponDrawnDuration ");
				}
				using (TimeWarning.New("SetWeaponDrawnDuration", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage weaponDrawnDuration = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetWeaponDrawnDuration(weaponDrawnDuration);
						}
					}
					catch (Exception exception16)
					{
						Net.cl.Disconnect("RPC Error in SetWeaponDrawnDuration", true);
						Debug.LogException(exception16);
					}
				}
				return true;
			}
			if (rpc == 2808526587U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: StartLoading ");
				}
				using (TimeWarning.New("StartLoading", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							this.StartLoading();
						}
					}
					catch (Exception exception17)
					{
						Net.cl.Disconnect("RPC Error in StartLoading", true);
						Debug.LogException(exception17);
					}
				}
				return true;
			}
			if (rpc == 2433356304U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: UnlockedBlueprint ");
				}
				using (TimeWarning.New("UnlockedBlueprint", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage msg9 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UnlockedBlueprint(msg9);
						}
					}
					catch (Exception exception18)
					{
						Net.cl.Disconnect("RPC Error in UnlockedBlueprint", true);
						Debug.LogException(exception18);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060002CC RID: 716 RVA: 0x00004B3B File Offset: 0x00002D3B
	public override BasePlayer ToPlayer()
	{
		return this;
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060002CD RID: 717 RVA: 0x00004B3E File Offset: 0x00002D3E
	public Connection Connection
	{
		get
		{
			if (this.net != null)
			{
				return this.net.connection;
			}
			return null;
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060002CE RID: 718 RVA: 0x00004B55 File Offset: 0x00002D55
	// (set) Token: 0x060002CF RID: 719 RVA: 0x00004B78 File Offset: 0x00002D78
	public string displayName
	{
		get
		{
			if (base.isClient && Global.streamermode)
			{
				return RandomUsernames.Get(this.userID);
			}
			return this._displayName;
		}
		set
		{
			this._displayName = value;
		}
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x00004B81 File Offset: 0x00002D81
	public override Quaternion GetNetworkRotation()
	{
		if (base.isClient)
		{
			return this.eyes.bodyRotation;
		}
		return Quaternion.identity;
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x00034974 File Offset: 0x00032B74
	public string GetSubName(int maxlen = 32)
	{
		string text = this.displayName;
		if (text.Length > maxlen)
		{
			text = text.Substring(0, maxlen) + "..";
		}
		return text;
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x00004B9C File Offset: 0x00002D9C
	public bool CanInteract()
	{
		return !this.IsDead() && !this.IsSleeping() && !this.IsWounded();
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x00004BB9 File Offset: 0x00002DB9
	public override float StartHealth()
	{
		return Random.Range(50f, 60f);
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x00004BCA File Offset: 0x00002DCA
	public override float StartMaxHealth()
	{
		return 1E+38f;
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00004BCA File Offset: 0x00002DCA
	public override float MaxHealth()
	{
		return 1E+38f;
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00004BD1 File Offset: 0x00002DD1
	public override float MaxVelocity()
	{
		if (this.IsSleeping())
		{
			return 0f;
		}
		if (this.isMounted)
		{
			return this.GetMounted().MaxVelocity();
		}
		return this.GetMaxSpeed();
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x000349A8 File Offset: 0x00032BA8
	public override void InitShared()
	{
		this.Belt = new PlayerBelt(this);
		this.cachedProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		this.baseProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		this.inventory = base.GetComponent<PlayerInventory>();
		this.blueprints = base.GetComponent<PlayerBlueprints>();
		this.metabolism = base.GetComponent<PlayerMetabolism>();
		this.eyes = base.GetComponent<PlayerEyes>();
		this.input = base.GetComponent<PlayerInput>();
		base.InitShared();
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x00004BFB File Offset: 0x00002DFB
	public override void DestroyShared()
	{
		Object.Destroy(this.cachedProtection);
		Object.Destroy(this.baseProtection);
		base.DestroyShared();
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x00004C19 File Offset: 0x00002E19
	public bool InSafeZone()
	{
		return this.HasPlayerFlag(BasePlayer.PlayerFlags.SafeZone);
	}

	// Token: 0x060002DA RID: 730 RVA: 0x00034A1C File Offset: 0x00032C1C
	public static void LateClientCycle()
	{
		BasePlayer[] buffer = BasePlayer.VisiblePlayerList.Buffer;
		int count = BasePlayer.VisiblePlayerList.Count;
		for (int i = 0; i < count; i++)
		{
			BasePlayer basePlayer = buffer[i];
			if (basePlayer.playerModel)
			{
				basePlayer.playerModel.LateCycle();
			}
		}
		if (LocalPlayer.Entity && MainCamera.mainCamera != null)
		{
			if (!CameraMan.Active)
			{
				LocalPlayer.Entity.input.ApplyViewAngles();
				LocalPlayer.Entity.eyes.FrameUpdate(MainCamera.mainCamera);
			}
			foreach (BaseViewModel baseViewModel in BaseViewModel.ActiveModels)
			{
				BaseViewModel.HideViewmodel = (LocalPlayer.Entity.currentViewMode != BasePlayer.CameraMode.FirstPerson || CameraMan.Active);
				baseViewModel.OnCameraPositionChanged(MainCamera.mainCamera);
			}
		}
	}

	// Token: 0x060002DB RID: 731 RVA: 0x00034B10 File Offset: 0x00032D10
	public static void ClientCycle(float deltaTime)
	{
		BasePlayer.UpdatePlayerVisibilities();
		BasePlayer[] buffer = BasePlayer.VisiblePlayerList.Buffer;
		int count = BasePlayer.VisiblePlayerList.Count;
		for (int i = 0; i < count; i++)
		{
			BasePlayer basePlayer = buffer[i];
			if (basePlayer.IsSleeping() && basePlayer.IsAlive())
			{
				basePlayer.ClientUpdate_Sleeping();
			}
			else
			{
				basePlayer.ClientUpdate();
			}
		}
		if (LocalPlayer.Entity)
		{
			LocalPlayer.Entity.ClientUpdateLocalPlayer();
		}
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00004C26 File Offset: 0x00002E26
	public Bounds GetBounds(bool ducked)
	{
		return new Bounds(base.transform.position + this.GetOffset(ducked), this.GetSize(ducked));
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00004C4B File Offset: 0x00002E4B
	public Bounds GetBounds()
	{
		return this.GetBounds(this.modelState.ducked);
	}

	// Token: 0x060002DE RID: 734 RVA: 0x00004C5E File Offset: 0x00002E5E
	public Vector3 GetCenter(bool ducked)
	{
		return base.transform.position + this.GetOffset(ducked);
	}

	// Token: 0x060002DF RID: 735 RVA: 0x00004C77 File Offset: 0x00002E77
	public Vector3 GetCenter()
	{
		return this.GetCenter(this.modelState.ducked);
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x00004C8A File Offset: 0x00002E8A
	public Vector3 GetOffset(bool ducked)
	{
		if (ducked)
		{
			return new Vector3(0f, 0.55f, 0f);
		}
		return new Vector3(0f, 0.9f, 0f);
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x00004CB8 File Offset: 0x00002EB8
	public Vector3 GetOffset()
	{
		return this.GetOffset(this.modelState.ducked);
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x00004CCB File Offset: 0x00002ECB
	public Vector3 GetSize(bool ducked)
	{
		if (ducked)
		{
			return new Vector3(1f, 1.1f, 1f);
		}
		return new Vector3(1f, 1.8f, 1f);
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x00004CF9 File Offset: 0x00002EF9
	public Vector3 GetSize()
	{
		return this.GetSize(this.modelState.ducked);
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x00004D0C File Offset: 0x00002F0C
	public float GetHeight(bool ducked)
	{
		if (ducked)
		{
			return 1.1f;
		}
		return 1.8f;
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x00004D1C File Offset: 0x00002F1C
	public float GetHeight()
	{
		return this.GetHeight(this.modelState.ducked);
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x00004D2F File Offset: 0x00002F2F
	public float GetRadius()
	{
		return 0.5f;
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00004D36 File Offset: 0x00002F36
	public float GetJumpHeight()
	{
		return 1.5f;
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00004D3D File Offset: 0x00002F3D
	public float MaxDeployDistance(Item item)
	{
		return 8f;
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x00004D44 File Offset: 0x00002F44
	public void ClientUpdatePersistantData(PersistantPlayer data)
	{
		this.blueprints.ClientUpdate(data);
	}

	// Token: 0x060002EA RID: 746 RVA: 0x00004D52 File Offset: 0x00002F52
	public float GetMinSpeed()
	{
		return this.GetSpeed(0f, 1f);
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00004D64 File Offset: 0x00002F64
	public float GetMaxSpeed()
	{
		return this.GetSpeed(1f, 0f);
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00034B7C File Offset: 0x00032D7C
	public float GetSpeed(float running, float ducking)
	{
		float num = 1f;
		num -= this.clothingMoveSpeedReduction;
		if (this.IsSwimming())
		{
			num += this.clothingWaterSpeedBonus;
		}
		return Mathf.Lerp(Mathf.Lerp(2.8f, 5.5f, running), 1.7f, ducking) * num;
	}

	// Token: 0x060002ED RID: 749 RVA: 0x00034BC8 File Offset: 0x00032DC8
	public override void OnAttacked(HitInfo info)
	{
		float health = base.health;
		base.OnAttacked(info);
		if (info.Initiator == LocalPlayer.Entity && info.isHeadshot && (info.damageTypes.Has(DamageType.Bullet) || info.damageTypes.Has(DamageType.Arrow)) && UnityEngine.Time.realtimeSinceStartup - this.lastHeadshotSoundTime > 0.01f)
		{
			Effect.client.Run("assets/bundled/prefabs/fx/headshot_2d.prefab", LocalPlayer.Entity.eyes.gameObject);
			this.lastHeadshotSoundTime = UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x060002EE RID: 750 RVA: 0x00034C54 File Offset: 0x00032E54
	public void UpdatePlayerCollider(bool state)
	{
		if (this.triggerCollider == null)
		{
			this.triggerCollider = base.gameObject.GetComponent<Collider>();
		}
		if (this.triggerCollider.enabled != state)
		{
			base.RemoveFromTriggers();
		}
		this.triggerCollider.enabled = state;
	}

	// Token: 0x060002EF RID: 751 RVA: 0x00034CA0 File Offset: 0x00032EA0
	public void UpdatePlayerRigidbody(bool state)
	{
		if (this.physicsRigidbody == null)
		{
			this.physicsRigidbody = base.gameObject.GetComponent<Rigidbody>();
		}
		if (state)
		{
			if (this.physicsRigidbody == null)
			{
				this.physicsRigidbody = base.gameObject.AddComponent<Rigidbody>();
				this.physicsRigidbody.useGravity = false;
				this.physicsRigidbody.isKinematic = true;
				this.physicsRigidbody.mass = 1f;
				this.physicsRigidbody.interpolation = 0;
				this.physicsRigidbody.collisionDetectionMode = 0;
				return;
			}
		}
		else
		{
			base.RemoveFromTriggers();
			if (this.physicsRigidbody != null)
			{
				GameManager.Destroy(this.physicsRigidbody, 0f);
				this.physicsRigidbody = null;
			}
		}
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x00034D5C File Offset: 0x00032F5C
	public bool CanAttack()
	{
		HeldEntity heldEntity = this.GetHeldEntity();
		if (heldEntity == null)
		{
			return false;
		}
		bool flag = this.IsSwimming();
		bool flag2 = heldEntity.CanBeUsedInWater();
		if (this.movement != null)
		{
			if (this.movement.adminCheat)
			{
				return true;
			}
			if (!flag && !this.movement.IsGrounded)
			{
				return false;
			}
		}
		return !this.modelState.onLadder && (flag || this.modelState.onground) && (!flag || flag2);
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x00004D76 File Offset: 0x00002F76
	public bool OnLadder()
	{
		return this.modelState.onLadder && base.FindTrigger<TriggerLadder>();
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00004D92 File Offset: 0x00002F92
	public bool IsSwimming()
	{
		if (this.modelState != null)
		{
			return this.modelState.waterLevel >= 0.65f;
		}
		return this.WaterFactor() >= 0.65f;
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x00004DC2 File Offset: 0x00002FC2
	public bool IsHeadUnderwater()
	{
		if (this.modelState != null)
		{
			return this.modelState.waterLevel > 0.75f;
		}
		return this.WaterFactor() > 0.75f;
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x00004DEC File Offset: 0x00002FEC
	public bool IsOnGround()
	{
		if (this.movement != null)
		{
			return this.movement.IsGrounded;
		}
		return this.modelState.onground;
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00004E13 File Offset: 0x00003013
	public bool IsRunning()
	{
		if (this.movement != null)
		{
			return this.movement.IsRunning;
		}
		return this.modelState != null && this.modelState.sprinting;
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00004E44 File Offset: 0x00003044
	public bool IsDucked()
	{
		if (this.movement != null)
		{
			return this.movement.IsDucked;
		}
		return this.modelState != null && this.modelState.ducked;
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00004E75 File Offset: 0x00003075
	public void ChatMessage(string msg)
	{
		if (base.isClient)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "chat.add", new object[]
			{
				0,
				msg
			});
		}
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x00004EA2 File Offset: 0x000030A2
	public void ConsoleMessage(string msg)
	{
		if (base.isClient)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "echo " + msg, Array.Empty<object>());
		}
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x00004EC7 File Offset: 0x000030C7
	public override float PenetrationResistance(HitInfo info)
	{
		return 100f;
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00034DE4 File Offset: 0x00032FE4
	public override void ScaleDamage(HitInfo info)
	{
		if (info.UseProtection)
		{
			HitArea boneArea = info.boneArea;
			if (boneArea != (HitArea)(-1))
			{
				this.cachedProtection.Clear();
				this.cachedProtection.Add(this.inventory.containerWear.itemList, boneArea);
				this.cachedProtection.Multiply(DamageType.Arrow, Server.arrowarmor);
				this.cachedProtection.Multiply(DamageType.Bullet, Server.bulletarmor);
				this.cachedProtection.Multiply(DamageType.Slash, Server.meleearmor);
				this.cachedProtection.Multiply(DamageType.Blunt, Server.meleearmor);
				this.cachedProtection.Multiply(DamageType.Stab, Server.meleearmor);
				this.cachedProtection.Multiply(DamageType.Bleeding, Server.bleedingarmor);
				this.cachedProtection.Scale(info.damageTypes, 1f);
			}
			else
			{
				this.baseProtection.Scale(info.damageTypes, 1f);
			}
		}
		if (info.damageProperties)
		{
			info.damageProperties.ScaleDamage(info);
		}
	}

	// Token: 0x060002FB RID: 763 RVA: 0x00034EE4 File Offset: 0x000330E4
	private void UpdateMoveSpeedFromClothing()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		bool flag = false;
		bool flag2 = false;
		float num4 = 0f;
		foreach (Item item in this.inventory.containerWear.itemList)
		{
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (component)
			{
				if (component.blocksAiming)
				{
					flag = true;
				}
				if (component.blocksEquipping)
				{
					flag2 = true;
				}
				num4 += component.accuracyBonus;
				if (component.movementProperties != null)
				{
					num2 += component.movementProperties.speedReduction;
					num = Mathf.Max(num, component.movementProperties.minSpeedReduction);
					num3 += component.movementProperties.waterSpeedBonus;
				}
			}
		}
		this.clothingAccuracyBonus = num4;
		this.clothingMoveSpeedReduction = Mathf.Max(num2, num);
		this.clothingBlocksAiming = flag;
		this.clothingWaterSpeedBonus = num3;
		this.equippingBlocked = flag2;
	}

	// Token: 0x060002FC RID: 764 RVA: 0x00035000 File Offset: 0x00033200
	public virtual void UpdateProtectionFromClothing()
	{
		this.baseProtection.Clear();
		this.baseProtection.Add(this.inventory.containerWear.itemList, (HitArea)(-1));
		float num = 0.16666667f;
		for (int i = 0; i < this.baseProtection.amounts.Length; i++)
		{
			if (i != 17)
			{
				this.baseProtection.amounts[i] *= num;
			}
		}
	}

	// Token: 0x060002FD RID: 765 RVA: 0x00004ECE File Offset: 0x000030CE
	public override string Categorize()
	{
		return "player";
	}

	// Token: 0x060002FE RID: 766 RVA: 0x00035070 File Offset: 0x00033270
	public override string ToString()
	{
		if (this._name == null)
		{
			if (base.isServer)
			{
				this._name = string.Format("{1}[{0}/{2}]", (this.net != null) ? this.net.ID : 0U, this.displayName, this.userID);
			}
			else
			{
				this._name = base.ShortPrefabName;
			}
		}
		return this._name;
	}

	// Token: 0x060002FF RID: 767 RVA: 0x000350E0 File Offset: 0x000332E0
	public string GetDebugStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Entity: {0}\n", this.ToString());
		stringBuilder.AppendFormat("Name: {0}\n", this.displayName);
		stringBuilder.AppendFormat("SteamID: {0}\n", this.userID);
		foreach (object obj in Enum.GetValues(typeof(BasePlayer.PlayerFlags)))
		{
			BasePlayer.PlayerFlags playerFlags = (BasePlayer.PlayerFlags)obj;
			stringBuilder.AppendFormat("{1}: {0}\n", this.HasPlayerFlag(playerFlags), playerFlags);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00004ED5 File Offset: 0x000030D5
	public override Item GetItem(uint itemId)
	{
		if (this.inventory == null)
		{
			return null;
		}
		return this.inventory.FindItemUID(itemId);
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x06000301 RID: 769 RVA: 0x00004EF3 File Offset: 0x000030F3
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return base.Traits | BaseEntity.TraitFlag.Human | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat | BaseEntity.TraitFlag.Alive;
		}
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00004F05 File Offset: 0x00003105
	public override float WaterFactor()
	{
		if (this.isMounted)
		{
			return this.GetMounted().WaterFactorForPlayer(this);
		}
		return base.WaterFactor();
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00004F22 File Offset: 0x00003122
	public override bool ShouldInheritNetworkGroup()
	{
		return this.IsNpc || this.IsSpectating();
	}

	// Token: 0x06000304 RID: 772 RVA: 0x000351A0 File Offset: 0x000333A0
	public static bool AnyPlayersVisibleToEntity(Vector3 pos, float radius, BaseEntity source, Vector3 entityEyePos, bool ignorePlayersWithPriv = false)
	{
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		List<BasePlayer> list2 = Pool.GetList<BasePlayer>();
		global::Vis.Entities<BasePlayer>(pos, radius, list2, 131072, 2);
		bool flag = false;
		foreach (BasePlayer basePlayer in list2)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive() && (!basePlayer.IsBuildingAuthed() || !ignorePlayersWithPriv))
			{
				list.Clear();
				GamePhysics.TraceAll(new Ray(basePlayer.eyes.position, (entityEyePos - basePlayer.eyes.position).normalized), 0f, list, 9f, 1218519297, 0);
				for (int i = 0; i < list.Count; i++)
				{
					BaseEntity entity = list[i].GetEntity();
					if (entity != null && (entity == source || entity.EqualNetID(source)))
					{
						flag = true;
						break;
					}
					if (!(entity != null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		Pool.FreeList<BasePlayer>(ref list2);
		return flag;
	}

	// Token: 0x06000305 RID: 773 RVA: 0x00004F34 File Offset: 0x00003134
	public override float GetExtrapolationTime()
	{
		return Mathf.Clamp(Lerp.extrapolation, 0f, 0.1f);
	}

	// Token: 0x06000306 RID: 774 RVA: 0x000352E8 File Offset: 0x000334E8
	public override Vector3 GetLocalVelocityClient()
	{
		if (!(this.movement != null))
		{
			return base.GetLocalVelocityClient();
		}
		if (base.transform.parent != null)
		{
			return base.transform.parent.InverseTransformDirection(this.movement.CurrentVelocity());
		}
		return this.movement.CurrentVelocity();
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00035344 File Offset: 0x00033544
	protected override void ClientInit(Entity info)
	{
		this.voiceSpeaker = base.GetComponent<PlayerVoiceSpeaker>();
		this.inventory.ClientInit(this);
		base.ClientInit(info);
		if (!this.IsNpc)
		{
			SteamFriendsList.JustSeen(this.userID);
		}
		BasePlayer.UnregisterFromVisibility(this.userID);
		BasePlayer.RegisterForVisibility(this);
		bool flag = this.userID == ((global::Client.Steam == null) ? 0UL : global::Client.Steam.SteamId);
		if (global::Client.IsPlayingDemo)
		{
			flag = false;
		}
		if (!flag)
		{
			this.InitRemotePlayer();
			return;
		}
		if (LocalPlayer.Entity != null && LocalPlayer.Entity != this)
		{
			Debug.LogError("Local player is being set up multiple times!!");
			return;
		}
		this.InitLocalPlayer();
	}

	// Token: 0x06000308 RID: 776 RVA: 0x000353F0 File Offset: 0x000335F0
	private void CreatePlayerModel()
	{
		if (this.playerModel != null)
		{
			Debug.LogWarning("playerModel isn't null - but we're creating a new one!");
		}
		GameObject gameObject = base.gameManager.CreatePrefab("assets/prefabs/player/player_model.prefab", base.transform.position, Quaternion.identity, false);
		BaseEntityChild.Setup(gameObject, this);
		gameObject.AwakeFromInstantiate();
		base.SetModel(gameObject.GetComponent<Model>());
		this.playerModel = gameObject.GetComponent<PlayerModel>();
		this.playerModel.skinType = ((BasePlayer.GetRandomFloatBasedOnUserID(this.userID, 4332UL) > 0.5f) ? 1 : 0);
		this.playerModel.skinColor = BasePlayer.GetRandomFloatBasedOnUserID(this.userID, 5977UL);
		this.playerModel.skinNumber = BasePlayer.GetRandomFloatBasedOnUserID(this.userID, 3975UL);
		this.playerModel.meshNumber = BasePlayer.GetRandomFloatBasedOnUserID(this.userID, 2647UL);
		this.playerModel.hairNumber = BasePlayer.GetRandomFloatBasedOnUserID(this.userID, 6338UL);
		this.playerModel.showSash = this.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash);
		this.playerModel.isLocalPlayer = this.IsLocalPlayer();
		this.playerModel.drawShadowOnly = !this.shouldDrawBody;
		this.playerModel.UpdateModelState(this.modelState);
		this.playerModel.AlwaysAnimate(this.IsLocalPlayer());
		this.playerModel.UpdateSkeleton((int)this.userID);
		this.playerModel.IsNpc = this.IsNpc;
		this.CL_ClothingChanged();
	}

	// Token: 0x06000309 RID: 777 RVA: 0x0003557C File Offset: 0x0003377C
	private void CreatePlayerCollision()
	{
		GameObject gameObject = base.gameManager.CreatePrefab("assets/prefabs/player/player_collision.prefab", base.transform.position, Quaternion.identity, false);
		BaseEntityChild.Setup(gameObject, this);
		gameObject.AwakeFromInstantiate();
		this.collision = gameObject.GetComponent<BaseCollision>();
		this.collision.Owner = this;
		this.collision.model = this.model;
		this.collision.transform.parent = this.playerModel.transform;
	}

	// Token: 0x0600030A RID: 778 RVA: 0x000355FC File Offset: 0x000337FC
	private void CreatePlayerMovement()
	{
		if (global::Client.IsPlayingDemo)
		{
			return;
		}
		GameObject gameObject = base.gameManager.CreatePrefab("assets/prefabs/player/player_movement.prefab", base.transform.position, Quaternion.identity, true);
		Assert.IsTrue(this.movement == null, "movement setup! InitLocalPlayer called multiple times!");
		this.movement = gameObject.GetComponent<BaseMovement>();
		this.movement.SetParent(base.transform.parent);
		this.movement.Init(this);
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00035678 File Offset: 0x00033878
	internal void InitLocalPlayer()
	{
		Assert.IsTrue(LocalPlayer.Entity != this, "LocalPlayer setup! InitLocalPlayer called multiple times!");
		base.gameObject.name = "LocalPlayer";
		LocalPlayer.Entity = this;
		this.blueprints.ClientInit();
		this.UpdatePlayerCollider(true);
		this.UpdatePlayerRigidbody(true);
		this.voiceRecorder = base.GetComponent<PlayerVoiceRecorder>();
		this.voiceRecorder.Init();
		this.CreatePlayerMovement();
		this.metabolism.ClientInit(this);
		LocalPlayer.OnInventoryChanged();
		SingletonComponent<SteamClient>.Instance.SendUpdatedInventory();
		this.CreatePlayerModel();
	}

	// Token: 0x0600030C RID: 780 RVA: 0x00004F4A File Offset: 0x0000314A
	internal void InitRemotePlayer()
	{
		this.UpdatePlayerCollider(false);
		this.UpdatePlayerRigidbody(false);
		this.CreatePlayerModel();
		this.CreatePlayerCollision();
	}

	// Token: 0x0600030D RID: 781 RVA: 0x00004F66 File Offset: 0x00003166
	public bool HasLocalControls()
	{
		return this.IsLocalPlayer() && !this.IsSpectating() && !this.IsDead() && !this.IsSleeping() && !this.IsWounded();
	}

	// Token: 0x0600030E RID: 782 RVA: 0x00004F9B File Offset: 0x0000319B
	public override void SetNetworkPosition(Vector3 vPos)
	{
		if (this.HasLocalControls())
		{
			return;
		}
		if (this.isMounted)
		{
			return;
		}
		base.SetNetworkPosition(vPos);
	}

	// Token: 0x0600030F RID: 783 RVA: 0x00004FB6 File Offset: 0x000031B6
	public override void SetNetworkRotation(Quaternion qRot)
	{
		if (this.HasLocalControls())
		{
			return;
		}
		if (this.IsSpectating())
		{
			return;
		}
		this.eyes.bodyRotation = qRot;
	}

	// Token: 0x06000310 RID: 784 RVA: 0x00035708 File Offset: 0x00033908
	protected override void DoClientDestroy()
	{
		BasePlayer.UnregisterFromVisibility(this.userID);
		if (this.inventory != null)
		{
			this.inventory.DoDestroy();
		}
		base.DoClientDestroy();
		if (this.movement != null)
		{
			GameObject gameObject = this.movement.gameObject;
			this.movement = null;
			gameObject.BroadcastOnParentDestroying();
			GameManager.Destroy(gameObject, 0f);
		}
		if (this.collision != null)
		{
			GameObject gameObject2 = this.collision.gameObject;
			this.collision = null;
			gameObject2.BroadcastOnParentDestroying();
			GameManager.client.Retire(gameObject2);
		}
		if (this.playerModel != null)
		{
			GameObject gameObject3 = this.playerModel.gameObject;
			this.playerModel = null;
			base.SetModel(null);
			gameObject3.BroadcastOnParentDestroying();
			GameManager.client.Retire(gameObject3);
		}
	}

	// Token: 0x06000311 RID: 785 RVA: 0x000357DC File Offset: 0x000339DC
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		if (base.isClient)
		{
			if (this.playerModel)
			{
				bool showSash = this.playerModel.showSash;
				this.playerModel.showSash = this.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash);
				if (showSash != this.playerModel.showSash)
				{
					this.needsClothesRebuild = true;
				}
			}
			if (this.wasSleeping && !this.IsSleeping())
			{
				this.wakeTime = UnityEngine.Time.realtimeSinceStartup;
			}
			this.wasSleeping = this.IsSleeping();
			if (this.movement != null)
			{
				this.movement.SetParent(base.transform.parent);
			}
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x06000312 RID: 786 RVA: 0x00004FD6 File Offset: 0x000031D6
	public float TimeAwake
	{
		get
		{
			if (!this.IsSleeping())
			{
				return UnityEngine.Time.realtimeSinceStartup - this.wakeTime;
			}
			return 0f;
		}
	}

	// Token: 0x06000313 RID: 787 RVA: 0x00035888 File Offset: 0x00033A88
	public void CL_ClothingChanged()
	{
		if (!this.playerModel)
		{
			return;
		}
		uint num = this.inventory.containerWear.ContentsHash();
		if (this.lastClothesHash != num)
		{
			this.needsClothesRebuild = true;
		}
		this.lastClothesHash = num;
		this.UpdateClothingItems(this.playerModel.multiMesh);
		if (this == LocalPlayer.Entity)
		{
			if (BaseViewModel.ActiveModel != null && this.needsClothesRebuild)
			{
				BaseViewModel.ActiveModel.OnClothingChanged();
			}
			if (this.inventory.containerWear.itemList.Count >= 3)
			{
				this.RecieveAchievement("EQUIP_CLOTHING");
			}
		}
	}

	// Token: 0x06000314 RID: 788 RVA: 0x0003592C File Offset: 0x00033B2C
	private void RebuildWorldModel()
	{
		if (!this.playerModel)
		{
			return;
		}
		this.playerModel.Clear();
		this.SetDefaultFootstepEffects();
		if (this.inventory.containerWear != null)
		{
			foreach (Item item in this.inventory.containerWear.itemList)
			{
				ItemModWearable component = item.info.GetComponent<ItemModWearable>();
				if (component)
				{
					component.OnDressModel(item, this.playerModel);
				}
			}
		}
		this.playerModel.Rebuild(true);
		this.UpdateClothingItems(this.playerModel.multiMesh);
		this.UpdateHolsterOffsets();
	}

	// Token: 0x06000315 RID: 789 RVA: 0x00004FF2 File Offset: 0x000031F2
	private void SetDefaultFootstepEffects()
	{
		if (!this.playerModel)
		{
			return;
		}
		FootstepEffects component = this.playerModel.GetComponent<FootstepEffects>();
		component.footstepEffectName = "footstep/barefoot";
		component.jumpStartEffectName = "jump-start/barefoot";
		component.jumpLandEffectName = "jump-land/barefoot";
	}

	// Token: 0x06000316 RID: 790 RVA: 0x000359F4 File Offset: 0x00033BF4
	public override void OnSignal(BaseEntity.Signal signal, string arg)
	{
		base.OnSignal(signal, arg);
		if (!this.isVisible)
		{
			return;
		}
		switch (signal)
		{
		case BaseEntity.Signal.Attack:
			if (this.playerModel)
			{
				this.playerModel.Attack();
				return;
			}
			break;
		case BaseEntity.Signal.Alt_Attack:
			if (this.playerModel)
			{
				this.playerModel.AltAttack();
				return;
			}
			break;
		case BaseEntity.Signal.DryFire:
		case BaseEntity.Signal.Deploy:
		case BaseEntity.Signal.Relax:
			break;
		case BaseEntity.Signal.Reload:
			if (this.playerModel)
			{
				this.playerModel.Reload();
				return;
			}
			break;
		case BaseEntity.Signal.Flinch_Head:
			if (this.playerModel)
			{
				this.playerModel.Flinch(0U);
				return;
			}
			break;
		case BaseEntity.Signal.Flinch_Chest:
			if (this.playerModel)
			{
				this.playerModel.Flinch(1U);
				return;
			}
			break;
		case BaseEntity.Signal.Flinch_Stomach:
			if (this.playerModel)
			{
				this.playerModel.Flinch(2U);
				return;
			}
			break;
		case BaseEntity.Signal.Flinch_RearHead:
			if (this.playerModel)
			{
				this.playerModel.Flinch(3U);
				return;
			}
			break;
		case BaseEntity.Signal.Flinch_RearTorso:
			if (this.playerModel)
			{
				this.playerModel.Flinch(4U);
				return;
			}
			break;
		case BaseEntity.Signal.Throw:
			if (this.playerModel)
			{
				this.playerModel.Throw();
				return;
			}
			break;
		case BaseEntity.Signal.Gesture:
			if (this.playerModel)
			{
				this.playerModel.Gesture(arg);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000317 RID: 791 RVA: 0x00035B5C File Offset: 0x00033D5C
	public override Transform FindBone(string strName)
	{
		if (this.playerModel && strName != string.Empty)
		{
			Transform transform = this.playerModel.FindBone(strName);
			if (transform)
			{
				return transform;
			}
		}
		return base.FindBone(strName);
	}

	// Token: 0x06000318 RID: 792 RVA: 0x0000502D File Offset: 0x0000322D
	public override bool ShouldLerp()
	{
		return !this.IsLocalPlayer() && !this.IsDead() && !this.IsSleeping() && !this.IsWounded();
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000319 RID: 793 RVA: 0x00005058 File Offset: 0x00003258
	public static BufferList<BasePlayer> VisiblePlayerList
	{
		get
		{
			return BasePlayer.visiblePlayerList.Values;
		}
	}

	// Token: 0x0600031A RID: 794 RVA: 0x00035BA4 File Offset: 0x00033DA4
	public static void ClearVisibility()
	{
		BasePlayer[] buffer = BasePlayer.VisiblePlayerList.Buffer;
		int count = BasePlayer.VisiblePlayerList.Count;
		for (int i = 0; i < count; i++)
		{
			buffer[i].UnregisterFromCulling();
		}
		BasePlayer.visiblePlayerList.Clear();
	}

	// Token: 0x0600031B RID: 795 RVA: 0x00005064 File Offset: 0x00003264
	public static void RegisterForVisibility(BasePlayer player)
	{
		if (!BasePlayer.visiblePlayerList.Contains(player.userID))
		{
			BasePlayer.visiblePlayerList.Add(player.userID, player);
		}
		player.RegisterForCulling();
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00035BE8 File Offset: 0x00033DE8
	public static void UnregisterFromVisibility(ulong userID)
	{
		BasePlayer basePlayer;
		if (BasePlayer.visiblePlayerList.TryGetValue(userID, ref basePlayer))
		{
			BasePlayer.visiblePlayerList.Remove(userID);
			basePlayer.UnregisterFromCulling();
		}
	}

	// Token: 0x0600031D RID: 797 RVA: 0x00035C18 File Offset: 0x00033E18
	public static BasePlayer FindByID_Clientside(ulong userID)
	{
		BasePlayer result;
		BasePlayer.visiblePlayerList.TryGetValue(userID, ref result);
		return result;
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00035C34 File Offset: 0x00033E34
	public static BasePlayer Find_Clientside(string strNameOrIDOrIP)
	{
		BasePlayer[] buffer = BasePlayer.VisiblePlayerList.Buffer;
		int count = BasePlayer.VisiblePlayerList.Count;
		for (int i = 0; i < count; i++)
		{
			BasePlayer basePlayer = buffer[i];
			if (basePlayer.UserIDString == strNameOrIDOrIP)
			{
				return basePlayer;
			}
		}
		for (int j = 0; j < count; j++)
		{
			BasePlayer basePlayer2 = buffer[j];
			if (basePlayer2.displayName.StartsWith(strNameOrIDOrIP, 1))
			{
				return basePlayer2;
			}
		}
		for (int k = 0; k < count; k++)
		{
			BasePlayer basePlayer3 = buffer[k];
			if (basePlayer3.net != null && basePlayer3.net.connection != null && basePlayer3.net.connection.ipaddress == strNameOrIDOrIP)
			{
				return basePlayer3;
			}
		}
		return null;
	}

	// Token: 0x0600031F RID: 799 RVA: 0x00035CEC File Offset: 0x00033EEC
	public static float GetRandomFloatBasedOnUserID(ulong steamid, ulong seed)
	{
		Random.State state = Random.state;
		Random.InitState((int)(seed + steamid));
		float result = Random.Range(0f, 1f);
		Random.state = state;
		return result;
	}

	// Token: 0x06000320 RID: 800 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool DisplayHealthInfo(BasePlayer player)
	{
		return false;
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00005092 File Offset: 0x00003292
	public override bool ShouldDestroyWithGroup()
	{
		return !this.IsLocalPlayer() && base.ShouldDestroyWithGroup();
	}

	// Token: 0x06000322 RID: 802 RVA: 0x000050A4 File Offset: 0x000032A4
	[BaseEntity.RPC_Client]
	public void GetPerformanceReport(BaseEntity.RPCMessage msg)
	{
		base.ServerRPC<int, int, float, int>("PerformanceReport", (int)Performance.report.memoryAllocations, (int)Performance.report.memoryCollections, Performance.report.frameRateAverage, (int)UnityEngine.Time.realtimeSinceStartup);
	}

	// Token: 0x06000323 RID: 803 RVA: 0x000050D7 File Offset: 0x000032D7
	public override void OnBecameRagdoll(Ragdoll rdoll)
	{
		base.OnBecameRagdoll(rdoll);
		this.lifestate = BaseCombatEntity.LifeState.Dead;
		this.deathTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x000050F2 File Offset: 0x000032F2
	public override void OnVoiceData(byte[] data)
	{
		this.voiceSpeaker.Receive(data);
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00005100 File Offset: 0x00003300
	[BaseEntity.RPC_Client]
	public void RecieveAchievement(string name)
	{
		if (!GameInfo.HasAchievements)
		{
			return;
		}
		Facepunch.Steamworks.Client.Instance.Achievements.Trigger(name, true);
	}

	// Token: 0x06000326 RID: 806 RVA: 0x00035D20 File Offset: 0x00033F20
	[BaseEntity.RPC_Client]
	public void CraftMode(BaseEntity.RPCMessage msg)
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		if (this != LocalPlayer.Entity)
		{
			return;
		}
		int num = msg.read.Int32();
		if (BasePlayer.craftMode != num)
		{
			BasePlayer.craftMode = num;
			LocalPlayer.OnInventoryChanged();
			UIBlueprints.Refresh();
		}
	}

	// Token: 0x06000327 RID: 807 RVA: 0x0000511C File Offset: 0x0000331C
	public override bool CanBeLooted(BasePlayer player)
	{
		return !(player == this) && (this.IsWounded() || this.IsSleeping());
	}

	// Token: 0x06000328 RID: 808 RVA: 0x00005139 File Offset: 0x00003339
	[BaseEntity.Menu("loot", "Loot")]
	[BaseEntity.Menu.Description("help_player_desc", "Access this player's inventory. Steal from them or dress them up all pretty,")]
	[BaseEntity.Menu.Icon("player_loot")]
	[BaseEntity.Menu.ShowIf("Menu_LootPlayer_ShowIf")]
	public void Menu_LootPlayer(BasePlayer player)
	{
		base.ServerRPC("RPC_LootPlayer");
		UIInventory.OpenLoot("player_corpse");
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00005150 File Offset: 0x00003350
	public bool Menu_LootPlayer_ShowIf(BasePlayer player)
	{
		return this.CanBeLooted(player);
	}

	// Token: 0x0600032A RID: 810 RVA: 0x00035D70 File Offset: 0x00033F70
	[BaseEntity.RPC_Client]
	private void RPC_OpenLootPanel(BaseEntity.RPCMessage rpc)
	{
		if (global::Client.IsPlayingDemo)
		{
			return;
		}
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		UIInventory.OpenLoot((LocalPlayer.Entity.IsAdmin && BasePlayer.lootPanelOverride != "") ? BasePlayer.lootPanelOverride : rpc.read.String());
		this.PlayOpenSound();
	}

	// Token: 0x0600032B RID: 811 RVA: 0x00035DD0 File Offset: 0x00033FD0
	private void PlayOpenSound()
	{
		StorageContainer storageContainer = LocalPlayer.Entity.inventory.loot.GetClientEntity() as StorageContainer;
		if (storageContainer != null && storageContainer.openSound != null)
		{
			SoundManager.PlayOneshot(storageContainer.openSound, storageContainer.gameObject, false, default(Vector3));
		}
	}

	// Token: 0x0600032C RID: 812 RVA: 0x00005159 File Offset: 0x00003359
	public bool InFirstPersonMode()
	{
		return this.currentViewMode == BasePlayer.CameraMode.FirstPerson;
	}

	// Token: 0x0600032D RID: 813 RVA: 0x00035E2C File Offset: 0x0003402C
	public void UpdateViewMode()
	{
		BasePlayer.CameraMode idealViewMode = this.idealViewMode;
		if (idealViewMode == this.currentViewMode)
		{
			return;
		}
		this.currentViewMode = idealViewMode;
		this.OnViewModeChanged();
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x0600032E RID: 814 RVA: 0x00005164 File Offset: 0x00003364
	internal BasePlayer.CameraMode idealViewMode
	{
		get
		{
			if (this.IsSpectating())
			{
				return this.selectedViewMode;
			}
			if (this.IsSleeping())
			{
				return BasePlayer.CameraMode.Eyes;
			}
			if (this.IsWounded())
			{
				return BasePlayer.CameraMode.Eyes;
			}
			if (this.IsDead())
			{
				return BasePlayer.CameraMode.Eyes;
			}
			return this.selectedViewMode;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x0600032F RID: 815 RVA: 0x00005199 File Offset: 0x00003399
	internal bool shouldDrawBody
	{
		get
		{
			return !this.IsLocalPlayer() || (!this.IsDead() && ((SingletonComponent<CameraMan>.Instance && SingletonComponent<CameraMan>.Instance.isActiveAndEnabled) || this.currentViewMode != BasePlayer.CameraMode.FirstPerson));
		}
	}

	// Token: 0x06000330 RID: 816 RVA: 0x00035E58 File Offset: 0x00034058
	public virtual void OnViewModeChanged()
	{
		if (this.playerModel)
		{
			this.playerModel.drawShadowOnly = !this.shouldDrawBody;
			this.playerModel.Rebuild(true);
			this.UpdateClothingItems(this.playerModel.multiMesh);
		}
		GlobalMessages.OnViewModeChanged();
	}

	// Token: 0x06000331 RID: 817 RVA: 0x00035EA8 File Offset: 0x000340A8
	public void ModifyCamera()
	{
		Item activeItem = this.Belt.GetActiveItem();
		if (activeItem != null)
		{
			HeldEntity heldEntity = activeItem.GetHeldEntity() as HeldEntity;
			if (heldEntity)
			{
				heldEntity.ModifyCamera();
			}
		}
	}

	// Token: 0x06000332 RID: 818 RVA: 0x000051D4 File Offset: 0x000033D4
	[BaseEntity.Menu.Description("help_player_desc", "Stop this player from dying by helping them to their feet")]
	[BaseEntity.Menu.Icon("player_assist")]
	[BaseEntity.Menu("help_player", "Help Player", Time = 6f, OnStart = "Menu_AssistPlayer_TimeStart", Order = 10)]
	[BaseEntity.Menu.ShowIf("Menu_AssistPlayer_ShowIf")]
	public void Menu_AssistPlayer(BasePlayer player)
	{
		base.ServerRPC("RPC_Assist");
	}

	// Token: 0x06000333 RID: 819 RVA: 0x000051E1 File Offset: 0x000033E1
	public void Menu_AssistPlayer_TimeStart()
	{
		base.ServerRPC("RPC_KeepAlive");
	}

	// Token: 0x06000334 RID: 820 RVA: 0x00035EE0 File Offset: 0x000340E0
	public bool Menu_AssistPlayer_ShowIf(BasePlayer player)
	{
		if (player == this)
		{
			return false;
		}
		if (!this.IsWounded())
		{
			return false;
		}
		if (player.lookingAtEntity == this)
		{
			this.lastRevivePoint = player.lookingAtPoint;
			this.lastReviveDirection = player.eyes.HeadForward();
		}
		else
		{
			if (Vector3.Angle(this.lastReviveDirection, player.eyes.HeadForward()) > 10f)
			{
				return false;
			}
			if (!GamePhysics.LineOfSight(this.lastRevivePoint, player.eyes.position, 2162688, 0.1f))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000335 RID: 821 RVA: 0x00035F74 File Offset: 0x00034174
	protected void UpdateClothesIfNeeded()
	{
		if (!this.needsClothesRebuild)
		{
			return;
		}
		if (!this.IsAlive())
		{
			return;
		}
		if (!this.isVisible)
		{
			return;
		}
		this.needsClothesRebuild = false;
		this.RebuildWorldModel();
		this.UpdateProtectionFromClothing();
		this.UpdateMoveSpeedFromClothing();
		if (this.IsLocalPlayer())
		{
			if (SingletonComponent<uiPlayerPreview>.Instance)
			{
				SingletonComponent<uiPlayerPreview>.Instance.UpdateFrom(this.playerModel);
			}
			bool flag = false;
			bool flag2 = false;
			foreach (Item item in this.inventory.containerWear.itemList)
			{
				ItemModWearable component = item.info.GetComponent<ItemModWearable>();
				if (component != null && component.occlusionType != UIBlackoutOverlay.blackoutType.NONE)
				{
					if (component.occlusionType == UIBlackoutOverlay.blackoutType.SNORKELGOGGLE)
					{
						flag2 = true;
						flag = false;
						break;
					}
					if (component.occlusionType == UIBlackoutOverlay.blackoutType.HELMETSLIT)
					{
						flag = true;
						flag2 = false;
						break;
					}
					break;
				}
			}
			UIBlackoutOverlay uiblackoutOverlay = UIBlackoutOverlay.Get(UIBlackoutOverlay.blackoutType.HELMETSLIT);
			if (uiblackoutOverlay)
			{
				uiblackoutOverlay.SetAlpha((float)(flag ? 1 : 0));
			}
			UIBlackoutOverlay uiblackoutOverlay2 = UIBlackoutOverlay.Get(UIBlackoutOverlay.blackoutType.SNORKELGOGGLE);
			if (uiblackoutOverlay2)
			{
				uiblackoutOverlay2.SetAlpha((float)(flag2 ? 1 : 0));
				WaterOverlay.goggles = flag2;
			}
		}
		GlobalMessages.OnClothingChanged();
	}

	// Token: 0x06000336 RID: 822 RVA: 0x000051EE File Offset: 0x000033EE
	public override void MakeVisible()
	{
		base.MakeVisible();
		if (this.playerModel)
		{
			this.playerModel.ApplyVisibility(this.isVisible, this.isAnimatorVisible, this.isShadowVisible);
		}
	}

	// Token: 0x06000337 RID: 823 RVA: 0x000360B0 File Offset: 0x000342B0
	protected void ClientUpdate_Sleeping()
	{
		if ((long)(UnityEngine.Time.frameCount % 5) != (long)((ulong)(this.net.ID % 5U)) && !base.HasParent())
		{
			return;
		}
		this.UpdateClothesIfNeeded();
		if (this.playerModel)
		{
			this.playerModel.ApplyVisibility(this.isVisible, this.isAnimatorVisible, this.isShadowVisible);
			if (!this.isVisible)
			{
				this.playerModel.position = base.transform.position;
				this.playerModel.rotation = base.transform.rotation;
				this.playerModel.visible = false;
				this.playerModel.UpdatePosition();
				this.playerModel.UpdateRotation();
				return;
			}
			this.playerModel.gameObject.SetActive(this.ragdoll == null);
			this.playerModel.position = base.transform.position;
			this.playerModel.rotation = base.transform.rotation;
			this.playerModel.visible = true;
			this.playerModel.isIncapacitated = false;
			this.modelState.lookDir = base.transform.forward;
			this.playerModel.UpdateModelState(this.modelState);
			this.playerModel.FrameUpdate(this.IsWounded());
		}
	}

	// Token: 0x06000338 RID: 824 RVA: 0x00036200 File Offset: 0x00034400
	protected void ClientUpdate()
	{
		this.UpdateClothesIfNeeded();
		if (this.movement)
		{
			this.movement.gameObject.SetActive(this.IsAlive() && !this.IsSpectating());
		}
		if (this.isMounted)
		{
			this.GetMounted().UpdatePlayerPosition(this);
		}
		if (this.playerModel)
		{
			this.playerModel.UpdateLocalVelocity(base.GetLocalVelocity(), base.transform.parent);
			this.playerModel.ApplyVisibility(this.isVisible, this.isAnimatorVisible, this.isShadowVisible);
			if (!this.isVisible)
			{
				this.playerModel.position = base.transform.position;
				this.playerModel.rotation = Quaternion.Euler(new Vector3(0f, this.eyes.rotation.eulerAngles.y, 0f));
				this.playerModel.visible = false;
				this.playerModel.UpdatePosition();
				this.playerModel.UpdateRotation();
			}
			else
			{
				bool flag = this.IsAlive() && !this.IsSpectating();
				if (this.playerModel.gameObject.activeSelf != flag)
				{
					if (!flag)
					{
						Effect.Strip(this.playerModel.gameObject);
					}
					this.playerModel.gameObject.SetActive(flag);
				}
				this.playerModel.position = base.transform.position;
				this.playerModel.rotation = Quaternion.Euler(new Vector3(0f, this.eyes.rotation.eulerAngles.y, 0f));
				this.playerModel.visible = !this.IsDead();
				this.playerModel.isIncapacitated = this.IsWounded();
				HeldEntity heldEntity = this.GetHeldEntity();
				if (heldEntity)
				{
					heldEntity.UpdatePlayerModel(this.playerModel);
				}
				else
				{
					this.playerModel.SetHoldType(null);
					this.playerModel.SetAimSounds(null, null);
				}
				this.playerModel.LookAngles = Quaternion.LookRotation(this.eyes.BodyForward(), this.isMounted ? base.transform.up : Vector3.up);
				this.playerModel.UpdateModelState(this.modelState);
				this.playerModel.FrameUpdate(this.IsWounded());
				if (this.isMounted)
				{
					BaseMountable baseMountable = this.GetMounted();
					baseMountable.UpdatePlayerRotation(this);
					baseMountable.UpdatePlayerModel(this);
				}
			}
		}
		if (this.voiceSpeaker)
		{
			this.voiceSpeaker.ClientFrame(this);
			if (this.playerModel)
			{
				this.playerModel.voiceVolume = this.voiceSpeaker.currentVolume;
			}
		}
	}

	// Token: 0x06000339 RID: 825 RVA: 0x000364C0 File Offset: 0x000346C0
	public void UpdateClothingItems(SkinnedMultiMesh multiMesh)
	{
		if (this.playerModel == null)
		{
			return;
		}
		if (this.inventory == null)
		{
			return;
		}
		foreach (Item item in this.inventory.containerWear.itemList)
		{
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (!(component == null))
			{
				SkinnedMultiMesh.Part[] array = multiMesh.FindParts(component.entityPrefab.resourcePath);
				if (array != null)
				{
					List<IItemUpdate> list = Pool.GetList<IItemUpdate>();
					SkinnedMultiMesh.Part[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i].gameObject.GetComponentsInChildren<IItemUpdate>(true, list);
						foreach (IItemUpdate itemUpdate in list)
						{
							itemUpdate.OnItemUpdate(item);
						}
					}
					Pool.FreeList<IItemUpdate>(ref list);
				}
			}
		}
	}

	// Token: 0x0600033A RID: 826 RVA: 0x000365E0 File Offset: 0x000347E0
	public void UpdateHolsterOffsets()
	{
		if (this.children != null)
		{
			foreach (BaseEntity baseEntity in this.children)
			{
				if (!(baseEntity == null))
				{
					HeldEntity component = baseEntity.GetComponent<HeldEntity>();
					if (!(component == null))
					{
						component.UpdateHolsteredOffset();
					}
				}
			}
		}
	}

	// Token: 0x0600033B RID: 827 RVA: 0x00005220 File Offset: 0x00003420
	public bool IsLocalPlayer()
	{
		return this == LocalPlayer.Entity;
	}

	// Token: 0x0600033C RID: 828 RVA: 0x00036654 File Offset: 0x00034854
	public void ClientUpdateLocalPlayer()
	{
		this.ClientInput(this.input.state);
		float num = 1f / ConVar.Client.tickrate;
		bool flag = this.input.state.current.buttons != this.input.state.previous.buttons;
		bool flag2 = UnityEngine.Time.realtimeSinceStartup - this.lastSentTickTime > num;
		if (flag || flag2)
		{
			this.ClientTick();
		}
		this.UpdateViewMode();
		BaseViewModel.HideViewmodel = (this.currentViewMode > BasePlayer.CameraMode.FirstPerson);
		LODComponent.UpdateDynamicOccludees();
		this.voiceRecorder.ClientFrame(this);
		if (this.IsSleeping())
		{
			this.BlockJump(0.5f);
		}
		if (this.Frozen)
		{
			return;
		}
		ContextMenuUI.FrameUpdate(this);
		ProgressBarUI.FrameUpdate(this);
		if (this.movement)
		{
			this.movement.FrameUpdate(this, this.modelState);
		}
		this.HeldEntityViewAngles();
		this.MountableOverrideViewAngles();
		this.input.ApplyViewAngles();
		this.eyes.FrameUpdate(MainCamera.mainCamera);
		this.modelState.lookDir = this.eyes.HeadForward();
		this.Belt.FrameUpdate();
		if (this.playerModel)
		{
			this.playerModel.UpdateLocalVelocity(base.GetLocalVelocity(), base.transform.parent);
			this.playerModel.position = base.transform.position;
			this.playerModel.rotation = Quaternion.Euler(new Vector3(0f, this.eyes.rotation.eulerAngles.y, 0f));
			this.playerModel.UpdatePosition();
		}
		if (!this.UpdateLookingAt(0f, false))
		{
			this.UpdateLookingAt(Mathf.Clamp(ConVar.Client.lookatradius, 0f, 0.4f), true);
		}
		this.HeldEntityFrame();
		if (this.isMounted)
		{
			base.transform.localRotation = this.GetMounted().GetMountedRotation();
		}
		else
		{
			base.transform.rotation = Quaternion.Euler(new Vector3(0f, this.eyes.rotation.eulerAngles.y, 0f));
		}
		if (this.IsAdmin || this.IsDeveloper)
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.F3))
			{
				this.selectedViewMode++;
				if (this.selectedViewMode > BasePlayer.CameraMode.Eyes)
				{
					this.selectedViewMode = BasePlayer.CameraMode.FirstPerson;
				}
			}
		}
		else
		{
			this.selectedViewMode = BasePlayer.CameraMode.FirstPerson;
		}
		if (this.HasPlayerFlag(BasePlayer.PlayerFlags.ThirdPersonViewmode))
		{
			this.selectedViewMode = BasePlayer.CameraMode.ThirdPerson;
		}
		if (this.HasPlayerFlag(BasePlayer.PlayerFlags.EyesViewmode))
		{
			this.selectedViewMode = BasePlayer.CameraMode.Eyes;
		}
		this.UpdateTopologyStats();
	}

	// Token: 0x0600033D RID: 829 RVA: 0x000368F8 File Offset: 0x00034AF8
	public void UpdateTopologyStats()
	{
		if (!TerrainMeta.TopologyMap)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextTopologyTestTime)
		{
			return;
		}
		this.nextTopologyTestTime = UnityEngine.Time.realtimeSinceStartup + 5f;
		if ((TerrainMeta.TopologyMap.GetTopology(base.transform.position) & 2048) != 0)
		{
			Analytics.TimeOnRoad += 5f;
		}
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0000522D File Offset: 0x0000342D
	private void ClientTick()
	{
		if (Net.cl == null || !Net.cl.IsConnected())
		{
			return;
		}
		if (LoadingScreen.isOpen)
		{
			return;
		}
		this.SendClientTick();
	}

	// Token: 0x0600033F RID: 831 RVA: 0x00005251 File Offset: 0x00003451
	public void DoMovement()
	{
		if (!this.IsLocalPlayer())
		{
			return;
		}
		if (!this.HasLocalControls())
		{
			return;
		}
		if (this.movement && !this.Frozen)
		{
			this.movement.DoFixedUpdate(this.modelState);
		}
	}

	// Token: 0x06000340 RID: 832 RVA: 0x00036960 File Offset: 0x00034B60
	private void MountableOverrideViewAngles()
	{
		BaseMountable baseMountable = this.GetMounted();
		if (baseMountable != null)
		{
			baseMountable.OverrideViewAngles(this);
		}
	}

	// Token: 0x06000341 RID: 833 RVA: 0x0000528B File Offset: 0x0000348B
	public virtual void BlockSprint(float duration = 0.2f)
	{
		if (this.movement)
		{
			this.movement.BlockSprint(duration);
		}
	}

	// Token: 0x06000342 RID: 834 RVA: 0x000052A6 File Offset: 0x000034A6
	public virtual void BlockJump(float duration = 0.5f)
	{
		if (this.movement)
		{
			this.movement.BlockJump(duration);
		}
	}

	// Token: 0x06000343 RID: 835 RVA: 0x00036984 File Offset: 0x00034B84
	internal virtual void ClientInput(InputState state)
	{
		this.input.FrameUpdate();
		this.modelState.ducked = false;
		this.modelState.sprinting = false;
		this.modelState.aiming = false;
		this.modelState.sleeping = this.IsSleeping();
		this.modelState.waterLevel = this.WaterFactor();
		this.voiceRecorder.ClientInput(state);
		if (this.HasLocalControls() && !NeedsKeyboard.AnyActive())
		{
			this.Belt.ClientInput(state);
		}
		else
		{
			UIInventory.Close();
			MapInterface.SetOpen(false);
		}
		if (this.Frozen)
		{
			return;
		}
		this.HeldEntityInput();
		if (this.movement)
		{
			using (TimeWarning.New("movement.ClientInput", 0.1f))
			{
				this.movement.ClientInput(state, this.modelState);
			}
		}
		using (TimeWarning.New("UseAction", 0.1f))
		{
			this.UseAction(state);
		}
		if (Buttons.Chat.JustPressed && this.input.hadInputBuffer && ConVar.Graphics.chat && Chat.enabled)
		{
			Chat.open();
			Rust.GC.Reset(1f);
		}
		using (TimeWarning.New("MapInterface Update", 0.1f))
		{
			MapInterface.DoPlayerUpdate();
			MapInterface.SetOpen(Buttons.Map.IsDown);
		}
	}

	// Token: 0x06000344 RID: 836 RVA: 0x00036B14 File Offset: 0x00034D14
	internal void UseAction(InputState state)
	{
		if (!state.IsDown(BUTTON.USE))
		{
			if (UnityEngine.Time.realtimeSinceStartup - this.usePressTime < ConVar.Input.holdtime)
			{
				this.usePressTime = 0f;
				this.QuickUse();
			}
			if (this.useHeldTime != 0f)
			{
				this.UseStop();
				this.useHeldTime = 0f;
			}
			return;
		}
		if (state.WasJustPressed(BUTTON.USE))
		{
			this.usePressTime = UnityEngine.Time.realtimeSinceStartup;
		}
		if (this.usePressTime == 0f)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup - this.usePressTime > ConVar.Input.holdtime)
		{
			this.usePressTime = 0f;
			this.LongUse();
			this.useHeldTime = UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x06000345 RID: 837 RVA: 0x00036BC8 File Offset: 0x00034DC8
	internal BaseEntity GetInteractionEntity()
	{
		BaseEntity baseEntity = null;
		if (!this.IsSpectating() && !this.IsDead())
		{
			baseEntity = this.lookingAtEntity;
			if (this.lookingAtCollider && this.lookingAtCollider.gameObject.CompareTag("Not Player Usable"))
			{
				baseEntity = null;
			}
		}
		if (baseEntity != null && !GameMenu.Util.GetInfo(baseEntity.gameObject, this).IsValid)
		{
			baseEntity = null;
		}
		if (baseEntity == null)
		{
			baseEntity = this;
			if (baseEntity != null && !GameMenu.Util.GetInfo(baseEntity.gameObject, this).IsValid)
			{
				baseEntity = null;
			}
		}
		return baseEntity;
	}

	// Token: 0x06000346 RID: 838 RVA: 0x00036C64 File Offset: 0x00034E64
	internal void QuickUse()
	{
		if (this.IsWounded())
		{
			return;
		}
		if (this.HeldItemUse())
		{
			return;
		}
		BaseEntity interactionEntity = this.GetInteractionEntity();
		if (interactionEntity == null)
		{
			return;
		}
		interactionEntity.gameObject.SendMessage("OnUse", this, SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x06000347 RID: 839 RVA: 0x00036CA8 File Offset: 0x00034EA8
	internal void LongUse()
	{
		if (this.IsWounded())
		{
			return;
		}
		BaseEntity interactionEntity = this.GetInteractionEntity();
		if (interactionEntity == null)
		{
			return;
		}
		interactionEntity.gameObject.SendMessage("OnUseHeld", this, SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x06000348 RID: 840 RVA: 0x00036CE4 File Offset: 0x00034EE4
	internal void UseStop()
	{
		if (this.IsWounded())
		{
			return;
		}
		BaseEntity interactionEntity = this.GetInteractionEntity();
		if (interactionEntity == null)
		{
			return;
		}
		interactionEntity.gameObject.SendMessage("OnUseStopped", this, SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x06000349 RID: 841 RVA: 0x00036D20 File Offset: 0x00034F20
	internal bool UpdateLookingAt(float radius, bool includeSecondaryEntities)
	{
		this.lookingAt = base.gameObject;
		this.lookingAtEntity = this;
		this.lookingAtCollider = this.triggerCollider;
		this.lookingAtPoint = this.eyes.position + this.eyes.HeadForward() * 8f;
		List<TraceInfo> list = Pool.GetList<TraceInfo>();
		if (this.lookingAtTest == null)
		{
			this.lookingAtTest = new HitTest();
		}
		else
		{
			this.lookingAtTest.Clear();
		}
		using (TimeWarning.New("Traces", 0.1f))
		{
			this.lookingAtTest.AttackRay = this.eyes.HeadRay();
			this.lookingAtTest.MaxDistance = 2f;
			this.lookingAtTest.Radius = radius;
			this.lookingAtTest.Forgiveness = radius;
			this.lookingAtTest.ignoreEntity = this;
			this.lookingAtTest.type = HitTest.Type.Use;
			GameTrace.TraceAll(this.lookingAtTest, list, 229731073);
			for (int i = 0; i < list.Count; i++)
			{
				TraceInfo traceInfo = list[i];
				if (traceInfo.distance > 2f)
				{
					traceInfo.distance = 100f;
				}
				else if (traceInfo.entity == null)
				{
					traceInfo.distance = 100f;
				}
				else if (!traceInfo.entity.HasMenuOptions && !(traceInfo.entity is WorldItem) && !(traceInfo.entity is CollectibleEntity) && !(traceInfo.entity is IOEntity))
				{
					if (!includeSecondaryEntities)
					{
						traceInfo.distance = 100f;
					}
					else
					{
						traceInfo.distance += 1f;
					}
				}
				list[i] = traceInfo;
			}
		}
		using (TimeWarning.New("Sort", 0.1f))
		{
			list.Sort((TraceInfo a, TraceInfo b) => a.distance.CompareTo(b.distance));
		}
		int j = 0;
		while (j < list.Count)
		{
			TraceInfo traceInfo2 = list[j];
			if (traceInfo2.distance < 100f && this.CheckLookingAtVisible(this.lookingAtTest, traceInfo2))
			{
				traceInfo2.UpdateHitTest(this.lookingAtTest);
				this.lookingAtPoint = this.lookingAtTest.HitPointWorld();
				this.lookingAt = this.lookingAtTest.gameObject;
				this.lookingAtCollider = this.lookingAtTest.collider;
				this.lookingAtPoint = this.lookingAtTest.AttackRay.origin + this.lookingAtTest.AttackRay.direction * this.lookingAtTest.HitDistance;
				this.lookingAtEntity = this.lookingAt.ToBaseEntity();
				if (this.lookingAtEntity)
				{
					this.lookingAt = this.lookingAtEntity.gameObject;
					break;
				}
				break;
			}
			else
			{
				j++;
			}
		}
		Pool.FreeList<TraceInfo>(ref list);
		return this.lookingAtEntity != this;
	}

	// Token: 0x0600034A RID: 842 RVA: 0x00037050 File Offset: 0x00035250
	private bool CheckLookingAtVisible(HitTest test, TraceInfo trace)
	{
		Vector3 origin = test.AttackRay.origin;
		Vector3 a = trace.point - origin;
		float magnitude = a.magnitude;
		if (magnitude < Mathf.Epsilon)
		{
			return true;
		}
		Vector3 direction = a / magnitude;
		RaycastHit hit;
		if (Physics.Raycast(new Ray(origin, direction), ref hit, magnitude + 0.01f, 1218519041))
		{
			BaseEntity entity = hit.GetEntity();
			return entity == trace.entity || (entity != null && trace.entity.GetParentEntity() && trace.entity.GetParentEntity().EqualNetID(entity) && hit.collider != null && hit.collider.gameObject.layer == 13);
		}
		return true;
	}

	// Token: 0x0600034B RID: 843 RVA: 0x000052C1 File Offset: 0x000034C1
	[BaseEntity.RPC_Client]
	private void OnDied(BaseEntity.RPCMessage msg)
	{
		UIInventory.Close();
		MapInterface.SetOpen(false);
		LocalPlayer.LastDeathTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x0600034C RID: 844 RVA: 0x000052D8 File Offset: 0x000034D8
	[BaseEntity.RPC_Client]
	private void OnRespawnInformation(BaseEntity.RPCMessage msg)
	{
		Debug.Log("Got Respawn Information");
		UIDeathScreen.OnRespawnInformation(RespawnInformation.Deserialize(msg.read));
		UIInventory.Close();
		MapInterface.SetOpen(false);
	}

	// Token: 0x0600034D RID: 845 RVA: 0x00037120 File Offset: 0x00035320
	public void OnLand(float fVelocity)
	{
		Effect.client.Run("assets/bundled/prefabs/fx/screen_land.prefab", default(Vector3), default(Vector3), default(Vector3));
		if (fVelocity < -8f)
		{
			base.ServerRPC<float>("OnPlayerLanded", fVelocity);
		}
	}

	// Token: 0x0600034E RID: 846 RVA: 0x000052FF File Offset: 0x000034FF
	[BaseEntity.RPC_Client]
	private void StartLoading()
	{
		MapInterface.ResetMap();
		MusicManager.RaiseIntensityTo(0.75f, 999);
		LoadingScreen.Update("Receiving Data");
		LoadingScreen.Show();
	}

	// Token: 0x0600034F RID: 847 RVA: 0x00005324 File Offset: 0x00003524
	[BaseEntity.RPC_Client]
	private void FinishLoading()
	{
		base.StartCoroutine(this.FinishedLoadingRoutine());
	}

	// Token: 0x06000350 RID: 848 RVA: 0x00005333 File Offset: 0x00003533
	private IEnumerator FinishedLoadingRoutine()
	{
		LoadingScreen.Update("Processing Data");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		UIInventory.Close();
		MapInterface.SetOpen(false);
		HostileNote.unhostileTime = 0f;
		HostileNote.weaponDrawnDuration = 0f;
		MusicManager.RaiseIntensityTo(1f, 999);
		this.eyes.FrameUpdate(MainCamera.mainCamera);
		DecorSpawn.RefreshAll(false);
		FoliageGrid.RefreshAll(false);
		LoadBalancer.ProcessAll();
		LODGrid.RefreshAll();
		RendererGrid.RefreshAll();
		LoadingScreen.Update("Cleaning Up");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		Rust.GC.Collect();
		LoadingScreen.Update("Entering Game");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		MainMenuSystem.Hide();
		LoadingScreen.Hide();
		base.ServerRPC("ClientLoadingComplete");
		yield break;
	}

	// Token: 0x06000351 RID: 849 RVA: 0x00005342 File Offset: 0x00003542
	[BaseEntity.RPC_Client]
	private void DirectionalDamage(Vector3 position, int damageType)
	{
		if (SingletonComponent<UIUnderlay>.Instance)
		{
			SingletonComponent<UIUnderlay>.Instance.DirectionalDamage(position);
		}
		Rust.GC.Pause(5f);
	}

	// Token: 0x06000352 RID: 850 RVA: 0x00005365 File Offset: 0x00003565
	[BaseEntity.RPC_Client]
	public void UnlockedBlueprint(BaseEntity.RPCMessage msg)
	{
		LocalPlayer.OnInventoryChanged();
		UIBlueprints.Refresh();
	}

	// Token: 0x06000353 RID: 851 RVA: 0x00037168 File Offset: 0x00035368
	private Vector3 GetWaterDrinkingPoint()
	{
		Vector3 result;
		using (TimeWarning.New("GetWaterDrinkingPoint", 0.1f))
		{
			RaycastHit raycastHit;
			if (!Physics.Raycast(this.eyes.BodyRay(), ref raycastHit, 1.5f, 8388624))
			{
				result = Vector3.zero;
			}
			else
			{
				result = raycastHit.point - new Vector3(0f, 0.5f, 0f);
			}
		}
		return result;
	}

	// Token: 0x06000354 RID: 852 RVA: 0x000371EC File Offset: 0x000353EC
	[BaseEntity.Menu.Icon("cup_water")]
	[BaseEntity.Menu.ShowIf("Drink_ShowIf")]
	[BaseEntity.Menu("drink", "Drink")]
	[BaseEntity.Menu.Description("drink_desc", "Drink water")]
	public void Drink(BasePlayer player)
	{
		if (!player.metabolism.CanConsume())
		{
			return;
		}
		Vector3 waterDrinkingPoint = player.GetWaterDrinkingPoint();
		if (waterDrinkingPoint == Vector3.zero)
		{
			return;
		}
		if (!WaterResource.IsFreshWater(waterDrinkingPoint))
		{
			return;
		}
		player.metabolism.MarkConsumption();
		base.ServerRPC<Vector3>("SV_Drink", waterDrinkingPoint);
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0003723C File Offset: 0x0003543C
	public bool Drink_ShowIf(BasePlayer player)
	{
		if (this.movement == null)
		{
			return false;
		}
		if (this.movement.CurrentMoveSpeed() > 0.1f)
		{
			return false;
		}
		Vector3 waterDrinkingPoint = player.GetWaterDrinkingPoint();
		if (waterDrinkingPoint == Vector3.zero)
		{
			return false;
		}
		if (!WaterResource.IsFreshWater(waterDrinkingPoint))
		{
			return false;
		}
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(waterDrinkingPoint);
		return waterInfo.isValid && waterInfo.overallDepth >= 0.01f;
	}

	// Token: 0x06000356 RID: 854 RVA: 0x00002ECE File Offset: 0x000010CE
	[BaseEntity.Menu.ShowIf("SaltWater_ShowIf")]
	[BaseEntity.Menu.Description("saltwater_desc", "Can't Drink This")]
	[BaseEntity.Menu("saltwater", "Salt Water")]
	[BaseEntity.Menu.Icon("close")]
	public void SaltWater(BasePlayer player)
	{
	}

	// Token: 0x06000357 RID: 855 RVA: 0x000372B0 File Offset: 0x000354B0
	public bool SaltWater_ShowIf(BasePlayer player)
	{
		if (this.movement == null)
		{
			return false;
		}
		if (this.movement.CurrentMoveSpeed() > 0.1f)
		{
			return false;
		}
		Vector3 waterDrinkingPoint = player.GetWaterDrinkingPoint();
		if (waterDrinkingPoint == Vector3.zero)
		{
			return false;
		}
		if (WaterResource.IsFreshWater(waterDrinkingPoint))
		{
			return false;
		}
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(waterDrinkingPoint);
		return waterInfo.isValid && waterInfo.overallDepth >= 0.05f;
	}

	// Token: 0x06000358 RID: 856 RVA: 0x00037324 File Offset: 0x00035524
	[BaseEntity.Menu.Description("climb_desc", "Climb")]
	[BaseEntity.Menu.Icon("upgrade")]
	[BaseEntity.Menu.ShowIf("Climb_ShowIf")]
	[BaseEntity.Menu("climb", "Climb")]
	public void Climb(BasePlayer player)
	{
		if (!player.isMounted)
		{
			return;
		}
		BaseVehicle mountedVehicle = player.GetMountedVehicle();
		if (mountedVehicle && !mountedVehicle.mountChaining)
		{
			return;
		}
		bool flag = false;
		RaycastHit hit = default(RaycastHit);
		foreach (RaycastHit raycastHit in Physics.SphereCastAll(this.eyes.BodyRay(), 0.5f, 3f, 262144, 2))
		{
			if (raycastHit.collider.GetComponent<TriggerLadder>() && !Physics.Raycast(this.eyes.BodyRay(), raycastHit.distance, 1218519041))
			{
				flag = true;
				hit = raycastHit;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		bool arg = false;
		BaseEntity entity = hit.GetEntity();
		Vector3 vector = hit.collider.transform.position;
		vector.y = hit.point.y;
		LadderMinMountHeight componentInChildren = hit.collider.gameObject.GetComponentInChildren<LadderMinMountHeight>();
		if (componentInChildren && vector.y < componentInChildren.transform.position.y)
		{
			vector.y = componentInChildren.transform.position.y;
		}
		vector += hit.collider.transform.forward * (player.GetRadius() + 0.1f);
		if (entity)
		{
			arg = true;
			vector = entity.transform.InverseTransformPoint(vector);
		}
		base.ServerRPC<bool, Vector3, uint>("RPC_StartClimb", arg, vector, entity.net.ID);
	}

	// Token: 0x06000359 RID: 857 RVA: 0x000374B8 File Offset: 0x000356B8
	public bool Climb_ShowIf(BasePlayer player)
	{
		if (!player.isMounted)
		{
			return false;
		}
		BaseVehicle mountedVehicle = player.GetMountedVehicle();
		if (mountedVehicle && !mountedVehicle.mountChaining)
		{
			return false;
		}
		foreach (RaycastHit raycastHit in Physics.RaycastAll(this.eyes.BodyRay(), 3f, 262144, 2))
		{
			if (raycastHit.collider.GetComponent<TriggerLadder>() && !Physics.Raycast(this.eyes.BodyRay(), raycastHit.distance, 1218519041))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600035A RID: 858 RVA: 0x00005371 File Offset: 0x00003571
	public bool HasPlayerFlag(BasePlayer.PlayerFlags f)
	{
		return (this.playerFlags & f) == f;
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x0600035B RID: 859 RVA: 0x0000537E File Offset: 0x0000357E
	public bool IsReceivingSnapshot
	{
		get
		{
			return this.HasPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot);
		}
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x0600035C RID: 860 RVA: 0x00002D44 File Offset: 0x00000F44
	public bool IsAdmin
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600035D RID: 861 RVA: 0x00005387 File Offset: 0x00003587
	public bool IsDeveloper
	{
		get
		{
			return this.HasPlayerFlag(BasePlayer.PlayerFlags.IsDeveloper);
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x0600035E RID: 862 RVA: 0x00005394 File Offset: 0x00003594
	public bool IsAiming
	{
		get
		{
			return this.HasPlayerFlag(BasePlayer.PlayerFlags.Aiming);
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x0600035F RID: 863 RVA: 0x000053A1 File Offset: 0x000035A1
	public bool IsFlying
	{
		get
		{
			return this.modelState != null && this.modelState.flying;
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x06000360 RID: 864 RVA: 0x000053B8 File Offset: 0x000035B8
	public bool IsConnected
	{
		get
		{
			return base.isClient && this.HasPlayerFlag(BasePlayer.PlayerFlags.Connected);
		}
	}

	// Token: 0x06000361 RID: 865 RVA: 0x00037550 File Offset: 0x00035750
	[BaseEntity.RPC_Client]
	public void CLIENT_ReceiveTeamInfo(BaseEntity.RPCMessage msg)
	{
		PlayerTeam playerTeam = PlayerTeam.Deserialize(msg.read);
		playerTeam.ShouldPool = false;
		this.clientTeam = playerTeam;
		TeamUI.dirty = true;
		SingletonComponent<MapInterface>.Instance.ClientTeamUpdated();
	}

	// Token: 0x06000362 RID: 866 RVA: 0x000053CF File Offset: 0x000035CF
	[BaseEntity.RPC_Client]
	public void CLIENT_ClearTeam(BaseEntity.RPCMessage msg)
	{
		this.clientTeam = null;
		TeamUI.dirty = true;
		SingletonComponent<MapInterface>.Instance.ClientTeamUpdated();
	}

	// Token: 0x06000363 RID: 867 RVA: 0x00037588 File Offset: 0x00035788
	[BaseEntity.RPC_Client]
	public void CLIENT_PendingInvite(BaseEntity.RPCMessage msg)
	{
		string text = msg.read.String();
		ulong num = msg.read.UInt64();
		if (TeamUI.pendingTeamID == 0UL && this.clientTeam == null && num != 0UL && LocalPlayer.Entity != null)
		{
			LocalPlayer.Entity.ChatMessage("You have been invited to " + text + "'s team");
			Effect.client.Run("assets/bundled/prefabs/fx/invite_notice.prefab", LocalPlayer.Entity.eyes.position, default(Vector3), default(Vector3));
		}
		TeamUI.pendingTeamLeaderName = text;
		TeamUI.pendingTeamID = num;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x00037620 File Offset: 0x00035820
	[BaseEntity.Menu.Icon("add")]
	[BaseEntity.Menu("inviteToTeam", "Invite To Team", Order = 100)]
	[BaseEntity.Menu.Description("invite_team_sesc", "Invite this player to your team")]
	[BaseEntity.Menu.ShowIf("Invite_ShowIf")]
	public void InviteToTeam(BasePlayer player)
	{
		BasePlayer component = LocalPlayer.Entity.lookingAtEntity.GetComponent<BasePlayer>();
		if (component == null)
		{
			return;
		}
		if (component.currentTeam != 0UL)
		{
			return;
		}
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "sendinvite", new object[]
		{
			component.userID
		});
	}

	// Token: 0x06000365 RID: 869 RVA: 0x000053E8 File Offset: 0x000035E8
	[BaseEntity.Menu.Description("promote_desc", "Promote this team member to leader")]
	[BaseEntity.Menu.Icon("upgrade")]
	[BaseEntity.Menu("promote", "Promote To Leader", Order = 120, Time = 3f, OnStart = "Menu_Promote_Start")]
	[BaseEntity.Menu.ShowIf("Promote_ShowIf")]
	public void Promote(BasePlayer player)
	{
		if (LocalPlayer.Entity.lookingAtEntity.GetComponent<BasePlayer>() != null)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client, "promote", Array.Empty<object>());
		}
	}

	// Token: 0x06000366 RID: 870 RVA: 0x00002ECE File Offset: 0x000010CE
	public void Menu_Promote_Start()
	{
	}

	// Token: 0x06000367 RID: 871 RVA: 0x00037674 File Offset: 0x00035874
	public bool Promote_ShowIf(BasePlayer player)
	{
		if (!RelationshipManager.TeamsEnabled())
		{
			return false;
		}
		if (!BasePlayer.LocalPlayerIsLeader())
		{
			return false;
		}
		if (this == LocalPlayer.Entity)
		{
			return false;
		}
		BasePlayer component = LocalPlayer.Entity.lookingAtEntity.GetComponent<BasePlayer>();
		return !(component == null) && !component.IsDead() && (component.currentTeam == LocalPlayer.Entity.currentTeam && LocalPlayer.Entity.currentTeam != 0UL);
	}

	// Token: 0x06000368 RID: 872 RVA: 0x00005416 File Offset: 0x00003616
	public static bool LocalPlayerIsLeader()
	{
		return LocalPlayer.Entity != null && LocalPlayer.Entity.clientTeam != null && LocalPlayer.Entity.clientTeam.teamLeader == LocalPlayer.Entity.userID;
	}

	// Token: 0x06000369 RID: 873 RVA: 0x000376E8 File Offset: 0x000358E8
	public bool Invite_ShowIf(BasePlayer player)
	{
		if (!RelationshipManager.TeamsEnabled())
		{
			return false;
		}
		if (this.currentTeam != 0UL)
		{
			return false;
		}
		BasePlayer entity = LocalPlayer.Entity;
		return !(entity == null) && !(this == LocalPlayer.Entity) && !this.IsWounded() && this.IsAlive() && !this.IsSleeping() && !this.IsNpc && entity.currentTeam != 0UL && LocalPlayer.Entity.clientTeam != null && LocalPlayer.Entity.clientTeam.teamLeader == LocalPlayer.Entity.userID;
	}

	// Token: 0x0600036A RID: 874 RVA: 0x00037780 File Offset: 0x00035980
	public HeldEntity GetHeldEntity()
	{
		if (!base.isClient)
		{
			return null;
		}
		if (this.IsLocalPlayer())
		{
			Item heldItem = this.GetHeldItem();
			if (heldItem == null)
			{
				return null;
			}
			HeldEntity heldEntity = heldItem.GetHeldEntity() as HeldEntity;
			if (heldEntity == null)
			{
				return null;
			}
			if (heldEntity.GetOwnerPlayer() != this)
			{
				return null;
			}
			return heldEntity;
		}
		else
		{
			if (this.clActiveItem == 0U)
			{
				return null;
			}
			Item item = this.inventory.containerBelt.FindItemByUID(this.clActiveItem);
			if (item == null)
			{
				return null;
			}
			return item.GetHeldEntity() as HeldEntity;
		}
	}

	// Token: 0x0600036B RID: 875 RVA: 0x00037808 File Offset: 0x00035A08
	public bool IsHoldingEntity<T>()
	{
		HeldEntity heldEntity = this.GetHeldEntity();
		return !(heldEntity == null) && heldEntity is T;
	}

	// Token: 0x0600036C RID: 876 RVA: 0x0000544E File Offset: 0x0000364E
	private Item GetHeldItem()
	{
		return this.Belt.GetActiveItem();
	}

	// Token: 0x0600036D RID: 877 RVA: 0x00037830 File Offset: 0x00035A30
	public uint GetHeldItemID()
	{
		if (!this.IsLocalPlayer())
		{
			return this.clActiveItem;
		}
		Item heldItem = this.GetHeldItem();
		if (heldItem == null)
		{
			return 0U;
		}
		return heldItem.uid;
	}

	// Token: 0x0600036E RID: 878 RVA: 0x00037860 File Offset: 0x00035A60
	private void HeldEntityViewAngles()
	{
		Assert.IsTrue(this.IsLocalPlayer(), "Not Local Player!");
		HeldEntity heldEntity = this.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		if (heldEntity.GetOwnerPlayer() != this)
		{
			return;
		}
		heldEntity.EditViewAngles();
	}

	// Token: 0x0600036F RID: 879 RVA: 0x000378A4 File Offset: 0x00035AA4
	private void HeldEntityFrame()
	{
		Assert.IsTrue(this.IsLocalPlayer(), "Not Local Player!");
		using (TimeWarning.New("HeldEntityFrame", 0.1f))
		{
			HeldEntity heldEntity = this.GetHeldEntity();
			if (!(heldEntity == null))
			{
				if (!(heldEntity.GetOwnerPlayer() != this))
				{
					heldEntity.OnFrame();
				}
			}
		}
	}

	// Token: 0x06000370 RID: 880 RVA: 0x00037918 File Offset: 0x00035B18
	private void HeldEntityInput()
	{
		Assert.IsTrue(this.IsLocalPlayer(), "Not Local Player!");
		using (TimeWarning.New("HeldEntityInput", 0.1f))
		{
			HeldEntity heldEntity = this.GetHeldEntity();
			if (!(heldEntity == null))
			{
				if (!(heldEntity.GetOwnerPlayer() != this))
				{
					heldEntity.OnInput();
				}
			}
		}
	}

	// Token: 0x06000371 RID: 881 RVA: 0x0003798C File Offset: 0x00035B8C
	private bool HeldItemUse()
	{
		Assert.IsTrue(this.IsLocalPlayer(), "Not Local Player!");
		HeldEntity heldEntity = this.GetHeldEntity();
		if (heldEntity == null)
		{
			return false;
		}
		heldEntity.GetOwnerPlayer() != this;
		return false;
	}

	// Token: 0x06000372 RID: 882 RVA: 0x000379CC File Offset: 0x00035BCC
	public void HeldEntityStart()
	{
		Assert.IsTrue(this.IsLocalPlayer(), "Not Local Player!");
		HeldEntity heldEntity = this.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		heldEntity.OnDeploy();
	}

	// Token: 0x06000373 RID: 883 RVA: 0x00037A00 File Offset: 0x00035C00
	public void HeldEntityEnd()
	{
		Assert.IsTrue(this.IsLocalPlayer(), "Not Local Player!");
		HeldEntity heldEntity = this.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		heldEntity.OnHolster();
	}

	// Token: 0x06000374 RID: 884 RVA: 0x0000545B File Offset: 0x0000365B
	[BaseEntity.RPC_Client]
	private void OnModelState(BaseEntity.RPCMessage data)
	{
		ModelState.Deserialize(data.read, this.modelState, false);
		this.OnModelStateChanged();
	}

	// Token: 0x06000375 RID: 885 RVA: 0x00005476 File Offset: 0x00003676
	private void OnModelStateChanged()
	{
		if (!this.IsLocalPlayer() && this.playerModel != null)
		{
			this.playerModel.UpdateModelState(this.modelState);
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x06000376 RID: 886 RVA: 0x0000549F File Offset: 0x0000369F
	public bool isMounted
	{
		get
		{
			return this.mounted.IsValid(base.isServer);
		}
	}

	// Token: 0x06000377 RID: 887 RVA: 0x000054B2 File Offset: 0x000036B2
	public BaseMountable GetMounted()
	{
		return this.mounted.Get(base.isServer) as BaseMountable;
	}

	// Token: 0x06000378 RID: 888 RVA: 0x00037A34 File Offset: 0x00035C34
	public BaseVehicle GetMountedVehicle()
	{
		BaseMountable baseMountable = this.GetMounted();
		if (baseMountable == null)
		{
			return null;
		}
		return baseMountable.VehicleParent();
	}

	// Token: 0x06000379 RID: 889 RVA: 0x000054CA File Offset: 0x000036CA
	public void MarkSwapSeat()
	{
		this.nextSeatSwapTime = UnityEngine.Time.time + 0.75f;
	}

	// Token: 0x0600037A RID: 890 RVA: 0x000054DD File Offset: 0x000036DD
	public bool SwapSeatCooldown()
	{
		return UnityEngine.Time.time < this.nextSeatSwapTime;
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00037A5C File Offset: 0x00035C5C
	public void ClientUpdateMounted(uint id)
	{
		if (this.mounted.uid == id)
		{
			return;
		}
		BaseMountable baseMountable = this.GetMounted();
		this.mounted.uid = id;
		BaseMountable baseMountable2 = this.GetMounted();
		if (baseMountable)
		{
			baseMountable.PlayerDismounted(this);
		}
		if (baseMountable2)
		{
			baseMountable2.PlayerMounted(this);
		}
	}

	// Token: 0x0600037C RID: 892 RVA: 0x000054EC File Offset: 0x000036EC
	public bool IsSleeping()
	{
		return this.HasPlayerFlag(BasePlayer.PlayerFlags.Sleeping);
	}

	// Token: 0x0600037D RID: 893 RVA: 0x000054F6 File Offset: 0x000036F6
	public bool IsSpectating()
	{
		return this.HasPlayerFlag(BasePlayer.PlayerFlags.Spectating);
	}

	// Token: 0x0600037E RID: 894 RVA: 0x00005500 File Offset: 0x00003700
	public bool IsRelaxed()
	{
		return this.HasPlayerFlag(BasePlayer.PlayerFlags.Relaxed);
	}

	// Token: 0x0600037F RID: 895 RVA: 0x00037AB0 File Offset: 0x00035CB0
	public override BuildingPrivlidge GetBuildingPrivilege()
	{
		if (base.isClient)
		{
			if (UnityEngine.Time.time - this.cachedBuildingPrivilegeTime > 1f)
			{
				this.cachedBuildingPrivilegeTime = UnityEngine.Time.time;
				this.cachedBuildingPrivilege = base.GetBuildingPrivilege(base.WorldSpaceBounds());
			}
			return this.cachedBuildingPrivilege;
		}
		return base.GetBuildingPrivilege();
	}

	// Token: 0x06000380 RID: 896 RVA: 0x00037B04 File Offset: 0x00035D04
	public bool CanBuild()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		return buildingPrivilege == null || buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x06000381 RID: 897 RVA: 0x00037B2C File Offset: 0x00035D2C
	public bool CanBuild(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(new OBB(position, rotation, bounds));
		return buildingPrivilege == null || buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x06000382 RID: 898 RVA: 0x00037B5C File Offset: 0x00035D5C
	public bool CanBuild(OBB obb)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(obb);
		return buildingPrivilege == null || buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00037B84 File Offset: 0x00035D84
	public bool IsBuildingBlocked()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		return !(buildingPrivilege == null) && !buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00037BB0 File Offset: 0x00035DB0
	public bool IsBuildingBlocked(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(new OBB(position, rotation, bounds));
		return !(buildingPrivilege == null) && !buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x06000385 RID: 901 RVA: 0x00037BE4 File Offset: 0x00035DE4
	public bool IsBuildingBlocked(OBB obb)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(obb);
		return !(buildingPrivilege == null) && !buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x06000386 RID: 902 RVA: 0x00037C10 File Offset: 0x00035E10
	public bool IsBuildingAuthed()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		return !(buildingPrivilege == null) && buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x06000387 RID: 903 RVA: 0x00037C38 File Offset: 0x00035E38
	public bool IsBuildingAuthed(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(new OBB(position, rotation, bounds));
		return !(buildingPrivilege == null) && buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x06000388 RID: 904 RVA: 0x00037C68 File Offset: 0x00035E68
	public bool IsBuildingAuthed(OBB obb)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(obb);
		return !(buildingPrivilege == null) && buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x06000389 RID: 905 RVA: 0x0000550D File Offset: 0x0000370D
	public bool CanPlaceBuildingPrivilege()
	{
		return this.GetBuildingPrivilege() == null;
	}

	// Token: 0x0600038A RID: 906 RVA: 0x0000551B File Offset: 0x0000371B
	public bool CanPlaceBuildingPrivilege(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		return base.GetBuildingPrivilege(new OBB(position, rotation, bounds)) == null;
	}

	// Token: 0x0600038B RID: 907 RVA: 0x00005531 File Offset: 0x00003731
	public bool CanPlaceBuildingPrivilege(OBB obb)
	{
		return base.GetBuildingPrivilege(obb) == null;
	}

	// Token: 0x0600038C RID: 908 RVA: 0x00037C90 File Offset: 0x00035E90
	public int NewProjectileID()
	{
		int result = this.maxProjectileID + 1;
		this.maxProjectileID = result;
		return result;
	}

	// Token: 0x0600038D RID: 909 RVA: 0x00005540 File Offset: 0x00003740
	public int NewProjectileSeed()
	{
		return Random.Range(0, int.MaxValue);
	}

	// Token: 0x0600038E RID: 910 RVA: 0x0000554D File Offset: 0x0000374D
	public void SendProjectileAttack(PlayerProjectileAttack attack)
	{
		base.ServerRPC<PlayerProjectileAttack>("OnProjectileAttack", attack);
	}

	// Token: 0x0600038F RID: 911 RVA: 0x0000555B File Offset: 0x0000375B
	public void SendProjectileRicochet(PlayerProjectileRicochet ricochet)
	{
		base.ServerRPC<PlayerProjectileRicochet>("OnProjectileRicochet", ricochet);
	}

	// Token: 0x06000390 RID: 912 RVA: 0x00005569 File Offset: 0x00003769
	public void SendProjectileUpdate(PlayerProjectileUpdate update)
	{
		base.ServerRPC<PlayerProjectileUpdate>("OnProjectileUpdate", update);
	}

	// Token: 0x06000391 RID: 913 RVA: 0x00037CB0 File Offset: 0x00035EB0
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.basePlayer != null)
		{
			BasePlayer basePlayer = info.msg.basePlayer;
			this.userID = basePlayer.userid;
			this.UserIDString = this.userID.ToString();
			if (basePlayer.name != null)
			{
				this._displayName = basePlayer.name;
				if (string.IsNullOrEmpty(this._displayName.Trim()))
				{
					this._displayName = "Blaster :D";
				}
			}
			this.playerFlags = (BasePlayer.PlayerFlags)basePlayer.playerFlags;
			this.currentTeam = basePlayer.currentTeam;
			if (basePlayer.metabolism != null)
			{
				this.metabolism.Load(basePlayer.metabolism);
			}
			if (basePlayer.inventory != null)
			{
				this.inventory.Load(basePlayer.inventory);
			}
			if (this.playerModel && this.playerModel.nameTag)
			{
				this.playerModel.nameTag.UpdateFrom(this);
			}
			if (basePlayer.modelState != null)
			{
				if (this.modelState != null)
				{
					this.modelState.ResetToPool();
					this.modelState = null;
				}
				this.modelState = basePlayer.modelState;
				basePlayer.modelState = null;
				this.OnModelStateChanged();
			}
			if (!info.fromDisk)
			{
				this.ClientUpdateMounted(basePlayer.mounted);
			}
			if (base.isClient && info.msg.basePlayer.persistantData != null)
			{
				this.ClientUpdatePersistantData(info.msg.basePlayer.persistantData);
			}
			this.clActiveItem = basePlayer.heldEntity;
		}
	}

	// Token: 0x06000392 RID: 914 RVA: 0x00005577 File Offset: 0x00003777
	public override float GetThreatLevel()
	{
		this.EnsureUpdated();
		return this.cachedThreatLevel;
	}

	// Token: 0x06000393 RID: 915 RVA: 0x00037E34 File Offset: 0x00036034
	public void EnsureUpdated()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastUpdateTime < 30f)
		{
			return;
		}
		this.lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		this.cachedThreatLevel = 0f;
		if (!this.IsSleeping())
		{
			if (this.inventory.containerWear.itemList.Count > 2)
			{
				this.cachedThreatLevel += 1f;
			}
			foreach (Item item in this.inventory.containerBelt.itemList)
			{
				BaseEntity heldEntity = item.GetHeldEntity();
				if (heldEntity && heldEntity is BaseProjectile && !(heldEntity is BowWeapon))
				{
					this.cachedThreatLevel += 2f;
					break;
				}
			}
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x00037F1C File Offset: 0x0003611C
	[BaseEntity.RPC_Client]
	public void SetHostileLength(BaseEntity.RPCMessage msg)
	{
		float num = msg.read.Float();
		HostileNote.unhostileTime = UnityEngine.Time.realtimeSinceStartup + num;
	}

	// Token: 0x06000395 RID: 917 RVA: 0x00005585 File Offset: 0x00003785
	[BaseEntity.RPC_Client]
	public void SetWeaponDrawnDuration(BaseEntity.RPCMessage msg)
	{
		HostileNote.weaponDrawnDuration = msg.read.Float();
	}

	// Token: 0x06000396 RID: 918 RVA: 0x00002ECE File Offset: 0x000010CE
	protected virtual void ModifyInputState(ref InputState inputState)
	{
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00037F44 File Offset: 0x00036144
	[BaseEntity.RPC_Client]
	private void ForcePositionToParentOffset(Vector3 position, uint entID)
	{
		if (entID != 0U)
		{
			BaseNetworkable baseNetworkable = BaseNetworkable.clientEntities.Find(entID);
			if (baseNetworkable != null)
			{
				this.ForcePositionTo(baseNetworkable.transform.TransformPoint(position));
				return;
			}
		}
		else
		{
			this.ForcePositionTo(position);
		}
	}

	// Token: 0x06000398 RID: 920 RVA: 0x00037F84 File Offset: 0x00036184
	[BaseEntity.RPC_Client]
	private void ForcePositionTo(Vector3 position)
	{
		this.SetNetworkPosition((base.transform.parent != null) ? base.transform.parent.InverseTransformPoint(position) : position);
		if (this.movement)
		{
			this.movement.TeleportTo(position, this);
		}
	}

	// Token: 0x06000399 RID: 921 RVA: 0x00037FD8 File Offset: 0x000361D8
	internal void SendVoiceData(byte[] data, int len)
	{
		if (Net.cl != null && Net.cl.IsConnected() && Net.cl.write.Start())
		{
			Net.cl.write.PacketID(21);
			Net.cl.write.BytesWithSize(data, len);
			Write write = Net.cl.write;
			SendInfo sendInfo;
			sendInfo..ctor(Net.cl.Connection);
			sendInfo.priority = 0;
			write.Send(sendInfo);
		}
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00038054 File Offset: 0x00036254
	internal void SendClientTick()
	{
		Assert.IsNotNull<PlayerInput>(this.input, "input is null");
		Assert.IsNotNull<InputState>(this.input.state, "input.state is null");
		Assert.IsNotNull<Network.Client>(Net.cl, "Network.Net.cl is null");
		Assert.IsNotNull<Write>(Net.cl.write, "Network.Net.cl.write is null");
		this.lastSentTickTime = UnityEngine.Time.realtimeSinceStartup;
		using (PlayerTick playerTick = Pool.Get<PlayerTick>())
		{
			Item activeItem = this.Belt.GetActiveItem();
			playerTick.activeItem = ((activeItem != null) ? activeItem.uid : 0U);
			this.ModifyInputState(ref this.input.state);
			playerTick.inputState = this.input.state.current;
			playerTick.position = base.transform.localPosition;
			playerTick.eyePos = this.eyes.position;
			playerTick.parentID = this.parentEntity.uid;
			if (playerTick.modelState == null)
			{
				playerTick.modelState = Pool.Get<ModelState>();
				playerTick.modelState.onground = true;
			}
			if (this.modelState != null)
			{
				this.modelState.CopyTo(playerTick.modelState);
			}
			if (Net.cl.write.Start())
			{
				Net.cl.write.PacketID(15);
				playerTick.WriteToStreamDelta(Net.cl.write, this.lastSentTick);
				Write write = Net.cl.write;
				SendInfo sendInfo = new SendInfo(Net.cl.Connection);
				sendInfo.priority = 0;
				write.Send(sendInfo);
			}
			if (Net.cl.IsRecording)
			{
				byte[] array = playerTick.ToProtoBytes();
				Net.cl.ManualRecordPacket(15, array, array.Length);
			}
			if (this.lastSentTick == null)
			{
				this.lastSentTick = Pool.Get<PlayerTick>();
			}
			playerTick.CopyTo(this.lastSentTick);
		}
	}

	// Token: 0x0600039B RID: 923 RVA: 0x00005597 File Offset: 0x00003797
	private void ForceUpdateTriggersAction()
	{
		if (!base.IsDestroyed)
		{
			this.ForceUpdateTriggers(false, true, false);
		}
	}

	// Token: 0x0600039C RID: 924 RVA: 0x00038234 File Offset: 0x00036434
	public void ForceUpdateTriggers(bool enter = true, bool exit = true, bool invoke = true)
	{
		List<TriggerBase> list = Pool.GetList<TriggerBase>();
		List<TriggerBase> list2 = Pool.GetList<TriggerBase>();
		if (this.triggers != null)
		{
			list.AddRange(this.triggers);
		}
		CapsuleCollider component = base.GetComponent<CapsuleCollider>();
		Vector3 point = base.transform.position + new Vector3(0f, this.GetRadius(), 0f);
		Vector3 point2 = base.transform.position + new Vector3(0f, this.GetHeight() - this.GetRadius(), 0f);
		GamePhysics.OverlapCapsule<TriggerBase>(point, point2, this.GetRadius(), list2, 262144, 2);
		if (exit)
		{
			foreach (TriggerBase triggerBase in list)
			{
				if (!list2.Contains(triggerBase))
				{
					triggerBase.OnTriggerExit(component);
				}
			}
		}
		if (enter)
		{
			foreach (TriggerBase triggerBase2 in list2)
			{
				if (!list.Contains(triggerBase2))
				{
					triggerBase2.OnTriggerEnter(component);
				}
				else if (triggerBase2 is TriggerParent)
				{
					triggerBase2.OnEntityEnter(this);
				}
			}
		}
		Pool.FreeList<TriggerBase>(ref list);
		Pool.FreeList<TriggerBase>(ref list2);
		if (invoke)
		{
			base.Invoke(new Action(this.ForceUpdateTriggersAction), UnityEngine.Time.fixedDeltaTime * 1.5f);
		}
	}

	// Token: 0x0600039D RID: 925 RVA: 0x000383AC File Offset: 0x000365AC
	public static void UpdatePlayerVisibilities()
	{
		if (PlayerCull.enabled || Culling.toggle)
		{
			BasePlayer[] buffer = BasePlayer.VisiblePlayerList.Buffer;
			int count = BasePlayer.VisiblePlayerList.Count;
			for (int i = 0; i < count; i++)
			{
				BasePlayer basePlayer = buffer[i];
				if (basePlayer.WantsVisUpdate())
				{
					basePlayer.VisUpdate();
				}
			}
		}
	}

	// Token: 0x0600039E RID: 926 RVA: 0x000055AA File Offset: 0x000037AA
	private float TimeSinceSeen()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastTimeSeen;
	}

	// Token: 0x0600039F RID: 927 RVA: 0x000055B8 File Offset: 0x000037B8
	private void SetNextVisThink(float addTime)
	{
		this.nextVisThink = UnityEngine.Time.realtimeSinceStartup + addTime;
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x000055C7 File Offset: 0x000037C7
	private bool WantsVisUpdate()
	{
		return UnityEngine.Time.realtimeSinceStartup >= this.nextVisThink;
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x000383FC File Offset: 0x000365FC
	private static bool IsAimingAt(BasePlayer aimer, BasePlayer target, float cone = 0.95f)
	{
		Vector3 normalized = (target.eyes.position - aimer.eyes.position).normalized;
		return Vector3.Dot(aimer.eyes.HeadForward(), normalized) > cone && aimer.VisPlayerArmed() && target.VisPlayerArmed();
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00038454 File Offset: 0x00036654
	protected override void UpdateCullingSpheres()
	{
		float num = this.GetRadius() * 2f;
		Vector3 center;
		Matrix4x4 localToWorldMatrix;
		if (this.playerModel != null && this.playerModel.collision != null)
		{
			center = this.playerModel.collision.center;
			localToWorldMatrix = this.playerModel.transform.localToWorldMatrix;
		}
		else
		{
			center = this.bounds.center;
			localToWorldMatrix = base.transform.localToWorldMatrix;
		}
		if (this.IsSleeping() || this.IsWounded())
		{
			center.y *= 0.25f;
			num *= (this.IsSleeping() ? 0.5f : 1f);
		}
		else if (this.IsDucked())
		{
			center.y *= 0.66f;
		}
		this.localOccludee.sphere = new OcclusionCulling.Sphere(localToWorldMatrix.MultiplyPoint3x4(center), num);
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x00038538 File Offset: 0x00036738
	private void VisUpdateUsingCulling(float dist, bool visibility)
	{
		float entityMinCullDist = Culling.entityMinCullDist;
		float entityMinAnimatorCullDist = Culling.entityMinAnimatorCullDist;
		float entityMinShadowCullDist = Culling.entityMinShadowCullDist;
		float entityMaxDist = Culling.entityMaxDist;
		this.UpdateCullingBounds();
		bool isVisible = dist <= entityMaxDist && (dist <= entityMinCullDist || visibility);
		this.isVisible = isVisible;
		this.isAnimatorVisible = (this.isVisible || dist <= entityMinAnimatorCullDist);
		this.isShadowVisible = (this.isVisible || dist <= entityMinShadowCullDist);
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x000385B0 File Offset: 0x000367B0
	private void VisUpdateUsingRays(float dist)
	{
		bool flag;
		if (this.IsSleeping() && dist > PlayerCull.maxSleeperDist)
		{
			flag = false;
		}
		else if (dist > PlayerCull.maxPlayerDist)
		{
			flag = false;
		}
		else if (dist <= PlayerCull.minCullDist)
		{
			flag = true;
		}
		else if (BasePlayer.IsAimingAt(LocalPlayer.Entity, this, 0.99f))
		{
			flag = true;
		}
		else if (BasePlayer.IsAimingAt(this, LocalPlayer.Entity, 0.99f))
		{
			flag = true;
		}
		else
		{
			Vector3 normalized = (this.eyes.position - LocalPlayer.Entity.eyes.position).normalized;
			flag = (Vector3.Dot(LocalPlayer.Entity.eyes.HeadForward(), normalized) >= 0f && this.AnyPartVisible());
		}
		if (flag)
		{
			this.lastTimeSeen = UnityEngine.Time.realtimeSinceStartup;
		}
		if (flag || this.TimeSinceSeen() <= 2f)
		{
			this.isVisible = true;
		}
		else
		{
			this.isVisible = false;
		}
		this.isAnimatorVisible = this.isVisible;
		this.isShadowVisible = this.isVisible;
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x000386B4 File Offset: 0x000368B4
	private void LogDebugCull(float dist)
	{
		if (this.isVisible != this.debugPrevVisible)
		{
			Debug.Log(string.Concat(new object[]
			{
				"VisChanged: ",
				this.displayName,
				", Distance: ",
				dist,
				", ",
				this.IsSleeping().ToString()
			}));
			this.debugPrevVisible = this.isVisible;
		}
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x00038728 File Offset: 0x00036928
	protected override void OnVisibilityChanged(bool visible)
	{
		if (LocalPlayer.Entity != null && MainCamera.mainCamera != null && !this.IsLocalPlayer())
		{
			bool isVisible = this.isVisible;
			bool isAnimatorVisible = this.isAnimatorVisible;
			bool isShadowVisible = this.isShadowVisible;
			float dist = Vector3.Distance(MainCamera.mainCamera.transform.position, base.transform.position);
			this.VisUpdateUsingCulling(dist, visible);
			if (this.playerModel != null && (this.isVisible != isVisible || this.isAnimatorVisible != isAnimatorVisible || this.isShadowVisible != isShadowVisible))
			{
				this.playerModel.ApplyVisibility(this.isVisible, this.isAnimatorVisible, this.isShadowVisible);
			}
		}
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x000387E4 File Offset: 0x000369E4
	private void VisUpdate()
	{
		if (LocalPlayer.Entity == null || MainCamera.mainCamera == null || this.IsLocalPlayer())
		{
			return;
		}
		if (Culling.toggle)
		{
			float dist = Vector3.Distance(MainCamera.mainCamera.transform.position, base.transform.position);
			if (UnityEngine.Time.realtimeSinceStartup >= this.nextVisThink)
			{
				this.VisUpdateUsingCulling(dist, this.CheckVisibility());
			}
			float fDuration = base.CalcEntityVisUpdateRate() + Random.Range(0f, 0.1f);
			this.SetNextVisThink(fDuration);
			if (OcclusionCulling.DebugFilterIsDynamic(Culling.debug))
			{
				Color color = this.IsDead() ? Color.black : (this.isVisible ? Color.green : Color.red);
				if (this.IsSleeping())
				{
					UnityEngine.DDraw.Box(this.localOccludee.sphere.position, this.localOccludee.sphere.radius * 1.5f, color, fDuration, false);
					return;
				}
				UnityEngine.DDraw.SphereGizmo(this.localOccludee.sphere.position, this.localOccludee.sphere.radius, color, fDuration, false, false);
				return;
			}
		}
		else
		{
			float dist2 = Vector3.Distance(LocalPlayer.Entity.eyes.position, base.transform.position);
			if (UnityEngine.Time.realtimeSinceStartup >= this.nextVisThink)
			{
				this.VisUpdateUsingRays(dist2);
			}
			float num = this.CalcVisUpdateRate(dist2) + Random.Range(0f, 0.1f);
			this.SetNextVisThink(num);
		}
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x00038960 File Offset: 0x00036B60
	private bool VisPlayerArmed()
	{
		HeldEntity heldEntity = this.GetHeldEntity();
		return heldEntity != null && heldEntity is BaseProjectile;
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x00038988 File Offset: 0x00036B88
	private bool AnyPartVisible()
	{
		Vector3 position = LocalPlayer.Entity.eyes.position;
		Vector3 a = LocalPlayer.Entity.eyes.HeadRight();
		Vector3 vector = base.CenterPoint();
		if (this.IsSleeping())
		{
			vector += new Vector3(0f, 1f, 0f);
		}
		float dist = Vector3.Distance(position, vector);
		bool flag = this.PointSeePoint(position, vector, dist, true);
		if (this.IsSleeping())
		{
			return flag;
		}
		if (!flag && PlayerCull.visQuality > 0)
		{
			Vector3 position2 = this.eyes.position;
			flag = this.PointSeePoint(position, position2, dist, false);
		}
		if (!flag && PlayerCull.visQuality > 1)
		{
			Vector3 origin = vector + a * 0.25f;
			flag = this.PointSeePoint(position, origin, dist, false);
			if (!flag)
			{
				Vector3 origin2 = vector + a * -0.25f;
				flag = this.PointSeePoint(position, origin2, dist, false);
			}
		}
		if (!flag && PlayerCull.visQuality > 2)
		{
			flag = this.PointSeePoint(position, base.transform.position + new Vector3(0f, 0.1f, 0f), dist, false);
		}
		return flag;
	}

	// Token: 0x060003AA RID: 938 RVA: 0x00038AB4 File Offset: 0x00036CB4
	private float CalcVisUpdateRate(float dist)
	{
		if (this.IsSleeping() && dist > PlayerCull.maxSleeperDist)
		{
			if (dist > 80f)
			{
				return 10f;
			}
			if (dist > 30f)
			{
				return 3f;
			}
			if (dist > 15f)
			{
				return 1f;
			}
		}
		return 1f / Mathf.Clamp(PlayerCull.updateRate, 0.015f, float.PositiveInfinity);
	}

	// Token: 0x060003AB RID: 939 RVA: 0x00038B18 File Offset: 0x00036D18
	private bool PointSeePoint(Vector3 target, Vector3 origin, float dist = 0f, bool useGameTrace = false)
	{
		bool flag = false;
		if (dist == 0f)
		{
			dist = Vector3.Distance(target, origin);
		}
		Vector3 normalized = (target - origin).normalized;
		Ray ray = new Ray(origin, normalized);
		RaycastHit raycastHit;
		if (useGameTrace ? GamePhysics.Trace(ray, 0f, out raycastHit, dist, 10551297, 0) : Physics.Raycast(ray, ref raycastHit, dist, 10551297))
		{
			ColliderInfo component = raycastHit.collider.GetComponent<ColliderInfo>();
			if (component == null || component.HasFlag(ColliderInfo.Flags.Opaque))
			{
				flag = true;
			}
		}
		return !flag;
	}

	// Token: 0x060003AC RID: 940 RVA: 0x000055D9 File Offset: 0x000037D9
	public bool IsWounded()
	{
		return this.HasPlayerFlag(BasePlayer.PlayerFlags.Wounded);
	}

	// Token: 0x02000020 RID: 32
	public enum CameraMode
	{
		// Token: 0x04000177 RID: 375
		FirstPerson,
		// Token: 0x04000178 RID: 376
		ThirdPerson,
		// Token: 0x04000179 RID: 377
		Eyes
	}

	// Token: 0x02000021 RID: 33
	[Flags]
	public enum PlayerFlags
	{
		// Token: 0x0400017B RID: 379
		Unused1 = 1,
		// Token: 0x0400017C RID: 380
		Unused2 = 2,
		// Token: 0x0400017D RID: 381
		IsAdmin = 4,
		// Token: 0x0400017E RID: 382
		ReceivingSnapshot = 8,
		// Token: 0x0400017F RID: 383
		Sleeping = 16,
		// Token: 0x04000180 RID: 384
		Spectating = 32,
		// Token: 0x04000181 RID: 385
		Wounded = 64,
		// Token: 0x04000182 RID: 386
		IsDeveloper = 128,
		// Token: 0x04000183 RID: 387
		Connected = 256,
		// Token: 0x04000184 RID: 388
		VoiceMuted = 512,
		// Token: 0x04000185 RID: 389
		ThirdPersonViewmode = 1024,
		// Token: 0x04000186 RID: 390
		EyesViewmode = 2048,
		// Token: 0x04000187 RID: 391
		ChatMute = 4096,
		// Token: 0x04000188 RID: 392
		NoSprint = 8192,
		// Token: 0x04000189 RID: 393
		Aiming = 16384,
		// Token: 0x0400018A RID: 394
		DisplaySash = 32768,
		// Token: 0x0400018B RID: 395
		Relaxed = 65536,
		// Token: 0x0400018C RID: 396
		SafeZone = 131072,
		// Token: 0x0400018D RID: 397
		Workbench1 = 1048576,
		// Token: 0x0400018E RID: 398
		Workbench2 = 2097152,
		// Token: 0x0400018F RID: 399
		Workbench3 = 4194304
	}
}
