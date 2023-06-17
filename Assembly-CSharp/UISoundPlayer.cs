using System;
using ConVar;
using UnityEngine;

// Token: 0x020006E7 RID: 1767
public class UISoundPlayer : MonoBehaviour
{
	// Token: 0x0600270F RID: 9999 RVA: 0x0001E7E3 File Offset: 0x0001C9E3
	public void PlaySound(AudioClip clip)
	{
		UISound.Play(clip, Audio.master);
	}

	// Token: 0x06002710 RID: 10000 RVA: 0x000CBAA8 File Offset: 0x000C9CA8
	public void PlaySoundDef(SoundDefinition sound)
	{
		SoundManager.PlayOneshot(sound, null, true, default(Vector3));
	}
}
