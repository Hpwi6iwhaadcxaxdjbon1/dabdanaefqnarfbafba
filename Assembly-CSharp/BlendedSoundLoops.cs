using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class BlendedSoundLoops : MonoBehaviour, IClientComponent
{
	// Token: 0x0400097D RID: 2429
	[Range(0f, 1f)]
	public float blend;

	// Token: 0x0400097E RID: 2430
	public float blendSmoothing = 1f;

	// Token: 0x0400097F RID: 2431
	public float loopFadeOutTime = 0.5f;

	// Token: 0x04000980 RID: 2432
	public float loopFadeInTime = 0.5f;

	// Token: 0x04000981 RID: 2433
	public float gainModSmoothing = 1f;

	// Token: 0x04000982 RID: 2434
	public float pitchModSmoothing = 1f;

	// Token: 0x04000983 RID: 2435
	public bool shouldPlay = true;

	// Token: 0x04000984 RID: 2436
	public List<BlendedSoundLoops.Loop> loops = new List<BlendedSoundLoops.Loop>();

	// Token: 0x04000985 RID: 2437
	public float maxDistance;

	// Token: 0x04000986 RID: 2438
	private float smoothedBlend;

	// Token: 0x06000CC8 RID: 3272 RVA: 0x0005D888 File Offset: 0x0005BA88
	private void OnValidate()
	{
		this.maxDistance = 0f;
		foreach (BlendedSoundLoops.Loop loop in this.loops)
		{
			if (loop.soundDef.maxDistance > this.maxDistance)
			{
				this.maxDistance = loop.soundDef.maxDistance;
			}
		}
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x0005D904 File Offset: 0x0005BB04
	private void Update()
	{
		if (MainCamera.Distance(base.transform.position) <= this.maxDistance)
		{
			this.smoothedBlend = Mathf.MoveTowards(this.smoothedBlend, this.blend, Time.deltaTime * this.blendSmoothing);
			using (List<BlendedSoundLoops.Loop>.Enumerator enumerator = this.loops.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BlendedSoundLoops.Loop loop = enumerator.Current;
					float num = loop.gainCurve.Evaluate(this.smoothedBlend);
					if (num > 0f && loop.sound == null && this.shouldPlay)
					{
						loop.sound = SoundManager.RequestSoundInstance(loop.soundDef, base.gameObject, default(Vector3), false);
						if (loop.sound != null)
						{
							loop.gainMod = loop.sound.modulation.CreateModulator(SoundModulation.Parameter.Gain);
							loop.gainMod.value = num;
							loop.pitchMod = loop.sound.modulation.CreateModulator(SoundModulation.Parameter.Pitch);
						}
					}
					if (loop.sound != null && (loop.gainMod.value <= 0.05f || !this.shouldPlay))
					{
						loop.sound.FadeOutAndRecycle(this.loopFadeOutTime);
						loop.sound = null;
						loop.gainMod = null;
						loop.pitchMod = null;
					}
					if (loop.sound != null)
					{
						float target = loop.pitchCurve.Evaluate(this.smoothedBlend);
						loop.gainMod.value = Mathf.MoveTowards(loop.gainMod.value, num, Time.deltaTime * this.gainModSmoothing);
						loop.pitchMod.value = Mathf.MoveTowards(loop.pitchMod.value, target, Time.deltaTime * this.pitchModSmoothing);
						if (!loop.sound.playing)
						{
							loop.sound.ApplyModulations();
							loop.sound.FadeInAndPlay(this.loopFadeInTime);
						}
					}
				}
				return;
			}
		}
		foreach (BlendedSoundLoops.Loop loop2 in this.loops)
		{
			if (loop2.sound != null)
			{
				loop2.sound.FadeOutAndRecycle(this.loopFadeOutTime);
				loop2.sound = null;
				loop2.gainMod = null;
				loop2.pitchMod = null;
			}
		}
	}

	// Token: 0x02000160 RID: 352
	[Serializable]
	public class Loop
	{
		// Token: 0x04000987 RID: 2439
		public SoundDefinition soundDef;

		// Token: 0x04000988 RID: 2440
		public AnimationCurve gainCurve;

		// Token: 0x04000989 RID: 2441
		public AnimationCurve pitchCurve;

		// Token: 0x0400098A RID: 2442
		[HideInInspector]
		public Sound sound;

		// Token: 0x0400098B RID: 2443
		[HideInInspector]
		public SoundModulation.Modulator gainMod;

		// Token: 0x0400098C RID: 2444
		[HideInInspector]
		public SoundModulation.Modulator pitchMod;
	}
}
