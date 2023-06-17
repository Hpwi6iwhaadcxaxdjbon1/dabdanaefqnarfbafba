using System;
using UnityEngine;

// Token: 0x02000165 RID: 357
public class FlybySound : MonoBehaviour, IClientComponent
{
	// Token: 0x040009B9 RID: 2489
	public SoundDefinition flybySound;

	// Token: 0x040009BA RID: 2490
	public float flybySoundDistance = 7f;

	// Token: 0x040009BB RID: 2491
	public SoundDefinition closeFlybySound;

	// Token: 0x040009BC RID: 2492
	public float closeFlybyDistance = 3f;

	// Token: 0x040009BD RID: 2493
	private bool flybyPlayed;

	// Token: 0x06000CDF RID: 3295 RVA: 0x0000BEF3 File Offset: 0x0000A0F3
	private void Update()
	{
		this.DoFlybySound();
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x0005E33C File Offset: 0x0005C53C
	private void DoFlybySound()
	{
		if (this.flybyPlayed)
		{
			return;
		}
		if (this.flybySound == null)
		{
			return;
		}
		if (!MainCamera.isValid)
		{
			return;
		}
		float num = MainCamera.Distance(base.transform.position);
		if (num <= this.flybySoundDistance)
		{
			SoundDefinition soundDefinition = (num > this.closeFlybyDistance || this.closeFlybySound == null) ? this.flybySound : this.closeFlybySound;
			if (soundDefinition != null)
			{
				SoundManager.PlayOneshot(soundDefinition, base.gameObject, false, default(Vector3));
			}
			MusicManager.RaiseIntensityTo(0.75f, 0);
			this.flybyPlayed = true;
		}
	}
}
