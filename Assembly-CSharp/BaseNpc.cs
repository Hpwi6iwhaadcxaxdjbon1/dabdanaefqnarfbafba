using System;
using System.Collections.Generic;
using Apex.AI.Components;
using ConVar;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000071 RID: 113
public class BaseNpc : BaseCombatEntity
{
	// Token: 0x04000402 RID: 1026
	public int agentTypeIndex;

	// Token: 0x04000403 RID: 1027
	public bool NewAI;

	// Token: 0x04000404 RID: 1028
	[NonSerialized]
	public Quaternion NetworkRotation;

	// Token: 0x04000405 RID: 1029
	private static List<BaseNpc> visibleNpcList = new List<BaseNpc>();

	// Token: 0x04000406 RID: 1030
	[Header("BaseNpc")]
	public GameObjectRef CorpsePrefab;

	// Token: 0x04000407 RID: 1031
	public BaseNpc.AiStatistics Stats;

	// Token: 0x04000408 RID: 1032
	public Vector3 AttackOffset;

	// Token: 0x04000409 RID: 1033
	public float AttackDamage = 20f;

	// Token: 0x0400040A RID: 1034
	public DamageType AttackDamageType = DamageType.Bite;

	// Token: 0x0400040B RID: 1035
	[Tooltip("Stamina to use per attack")]
	public float AttackCost = 0.1f;

	// Token: 0x0400040C RID: 1036
	[Tooltip("How often can we attack")]
	public float AttackRate = 1f;

	// Token: 0x0400040D RID: 1037
	[Tooltip("Maximum Distance for an attack")]
	public float AttackRange = 1f;

	// Token: 0x0400040E RID: 1038
	public NavMeshAgent NavAgent;

	// Token: 0x0400040F RID: 1039
	[SerializeField]
	private UtilityAIComponent utilityAiComponent;

	// Token: 0x04000410 RID: 1040
	public LayerMask movementMask = 429990145;

	// Token: 0x04000411 RID: 1041
	[InspectorFlags]
	public BaseNpc.AiFlags aiFlags;

	// Token: 0x04000412 RID: 1042
	[Header("NPC Senses")]
	public int ForgetUnseenEntityTime = 10;

	// Token: 0x04000413 RID: 1043
	public float SensesTickRate = 0.5f;

	// Token: 0x04000414 RID: 1044
	private float nextVisThink;

	// Token: 0x04000415 RID: 1045
	private float lastTimeSeen;

	// Token: 0x04000416 RID: 1046
	private Vector3 lastPosition;

	// Token: 0x0600073E RID: 1854 RVA: 0x00046BC8 File Offset: 0x00044DC8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseNpc.OnRpcMessage", 0.1f))
		{
			if (rpc == 2140700412U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: Attack ");
				}
				using (TimeWarning.New("Attack", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							Vector3 position = msg.read.Vector3();
							this.Attack(position);
						}
					}
					catch (Exception exception)
					{
						Net.cl.Disconnect("RPC Error in Attack", true);
						Debug.LogException(exception);
					}
				}
				return true;
			}
			if (rpc == 2800588698U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: Eat ");
				}
				using (TimeWarning.New("Eat", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							Vector3 position2 = msg.read.Vector3();
							this.Eat(position2);
						}
					}
					catch (Exception exception2)
					{
						Net.cl.Disconnect("RPC Error in Eat", true);
						Debug.LogException(exception2);
					}
				}
				return true;
			}
			if (rpc == 3965393509U && player == null)
			{
				if (Global.developer > 2)
				{
					Debug.Log("CL_RPCMessage: Startled ");
				}
				using (TimeWarning.New("Startled", 0.1f))
				{
					try
					{
						using (TimeWarning.New("Call", 0.1f))
						{
							Vector3 position3 = msg.read.Vector3();
							this.Startled(position3);
						}
					}
					catch (Exception exception3)
					{
						Net.cl.Disconnect("RPC Error in Startled", true);
						Debug.LogException(exception3);
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x00004F34 File Offset: 0x00003134
	public override float GetExtrapolationTime()
	{
		return Mathf.Clamp(Lerp.extrapolation, 0f, 0.1f);
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x00046EA4 File Offset: 0x000450A4
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

	// Token: 0x06000741 RID: 1857 RVA: 0x00008096 File Offset: 0x00006296
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		BaseNpc.RegisterForVisibility(this);
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x000080A5 File Offset: 0x000062A5
	protected override void DoClientDestroy()
	{
		BaseNpc.UnregisterFromVisibility(this);
		base.DoClientDestroy();
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x00046F34 File Offset: 0x00045134
	public static void ClientCycle(float deltaTime)
	{
		BaseNpc.UpdateNpcVisibilities();
		for (int i = 0; i < BaseNpc.visibleNpcList.Count; i++)
		{
			BaseNpc.visibleNpcList[i].ClientUpdate();
		}
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x000080B3 File Offset: 0x000062B3
	public override void MakeVisible()
	{
		base.MakeVisible();
		if (this.model)
		{
			this.model.ApplyVisibility(this.isVisible, this.isAnimatorVisible, this.isShadowVisible);
		}
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x000080E5 File Offset: 0x000062E5
	protected void ClientUpdate()
	{
		if (this.model)
		{
			this.model.ApplyVisibility(this.isVisible, this.isAnimatorVisible, this.isShadowVisible);
		}
	}

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x06000746 RID: 1862 RVA: 0x00008111 File Offset: 0x00006311
	public static List<BaseNpc> VisibleNpcList
	{
		get
		{
			return BaseNpc.visibleNpcList;
		}
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x00046F6C File Offset: 0x0004516C
	public static void ClearVisibility()
	{
		for (int i = 0; i < BaseNpc.visibleNpcList.Count; i++)
		{
			BaseNpc.visibleNpcList[i].UnregisterFromCulling();
		}
		BaseNpc.visibleNpcList.Clear();
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x00008118 File Offset: 0x00006318
	public static void RegisterForVisibility(BaseNpc npc)
	{
		BaseNpc.visibleNpcList.Add(npc);
		npc.RegisterForCulling();
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x0000812B File Offset: 0x0000632B
	public static void UnregisterFromVisibility(BaseNpc npc)
	{
		npc.UnregisterFromCulling();
		BaseNpc.visibleNpcList.Remove(npc);
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x0000813F File Offset: 0x0000633F
	public override void SetNetworkRotation(Quaternion rot)
	{
		this.NetworkRotation = rot;
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x00008148 File Offset: 0x00006348
	public override Quaternion GetNetworkRotation()
	{
		if (base.isClient)
		{
			return this.NetworkRotation;
		}
		return Quaternion.identity;
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x0000815E File Offset: 0x0000635E
	[BaseEntity.RPC_Client]
	public virtual void Eat(Vector3 position)
	{
		if (this.model.animator)
		{
			this.model.animator.SetTrigger("eat");
		}
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00008187 File Offset: 0x00006387
	[BaseEntity.RPC_Client]
	public virtual void Attack(Vector3 position)
	{
		if (this.model.animator)
		{
			this.model.animator.SetTrigger("attack");
		}
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x000081B0 File Offset: 0x000063B0
	[BaseEntity.RPC_Client]
	public virtual void Startled(Vector3 position)
	{
		if (this.model.animator)
		{
			this.model.animator.SetTrigger("startled");
		}
	}

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x0600074F RID: 1871 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x000081D9 File Offset: 0x000063D9
	public override float MaxVelocity()
	{
		return this.Stats.Speed;
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x000081E6 File Offset: 0x000063E6
	public bool HasAiFlag(BaseNpc.AiFlags f)
	{
		return (this.aiFlags & f) == f;
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x000081F3 File Offset: 0x000063F3
	public void SetAiFlag(BaseNpc.AiFlags f, bool set)
	{
		if (set)
		{
			this.aiFlags |= f;
			return;
		}
		this.aiFlags &= ~f;
	}

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x06000753 RID: 1875 RVA: 0x00008216 File Offset: 0x00006416
	// (set) Token: 0x06000754 RID: 1876 RVA: 0x0000821F File Offset: 0x0000641F
	public bool IsSitting
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Sitting);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Sitting, value);
		}
	}

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x06000755 RID: 1877 RVA: 0x00008229 File Offset: 0x00006429
	// (set) Token: 0x06000756 RID: 1878 RVA: 0x00008232 File Offset: 0x00006432
	public bool IsChasing
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Chasing);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Chasing, value);
		}
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x06000757 RID: 1879 RVA: 0x0000823C File Offset: 0x0000643C
	// (set) Token: 0x06000758 RID: 1880 RVA: 0x00008245 File Offset: 0x00006445
	public bool IsSleeping
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Sleeping);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Sleeping, value);
		}
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00046FA8 File Offset: 0x000451A8
	public static void UpdateNpcVisibilities()
	{
		if (Culling.toggle)
		{
			for (int i = 0; i < BaseNpc.visibleNpcList.Count; i++)
			{
				BaseNpc baseNpc = BaseNpc.visibleNpcList[i];
				if (baseNpc.WantsVisUpdate())
				{
					baseNpc.VisUpdate();
				}
			}
		}
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x0000824F File Offset: 0x0000644F
	private float TimeSinceSeen()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastTimeSeen;
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x0000825D File Offset: 0x0000645D
	private void SetNextVisThink(float addTime)
	{
		this.nextVisThink = UnityEngine.Time.realtimeSinceStartup + addTime;
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x0000826C File Offset: 0x0000646C
	private bool WantsVisUpdate()
	{
		return UnityEngine.Time.realtimeSinceStartup >= this.nextVisThink;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x00046FEC File Offset: 0x000451EC
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

	// Token: 0x0600075E RID: 1886 RVA: 0x000470E0 File Offset: 0x000452E0
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

	// Token: 0x0600075F RID: 1887 RVA: 0x00047158 File Offset: 0x00045358
	protected override void OnVisibilityChanged(bool visible)
	{
		if (LocalPlayer.Entity != null && MainCamera.mainCamera != null)
		{
			bool isVisible = this.isVisible;
			bool isAnimatorVisible = this.isAnimatorVisible;
			bool isShadowVisible = this.isShadowVisible;
			float dist = Vector3.Distance(MainCamera.mainCamera.transform.position, base.transform.position);
			this.VisUpdateUsingCulling(dist, visible);
			if (this.model != null && (this.isVisible != isVisible || this.isAnimatorVisible != isAnimatorVisible || this.isShadowVisible != isShadowVisible))
			{
				this.model.ApplyVisibility(this.isVisible, this.isAnimatorVisible, this.isShadowVisible);
			}
		}
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x00047208 File Offset: 0x00045408
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

	// Token: 0x02000072 RID: 114
	[Flags]
	public enum AiFlags
	{
		// Token: 0x04000418 RID: 1048
		Sitting = 2,
		// Token: 0x04000419 RID: 1049
		Chasing = 4,
		// Token: 0x0400041A RID: 1050
		Sleeping = 8
	}

	// Token: 0x02000073 RID: 115
	[Serializable]
	public struct AiStatistics
	{
		// Token: 0x0400041B RID: 1051
		[Range(0f, 1f)]
		[Tooltip("Ai will be less likely to fight animals that are larger than them, and more likely to flee from them.")]
		public float Size;

		// Token: 0x0400041C RID: 1052
		[Tooltip("How fast we can move")]
		public float Speed;

		// Token: 0x0400041D RID: 1053
		[Tooltip("How fast can we accelerate")]
		public float Acceleration;

		// Token: 0x0400041E RID: 1054
		[Tooltip("How fast can we turn around")]
		public float TurnSpeed;

		// Token: 0x0400041F RID: 1055
		[Tooltip("Determines things like how near we'll allow other species to get")]
		[Range(0f, 1f)]
		public float Tolerance;

		// Token: 0x04000420 RID: 1056
		[Tooltip("How far this NPC can see")]
		public float VisionRange;

		// Token: 0x04000421 RID: 1057
		[Tooltip("Our vision cone for dot product - a value of -1 means we can see all around us, 0 = only infront ")]
		public float VisionCone;

		// Token: 0x04000422 RID: 1058
		[Tooltip("NPCs use distance visibility to basically make closer enemies easier to detect than enemies further away")]
		public AnimationCurve DistanceVisibility;

		// Token: 0x04000423 RID: 1059
		[Tooltip("How likely are we to be offensive without being threatened")]
		public float Hostility;

		// Token: 0x04000424 RID: 1060
		[Tooltip("How likely are we to defend ourselves when attacked")]
		public float Defensiveness;

		// Token: 0x04000425 RID: 1061
		[Tooltip("The range at which we will engage targets")]
		public float AggressionRange;

		// Token: 0x04000426 RID: 1062
		[Tooltip("The range at which an aggrified npc will disengage it's current target")]
		public float DeaggroRange;

		// Token: 0x04000427 RID: 1063
		[Tooltip("For how long will we chase a target until we give up")]
		public float DeaggroChaseTime;

		// Token: 0x04000428 RID: 1064
		[Tooltip("When we deaggro, how long do we wait until we can aggro again.")]
		public float DeaggroCooldown;

		// Token: 0x04000429 RID: 1065
		[Tooltip("The threshold of our health fraction where there's a chance that we want to flee")]
		public float HealthThresholdForFleeing;

		// Token: 0x0400042A RID: 1066
		[Tooltip("The chance that we will flee when our health threshold is triggered")]
		public float HealthThresholdFleeChance;

		// Token: 0x0400042B RID: 1067
		[Tooltip("When we flee, what is the minimum distance we should flee?")]
		public float MinFleeRange;

		// Token: 0x0400042C RID: 1068
		[Tooltip("When we flee, what is the maximum distance we should flee?")]
		public float MaxFleeRange;

		// Token: 0x0400042D RID: 1069
		[Tooltip("When we flee, what is the maximum time that can pass until we stop?")]
		public float MaxFleeTime;

		// Token: 0x0400042E RID: 1070
		[Tooltip("At what range we are afraid of a target that is in our Is Afraid Of list.")]
		public float AfraidRange;

		// Token: 0x0400042F RID: 1071
		[Tooltip("The family this npc belong to. Npcs in the same family will not attack each other.")]
		public BaseNpc.AiStatistics.FamilyEnum Family;

		// Token: 0x04000430 RID: 1072
		[Tooltip("List of the types of Npc that we are afraid of.")]
		public BaseNpc.AiStatistics.FamilyEnum[] IsAfraidOf;

		// Token: 0x04000431 RID: 1073
		[Tooltip("The minimum distance this npc will wander when idle.")]
		public float MinRoamRange;

		// Token: 0x04000432 RID: 1074
		[Tooltip("The maximum distance this npc will wander when idle.")]
		public float MaxRoamRange;

		// Token: 0x04000433 RID: 1075
		[Tooltip("The minimum amount of time between each time we seek a new roam destination (when idle)")]
		public float MinRoamDelay;

		// Token: 0x04000434 RID: 1076
		[Tooltip("The maximum amount of time between each time we seek a new roam destination (when idle)")]
		public float MaxRoamDelay;

		// Token: 0x04000435 RID: 1077
		[Tooltip("If an npc is mobile, they are allowed to move when idle.")]
		public bool IsMobile;

		// Token: 0x04000436 RID: 1078
		[Tooltip("In the range between min and max roam delay, we evaluate the random value through this curve")]
		public AnimationCurve RoamDelayDistribution;

		// Token: 0x04000437 RID: 1079
		[Tooltip("For how long do we remember that someone attacked us")]
		public float AttackedMemoryTime;

		// Token: 0x04000438 RID: 1080
		[Tooltip("How long should we block movement to make the wakeup animation not look whack?")]
		public float WakeupBlockMoveTime;

		// Token: 0x04000439 RID: 1081
		[Tooltip("The maximum water depth this npc willingly will walk into.")]
		public float MaxWaterDepth;

		// Token: 0x0400043A RID: 1082
		[Tooltip("The water depth at which they will start swimming.")]
		public float WaterLevelNeck;

		// Token: 0x0400043B RID: 1083
		[Tooltip("The range we consider using close range weapons.")]
		public float CloseRange;

		// Token: 0x0400043C RID: 1084
		[Tooltip("The range we consider using medium range weapons.")]
		public float MediumRange;

		// Token: 0x0400043D RID: 1085
		[Tooltip("The range we consider using long range weapons.")]
		public float LongRange;

		// Token: 0x0400043E RID: 1086
		[Tooltip("How long can we be out of range of our spawn point before we time out and make our way back home (when idle).")]
		public float OutOfRangeOfSpawnPointTimeout;

		// Token: 0x0400043F RID: 1087
		[Tooltip("What is the maximum distance we are allowed to have to our spawn location before we are being encourraged to go back home.")]
		public NPCPlayerApex.EnemyRangeEnum MaxRangeToSpawnLoc;

		// Token: 0x04000440 RID: 1088
		[Tooltip("If this is set to true, then a target must hold special markers (like IsHostile) for the target to be considered for aggressive action.")]
		public bool OnlyAggroMarkedTargets;

		// Token: 0x02000074 RID: 116
		public enum FamilyEnum
		{
			// Token: 0x04000442 RID: 1090
			Bear,
			// Token: 0x04000443 RID: 1091
			Wolf,
			// Token: 0x04000444 RID: 1092
			Deer,
			// Token: 0x04000445 RID: 1093
			Boar,
			// Token: 0x04000446 RID: 1094
			Chicken,
			// Token: 0x04000447 RID: 1095
			Horse,
			// Token: 0x04000448 RID: 1096
			Zombie,
			// Token: 0x04000449 RID: 1097
			Scientist,
			// Token: 0x0400044A RID: 1098
			Murderer,
			// Token: 0x0400044B RID: 1099
			Player
		}
	}
}
