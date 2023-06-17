using System;
using System.Collections.Generic;
using ConVar;
using Network;
using ProtoBuf;
using Rust.Ai.HTN;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class HTNAnimal : BaseCombatEntity
{
	// Token: 0x040005CF RID: 1487
	[Header("Client Animation")]
	public Vector3 HipFudge = new Vector3(-90f, 0f, 90f);

	// Token: 0x040005D0 RID: 1488
	public Transform HipBone;

	// Token: 0x040005D1 RID: 1489
	public Transform LookBone;

	// Token: 0x040005D2 RID: 1490
	public bool UpdateWalkSpeed = true;

	// Token: 0x040005D3 RID: 1491
	public bool UpdateFacingDirection = true;

	// Token: 0x040005D4 RID: 1492
	public bool UpdateGroundNormal = true;

	// Token: 0x040005D5 RID: 1493
	public Transform alignmentRoot;

	// Token: 0x040005D6 RID: 1494
	public bool LaggyAss = true;

	// Token: 0x040005D7 RID: 1495
	public bool LookAtTarget;

	// Token: 0x040005D8 RID: 1496
	public float MaxLaggyAssRotation = 70f;

	// Token: 0x040005D9 RID: 1497
	public float MaxWalkAnimSpeed = 25f;

	// Token: 0x040005DA RID: 1498
	private static int ParamWalkSpeed = Animator.StringToHash("speed");

	// Token: 0x040005DB RID: 1499
	private static int ParamWalkSpeedAverage = Animator.StringToHash("speedAvg");

	// Token: 0x040005DC RID: 1500
	private AverageVelocity AverageVelocity = new AverageVelocity();

	// Token: 0x040005DD RID: 1501
	private Vector3 oldPosition;

	// Token: 0x040005DE RID: 1502
	private Quaternion hipForward;

	// Token: 0x040005DF RID: 1503
	private Quaternion baseHipRotation;

	// Token: 0x040005E0 RID: 1504
	private Quaternion baseLookRotation;

	// Token: 0x040005E1 RID: 1505
	private Vector3 targetUp;

	// Token: 0x040005E2 RID: 1506
	private Vector3 targetOffset;

	// Token: 0x040005E3 RID: 1507
	private Animator _animator;

	// Token: 0x040005E4 RID: 1508
	[NonSerialized]
	public Quaternion NetworkRotation;

	// Token: 0x040005E5 RID: 1509
	private static List<HTNAnimal> visibleNpcList = new List<HTNAnimal>();

	// Token: 0x040005E6 RID: 1510
	[Header("Client Effects")]
	public MaterialEffect FootstepEffects;

	// Token: 0x040005E7 RID: 1511
	public Transform[] Feet;

	// Token: 0x040005E8 RID: 1512
	[ReadOnly]
	public string BaseFolder;

	// Token: 0x040005E9 RID: 1513
	private float nextVisThink;

	// Token: 0x040005EA RID: 1514
	private float lastTimeSeen;

	// Token: 0x040005EB RID: 1515
	private Vector3 lastPosition;

	// Token: 0x040005EC RID: 1516
	[Header("Hierarchical Task Network")]
	public HTNDomain _aiDomain;

	// Token: 0x040005ED RID: 1517
	[Header("Ai Definition")]
	public BaseNpcDefinition _aiDefinition;

	// Token: 0x060008FD RID: 2301 RVA: 0x0004EC90 File Offset: 0x0004CE90
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HTNAnimal.OnRpcMessage", 0.1f))
		{
			if (rpc == 1339610199U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: PlayAnimationBool ");
				}
				using (TimeWarning.New("PlayAnimationBool", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							string animationStr = msg.read.String();
							int value = msg.read.Int32();
							this.PlayAnimationBool(animationStr, value);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in PlayAnimationBool", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
			if (rpc == 972001481U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: PlayAnimationInt ");
				}
				using (TimeWarning.New("PlayAnimationInt", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							string animationStr2 = msg.read.String();
							int value2 = msg.read.Int32();
							this.PlayAnimationInt(animationStr2, value2);
						}
					}
					catch (Exception exception2)
					{
						Net.cl.Disconnect("RPC Error in PlayAnimationInt", true);
						Debug.LogException(exception2);
					}
				}
				return true;
			}
			if (rpc == 29352892U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: PlayAnimationTrigger ");
				}
				using (TimeWarning.New("PlayAnimationTrigger", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							string animationStr3 = msg.read.String();
							this.PlayAnimationTrigger(animationStr3);
						}
					}
					catch (Exception exception3)
					{
						Net.cl.Disconnect("RPC Error in PlayAnimationTrigger", true);
						Debug.LogException(exception3);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x0004EF90 File Offset: 0x0004D190
	private void InitializeAnimation()
	{
		if (base.isServer)
		{
			return;
		}
		if (this.HipBone)
		{
			this.hipForward = this.HipBone.rotation;
			this.baseHipRotation = this.HipBone.localRotation;
		}
		if (this.LookBone)
		{
			this.baseLookRotation = this.LookBone.localRotation;
		}
		if (this.model)
		{
			this._animator = this.model.GetComponent<Animator>();
		}
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x0004F014 File Offset: 0x0004D214
	private void UpdateAnimation()
	{
		if (base.isServer)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 vector = localPosition - this.oldPosition;
		this.oldPosition = localPosition;
		this.AverageVelocity.Record(localPosition);
		float magnitude = vector.magnitude;
		Quaternion b = this.NetworkRotation;
		if (this.UpdateGroundNormal && AI.groundAlign && MainCamera.Distance(base.transform.position) < AI.maxGroundAlignDist)
		{
			Vector3 vector2 = Vector3.up;
			RaycastHit raycastHit;
			if (this.GroundSample(base.transform.position, out raycastHit))
			{
				this.targetUp = raycastHit.normal;
				this.targetOffset = base.transform.InverseTransformPoint(raycastHit.point);
			}
			else
			{
				this.targetUp = Vector3.up;
				this.targetOffset = Vector3.zero;
			}
			if (this.alignmentRoot)
			{
				this.alignmentRoot.transform.localPosition = Vector3.MoveTowards(this.alignmentRoot.transform.localPosition, this.targetOffset, UnityEngine.Time.deltaTime * 2f);
			}
			vector2 = Vector3.MoveTowards(base.transform.up, this.targetUp, UnityEngine.Time.deltaTime * 180f);
			b = Quaternion.LookRotation(Vector3.Cross(this.NetworkRotation * Vector3.right, vector2), vector2);
		}
		if (this.UpdateWalkSpeed && this._animator != null && this._animator.gameObject.activeSelf)
		{
			this._animator.SetFloat(HTNAnimal.ParamWalkSpeed, Mathf.Min(magnitude / UnityEngine.Time.deltaTime, this.MaxWalkAnimSpeed));
			this._animator.SetFloat(HTNAnimal.ParamWalkSpeedAverage, Mathf.Min(this.AverageVelocity.Speed, this.MaxWalkAnimSpeed), 0.1f, UnityEngine.Time.smoothDeltaTime);
		}
		if (this.UpdateFacingDirection)
		{
			if (vector.magnitude > 0.5f)
			{
				b = Quaternion.LookRotation(vector.normalized);
			}
			float turnSpeed = this.AiDefinition.Movement.TurnSpeed;
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, UnityEngine.Time.smoothDeltaTime * turnSpeed);
			if (this.LaggyAss)
			{
				this.hipForward = Quaternion.Slerp(this.hipForward, b, UnityEngine.Time.smoothDeltaTime * 15f);
				if (Quaternion.Angle(this.hipForward, base.transform.rotation) > this.MaxLaggyAssRotation)
				{
					this.hipForward = Quaternion.RotateTowards(base.transform.rotation, this.hipForward, this.MaxLaggyAssRotation);
				}
			}
		}
		bool lookAtTarget = this.LookAtTarget;
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x0004F2B4 File Offset: 0x0004D4B4
	public void LateUpdateAnimation()
	{
		if (this.LaggyAss)
		{
			Quaternion rhs = Quaternion.Inverse(this.baseHipRotation) * this.HipBone.localRotation;
			this.HipBone.rotation = this.hipForward * Quaternion.Euler(this.HipFudge) * rhs;
		}
		if (this.LookAtTarget)
		{
			Quaternion rotation = Quaternion.Inverse(this.baseLookRotation) * this.LookBone.localRotation;
			this.LookBone.rotation = rotation;
		}
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00009303 File Offset: 0x00007503
	public bool GroundSample(Vector3 origin, out RaycastHit hit)
	{
		return Physics.Raycast(origin + Vector3.up * 0.5f, Vector3.down, ref hit, 1f, 8454144);
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00004F34 File Offset: 0x00003134
	public override float GetExtrapolationTime()
	{
		return Mathf.Clamp(Lerp.extrapolation, 0f, 0.1f);
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00046EA4 File Offset: 0x000450A4
	public override void OnSignal(BaseEntity.Signal signal, string arg)
	{
		base.OnSignal(signal, arg);
		switch (signal)
		{
		case BaseEntity.Signal.Attack:
			this.model.Trigger("attack");
			return;
		case BaseEntity.Signal.Alt_Attack:
			this.model.Trigger("attack_alt");
			return;
		case BaseEntity.Signal.DryFire:
		case BaseEntity.Signal.Reload:
		case BaseEntity.Signal.Deploy:
		case BaseEntity.Signal.Flinch_Head:
		case BaseEntity.Signal.Flinch_Chest:
		case BaseEntity.Signal.Flinch_Stomach:
		case BaseEntity.Signal.Flinch_RearHead:
		case BaseEntity.Signal.Flinch_RearTorso:
			break;
		case BaseEntity.Signal.Throw:
			this.model.Trigger("throw");
			return;
		default:
			if (signal != BaseEntity.Signal.Eat)
			{
				return;
			}
			this.model.Trigger("eat");
			break;
		}
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00009334 File Offset: 0x00007534
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		HTNAnimal.RegisterForVisibility(this);
		this.InitializeAnimation();
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00009349 File Offset: 0x00007549
	protected override void DoClientDestroy()
	{
		HTNAnimal.UnregisterFromVisibility(this);
		base.DoClientDestroy();
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x0004F33C File Offset: 0x0004D53C
	public static void ClientCycle(float deltaTime)
	{
		HTNAnimal.UpdateNpcVisibilities();
		for (int i = 0; i < HTNAnimal.visibleNpcList.Count; i++)
		{
			HTNAnimal.visibleNpcList[i].ClientUpdate();
		}
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x0004F374 File Offset: 0x0004D574
	public static void LateClientCycle()
	{
		for (int i = 0; i < HTNAnimal.visibleNpcList.Count; i++)
		{
			HTNAnimal.visibleNpcList[i].LateClientUpdate();
		}
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x000080B3 File Offset: 0x000062B3
	public override void MakeVisible()
	{
		base.MakeVisible();
		if (this.model)
		{
			this.model.ApplyVisibility(this.isVisible, this.isAnimatorVisible, this.isShadowVisible);
		}
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x00009357 File Offset: 0x00007557
	protected void ClientUpdate()
	{
		if (this.model)
		{
			this.model.ApplyVisibility(this.isVisible, this.isAnimatorVisible, this.isShadowVisible);
			this.UpdateAnimation();
		}
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00009389 File Offset: 0x00007589
	protected void LateClientUpdate()
	{
		if (this.model)
		{
			this.LateUpdateAnimation();
		}
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x0600090B RID: 2315 RVA: 0x0000939E File Offset: 0x0000759E
	public static List<HTNAnimal> VisibleNpcList
	{
		get
		{
			return HTNAnimal.visibleNpcList;
		}
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x0004F3A8 File Offset: 0x0004D5A8
	public static void ClearVisibility()
	{
		for (int i = 0; i < HTNAnimal.visibleNpcList.Count; i++)
		{
			HTNAnimal.visibleNpcList[i].UnregisterFromCulling();
		}
		HTNAnimal.visibleNpcList.Clear();
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x000093A5 File Offset: 0x000075A5
	public static void RegisterForVisibility(HTNAnimal npc)
	{
		HTNAnimal.visibleNpcList.Add(npc);
		npc.RegisterForCulling();
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x000093B8 File Offset: 0x000075B8
	public static void UnregisterFromVisibility(HTNAnimal npc)
	{
		npc.UnregisterFromCulling();
		HTNAnimal.visibleNpcList.Remove(npc);
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x000093CC File Offset: 0x000075CC
	public override void SetNetworkRotation(Quaternion rot)
	{
		this.NetworkRotation = rot;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x000093D5 File Offset: 0x000075D5
	public override Quaternion GetNetworkRotation()
	{
		if (base.isClient)
		{
			return this.NetworkRotation;
		}
		return Quaternion.identity;
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x000093EB File Offset: 0x000075EB
	[BaseEntity.RPC_Client]
	public virtual void PlayAnimationTrigger(string animationStr)
	{
		this.model.Trigger(animationStr);
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x000093F9 File Offset: 0x000075F9
	[BaseEntity.RPC_Client]
	public virtual void PlayAnimationBool(string animationStr, int value)
	{
		if (this.model.animator == null || !this.model.animator.isActiveAndEnabled)
		{
			return;
		}
		this.model.animator.SetBool(animationStr, value > 0);
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x00009436 File Offset: 0x00007636
	[BaseEntity.RPC_Client]
	public virtual void PlayAnimationInt(string animationStr, int value)
	{
		if (this.model.animator == null || !this.model.animator.isActiveAndEnabled)
		{
			return;
		}
		this.model.animator.SetInteger(animationStr, value);
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00009470 File Offset: 0x00007670
	public void FrontLeftFootstep()
	{
		this.Footstep(this.Feet[0]);
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x00009480 File Offset: 0x00007680
	public void FrontRightFootstep()
	{
		this.Footstep(this.Feet[1]);
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00009490 File Offset: 0x00007690
	public void BackLeftFootstep()
	{
		this.Footstep(this.Feet[2]);
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x000094A0 File Offset: 0x000076A0
	public void BackRightFootstep()
	{
		this.Footstep(this.Feet[3]);
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x000094B0 File Offset: 0x000076B0
	private void Footstep(Transform tx)
	{
		if (!this.FootstepEffects)
		{
			return;
		}
		this.FootstepEffects.SpawnOnRay(new Ray(tx.position, Vector3.down), 10551297, 0.5f, base.transform.forward);
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00002ECE File Offset: 0x000010CE
	public void DoEffect(string effect)
	{
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x0004F3E4 File Offset: 0x0004D5E4
	public void PlaySound(string soundName)
	{
		string text = StringFormatCache.Get("{0}/sound/{1}.asset", this.BaseFolder, soundName);
		SoundDefinition soundDefinition = FileSystem.Load<SoundDefinition>(text, true);
		if (soundDefinition != null)
		{
			SoundManager.PlayOneshot(soundDefinition, null, false, base.transform.position);
			return;
		}
		Debug.LogWarningFormat("Couldn't find sound {0}", new object[]
		{
			text
		});
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x0004F440 File Offset: 0x0004D640
	public static void UpdateNpcVisibilities()
	{
		if (Culling.toggle)
		{
			for (int i = 0; i < HTNAnimal.visibleNpcList.Count; i++)
			{
				HTNAnimal htnanimal = HTNAnimal.visibleNpcList[i];
				if (htnanimal.WantsVisUpdate())
				{
					htnanimal.VisUpdate();
				}
			}
		}
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x000094F0 File Offset: 0x000076F0
	private float TimeSinceSeen()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastTimeSeen;
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x000094FE File Offset: 0x000076FE
	private void SetNextVisThink(float addTime)
	{
		this.nextVisThink = UnityEngine.Time.realtimeSinceStartup + addTime;
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x0000950D File Offset: 0x0000770D
	private bool WantsVisUpdate()
	{
		return UnityEngine.Time.realtimeSinceStartup >= this.nextVisThink;
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x0004F484 File Offset: 0x0004D684
	protected override void UpdateCullingSpheres()
	{
		if (this.model != null)
		{
			Vector3 center;
			Matrix4x4 localToWorldMatrix;
			float num;
			if (this.model.collision != null)
			{
				center = this.model.collision.center;
				localToWorldMatrix = this.model.transform.localToWorldMatrix;
				num = this.model.collision.radius;
			}
			else
			{
				center = this.bounds.center;
				localToWorldMatrix = base.transform.localToWorldMatrix;
				num = Mathf.Max(Mathf.Max(this.bounds.extents.x, this.bounds.extents.y), this.bounds.extents.z);
			}
			Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(center);
			num += Vector3.Distance(vector, this.lastPosition);
			this.localOccludee.sphere = new OcclusionCulling.Sphere(vector, num);
			this.lastPosition = vector;
			return;
		}
		base.UpdateCullingSpheres();
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x000470E0 File Offset: 0x000452E0
	private void VisUpdateUsingCulling(float dist, bool visibility)
	{
		this.UpdateCullingBounds();
		float entityMinCullDist = Culling.entityMinCullDist;
		float entityMinAnimatorCullDist = Culling.entityMinAnimatorCullDist;
		float entityMinShadowCullDist = Culling.entityMinShadowCullDist;
		float entityMaxDist = Culling.entityMaxDist;
		bool isVisible = dist <= entityMaxDist && (dist <= entityMinCullDist || visibility);
		this.isVisible = isVisible;
		this.isAnimatorVisible = (this.isVisible || dist <= entityMinAnimatorCullDist);
		this.isShadowVisible = (this.isVisible || dist <= entityMinShadowCullDist);
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x0004F578 File Offset: 0x0004D778
	protected override void OnVisibilityChanged(bool visible)
	{
		if (LocalPlayer.Entity != null && MainCamera.mainCamera != null)
		{
			float dist = Vector3.Distance(MainCamera.mainCamera.transform.position, base.transform.position);
			this.VisUpdateUsingCulling(dist, visible);
		}
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x0004F5C8 File Offset: 0x0004D7C8
	private void VisUpdate()
	{
		if (LocalPlayer.Entity == null || MainCamera.mainCamera == null)
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
				OcclusionCulling.Sphere sphere = this.localOccludee.sphere;
				Color color = this.IsDead() ? Color.black : (this.isVisible ? Color.green : Color.red);
				UnityEngine.DDraw.SphereGizmo(sphere.position, sphere.radius, color, fDuration, false, false);
			}
		}
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x06000923 RID: 2339 RVA: 0x0000951F File Offset: 0x0000771F
	public BaseNpcDefinition AiDefinition
	{
		get
		{
			return this._aiDefinition;
		}
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x06000924 RID: 2340 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x00009527 File Offset: 0x00007727
	public override float StartHealth()
	{
		return this.AiDefinition.Vitals.HP;
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x00009527 File Offset: 0x00007727
	public override float StartMaxHealth()
	{
		return this.AiDefinition.Vitals.HP;
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00009527 File Offset: 0x00007727
	public override float MaxHealth()
	{
		return this.AiDefinition.Vitals.HP;
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x00009539 File Offset: 0x00007739
	public override float MaxVelocity()
	{
		return this.AiDefinition.Movement.RunSpeed;
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x06000929 RID: 2345 RVA: 0x00004B3B File Offset: 0x00002D3B
	public BaseEntity Body
	{
		get
		{
			return this;
		}
	}

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x0600092A RID: 2346 RVA: 0x000079E3 File Offset: 0x00005BE3
	public Vector3 BodyPosition
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x0600092B RID: 2347 RVA: 0x0000954B File Offset: 0x0000774B
	public Vector3 EyePosition
	{
		get
		{
			return base.CenterPoint();
		}
	}

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x0600092C RID: 2348 RVA: 0x00009553 File Offset: 0x00007753
	public Quaternion EyeRotation
	{
		get
		{
			return base.transform.rotation;
		}
	}
}
