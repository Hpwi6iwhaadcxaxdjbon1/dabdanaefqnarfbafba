using System;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class FootstepSound : MonoBehaviour, IClientComponent
{
	// Token: 0x040009BE RID: 2494
	public SoundDefinition lightSound;

	// Token: 0x040009BF RID: 2495
	public SoundDefinition medSound;

	// Token: 0x040009C0 RID: 2496
	public SoundDefinition hardSound;

	// Token: 0x040009C1 RID: 2497
	private const float panAmount = 0.05f;

	// Token: 0x06000CE2 RID: 3298 RVA: 0x0005E3DC File Offset: 0x0005C5DC
	public void PlayFootstep(FootstepSound.Hardness hardness, bool localPlayer = false, GameObject sourceObject = null, bool left = false)
	{
		Sound sound = SoundManager.RequestSoundInstance(this.GetSound(hardness), base.gameObject, default(Vector3), false);
		if (sound == null)
		{
			return;
		}
		if (localPlayer)
		{
			sound.MakeFirstPerson();
		}
		if (sourceObject)
		{
			sound.soundSource = sourceObject.GetComponentInParent<SoundSource>();
		}
		if (hardness == FootstepSound.Hardness.Hard)
		{
			sound.modulation.CreateModulator(SoundModulation.Parameter.MaxDistance).value = 1.75f;
		}
		else if (hardness == FootstepSound.Hardness.Light)
		{
			sound.modulation.CreateModulator(SoundModulation.Parameter.MaxDistance).value = 0.5f;
		}
		if (localPlayer)
		{
			sound.pan = (left ? -0.05f : 0.05f);
		}
		sound.Play();
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x0000BF19 File Offset: 0x0000A119
	private SoundDefinition GetSound(FootstepSound.Hardness hardness)
	{
		if (hardness == FootstepSound.Hardness.Light)
		{
			return this.lightSound;
		}
		if (hardness == FootstepSound.Hardness.Hard)
		{
			return this.hardSound;
		}
		return this.medSound;
	}

	// Token: 0x02000167 RID: 359
	public enum Hardness
	{
		// Token: 0x040009C3 RID: 2499
		Light = 1,
		// Token: 0x040009C4 RID: 2500
		Medium,
		// Token: 0x040009C5 RID: 2501
		Hard
	}
}
