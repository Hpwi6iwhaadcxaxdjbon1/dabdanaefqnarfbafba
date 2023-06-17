using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x0200021E RID: 542
public class Projectile : BaseMonoBehaviour
{
	// Token: 0x04000D4E RID: 3406
	public const float lifeTime = 8f;

	// Token: 0x04000D4F RID: 3407
	[Header("Attributes")]
	public Vector3 initialVelocity;

	// Token: 0x04000D50 RID: 3408
	public float drag;

	// Token: 0x04000D51 RID: 3409
	public float gravityModifier = 1f;

	// Token: 0x04000D52 RID: 3410
	public float thickness;

	// Token: 0x04000D53 RID: 3411
	[Tooltip("This projectile will raycast for this many units, and then become a projectile. This is typically done for bullets.")]
	public float initialDistance;

	// Token: 0x04000D54 RID: 3412
	[Header("Impact Rules")]
	public bool remainInWorld;

	// Token: 0x04000D55 RID: 3413
	[Range(0f, 1f)]
	public float stickProbability = 1f;

	// Token: 0x04000D56 RID: 3414
	[Range(0f, 1f)]
	public float breakProbability;

	// Token: 0x04000D57 RID: 3415
	[Range(0f, 1f)]
	public float conditionLoss;

	// Token: 0x04000D58 RID: 3416
	[Range(0f, 1f)]
	public float ricochetChance;

	// Token: 0x04000D59 RID: 3417
	public float penetrationPower = 1f;

	// Token: 0x04000D5A RID: 3418
	[Header("Damage")]
	public DamageProperties damageProperties;

	// Token: 0x04000D5B RID: 3419
	[Horizontal(2, -1)]
	public MinMax damageDistances = new MinMax(10f, 100f);

	// Token: 0x04000D5C RID: 3420
	[Horizontal(2, -1)]
	public MinMax damageMultipliers = new MinMax(1f, 0.8f);

	// Token: 0x04000D5D RID: 3421
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	// Token: 0x04000D5E RID: 3422
	[Header("Rendering")]
	public ScaleRenderer rendererToScale;

	// Token: 0x04000D5F RID: 3423
	public ScaleRenderer firstPersonRenderer;

	// Token: 0x04000D60 RID: 3424
	public bool createDecals = true;

	// Token: 0x04000D61 RID: 3425
	[Header("Audio")]
	public SoundDefinition flybySound;

	// Token: 0x04000D62 RID: 3426
	public float flybySoundDistance = 7f;

	// Token: 0x04000D63 RID: 3427
	public SoundDefinition closeFlybySound;

	// Token: 0x04000D64 RID: 3428
	public float closeFlybyDistance = 3f;

	// Token: 0x04000D65 RID: 3429
	[Header("Tumble")]
	public float tumbleSpeed;

	// Token: 0x04000D66 RID: 3430
	public Vector3 tumbleAxis = Vector3.right;

	// Token: 0x04000D67 RID: 3431
	[NonSerialized]
	public BasePlayer owner;

	// Token: 0x04000D68 RID: 3432
	[NonSerialized]
	public AttackEntity sourceWeaponPrefab;

	// Token: 0x04000D69 RID: 3433
	[NonSerialized]
	public Projectile sourceProjectilePrefab;

	// Token: 0x04000D6A RID: 3434
	[NonSerialized]
	public ItemModProjectile mod;

	// Token: 0x04000D6B RID: 3435
	[NonSerialized]
	public int projectileID;

	// Token: 0x04000D6C RID: 3436
	[NonSerialized]
	public int seed;

	// Token: 0x04000D6D RID: 3437
	[NonSerialized]
	public bool clientsideEffect;

	// Token: 0x04000D6E RID: 3438
	[NonSerialized]
	public bool clientsideAttack;

	// Token: 0x04000D6F RID: 3439
	[NonSerialized]
	public float integrity = 1f;

	// Token: 0x04000D70 RID: 3440
	[NonSerialized]
	public float maxDistance = float.PositiveInfinity;

	// Token: 0x04000D71 RID: 3441
	[NonSerialized]
	public Projectile.Modifier modifier = Projectile.Modifier.Default;

	// Token: 0x04000D72 RID: 3442
	[NonSerialized]
	public bool invisible;

	// Token: 0x04000D73 RID: 3443
	private Vector3 currentVelocity;

	// Token: 0x04000D74 RID: 3444
	private Vector3 currentPosition;

	// Token: 0x04000D75 RID: 3445
	private float traveledDistance;

	// Token: 0x04000D76 RID: 3446
	private float traveledTime;

	// Token: 0x04000D77 RID: 3447
	private Vector3 sentPosition;

	// Token: 0x04000D78 RID: 3448
	private Vector3 previousPosition;

	// Token: 0x04000D79 RID: 3449
	private float previousTraveledTime;

	// Token: 0x04000D7A RID: 3450
	private bool isRetiring;

	// Token: 0x04000D7B RID: 3451
	private bool flybyPlayed;

	// Token: 0x04000D7C RID: 3452
	private bool wasFacingPlayer;

	// Token: 0x04000D7D RID: 3453
	private Plane flybyPlane;

	// Token: 0x04000D7E RID: 3454
	private Ray flybyRay;

	// Token: 0x04000D7F RID: 3455
	private Action cleanupAction;

	// Token: 0x04000D80 RID: 3456
	private HitTest hitTest;

	// Token: 0x04000D81 RID: 3457
	private static uint _fleshMaterialID;

	// Token: 0x04000D82 RID: 3458
	private static uint _waterMaterialID;

	// Token: 0x04000D83 RID: 3459
	private static uint cachedWaterString;

	// Token: 0x06001081 RID: 4225 RVA: 0x0006F190 File Offset: 0x0006D390
	public void CalculateDamage(HitInfo info, Projectile.Modifier mod, float scale)
	{
		float num = this.damageMultipliers.Lerp(mod.distanceOffset + mod.distanceScale * this.damageDistances.x, mod.distanceOffset + mod.distanceScale * this.damageDistances.y, info.ProjectileDistance);
		float num2 = scale * (mod.damageOffset + mod.damageScale * num);
		foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
		{
			info.damageTypes.Add(damageTypeEntry.type, damageTypeEntry.amount * num2);
		}
		if (Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				" Projectile damage: ",
				info.damageTypes.Total(),
				" (scalar=",
				num2,
				")"
			}));
		}
	}

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x06001082 RID: 4226 RVA: 0x0000E87E File Offset: 0x0000CA7E
	public bool isAuthoritative
	{
		get
		{
			return this.owner != null && this.owner.IsLocalPlayer();
		}
	}

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x06001083 RID: 4227 RVA: 0x0000E89B File Offset: 0x0000CA9B
	private bool isAlive
	{
		get
		{
			return this.integrity > 0.001f && this.traveledDistance < this.maxDistance && this.traveledTime < 8f;
		}
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x0006F298 File Offset: 0x0006D498
	private void Retire()
	{
		if (this.isRetiring)
		{
			return;
		}
		this.isRetiring = true;
		float num = 0f;
		List<MeshRenderer> list = Pool.GetList<MeshRenderer>();
		base.GetComponentsInChildren<MeshRenderer>(list);
		foreach (MeshRenderer meshRenderer in list)
		{
			meshRenderer.enabled = false;
		}
		Pool.FreeList<MeshRenderer>(ref list);
		List<TrailRenderer> list2 = Pool.GetList<TrailRenderer>();
		base.GetComponentsInChildren<TrailRenderer>(list2);
		foreach (TrailRenderer trailRenderer in list2)
		{
			num = Mathf.Max(num, trailRenderer.time);
		}
		Pool.FreeList<TrailRenderer>(ref list2);
		List<ParticleSystem> list3 = Pool.GetList<ParticleSystem>();
		base.GetComponentsInChildren<ParticleSystem>(list3);
		foreach (ParticleSystem particleSystem in list3)
		{
			num = Mathf.Max(num, particleSystem.main.startLifetime.constantMax);
			particleSystem.Stop();
		}
		Pool.FreeList<ParticleSystem>(ref list3);
		if (this.cleanupAction == null)
		{
			this.cleanupAction = new Action(this.Cleanup);
		}
		base.Invoke(this.cleanupAction, num);
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x0006F404 File Offset: 0x0006D604
	private void Cleanup()
	{
		base.gameObject.BroadcastOnParentDestroying();
		List<MeshRenderer> list = Pool.GetList<MeshRenderer>();
		base.GetComponentsInChildren<MeshRenderer>(list);
		foreach (MeshRenderer meshRenderer in list)
		{
			meshRenderer.enabled = true;
		}
		Pool.FreeList<MeshRenderer>(ref list);
		GameManager.client.Retire(base.gameObject);
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x0000E8C7 File Offset: 0x0000CAC7
	public void AdjustVelocity(Vector3 delta)
	{
		this.currentVelocity += delta;
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x0000E8DB File Offset: 0x0000CADB
	public void InitializeVelocity(Vector3 overrideVel)
	{
		this.initialVelocity = overrideVel;
		this.currentVelocity = this.initialVelocity;
		this.SetEffectScale(0f);
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x0006F480 File Offset: 0x0006D680
	protected void OnDisable()
	{
		this.currentVelocity = Vector3.zero;
		this.currentPosition = Vector3.zero;
		this.traveledDistance = 0f;
		this.traveledTime = 0f;
		this.sentPosition = Vector3.zero;
		this.previousPosition = Vector3.zero;
		this.previousTraveledTime = 0f;
		this.isRetiring = false;
		this.flybyPlayed = false;
		this.wasFacingPlayer = false;
		this.flybyPlane = default(Plane);
		this.flybyRay = default(Ray);
		this.owner = null;
		this.sourceWeaponPrefab = null;
		this.sourceProjectilePrefab = null;
		this.mod = null;
		this.projectileID = 0;
		this.seed = 0;
		this.clientsideEffect = false;
		this.clientsideAttack = false;
		this.integrity = 1f;
		this.maxDistance = float.PositiveInfinity;
		this.modifier = Projectile.Modifier.Default;
		this.invisible = false;
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x0000E8FB File Offset: 0x0000CAFB
	protected void FixedUpdate()
	{
		if (this.isAlive)
		{
			this.UpdateVelocity(UnityEngine.Time.fixedDeltaTime);
		}
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x0000E910 File Offset: 0x0000CB10
	protected void Update()
	{
		if (this.isAlive)
		{
			this.SetEffectScale(this.CalculateEffectScale());
			this.DoFlybySound();
			return;
		}
		this.Retire();
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x0006F568 File Offset: 0x0006D768
	private void SetEffectScale(float eScale)
	{
		if (this.rendererToScale)
		{
			this.rendererToScale.SetScale(eScale);
		}
		if (this.firstPersonRenderer)
		{
			this.firstPersonRenderer.SetScale(this.isAuthoritative ? eScale : 0f);
		}
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x0006F5B8 File Offset: 0x0006D7B8
	private void DoFlybySound()
	{
		if (this.flybyPlayed)
		{
			return;
		}
		if (this.flybySound == null)
		{
			return;
		}
		if (!MainCamera.isValid)
		{
			return;
		}
		if (this.owner != null && this.owner.IsLocalPlayer())
		{
			return;
		}
		Vector3 normalized = (MainCamera.position - base.transform.position).normalized;
		bool flag = Vector3.Dot(base.transform.forward, normalized) > 0f;
		if (MainCamera.Distance(base.transform.position) <= this.flybySoundDistance)
		{
			this.flybyPlane.SetNormalAndPosition(base.transform.forward, MainCamera.position);
			this.flybyRay.origin = base.transform.position + base.transform.forward * -100f;
			this.flybyRay.direction = base.transform.forward;
			float distance = 0f;
			Vector3 vector = base.transform.position;
			if (this.flybyPlane.Raycast(this.flybyRay, out distance))
			{
				vector = this.flybyRay.GetPoint(distance);
			}
			float num = Vector3.Distance(vector, MainCamera.position);
			if (this.wasFacingPlayer && !flag)
			{
				SoundDefinition soundDefinition = (num > this.closeFlybyDistance || this.closeFlybySound == null) ? this.flybySound : this.closeFlybySound;
				if (soundDefinition != null)
				{
					SoundManager.PlayOneshot(soundDefinition, null, false, vector);
				}
				MusicManager.RaiseIntensityTo(0.75f, 0);
				this.flybyPlayed = true;
			}
		}
		this.wasFacingPlayer = flag;
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x0000E933 File Offset: 0x0000CB33
	private float CalculateEffectScale()
	{
		if (this.invisible)
		{
			return 0f;
		}
		if (this.traveledDistance < 6f)
		{
			return 0f;
		}
		return Mathf.InverseLerp(12f, 20f, this.currentVelocity.magnitude);
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x0006F75C File Offset: 0x0006D95C
	private void UpdateVelocity(float deltaTime)
	{
		if (this.traveledTime != 0f)
		{
			this.previousPosition = this.currentPosition;
			this.previousTraveledTime = this.traveledTime;
		}
		this.currentPosition = base.transform.position;
		if (this.traveledTime == 0f)
		{
			this.sentPosition = (this.previousPosition = this.currentPosition);
		}
		deltaTime *= UnityEngine.Time.timeScale;
		this.DoMovement(deltaTime);
		this.DoVelocityUpdate(deltaTime);
		base.transform.position = this.currentPosition;
		if (this.tumbleSpeed > 0f)
		{
			base.transform.Rotate(this.tumbleAxis, this.tumbleSpeed * deltaTime);
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(this.currentVelocity);
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x0006F828 File Offset: 0x0006DA28
	private void DoVelocityUpdate(float deltaTime)
	{
		this.currentVelocity += Physics.gravity * this.gravityModifier * deltaTime;
		this.currentVelocity -= this.currentVelocity * this.drag * deltaTime;
		if (this.isAuthoritative)
		{
			Vector3 p = this.sentPosition;
			Vector3 p2 = this.currentPosition;
			if (!GamePhysics.LineOfSight(p, p2, 2162688, 0f))
			{
				using (PlayerProjectileUpdate playerProjectileUpdate = Pool.Get<PlayerProjectileUpdate>())
				{
					playerProjectileUpdate.projectileID = this.projectileID;
					playerProjectileUpdate.curPosition = this.previousPosition;
					playerProjectileUpdate.travelTime = this.previousTraveledTime;
					this.owner.SendProjectileUpdate(playerProjectileUpdate);
					this.sentPosition = this.previousPosition;
				}
			}
		}
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x0006F90C File Offset: 0x0006DB0C
	private void DoMovement(float deltaTime)
	{
		Vector3 a = this.currentVelocity * deltaTime;
		float magnitude = a.magnitude;
		float num = 1f / magnitude;
		Vector3 vector = a * num;
		bool flag = false;
		Vector3 vector2 = this.currentPosition + vector * magnitude;
		float num2 = this.traveledTime + deltaTime;
		if (this.hitTest == null)
		{
			this.hitTest = new HitTest();
		}
		else
		{
			this.hitTest.Clear();
		}
		this.hitTest.AttackRay = new Ray(this.currentPosition, vector);
		this.hitTest.MaxDistance = magnitude;
		this.hitTest.ignoreEntity = this.owner;
		this.hitTest.Radius = 0f;
		this.hitTest.Forgiveness = this.thickness;
		this.hitTest.type = (this.isAuthoritative ? HitTest.Type.Projectile : HitTest.Type.ProjectileEffect);
		if (this.sourceWeaponPrefab)
		{
			this.hitTest.BestHit = true;
			this.hitTest.damageProperties = this.damageProperties;
		}
		List<TraceInfo> list = Pool.GetList<TraceInfo>();
		GameTrace.TraceAll(this.hitTest, list, 1269916433);
		int num3 = 0;
		while (num3 < list.Count && this.isAlive && !flag)
		{
			if (list[num3].valid)
			{
				list[num3].UpdateHitTest(this.hitTest);
				Vector3 vector3 = this.hitTest.HitPointWorld();
				Vector3 normal = this.hitTest.HitNormalWorld();
				if (ConVar.Vis.attack && this.isAuthoritative && LocalPlayer.isAdmin)
				{
					UnityEngine.DDraw.Line(this.currentPosition, vector3, Color.yellow, 60f, true, true);
					UnityEngine.DDraw.Sphere(vector3, this.thickness, Color.yellow, 60f, true);
					if (this.hitTest.HitTransform)
					{
						UnityEngine.DDraw.Text(this.hitTest.HitTransform.name, vector3, Color.white, 60f);
					}
				}
				float magnitude2 = (vector3 - this.currentPosition).magnitude;
				float num4 = magnitude2 * num * deltaTime;
				this.traveledDistance += magnitude2;
				this.traveledTime += num4;
				this.currentPosition = vector3;
				if (this.DoRicochet(this.hitTest, vector3, normal) || this.DoHit(this.hitTest, vector3, normal))
				{
					flag = true;
				}
			}
			num3++;
		}
		Pool.FreeList<TraceInfo>(ref list);
		if (this.isAlive)
		{
			if (flag && this.traveledTime < num2)
			{
				this.DoMovement(num2 - this.traveledTime);
				return;
			}
			if (ConVar.Vis.attack && this.isAuthoritative && LocalPlayer.isAdmin)
			{
				UnityEngine.DDraw.Arrow(this.currentPosition, vector2, this.thickness, Color.yellow, 60f);
			}
			float magnitude3 = (vector2 - this.currentPosition).magnitude;
			float num5 = magnitude3 * num * deltaTime;
			this.traveledDistance += magnitude3;
			this.traveledTime += num5;
			this.currentPosition = vector2;
		}
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x0006FC2C File Offset: 0x0006DE2C
	private bool DoWaterHit(ref HitTest test, Vector3 targetPosition)
	{
		float height = TerrainMeta.WaterMap.GetHeight(targetPosition);
		if (height >= targetPosition.y)
		{
			Vector3 vector = targetPosition;
			vector.y = height;
			Vector3 normal = TerrainMeta.WaterMap.GetNormal(targetPosition);
			test.DidHit = true;
			test.HitEntity = null;
			test.HitDistance = Vector3.Distance(test.AttackRay.origin, targetPosition);
			test.HitMaterial = "Water";
			test.HitPart = 0U;
			test.HitTransform = null;
			test.HitPoint = vector;
			test.HitNormal = normal;
			test.collider = null;
			test.gameObject = null;
			this.DoHit(test, vector, normal);
			this.integrity = 0f;
			return true;
		}
		return false;
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x0006FCE8 File Offset: 0x0006DEE8
	private bool DoRicochet(HitTest test, Vector3 point, Vector3 normal)
	{
		Vector3 hitPosition = this.currentPosition;
		Vector3 inVelocity = this.currentVelocity;
		bool flag = false;
		uint num = (uint)this.seed;
		if ((!(test.HitEntity != null) || !(test.HitEntity is BaseCombatEntity)) && this.ricochetChance > 0f && SeedRandom.Value(ref num) <= this.ricochetChance && !Projectile.IsWaterMaterial(test.HitMaterial))
		{
			flag = this.Reflect(ref num, point, normal);
		}
		if (flag)
		{
			Effect.client.Run("assets/bundled/prefabs/fx/ricochet/ricochet" + SeedRandom.Range(ref num, 1, 5) + ".prefab", point, normal, default(Vector3));
			if (this.isAuthoritative)
			{
				using (PlayerProjectileRicochet playerProjectileRicochet = Pool.Get<PlayerProjectileRicochet>())
				{
					playerProjectileRicochet.projectileID = this.projectileID;
					playerProjectileRicochet.hitPosition = hitPosition;
					playerProjectileRicochet.inVelocity = inVelocity;
					playerProjectileRicochet.outVelocity = this.currentVelocity;
					playerProjectileRicochet.hitNormal = normal;
					playerProjectileRicochet.travelTime = this.traveledTime;
					this.owner.SendProjectileRicochet(playerProjectileRicochet);
					this.sentPosition = this.currentPosition;
				}
			}
		}
		this.seed = (int)num;
		return flag;
	}

	// Token: 0x06001093 RID: 4243 RVA: 0x0006FE1C File Offset: 0x0006E01C
	private bool DoHit(HitTest test, Vector3 point, Vector3 normal)
	{
		bool result = false;
		uint num = (uint)this.seed;
		using (PlayerProjectileAttack playerProjectileAttack = Pool.Get<PlayerProjectileAttack>())
		{
			playerProjectileAttack.playerAttack = Pool.Get<PlayerAttack>();
			playerProjectileAttack.playerAttack.attack = test.BuildAttackMessage();
			playerProjectileAttack.playerAttack.projectileID = this.projectileID;
			HitInfo hitInfo = new HitInfo();
			hitInfo.LoadFromAttack(playerProjectileAttack.playerAttack.attack, false);
			hitInfo.Initiator = this.owner;
			hitInfo.ProjectileID = this.projectileID;
			hitInfo.ProjectileDistance = this.traveledDistance;
			hitInfo.ProjectileVelocity = this.currentVelocity;
			hitInfo.ProjectilePrefab = this.sourceProjectilePrefab;
			hitInfo.IsPredicting = true;
			hitInfo.WeaponPrefab = this.sourceWeaponPrefab;
			hitInfo.DoDecals = this.createDecals;
			this.CalculateDamage(hitInfo, this.modifier, this.integrity);
			if (hitInfo.HitEntity == null && Projectile.IsWaterMaterial(test.HitMaterial))
			{
				this.currentVelocity *= 0.1f;
				this.currentPosition += this.currentVelocity.normalized * 0.001f;
				this.integrity = Mathf.Clamp01(this.integrity - 0.1f);
				result = true;
			}
			else if (this.penetrationPower <= 0f || hitInfo.HitEntity == null)
			{
				this.integrity = 0f;
			}
			else
			{
				float num2 = hitInfo.HitEntity.PenetrationResistance(hitInfo) / this.penetrationPower;
				result = this.Refract(ref num, point, normal, num2);
				this.integrity = Mathf.Clamp01(this.integrity - num2);
			}
			if (this.isAuthoritative)
			{
				playerProjectileAttack.hitVelocity = this.currentVelocity;
				playerProjectileAttack.hitDistance = this.traveledDistance;
				playerProjectileAttack.travelTime = this.traveledTime;
				this.owner.SendProjectileAttack(playerProjectileAttack);
				this.sentPosition = this.currentPosition;
			}
			if ((this.clientsideAttack || (this.isAuthoritative && ConVar.Client.prediction)) && hitInfo.HitEntity != null)
			{
				hitInfo.HitEntity.OnAttacked(hitInfo);
			}
			if (this.clientsideEffect || (this.isAuthoritative && ConVar.Client.prediction))
			{
				Effect.client.ImpactEffect(hitInfo);
			}
		}
		this.seed = (int)num;
		return result;
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x00070080 File Offset: 0x0006E280
	private bool Reflect(ref uint seed, Vector3 point, Vector3 normal)
	{
		bool result = false;
		if (this.currentVelocity.magnitude > 50f)
		{
			float num = 90f - Vector3.Angle(-this.currentVelocity, normal);
			float num2 = Mathf.Clamp01(1f - num / 30f) * 0.8f;
			if (num2 > 0f)
			{
				Quaternion lhs = Quaternion.LookRotation(normal);
				Quaternion rhs = this.RandomRotation(ref seed, 10f);
				Vector3 normalized = (lhs * rhs * Vector3.forward).normalized;
				this.currentVelocity = Vector3.Reflect(this.currentVelocity, normalized) * num2;
				this.currentPosition += this.currentVelocity.normalized * 0.001f;
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x00070150 File Offset: 0x0006E350
	private bool Refract(ref uint seed, Vector3 point, Vector3 normal, float resistance)
	{
		bool result = false;
		if (resistance < 1f)
		{
			float num = Mathf.Lerp(1f, 0.5f, resistance);
			if (num > 0f)
			{
				Quaternion lhs = Quaternion.LookRotation(normal);
				Quaternion rhs = this.RandomRotation(ref seed, 10f);
				Vector3 normalized = (lhs * rhs * Vector3.forward).normalized;
				this.currentVelocity = this.Refract(this.currentVelocity, normalized, resistance) * num;
				this.currentPosition += this.currentVelocity.normalized * 0.001f;
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001096 RID: 4246 RVA: 0x000701F8 File Offset: 0x0006E3F8
	private Vector3 Refract(Vector3 v, Vector3 n, float f)
	{
		float magnitude = v.magnitude;
		return Vector3.Slerp(v / magnitude, -n, f) * magnitude;
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x00070228 File Offset: 0x0006E428
	private Quaternion RandomRotation(ref uint seed, float range)
	{
		float x = SeedRandom.Range(ref seed, -range, range);
		float y = SeedRandom.Range(ref seed, -range, range);
		float z = SeedRandom.Range(ref seed, -range, range);
		return Quaternion.Euler(x, y, z);
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x0000E970 File Offset: 0x0000CB70
	internal void Launch()
	{
		while (this.isAlive && (this.traveledDistance < this.initialDistance || this.traveledTime < 0.1f))
		{
			this.UpdateVelocity(UnityEngine.Time.fixedDeltaTime);
		}
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x0000E9A2 File Offset: 0x0000CBA2
	public static uint FleshMaterialID()
	{
		if (Projectile._fleshMaterialID == 0U)
		{
			Projectile._fleshMaterialID = StringPool.Get("flesh");
		}
		return Projectile._fleshMaterialID;
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x0000E9BF File Offset: 0x0000CBBF
	public static uint WaterMaterialID()
	{
		if (Projectile._waterMaterialID == 0U)
		{
			Projectile._waterMaterialID = StringPool.Get("Water");
		}
		return Projectile._waterMaterialID;
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x0000E9DC File Offset: 0x0000CBDC
	public static bool IsWaterMaterial(string hitMaterial)
	{
		if (Projectile.cachedWaterString == 0U)
		{
			Projectile.cachedWaterString = StringPool.Get("Water");
		}
		return StringPool.Get(hitMaterial) == Projectile.cachedWaterString;
	}

	// Token: 0x0200021F RID: 543
	public struct Modifier
	{
		// Token: 0x04000D84 RID: 3460
		public float damageScale;

		// Token: 0x04000D85 RID: 3461
		public float damageOffset;

		// Token: 0x04000D86 RID: 3462
		public float distanceScale;

		// Token: 0x04000D87 RID: 3463
		public float distanceOffset;

		// Token: 0x04000D88 RID: 3464
		public static Projectile.Modifier Default = new Projectile.Modifier
		{
			damageScale = 1f,
			damageOffset = 0f,
			distanceScale = 1f,
			distanceOffset = 0f
		};
	}
}
