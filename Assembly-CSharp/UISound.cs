using System;
using ConVar;
using UnityEngine;

// Token: 0x020006AF RID: 1711
public static class UISound
{
	// Token: 0x0400223C RID: 8764
	private static AudioSource source;

	// Token: 0x06002620 RID: 9760 RVA: 0x000C9014 File Offset: 0x000C7214
	private static AudioSource GetAudioSource()
	{
		if (UISound.source != null)
		{
			return UISound.source;
		}
		UISound.source = new GameObject("UISound").AddComponent<AudioSource>();
		UISound.source.spatialBlend = 0f;
		UISound.source.volume = 1f;
		return UISound.source;
	}

	// Token: 0x06002621 RID: 9761 RVA: 0x0001DB6E File Offset: 0x0001BD6E
	public static void Play(AudioClip clip, float volume = 1f)
	{
		if (clip == null)
		{
			return;
		}
		UISound.GetAudioSource().volume = volume * Audio.master * 0.4f;
		UISound.GetAudioSource().PlayOneShot(clip);
	}
}
