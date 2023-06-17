using System;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class SoundFade : MonoBehaviour, IClientComponent
{
	// Token: 0x04000ADD RID: 2781
	private Sound _sound;

	// Token: 0x04000ADE RID: 2782
	private SoundModulation.Modulator fadeGainModulator;

	// Token: 0x04000ADF RID: 2783
	private SoundFade.Direction currentDirection;

	// Token: 0x04000AE0 RID: 2784
	private float startTime = float.PositiveInfinity;

	// Token: 0x04000AE1 RID: 2785
	private float length;

	// Token: 0x04000AE2 RID: 2786
	private float fadeStart;

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000DB9 RID: 3513 RVA: 0x0000C9D8 File Offset: 0x0000ABD8
	public Sound sound
	{
		get
		{
			if (this._sound == null)
			{
				this._sound = base.GetComponent<Sound>();
			}
			return this._sound;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000DBA RID: 3514 RVA: 0x0000C9FA File Offset: 0x0000ABFA
	public bool isFading
	{
		get
		{
			return this.length != 0f;
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000DBB RID: 3515 RVA: 0x0000CA0C File Offset: 0x0000AC0C
	public bool isFadingOut
	{
		get
		{
			return this.isFading && this.currentDirection == SoundFade.Direction.Out;
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x06000DBC RID: 3516 RVA: 0x0000CA21 File Offset: 0x0000AC21
	public bool isFadingIn
	{
		get
		{
			return this.isFading && this.currentDirection == SoundFade.Direction.In;
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000DBD RID: 3517 RVA: 0x0000CA36 File Offset: 0x0000AC36
	public float fadeTimeLeft
	{
		get
		{
			return this.length - (Time.time - this.startTime);
		}
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x0000CA4B File Offset: 0x0000AC4B
	public void FadeIn(float time)
	{
		this.DoFade(time, SoundFade.Direction.In);
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x0000CA55 File Offset: 0x0000AC55
	public void FadeOut(float time)
	{
		this.DoFade(time, SoundFade.Direction.Out);
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x00061FF4 File Offset: 0x000601F4
	public void DoUpdate()
	{
		if (this.length == 0f || this.fadeGainModulator == null)
		{
			return;
		}
		float b = (this.currentDirection == SoundFade.Direction.In) ? 1f : 0f;
		float num = (Time.time - this.startTime) / this.length;
		this.fadeGainModulator.value = Mathf.Lerp(this.fadeStart, b, num);
		if (num > 1f)
		{
			this.length = 0f;
		}
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x0006206C File Offset: 0x0006026C
	public void Init()
	{
		if (this.fadeGainModulator != null)
		{
			this.sound.modulation.RemoveModulator(this.fadeGainModulator);
		}
		this.fadeGainModulator = this.sound.modulation.CreateModulator(SoundModulation.Parameter.Gain);
		this.startTime = float.PositiveInfinity;
		this.length = 0f;
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x000620C4 File Offset: 0x000602C4
	private void DoFade(float time, SoundFade.Direction direction)
	{
		this.fadeStart = ((direction == SoundFade.Direction.In) ? 0f : 1f);
		if (this.isFading)
		{
			this.fadeStart = this.fadeGainModulator.value;
		}
		this.startTime = Time.time;
		this.length = time;
		this.currentDirection = direction;
		this.fadeGainModulator.value = this.fadeStart;
		if (this.sound != null)
		{
			this.sound.ApplyModulations();
		}
	}

	// Token: 0x02000187 RID: 391
	public enum Direction
	{
		// Token: 0x04000AE4 RID: 2788
		In,
		// Token: 0x04000AE5 RID: 2789
		Out
	}
}
