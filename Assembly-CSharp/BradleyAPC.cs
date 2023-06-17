using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class BradleyAPC : BaseCombatEntity
{
	// Token: 0x040004B7 RID: 1207
	[Header("Sound")]
	public EngineAudioClip engineAudioClip;

	// Token: 0x040004B8 RID: 1208
	public SlicedGranularAudioClip treadAudioClip;

	// Token: 0x040004B9 RID: 1209
	public float treadGrainFreqMin = 0.025f;

	// Token: 0x040004BA RID: 1210
	public float treadGrainFreqMax = 0.5f;

	// Token: 0x040004BB RID: 1211
	public AnimationCurve treadFreqCurve;

	// Token: 0x040004BC RID: 1212
	public SoundDefinition chasisLurchSoundDef;

	// Token: 0x040004BD RID: 1213
	public float chasisLurchAngleDelta = 2f;

	// Token: 0x040004BE RID: 1214
	public float chasisLurchSpeedDelta = 2f;

	// Token: 0x040004BF RID: 1215
	private float lastAngle;

	// Token: 0x040004C0 RID: 1216
	private float lastSpeed;

	// Token: 0x040004C1 RID: 1217
	public SoundDefinition turretTurnLoopDef;

	// Token: 0x040004C2 RID: 1218
	public float turretLoopGainSpeed = 3f;

	// Token: 0x040004C3 RID: 1219
	public float turretLoopPitchSpeed = 3f;

	// Token: 0x040004C4 RID: 1220
	public float turretLoopMinAngleDelta;

	// Token: 0x040004C5 RID: 1221
	public float turretLoopMaxAngleDelta = 10f;

	// Token: 0x040004C6 RID: 1222
	public float turretLoopPitchMin = 0.5f;

	// Token: 0x040004C7 RID: 1223
	public float turretLoopPitchMax = 1f;

	// Token: 0x040004C8 RID: 1224
	public float turretLoopGainThreshold = 0.0001f;

	// Token: 0x040004C9 RID: 1225
	private Sound turretTurnLoop;

	// Token: 0x040004CA RID: 1226
	private SoundModulation.Modulator turretTurnLoopGain;

	// Token: 0x040004CB RID: 1227
	private SoundModulation.Modulator turretTurnLoopPitch;

	// Token: 0x040004CC RID: 1228
	public float enginePitch = 0.9f;

	// Token: 0x040004CD RID: 1229
	public float rpmMultiplier = 0.6f;

	// Token: 0x040004CE RID: 1230
	private TreadAnimator treadAnimator;

	// Token: 0x040004CF RID: 1231
	private float lastTurretAngle;

	// Token: 0x040004D0 RID: 1232
	[Header("Wheels")]
	public WheelCollider[] leftWheels;

	// Token: 0x040004D1 RID: 1233
	public WheelCollider[] rightWheels;

	// Token: 0x040004D2 RID: 1234
	[Header("Movement Config")]
	public float moveForceMax = 2000f;

	// Token: 0x040004D3 RID: 1235
	public float brakeForce = 100f;

	// Token: 0x040004D4 RID: 1236
	public float turnForce = 2000f;

	// Token: 0x040004D5 RID: 1237
	public float sideStiffnessMax = 1f;

	// Token: 0x040004D6 RID: 1238
	public float sideStiffnessMin = 0.5f;

	// Token: 0x040004D7 RID: 1239
	public Transform centerOfMass;

	// Token: 0x040004D8 RID: 1240
	public float stoppingDist = 5f;

	// Token: 0x040004D9 RID: 1241
	[Header("Control")]
	public float throttle = 1f;

	// Token: 0x040004DA RID: 1242
	public float turning;

	// Token: 0x040004DB RID: 1243
	public float rightThrottle;

	// Token: 0x040004DC RID: 1244
	public float leftThrottle;

	// Token: 0x040004DD RID: 1245
	public bool brake;

	// Token: 0x040004DE RID: 1246
	[Header("Other")]
	public Rigidbody myRigidBody;

	// Token: 0x040004DF RID: 1247
	public Collider myCollider;

	// Token: 0x040004E0 RID: 1248
	public Vector3 destination;

	// Token: 0x040004E1 RID: 1249
	private Vector3 finalDestination;

	// Token: 0x040004E2 RID: 1250
	public Transform followTest;

	// Token: 0x040004E3 RID: 1251
	public TriggerHurtEx impactDamager;

	// Token: 0x040004E4 RID: 1252
	[Header("Weapons")]
	public Transform mainTurretEyePos;

	// Token: 0x040004E5 RID: 1253
	public Transform mainTurret;

	// Token: 0x040004E6 RID: 1254
	public Transform CannonPitch;

	// Token: 0x040004E7 RID: 1255
	public Transform CannonMuzzle;

	// Token: 0x040004E8 RID: 1256
	public Transform coaxPitch;

	// Token: 0x040004E9 RID: 1257
	public Transform coaxMuzzle;

	// Token: 0x040004EA RID: 1258
	public Transform topTurretEyePos;

	// Token: 0x040004EB RID: 1259
	public Transform topTurretYaw;

	// Token: 0x040004EC RID: 1260
	public Transform topTurretPitch;

	// Token: 0x040004ED RID: 1261
	public Transform topTurretMuzzle;

	// Token: 0x040004EE RID: 1262
	private Vector3 turretAimVector = Vector3.forward;

	// Token: 0x040004EF RID: 1263
	private Vector3 desiredAimVector = Vector3.forward;

	// Token: 0x040004F0 RID: 1264
	private Vector3 topTurretAimVector = Vector3.forward;

	// Token: 0x040004F1 RID: 1265
	private Vector3 desiredTopTurretAimVector = Vector3.forward;

	// Token: 0x040004F2 RID: 1266
	[Header("Effects")]
	public GameObjectRef explosionEffect;

	// Token: 0x040004F3 RID: 1267
	public GameObjectRef servergibs;

	// Token: 0x040004F4 RID: 1268
	public GameObjectRef fireBall;

	// Token: 0x040004F5 RID: 1269
	public GameObjectRef crateToDrop;

	// Token: 0x040004F6 RID: 1270
	public GameObjectRef debrisFieldMarker;

	// Token: 0x040004F7 RID: 1271
	[Header("Loot")]
	public int maxCratesToSpawn;

	// Token: 0x040004F8 RID: 1272
	[Header("Pathing")]
	public List<Vector3> currentPath;

	// Token: 0x040004F9 RID: 1273
	public int currentPathIndex;

	// Token: 0x040004FA RID: 1274
	public bool pathLooping;

	// Token: 0x040004FB RID: 1275
	[Header("Targeting")]
	public float viewDistance = 100f;

	// Token: 0x040004FC RID: 1276
	public float searchRange = 100f;

	// Token: 0x040004FD RID: 1277
	public float searchFrequency = 2f;

	// Token: 0x040004FE RID: 1278
	public float memoryDuration = 20f;

	// Token: 0x040004FF RID: 1279
	public static float sightUpdateRate = 0.5f;

	// Token: 0x04000500 RID: 1280
	private BaseCombatEntity mainGunTarget;

	// Token: 0x04000501 RID: 1281
	public List<BradleyAPC.TargetInfo> targetList = new List<BradleyAPC.TargetInfo>();

	// Token: 0x04000502 RID: 1282
	public GameObjectRef gun_fire_effect;

	// Token: 0x04000503 RID: 1283
	public GameObjectRef bulletEffect;

	// Token: 0x04000504 RID: 1284
	private float lastLateUpdate;

	// Token: 0x060007DF RID: 2015 RVA: 0x00049450 File Offset: 0x00047650
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BradleyAPC.OnRpcMessage", 0.1f))
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
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0004956C File Offset: 0x0004776C
	public void InitializeClientsideEffects()
	{
		this.treadAnimator = base.GetComponent<TreadAnimator>();
		this.engineAudioClip = base.GetComponentInChildren<EngineAudioClip>();
		this.treadAudioClip = base.GetComponentInChildren<SlicedGranularAudioClip>();
		if (this.turretTurnLoop == null)
		{
			this.turretTurnLoop = SoundManager.RequestSoundInstance(this.turretTurnLoopDef, this.mainTurret.gameObject, default(Vector3), false);
			this.turretTurnLoopGain = this.turretTurnLoop.modulation.CreateModulator(SoundModulation.Parameter.Gain);
			this.turretTurnLoopPitch = this.turretTurnLoop.modulation.CreateModulator(SoundModulation.Parameter.Pitch);
		}
		base.InvokeRepeating(new Action(this.UpdateSounds), 0f, 0.0333f);
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x000086C2 File Offset: 0x000068C2
	public void ShutdownClientsideEffects()
	{
		if (this.turretTurnLoop != null)
		{
			this.turretTurnLoop.FadeOutAndRecycle(0.1f);
			this.turretTurnLoopGain = null;
			this.turretTurnLoopPitch = null;
		}
		base.CancelInvoke(new Action(this.UpdateSounds));
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0004961C File Offset: 0x0004781C
	public void UpdateSounds()
	{
		if (this.treadAnimator == null)
		{
			return;
		}
		Vector3 currentVelocity = this.treadAnimator.GetCurrentVelocity();
		float b = Mathf.InverseLerp(0f, 5f, currentVelocity.magnitude);
		float num = Mathf.Max(Mathf.InverseLerp(0f, 60f, this.treadAnimator.GetAngularSpeed()), b);
		this.engineAudioClip.RPMControl = (this.throttle * 0.7f + num * 0.3f) * this.rpmMultiplier;
		this.engineAudioClip.source.pitch = this.enginePitch;
		if (num <= 0.05f)
		{
			this.treadAudioClip.grainFrequency = 0f;
			this.engineAudioClip.RPMControl = 0f;
		}
		else
		{
			this.treadAudioClip.grainFrequency = Mathf.Lerp(this.treadGrainFreqMax, this.treadGrainFreqMin, this.treadFreqCurve.Evaluate(num));
		}
		float y = this.mainTurret.localRotation.eulerAngles.y;
		float value = Mathf.Abs(this.lastTurretAngle - y) * UnityEngine.Time.deltaTime;
		float num2 = Mathf.Clamp01(Mathf.InverseLerp(this.turretLoopMinAngleDelta, this.turretLoopMaxAngleDelta, value));
		float target = (num2 > this.turretLoopGainThreshold) ? 1f : 0f;
		float target2 = (num2 > this.turretLoopGainThreshold) ? this.turretLoopPitchMax : this.turretLoopPitchMin;
		this.turretTurnLoopGain.value = Mathf.MoveTowards(this.turretTurnLoopGain.value, target, this.turretLoopGainSpeed * UnityEngine.Time.deltaTime);
		this.turretTurnLoopPitch.value = Mathf.MoveTowards(this.turretTurnLoopPitch.value, target2, this.turretLoopPitchSpeed * UnityEngine.Time.deltaTime);
		if (this.turretTurnLoopGain.value > 0f && !this.turretTurnLoop.isAudioSourcePlaying)
		{
			this.turretTurnLoop.Play();
		}
		else if (this.turretTurnLoopGain.value == 0f && this.turretTurnLoop.isAudioSourcePlaying)
		{
			this.turretTurnLoop.Stop();
		}
		float magnitude = currentVelocity.magnitude;
		float num3 = Vector3.Angle(base.transform.up, Vector3.up);
		float num4 = Mathf.Abs(this.lastAngle - num3) * UnityEngine.Time.deltaTime;
		float num5 = Mathf.Abs(this.lastSpeed - magnitude) * UnityEngine.Time.deltaTime;
		if (num4 > this.chasisLurchAngleDelta || num5 > this.chasisLurchSpeedDelta)
		{
			SoundManager.PlayOneshot(this.chasisLurchSoundDef, base.gameObject, false, default(Vector3));
		}
		this.lastSpeed = magnitude;
		this.lastAngle = num3;
		this.lastTurretAngle = y;
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x000498B8 File Offset: 0x00047AB8
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.bradley != null && !info.fromDisk)
		{
			this.throttle = info.msg.bradley.engineThrottle;
			this.rightThrottle = info.msg.bradley.throttleRight;
			this.leftThrottle = info.msg.bradley.throttleLeft;
			this.desiredAimVector = info.msg.bradley.mainGunVec;
			this.desiredTopTurretAimVector = info.msg.bradley.topTurretVec;
		}
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x00008702 File Offset: 0x00006902
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.InitializeClientsideEffects();
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x00008711 File Offset: 0x00006911
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		this.ShutdownClientsideEffects();
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x00049950 File Offset: 0x00047B50
	public void AddOrUpdateTarget(BaseEntity ent, Vector3 pos, float damageFrom = 0f)
	{
		if (!(ent is BasePlayer))
		{
			return;
		}
		BradleyAPC.TargetInfo targetInfo = null;
		foreach (BradleyAPC.TargetInfo targetInfo2 in this.targetList)
		{
			if (targetInfo2.entity == ent)
			{
				targetInfo = targetInfo2;
				break;
			}
		}
		if (targetInfo == null)
		{
			targetInfo = Pool.Get<BradleyAPC.TargetInfo>();
			targetInfo.Setup(ent, UnityEngine.Time.time - 1f);
			this.targetList.Add(targetInfo);
		}
		targetInfo.lastSeenPosition = pos;
		targetInfo.damageReceivedFrom += damageFrom;
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x000499F8 File Offset: 0x00047BF8
	public void UpdateTargetList()
	{
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		global::Vis.Entities<BaseEntity>(base.transform.position, this.searchRange, list, 133120, 2);
		foreach (BaseEntity baseEntity in list)
		{
			if (baseEntity is BasePlayer)
			{
				BasePlayer basePlayer = baseEntity as BasePlayer;
				if (!basePlayer.IsDead() && !(basePlayer is Scientist) && this.VisibilityTest(baseEntity))
				{
					bool flag = false;
					foreach (BradleyAPC.TargetInfo targetInfo in this.targetList)
					{
						if (targetInfo.entity == baseEntity)
						{
							targetInfo.lastSeenTime = UnityEngine.Time.time;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						BradleyAPC.TargetInfo targetInfo2 = Pool.Get<BradleyAPC.TargetInfo>();
						targetInfo2.Setup(baseEntity, UnityEngine.Time.time);
						this.targetList.Add(targetInfo2);
					}
				}
			}
		}
		for (int i = this.targetList.Count - 1; i >= 0; i--)
		{
			BradleyAPC.TargetInfo targetInfo3 = this.targetList[i];
			BasePlayer basePlayer2 = targetInfo3.entity as BasePlayer;
			if (targetInfo3.entity == null || UnityEngine.Time.time - targetInfo3.lastSeenTime > this.memoryDuration || basePlayer2.IsDead())
			{
				this.targetList.Remove(targetInfo3);
				Pool.Free<BradleyAPC.TargetInfo>(ref targetInfo3);
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		this.targetList.Sort(new Comparison<BradleyAPC.TargetInfo>(this.SortTargets));
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x00049BB8 File Offset: 0x00047DB8
	public int SortTargets(BradleyAPC.TargetInfo t1, BradleyAPC.TargetInfo t2)
	{
		return t2.GetPriorityScore(this).CompareTo(t1.GetPriorityScore(this));
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x00049BDC File Offset: 0x00047DDC
	public Vector3 GetAimPoint(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (basePlayer != null)
		{
			return basePlayer.eyes.position;
		}
		return ent.CenterPoint();
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x00049C0C File Offset: 0x00047E0C
	public bool VisibilityTest(BaseEntity ent)
	{
		if (ent == null)
		{
			return false;
		}
		if (Vector3.Distance(ent.transform.position, base.transform.position) >= this.viewDistance)
		{
			return false;
		}
		bool result;
		if (ent is BasePlayer)
		{
			BasePlayer basePlayer = ent as BasePlayer;
			Vector3 position = this.mainTurret.transform.position;
			result = (base.IsVisible(basePlayer.eyes.position, position, float.PositiveInfinity) || base.IsVisible(basePlayer.transform.position, position, float.PositiveInfinity));
		}
		else
		{
			Debug.LogWarning("Standard vis test!");
			result = base.IsVisible(ent.CenterPoint(), float.PositiveInfinity);
		}
		return result;
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x00049CC0 File Offset: 0x00047EC0
	public void UpdateTargetVisibilities()
	{
		foreach (BradleyAPC.TargetInfo targetInfo in this.targetList)
		{
			if (targetInfo.IsValid() && this.VisibilityTest(targetInfo.entity))
			{
				targetInfo.lastSeenTime = UnityEngine.Time.time;
				targetInfo.lastSeenPosition = targetInfo.entity.transform.position;
			}
		}
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x00049D44 File Offset: 0x00047F44
	[BaseEntity.RPC_Client]
	public void CLIENT_FireGun(BaseEntity.RPCMessage rpc)
	{
		bool flag = rpc.read.Bit();
		Vector3 a = rpc.read.Vector3();
		Transform transform = flag ? this.coaxMuzzle : this.topTurretMuzzle;
		Vector3 position = transform.transform.position;
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
			component.InitializeVelocity(normalized * 250f);
		}
		gameObject.SetActive(true);
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x00049E40 File Offset: 0x00048040
	public void AimWeaponAt(Transform weaponYaw, Transform weaponPitch, Vector3 direction, float minPitch = -360f, float maxPitch = 360f, float maxYaw = 360f, Transform parentOverride = null)
	{
		Vector3 forward = weaponYaw.parent.InverseTransformDirection(direction);
		Quaternion localRotation = Quaternion.LookRotation(forward);
		Vector3 eulerAngles = localRotation.eulerAngles;
		for (int i = 0; i < 3; i++)
		{
			eulerAngles[i] -= ((eulerAngles[i] > 180f) ? 360f : 0f);
		}
		Quaternion localRotation2 = Quaternion.Euler(0f, Mathf.Clamp(eulerAngles.y, -maxYaw, maxYaw), 0f);
		Quaternion localRotation3 = Quaternion.Euler(Mathf.Clamp(eulerAngles.x, minPitch, maxPitch), 0f, 0f);
		if (weaponYaw == null && weaponPitch != null)
		{
			weaponPitch.transform.localRotation = localRotation3;
			return;
		}
		if (weaponPitch == null && weaponYaw != null)
		{
			weaponYaw.transform.localRotation = localRotation;
			return;
		}
		weaponYaw.transform.localRotation = localRotation2;
		weaponPitch.transform.localRotation = localRotation3;
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x00049F44 File Offset: 0x00048144
	public void LateUpdate()
	{
		float num = UnityEngine.Time.time - this.lastLateUpdate;
		this.lastLateUpdate = UnityEngine.Time.time;
		if (base.isServer)
		{
			float num2 = 2.0943952f;
			this.turretAimVector = Vector3.RotateTowards(this.turretAimVector, this.desiredAimVector, num2 * num, 0f);
		}
		else
		{
			this.turretAimVector = Vector3.Lerp(this.turretAimVector, this.desiredAimVector, UnityEngine.Time.deltaTime * 10f);
		}
		this.AimWeaponAt(this.mainTurret, this.coaxPitch, this.turretAimVector, -90f, 90f, 360f, null);
		this.AimWeaponAt(this.mainTurret, this.CannonPitch, this.turretAimVector, -90f, 7f, 360f, null);
		this.topTurretAimVector = Vector3.Lerp(this.topTurretAimVector, this.desiredTopTurretAimVector, UnityEngine.Time.deltaTime * 5f);
		this.AimWeaponAt(this.topTurretYaw, this.topTurretPitch, this.topTurretAimVector, -360f, 360f, 360f, this.mainTurret);
	}

	// Token: 0x0200007F RID: 127
	[Serializable]
	public class TargetInfo : Pool.IPooled
	{
		// Token: 0x04000505 RID: 1285
		public float damageReceivedFrom;

		// Token: 0x04000506 RID: 1286
		public BaseEntity entity;

		// Token: 0x04000507 RID: 1287
		public float lastSeenTime;

		// Token: 0x04000508 RID: 1288
		public Vector3 lastSeenPosition;

		// Token: 0x060007F2 RID: 2034 RVA: 0x0000872B File Offset: 0x0000692B
		public void EnterPool()
		{
			this.entity = null;
			this.lastSeenPosition = Vector3.zero;
			this.lastSeenTime = 0f;
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x0000874A File Offset: 0x0000694A
		public void Setup(BaseEntity ent, float time)
		{
			this.entity = ent;
			this.lastSeenTime = time;
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x00002ECE File Offset: 0x000010CE
		public void LeavePool()
		{
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0004A1A4 File Offset: 0x000483A4
		public float GetPriorityScore(BradleyAPC apc)
		{
			BasePlayer basePlayer = this.entity as BasePlayer;
			if (basePlayer)
			{
				float value = Vector3.Distance(this.entity.transform.position, apc.transform.position);
				float num = (1f - Mathf.InverseLerp(10f, 80f, value)) * 50f;
				float value2 = (basePlayer.GetHeldEntity() == null) ? 0f : basePlayer.GetHeldEntity().hostileScore;
				float num2 = Mathf.InverseLerp(4f, 20f, value2) * 100f;
				float num3 = Mathf.InverseLerp(10f, 3f, UnityEngine.Time.time - this.lastSeenTime) * 100f;
				float num4 = Mathf.InverseLerp(0f, 100f, this.damageReceivedFrom) * 50f;
				return num + num2 + num4 + num3;
			}
			return 0f;
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x0000875A File Offset: 0x0000695A
		public bool IsVisible()
		{
			return this.lastSeenTime != -1f && UnityEngine.Time.time - this.lastSeenTime < BradleyAPC.sightUpdateRate * 2f;
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x00008784 File Offset: 0x00006984
		public bool IsValid()
		{
			return this.entity != null;
		}
	}
}
