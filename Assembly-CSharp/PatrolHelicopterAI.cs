using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F1 RID: 753
public class PatrolHelicopterAI : BaseMonoBehaviour
{
	// Token: 0x040010BD RID: 4285
	public Vector3 interestZoneOrigin;

	// Token: 0x040010BE RID: 4286
	public Vector3 destination;

	// Token: 0x040010BF RID: 4287
	public bool hasInterestZone;

	// Token: 0x040010C0 RID: 4288
	public float moveSpeed;

	// Token: 0x040010C1 RID: 4289
	public float maxSpeed = 25f;

	// Token: 0x040010C2 RID: 4290
	public float courseAdjustLerpTime = 2f;

	// Token: 0x040010C3 RID: 4291
	public Quaternion targetRotation;

	// Token: 0x040010C4 RID: 4292
	public Vector3 windVec;

	// Token: 0x040010C5 RID: 4293
	public Vector3 targetWindVec;

	// Token: 0x040010C6 RID: 4294
	public float windForce = 5f;

	// Token: 0x040010C7 RID: 4295
	public float windFrequency = 1f;

	// Token: 0x040010C8 RID: 4296
	public float targetThrottleSpeed;

	// Token: 0x040010C9 RID: 4297
	public float throttleSpeed;

	// Token: 0x040010CA RID: 4298
	public float maxRotationSpeed = 90f;

	// Token: 0x040010CB RID: 4299
	public float rotationSpeed;

	// Token: 0x040010CC RID: 4300
	public float terrainPushForce = 100f;

	// Token: 0x040010CD RID: 4301
	public float obstaclePushForce = 100f;

	// Token: 0x040010CE RID: 4302
	public HelicopterTurret leftGun;

	// Token: 0x040010CF RID: 4303
	public HelicopterTurret rightGun;

	// Token: 0x040010D0 RID: 4304
	public static PatrolHelicopterAI heliInstance;

	// Token: 0x040010D1 RID: 4305
	public BaseHelicopter helicopterBase;

	// Token: 0x040010D2 RID: 4306
	public PatrolHelicopterAI.aiState _currentState;

	// Token: 0x040010D3 RID: 4307
	public List<PatrolHelicopterAI.targetinfo> _targetList = new List<PatrolHelicopterAI.targetinfo>();

	// Token: 0x040010D4 RID: 4308
	public List<MonumentInfo> _visitedMonuments;

	// Token: 0x040010D5 RID: 4309
	public float arrivalTime;

	// Token: 0x040010D6 RID: 4310
	public GameObjectRef rocketProjectile;

	// Token: 0x040010D7 RID: 4311
	public GameObjectRef rocketProjectile_Napalm;

	// Token: 0x020002F2 RID: 754
	public class targetinfo
	{
		// Token: 0x040010D8 RID: 4312
		public BasePlayer ply;

		// Token: 0x040010D9 RID: 4313
		public BaseEntity ent;

		// Token: 0x040010DA RID: 4314
		public float lastSeenTime = float.PositiveInfinity;

		// Token: 0x040010DB RID: 4315
		public float visibleFor;

		// Token: 0x040010DC RID: 4316
		public float nextLOSCheck;

		// Token: 0x06001404 RID: 5124 RVA: 0x00011103 File Offset: 0x0000F303
		public targetinfo(BaseEntity initEnt, BasePlayer initPly = null)
		{
			this.ply = initPly;
			this.ent = initEnt;
			this.lastSeenTime = float.PositiveInfinity;
			this.nextLOSCheck = Time.realtimeSinceStartup + 1.5f;
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x00011140 File Offset: 0x0000F340
		public bool IsVisible()
		{
			return this.TimeSinceSeen() < 1.5f;
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x0001114F File Offset: 0x0000F34F
		public float TimeSinceSeen()
		{
			return Time.realtimeSinceStartup - this.lastSeenTime;
		}
	}

	// Token: 0x020002F3 RID: 755
	public enum aiState
	{
		// Token: 0x040010DE RID: 4318
		IDLE,
		// Token: 0x040010DF RID: 4319
		MOVE,
		// Token: 0x040010E0 RID: 4320
		ORBIT,
		// Token: 0x040010E1 RID: 4321
		STRAFE,
		// Token: 0x040010E2 RID: 4322
		PATROL,
		// Token: 0x040010E3 RID: 4323
		GUARD,
		// Token: 0x040010E4 RID: 4324
		DEATH
	}
}
