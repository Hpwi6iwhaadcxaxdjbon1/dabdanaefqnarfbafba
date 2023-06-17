using System;
using UnityEngine;

// Token: 0x02000157 RID: 343
public class AmbienceLocalStings : MonoBehaviour
{
	// Token: 0x04000944 RID: 2372
	public float maxDistance = 100f;

	// Token: 0x04000945 RID: 2373
	public float stingRadius = 10f;

	// Token: 0x04000946 RID: 2374
	public float stingFrequency = 30f;

	// Token: 0x04000947 RID: 2375
	public float stingFrequencyVariance = 15f;

	// Token: 0x04000948 RID: 2376
	public SoundDefinition[] stingSounds;

	// Token: 0x04000949 RID: 2377
	private SynchronizedClock stingClock = new SynchronizedClock();

	// Token: 0x06000C99 RID: 3225 RVA: 0x0005C4A0 File Offset: 0x0005A6A0
	private void PlaySting(uint seed)
	{
		if (!MainCamera.isValid)
		{
			return;
		}
		if (MainCamera.Distance(base.transform.position) > this.maxDistance)
		{
			return;
		}
		SoundDefinition soundDefinition = this.stingSounds[Random.Range(0, this.stingSounds.Length)];
		if (soundDefinition == null)
		{
			return;
		}
		Sound sound = SoundManager.RequestSoundInstance(soundDefinition, base.gameObject, default(Vector3), false);
		if (sound == null)
		{
			return;
		}
		sound.transform.rotation = Random.rotation;
		sound.transform.Translate(new Vector3(Random.Range(0f, this.stingRadius), 0f, 0f));
		sound.Play();
		sound.RecycleAfterPlaying();
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x0000BC1A File Offset: 0x00009E1A
	private void Start()
	{
		this.stingClock.Add(this.stingFrequency, this.stingFrequencyVariance, new Action<uint>(this.PlaySting));
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x0000BC3F File Offset: 0x00009E3F
	private void Update()
	{
		this.stingClock.Tick();
	}
}
