using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000311 RID: 785
public class PlayerModel : ListComponent<PlayerModel>, IOnParentDestroying
{
	// Token: 0x04001176 RID: 4470
	protected static int speed = Animator.StringToHash("speed");

	// Token: 0x04001177 RID: 4471
	protected static int acceleration = Animator.StringToHash("acceleration");

	// Token: 0x04001178 RID: 4472
	protected static int rotationYaw = Animator.StringToHash("rotationYaw");

	// Token: 0x04001179 RID: 4473
	protected static int forward = Animator.StringToHash("forward");

	// Token: 0x0400117A RID: 4474
	protected static int right = Animator.StringToHash("right");

	// Token: 0x0400117B RID: 4475
	protected static int up = Animator.StringToHash("up");

	// Token: 0x0400117C RID: 4476
	protected static int ducked = Animator.StringToHash("ducked");

	// Token: 0x0400117D RID: 4477
	protected static int grounded = Animator.StringToHash("grounded");

	// Token: 0x0400117E RID: 4478
	protected static int waterlevel = Animator.StringToHash("waterlevel");

	// Token: 0x0400117F RID: 4479
	protected static int attack = Animator.StringToHash("attack");

	// Token: 0x04001180 RID: 4480
	protected static int attack_alt = Animator.StringToHash("attack_alt");

	// Token: 0x04001181 RID: 4481
	protected static int deploy = Animator.StringToHash("deploy");

	// Token: 0x04001182 RID: 4482
	protected static int reload = Animator.StringToHash("reload");

	// Token: 0x04001183 RID: 4483
	protected static int throwWeapon = Animator.StringToHash("throw");

	// Token: 0x04001184 RID: 4484
	protected static int holster = Animator.StringToHash("holster");

	// Token: 0x04001185 RID: 4485
	protected static int aiming = Animator.StringToHash("aiming");

	// Token: 0x04001186 RID: 4486
	protected static int onLadder = Animator.StringToHash("onLadder");

	// Token: 0x04001187 RID: 4487
	protected static int posing = Animator.StringToHash("posing");

	// Token: 0x04001188 RID: 4488
	protected static int poseType = Animator.StringToHash("poseType");

	// Token: 0x04001189 RID: 4489
	protected static int relaxGunPose = Animator.StringToHash("relaxGunPose");

	// Token: 0x0400118A RID: 4490
	protected static int vehicle_aim_yaw = Animator.StringToHash("vehicleAimYaw");

	// Token: 0x0400118B RID: 4491
	protected static int vehicle_aim_speed = Animator.StringToHash("vehicleAimYawSpeed");

	// Token: 0x0400118C RID: 4492
	protected static int leftFootIK = Animator.StringToHash("leftFootIK");

	// Token: 0x0400118D RID: 4493
	protected static int rightFootIK = Animator.StringToHash("rightFootIK");

	// Token: 0x0400118E RID: 4494
	public BoxCollider collision;

	// Token: 0x0400118F RID: 4495
	public GameObject censorshipCube;

	// Token: 0x04001190 RID: 4496
	public GameObject censorshipCubeBreasts;

	// Token: 0x04001191 RID: 4497
	public GameObject jawBone;

	// Token: 0x04001192 RID: 4498
	public GameObject neckBone;

	// Token: 0x04001193 RID: 4499
	public GameObject headBone;

	// Token: 0x04001194 RID: 4500
	public SkeletonScale skeletonScale;

	// Token: 0x04001195 RID: 4501
	public EyeController eyeController;

	// Token: 0x04001196 RID: 4502
	public Transform[] SpineBones;

	// Token: 0x04001197 RID: 4503
	public Transform leftFootBone;

	// Token: 0x04001198 RID: 4504
	public Transform rightFootBone;

	// Token: 0x04001199 RID: 4505
	public Vector3 rightHandTarget;

	// Token: 0x0400119A RID: 4506
	public Vector3 leftHandTargetPosition;

	// Token: 0x0400119B RID: 4507
	public Quaternion leftHandTargetRotation;

	// Token: 0x0400119C RID: 4508
	public RuntimeAnimatorController DefaultHoldType;

	// Token: 0x0400119D RID: 4509
	public RuntimeAnimatorController SleepGesture;

	// Token: 0x0400119E RID: 4510
	public RuntimeAnimatorController WoundedGesture;

	// Token: 0x0400119F RID: 4511
	public RuntimeAnimatorController CurrentGesture;

	// Token: 0x040011A0 RID: 4512
	[Header("Skin")]
	public SkinSetCollection MaleSkin;

	// Token: 0x040011A1 RID: 4513
	public SkinSetCollection FemaleSkin;

	// Token: 0x040011A2 RID: 4514
	public SubsurfaceProfile subsurfaceProfile;

	// Token: 0x040011A3 RID: 4515
	[Header("Parameters")]
	[Range(0f, 1f)]
	public float voiceVolume;

	// Token: 0x040011A4 RID: 4516
	[Range(0f, 1f)]
	public float skinColor = 1f;

	// Token: 0x040011A5 RID: 4517
	[Range(0f, 1f)]
	public float skinNumber = 1f;

	// Token: 0x040011A6 RID: 4518
	[Range(0f, 1f)]
	public float meshNumber;

	// Token: 0x040011A7 RID: 4519
	[Range(0f, 1f)]
	public float hairNumber;

	// Token: 0x040011A8 RID: 4520
	[Range(0f, 1f)]
	public int skinType;

	// Token: 0x040011A9 RID: 4521
	public MovementSounds movementSounds;

	// Token: 0x040011AA RID: 4522
	private ModelState modelState;

	// Token: 0x040011AB RID: 4523
	internal Vector3 velocity = Vector3.zero;

	// Token: 0x040011AC RID: 4524
	internal Vector3 speedOverride = Vector3.zero;

	// Token: 0x040011AD RID: 4525
	private Vector3 newVelocity = Vector3.zero;

	// Token: 0x040011AE RID: 4526
	internal Vector3 position = Vector3.zero;

	// Token: 0x040011AF RID: 4527
	internal Vector3 smoothLeftFootIK;

	// Token: 0x040011B0 RID: 4528
	internal Vector3 smoothRightFootIK;

	// Token: 0x040011B1 RID: 4529
	internal Quaternion rotation = Quaternion.identity;

	// Token: 0x040011B2 RID: 4530
	internal Quaternion mountedRotation = Quaternion.identity;

	// Token: 0x040011B3 RID: 4531
	internal Vector3 mountedPosition;

	// Token: 0x040011B4 RID: 4532
	internal bool drawShadowOnly;

	// Token: 0x040011B5 RID: 4533
	internal bool isIncapacitated;

	// Token: 0x040011B6 RID: 4534
	internal uint flinchLocation;

	// Token: 0x040011B7 RID: 4535
	internal bool visible = true;

	// Token: 0x040011B8 RID: 4536
	internal PlayerNameTag nameTag;

	// Token: 0x040011B9 RID: 4537
	private bool animatorNeedsWarmup;

	// Token: 0x040011BA RID: 4538
	internal bool isLocalPlayer;

	// Token: 0x040011BB RID: 4539
	private SoundDefinition aimSoundDef;

	// Token: 0x040011BC RID: 4540
	private SoundDefinition aimEndSoundDef;

	// Token: 0x040011BD RID: 4541
	private bool InGesture;

	// Token: 0x040011BE RID: 4542
	private RuntimeAnimatorController defaultAnimatorController;

	// Token: 0x040011BF RID: 4543
	private SkinnedMultiMesh _multiMesh;

	// Token: 0x040011C0 RID: 4544
	private Animator _animator;

	// Token: 0x040011C1 RID: 4545
	private LODGroup _lodGroup;

	// Token: 0x040011C3 RID: 4547
	private float holdTypeLock;

	// Token: 0x040011C4 RID: 4548
	private const int LayerHands = 1;

	// Token: 0x040011C5 RID: 4549
	private const int LayerGestures = 2;

	// Token: 0x040011C6 RID: 4550
	private RuntimeAnimatorController _currentGesture;

	// Token: 0x040011C7 RID: 4551
	public int tempPoseType;

	// Token: 0x040011C8 RID: 4552
	private bool wasMountedRightAim;

	// Token: 0x040011C9 RID: 4553
	private int cachedMask = -1;

	// Token: 0x040011CA RID: 4554
	private int cachedConstructionMask = -1;

	// Token: 0x040011CB RID: 4555
	public bool showSash;

	// Token: 0x040011CC RID: 4556
	private HeldEntity WorkshopHeldEntity;

	// Token: 0x040011CE RID: 4558
	public static float pm_uplimit = -70f;

	// Token: 0x040011CF RID: 4559
	public static float pm_downlimit = 70f;

	// Token: 0x040011D0 RID: 4560
	public static float pm_upspine = 40f;

	// Token: 0x040011D1 RID: 4561
	public static float pm_downspine = 70f;

	// Token: 0x040011D2 RID: 4562
	public static Vector3 pm_lookoffset = new Vector3(0f, -73f, -109f);

	// Token: 0x040011D3 RID: 4563
	public static float pm_anglesmoothspeed = 32f;

	// Token: 0x040011D4 RID: 4564
	public static float pm_nohold = 0.5f;

	// Token: 0x040011D5 RID: 4565
	public static float pm_ladder = 0.5f;

	// Token: 0x040011D6 RID: 4566
	public static float pm_minweight = 0.1f;

	// Token: 0x040011D7 RID: 4567
	private float _smoothAimWeight;

	// Token: 0x040011D8 RID: 4568
	private float _smoothVelocity;

	// Token: 0x040011D9 RID: 4569
	private Vector3 _smoothlookAngle;

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x060014A7 RID: 5287 RVA: 0x0001198F File Offset: 0x0000FB8F
	public bool IsFemale
	{
		get
		{
			return this.skinType == 1;
		}
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x060014A8 RID: 5288 RVA: 0x0001199A File Offset: 0x0000FB9A
	public SkinSetCollection SkinSet
	{
		get
		{
			if (this.IsFemale)
			{
				return this.FemaleSkin;
			}
			return this.MaleSkin;
		}
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x060014A9 RID: 5289 RVA: 0x000119B1 File Offset: 0x0000FBB1
	public SkinnedMultiMesh multiMesh
	{
		get
		{
			if (this._multiMesh == null)
			{
				this._multiMesh = base.GetComponent<SkinnedMultiMesh>();
			}
			return this._multiMesh;
		}
	}

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x060014AA RID: 5290 RVA: 0x000119D3 File Offset: 0x0000FBD3
	public Animator animator
	{
		get
		{
			if (this._animator == null)
			{
				this._animator = base.GetComponent<Animator>();
			}
			return this._animator;
		}
	}

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x060014AB RID: 5291 RVA: 0x000119F5 File Offset: 0x0000FBF5
	public LODGroup lodGroup
	{
		get
		{
			if (this._lodGroup == null)
			{
				this._lodGroup = base.GetComponent<LODGroup>();
			}
			return this._lodGroup;
		}
	}

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x060014AC RID: 5292 RVA: 0x00011A17 File Offset: 0x0000FC17
	// (set) Token: 0x060014AD RID: 5293 RVA: 0x00011A1F File Offset: 0x0000FC1F
	public bool IsNpc { get; set; }

	// Token: 0x060014AE RID: 5294 RVA: 0x00011A28 File Offset: 0x0000FC28
	protected void Awake()
	{
		if (this.animator)
		{
			this.defaultAnimatorController = this.animator.runtimeAnimatorController;
		}
	}

	// Token: 0x060014AF RID: 5295 RVA: 0x0007FAA0 File Offset: 0x0007DCA0
	protected override void OnEnable()
	{
		base.OnEnable();
		this.velocity = (this.speedOverride = (this.newVelocity = Vector3.zero));
		this.rotation = base.transform.localRotation;
		this.LookAngles = this.rotation;
		this.InGesture = false;
		this._currentGesture = null;
		if (this.leftFootBone != null)
		{
			this.smoothLeftFootIK = base.transform.InverseTransformPoint(this.leftFootBone.position);
			this.smoothRightFootIK = base.transform.InverseTransformPoint(this.rightFootBone.position);
		}
		if (this.animator)
		{
			this.animator.SetLayerWeight(2, 0f);
			this.animator.enabled = true;
			if (this.animator.runtimeAnimatorController == this.DefaultHoldType || this.tempPoseType == 3 || this.tempPoseType == 4)
			{
				this.animator.SetLayerWeight(1, 0f);
			}
			else
			{
				this.animator.SetLayerWeight(1, 1f);
			}
		}
		if (this.lodGroup)
		{
			this.lodGroup.enabled = true;
		}
		if (this.nameTag)
		{
			this.nameTag.gameObject.SetActive(true);
		}
		this.UpdateCensorship();
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x0007FBF8 File Offset: 0x0007DDF8
	protected override void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		base.OnDisable();
		this.modelState = null;
		this.isIncapacitated = false;
		this.flinchLocation = 0U;
		this.visible = true;
		if (this.animator)
		{
			this.animator.runtimeAnimatorController = this.defaultAnimatorController;
		}
		if (this.nameTag)
		{
			this.nameTag.gameObject.SetActive(false);
		}
		if (this.lodGroup != null)
		{
			this.lodGroup.localReferencePoint = Vector3.zero;
			this.lodGroup.RecalculateBounds();
		}
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x00011A48 File Offset: 0x0000FC48
	public void UpdateSkeleton(int s)
	{
		this.skeletonScale.UpdateBones(s);
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x00011A56 File Offset: 0x0000FC56
	public Color GetSkinColor()
	{
		return this.SkinSet.Get(this.meshNumber).GetSkinColor(this.skinNumber);
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x00011A74 File Offset: 0x0000FC74
	public void Rebuild(bool reset = true)
	{
		if (this.SkinSet == null)
		{
			return;
		}
		this.UpdateMultiMesh(reset);
		this.UpdateCensorship();
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x00011A92 File Offset: 0x0000FC92
	public void SetSkinProperties(MaterialPropertyBlock block)
	{
		block.SetColor("_BaseColorTint", this.GetSkinColor());
		if (this.subsurfaceProfile != null)
		{
			block.SetFloat("_SubsurfaceProfile", (float)this.subsurfaceProfile.Id);
		}
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x0007FC94 File Offset: 0x0007DE94
	private void UpdateMultiMesh(bool reset)
	{
		this.multiMesh.SkinCollection = this.SkinSet;
		this.multiMesh.skinNumber = this.skinNumber;
		this.multiMesh.meshNumber = this.meshNumber;
		this.multiMesh.hairNumber = this.hairNumber;
		this.multiMesh.skinType = this.skinType;
		this.multiMesh.shadowOnly = this.drawShadowOnly;
		this.multiMesh.RebuildModel(this, reset);
		if (Effects.motionblur)
		{
			ObjectMotionVectorFix.DisableObjectMotionVectors(this.lodGroup);
		}
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x0007FD28 File Offset: 0x0007DF28
	private void UpdateCensorship()
	{
		List<Wearable> list = Pool.GetList<Wearable>();
		base.GetComponentsInChildren<Wearable>(true, list);
		bool flag = Global.censornudity > 0 && this.lodGroup.enabled && !this.drawShadowOnly;
		bool flag2;
		if (flag)
		{
			flag2 = Enumerable.Any<Wearable>(list, (Wearable x) => x.showCensorshipCube);
		}
		else
		{
			flag2 = false;
		}
		bool active = flag2;
		bool flag3;
		if (flag && this.IsFemale)
		{
			if (Enumerable.Any<Wearable>(list, (Wearable x) => x.showCensorshipCubeBreasts))
			{
				flag3 = !Enumerable.Any<Wearable>(list, (Wearable y) => y.forceHideCensorshipBreasts);
				goto IL_B7;
			}
		}
		flag3 = false;
		IL_B7:
		bool active2 = flag3;
		if (this.censorshipCube)
		{
			this.censorshipCube.SetActive(active);
		}
		if (this.censorshipCubeBreasts)
		{
			this.censorshipCubeBreasts.SetActive(active2);
		}
		Pool.FreeList<Wearable>(ref list);
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x0007FE28 File Offset: 0x0007E028
	public void UpdateCollider(bool visible, bool sleeping, bool ducking, bool wounded)
	{
		if (this.isLocalPlayer || !visible || sleeping || wounded)
		{
			this.collision.enabled = false;
			return;
		}
		this.collision.enabled = true;
		if (ducking)
		{
			this.collision.size = new Vector3(0.2f, 0.8f, 0.2f);
			this.collision.center = new Vector3(0f, 0.4f, 0f);
			return;
		}
		this.collision.size = new Vector3(0.2f, 1.6f, 0.2f);
		this.collision.center = new Vector3(0f, 0.8f, 0f);
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x00011ACA File Offset: 0x0000FCCA
	public void UpdateModelState(ModelState ms)
	{
		if (this.modelState != null)
		{
			this.modelState.ResetToPool();
		}
		this.modelState = ms.Copy();
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x00011AEB File Offset: 0x0000FCEB
	public void Attack()
	{
		this.animator.SetTrigger(PlayerModel.attack);
	}

	// Token: 0x060014BA RID: 5306 RVA: 0x00011AFD File Offset: 0x0000FCFD
	public void AltAttack()
	{
		this.animator.SetTrigger(PlayerModel.attack_alt);
	}

	// Token: 0x060014BB RID: 5307 RVA: 0x00011B0F File Offset: 0x0000FD0F
	public void Deploy()
	{
		if (!this.animator.isInitialized)
		{
			return;
		}
		this.animator.SetTrigger(PlayerModel.deploy);
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x00011B2F File Offset: 0x0000FD2F
	public void Reload()
	{
		this.animator.SetTrigger(PlayerModel.reload);
	}

	// Token: 0x060014BD RID: 5309 RVA: 0x00011B41 File Offset: 0x0000FD41
	public void Holster()
	{
		this.animator.SetTrigger(PlayerModel.holster);
	}

	// Token: 0x060014BE RID: 5310 RVA: 0x00011B53 File Offset: 0x0000FD53
	public void Flinch(uint location)
	{
		this.flinchLocation = location;
		this.Flinch();
	}

	// Token: 0x060014BF RID: 5311 RVA: 0x00011B62 File Offset: 0x0000FD62
	public void Flinch()
	{
		bool isInitialized = this.animator.isInitialized;
	}

	// Token: 0x060014C0 RID: 5312 RVA: 0x00011B70 File Offset: 0x0000FD70
	public void Throw()
	{
		this.animator.SetTrigger(PlayerModel.throwWeapon);
		this.holdTypeLock = UnityEngine.Time.time + 1f;
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x0007FEE4 File Offset: 0x0007E0E4
	public void Gesture(string gesture)
	{
		if (this.animator.runtimeAnimatorController == this.DefaultHoldType || this.tempPoseType == 3 || this.tempPoseType == 4)
		{
			this.animator.SetLayerWeight(1, 1f);
			base.Invoke(new Action(this.StopGesture), 2f);
		}
		this.animator.Play(gesture, 1);
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x0007FF50 File Offset: 0x0007E150
	public void StopGesture()
	{
		if (this.animator.runtimeAnimatorController == this.DefaultHoldType || this.tempPoseType == 3 || this.tempPoseType == 4)
		{
			this.animator.SetLayerWeight(1, 0f);
			return;
		}
		this.animator.SetLayerWeight(1, 1f);
	}

	// Token: 0x060014C3 RID: 5315 RVA: 0x0007FFAC File Offset: 0x0007E1AC
	public void SetHoldType(RuntimeAnimatorController holdType)
	{
		if (this.InGesture)
		{
			return;
		}
		if (this.holdTypeLock > UnityEngine.Time.time)
		{
			return;
		}
		if (holdType == null)
		{
			holdType = this.DefaultHoldType;
		}
		if (!this.animator.isInitialized)
		{
			return;
		}
		if (this.animator.runtimeAnimatorController == holdType)
		{
			return;
		}
		this.animator.runtimeAnimatorController = holdType;
		this.UpdateParameters();
		if (this.animator.runtimeAnimatorController == this.DefaultHoldType || this.tempPoseType == 3 || this.tempPoseType == 4)
		{
			this.animator.SetLayerWeight(1, 0f);
		}
		else
		{
			this.animator.SetLayerWeight(1, 1f);
			this.Deploy();
		}
		if (this.animator.isInitialized && this.animator.IsInTransition(0))
		{
			AnimatorStateInfo nextAnimatorStateInfo = this.animator.GetNextAnimatorStateInfo(0);
			this.animator.Play(nextAnimatorStateInfo.shortNameHash);
		}
	}

	// Token: 0x060014C4 RID: 5316 RVA: 0x000800A4 File Offset: 0x0007E2A4
	public void SetGesture()
	{
		if (this.animator == null)
		{
			return;
		}
		if (this._currentGesture == this.CurrentGesture)
		{
			return;
		}
		this._currentGesture = this.CurrentGesture;
		if (this._currentGesture == null)
		{
			this.animator.SetLayerWeight(2, 0f);
			this.InGesture = false;
			return;
		}
		this.animator.runtimeAnimatorController = this._currentGesture;
		this.animator.SetLayerWeight(2, 1f);
		this.animator.Play("gesture_start", 2, 0f);
		base.StopAllCoroutines();
		this.InGesture = true;
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x00011B93 File Offset: 0x0000FD93
	public IEnumerator FadeOutGestureLayer()
	{
		Debug.LogFormat("FadeOutGestureLayer", Array.Empty<object>());
		this.animator.CrossFade("gesture_end", 0.1f, 2);
		AnimatorOverrideController animatorOverrideController = this.animator.runtimeAnimatorController as AnimatorOverrideController;
		if (animatorOverrideController)
		{
			Debug.LogFormat("gesture_end is {0} seconds ", new object[]
			{
				animatorOverrideController["gesture_end"].length
			});
			yield return CoroutineEx.waitForSeconds(animatorOverrideController["gesture_end"].length);
		}
		this.animator.SetLayerWeight(2, 0f);
		this.InGesture = false;
		yield break;
	}

	// Token: 0x060014C6 RID: 5318 RVA: 0x0008014C File Offset: 0x0007E34C
	internal void UpdateRotation()
	{
		float num = 0f;
		if (this.modelState != null && this.modelState.mounted)
		{
			base.transform.rotation = this.mountedRotation;
			return;
		}
		if (base.transform.rotation != this.rotation)
		{
			float num2 = Quaternion.Angle(base.transform.rotation, this.rotation);
			if (num2 > 40f || this.velocity.sqrMagnitude > 0.2f)
			{
				float num3 = Mathf.Lerp(2f, 32f, (num2 - 60f) / 90f + this.velocity.magnitude / 2f);
				Quaternion b = base.transform.rotation;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.rotation, UnityEngine.Time.smoothDeltaTime * num3);
				num = Quaternion.Angle(base.transform.rotation, b);
			}
		}
		if (this.animator.isInitialized)
		{
			this.animator.SetFloat(PlayerModel.rotationYaw, num, 0.1f, UnityEngine.Time.smoothDeltaTime);
		}
	}

	// Token: 0x060014C7 RID: 5319 RVA: 0x00011BA2 File Offset: 0x0000FDA2
	public void UpdatePosition()
	{
		if (base.transform.position != this.position)
		{
			base.transform.position = this.position;
		}
	}

	// Token: 0x060014C8 RID: 5320 RVA: 0x00011BCD File Offset: 0x0000FDCD
	public void UpdateLocalVelocity(Vector3 velocity, Transform parent)
	{
		if (parent != null)
		{
			velocity = parent.TransformDirection(velocity);
		}
		this.newVelocity = velocity;
	}

	// Token: 0x060014C9 RID: 5321 RVA: 0x00080270 File Offset: 0x0007E470
	public void UpdateVelocity()
	{
		Vector3 vector = base.transform.InverseTransformDirection(this.newVelocity);
		if (this.speedOverride != Vector3.zero)
		{
			vector = this.speedOverride;
		}
		if (Vector3Ex.IsNaNOrInfinity(vector))
		{
			vector = Vector3.zero;
		}
		this.velocity = Vector3.Lerp(this.velocity, vector, UnityEngine.Time.deltaTime * 10f);
		if (this.modelState.mounted)
		{
			this.velocity = Vector3.zero;
		}
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x00011BE8 File Offset: 0x0000FDE8
	private void AnimatorWarmup()
	{
		this.animatorNeedsWarmup = false;
		this.animator.Update(1f);
		this.animator.Update(1f);
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x000802EC File Offset: 0x0007E4EC
	private void FrameUpdate_Sleeping()
	{
		this.UpdateRotation();
		this.UpdatePosition();
		if (!this.animator.isInitialized)
		{
			return;
		}
		this.animator.SetFloat(PlayerModel.speed, 0f);
		this.animator.SetFloat(PlayerModel.forward, 0f);
		this.animator.SetFloat(PlayerModel.right, 0f);
		this.animator.SetBool(PlayerModel.ducked, false);
		this.animator.SetBool(PlayerModel.grounded, true);
		this.animator.SetBool(PlayerModel.posing, false);
		this.animator.SetFloat(PlayerModel.waterlevel, 0f);
		this.animator.SetBool(PlayerModel.relaxGunPose, false);
		this.CurrentGesture = this.SleepGesture;
		if (this.animatorNeedsWarmup)
		{
			this.AnimatorWarmup();
		}
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x000803C8 File Offset: 0x0007E5C8
	private void FrameUpdate_Incapacitated()
	{
		this.UpdateRotation();
		this.UpdatePosition();
		if (!this.animator.isInitialized)
		{
			return;
		}
		this.animator.SetFloat(PlayerModel.speed, 0f);
		this.animator.SetFloat(PlayerModel.forward, 0f);
		this.animator.SetFloat(PlayerModel.right, 0f);
		this.animator.SetBool(PlayerModel.ducked, false);
		this.animator.SetBool(PlayerModel.grounded, true);
		this.animator.SetBool(PlayerModel.posing, false);
		this.animator.SetFloat(PlayerModel.waterlevel, 0f);
		this.animator.SetBool(PlayerModel.relaxGunPose, false);
		this.CurrentGesture = this.WoundedGesture;
		this.SetHoldType(null);
		this.SetAimSounds(null, null);
		if (this.animatorNeedsWarmup)
		{
			this.AnimatorWarmup();
		}
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x000804B0 File Offset: 0x0007E6B0
	private void FrameUpdate_OnLadder()
	{
		this.UpdateVelocity();
		this.UpdateRotation();
		this.UpdatePosition();
		if (!this.animator.isInitialized)
		{
			return;
		}
		this.animator.SetFloat(PlayerModel.speed, 0f);
		this.animator.SetFloat(PlayerModel.forward, 0f);
		this.animator.SetBool(PlayerModel.ducked, false);
		this.animator.SetBool(PlayerModel.grounded, true);
		this.animator.SetFloat(PlayerModel.waterlevel, 0f);
		this.animator.SetBool(PlayerModel.onLadder, true);
		this.animator.SetFloat(PlayerModel.up, this.velocity.y, 0.05f, UnityEngine.Time.smoothDeltaTime);
		this.animator.SetFloat(PlayerModel.right, this.velocity.x, 0.05f, UnityEngine.Time.smoothDeltaTime);
		if (this.animatorNeedsWarmup)
		{
			this.AnimatorWarmup();
		}
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x00011C11 File Offset: 0x0000FE11
	private static Vector3 GetFlat(Vector3 dir)
	{
		dir.y = 0f;
		return dir.normalized;
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x000805A8 File Offset: 0x0007E7A8
	public float GetAim360()
	{
		Vector3 from = this.mountedRotation * Vector3.forward;
		from.y = 0f;
		from.Normalize();
		Vector3 lookDir = this.modelState.lookDir;
		lookDir.y = 0f;
		lookDir.Normalize();
		float num = Vector3.Angle(from, lookDir) / 360f;
		if (Vector3.Dot(PlayerModel.GetFlat(this.mountedRotation * Vector3.right), lookDir) > 0f)
		{
			num = 1f - num;
		}
		return num;
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x00080634 File Offset: 0x0007E834
	public void UpdateVehicleAim360()
	{
		if (this.modelState.mounted)
		{
			Vector3 from = this.mountedRotation * Vector3.forward;
			from.y = 0f;
			from.Normalize();
			Vector3 lookDir = this.modelState.lookDir;
			lookDir.y = 0f;
			lookDir.Normalize();
			float num = Vector3.Angle(from, lookDir) / 360f;
			bool flag = Vector3.Dot(PlayerModel.GetFlat(this.mountedRotation * Vector3.right), lookDir) > 0f;
			if (flag)
			{
				num = 1f - num;
			}
			if (this.wasMountedRightAim != flag)
			{
				this.animator.SetFloat(PlayerModel.vehicle_aim_yaw, num);
			}
			else
			{
				this.animator.SetFloat(PlayerModel.vehicle_aim_yaw, num, 0.3f, UnityEngine.Time.smoothDeltaTime * 10f);
			}
			this.wasMountedRightAim = flag;
			this.animator.SetFloat(PlayerModel.vehicle_aim_speed, 0f);
			return;
		}
		this.animator.SetFloat(PlayerModel.vehicle_aim_speed, 1f);
		this.animator.SetFloat(PlayerModel.vehicle_aim_yaw, 0f);
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x00080754 File Offset: 0x0007E954
	private void UpdateParameters()
	{
		if (this.modelState == null)
		{
			return;
		}
		float @float = this.animator.GetFloat(PlayerModel.aiming);
		float num = (this.modelState.waterLevel < 0.5f) ? 0.2f : 2f;
		this.animator.SetFloat(PlayerModel.speed, this.velocity.magnitude, 1f, UnityEngine.Time.smoothDeltaTime);
		this.animator.SetFloat(PlayerModel.forward, this.velocity.z, num, UnityEngine.Time.smoothDeltaTime);
		this.animator.SetFloat(PlayerModel.right, this.velocity.x, num, UnityEngine.Time.smoothDeltaTime);
		this.animator.SetBool(PlayerModel.ducked, this.modelState.ducked);
		this.animator.SetBool(PlayerModel.grounded, this.modelState.onground);
		this.animator.SetFloat(PlayerModel.waterlevel, this.modelState.waterLevel, 0.05f, UnityEngine.Time.smoothDeltaTime * 10f);
		this.animator.SetFloat(PlayerModel.aiming, (float)(this.modelState.aiming ? 1 : 0), 0.3f, UnityEngine.Time.smoothDeltaTime * 5f);
		this.animator.SetBool(PlayerModel.posing, this.modelState.mounted && this.modelState.poseType != 128);
		this.UpdateVehicleAim360();
		this.animator.SetFloat(PlayerModel.poseType, this.modelState.mounted ? ((float)this.modelState.poseType) : 0f);
		this.animator.SetBool(PlayerModel.relaxGunPose, this.modelState.relaxed);
		float float2 = this.animator.GetFloat(PlayerModel.aiming);
		this.DoAimingSounds(@float, float2);
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x00080934 File Offset: 0x0007EB34
	private void DoAimingSounds(float prevAiming, float currentAiming)
	{
		if (LocalPlayer.Entity == null)
		{
			return;
		}
		if (LocalPlayer.Entity.playerModel == this && LocalPlayer.Entity.InFirstPersonMode())
		{
			return;
		}
		if (prevAiming <= 0.02f && currentAiming > 0.02f && this.aimSoundDef != null)
		{
			SoundManager.PlayOneshot(this.aimSoundDef, base.gameObject, false, default(Vector3));
			return;
		}
		if (prevAiming >= 0.9f && currentAiming < 0.9f && this.aimEndSoundDef != null)
		{
			SoundManager.PlayOneshot(this.aimEndSoundDef, base.gameObject, false, default(Vector3));
		}
	}

	// Token: 0x060014D3 RID: 5331 RVA: 0x00011C26 File Offset: 0x0000FE26
	public void SetAimSounds(SoundDefinition aimDef, SoundDefinition aimEndDef)
	{
		this.aimSoundDef = aimDef;
		this.aimEndSoundDef = aimEndDef;
	}

	// Token: 0x060014D4 RID: 5332 RVA: 0x000809E4 File Offset: 0x0007EBE4
	private void FrameUpdate_Default()
	{
		this.UpdateVelocity();
		this.UpdateRotation();
		this.UpdatePosition();
		if (!this.animator.isInitialized)
		{
			return;
		}
		this.animator.SetBool(PlayerModel.onLadder, false);
		this.animator.SetBool(PlayerModel.posing, false);
		this.UpdateParameters();
		if (this.movementSounds)
		{
			this.movementSounds.mute = this.modelState.mounted;
		}
		if (this.animatorNeedsWarmup)
		{
			this.AnimatorWarmup();
		}
	}

	// Token: 0x060014D5 RID: 5333 RVA: 0x00080A6C File Offset: 0x0007EC6C
	public void ApplyVisibility(bool vis, bool animatorVis, bool shadowVis)
	{
		bool enabled = this.lodGroup.enabled;
		bool enabled2 = this.animator.enabled;
		float num = (float)(vis ? 0 : 100000);
		if (num != this.lodGroup.localReferencePoint.x)
		{
			this.lodGroup.localReferencePoint = new Vector3(num, num, num);
		}
		this.animator.enabled = (animatorVis || this.isIncapacitated);
		if (!enabled2 && this.animator.enabled)
		{
			this.animatorNeedsWarmup = true;
		}
		if (enabled != this.lodGroup.enabled)
		{
			this.UpdateCensorship();
		}
		if (this.nameTag != null)
		{
			this.nameTag.PositionUpdate(this.visible);
		}
	}

	// Token: 0x060014D6 RID: 5334 RVA: 0x00080B24 File Offset: 0x0007ED24
	public void FrameUpdate(bool wounded)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		this.UpdateCollider(this.visible, this.modelState != null && this.modelState.sleeping, this.modelState != null && this.modelState.ducked, wounded);
		this.multiMesh.SetVisible(this.visible);
		if (this.nameTag != null)
		{
			this.nameTag.PositionUpdate(this.visible);
		}
		if (!this.visible)
		{
			return;
		}
		if (this.animator == null)
		{
			return;
		}
		if (this.modelState != null && this.modelState.sleeping)
		{
			this.FrameUpdate_Sleeping();
		}
		else if (this.isIncapacitated)
		{
			this.FrameUpdate_Incapacitated();
		}
		else if (this.modelState != null && this.modelState.onLadder)
		{
			this.FrameUpdate_OnLadder();
		}
		else
		{
			this.FrameUpdate_Default();
		}
		this.SetGesture();
		this.CurrentGesture = null;
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x00080C20 File Offset: 0x0007EE20
	private void UpdateGlobalShaderConstants()
	{
		if (this.isLocalPlayer)
		{
			Vector3 v = Vector3.Lerp(this.leftFootBone.transform.position, this.rightFootBone.transform.position, 0.5f);
			Shader.SetGlobalVector("global_ViewModelFeetPosition", v);
		}
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x00080C70 File Offset: 0x0007EE70
	public void LateCycle()
	{
		if (this.WorkshopHeldEntity)
		{
			this.WorkshopHeldEntity.UpdatePlayerModel(this);
		}
		if (this.modelState == null)
		{
			return;
		}
		if (this.modelState.sleeping)
		{
			return;
		}
		if (this.jawBone != null && this.voiceVolume > 0f)
		{
			Vector3 localPosition = this.jawBone.transform.localPosition;
			localPosition.x = Mathf.Lerp(-0.033f, 0f, this.voiceVolume);
			localPosition.y += Mathf.Sin(UnityEngine.Time.time * 1f) * this.voiceVolume * 0.001f;
			localPosition.z += Mathf.Cos(UnityEngine.Time.time * 1f) * this.voiceVolume * 0.001f;
			this.jawBone.transform.localPosition = localPosition;
			Transform parent = this.jawBone.transform.parent;
			Quaternion localRotation = parent.localRotation;
			localRotation.z += -0.03f * this.voiceVolume;
			parent.localRotation = localRotation;
		}
		if (this.eyeController != null)
		{
			this.eyeController.UpdateEyes();
		}
		if (this.SpineBones != null && this.SpineBones.Length != 0 && !this.InGesture)
		{
			this.SpineIk();
		}
		this.UpdateGlobalShaderConstants();
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x00002ECE File Offset: 0x000010CE
	public void LateUpdate()
	{
	}

	// Token: 0x060014DA RID: 5338 RVA: 0x00080DD0 File Offset: 0x0007EFD0
	private void OnAnimatorIK()
	{
		if (this.leftFootBone == null)
		{
			return;
		}
		float num = MainCamera.Distance(base.transform.position);
		Vector3 footPos = this.leftFootBone.position;
		Vector3 vector = base.transform.InverseTransformPoint(footPos);
		Vector3 footPos2 = this.rightFootBone.position;
		Vector3 vector2 = base.transform.InverseTransformPoint(footPos2);
		if (this.modelState.mounted && this.leftHandTargetPosition != Vector3.zero && num < 30f && Player.footik)
		{
			this.animator.SetIKPositionWeight(2, 1f);
			this.animator.SetIKRotationWeight(2, 1f);
			this.animator.SetIKPosition(2, this.leftHandTargetPosition);
			this.animator.SetIKRotation(2, this.leftHandTargetRotation);
		}
		if (!Player.footik || this.modelState == null || this.modelState.sleeping || this.modelState.waterLevel > 0.5f || this.modelState.mounted || this.modelState.flying || this.modelState.onLadder || !this.modelState.onground || this.isIncapacitated || num > Player.footikdistance || (this.IsNpc && AI.npc_no_foot_ik) || base.transform.parent != null)
		{
			this.smoothLeftFootIK = vector;
			this.smoothRightFootIK = vector2;
			return;
		}
		float num2 = (this.velocity.magnitude > 0.5f) ? 40f : 10f;
		if (this.animator.GetFloat(PlayerModel.leftFootIK) > 0.01f)
		{
			Vector3 vector3;
			Vector3 footPlacement = this.GetFootPlacement(footPos, out vector3);
			Vector3 vector4 = base.transform.InverseTransformPoint(footPlacement);
			this.smoothLeftFootIK = new Vector3(vector.x, Mathf.Lerp(this.smoothLeftFootIK.y, vector4.y, UnityEngine.Time.deltaTime * num2), vector.z);
			this.animator.SetIKPositionWeight(0, this.animator.GetFloat(PlayerModel.leftFootIK));
			if (this.modelState.ducked && this.velocity.magnitude < 0.5f && footPlacement != this.leftFootBone.position)
			{
				this.animator.SetIKPosition(0, base.transform.TransformPoint(this.smoothLeftFootIK + Vector3.up * 0.075f));
			}
			else
			{
				this.animator.SetIKPosition(0, base.transform.TransformPoint(this.smoothLeftFootIK));
			}
			if (vector3 != Vector3.zero && Vector3.Dot(vector3, Vector3.up) > 0.6f && (!this.modelState.ducked || this.velocity.magnitude > 0.5f))
			{
				this.animator.SetIKRotationWeight(0, this.animator.GetFloat(PlayerModel.leftFootIK));
				this.animator.SetIKRotation(0, Quaternion.LookRotation(Vector3.Cross(this.leftFootBone.forward * -1f + this.leftFootBone.right * 0.4f + this.leftFootBone.up * 0.14f, vector3), vector3));
			}
		}
		else
		{
			this.smoothLeftFootIK = vector;
		}
		if (this.animator.GetFloat(PlayerModel.rightFootIK) > 0.01f)
		{
			Vector3 vector5;
			Vector3 footPlacement2 = this.GetFootPlacement(footPos2, out vector5);
			Vector3 vector6 = base.transform.InverseTransformPoint(footPlacement2);
			this.smoothRightFootIK = new Vector3(vector2.x, Mathf.Lerp(this.smoothRightFootIK.y, vector6.y, UnityEngine.Time.deltaTime * num2), vector2.z);
			this.animator.SetIKPositionWeight(1, this.animator.GetFloat(PlayerModel.rightFootIK));
			this.animator.SetIKPosition(1, base.transform.TransformPoint(this.smoothRightFootIK));
			if (vector5 != Vector3.zero && Vector3.Dot(vector5, Vector3.up) > 0.6f)
			{
				this.animator.SetIKRotationWeight(1, this.animator.GetFloat(PlayerModel.rightFootIK));
				this.animator.SetIKRotation(1, Quaternion.LookRotation(Vector3.Cross(this.rightFootBone.forward * -1f, vector5), vector5));
				return;
			}
		}
		else
		{
			this.smoothRightFootIK = vector2;
		}
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x00081264 File Offset: 0x0007F464
	private Vector3 GetFootPlacement(Vector3 footPos, out Vector3 surfaceNormal)
	{
		if (this.cachedMask == -1)
		{
			this.cachedMask = LayerMask.GetMask(new string[]
			{
				"Terrain",
				"World",
				"Construction",
				"Deployed",
				"Clutter",
				"Deployed",
				"Tree",
				"Default",
				"Vehicle Movement"
			});
		}
		if (this.cachedConstructionMask == -1)
		{
			this.cachedConstructionMask = LayerMask.GetMask(new string[]
			{
				"Construction"
			});
		}
		float num = 0.4f;
		if (this.modelState.ducked)
		{
			num = ((this.velocity.magnitude > 0.5f) ? 0.28f : 0.18f);
		}
		float num2 = num + 0.2f;
		if (this.modelState.ducked)
		{
			num2 = num + 0.45f;
		}
		RaycastHit raycastHit;
		if (!Physics.SphereCast(new Ray(footPos + Vector3.up * num, Vector3.up * -1f), 0.15f, ref raycastHit, num2, this.cachedMask) || !(raycastHit.collider != null) || !(raycastHit.collider.gameObject != null))
		{
			surfaceNormal = Vector3.zero;
			return footPos;
		}
		if (Mathf.Pow(2f, (float)raycastHit.collider.gameObject.layer) == (float)this.cachedConstructionMask)
		{
			surfaceNormal = new Vector3(0f, 1f, 0f);
			return new Vector3(footPos.x, raycastHit.point.y + 0.08f, footPos.z);
		}
		surfaceNormal = raycastHit.normal;
		return new Vector3(footPos.x, raycastHit.point.y + 0.08f, footPos.z);
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x00002ECE File Offset: 0x000010CE
	private void SetFootRotation(Transform footBone, Vector3 surfaceNormal)
	{
	}

	// Token: 0x060014DD RID: 5341 RVA: 0x00011C36 File Offset: 0x0000FE36
	public void AlwaysAnimate(bool b)
	{
		if (b)
		{
			this.animator.cullingMode = 0;
			return;
		}
		this.animator.cullingMode = 1;
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x00011C54 File Offset: 0x0000FE54
	public Transform FindBone(string strName)
	{
		return this.multiMesh.FindBone(strName);
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x0008144C File Offset: 0x0007F64C
	public void Clear()
	{
		this.multiMesh.Clear();
		this.multiMesh.parts.Clear();
		bool flag2;
		bool flag = (flag2 = (Global.censornudity > 1)) && this.IsFemale;
		this.AddPart("assets/prefabs/clothes/skin/skin_head.prefab", null, Array.Empty<Type>());
		this.AddPart("assets/prefabs/clothes/skin/skin_feet.prefab", null, Array.Empty<Type>());
		this.AddPart("assets/prefabs/clothes/skin/skin_hands.prefab", null, Array.Empty<Type>());
		if (flag2)
		{
			this.AddPart("assets/prefabs/clothes/skin/underwear/skin_legs.prefab", null, Array.Empty<Type>());
		}
		else
		{
			this.AddPart("assets/prefabs/clothes/skin/skin_legs.prefab", null, Array.Empty<Type>());
		}
		if (flag)
		{
			this.AddPart("assets/prefabs/clothes/skin/underwear/skin_torso.prefab", null, Array.Empty<Type>());
		}
		else
		{
			this.AddPart("assets/prefabs/clothes/skin/skin_torso.prefab", null, Array.Empty<Type>());
		}
		this.AddPart("assets/prefabs/clothes/hair/hair_head.prefab", null, Array.Empty<Type>());
		this.AddPart("assets/prefabs/clothes/hair/hair_facial.prefab", null, Array.Empty<Type>());
		this.AddPart("assets/prefabs/clothes/hair/hair_eyebrow.prefab", null, Array.Empty<Type>());
		this.AddPart("assets/prefabs/clothes/hair/hair_armpit.prefab", null, Array.Empty<Type>());
		if (!flag2)
		{
			this.AddPart("assets/prefabs/clothes/hair/hair_pubic.prefab", null, Array.Empty<Type>());
		}
		if (this.showSash)
		{
			this.AddPart("assets/prefabs/clothes/skin/skin_clothsash.prefab", null, Array.Empty<Type>());
		}
		if (!this.isLocalPlayer)
		{
			this.AddPart("assets/prefabs/clothes/collider/collider_head.prefab", null, Array.Empty<Type>());
			this.AddPart("assets/prefabs/clothes/collider/collider_feet.prefab", null, Array.Empty<Type>());
			this.AddPart("assets/prefabs/clothes/collider/collider_legs.prefab", null, Array.Empty<Type>());
			this.AddPart("assets/prefabs/clothes/collider/collider_torso.prefab", null, Array.Empty<Type>());
			this.AddPart("assets/prefabs/clothes/collider/collider_hands.prefab", null, Array.Empty<Type>());
		}
		if (this.censorshipCube)
		{
			this.censorshipCube.SetActive(true);
		}
		if (this.censorshipCubeBreasts)
		{
			this.censorshipCubeBreasts.SetActive(false);
		}
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x0008160C File Offset: 0x0007F80C
	public void RefreshUnderwear()
	{
		bool flag2;
		bool flag = (flag2 = (Global.censornudity > 1)) && this.IsFemale;
		if (this.HasPart("skin_legs"))
		{
			this.RemovePart("skin_legs");
			if (flag2)
			{
				this.AddPart("assets/prefabs/clothes/skin/underwear/skin_legs.prefab", null, Array.Empty<Type>());
			}
			else
			{
				this.AddPart("assets/prefabs/clothes/skin/skin_legs.prefab", null, Array.Empty<Type>());
			}
		}
		if (this.HasPart("skin_torso"))
		{
			this.RemovePart("skin_torso");
			if (flag)
			{
				this.AddPart("assets/prefabs/clothes/skin/underwear/skin_torso.prefab", null, Array.Empty<Type>());
			}
			else
			{
				this.AddPart("assets/prefabs/clothes/skin/skin_torso.prefab", null, Array.Empty<Type>());
			}
		}
		if (this.HasPart("hair_pubic"))
		{
			if (flag2)
			{
				this.RemovePart("hair_pubic");
				return;
			}
		}
		else if (!flag2)
		{
			this.AddPart("assets/prefabs/clothes/hair/hair_pubic.prefab", null, Array.Empty<Type>());
		}
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x000816E0 File Offset: 0x0007F8E0
	public void AddPart(string strName, Item item = null, params Type[] resetTypes)
	{
		GameObject gameObject = GameManager.client.FindPrefab(strName);
		if (!gameObject)
		{
			Debug.LogError("Couldn't find clothes part " + strName);
			return;
		}
		this.multiMesh.parts.Add(new SkinnedMultiMesh.Part
		{
			gameObject = gameObject,
			name = strName,
			item = item
		});
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x00081744 File Offset: 0x0007F944
	public void RemovePart(string strName)
	{
		this.multiMesh.parts.RemoveAll((SkinnedMultiMesh.Part x) => x.gameObject.name == strName);
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x0008177C File Offset: 0x0007F97C
	public bool HasPart(string strName)
	{
		for (int i = 0; i < this.multiMesh.parts.Count; i++)
		{
			if (this.multiMesh.parts[i].gameObject.name == strName)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x000817CC File Offset: 0x0007F9CC
	public void UpdateFrom(PlayerModel mdl)
	{
		this.skinType = mdl.skinType;
		this.skinColor = mdl.skinColor;
		this.skinNumber = mdl.skinNumber;
		this.meshNumber = mdl.meshNumber;
		this.hairNumber = mdl.hairNumber;
		this.multiMesh.parts = mdl.multiMesh.parts;
		this.Rebuild(true);
	}

	// Token: 0x060014E5 RID: 5349 RVA: 0x00011C62 File Offset: 0x0000FE62
	public void OnParentDestroying()
	{
		if (this.nameTag)
		{
			GameManager.client.Retire(this.nameTag.gameObject);
			this.nameTag = null;
		}
		this.multiMesh.Clear();
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x00081834 File Offset: 0x0007FA34
	public static void RebuildAll()
	{
		foreach (PlayerModel playerModel in ListComponent<PlayerModel>.InstanceList)
		{
			playerModel.RefreshUnderwear();
			playerModel.Rebuild(true);
			BasePlayer basePlayer = playerModel.gameObject.ToBaseEntity() as BasePlayer;
			if (basePlayer)
			{
				basePlayer.UpdateClothingItems(playerModel.multiMesh);
			}
		}
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x000818AC File Offset: 0x0007FAAC
	public void ForceModelSeed(ulong userID)
	{
		this.skinType = ((BasePlayer.GetRandomFloatBasedOnUserID(userID, 4332UL) > 0.5f) ? 1 : 0);
		this.skinColor = BasePlayer.GetRandomFloatBasedOnUserID(userID, 5977UL);
		this.skinNumber = BasePlayer.GetRandomFloatBasedOnUserID(userID, 3975UL);
		this.meshNumber = BasePlayer.GetRandomFloatBasedOnUserID(userID, 2647UL);
		this.hairNumber = BasePlayer.GetRandomFloatBasedOnUserID(userID, 6338UL);
		this.UpdateSkeleton((int)userID);
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x00081928 File Offset: 0x0007FB28
	public void WorkshopPreviewSetup(GameObject[] objects)
	{
		this.Clear();
		this.position = base.transform.position;
		this.rotation = base.transform.rotation;
		foreach (GameObject gameObject in objects)
		{
			Wearable component = gameObject.GetComponent<Wearable>();
			this.WorkshopHeldEntity = gameObject.GetComponent<HeldEntity>();
			if (component)
			{
				component.SetupRendererCache(null);
				component.transform.SetParent(base.transform);
				component.gameObject.SetActive(false);
				this.multiMesh.parts.Add(new SkinnedMultiMesh.Part
				{
					gameObject = component.gameObject,
					name = null,
					item = null
				});
				if (component)
				{
					if ((component.removeSkin & Wearable.RemoveSkin.Feet) != (Wearable.RemoveSkin)0)
					{
						this.RemovePart("skin_feet");
					}
					if ((component.removeSkin & Wearable.RemoveSkin.Hands) != (Wearable.RemoveSkin)0)
					{
						this.RemovePart("skin_hands");
					}
					if ((component.removeSkin & Wearable.RemoveSkin.Torso) != (Wearable.RemoveSkin)0)
					{
						this.RemovePart("skin_torso");
					}
					if ((component.removeSkin & Wearable.RemoveSkin.Legs) != (Wearable.RemoveSkin)0)
					{
						this.RemovePart("skin_legs");
					}
					if ((component.removeSkin & Wearable.RemoveSkin.Head) != (Wearable.RemoveSkin)0)
					{
						this.RemovePart("skin_head");
					}
					if ((component.removeHair & Wearable.RemoveHair.Head) != (Wearable.RemoveHair)0)
					{
						this.RemovePart("hair_head");
					}
					if ((component.removeHair & Wearable.RemoveHair.Eyebrow) != (Wearable.RemoveHair)0)
					{
						this.RemovePart("hair_eyebrow");
					}
					if ((component.removeHair & Wearable.RemoveHair.Facial) != (Wearable.RemoveHair)0)
					{
						this.RemovePart("hair_facial");
					}
					if ((component.removeHair & Wearable.RemoveHair.Armpit) != (Wearable.RemoveHair)0)
					{
						this.RemovePart("hair_armpit");
					}
					if ((component.removeHair & Wearable.RemoveHair.Pubic) != (Wearable.RemoveHair)0)
					{
						this.RemovePart("hair_pubic");
					}
				}
			}
			if (this.WorkshopHeldEntity)
			{
				this.WorkshopHeldEntity.gameObject.SetActive(true);
				Transform parent = this.FindBone(this.WorkshopHeldEntity.handBone);
				this.WorkshopHeldEntity.transform.SetParent(parent, false);
				this.WorkshopHeldEntity.transform.localPosition = Vector3.zero;
				this.WorkshopHeldEntity.transform.localRotation = Quaternion.identity;
				this.WorkshopHeldEntity.UpdatePlayerModel(this);
			}
		}
		this.Rebuild(false);
		ModelState ms = new ModelState
		{
			onground = true,
			waterLevel = 0f,
			flying = false,
			sprinting = false,
			ducked = false,
			onLadder = false,
			sleeping = false,
			mounted = false,
			relaxed = false,
			poseType = 0
		};
		this.UpdateModelState(ms);
		this.FrameUpdate(false);
		this.animator.Update(1f);
		this.animator.Update(1f);
		this.animator.Update(1f);
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x060014E9 RID: 5353 RVA: 0x00011C98 File Offset: 0x0000FE98
	// (set) Token: 0x060014EA RID: 5354 RVA: 0x00011CA0 File Offset: 0x0000FEA0
	public Quaternion LookAngles { get; set; }

	// Token: 0x060014EB RID: 5355 RVA: 0x00081BF0 File Offset: 0x0007FDF0
	private void SpineIk()
	{
		Vector3 eulerAngles = this.LookAngles.eulerAngles;
		this._smoothlookAngle.x = Mathf.LerpAngle(this._smoothlookAngle.x, eulerAngles.x, UnityEngine.Time.smoothDeltaTime * PlayerModel.pm_anglesmoothspeed);
		this._smoothlookAngle.y = Mathf.LerpAngle(this._smoothlookAngle.y, eulerAngles.y, UnityEngine.Time.smoothDeltaTime * PlayerModel.pm_anglesmoothspeed);
		this._smoothlookAngle.z = Mathf.LerpAngle(this._smoothlookAngle.z, eulerAngles.z, UnityEngine.Time.smoothDeltaTime * PlayerModel.pm_anglesmoothspeed);
		float num = 1f;
		if (this.animator.runtimeAnimatorController == this.DefaultHoldType)
		{
			num *= PlayerModel.pm_nohold;
		}
		else if (this.tempPoseType == 3 || this.tempPoseType == 4)
		{
			num = 0f;
		}
		if (this.modelState != null && this.modelState.onLadder)
		{
			num *= PlayerModel.pm_ladder;
		}
		float magnitude = this.velocity.magnitude;
		num -= magnitude / 10f;
		if (num < PlayerModel.pm_minweight)
		{
			num = PlayerModel.pm_minweight;
		}
		if (this.modelState != null && this.modelState.mounted && this.modelState.poseType != 128)
		{
			if (this.animator.runtimeAnimatorController == this.DefaultHoldType)
			{
				num = 0f;
			}
			else
			{
				num = 1f;
			}
		}
		this._smoothAimWeight = Mathf.SmoothDamp(this._smoothAimWeight, num, ref this._smoothVelocity, UnityEngine.Time.smoothDeltaTime * 4f);
		num = this._smoothAimWeight;
		float t = Mathf.InverseLerp(PlayerModel.pm_upspine, PlayerModel.pm_downspine, this._smoothlookAngle.x);
		this._smoothlookAngle.x = Mathf.Clamp(this._smoothlookAngle.x, PlayerModel.pm_uplimit, PlayerModel.pm_downlimit);
		Quaternion b = Quaternion.Euler(this._smoothlookAngle) * Quaternion.Euler(PlayerModel.pm_lookoffset);
		this.SpineBones[0].rotation = Quaternion.Lerp(this.SpineBones[0].rotation, b, Mathf.Lerp(0.1f, 0.5f, t) * num);
		this.SpineBones[1].rotation = Quaternion.Lerp(this.SpineBones[1].rotation, b, Mathf.Lerp(0.2f, 0.6f, t) * num);
		this.SpineBones[2].rotation = Quaternion.Lerp(this.SpineBones[2].rotation, b, Mathf.Lerp(0.4f, 0.8f, t) * num);
		this.SpineBones[3].rotation = Quaternion.Lerp(this.SpineBones[3].rotation, b, Mathf.Lerp(1f, 1f, t) * num);
	}

	// Token: 0x02000312 RID: 786
	public enum MountPoses
	{
		// Token: 0x040011DB RID: 4571
		Chair,
		// Token: 0x040011DC RID: 4572
		Driving,
		// Token: 0x040011DD RID: 4573
		Horseback,
		// Token: 0x040011DE RID: 4574
		HeliUnarmed,
		// Token: 0x040011DF RID: 4575
		HeliArmed,
		// Token: 0x040011E0 RID: 4576
		HandMotorBoat,
		// Token: 0x040011E1 RID: 4577
		MotorBoatPassenger,
		// Token: 0x040011E2 RID: 4578
		SitGeneric,
		// Token: 0x040011E3 RID: 4579
		SitRaft,
		// Token: 0x040011E4 RID: 4580
		StandDrive,
		// Token: 0x040011E5 RID: 4581
		SitShootingGeneric,
		// Token: 0x040011E6 RID: 4582
		SitMinicopter_Pilot,
		// Token: 0x040011E7 RID: 4583
		SitMinicopter_Passenger,
		// Token: 0x040011E8 RID: 4584
		Standing = 128
	}
}
