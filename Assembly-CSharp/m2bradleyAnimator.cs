using System;
using UnityEngine;

// Token: 0x020002EA RID: 746
public class m2bradleyAnimator : MonoBehaviour
{
	// Token: 0x0400106F RID: 4207
	public Animator m2Animator;

	// Token: 0x04001070 RID: 4208
	public Material treadLeftMaterial;

	// Token: 0x04001071 RID: 4209
	public Material treadRightMaterial;

	// Token: 0x04001072 RID: 4210
	private Rigidbody mainRigidbody;

	// Token: 0x04001073 RID: 4211
	[Header("GunBones")]
	public Transform turret;

	// Token: 0x04001074 RID: 4212
	public Transform mainCannon;

	// Token: 0x04001075 RID: 4213
	public Transform coaxGun;

	// Token: 0x04001076 RID: 4214
	public Transform rocketsPitch;

	// Token: 0x04001077 RID: 4215
	public Transform spotLightYaw;

	// Token: 0x04001078 RID: 4216
	public Transform spotLightPitch;

	// Token: 0x04001079 RID: 4217
	public Transform sideMG;

	// Token: 0x0400107A RID: 4218
	public Transform[] sideguns;

	// Token: 0x0400107B RID: 4219
	[Header("WheelBones")]
	public Transform[] ShocksBones;

	// Token: 0x0400107C RID: 4220
	public Transform[] ShockTraceLineBegin;

	// Token: 0x0400107D RID: 4221
	public Vector3[] vecShocksOffsetPosition;

	// Token: 0x0400107E RID: 4222
	[Header("Targeting")]
	public Transform targetTurret;

	// Token: 0x0400107F RID: 4223
	public Transform targetSpotLight;

	// Token: 0x04001080 RID: 4224
	public Transform[] targetSideguns;

	// Token: 0x04001081 RID: 4225
	private Vector3 vecTurret = new Vector3(0f, 0f, 0f);

	// Token: 0x04001082 RID: 4226
	private Vector3 vecMainCannon = new Vector3(0f, 0f, 0f);

	// Token: 0x04001083 RID: 4227
	private Vector3 vecCoaxGun = new Vector3(0f, 0f, 0f);

	// Token: 0x04001084 RID: 4228
	private Vector3 vecRocketsPitch = new Vector3(0f, 0f, 0f);

	// Token: 0x04001085 RID: 4229
	private Vector3 vecSpotLightBase = new Vector3(0f, 0f, 0f);

	// Token: 0x04001086 RID: 4230
	private Vector3 vecSpotLight = new Vector3(0f, 0f, 0f);

	// Token: 0x04001087 RID: 4231
	private float sideMGPitchValue;

	// Token: 0x04001088 RID: 4232
	[Header("MuzzleFlash locations")]
	public GameObject muzzleflashCannon;

	// Token: 0x04001089 RID: 4233
	public GameObject muzzleflashCoaxGun;

	// Token: 0x0400108A RID: 4234
	public GameObject muzzleflashSideMG;

	// Token: 0x0400108B RID: 4235
	public GameObject[] muzzleflashRockets;

	// Token: 0x0400108C RID: 4236
	public GameObject spotLightHaloSawnpoint;

	// Token: 0x0400108D RID: 4237
	public GameObject[] muzzleflashSideguns;

	// Token: 0x0400108E RID: 4238
	[Header("MuzzleFlash Particle Systems")]
	public GameObjectRef machineGunMuzzleFlashFX;

	// Token: 0x0400108F RID: 4239
	public GameObjectRef mainCannonFireFX;

	// Token: 0x04001090 RID: 4240
	public GameObjectRef rocketLaunchFX;

	// Token: 0x04001091 RID: 4241
	[Header("Misc")]
	public bool rocketsOpen;

	// Token: 0x04001092 RID: 4242
	public Vector3[] vecSideGunRotation;

	// Token: 0x04001093 RID: 4243
	public float treadConstant = 0.14f;

	// Token: 0x04001094 RID: 4244
	public float wheelSpinConstant = 80f;

	// Token: 0x04001095 RID: 4245
	[Header("Gun Movement speeds")]
	public float sidegunsTurnSpeed = 30f;

	// Token: 0x04001096 RID: 4246
	public float turretTurnSpeed = 6f;

	// Token: 0x04001097 RID: 4247
	public float cannonPitchSpeed = 10f;

	// Token: 0x04001098 RID: 4248
	public float rocketPitchSpeed = 20f;

	// Token: 0x04001099 RID: 4249
	public float spotLightTurnSpeed = 60f;

	// Token: 0x0400109A RID: 4250
	public float machineGunSpeed = 20f;

	// Token: 0x0400109B RID: 4251
	private float wheelAngle;

	// Token: 0x060013E7 RID: 5095 RVA: 0x0007C7B4 File Offset: 0x0007A9B4
	private void Start()
	{
		this.mainRigidbody = base.GetComponent<Rigidbody>();
		for (int i = 0; i < this.ShocksBones.Length; i++)
		{
			this.vecShocksOffsetPosition[i] = this.ShocksBones[i].localPosition;
		}
	}

	// Token: 0x060013E8 RID: 5096 RVA: 0x00010F73 File Offset: 0x0000F173
	private void Update()
	{
		this.TrackTurret();
		this.TrackSpotLight();
		this.TrackSideGuns();
		this.AnimateWheelsTreads();
		this.AdjustShocksHeight();
		this.m2Animator.SetBool("rocketpods", this.rocketsOpen);
	}

	// Token: 0x060013E9 RID: 5097 RVA: 0x0007C7FC File Offset: 0x0007A9FC
	private void AnimateWheelsTreads()
	{
		float num = 0f;
		if (this.mainRigidbody != null)
		{
			num = Vector3.Dot(this.mainRigidbody.velocity, base.transform.forward);
		}
		float x = Time.time * -1f * num * this.treadConstant % 1f;
		this.treadLeftMaterial.SetTextureOffset("_MainTex", new Vector2(x, 0f));
		this.treadLeftMaterial.SetTextureOffset("_BumpMap", new Vector2(x, 0f));
		this.treadLeftMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(x, 0f));
		this.treadRightMaterial.SetTextureOffset("_MainTex", new Vector2(x, 0f));
		this.treadRightMaterial.SetTextureOffset("_BumpMap", new Vector2(x, 0f));
		this.treadRightMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(x, 0f));
		if (num >= 0f)
		{
			this.wheelAngle = (this.wheelAngle + Time.deltaTime * num * this.wheelSpinConstant) % 360f;
		}
		else
		{
			this.wheelAngle += Time.deltaTime * num * this.wheelSpinConstant;
			if (this.wheelAngle <= 0f)
			{
				this.wheelAngle = 360f;
			}
		}
		this.m2Animator.SetFloat("wheel_spin", this.wheelAngle);
		this.m2Animator.SetFloat("speed", num);
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x0007C97C File Offset: 0x0007AB7C
	private void AdjustShocksHeight()
	{
		Ray ray = default(Ray);
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction"
		});
		int num = this.ShocksBones.Length;
		float num2 = 0.55f;
		float num3 = 0.79f;
		for (int i = 0; i < num; i++)
		{
			ray.origin = this.ShockTraceLineBegin[i].position;
			ray.direction = base.transform.up * -1f;
			RaycastHit raycastHit;
			float num4;
			if (Physics.SphereCast(ray, 0.15f, ref raycastHit, num3, mask))
			{
				num4 = raycastHit.distance - num2;
			}
			else
			{
				num4 = 0.26f;
			}
			this.vecShocksOffsetPosition[i].y = Mathf.Lerp(this.vecShocksOffsetPosition[i].y, Mathf.Clamp(num4 * -1f, -0.26f, 0f), Time.deltaTime * 5f);
			this.ShocksBones[i].localPosition = this.vecShocksOffsetPosition[i];
		}
	}

	// Token: 0x060013EB RID: 5099 RVA: 0x0007CAA8 File Offset: 0x0007ACA8
	private void TrackTurret()
	{
		if (this.targetTurret != null)
		{
			Vector3 normalized = (this.targetTurret.position - this.turret.position).normalized;
			float num;
			float num2;
			this.CalculateYawPitchOffset(this.turret, this.turret.position, this.targetTurret.position, out num, out num2);
			num = this.NormalizeYaw(num);
			float num3 = Time.deltaTime * this.turretTurnSpeed;
			if (num < -0.5f)
			{
				this.vecTurret.y = (this.vecTurret.y - num3) % 360f;
			}
			else if (num > 0.5f)
			{
				this.vecTurret.y = (this.vecTurret.y + num3) % 360f;
			}
			this.turret.localEulerAngles = this.vecTurret;
			float num4 = Time.deltaTime * this.cannonPitchSpeed;
			this.CalculateYawPitchOffset(this.mainCannon, this.mainCannon.position, this.targetTurret.position, out num, out num2);
			if (num2 < -0.5f)
			{
				this.vecMainCannon.x = this.vecMainCannon.x - num4;
			}
			else if (num2 > 0.5f)
			{
				this.vecMainCannon.x = this.vecMainCannon.x + num4;
			}
			this.vecMainCannon.x = Mathf.Clamp(this.vecMainCannon.x, -55f, 5f);
			this.mainCannon.localEulerAngles = this.vecMainCannon;
			if (num2 < -0.5f)
			{
				this.vecCoaxGun.x = this.vecCoaxGun.x - num4;
			}
			else if (num2 > 0.5f)
			{
				this.vecCoaxGun.x = this.vecCoaxGun.x + num4;
			}
			this.vecCoaxGun.x = Mathf.Clamp(this.vecCoaxGun.x, -65f, 15f);
			this.coaxGun.localEulerAngles = this.vecCoaxGun;
			if (this.rocketsOpen)
			{
				num4 = Time.deltaTime * this.rocketPitchSpeed;
				this.CalculateYawPitchOffset(this.rocketsPitch, this.rocketsPitch.position, this.targetTurret.position, out num, out num2);
				if (num2 < -0.5f)
				{
					this.vecRocketsPitch.x = this.vecRocketsPitch.x - num4;
				}
				else if (num2 > 0.5f)
				{
					this.vecRocketsPitch.x = this.vecRocketsPitch.x + num4;
				}
				this.vecRocketsPitch.x = Mathf.Clamp(this.vecRocketsPitch.x, -45f, 45f);
			}
			else
			{
				this.vecRocketsPitch.x = Mathf.Lerp(this.vecRocketsPitch.x, 0f, Time.deltaTime * 1.7f);
			}
			this.rocketsPitch.localEulerAngles = this.vecRocketsPitch;
		}
	}

	// Token: 0x060013EC RID: 5100 RVA: 0x0007CD7C File Offset: 0x0007AF7C
	private void TrackSpotLight()
	{
		if (this.targetSpotLight != null)
		{
			Vector3 normalized = (this.targetSpotLight.position - this.spotLightYaw.position).normalized;
			float num;
			float num2;
			this.CalculateYawPitchOffset(this.spotLightYaw, this.spotLightYaw.position, this.targetSpotLight.position, out num, out num2);
			num = this.NormalizeYaw(num);
			float num3 = Time.deltaTime * this.spotLightTurnSpeed;
			if (num < -0.5f)
			{
				this.vecSpotLightBase.y = (this.vecSpotLightBase.y - num3) % 360f;
			}
			else if (num > 0.5f)
			{
				this.vecSpotLightBase.y = (this.vecSpotLightBase.y + num3) % 360f;
			}
			this.spotLightYaw.localEulerAngles = this.vecSpotLightBase;
			this.CalculateYawPitchOffset(this.spotLightPitch, this.spotLightPitch.position, this.targetSpotLight.position, out num, out num2);
			if (num2 < -0.5f)
			{
				this.vecSpotLight.x = this.vecSpotLight.x - num3;
			}
			else if (num2 > 0.5f)
			{
				this.vecSpotLight.x = this.vecSpotLight.x + num3;
			}
			this.vecSpotLight.x = Mathf.Clamp(this.vecSpotLight.x, -50f, 50f);
			this.spotLightPitch.localEulerAngles = this.vecSpotLight;
			this.m2Animator.SetFloat("sideMG_pitch", this.vecSpotLight.x, 0.5f, Time.deltaTime);
		}
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x0007CF0C File Offset: 0x0007B10C
	private void TrackSideGuns()
	{
		for (int i = 0; i < this.sideguns.Length; i++)
		{
			if (!(this.targetSideguns[i] == null))
			{
				Vector3 normalized = (this.targetSideguns[i].position - this.sideguns[i].position).normalized;
				float num;
				float num2;
				this.CalculateYawPitchOffset(this.sideguns[i], this.sideguns[i].position, this.targetSideguns[i].position, out num, out num2);
				num = this.NormalizeYaw(num);
				float num3 = Time.deltaTime * this.sidegunsTurnSpeed;
				if (num < -0.5f)
				{
					Vector3[] array = this.vecSideGunRotation;
					int num4 = i;
					array[num4].y = array[num4].y - num3;
				}
				else if (num > 0.5f)
				{
					Vector3[] array2 = this.vecSideGunRotation;
					int num5 = i;
					array2[num5].y = array2[num5].y + num3;
				}
				if (num2 < -0.5f)
				{
					Vector3[] array3 = this.vecSideGunRotation;
					int num6 = i;
					array3[num6].x = array3[num6].x - num3;
				}
				else if (num2 > 0.5f)
				{
					Vector3[] array4 = this.vecSideGunRotation;
					int num7 = i;
					array4[num7].x = array4[num7].x + num3;
				}
				this.vecSideGunRotation[i].x = Mathf.Clamp(this.vecSideGunRotation[i].x, -45f, 45f);
				this.vecSideGunRotation[i].y = Mathf.Clamp(this.vecSideGunRotation[i].y, -45f, 45f);
				this.sideguns[i].localEulerAngles = this.vecSideGunRotation[i];
			}
		}
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x0007D0A8 File Offset: 0x0007B2A8
	public void CalculateYawPitchOffset(Transform objectTransform, Vector3 vecStart, Vector3 vecEnd, out float yaw, out float pitch)
	{
		Vector3 vector = objectTransform.InverseTransformDirection(vecEnd - vecStart);
		float x = Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
		pitch = -Mathf.Atan2(vector.y, x) * 57.295776f;
		vector = (vecEnd - vecStart).normalized;
		Vector3 forward = objectTransform.forward;
		forward.y = 0f;
		forward.Normalize();
		float num = Vector3.Dot(vector, forward);
		float num2 = Vector3.Dot(vector, objectTransform.right);
		float y = 360f * num2;
		float x2 = 360f * -num;
		yaw = (Mathf.Atan2(y, x2) + 3.1415927f) * 57.295776f;
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x0007D168 File Offset: 0x0007B368
	public float NormalizeYaw(float flYaw)
	{
		float result;
		if (flYaw > 180f)
		{
			result = 360f - flYaw;
		}
		else
		{
			result = flYaw * -1f;
		}
		return result;
	}
}
