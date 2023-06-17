using System;
using System.Collections.Generic;
using ConVar;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000098 RID: 152
public class HeldEntity : BaseEntity
{
	// Token: 0x040005AF RID: 1455
	public Animator worldModelAnimator;

	// Token: 0x040005B0 RID: 1456
	public SoundDefinition thirdPersonDeploySound;

	// Token: 0x040005B1 RID: 1457
	public SoundDefinition thirdPersonAimSound;

	// Token: 0x040005B2 RID: 1458
	public SoundDefinition thirdPersonAimEndSound;

	// Token: 0x040005B3 RID: 1459
	protected ViewModel viewModel;

	// Token: 0x040005B4 RID: 1460
	protected bool isDeployed;

	// Token: 0x040005B5 RID: 1461
	public static float lastExamineTime = -120f;

	// Token: 0x040005B6 RID: 1462
	private float nextExamineTime;

	// Token: 0x040005B7 RID: 1463
	[Header("Held Entity")]
	public string handBone = "r_prop";

	// Token: 0x040005B8 RID: 1464
	public AnimatorOverrideController HoldAnimationOverride;

	// Token: 0x040005B9 RID: 1465
	public NPCPlayerApex.ToolTypeEnum toolType;

	// Token: 0x040005BA RID: 1466
	[Header("Hostility")]
	public float hostileScore;

	// Token: 0x040005BB RID: 1467
	public HeldEntity.HolsterInfo holsterInfo;

	// Token: 0x040005BC RID: 1468
	internal uint ownerItemUID;

	// Token: 0x040005BD RID: 1469
	protected List<HeldEntity.PunchEntry> _punches = new List<HeldEntity.PunchEntry>();

	// Token: 0x040005BE RID: 1470
	protected Vector3 punchAdded = Vector3.zero;

	// Token: 0x040005BF RID: 1471
	protected float lastPunchTime;

	// Token: 0x060008D9 RID: 2265 RVA: 0x0004E4CC File Offset: 0x0004C6CC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HeldEntity.OnRpcMessage", 0.1f))
		{
			if (rpc == 471191426U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: CL_Punch ");
				}
				using (TimeWarning.New("CL_Punch", 0.1f))
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
							this.CL_Punch(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in CL_Punch", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x000091F2 File Offset: 0x000073F2
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.viewModel = base.GetComponent<ViewModel>();
		if (this.viewModel)
		{
			this.viewModel.targetEntity = this;
		}
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x00009220 File Offset: 0x00007420
	internal override void DoNetworkDestroy()
	{
		base.DoNetworkDestroy();
		if (this.isDeployed)
		{
			this.OnHolster();
		}
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x0004E5E8 File Offset: 0x0004C7E8
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		if (this.isDeployed && base.GetParentEntity() == null)
		{
			this.OnHolster();
		}
		BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer != null && ownerPlayer.IsLocalPlayer())
		{
			bool flag = this.IsDeployed();
			if (this.isDeployed && !flag)
			{
				this.OnHolster();
			}
			else if (!this.isDeployed && flag)
			{
				this.OnDeploy();
			}
		}
		if (this.viewModel && this.viewModel.instance && this.viewModel.instance.wantsHeldItemFlags)
		{
			this.viewModel.instance.gameObject.BroadcastOnPostNetworkUpdate(this);
		}
		this.UpdateHolsteredOffset();
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x0004E6AC File Offset: 0x0004C8AC
	public void UpdateHolsteredOffset()
	{
		if (!this.holsterInfo.displayWhenHolstered)
		{
			return;
		}
		if (!this.parentEntity.IsSet())
		{
			return;
		}
		if (this.parentEntity.Get(false) == null)
		{
			base.Invoke(new Action(this.UpdateHolsteredOffset), 0.1f);
			return;
		}
		BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (!this.IsDeployed() && !base.IsDisabled() && ownerPlayer != null)
		{
			int num = -1;
			Vector3 a = Vector3.zero;
			Vector3 vector = Vector3.zero;
			foreach (Item item in ownerPlayer.inventory.containerWear.itemList)
			{
				ItemModWearable component = item.info.GetComponent<ItemModWearable>();
				if (component != null)
				{
					WearableHolsterOffset component2 = component.entityPrefab.Get().GetComponent<WearableHolsterOffset>();
					if (component2 != null)
					{
						foreach (WearableHolsterOffset.offsetInfo offsetInfo in component2.Offsets)
						{
							if (offsetInfo.type == this.holsterInfo.slot && offsetInfo.priority > num)
							{
								num = offsetInfo.priority;
								a = offsetInfo.offset;
								vector = offsetInfo.rotationOffset;
							}
						}
					}
				}
			}
			base.transform.localPosition = a + this.holsterInfo.holsterOffset;
			base.transform.localRotation = ((this.holsterInfo.holsterRotationOffset == Vector3.zero) ? Quaternion.identity : Quaternion.Euler(this.holsterInfo.holsterRotationOffset)) * ((vector == Vector3.zero) ? Quaternion.identity : Quaternion.Euler(vector));
			return;
		}
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void EditViewAngles()
	{
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x00009236 File Offset: 0x00007436
	public virtual void OnFrame()
	{
		this.SimPunches();
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnInput()
	{
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x0004E8A8 File Offset: 0x0004CAA8
	public virtual void Examine()
	{
		if (this.viewModel != null && UnityEngine.Time.realtimeSinceStartup >= this.nextExamineTime)
		{
			this.viewModel.Trigger("admire");
			this.nextExamineTime = UnityEngine.Time.realtimeSinceStartup + 2f;
			HeldEntity.lastExamineTime = UnityEngine.Time.realtimeSinceStartup;
			Analytics.TimesExamined++;
		}
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x0004E908 File Offset: 0x0004CB08
	public virtual void OnDeploy()
	{
		BasePlayer ownerPlayer = this.GetOwnerPlayer();
		Assert.IsTrue(ownerPlayer.IsLocalPlayer(), "OnDeploy called on NOT the local player");
		base.SetFlag(BaseEntity.Flags.Disabled, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved4, true, false, true);
		base.UpdateDisableState();
		if (this.viewModel && ownerPlayer && ownerPlayer.IsLocalPlayer())
		{
			this.viewModel.Deploy(this);
		}
		if (this.worldModelAnimator != null)
		{
			this.worldModelAnimator.SetTrigger("deploy");
		}
		if (ownerPlayer)
		{
			ownerPlayer.SendSignalBroadcast(BaseEntity.Signal.Deploy, "");
		}
		this.OnDeployed();
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x0000923E File Offset: 0x0000743E
	public virtual void OnDeployed()
	{
		this.isDeployed = true;
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0004E9AC File Offset: 0x0004CBAC
	public virtual void OnHolster()
	{
		BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer)
		{
			Assert.IsTrue(ownerPlayer.IsLocalPlayer(), "OnHolster called on NOT the local player");
			if (this.viewModel && ownerPlayer.IsLocalPlayer())
			{
				this.viewModel.Holster();
			}
			base.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
		}
		base.UpdateDisableState();
		this.OnHolstered();
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00009247 File Offset: 0x00007447
	public virtual void OnHolstered()
	{
		this.isDeployed = false;
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnViewmodelEvent(string name)
	{
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void ModifyCamera()
	{
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool ShouldDestroyImmediately()
	{
		return true;
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x00009250 File Offset: 0x00007450
	internal void UpdatePlayerModel(PlayerModel playerModel)
	{
		playerModel.SetHoldType(this.GetHoldAnimations());
		playerModel.SetAimSounds(this.thirdPersonAimSound, this.thirdPersonAimEndSound);
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x00009270 File Offset: 0x00007470
	public virtual AnimatorOverrideController GetHoldAnimations()
	{
		return this.HoldAnimationOverride;
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060008EB RID: 2283 RVA: 0x00009278 File Offset: 0x00007478
	public bool hostile
	{
		get
		{
			return this.hostileScore > 0f;
		}
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x00002D37 File Offset: 0x00000F37
	public bool LightsOn()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5);
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00002FD4 File Offset: 0x000011D4
	public bool IsDeployed()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x0004EA10 File Offset: 0x0004CC10
	public BasePlayer GetOwnerPlayer()
	{
		BaseEntity parentEntity = base.GetParentEntity();
		if (!parentEntity.IsValid())
		{
			return null;
		}
		BasePlayer basePlayer = parentEntity.ToPlayer();
		if (basePlayer == null)
		{
			return null;
		}
		if (basePlayer.IsDead())
		{
			return null;
		}
		return basePlayer;
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x0004EA4C File Offset: 0x0004CC4C
	public Connection GetOwnerConnection()
	{
		BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return null;
		}
		if (ownerPlayer.net == null)
		{
			return null;
		}
		return ownerPlayer.net.connection;
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool CanBeUsedInWater()
	{
		return false;
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x0004EA80 File Offset: 0x0004CC80
	protected Item GetOwnerItem()
	{
		BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer == null || ownerPlayer.inventory == null)
		{
			return null;
		}
		return ownerPlayer.inventory.FindItemUID(this.ownerItemUID);
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00009287 File Offset: 0x00007487
	public override Item GetItem()
	{
		return this.GetOwnerItem();
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x0004EAC0 File Offset: 0x0004CCC0
	public ItemDefinition GetOwnerItemDefinition()
	{
		Item ownerItem = this.GetOwnerItem();
		if (ownerItem == null)
		{
			Debug.LogWarning("GetOwnerItem - null!", this);
			return null;
		}
		return ownerItem.info;
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x0000928F File Offset: 0x0000748F
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.heldEntity != null)
		{
			this.ownerItemUID = info.msg.heldEntity.itemUID;
		}
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x0004EAEC File Offset: 0x0004CCEC
	public virtual void AddPunch(Vector3 amount, float duration)
	{
		if (base.isClient)
		{
			HeldEntity.PunchEntry punchEntry = new HeldEntity.PunchEntry();
			punchEntry.startTime = UnityEngine.Time.time;
			punchEntry.amount = amount;
			punchEntry.duration = duration;
			this._punches.Add(punchEntry);
			this.lastPunchTime = UnityEngine.Time.time;
		}
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x0004EB38 File Offset: 0x0004CD38
	[BaseEntity.RPC_Client]
	public void CL_Punch(BaseEntity.RPCMessage msg)
	{
		Vector3 amount = msg.read.Vector3();
		float duration = msg.read.Float();
		this.AddPunch(amount, duration);
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void DoRecoilCompensation()
	{
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x0004EB68 File Offset: 0x0004CD68
	public virtual void SimPunches()
	{
		this.DoRecoilCompensation();
		if (this._punches.Count == 0)
		{
			return;
		}
		BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		Vector3 vector = ownerPlayer.input.ClientLookVars();
		for (int i = this._punches.Count - 1; i >= 0; i--)
		{
			HeldEntity.PunchEntry punchEntry = this._punches[i];
			Vector3 vector2 = punchEntry.amount - punchEntry.amountAdded;
			Vector3 vector3 = punchEntry.amount * (UnityEngine.Time.deltaTime / punchEntry.duration);
			vector3 = ((vector3.magnitude < vector2.magnitude) ? vector3 : vector2);
			vector += vector3;
			if (ownerPlayer.isMounted)
			{
				ownerPlayer.input.offsetAngles += vector3;
			}
			punchEntry.amountAdded += vector3;
			this.punchAdded += vector3;
			if (UnityEngine.Time.time - punchEntry.startTime >= punchEntry.duration)
			{
				this._punches.RemoveAt(i);
			}
		}
		ownerPlayer.input.SetViewVars(vector);
	}

	// Token: 0x02000099 RID: 153
	[Serializable]
	public class HolsterInfo
	{
		// Token: 0x040005C0 RID: 1472
		public HeldEntity.HolsterInfo.HolsterSlot slot;

		// Token: 0x040005C1 RID: 1473
		public bool displayWhenHolstered;

		// Token: 0x040005C2 RID: 1474
		public string holsterBone = "spine3";

		// Token: 0x040005C3 RID: 1475
		public Vector3 holsterOffset;

		// Token: 0x040005C4 RID: 1476
		public Vector3 holsterRotationOffset;

		// Token: 0x0200009A RID: 154
		public enum HolsterSlot
		{
			// Token: 0x040005C6 RID: 1478
			BACK,
			// Token: 0x040005C7 RID: 1479
			RIGHT_THIGH,
			// Token: 0x040005C8 RID: 1480
			LEFT_THIGH
		}
	}

	// Token: 0x0200009B RID: 155
	public static class HeldEntityFlags
	{
		// Token: 0x040005C9 RID: 1481
		public const BaseEntity.Flags Deployed = BaseEntity.Flags.Reserved4;

		// Token: 0x040005CA RID: 1482
		public const BaseEntity.Flags LightsOn = BaseEntity.Flags.Reserved5;
	}

	// Token: 0x0200009C RID: 156
	public class PunchEntry
	{
		// Token: 0x040005CB RID: 1483
		public Vector3 amount;

		// Token: 0x040005CC RID: 1484
		public float duration;

		// Token: 0x040005CD RID: 1485
		public float startTime;

		// Token: 0x040005CE RID: 1486
		public Vector3 amountAdded;
	}
}
