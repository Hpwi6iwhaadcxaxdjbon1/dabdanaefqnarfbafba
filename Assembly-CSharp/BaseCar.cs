using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000330 RID: 816
public class BaseCar : BaseWheeledVehicle
{
	// Token: 0x04001280 RID: 4736
	public float brakePedal;

	// Token: 0x04001281 RID: 4737
	public float gasPedal;

	// Token: 0x04001282 RID: 4738
	public float steering;

	// Token: 0x04001283 RID: 4739
	public Transform centerOfMass;

	// Token: 0x04001284 RID: 4740
	public Transform steeringWheel;

	// Token: 0x04001285 RID: 4741
	public float motorForceConstant = 150f;

	// Token: 0x04001286 RID: 4742
	public float brakeForceConstant = 500f;

	// Token: 0x04001287 RID: 4743
	public float GasLerpTime = 20f;

	// Token: 0x04001288 RID: 4744
	public float SteeringLerpTime = 20f;

	// Token: 0x04001289 RID: 4745
	public Transform driverEye;

	// Token: 0x0400128A RID: 4746
	public Rigidbody myRigidBody;

	// Token: 0x0400128B RID: 4747
	private SedanWheelSmoke wheelSmoke;

	// Token: 0x0400128C RID: 4748
	private float shockRestingPosY = -0.27f;

	// Token: 0x0400128D RID: 4749
	private float shockDistance = 0.3f;

	// Token: 0x0400128E RID: 4750
	private float traceDistanceNeutralPoint = 0.7f;

	// Token: 0x0400128F RID: 4751
	private int cachedMask = -1;

	// Token: 0x04001290 RID: 4752
	private const float wheelSpinConstant = 120f;

	// Token: 0x04001291 RID: 4753
	private AverageVelocity averageVelocity = new AverageVelocity();

	// Token: 0x04001292 RID: 4754
	public SoundPlayer idleLoopPlayer;

	// Token: 0x04001293 RID: 4755
	public Transform engineOffset;

	// Token: 0x04001294 RID: 4756
	public SoundDefinition engineSoundDef;

	// Token: 0x04001295 RID: 4757
	private Sound engineSoundLoop;

	// Token: 0x04001296 RID: 4758
	private SoundModulation.Modulator engineSoundVolume;

	// Token: 0x04001297 RID: 4759
	private SoundModulation.Modulator engineSoundPitch;

	// Token: 0x060015A2 RID: 5538 RVA: 0x000124B5 File Offset: 0x000106B5
	public override float MaxVelocity()
	{
		return 50f;
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x000124BC File Offset: 0x000106BC
	public override Vector3 EyePositionForPlayer(BasePlayer player)
	{
		if (player.GetMounted() == this)
		{
			return this.driverEye.transform.position;
		}
		return Vector3.zero;
	}

	// Token: 0x060015A4 RID: 5540 RVA: 0x000124E2 File Offset: 0x000106E2
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		this.ShutdownClientsideEffects();
	}

	// Token: 0x060015A5 RID: 5541 RVA: 0x000124F0 File Offset: 0x000106F0
	public override void SetNetworkPosition(Vector3 vPos)
	{
		base.SetNetworkPosition(vPos);
		this.averageVelocity.Record(vPos);
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x00012505 File Offset: 0x00010705
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.InitializeClientsideEffects();
		this.wheelSmoke.InitWheelSmoke();
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x0001251F File Offset: 0x0001071F
	public void Update()
	{
		if (base.isServer)
		{
			return;
		}
		this.UpdateTireAnimation();
		this.UpdateSounds();
		this.wheelSmoke.UpdateWheelSmoke();
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x00084BB0 File Offset: 0x00082DB0
	private float GetShockHeightDelta(WheelCollider wheel, int i)
	{
		if (this.cachedMask == -1)
		{
			this.cachedMask = LayerMask.GetMask(new string[]
			{
				"Terrain",
				"World",
				"Construction",
				"Debris",
				"Clutter",
				"Deployed",
				"Tree",
				"Default"
			});
		}
		float num = 0.1f;
		RaycastHit raycastHit;
		if (!Physics.SphereCast(new Ray(wheel.transform.position + base.transform.up * num, -base.transform.up), num, ref raycastHit, 2f, this.cachedMask))
		{
			this.wheelSmoke.wheelTouching[i] = false;
			raycastHit.distance = 2f;
		}
		else
		{
			this.wheelSmoke.wheelTouching[i] = true;
		}
		return Mathx.RemapValClamped(raycastHit.distance, this.traceDistanceNeutralPoint - this.shockDistance, this.traceDistanceNeutralPoint + this.shockDistance, this.shockDistance * 0.75f, -0.75f * this.shockDistance);
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x00084CD0 File Offset: 0x00082ED0
	private float ClientSteering()
	{
		float num = 0f;
		if (base.HasFlag(BaseEntity.Flags.Reserved5))
		{
			num += 1f;
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved4))
		{
			num -= 1f;
		}
		return num;
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x00005EBB File Offset: 0x000040BB
	public bool IsBreaking()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved3);
	}

	// Token: 0x060015AB RID: 5547 RVA: 0x00084D10 File Offset: 0x00082F10
	private void UpdateTireAnimation()
	{
		float num = this.averageVelocity.Speed * Vector3.Dot(this.averageVelocity.Average.normalized, base.transform.forward);
		this.steering = Mathf.Lerp(this.steering, this.ClientSteering(), Time.deltaTime * 2f);
		for (int i = 0; i < this.wheels.Length; i++)
		{
			BaseWheeledVehicle.VehicleWheel vehicleWheel = this.wheels[i];
			vehicleWheel.shock.localPosition = new Vector3(vehicleWheel.shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(vehicleWheel.wheelCollider, i), vehicleWheel.shock.localPosition.z);
			if (!this.IsBreaking())
			{
				vehicleWheel.wheel.localEulerAngles = new Vector3(vehicleWheel.wheel.localEulerAngles.x, vehicleWheel.wheel.localEulerAngles.y, vehicleWheel.wheel.localEulerAngles.z + num * Time.deltaTime * 120f);
			}
			if (vehicleWheel.steerWheel)
			{
				vehicleWheel.axle.localEulerAngles = new Vector3(this.steering * 45f, vehicleWheel.axle.localEulerAngles.y, vehicleWheel.axle.localEulerAngles.z);
			}
		}
		this.steeringWheel.localEulerAngles = new Vector3(this.steeringWheel.localEulerAngles.x, this.steering * 180f, this.steeringWheel.localEulerAngles.z);
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x00084EAC File Offset: 0x000830AC
	public void InitializeClientsideEffects()
	{
		if (this.engineSoundLoop == null)
		{
			this.engineSoundLoop = SoundManager.RequestSoundInstance(this.engineSoundDef, this.engineOffset.gameObject, default(Vector3), false);
			this.engineSoundVolume = this.engineSoundLoop.modulation.CreateModulator(SoundModulation.Parameter.Gain);
			this.engineSoundPitch = this.engineSoundLoop.modulation.CreateModulator(SoundModulation.Parameter.Pitch);
			this.engineSoundVolume.value = 0f;
		}
		this.wheelSmoke = base.GetComponent<SedanWheelSmoke>();
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x00012541 File Offset: 0x00010741
	public void ShutdownClientsideEffects()
	{
		if (this.engineSoundLoop != null)
		{
			this.engineSoundLoop.FadeOutAndRecycle(0.1f);
			this.engineSoundVolume = null;
			this.engineSoundPitch = null;
			this.engineSoundLoop = null;
		}
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x00004723 File Offset: 0x00002923
	public bool IsEngineOn()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x00084F38 File Offset: 0x00083138
	public void UpdateSounds()
	{
		if (this.engineSoundLoop == null)
		{
			return;
		}
		if (!this.IsEngineOn())
		{
			if (this.engineSoundLoop.isAudioSourcePlaying)
			{
				this.engineSoundLoop.Stop();
			}
			if (this.idleLoopPlayer.IsPlaying())
			{
				this.idleLoopPlayer.FadeOutAndRecycle(0.3f);
			}
			return;
		}
		if (!this.idleLoopPlayer.IsPlaying())
		{
			this.idleLoopPlayer.FadeInAndPlay(0.3f);
		}
		float value = Mathf.Abs(this.averageVelocity.Speed);
		float num = Mathf.InverseLerp(0f, 40f, value);
		float num2 = Mathf.InverseLerp(0f, 0.4f, num);
		this.engineSoundPitch.value = Mathf.Lerp(this.engineSoundPitch.value, 0.5f + 0.75f * num, Time.deltaTime * 4f);
		this.engineSoundVolume.value = Mathf.Lerp(this.engineSoundVolume.value, 0.2f + num2 * 0.8f, Time.deltaTime * 4f);
		if (this.engineSoundVolume.value > 0f && !this.engineSoundLoop.isAudioSourcePlaying)
		{
			this.engineSoundLoop.Play();
			return;
		}
		if (this.engineSoundVolume.value == 0f && this.engineSoundLoop.isAudioSourcePlaying)
		{
			this.engineSoundLoop.Stop();
		}
	}
}
