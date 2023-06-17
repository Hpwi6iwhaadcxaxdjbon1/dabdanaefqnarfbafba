using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class SoundVoiceLimiter : MonoBehaviour, IClientComponent
{
	// Token: 0x04000B26 RID: 2854
	public int maxSimultaneousSounds = 5;

	// Token: 0x04000B27 RID: 2855
	private List<Sound> sounds = new List<Sound>();

	// Token: 0x06000E19 RID: 3609 RVA: 0x00063624 File Offset: 0x00061824
	public void EnforceSoundLimit()
	{
		using (TimeWarning.New("SoundVoiceLimiter.EnforceSoundLimit", 0.1f))
		{
			for (int i = 0; i < this.sounds.Count; i++)
			{
				Sound sound = this.sounds[i];
				if (sound == null || !sound.playing)
				{
					this.sounds.RemoveAt(i--);
				}
			}
			int num = 0;
			while (num < this.sounds.Count && this.sounds.Count > this.maxSimultaneousSounds)
			{
				Sound sound2 = this.sounds[num];
				if (sound2.definition.dontVoiceLimit)
				{
					sound2.RecycleAfterPlaying();
				}
				else
				{
					sound2.StopAndRecycle(0f);
				}
				this.sounds.RemoveAt(num--);
				num++;
			}
		}
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x0000D020 File Offset: 0x0000B220
	public void AddSound(Sound sound)
	{
		this.sounds.Add(sound);
		this.EnforceSoundLimit();
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x0000D034 File Offset: 0x0000B234
	public void RemoveSound(Sound sound)
	{
		this.sounds.Remove(sound);
	}
}
