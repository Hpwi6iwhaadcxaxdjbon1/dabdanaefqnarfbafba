using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using GameMenu;
using Network;
using ProtoBuf;
using Rust.Workshop;
using Spatial;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class BaseEntity : BaseNetworkable, IProvider, ILerpTarget, IPrefabPreProcess
{
	// Token: 0x0400033C RID: 828
	protected Ragdoll ragdoll;

	// Token: 0x0400033D RID: 829
	internal PositionLerp positionLerp;

	// Token: 0x0400033E RID: 830
	private List<Option> menuOptions;

	// Token: 0x0400033F RID: 831
	[Header("BaseEntity")]
	public Bounds bounds;

	// Token: 0x04000340 RID: 832
	public GameObjectRef impactEffect;

	// Token: 0x04000341 RID: 833
	public bool enableSaving = true;

	// Token: 0x04000342 RID: 834
	public bool syncPosition;

	// Token: 0x04000343 RID: 835
	public Model model;

	// Token: 0x04000344 RID: 836
	[InspectorFlags]
	public BaseEntity.Flags flags;

	// Token: 0x04000345 RID: 837
	[NonSerialized]
	public uint parentBone;

	// Token: 0x04000346 RID: 838
	[NonSerialized]
	public ulong skinID;

	// Token: 0x04000347 RID: 839
	private EntityComponentBase[] _components;

	// Token: 0x04000348 RID: 840
	[NonSerialized]
	protected string _name;

	// Token: 0x0400034A RID: 842
	private static Queue<BaseEntity> globalBroadcastQueue = new Queue<BaseEntity>();

	// Token: 0x0400034B RID: 843
	private static uint globalBroadcastProtocol = 0U;

	// Token: 0x0400034C RID: 844
	private uint broadcastProtocol;

	// Token: 0x0400034D RID: 845
	private List<EntityLink> links = new List<EntityLink>();

	// Token: 0x0400034E RID: 846
	private bool linkedToNeighbours;

	// Token: 0x0400034F RID: 847
	private BaseEntity addedToParentEntity;

	// Token: 0x04000350 RID: 848
	[NonSerialized]
	public ItemSkin itemSkin;

	// Token: 0x04000351 RID: 849
	private EntityRef[] entitySlots = new EntityRef[7];

	// Token: 0x04000352 RID: 850
	protected List<TriggerBase> triggers;

	// Token: 0x04000353 RID: 851
	protected bool isVisible = true;

	// Token: 0x04000354 RID: 852
	protected bool isAnimatorVisible = true;

	// Token: 0x04000355 RID: 853
	protected bool isShadowVisible = true;

	// Token: 0x04000356 RID: 854
	protected OccludeeSphere localOccludee = new OccludeeSphere(-1);

	// Token: 0x06000633 RID: 1587 RVA: 0x00042D80 File Offset: 0x00040F80
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseEntity.OnRpcMessage", 0.1f))
		{
			if (rpc == 2050890860U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: Cl_ReceiveFilePng ");
				}
				using (TimeWarning.New("Cl_ReceiveFilePng", 0.1f))
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
							this.Cl_ReceiveFilePng(msg2);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in Cl_ReceiveFilePng", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
			if (rpc == 1474532722U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: SignalFromServer ");
				}
				using (TimeWarning.New("SignalFromServer", 0.1f))
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
							this.SignalFromServer(msg3);
						}
					}
					catch (Exception exception2)
					{
						Net.cl.Disconnect("RPC Error in SignalFromServer", true);
						Debug.LogException(exception2);
					}
				}
				return true;
			}
			if (rpc == 2406343387U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: SignalFromServerEx ");
				}
				using (TimeWarning.New("SignalFromServerEx", 0.1f))
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
							this.SignalFromServerEx(msg4);
						}
					}
					catch (Exception exception3)
					{
						Net.cl.Disconnect("RPC Error in SignalFromServerEx", true);
						Debug.LogException(exception3);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x000076FD File Offset: 0x000058FD
	public virtual float GetExtrapolationTime()
	{
		return Mathf.Max(Lerp.extrapolation, 0f);
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x0000770E File Offset: 0x0000590E
	public virtual float GetInterpolationDelay()
	{
		return Mathf.Max(Lerp.interpolation, 0f);
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x0000771F File Offset: 0x0000591F
	public virtual float GetInterpolationSmoothing()
	{
		return Mathf.Max(Lerp.smoothing, 0f);
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00007730 File Offset: 0x00005930
	public virtual Quaternion GetAngularVelocityClient()
	{
		if (!(this.positionLerp != null))
		{
			return Quaternion.identity;
		}
		return this.positionLerp.GetEstimatedAngularVelocity();
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x00007751 File Offset: 0x00005951
	public virtual Vector3 GetLocalVelocityClient()
	{
		if (!(this.positionLerp != null))
		{
			return Vector3.zero;
		}
		return this.positionLerp.GetEstimatedVelocity();
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x000430C8 File Offset: 0x000412C8
	public void DrawInterpolationState(TransformInterpolator.Segment segment, List<TransformInterpolator.Entry> entries)
	{
		Matrix4x4 matrix4x = (base.transform.parent != null) ? base.transform.parent.localToWorldMatrix : Matrix4x4.identity;
		TransformInterpolator.Entry entry = default(TransformInterpolator.Entry);
		foreach (TransformInterpolator.Entry entry2 in entries)
		{
			float time = entry.time;
			float time2 = entry2.time;
			Vector3 vector = matrix4x.MultiplyPoint3x4(entry.pos);
			Vector3 vector2 = matrix4x.MultiplyPoint3x4(entry2.pos);
			if (entry.time != 0f)
			{
				int num = (int)((time2 - time) * 1000f);
				UnityEngine.DDraw.Line(vector, vector2, Color.white, 0f, true, true);
				UnityEngine.DDraw.Text(num.ToString(), (vector + vector2) * 0.5f, Color.white, 0f);
			}
			UnityEngine.DDraw.Box(vector2, 0.05f, Color.yellow, 0f, true);
			entry = entry2;
		}
		Vector3 vPos = matrix4x.MultiplyPoint3x4(segment.prev.pos);
		Vector3 vector3 = matrix4x.MultiplyPoint3x4(segment.tick.pos);
		Vector3 vPosB = matrix4x.MultiplyPoint3x4(segment.next.pos);
		UnityEngine.DDraw.Line(vPos, vector3, Color.green, 0f, true, true);
		UnityEngine.DDraw.Line(vector3, vPosB, Color.red, 0f, true, true);
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00007772 File Offset: 0x00005972
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.SetNetworkPosition(base.transform.localPosition);
		this.SetNetworkRotation(base.transform.localRotation);
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x0000779D File Offset: 0x0000599D
	public override void ClientOnEnable()
	{
		base.ClientOnEnable();
		this.UpdateDisableState();
		if (this.model != null && !this.model.gameObject.activeSelf)
		{
			this.model.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x0004324C File Offset: 0x0004144C
	public void OnPositionalFromNetwork(Vector3 vPos, Vector3 vAng, float time)
	{
		Quaternion quaternion = Quaternion.Euler(vAng);
		if (!this.ShouldLerp() || !Lerp.enabled)
		{
			this.StopLerping();
			this.SetNetworkPosition(vPos);
			this.SetNetworkRotation(quaternion);
			return;
		}
		if (this.positionLerp == null)
		{
			base.transform.localPosition = vPos;
			base.transform.localRotation = quaternion;
		}
		this.StartLerping(time);
		if (this.positionLerp != null)
		{
			this.positionLerp.Snapshot(vPos, quaternion, time);
		}
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x000432D0 File Offset: 0x000414D0
	public void StartLerping(float time)
	{
		if (this.positionLerp == null)
		{
			this.positionLerp = base.gameObject.AddComponent<PositionLerp>();
			this.positionLerp.Initialize(this);
		}
		if (!this.positionLerp.enabled)
		{
			this.positionLerp.SnapTo(base.transform.localPosition, base.transform.localRotation, time - 0.1f);
			this.positionLerp.enabled = true;
		}
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x000077DC File Offset: 0x000059DC
	public void StopLerping()
	{
		if (this.positionLerp == null)
		{
			return;
		}
		if (!this.positionLerp.enabled)
		{
			return;
		}
		this.positionLerp.SnapToEnd();
		this.positionLerp.enabled = false;
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x00007812 File Offset: 0x00005A12
	public virtual bool ShouldLerp()
	{
		return this.syncPosition && this.parentBone != StringPool.closest;
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x0000782E File Offset: 0x00005A2E
	public virtual void SetNetworkPosition(Vector3 vPos)
	{
		bool flag = this.HasClientTransformOffset();
		if (flag)
		{
			this.RemoveClientTransformOffset();
		}
		if (base.transform.localPosition != vPos)
		{
			base.transform.localPosition = vPos;
		}
		if (flag)
		{
			this.AddClientTransformOffset();
		}
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00007866 File Offset: 0x00005A66
	public virtual void SetNetworkRotation(Quaternion qRot)
	{
		bool flag = this.HasClientTransformOffset();
		if (flag)
		{
			this.RemoveClientTransformOffset();
		}
		base.transform.localRotation = qRot;
		if (flag)
		{
			this.AddClientTransformOffset();
		}
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x0000788B File Offset: 0x00005A8B
	public virtual void DoDestroyEffects(BaseNetworkable.DestroyMode mode, Message msg)
	{
		if (mode == BaseNetworkable.DestroyMode.None)
		{
			return;
		}
		if (mode == BaseNetworkable.DestroyMode.Gib)
		{
			base.gameObject.CustomGib();
			return;
		}
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x000078A1 File Offset: 0x00005AA1
	internal override void DoNetworkDestroy()
	{
		base.DoNetworkDestroy();
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x000078A9 File Offset: 0x00005AA9
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		this.UpdateDisableState();
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x000078B7 File Offset: 0x00005AB7
	public virtual void OnBecameRagdoll(Ragdoll rdoll)
	{
		if (this.model != null)
		{
			this.model.gameObject.SetActive(false);
		}
		this.ragdoll = rdoll;
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x0004334C File Offset: 0x0004154C
	public virtual Info GetMenuInformation(GameObject primaryObject, BasePlayer player)
	{
		List<Option> menuItems = this.GetMenuItems(player);
		Info result;
		if (menuItems == null || menuItems.Count == 0)
		{
			result = default(Info);
			return result;
		}
		using (TimeWarning.New("FirstOption", 0.1f))
		{
			for (int i = 0; i < menuItems.Count; i++)
			{
				Option option = menuItems[i];
				if (option.show && !option.showDisabled)
				{
					return new Info
					{
						action = option.title,
						icon = option.icon,
						iconSprite = option.iconSprite,
						hasMoreOptions = (menuItems.Count > 1)
					};
				}
			}
		}
		using (TimeWarning.New("defaultInfo", 0.1f))
		{
			result = new Info
			{
				action = menuItems[0].title,
				icon = menuItems[0].icon,
				iconSprite = menuItems[0].iconSprite,
				hasMoreOptions = (menuItems.Count > 1)
			};
		}
		return result;
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x00043498 File Offset: 0x00041698
	public virtual List<Option> GetMenuItems(BasePlayer player)
	{
		if (this.IsBusy())
		{
			return null;
		}
		if (this.menuOptions == null)
		{
			this.menuOptions = new List<Option>(8);
		}
		this.menuOptions.Clear();
		using (TimeWarning.New("GetEntityMenu", 0.1f))
		{
			this.GetEntityMenu(player, this.menuOptions);
		}
		if (this.menuOptions.Count > 0)
		{
			if (this.menuOptions.Count > 1)
			{
				this.menuOptions.Sort((Option x, Option y) => x.order.CompareTo(y.order));
			}
			return this.menuOptions;
		}
		return null;
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x00043558 File Offset: 0x00041758
	public virtual void OnUse(BasePlayer player)
	{
		List<Option> menuItems = this.GetMenuItems(player);
		if (menuItems == null || menuItems.Count == 0)
		{
			return;
		}
		if (!menuItems[0].show)
		{
			return;
		}
		if (menuItems[0].showDisabled)
		{
			return;
		}
		if (menuItems[0].longUseOnly)
		{
			return;
		}
		if (menuItems.Count <= 1 && menuItems[0].time > 0f)
		{
			return;
		}
		menuItems[0].function.Invoke(player);
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x000435D8 File Offset: 0x000417D8
	public virtual void OnUseHeld(BasePlayer player)
	{
		List<Option> menuItems = this.GetMenuItems(player);
		if (menuItems == null || menuItems.Count == 0)
		{
			return;
		}
		for (int i = 0; i < menuItems.Count; i++)
		{
			Option option = menuItems[i];
			if (option.time > 0f)
			{
				ProgressBarUI.Open(option);
				return;
			}
		}
		if (menuItems.Count == 1)
		{
			menuItems[0].function.Invoke(player);
			return;
		}
		ContextMenuUI.Open(menuItems, ContextMenuUI.MenuType.Use);
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnUseStopped(BasePlayer player)
	{
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnRendered()
	{
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x0004364C File Offset: 0x0004184C
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		if (this.addedToParentEntity.IsValid())
		{
			this.addedToParentEntity.RemoveChild(this);
			this.addedToParentEntity = null;
		}
		if (this.children != null && this.children.Count > 0)
		{
			foreach (BaseEntity baseEntity in this.children)
			{
				if (baseEntity.IsValid())
				{
					baseEntity.transform.SetParent(null, false);
				}
			}
			this.children.Clear();
		}
		this.StopLerping();
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x000078DF File Offset: 0x00005ADF
	internal virtual Transform GetEyeTransform()
	{
		if (this.model == null)
		{
			return null;
		}
		return this.model.eyeBone;
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x000436FC File Offset: 0x000418FC
	public virtual bool NeedsCrosshair()
	{
		return this.GetMenuInformation(base.gameObject, LocalPlayer.Entity).IsValid;
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x00043724 File Offset: 0x00041924
	public override bool ShouldDestroyWithGroup()
	{
		BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity != null)
		{
			return parentEntity.ShouldDestroyWithGroup();
		}
		return base.ShouldDestroyWithGroup();
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x00043750 File Offset: 0x00041950
	public void DetachEffects()
	{
		List<EffectRecycle> list = Pool.GetList<EffectRecycle>();
		base.gameObject.GetComponentsInChildren<EffectRecycle>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<EffectRecycle>(ref list);
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnVoiceData(byte[] data)
	{
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x00043794 File Offset: 0x00041994
	public void GetEntityMenu(BasePlayer player, List<Option> optionList)
	{
		this.GetMenuOptions(optionList);
		for (int i = 0; i < this.Components.Length; i++)
		{
			this.Components[i].GetMenuOptions(optionList);
		}
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void GetMenuOptions(List<Option> list)
	{
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x06000654 RID: 1620 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool HasMenuOptions
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x000437CC File Offset: 0x000419CC
	public virtual void GetItemOptions(List<Option> options)
	{
		List<Option> menuItems = this.GetMenuItems(LocalPlayer.Entity);
		if (menuItems != null)
		{
			foreach (Option option in menuItems)
			{
				if (option.showOnItem)
				{
					options.Add(option);
				}
			}
		}
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x00043834 File Offset: 0x00041A34
	public BaseEntity.MovementModify GetMovementModify()
	{
		BaseEntity.MovementModify movementModify = default(BaseEntity.MovementModify);
		movementModify.drag = 0f;
		if (this.triggers == null)
		{
			return movementModify;
		}
		foreach (TriggerBase triggerBase in this.triggers)
		{
			TriggerMovement triggerMovement = triggerBase as TriggerMovement;
			if (!(triggerMovement == null))
			{
				movementModify.drag = Mathf.Max(triggerMovement.movementModify.drag, movementModify.drag);
			}
		}
		return movementModify;
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x000078FC File Offset: 0x00005AFC
	public virtual void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x00007903 File Offset: 0x00005B03
	protected void ReceiveCollisionMessages(bool b)
	{
		if (b)
		{
			TransformEx.GetOrAddComponent<EntityCollisionMessage>(base.gameObject.transform);
			return;
		}
		base.gameObject.transform.RemoveComponent<EntityCollisionMessage>();
	}

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x06000659 RID: 1625 RVA: 0x000438CC File Offset: 0x00041ACC
	public EntityComponentBase[] Components
	{
		get
		{
			EntityComponentBase[] result;
			if ((result = this._components) == null)
			{
				result = (this._components = base.GetComponentsInChildren<EntityComponentBase>(true));
			}
			return result;
		}
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x0000792A File Offset: 0x00005B2A
	public virtual BasePlayer ToPlayer()
	{
		return null;
	}

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x0600065B RID: 1627 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool IsNpc
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x0000792D File Offset: 0x00005B2D
	public override void InitShared()
	{
		base.InitShared();
		this.InitEntityLinks();
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x0000793B File Offset: 0x00005B3B
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.FreeEntityLinks();
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00007949 File Offset: 0x00005B49
	public override void ResetState()
	{
		base.ResetState();
		this.parentBone = 0U;
		this.OwnerID = 0UL;
		this.flags = (BaseEntity.Flags)0;
		this.parentEntity = default(EntityRef);
		this.addedToParentEntity = null;
		this.ragdoll = null;
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x0000481F File Offset: 0x00002A1F
	public virtual float InheritedVelocityScale()
	{
		return 0f;
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x000438F4 File Offset: 0x00041AF4
	public Vector3 GetInheritedProjectileVelocity()
	{
		BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (baseEntity == null)
		{
			return Vector3.zero;
		}
		return this.GetParentVelocity() * baseEntity.InheritedVelocityScale();
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x00007981 File Offset: 0x00005B81
	public Vector3 GetInheritedThrowVelocity()
	{
		return this.GetParentVelocity();
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x00043934 File Offset: 0x00041B34
	public Vector3 GetInheritedDropVelocity()
	{
		BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity();
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00043968 File Offset: 0x00041B68
	public Vector3 GetParentVelocity()
	{
		BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity() + (baseEntity.GetAngularVelocity() * base.transform.localPosition - base.transform.localPosition);
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x000439C8 File Offset: 0x00041BC8
	public Vector3 GetWorldVelocity()
	{
		BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return this.GetLocalVelocity();
		}
		return baseEntity.GetWorldVelocity() + (baseEntity.GetAngularVelocity() * base.transform.localPosition - base.transform.localPosition) + baseEntity.transform.TransformDirection(this.GetLocalVelocity());
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x00007989 File Offset: 0x00005B89
	public Vector3 GetLocalVelocity()
	{
		if (base.isClient)
		{
			return this.GetLocalVelocityClient();
		}
		return Vector3.zero;
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x0000799F File Offset: 0x00005B9F
	public Quaternion GetAngularVelocity()
	{
		if (base.isClient)
		{
			return this.GetAngularVelocityClient();
		}
		return Quaternion.identity;
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x000079B5 File Offset: 0x00005BB5
	public OBB WorldSpaceBounds()
	{
		return new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x000079E3 File Offset: 0x00005BE3
	public Vector3 PivotPoint()
	{
		return base.transform.position;
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x000079F0 File Offset: 0x00005BF0
	public Vector3 CenterPoint()
	{
		return this.WorldSpaceBounds().position;
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x00043A40 File Offset: 0x00041C40
	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.WorldSpaceBounds().ClosestPoint(position);
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x00043A5C File Offset: 0x00041C5C
	public float Distance(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).magnitude;
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00043A80 File Offset: 0x00041C80
	public float SqrDistance(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).sqrMagnitude;
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x000079FD File Offset: 0x00005BFD
	public float Distance(BaseEntity other)
	{
		return this.Distance(other.transform.position);
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00007A10 File Offset: 0x00005C10
	public float SqrDistance(BaseEntity other)
	{
		return this.SqrDistance(other.transform.position);
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x00007A23 File Offset: 0x00005C23
	public float Distance2D(Vector3 position)
	{
		return Vector3Ex.Magnitude2D(this.ClosestPoint(position) - position);
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00007A37 File Offset: 0x00005C37
	public float SqrDistance2D(Vector3 position)
	{
		return Vector3Ex.SqrMagnitude2D(this.ClosestPoint(position) - position);
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x000079FD File Offset: 0x00005BFD
	public float Distance2D(BaseEntity other)
	{
		return this.Distance(other.transform.position);
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00007A10 File Offset: 0x00005C10
	public float SqrDistance2D(BaseEntity other)
	{
		return this.SqrDistance(other.transform.position);
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x00043AA4 File Offset: 0x00041CA4
	public bool IsVisible(Ray ray, float maxDistance = float.PositiveInfinity)
	{
		if (Vector3Ex.IsNaNOrInfinity(ray.origin))
		{
			return false;
		}
		if (Vector3Ex.IsNaNOrInfinity(ray.direction))
		{
			return false;
		}
		if (ray.direction == Vector3.zero)
		{
			return false;
		}
		RaycastHit raycastHit;
		if (!this.WorldSpaceBounds().Trace(ray, ref raycastHit, maxDistance))
		{
			return false;
		}
		RaycastHit hit;
		if (GamePhysics.Trace(ray, 0f, out hit, maxDistance, 1218519041, 0))
		{
			BaseEntity entity = hit.GetEntity();
			if (entity == this)
			{
				return true;
			}
			if (entity != null && base.GetParentEntity() && base.GetParentEntity().EqualNetID(entity) && hit.collider != null && hit.collider.gameObject.layer == 13)
			{
				return true;
			}
			if (hit.distance <= raycastHit.distance)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00043B84 File Offset: 0x00041D84
	public bool IsVisible(Vector3 position, Vector3 target, float maxDistance = float.PositiveInfinity)
	{
		Vector3 a = target - position;
		float magnitude = a.magnitude;
		if (magnitude < Mathf.Epsilon)
		{
			return true;
		}
		Vector3 vector = a / magnitude;
		Vector3 b = vector * Mathf.Min(magnitude, 0.01f);
		return this.IsVisible(new Ray(position + b, vector), maxDistance);
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x00007A4B File Offset: 0x00005C4B
	public bool IsVisible(Vector3 position, float maxDistance = float.PositiveInfinity)
	{
		return this.IsVisible(position, this.CenterPoint(), maxDistance) || this.IsVisible(position, this.ClosestPoint(position), maxDistance);
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00043BDC File Offset: 0x00041DDC
	public bool IsOlderThan(BaseEntity other)
	{
		if (other == null)
		{
			return true;
		}
		uint num = (this.net == null) ? 0U : this.net.ID;
		uint num2 = (other.net == null) ? 0U : other.net.ID;
		return num < num2;
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00043C24 File Offset: 0x00041E24
	public virtual bool IsOutside()
	{
		OBB obb = this.WorldSpaceBounds();
		Vector3 position = obb.position + obb.up * obb.extents.y;
		return this.IsOutside(position);
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00007A6E File Offset: 0x00005C6E
	public bool IsOutside(Vector3 position)
	{
		return !Physics.Raycast(position, Vector3.up, 100f, 1101070337);
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x00043C64 File Offset: 0x00041E64
	public virtual float WaterFactor()
	{
		return WaterLevel.Factor(this.WorldSpaceBounds().ToBounds());
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0000481F File Offset: 0x00002A1F
	public virtual float Health()
	{
		return 0f;
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x0000481F File Offset: 0x00002A1F
	public virtual float MaxHealth()
	{
		return 0f;
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x0000481F File Offset: 0x00002A1F
	public virtual float MaxVelocity()
	{
		return 0f;
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00007A88 File Offset: 0x00005C88
	public virtual float BoundsPadding()
	{
		return 0.1f;
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00005B85 File Offset: 0x00003D85
	public virtual float PenetrationResistance(HitInfo info)
	{
		return 1f;
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x00007A8F File Offset: 0x00005C8F
	public virtual GameObjectRef GetImpactEffect(HitInfo info)
	{
		return this.impactEffect;
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnAttacked(HitInfo info)
	{
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x0000792A File Offset: 0x00005B2A
	public virtual Item GetItem()
	{
		return null;
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x0000792A File Offset: 0x00005B2A
	public virtual Item GetItem(uint itemId)
	{
		return null;
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x00007A97 File Offset: 0x00005C97
	public virtual void GiveItem(Item item, BaseEntity.GiveItemReason reason = BaseEntity.GiveItemReason.Generic)
	{
		item.Remove(0f);
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool CanBeLooted(BasePlayer player)
	{
		return true;
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x00004B3B File Offset: 0x00002D3B
	public virtual BaseEntity GetEntity()
	{
		return this;
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x00043C84 File Offset: 0x00041E84
	public override string ToString()
	{
		if (this._name == null)
		{
			if (base.isServer)
			{
				this._name = string.Format("{1}[{0}]", (this.net != null) ? this.net.ID : 0U, base.ShortPrefabName);
			}
			else
			{
				this._name = base.ShortPrefabName;
			}
		}
		return this._name;
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x00007AA4 File Offset: 0x00005CA4
	public virtual string Categorize()
	{
		return "entity";
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00043CE8 File Offset: 0x00041EE8
	public void Log(string str)
	{
		if (base.isClient)
		{
			Debug.Log(string.Concat(new string[]
			{
				"<color=#ffa>[",
				this.ToString(),
				"] ",
				str,
				"</color>"
			}), base.gameObject);
			return;
		}
		Debug.Log(string.Concat(new string[]
		{
			"<color=#aff>[",
			this.ToString(),
			"] ",
			str,
			"</color>"
		}), base.gameObject);
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x00007AAB File Offset: 0x00005CAB
	public void SetModel(Model mdl)
	{
		if (this.model == mdl)
		{
			return;
		}
		this.model = mdl;
		this.UpdateParenting();
		this.UpdateChildren();
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x00007ACF File Offset: 0x00005CCF
	public Model GetModel()
	{
		return this.model;
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x0000792A File Offset: 0x00005B2A
	public virtual Transform[] GetBones()
	{
		return null;
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00007AD7 File Offset: 0x00005CD7
	public virtual Transform FindBone(string strName)
	{
		if (this.model)
		{
			return this.model.FindBone(strName);
		}
		return base.transform;
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x00007AF9 File Offset: 0x00005CF9
	public virtual Transform FindClosestBone(Vector3 worldPos)
	{
		if (this.model)
		{
			return this.model.FindClosestBone(worldPos);
		}
		return base.transform;
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x0600068E RID: 1678 RVA: 0x00007B1B File Offset: 0x00005D1B
	// (set) Token: 0x0600068F RID: 1679 RVA: 0x00007B23 File Offset: 0x00005D23
	public ulong OwnerID { get; set; }

	// Token: 0x06000690 RID: 1680 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool ShouldBlockProjectiles()
	{
		return true;
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool ShouldInheritNetworkGroup()
	{
		return true;
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00043D74 File Offset: 0x00041F74
	public void BroadcastEntityMessage(string msg, float radius = 20f, int layerMask = 1218652417)
	{
		if (base.isClient)
		{
			return;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		global::Vis.Entities<BaseEntity>(base.transform.position, radius, list, layerMask, 2);
		foreach (BaseEntity baseEntity in list)
		{
			if (baseEntity.isServer)
			{
				baseEntity.OnEntityMessage(this, msg);
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnEntityMessage(BaseEntity from, string msg)
	{
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void DebugClient(int rep, float time)
	{
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x00043DF8 File Offset: 0x00041FF8
	public void OnDebugStart()
	{
		EntityDebug entityDebug = base.gameObject.GetComponent<EntityDebug>();
		if (entityDebug == null)
		{
			entityDebug = base.gameObject.AddComponent<EntityDebug>();
		}
		entityDebug.enabled = true;
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x00007B2C File Offset: 0x00005D2C
	protected void DebugText(Vector3 pos, string str, Color color, float time)
	{
		if (base.isClient)
		{
			UnityEngine.DDraw.Text(str, pos, color, time);
		}
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x00007B40 File Offset: 0x00005D40
	public bool HasFlag(BaseEntity.Flags f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x00043E30 File Offset: 0x00042030
	public bool ParentHasFlag(BaseEntity.Flags f)
	{
		BaseEntity parentEntity = base.GetParentEntity();
		return !(parentEntity == null) && parentEntity.HasFlag(f);
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00043E58 File Offset: 0x00042058
	public void SetFlag(BaseEntity.Flags f, bool b, bool recursive = false, bool networkupdate = true)
	{
		if (b)
		{
			if (this.HasFlag(f))
			{
				return;
			}
			this.flags |= f;
		}
		else
		{
			if (!this.HasFlag(f))
			{
				return;
			}
			this.flags &= ~f;
		}
		if (recursive && this.children != null)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].SetFlag(f, b, true, true);
			}
		}
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x000056A3 File Offset: 0x000038A3
	public bool IsOn()
	{
		return this.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x000061E1 File Offset: 0x000043E1
	public bool IsOpen()
	{
		return this.HasFlag(BaseEntity.Flags.Open);
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00007B4D File Offset: 0x00005D4D
	public bool IsOnFire()
	{
		return this.HasFlag(BaseEntity.Flags.OnFire);
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00007B56 File Offset: 0x00005D56
	public bool IsLocked()
	{
		return this.HasFlag(BaseEntity.Flags.Locked);
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00007B60 File Offset: 0x00005D60
	public override bool IsDebugging()
	{
		return this.HasFlag(BaseEntity.Flags.Debugging);
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00007B6A File Offset: 0x00005D6A
	public bool IsDisabled()
	{
		return this.HasFlag(BaseEntity.Flags.Disabled) || this.ParentHasFlag(BaseEntity.Flags.Disabled);
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00007B80 File Offset: 0x00005D80
	public bool IsBroken()
	{
		return this.HasFlag(BaseEntity.Flags.Broken);
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00007B8D File Offset: 0x00005D8D
	public bool IsBusy()
	{
		return this.HasFlag(BaseEntity.Flags.Busy);
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00007B9A File Offset: 0x00005D9A
	public override string GetLogColor()
	{
		if (base.isServer)
		{
			return "cyan";
		}
		return "yellow";
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00007BAF File Offset: 0x00005DAF
	public virtual void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		if (this.IsDebugging() && (old & BaseEntity.Flags.Debugging) != (next & BaseEntity.Flags.Debugging))
		{
			this.OnDebugStart();
		}
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x00043ED4 File Offset: 0x000420D4
	public bool IsOccupied(Socket_Base socket)
	{
		EntityLink entityLink = this.FindLink(socket);
		return entityLink != null && entityLink.IsOccupied();
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x00043EF4 File Offset: 0x000420F4
	public bool IsOccupied(string socketName)
	{
		EntityLink entityLink = this.FindLink(socketName);
		return entityLink != null && entityLink.IsOccupied();
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x00043F14 File Offset: 0x00042114
	public EntityLink FindLink(Socket_Base socket)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket == socket)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x00043F58 File Offset: 0x00042158
	public EntityLink FindLink(string socketName)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket.socketName == socketName)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x00043FA0 File Offset: 0x000421A0
	public T FindLinkedEntity<T>() where T : BaseEntity
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				EntityLink entityLink2 = entityLink.connections[j];
				if (entityLink2.owner is T)
				{
					return entityLink2.owner as T;
				}
			}
		}
		return default(T);
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x0004401C File Offset: 0x0004221C
	public void EntityLinkMessage<T>(Action<T> action) where T : BaseEntity
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				EntityLink entityLink2 = entityLink.connections[j];
				if (entityLink2.owner is T)
				{
					action.Invoke(entityLink2.owner as T);
				}
			}
		}
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00044094 File Offset: 0x00042294
	public void EntityLinkBroadcast<T>(Action<T> action) where T : BaseEntity
	{
		BaseEntity.globalBroadcastProtocol += 1U;
		BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
		BaseEntity.globalBroadcastQueue.Enqueue(this);
		if (this is T)
		{
			action.Invoke(this as T);
		}
		while (BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					BaseEntity owner = entityLink.connections[j].owner;
					if (owner.broadcastProtocol != BaseEntity.globalBroadcastProtocol)
					{
						owner.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
						BaseEntity.globalBroadcastQueue.Enqueue(owner);
						if (owner is T)
						{
							action.Invoke(owner as T);
						}
					}
				}
			}
		}
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x00044194 File Offset: 0x00042394
	public void EntityLinkBroadcast()
	{
		BaseEntity.globalBroadcastProtocol += 1U;
		BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
		BaseEntity.globalBroadcastQueue.Enqueue(this);
		while (BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					BaseEntity owner = entityLink.connections[j].owner;
					if (owner.broadcastProtocol != BaseEntity.globalBroadcastProtocol)
					{
						owner.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
						BaseEntity.globalBroadcastQueue.Enqueue(owner);
					}
				}
			}
		}
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x00007BC9 File Offset: 0x00005DC9
	public bool ReceivedEntityLinkBroadcast()
	{
		return this.broadcastProtocol == BaseEntity.globalBroadcastProtocol;
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x00007BD8 File Offset: 0x00005DD8
	public List<EntityLink> GetEntityLinks(bool linkToNeighbours = true)
	{
		if (!this.linkedToNeighbours && linkToNeighbours)
		{
			this.LinkToNeighbours();
		}
		return this.links;
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x00044258 File Offset: 0x00042458
	private void LinkToEntity(BaseEntity other)
	{
		if (this == other)
		{
			return;
		}
		if (this.links.Count == 0 || other.links.Count == 0)
		{
			return;
		}
		using (TimeWarning.New("LinkToEntity", 0.1f))
		{
			for (int i = 0; i < this.links.Count; i++)
			{
				EntityLink entityLink = this.links[i];
				for (int j = 0; j < other.links.Count; j++)
				{
					EntityLink entityLink2 = other.links[j];
					if (entityLink.CanConnect(entityLink2))
					{
						if (!entityLink.Contains(entityLink2))
						{
							entityLink.Add(entityLink2);
						}
						if (!entityLink2.Contains(entityLink))
						{
							entityLink2.Add(entityLink);
						}
					}
				}
			}
		}
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x0004432C File Offset: 0x0004252C
	private void LinkToNeighbours()
	{
		if (this.links.Count == 0)
		{
			return;
		}
		this.linkedToNeighbours = true;
		using (TimeWarning.New("LinkToNeighbours", 0.1f))
		{
			List<BaseEntity> list = Pool.GetList<BaseEntity>();
			OBB obb = this.WorldSpaceBounds();
			global::Vis.Entities<BaseEntity>(obb.position, obb.extents.magnitude + 1f, list, -1, 2);
			for (int i = 0; i < list.Count; i++)
			{
				BaseEntity baseEntity = list[i];
				if (baseEntity.isServer == base.isServer)
				{
					this.LinkToEntity(baseEntity);
				}
			}
			Pool.FreeList<BaseEntity>(ref list);
		}
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x000443E0 File Offset: 0x000425E0
	private void InitEntityLinks()
	{
		using (TimeWarning.New("InitEntityLinks", 0.1f))
		{
			if (base.isClient)
			{
				this.links.AddLinks(this, PrefabAttribute.client.FindAll<Socket_Base>(this.prefabID));
			}
		}
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x00044440 File Offset: 0x00042640
	private void FreeEntityLinks()
	{
		using (TimeWarning.New("FreeEntityLinks", 0.1f))
		{
			this.links.FreeLinks();
			this.linkedToNeighbours = false;
		}
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x0004448C File Offset: 0x0004268C
	public void RefreshEntityLinks()
	{
		using (TimeWarning.New("RefreshEntityLinks", 0.1f))
		{
			this.links.ClearLinks();
			this.LinkToNeighbours();
		}
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x00007BF3 File Offset: 0x00005DF3
	public void RequestFileFromServer(uint id, FileStorage.Type type, string responseFunction = "Cl_ReceiveFilePng")
	{
		if (id == 0U)
		{
			return;
		}
		this.ServerRPC<uint, byte, uint>("SV_RequestFile", id, (byte)type, StringPool.Get(responseFunction));
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x000444D8 File Offset: 0x000426D8
	[BaseEntity.RPC_Client]
	public void Cl_ReceiveFilePng(BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		byte[] array = msg.read.BytesWithSize();
		if (array == null)
		{
			return;
		}
		if (FileStorage.client.Store(array, FileStorage.Type.png, this.net.ID, 0U) != num)
		{
			Debug.LogWarning("Client/Server FileStorage CRC differs");
		}
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x00044528 File Offset: 0x00042728
	public void UpdateChildren()
	{
		if (this.children == null)
		{
			return;
		}
		foreach (BaseEntity baseEntity in this.children)
		{
			if (baseEntity.IsValid())
			{
				baseEntity.UpdateParenting();
			}
		}
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x00007C0D File Offset: 0x00005E0D
	public void UpdateParenting()
	{
		this.UpdateParenting(false);
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x0004458C File Offset: 0x0004278C
	public void UpdateParenting(bool worldPositionStays)
	{
		using (TimeWarning.New("UpdateParenting", 0.1f))
		{
			BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
			if (this.addedToParentEntity != baseEntity)
			{
				if (this.addedToParentEntity.IsValid())
				{
					this.addedToParentEntity.RemoveChild(this);
				}
				this.addedToParentEntity = this.parentEntity.Get(base.isServer);
				if (this.addedToParentEntity.IsValid())
				{
					this.addedToParentEntity.AddChild(this);
					if (this.net != null && this.addedToParentEntity.net != null)
					{
						this.net.SwitchGroup(this.addedToParentEntity.net.group);
					}
				}
				this.UpdateDisableState();
			}
			if (!this.parentEntity.IsSet())
			{
				if (!(base.transform.parent == null))
				{
					this.SetParentTransform(null, worldPositionStays);
				}
			}
			else if (baseEntity == null)
			{
				base.Invoke(new Action(this.UpdateParenting), 0f);
			}
			else
			{
				if (this.parentBone == 0U)
				{
					if (base.transform.parent == baseEntity.transform)
					{
						return;
					}
					this.SetParentTransform(baseEntity.transform, worldPositionStays);
				}
				else if (this.parentBone == StringPool.closest)
				{
					if (base.transform.parent == null)
					{
						this.SetParentTransform(baseEntity.transform, worldPositionStays);
					}
					Transform transform = baseEntity.FindClosestBone(base.transform.position);
					if (base.transform.parent == transform)
					{
						return;
					}
					this.SetParentTransform(transform, true);
				}
				else
				{
					Transform transform2 = baseEntity.FindBone(StringPool.Get(this.parentBone));
					if (base.transform.parent == transform2)
					{
						return;
					}
					this.SetParentTransform(transform2, worldPositionStays);
				}
				base.BroadcastMessage("RefreshLOD", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x0004479C File Offset: 0x0004299C
	private void SetParentTransform(Transform parent, bool worldPositionStays = true)
	{
		if (worldPositionStays && this.positionLerp != null)
		{
			if (base.transform.parent != null)
			{
				this.positionLerp.TransformEntries(base.transform.parent.localToWorldMatrix);
			}
			if (parent != null)
			{
				this.positionLerp.TransformEntries(parent.worldToLocalMatrix);
			}
		}
		base.transform.SetParent(parent, worldPositionStays);
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x00044810 File Offset: 0x00042A10
	protected void UpdateDisableState()
	{
		using (TimeWarning.New("UpdateDisableState", 0.1f))
		{
			if (!base.JustCreated)
			{
				bool flag = !base.gameObject.activeSelf;
				bool flag2 = this.IsDisabled();
				if (flag != flag2)
				{
					if (flag2)
					{
						this.DetachEffects();
					}
					base.gameObject.SetActive(!flag2);
					if (this.children != null)
					{
						foreach (BaseEntity baseEntity in this.children)
						{
							baseEntity.UpdateDisableState();
						}
					}
				}
			}
		}
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x000448CC File Offset: 0x00042ACC
	private bool HasClientTransformOffset()
	{
		if (!this.parentEntity.IsSet())
		{
			return false;
		}
		if (this.parentBone != StringPool.closest)
		{
			return false;
		}
		BaseEntity baseEntity = this.parentEntity.Get(false);
		return !(baseEntity == null) && !(base.transform.parent == null) && base.transform.parent != baseEntity.transform;
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x0004493C File Offset: 0x00042B3C
	private void RemoveClientTransformOffset()
	{
		BaseEntity baseEntity = this.parentEntity.Get(false);
		base.transform.SetParent(baseEntity.transform, true);
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x00044968 File Offset: 0x00042B68
	private void AddClientTransformOffset()
	{
		Transform parent = this.parentEntity.Get(false).FindClosestBone(base.transform.position);
		base.transform.SetParent(parent, true);
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x00007C16 File Offset: 0x00005E16
	public virtual BuildingPrivlidge GetBuildingPrivilege()
	{
		return this.GetBuildingPrivilege(this.WorldSpaceBounds());
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x000449A0 File Offset: 0x00042BA0
	public BuildingPrivlidge GetBuildingPrivilege(OBB obb)
	{
		BuildingBlock other = null;
		BuildingPrivlidge result = null;
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		global::Vis.Entities<BuildingBlock>(obb.position, 16f + obb.extents.magnitude, list, 2097152, 2);
		for (int i = 0; i < list.Count; i++)
		{
			BuildingBlock buildingBlock = list[i];
			if (buildingBlock.isServer == base.isServer && buildingBlock.IsOlderThan(other) && obb.Distance(buildingBlock.WorldSpaceBounds()) <= 16f)
			{
				BuildingManager.Building building = buildingBlock.GetBuilding();
				if (building != null)
				{
					BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
					if (!(dominatingBuildingPrivilege == null))
					{
						other = buildingBlock;
						result = dominatingBuildingPrivilege;
					}
				}
			}
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return result;
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00044A54 File Offset: 0x00042C54
	public void ServerRPC<T1, T2, T3, T4, T5>(SendMethod sendMethod, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (Net.cl == null || !Net.cl.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ServerRPCStart(funcName))
		{
			this.ServerRPCWrite<T1>(arg1);
			this.ServerRPCWrite<T2>(arg2);
			this.ServerRPCWrite<T3>(arg3);
			this.ServerRPCWrite<T4>(arg4);
			this.ServerRPCWrite<T5>(arg5);
			SendInfo sendInfo;
			sendInfo..ctor(Net.cl.Connection);
			sendInfo.priority = 0;
			this.ServerRPCSend(sendInfo);
		}
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x00044AD0 File Offset: 0x00042CD0
	public void ServerRPC<T1, T2, T3, T4>(SendMethod sendMethod, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (Net.cl == null || !Net.cl.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ServerRPCStart(funcName))
		{
			this.ServerRPCWrite<T1>(arg1);
			this.ServerRPCWrite<T2>(arg2);
			this.ServerRPCWrite<T3>(arg3);
			this.ServerRPCWrite<T4>(arg4);
			SendInfo sendInfo;
			sendInfo..ctor(Net.cl.Connection);
			sendInfo.priority = 0;
			this.ServerRPCSend(sendInfo);
		}
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00044B44 File Offset: 0x00042D44
	public void ServerRPC<T1, T2, T3>(SendMethod sendMethod, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (Net.cl == null || !Net.cl.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ServerRPCStart(funcName))
		{
			this.ServerRPCWrite<T1>(arg1);
			this.ServerRPCWrite<T2>(arg2);
			this.ServerRPCWrite<T3>(arg3);
			SendInfo sendInfo;
			sendInfo..ctor(Net.cl.Connection);
			sendInfo.priority = 0;
			this.ServerRPCSend(sendInfo);
		}
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00044BB0 File Offset: 0x00042DB0
	public void ServerRPC<T1, T2>(SendMethod sendMethod, string funcName, T1 arg1, T2 arg2)
	{
		if (Net.cl == null || !Net.cl.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ServerRPCStart(funcName))
		{
			this.ServerRPCWrite<T1>(arg1);
			this.ServerRPCWrite<T2>(arg2);
			SendInfo sendInfo;
			sendInfo..ctor(Net.cl.Connection);
			sendInfo.priority = 0;
			this.ServerRPCSend(sendInfo);
		}
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x00044C14 File Offset: 0x00042E14
	public void ServerRPC<T1>(SendMethod sendMethod, string funcName, T1 arg1)
	{
		if (Net.cl == null || !Net.cl.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ServerRPCStart(funcName))
		{
			this.ServerRPCWrite<T1>(arg1);
			SendInfo sendInfo;
			sendInfo..ctor(Net.cl.Connection);
			sendInfo.priority = 0;
			this.ServerRPCSend(sendInfo);
		}
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x00044C70 File Offset: 0x00042E70
	public void ServerRPC(SendMethod sendMethod, string funcName)
	{
		if (Net.cl == null || !Net.cl.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ServerRPCStart(funcName))
		{
			SendInfo sendInfo;
			sendInfo..ctor(Net.cl.Connection);
			sendInfo.priority = 0;
			this.ServerRPCSend(sendInfo);
		}
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x00007C24 File Offset: 0x00005E24
	public void ServerRPC<T1, T2, T3, T4, T5>(string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		this.ServerRPC<T1, T2, T3, T4, T5>(0, funcName, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x00007C36 File Offset: 0x00005E36
	public void ServerRPC<T1, T2, T3, T4>(string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		this.ServerRPC<T1, T2, T3, T4>(0, funcName, arg1, arg2, arg3, arg4);
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00007C46 File Offset: 0x00005E46
	public void ServerRPC<T1, T2, T3>(string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		this.ServerRPC<T1, T2, T3>(0, funcName, arg1, arg2, arg3);
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x00007C54 File Offset: 0x00005E54
	public void ServerRPC<T1, T2>(string funcName, T1 arg1, T2 arg2)
	{
		this.ServerRPC<T1, T2>(0, funcName, arg1, arg2);
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x00007C60 File Offset: 0x00005E60
	public void ServerRPC<T1>(string funcName, T1 arg1)
	{
		this.ServerRPC<T1>(0, funcName, arg1);
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x00007C6B File Offset: 0x00005E6B
	public void ServerRPC(string funcName)
	{
		this.ServerRPC(0, funcName);
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x00044CC4 File Offset: 0x00042EC4
	private bool ServerRPCStart(string funcName)
	{
		if (Net.cl.write.Start())
		{
			Net.cl.write.PacketID(9);
			Net.cl.write.UInt32(this.net.ID);
			Net.cl.write.UInt32(StringPool.Get(funcName));
			return true;
		}
		return false;
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x00007C75 File Offset: 0x00005E75
	private void ServerRPCWrite<T>(T arg)
	{
		Net.cl.write.WriteObject(arg);
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x00007C87 File Offset: 0x00005E87
	private void ServerRPCSend(SendInfo sendInfo)
	{
		Net.cl.write.Send(sendInfo);
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x00044D28 File Offset: 0x00042F28
	public void ServerRPCList<T1, T2, T3, T4, T5>(string funcName, List<T1> list, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (Net.cl == null || !Net.cl.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ServerRPCStart(funcName))
		{
			this.ServerRPCWrite<int>(list.Count);
			foreach (T1 arg6 in list)
			{
				this.ServerRPCWrite<T1>(arg6);
			}
			this.ServerRPCWrite<T2>(arg2);
			this.ServerRPCWrite<T3>(arg3);
			this.ServerRPCWrite<T4>(arg4);
			this.ServerRPCWrite<T5>(arg5);
			SendInfo sendInfo;
			sendInfo..ctor(Net.cl.Connection);
			sendInfo.priority = 0;
			this.ServerRPCSend(sendInfo);
		}
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x00044DE8 File Offset: 0x00042FE8
	public void CL_RPCMessage(uint nameID, ulong sourceConnection, Message message)
	{
		if (LocalPlayer.Entity && LocalPlayer.Entity.userID == sourceConnection && ConVar.Client.prediction)
		{
			return;
		}
		if (this.OnRpcMessage(null, nameID, message))
		{
			return;
		}
		for (int i = 0; i < this.Components.Length; i++)
		{
			if (this.Components[i].OnRpcMessage(null, nameID, message))
			{
				return;
			}
		}
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00044E48 File Offset: 0x00043048
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseEntity != null)
		{
			BaseEntity baseEntity = info.msg.baseEntity;
			BaseEntity.Flags old = this.flags;
			this.flags = (BaseEntity.Flags)baseEntity.flags;
			this.OnFlagsChanged(old, this.flags);
			this.OnSkinChanged(this.skinID, info.msg.baseEntity.skinid);
			if (info.fromDisk)
			{
				if (Vector3Ex.IsNaNOrInfinity(baseEntity.pos))
				{
					Debug.LogWarning(this.ToString() + " has broken position - " + baseEntity.pos);
					baseEntity.pos = Vector3.zero;
				}
				base.transform.localPosition = baseEntity.pos;
				base.transform.localRotation = Quaternion.Euler(baseEntity.rot);
			}
		}
		if (info.msg.entitySlots != null)
		{
			this.entitySlots[0].uid = info.msg.entitySlots.slotLock;
			this.entitySlots[1].uid = info.msg.entitySlots.slotFireMod;
			this.entitySlots[2].uid = info.msg.entitySlots.slotUpperModification;
			this.entitySlots[5].uid = info.msg.entitySlots.centerDecoration;
			this.entitySlots[6].uid = info.msg.entitySlots.lowerCenterDecoration;
		}
		if (info.msg.parent != null)
		{
			this.parentEntity.uid = info.msg.parent.uid;
			this.parentBone = info.msg.parent.bone;
		}
		else
		{
			this.parentEntity.uid = 0U;
			this.parentBone = 0U;
		}
		if (base.isClient)
		{
			if (info.msg.baseEntity != null)
			{
				this.UpdateParenting(true);
				this.OnPositionalFromNetwork(info.msg.baseEntity.pos, info.msg.baseEntity.rot, info.msg.baseEntity.time);
			}
			else
			{
				this.UpdateParenting();
			}
		}
		if (info.msg.ownerInfo != null)
		{
			this.OwnerID = info.msg.ownerInfo.steamid;
		}
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00007C99 File Offset: 0x00005E99
	public void SendSignalBroadcast(BaseEntity.Signal signal, string arg = "")
	{
		this.ServerRPC<int, string>("BroadcastSignalFromClient", (int)signal, arg);
		if (ConVar.Client.prediction)
		{
			this.OnSignal(signal, arg);
		}
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x000450A0 File Offset: 0x000432A0
	[BaseEntity.RPC_Client]
	private void SignalFromServerEx(BaseEntity.RPCMessage msg)
	{
		BaseEntity.Signal signal = (BaseEntity.Signal)msg.read.Int32();
		string arg = msg.read.String();
		this.OnSignal(signal, arg);
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x000450D0 File Offset: 0x000432D0
	[BaseEntity.RPC_Client]
	private void SignalFromServer(BaseEntity.RPCMessage msg)
	{
		BaseEntity.Signal signal = (BaseEntity.Signal)msg.read.Int32();
		this.OnSignal(signal, null);
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void OnSignal(BaseEntity.Signal signal, string arg)
	{
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x000450F4 File Offset: 0x000432F4
	private void OnSkinChanged(ulong oldSkinID, ulong newSkinID)
	{
		if (oldSkinID == newSkinID)
		{
			return;
		}
		this.skinID = newSkinID;
		if (base.isClient)
		{
			this.ResetSkin();
			if (newSkinID == 0UL)
			{
				return;
			}
			ItemSkinDirectory.Skin skin = ItemSkinDirectory.FindByInventoryDefinitionId((int)newSkinID);
			if (skin.id == 0)
			{
				this.OnSkinRefreshStart();
				WorkshopSkin.Apply(base.gameObject, newSkinID, new Action(this.OnSkinRefreshEnd));
				return;
			}
			if ((long)skin.id == (long)newSkinID)
			{
				this.itemSkin = (skin.invItem as ItemSkin);
				if (this.itemSkin != null && this.model != null)
				{
					this.itemSkin.ApplySkin(this.model.gameObject);
				}
			}
		}
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00007CB7 File Offset: 0x00005EB7
	private void ResetSkin()
	{
		MaterialReplacement.Reset(base.gameObject);
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00007CC4 File Offset: 0x00005EC4
	private void OnSkinRefreshStart()
	{
		base.gameObject.BroadcastBatchingToggle(false);
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00007CD2 File Offset: 0x00005ED2
	private void OnSkinRefreshEnd()
	{
		base.gameObject.BroadcastBatchingToggle(true);
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00007CE0 File Offset: 0x00005EE0
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside && Skinnable.All != null && Skinnable.FindForEntity(name) != null)
		{
			WorkshopSkin.Prepare(rootObj);
			MaterialReplacement.Prepare(rootObj);
		}
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x000451A0 File Offset: 0x000433A0
	public bool HasAnySlot()
	{
		for (int i = 0; i < this.entitySlots.Length; i++)
		{
			if (this.entitySlots[i].IsValid(base.isServer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00007D07 File Offset: 0x00005F07
	public BaseEntity GetSlot(BaseEntity.Slot slot)
	{
		return this.entitySlots[(int)slot].Get(base.isServer);
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00007D20 File Offset: 0x00005F20
	public string GetSlotAnchorName(BaseEntity.Slot slot)
	{
		return slot.ToString().ToLower();
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x000451DC File Offset: 0x000433DC
	public Transform GetSlotAnchor(BaseEntity.Slot slot)
	{
		string slotAnchorName = this.GetSlotAnchorName(slot);
		return this.FindBone(slotAnchorName);
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool HasSlot(BaseEntity.Slot slot)
	{
		return false;
	}

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x060006E0 RID: 1760 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.None;
		}
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x00007D34 File Offset: 0x00005F34
	public bool HasTrait(BaseEntity.TraitFlag f)
	{
		return (this.Traits & f) == f;
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00007D41 File Offset: 0x00005F41
	public bool HasAnyTrait(BaseEntity.TraitFlag f)
	{
		return (this.Traits & f) > BaseEntity.TraitFlag.None;
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00007D4E File Offset: 0x00005F4E
	public virtual bool EnterTrigger(TriggerBase trigger)
	{
		if (this.triggers == null)
		{
			this.triggers = Pool.Get<List<TriggerBase>>();
		}
		this.triggers.Add(trigger);
		return true;
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00007D70 File Offset: 0x00005F70
	public virtual void LeaveTrigger(TriggerBase trigger)
	{
		if (this.triggers == null)
		{
			return;
		}
		this.triggers.Remove(trigger);
		if (this.triggers.Count == 0)
		{
			Pool.FreeList<TriggerBase>(ref this.triggers);
		}
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x000451F8 File Offset: 0x000433F8
	public void RemoveFromTriggers()
	{
		if (this.triggers == null)
		{
			return;
		}
		using (TimeWarning.New("RemoveFromTriggers", 0.1f))
		{
			foreach (TriggerBase triggerBase in this.triggers.ToArray())
			{
				if (triggerBase)
				{
					triggerBase.RemoveEntity(this);
				}
			}
			if (this.triggers != null && this.triggers.Count == 0)
			{
				Pool.FreeList<TriggerBase>(ref this.triggers);
			}
		}
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x00045288 File Offset: 0x00043488
	public T FindTrigger<T>() where T : TriggerBase
	{
		if (this.triggers == null)
		{
			return default(T);
		}
		foreach (TriggerBase triggerBase in this.triggers)
		{
			if (!(triggerBase as T == null))
			{
				return triggerBase as T;
			}
		}
		return default(T);
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00007DA0 File Offset: 0x00005FA0
	public virtual void MakeVisible()
	{
		this.isVisible = true;
		this.isAnimatorVisible = true;
		this.isShadowVisible = true;
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x00045318 File Offset: 0x00043518
	protected virtual void UpdateCullingSpheres()
	{
		Vector3 position = this.CenterPoint();
		float magnitude = Vector3.Scale(this.bounds.extents, base.transform.lossyScale).magnitude;
		this.localOccludee.sphere = new OcclusionCulling.Sphere(position, magnitude);
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x00045364 File Offset: 0x00043564
	protected virtual void RegisterForCulling()
	{
		this.UpdateCullingSpheres();
		if (this.localOccludee.IsRegistered)
		{
			this.UnregisterFromCulling();
		}
		int num = OcclusionCulling.RegisterOccludee(this.localOccludee.sphere.position, this.localOccludee.sphere.radius, this.isVisible, 0.25f, false, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
		if (num >= 0)
		{
			this.localOccludee = new OccludeeSphere(num, this.localOccludee.sphere);
			return;
		}
		this.localOccludee.Invalidate();
		Debug.LogWarning("[OcclusionCulling] Occludee registration failed for " + base.name + ". Too many registered.");
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00002ECE File Offset: 0x000010CE
	protected virtual void OnVisibilityChanged(bool visible)
	{
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x00007DB7 File Offset: 0x00005FB7
	protected virtual void UnregisterFromCulling()
	{
		if (this.localOccludee.IsRegistered)
		{
			OcclusionCulling.UnregisterOccludee(this.localOccludee.id);
			this.localOccludee.id = -1;
		}
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x00045418 File Offset: 0x00043618
	protected virtual void UpdateCullingBounds()
	{
		this.UpdateCullingSpheres();
		if (this.localOccludee.IsRegistered)
		{
			OcclusionCulling.UpdateDynamicOccludee(this.localOccludee.id, this.localOccludee.sphere.position, this.localOccludee.sphere.radius);
		}
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00007DE2 File Offset: 0x00005FE2
	protected virtual bool CheckVisibility()
	{
		return this.localOccludee.state == null || this.localOccludee.state.isVisible;
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00007E03 File Offset: 0x00006003
	protected float CalcEntityVisUpdateRate()
	{
		return 1f / Mathf.Clamp(Culling.entityUpdateRate, 0.015f, float.PositiveInfinity);
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x00045468 File Offset: 0x00043668
	protected virtual void DebugDrawCullingBounds()
	{
		if (Culling.toggle && this.localOccludee.IsRegistered)
		{
			Gizmos.color = (this.isVisible ? Color.blue : Color.red);
			Gizmos.DrawWireSphere(this.localOccludee.sphere.position, this.localOccludee.sphere.radius);
		}
	}

	// Token: 0x02000059 RID: 89
	public class Menu : Attribute
	{
		// Token: 0x04000357 RID: 855
		public string TitleToken;

		// Token: 0x04000358 RID: 856
		public string TitleEnglish;

		// Token: 0x04000359 RID: 857
		public string UseVariable;

		// Token: 0x0400035A RID: 858
		public int Order;

		// Token: 0x0400035B RID: 859
		public string ProxyFunction;

		// Token: 0x0400035C RID: 860
		public float Time;

		// Token: 0x0400035D RID: 861
		public string OnStart;

		// Token: 0x0400035E RID: 862
		public string OnProgress;

		// Token: 0x0400035F RID: 863
		public bool LongUseOnly;

		// Token: 0x060006F2 RID: 1778 RVA: 0x00007E31 File Offset: 0x00006031
		public Menu()
		{
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x00007E39 File Offset: 0x00006039
		public Menu(string menuTitleToken, string menuTitleEnglish)
		{
			this.TitleToken = menuTitleToken;
			this.TitleEnglish = menuTitleEnglish;
		}

		// Token: 0x0200005A RID: 90
		[Serializable]
		public struct Option
		{
			// Token: 0x04000360 RID: 864
			public Translate.Phrase name;

			// Token: 0x04000361 RID: 865
			public Translate.Phrase description;

			// Token: 0x04000362 RID: 866
			public Sprite icon;

			// Token: 0x04000363 RID: 867
			public int order;

			// Token: 0x060006F4 RID: 1780 RVA: 0x00007E4F File Offset: 0x0000604F
			public void CopyTo(ref GameMenu.Option option)
			{
				option.title = this.name.token;
				option.desc = this.description.token;
				option.iconSprite = this.icon;
				option.order = this.order;
			}
		}

		// Token: 0x0200005B RID: 91
		public class Description : Attribute
		{
			// Token: 0x04000364 RID: 868
			public string token;

			// Token: 0x04000365 RID: 869
			public string english;

			// Token: 0x060006F5 RID: 1781 RVA: 0x00007E8B File Offset: 0x0000608B
			public Description(string t, string e)
			{
				this.token = t;
				this.english = e;
			}
		}

		// Token: 0x0200005C RID: 92
		public class Icon : Attribute
		{
			// Token: 0x04000366 RID: 870
			public string icon;

			// Token: 0x060006F6 RID: 1782 RVA: 0x00007EA1 File Offset: 0x000060A1
			public Icon(string i)
			{
				this.icon = i;
			}
		}

		// Token: 0x0200005D RID: 93
		public class ShowIf : Attribute
		{
			// Token: 0x04000367 RID: 871
			public string functionName;

			// Token: 0x060006F7 RID: 1783 RVA: 0x00007EB0 File Offset: 0x000060B0
			public ShowIf(string testFunc)
			{
				this.functionName = testFunc;
			}
		}
	}

	// Token: 0x0200005E RID: 94
	[Serializable]
	public struct MovementModify
	{
		// Token: 0x04000368 RID: 872
		public float drag;
	}

	// Token: 0x0200005F RID: 95
	public enum GiveItemReason
	{
		// Token: 0x0400036A RID: 874
		Generic,
		// Token: 0x0400036B RID: 875
		ResourceHarvested,
		// Token: 0x0400036C RID: 876
		PickedUp,
		// Token: 0x0400036D RID: 877
		Crafted
	}

	// Token: 0x02000060 RID: 96
	[Flags]
	public enum Flags
	{
		// Token: 0x0400036F RID: 879
		Placeholder = 1,
		// Token: 0x04000370 RID: 880
		On = 2,
		// Token: 0x04000371 RID: 881
		OnFire = 4,
		// Token: 0x04000372 RID: 882
		Open = 8,
		// Token: 0x04000373 RID: 883
		Locked = 16,
		// Token: 0x04000374 RID: 884
		Debugging = 32,
		// Token: 0x04000375 RID: 885
		Disabled = 64,
		// Token: 0x04000376 RID: 886
		Reserved1 = 128,
		// Token: 0x04000377 RID: 887
		Reserved2 = 256,
		// Token: 0x04000378 RID: 888
		Reserved3 = 512,
		// Token: 0x04000379 RID: 889
		Reserved4 = 1024,
		// Token: 0x0400037A RID: 890
		Reserved5 = 2048,
		// Token: 0x0400037B RID: 891
		Broken = 4096,
		// Token: 0x0400037C RID: 892
		Busy = 8192,
		// Token: 0x0400037D RID: 893
		Reserved6 = 16384,
		// Token: 0x0400037E RID: 894
		Reserved7 = 32768,
		// Token: 0x0400037F RID: 895
		Reserved8 = 65536
	}

	// Token: 0x02000061 RID: 97
	public static class Query
	{
		// Token: 0x02000062 RID: 98
		public class EntityTree
		{
			// Token: 0x04000380 RID: 896
			private Grid<BaseEntity> Grid;

			// Token: 0x04000381 RID: 897
			private Grid<BasePlayer> PlayerGrid;

			// Token: 0x060006F8 RID: 1784 RVA: 0x00007EBF File Offset: 0x000060BF
			public EntityTree(float worldSize)
			{
				this.Grid = new Grid<BaseEntity>(32, worldSize);
				this.PlayerGrid = new Grid<BasePlayer>(32, worldSize);
			}

			// Token: 0x060006F9 RID: 1785 RVA: 0x0004551C File Offset: 0x0004371C
			public void Add(BaseEntity ent)
			{
				Vector3 position = ent.transform.position;
				this.Grid.Add(ent, position.x, position.z);
			}

			// Token: 0x060006FA RID: 1786 RVA: 0x00045550 File Offset: 0x00043750
			public void AddPlayer(BasePlayer player)
			{
				Vector3 position = player.transform.position;
				this.PlayerGrid.Add(player, position.x, position.z);
			}

			// Token: 0x060006FB RID: 1787 RVA: 0x00045584 File Offset: 0x00043784
			public void Remove(BaseEntity ent, bool isPlayer = false)
			{
				this.Grid.Remove(ent);
				if (isPlayer)
				{
					BasePlayer basePlayer = ent as BasePlayer;
					if (basePlayer != null)
					{
						this.PlayerGrid.Remove(basePlayer);
					}
				}
			}

			// Token: 0x060006FC RID: 1788 RVA: 0x00007EE3 File Offset: 0x000060E3
			public void RemovePlayer(BasePlayer player)
			{
				this.PlayerGrid.Remove(player);
			}

			// Token: 0x060006FD RID: 1789 RVA: 0x000455C0 File Offset: 0x000437C0
			public void Move(BaseEntity ent)
			{
				Vector3 position = ent.transform.position;
				this.Grid.Move(ent, position.x, position.z);
				BasePlayer basePlayer = ent as BasePlayer;
				if (basePlayer != null)
				{
					this.MovePlayer(basePlayer);
				}
			}

			// Token: 0x060006FE RID: 1790 RVA: 0x00045608 File Offset: 0x00043808
			public void MovePlayer(BasePlayer player)
			{
				Vector3 position = player.transform.position;
				this.PlayerGrid.Move(player, position.x, position.z);
			}

			// Token: 0x060006FF RID: 1791 RVA: 0x00007EF2 File Offset: 0x000060F2
			public int GetInSphere(Vector3 position, float distance, BaseEntity[] results, Func<BaseEntity, bool> filter = null)
			{
				return this.Grid.Query(position.x, position.z, distance, results, filter);
			}

			// Token: 0x06000700 RID: 1792 RVA: 0x00007F0F File Offset: 0x0000610F
			public int GetPlayersInSphere(Vector3 position, float distance, BasePlayer[] results, Func<BasePlayer, bool> filter = null)
			{
				return this.PlayerGrid.Query(position.x, position.z, distance, results, filter);
			}
		}
	}

	// Token: 0x02000063 RID: 99
	public class RPC_Shared : Attribute
	{
	}

	// Token: 0x02000064 RID: 100
	public struct RPCMessage
	{
		// Token: 0x04000382 RID: 898
		public Connection connection;

		// Token: 0x04000383 RID: 899
		public BasePlayer player;

		// Token: 0x04000384 RID: 900
		public Read read;
	}

	// Token: 0x02000065 RID: 101
	public class RPC_Client : BaseEntity.RPC_Shared
	{
	}

	// Token: 0x02000066 RID: 102
	public enum Signal
	{
		// Token: 0x04000386 RID: 902
		Attack,
		// Token: 0x04000387 RID: 903
		Alt_Attack,
		// Token: 0x04000388 RID: 904
		DryFire,
		// Token: 0x04000389 RID: 905
		Reload,
		// Token: 0x0400038A RID: 906
		Deploy,
		// Token: 0x0400038B RID: 907
		Flinch_Head,
		// Token: 0x0400038C RID: 908
		Flinch_Chest,
		// Token: 0x0400038D RID: 909
		Flinch_Stomach,
		// Token: 0x0400038E RID: 910
		Flinch_RearHead,
		// Token: 0x0400038F RID: 911
		Flinch_RearTorso,
		// Token: 0x04000390 RID: 912
		Throw,
		// Token: 0x04000391 RID: 913
		Relax,
		// Token: 0x04000392 RID: 914
		Gesture,
		// Token: 0x04000393 RID: 915
		PhysImpact,
		// Token: 0x04000394 RID: 916
		Eat,
		// Token: 0x04000395 RID: 917
		Startled
	}

	// Token: 0x02000067 RID: 103
	public enum Slot
	{
		// Token: 0x04000397 RID: 919
		Lock,
		// Token: 0x04000398 RID: 920
		FireMod,
		// Token: 0x04000399 RID: 921
		UpperModifier,
		// Token: 0x0400039A RID: 922
		MiddleModifier,
		// Token: 0x0400039B RID: 923
		LowerModifier,
		// Token: 0x0400039C RID: 924
		CenterDecoration,
		// Token: 0x0400039D RID: 925
		LowerCenterDecoration,
		// Token: 0x0400039E RID: 926
		Count
	}

	// Token: 0x02000068 RID: 104
	[Flags]
	public enum TraitFlag
	{
		// Token: 0x040003A0 RID: 928
		None = 0,
		// Token: 0x040003A1 RID: 929
		Alive = 1,
		// Token: 0x040003A2 RID: 930
		Animal = 2,
		// Token: 0x040003A3 RID: 931
		Human = 4,
		// Token: 0x040003A4 RID: 932
		Interesting = 8,
		// Token: 0x040003A5 RID: 933
		Food = 16,
		// Token: 0x040003A6 RID: 934
		Meat = 32,
		// Token: 0x040003A7 RID: 935
		Water = 32
	}

	// Token: 0x02000069 RID: 105
	public static class Util
	{
	}
}
