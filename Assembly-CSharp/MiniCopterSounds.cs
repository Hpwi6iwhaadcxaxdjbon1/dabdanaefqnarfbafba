using System;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class MiniCopterSounds : MonoBehaviour, IClientComponent
{
	// Token: 0x0400007D RID: 125
	public MiniCopter miniCopter;

	// Token: 0x0400007E RID: 126
	public GameObject soundAttachPoint;

	// Token: 0x0400007F RID: 127
	public SoundDefinition engineStartDef;

	// Token: 0x04000080 RID: 128
	public SoundDefinition engineLoopDef;

	// Token: 0x04000081 RID: 129
	public SoundDefinition engineStopDef;

	// Token: 0x04000082 RID: 130
	public SoundDefinition rotorLoopDef;

	// Token: 0x04000083 RID: 131
	public float engineStartFadeOutTime = 1f;

	// Token: 0x04000084 RID: 132
	public float engineLoopFadeInTime = 0.7f;

	// Token: 0x04000085 RID: 133
	public float engineLoopFadeOutTime = 0.25f;

	// Token: 0x04000086 RID: 134
	public float engineStopFadeOutTime = 0.25f;

	// Token: 0x04000087 RID: 135
	public float rotorLoopFadeInTime = 0.7f;

	// Token: 0x04000088 RID: 136
	public float rotorLoopFadeOutTime = 0.25f;

	// Token: 0x04000089 RID: 137
	public float enginePitchInterpRate = 0.5f;

	// Token: 0x0400008A RID: 138
	public float rotorPitchInterpRate = 1f;

	// Token: 0x0400008B RID: 139
	public float rotorGainInterpRate = 0.5f;

	// Token: 0x0400008C RID: 140
	public float rotorStartStopPitchRateUp = 7f;

	// Token: 0x0400008D RID: 141
	public float rotorStartStopPitchRateDown = 9f;

	// Token: 0x0400008E RID: 142
	public float rotorStartStopGainRateUp = 5f;

	// Token: 0x0400008F RID: 143
	public float rotorStartStopGainRateDown = 4f;

	// Token: 0x04000090 RID: 144
	public AnimationCurve engineUpDotPitchCurve;

	// Token: 0x04000091 RID: 145
	public AnimationCurve rotorUpDotPitchCurve;

	// Token: 0x04000092 RID: 146
	private float rotorStartStopPitchMult = 0.25f;

	// Token: 0x04000093 RID: 147
	private float rotorStartStopGainMult;

	// Token: 0x04000094 RID: 148
	private Sound engineStart;

	// Token: 0x04000095 RID: 149
	private Sound engineStop;

	// Token: 0x04000096 RID: 150
	private Sound engineLoop;

	// Token: 0x04000097 RID: 151
	private SoundModulation.Modulator engineGain;

	// Token: 0x04000098 RID: 152
	private SoundModulation.Modulator enginePitch;

	// Token: 0x04000099 RID: 153
	private Sound rotorLoop;

	// Token: 0x0400009A RID: 154
	private SoundModulation.Modulator rotorGain;

	// Token: 0x0400009B RID: 155
	private SoundModulation.Modulator rotorPitch;

	// Token: 0x0400009C RID: 156
	private bool isOn;

	// Token: 0x0400009D RID: 157
	private bool wasOn;

	// Token: 0x0400009E RID: 158
	private bool isStartingUp;

	// Token: 0x0400009F RID: 159
	private bool wasStartingUp;

	// Token: 0x06000060 RID: 96 RVA: 0x00028318 File Offset: 0x00026518
	private void Update()
	{
		this.isOn = this.miniCopter.IsOn();
		this.isStartingUp = this.miniCopter.IsStartingUp();
		if (this.isStartingUp && this.engineStart == null)
		{
			this.engineStart = SoundManager.PlayOneshot(this.engineStartDef, this.soundAttachPoint, false, default(Vector3));
		}
		if (this.engineStart != null && !this.isStartingUp)
		{
			this.engineStart.FadeOutAndRecycle(this.engineStartFadeOutTime);
			this.engineStart = null;
		}
		if (this.isOn && this.engineLoop == null)
		{
			this.engineLoop = SoundManager.RequestSoundInstance(this.engineLoopDef, this.soundAttachPoint, default(Vector3), false);
			this.engineLoop.FadeInAndPlay(this.engineLoopFadeInTime);
			if (this.engineLoop != null)
			{
				this.engineGain = this.engineLoop.modulation.CreateModulator(SoundModulation.Parameter.Gain);
				this.enginePitch = this.engineLoop.modulation.CreateModulator(SoundModulation.Parameter.Pitch);
			}
		}
		if (!this.isOn && this.engineLoop != null)
		{
			this.engineLoop.FadeOutAndRecycle(this.engineLoopFadeOutTime);
			this.engineLoop = null;
			this.engineGain = null;
			this.enginePitch = null;
		}
		if (!this.isOn && this.wasOn && this.engineStop == null)
		{
			this.engineStop = SoundManager.PlayOneshot(this.engineStopDef, this.soundAttachPoint, false, default(Vector3));
		}
		if (this.engineStop != null && (this.isOn || !this.engineStop.isAudioSourcePlaying))
		{
			if (this.engineStop.isAudioSourcePlaying)
			{
				this.engineStop.FadeOutAndRecycle(this.engineStopFadeOutTime);
			}
			this.engineStop = null;
		}
		if ((this.isOn || this.isStartingUp) && this.rotorLoop == null)
		{
			this.rotorLoop = SoundManager.RequestSoundInstance(this.rotorLoopDef, this.soundAttachPoint, default(Vector3), false);
			this.rotorLoop.FadeInAndPlay(this.rotorLoopFadeInTime);
			if (this.rotorLoop != null)
			{
				this.rotorGain = this.rotorLoop.modulation.CreateModulator(SoundModulation.Parameter.Gain);
				this.rotorGain.value = 0f;
				this.rotorPitch = this.rotorLoop.modulation.CreateModulator(SoundModulation.Parameter.Pitch);
				this.rotorPitch.value = 0.25f;
			}
		}
		float time = Vector3.Dot(base.transform.up, Vector3.up);
		if (this.engineLoop != null && this.enginePitch != null)
		{
			float num = 1f;
			num *= this.engineUpDotPitchCurve.Evaluate(time);
			this.enginePitch.value = Mathf.MoveTowards(this.enginePitch.value, num, Time.deltaTime / this.enginePitchInterpRate);
		}
		if (this.rotorLoop != null && this.rotorPitch != null)
		{
			float num2 = 1f;
			float num3 = 1f;
			num2 *= this.rotorUpDotPitchCurve.Evaluate(time);
			float num4 = (this.isOn || this.isStartingUp) ? 1f : 0.25f;
			float num5 = (num4 > this.rotorStartStopPitchMult) ? this.rotorStartStopPitchRateUp : this.rotorStartStopPitchRateDown;
			this.rotorStartStopPitchMult = Mathf.MoveTowards(this.rotorStartStopPitchMult, num4, Time.deltaTime / num5);
			float num6 = (this.isOn || this.isStartingUp) ? 1f : 0f;
			float num7 = (num6 > this.rotorStartStopGainMult) ? this.rotorStartStopGainRateUp : this.rotorStartStopGainRateDown;
			this.rotorStartStopGainMult = Mathf.MoveTowards(this.rotorStartStopGainMult, num6, Time.deltaTime / num7);
			num2 *= this.rotorStartStopPitchMult;
			this.rotorPitch.value = Mathf.MoveTowards(this.rotorPitch.value, num2, Time.deltaTime / this.rotorPitchInterpRate);
			num3 *= this.rotorStartStopGainMult;
			this.rotorGain.value = Mathf.MoveTowards(this.rotorGain.value, num3, Time.deltaTime / this.rotorGainInterpRate);
			if (!this.isStartingUp && !this.isOn && this.rotorLoop != null && this.rotorLoop.audioSources[0].volume <= 0.01f)
			{
				this.rotorLoop.StopAndRecycle(0f);
				this.rotorLoop = null;
				this.rotorGain = null;
				this.rotorPitch = null;
			}
		}
		this.wasOn = this.isOn;
		this.wasStartingUp = this.isStartingUp;
	}
}
