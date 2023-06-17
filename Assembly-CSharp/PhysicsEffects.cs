using System;
using UnityEngine;

// Token: 0x02000285 RID: 645
public class PhysicsEffects : MonoBehaviour
{
	// Token: 0x04000F07 RID: 3847
	public BaseEntity entity;

	// Token: 0x04000F08 RID: 3848
	public SoundDefinition physImpactSoundDef;

	// Token: 0x04000F09 RID: 3849
	public float minTimeBetweenEffects = 0.25f;

	// Token: 0x04000F0A RID: 3850
	public float minDistBetweenEffects = 0.1f;

	// Token: 0x04000F0B RID: 3851
	public float hardnessScale = 1f;

	// Token: 0x04000F0C RID: 3852
	public float lowMedThreshold = 0.4f;

	// Token: 0x04000F0D RID: 3853
	public float medHardThreshold = 0.7f;

	// Token: 0x04000F0E RID: 3854
	public float enableDelay = 0.1f;

	// Token: 0x04000F0F RID: 3855
	public LayerMask ignoreLayers;

	// Token: 0x06001269 RID: 4713 RVA: 0x00078B10 File Offset: 0x00076D10
	public void PlayImpactSound(float hardness)
	{
		Sound sound = SoundManager.RequestSoundInstance(this.physImpactSoundDef, null, base.transform.position, false);
		if (sound != null)
		{
			sound.modulation.CreateModulator(SoundModulation.Parameter.Gain).value = hardness;
			sound.Play();
			sound.RecycleAfterPlaying();
		}
	}
}
