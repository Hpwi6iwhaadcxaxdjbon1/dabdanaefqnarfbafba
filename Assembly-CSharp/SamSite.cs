using System;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class SamSite : StorageContainer
{
	// Token: 0x040007F8 RID: 2040
	public Animator pitchAnimator;

	// Token: 0x040007F9 RID: 2041
	public GameObject yaw;

	// Token: 0x040007FA RID: 2042
	public GameObject pitch;

	// Token: 0x040007FB RID: 2043
	public GameObject gear;

	// Token: 0x040007FC RID: 2044
	public Transform eyePoint;

	// Token: 0x040007FD RID: 2045
	public float gearEpislonDegrees = 20f;

	// Token: 0x040007FE RID: 2046
	public float turnSpeed = 1f;

	// Token: 0x040007FF RID: 2047
	public float clientLerpSpeed = 1f;

	// Token: 0x04000800 RID: 2048
	public Vector3 currentAimDir = Vector3.forward;

	// Token: 0x04000801 RID: 2049
	public Vector3 targetAimDir = Vector3.forward;

	// Token: 0x04000802 RID: 2050
	public BaseCombatEntity currentTarget;

	// Token: 0x04000803 RID: 2051
	public float scanRadius = 350f;

	// Token: 0x04000804 RID: 2052
	public GameObjectRef projectileTest;

	// Token: 0x04000805 RID: 2053
	public GameObjectRef muzzleFlashTest;

	// Token: 0x04000806 RID: 2054
	public bool staticRespawn;

	// Token: 0x04000807 RID: 2055
	public ItemDefinition ammoType;

	// Token: 0x04000808 RID: 2056
	[ServerVar(Help = "targetmode, 1 = all air vehicles, 0 = only hot air ballons")]
	public static bool alltarget = false;

	// Token: 0x04000809 RID: 2057
	[ServerVar(Help = "how long until static sam sites auto repair")]
	public static float staticrepairseconds = 1200f;

	// Token: 0x0400080A RID: 2058
	public SoundDefinition yawMovementLoopDef;

	// Token: 0x0400080B RID: 2059
	public float yawGainLerp = 8f;

	// Token: 0x0400080C RID: 2060
	public float yawGainMovementSpeedMult = 0.1f;

	// Token: 0x0400080D RID: 2061
	public SoundDefinition pitchMovementLoopDef;

	// Token: 0x0400080E RID: 2062
	public float pitchGainLerp = 10f;

	// Token: 0x0400080F RID: 2063
	public float pitchGainMovementSpeedMult = 0.5f;

	// Token: 0x04000810 RID: 2064
	private Sound yawMovementLoop;

	// Token: 0x04000811 RID: 2065
	private Sound pitchMovementLoop;

	// Token: 0x04000812 RID: 2066
	private SoundModulation.Modulator yawGainModulator;

	// Token: 0x04000813 RID: 2067
	private SoundModulation.Modulator pitchGainModulator;

	// Token: 0x04000814 RID: 2068
	private float previousYawAngle;

	// Token: 0x04000815 RID: 2069
	private float previousPitchAngle;

	// Token: 0x06000B7F RID: 2943 RVA: 0x0000AFE3 File Offset: 0x000091E3
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (!info.fromDisk && base.isClient && info.msg.samSite != null)
		{
			this.UpdateClientTargetAimDir(info.msg.samSite.aimDir);
		}
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x0000B01F File Offset: 0x0000921F
	public void UpdateClientTargetAimDir(Vector3 aimDir)
	{
		this.targetAimDir = aimDir;
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x000593DC File Offset: 0x000575DC
	public void SetClientAim(Vector3 aimDir)
	{
		Vector3 vector = Quaternion.LookRotation(aimDir, base.transform.up).eulerAngles;
		vector = BaseMountable.ConvertVector(vector);
		float num = Mathf.InverseLerp(0f, 90f, -vector.x);
		this.pitchAnimator.SetFloat("pitch", num);
		Quaternion rotation = Quaternion.Euler(0f, vector.y, 0f);
		this.yaw.transform.rotation = rotation;
		Quaternion localRotation = Quaternion.Euler(vector.y * this.gearEpislonDegrees, 0f, 0f);
		this.gear.transform.localRotation = localRotation;
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x0000B028 File Offset: 0x00009228
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		this.currentAimDir = Vector3.Lerp(this.currentAimDir, this.targetAimDir, Time.deltaTime * this.clientLerpSpeed);
		this.SetClientAim(this.currentAimDir);
		this.UpdateSounds();
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x0005948C File Offset: 0x0005768C
	private void UpdateSounds()
	{
		float z = this.pitch.transform.eulerAngles.z;
		float y = this.yaw.transform.eulerAngles.y;
		float num = Mathf.Abs(z - this.previousPitchAngle) / Time.deltaTime;
		float num2 = Mathf.Abs(y - this.previousYawAngle) / Time.deltaTime;
		bool flag = num > 0.01f;
		bool flag2 = num2 > 0.01f;
		if (flag && this.pitchMovementLoop == null)
		{
			this.pitchMovementLoop = SoundManager.RequestSoundInstance(this.pitchMovementLoopDef, base.gameObject, default(Vector3), false);
			if (this.pitchMovementLoop)
			{
				this.pitchGainModulator = this.pitchMovementLoop.modulation.CreateModulator(SoundModulation.Parameter.Gain);
				this.pitchGainModulator.value = 0.002f;
				this.pitchMovementLoop.Play();
			}
		}
		if (flag2 && this.yawMovementLoop == null)
		{
			this.yawMovementLoop = SoundManager.RequestSoundInstance(this.yawMovementLoopDef, base.gameObject, default(Vector3), false);
			if (this.yawMovementLoop)
			{
				this.yawGainModulator = this.yawMovementLoop.modulation.CreateModulator(SoundModulation.Parameter.Gain);
				this.yawGainModulator.value = 0.002f;
				this.yawMovementLoop.Play();
			}
		}
		float b = flag ? Mathf.Lerp(0f, 1f, num * this.pitchGainMovementSpeedMult) : 0f;
		float b2 = flag2 ? Mathf.Lerp(0f, 1f, num2 * this.yawGainMovementSpeedMult) : 0f;
		if (this.pitchMovementLoop != null && this.pitchGainModulator != null)
		{
			this.pitchGainModulator.value = Mathf.Lerp(this.pitchGainModulator.value, b, Time.deltaTime * this.pitchGainLerp);
			if (this.pitchGainModulator.value < 0.001f && this.pitchMovementLoop.isAudioSourcePlaying)
			{
				this.pitchMovementLoop.StopAndRecycle(0f);
				this.pitchMovementLoop = null;
				this.pitchGainModulator = null;
			}
		}
		if (this.yawMovementLoop != null && this.yawGainModulator != null)
		{
			this.yawGainModulator.value = Mathf.Lerp(this.yawGainModulator.value, b2, Time.deltaTime * this.yawGainLerp);
			if (this.yawGainModulator.value < 0.001f && this.yawMovementLoop.isAudioSourcePlaying)
			{
				this.yawMovementLoop.StopAndRecycle(0f);
				this.yawMovementLoop = null;
				this.yawGainModulator = null;
			}
		}
		this.previousPitchAngle = z;
		this.previousYawAngle = y;
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x00059730 File Offset: 0x00057930
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		if (this.pitchMovementLoop)
		{
			this.pitchMovementLoop.StopAndRecycle(0f);
			this.pitchMovementLoop = null;
			this.pitchGainModulator = null;
		}
		if (this.yawMovementLoop)
		{
			this.yawMovementLoop.StopAndRecycle(0f);
			this.yawMovementLoop = null;
			this.yawGainModulator = null;
		}
	}
}
