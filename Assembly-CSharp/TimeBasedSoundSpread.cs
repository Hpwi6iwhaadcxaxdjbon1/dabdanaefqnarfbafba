using System;
using UnityEngine;

// Token: 0x02000197 RID: 407
public class TimeBasedSoundSpread : SoundModifier
{
	// Token: 0x04000B28 RID: 2856
	public AnimationCurve spreadCurve;

	// Token: 0x04000B29 RID: 2857
	public AnimationCurve wanderIntensityCurve;

	// Token: 0x04000B2A RID: 2858
	private float startTime;

	// Token: 0x04000B2B RID: 2859
	private float wanderTime;

	// Token: 0x04000B2C RID: 2860
	private SoundModulation.Modulator modulator;

	// Token: 0x06000E1D RID: 3613 RVA: 0x0000D05D File Offset: 0x0000B25D
	public override void Init(Sound targetSound)
	{
		base.Init(targetSound);
		this.startTime = Time.time;
		this.modulator = this.sound.modulation.CreateModulator(SoundModulation.Parameter.Spread);
		this.modulator.value = 0f;
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x0000D098 File Offset: 0x0000B298
	public override void OnSoundPlay()
	{
		this.startTime = Time.time;
		this.wanderTime = Time.time;
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x0006370C File Offset: 0x0006190C
	public override void ApplyModification()
	{
		if (this.modulator == null)
		{
			return;
		}
		this.modulator.value = this.spreadCurve.Evaluate(Time.time - this.startTime);
		this.wanderTime += Time.deltaTime * this.wanderIntensityCurve.Evaluate(Time.time - this.startTime) * 0.1f;
		this.modulator.value *= Mathf.PerlinNoise(base.transform.position.x * 2f, base.transform.position.y * 2f + this.wanderTime);
	}
}
