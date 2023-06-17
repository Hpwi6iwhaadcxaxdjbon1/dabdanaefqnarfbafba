using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class CargoShipSounds : MonoBehaviour, IClientComponent
{
	// Token: 0x04000040 RID: 64
	public SoundDefinition waveSoundDef;

	// Token: 0x04000041 RID: 65
	public AnimationCurve waveSoundYGainCurve;

	// Token: 0x04000042 RID: 66
	public AnimationCurve waveSoundEdgeDistanceGainCurve;

	// Token: 0x04000043 RID: 67
	private Sound waveSoundL;

	// Token: 0x04000044 RID: 68
	private Sound waveSoundR;

	// Token: 0x04000045 RID: 69
	private SoundModulation.Modulator waveSoundLGainMod;

	// Token: 0x04000046 RID: 70
	private SoundModulation.Modulator waveSoundRGainMod;

	// Token: 0x04000047 RID: 71
	public SoundDefinition sternWakeSoundDef;

	// Token: 0x04000048 RID: 72
	private Sound sternWakeSound;

	// Token: 0x04000049 RID: 73
	private SoundModulation.Modulator sternWakeSoundGainMod;

	// Token: 0x0400004A RID: 74
	public SoundDefinition engineHumSoundDef;

	// Token: 0x0400004B RID: 75
	private Sound engineHumSound;

	// Token: 0x0400004C RID: 76
	public GameObject engineHumTarget;

	// Token: 0x0400004D RID: 77
	public SoundDefinition hugeRumbleSoundDef;

	// Token: 0x0400004E RID: 78
	public AnimationCurve hugeRumbleYDiffCurve;

	// Token: 0x0400004F RID: 79
	public AnimationCurve hugeRumbleRelativeSpeedCurve;

	// Token: 0x04000050 RID: 80
	private Sound hugeRumbleSound;

	// Token: 0x04000051 RID: 81
	private SoundModulation.Modulator hugeRumbleGainMod;

	// Token: 0x04000052 RID: 82
	private Vector3 lastCameraPos;

	// Token: 0x04000053 RID: 83
	private Vector3 lastRumblePos;

	// Token: 0x04000054 RID: 84
	private Vector3 lastRumbleLocalPos;

	// Token: 0x04000055 RID: 85
	public Collider soundFollowCollider;

	// Token: 0x04000056 RID: 86
	public Collider soundFollowColliderL;

	// Token: 0x04000057 RID: 87
	public Collider soundFollowColliderR;

	// Token: 0x04000058 RID: 88
	public Collider sternSoundFollowCollider;

	// Token: 0x04000059 RID: 89
	private HashSet<CargoShipInteriorSoundTrigger> interiorSoundTriggers = new HashSet<CargoShipInteriorSoundTrigger>();

	// Token: 0x06000048 RID: 72 RVA: 0x00027A38 File Offset: 0x00025C38
	public void UpdateSounds()
	{
		Vector3 position = this.soundFollowCollider.ClosestPoint(MainCamera.position);
		Vector3 position2 = this.soundFollowColliderL.ClosestPoint(MainCamera.position);
		Vector3 position3 = this.soundFollowColliderR.ClosestPoint(MainCamera.position);
		Vector3 position4 = this.sternSoundFollowCollider.ClosestPoint(MainCamera.position);
		Vector3 vector = base.transform.InverseTransformPoint(MainCamera.position);
		float num = 10f;
		float num2 = 60f;
		float num3 = 8f;
		float b = num2 - Mathf.Abs(vector.z - num3);
		float num4 = this.waveSoundYGainCurve.Evaluate(vector.y);
		if (this.waveSoundL != null)
		{
			this.waveSoundL.transform.position = position2;
			float time = Mathf.Min(vector.x + num, b);
			float num5 = this.waveSoundEdgeDistanceGainCurve.Evaluate(time);
			float target = 1f * num5 * num4;
			if (this.interiorSoundTriggers.Count > 0)
			{
				target = 0f;
			}
			this.waveSoundLGainMod.value = Mathf.MoveTowards(this.waveSoundLGainMod.value, target, Time.deltaTime);
		}
		if (this.waveSoundR != null)
		{
			this.waveSoundR.transform.position = position3;
			float time2 = Mathf.Min(num - vector.x, b);
			float num6 = this.waveSoundEdgeDistanceGainCurve.Evaluate(time2);
			float target2 = 1f * num6 * num4;
			if (this.interiorSoundTriggers.Count > 0)
			{
				target2 = 0f;
			}
			this.waveSoundRGainMod.value = Mathf.MoveTowards(this.waveSoundRGainMod.value, target2, Time.deltaTime);
		}
		if (this.hugeRumbleSound != null && this.hugeRumbleGainMod != null)
		{
			this.hugeRumbleSound.transform.position = position;
			Vector3 a = (this.hugeRumbleSound.transform.position - this.lastRumblePos) / Time.deltaTime;
			Vector3 b2 = (this.lastRumbleLocalPos - this.hugeRumbleSound.transform.localPosition) / Time.deltaTime;
			Vector3 a2 = a + b2;
			Vector3 b3 = (MainCamera.position - this.lastCameraPos) / Time.deltaTime;
			float magnitude = (a2 - b3).magnitude;
			float target3 = Mathf.Clamp01(1f * this.hugeRumbleYDiffCurve.Evaluate(vector.y) * this.hugeRumbleRelativeSpeedCurve.Evaluate(magnitude));
			if (LocalPlayer.Entity != null && LocalPlayer.Entity.gameObject != null && LocalPlayer.Entity.gameObject.transform.parent != null)
			{
				target3 = 0f;
			}
			this.hugeRumbleGainMod.value = Mathf.MoveTowards(this.hugeRumbleGainMod.value, target3, Time.deltaTime * 0.5f);
			this.lastRumblePos = this.hugeRumbleSound.transform.position;
			this.lastRumbleLocalPos = this.hugeRumbleSound.transform.localPosition;
		}
		if (this.sternWakeSound != null)
		{
			this.sternWakeSound.transform.position = position4;
			float time3 = vector.z + (num2 - num3);
			float num7 = this.waveSoundEdgeDistanceGainCurve.Evaluate(time3);
			float target4 = 1f * num7 * num4;
			if (this.interiorSoundTriggers.Count > 0)
			{
				target4 = 0f;
			}
			this.sternWakeSoundGainMod.value = Mathf.MoveTowards(this.sternWakeSoundGainMod.value, target4, Time.deltaTime);
		}
		this.lastCameraPos = MainCamera.position;
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00027DE0 File Offset: 0x00025FE0
	public void InitSounds()
	{
		this.waveSoundL = SoundManager.RequestSoundInstance(this.waveSoundDef, null, default(Vector3), false);
		this.waveSoundR = SoundManager.RequestSoundInstance(this.waveSoundDef, null, default(Vector3), false);
		this.sternWakeSound = SoundManager.RequestSoundInstance(this.sternWakeSoundDef, null, default(Vector3), false);
		this.hugeRumbleSound = SoundManager.RequestSoundInstance(this.hugeRumbleSoundDef, base.gameObject, default(Vector3), false);
		this.engineHumSound = SoundManager.RequestSoundInstance(this.engineHumSoundDef, this.engineHumTarget, default(Vector3), false);
		if (this.waveSoundL != null)
		{
			this.waveSoundL.Play();
			this.waveSoundLGainMod = this.waveSoundL.modulation.CreateModulator(SoundModulation.Parameter.Gain);
		}
		if (this.waveSoundR != null)
		{
			this.waveSoundR.Play();
			this.waveSoundRGainMod = this.waveSoundR.modulation.CreateModulator(SoundModulation.Parameter.Gain);
		}
		if (this.sternWakeSound != null)
		{
			this.sternWakeSound.Play();
			this.sternWakeSoundGainMod = this.sternWakeSound.modulation.CreateModulator(SoundModulation.Parameter.Gain);
		}
		if (this.hugeRumbleSound != null)
		{
			this.hugeRumbleSound.Play();
			this.hugeRumbleGainMod = this.hugeRumbleSound.modulation.CreateModulator(SoundModulation.Parameter.Gain);
		}
		if (this.engineHumSound != null)
		{
			this.engineHumSound.Play();
		}
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00002F90 File Offset: 0x00001190
	public void InteriorTriggerEntered(CargoShipInteriorSoundTrigger trigger)
	{
		this.interiorSoundTriggers.Add(trigger);
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00002F9F File Offset: 0x0000119F
	public void InteriorTriggerExited(CargoShipInteriorSoundTrigger trigger)
	{
		this.interiorSoundTriggers.Remove(trigger);
	}
}
