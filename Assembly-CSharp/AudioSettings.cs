using System;
using ConVar;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020001CF RID: 463
public class AudioSettings : MonoBehaviour
{
	// Token: 0x04000C06 RID: 3078
	public AudioMixer mixer;

	// Token: 0x06000EE8 RID: 3816 RVA: 0x00067064 File Offset: 0x00065264
	private void Update()
	{
		if (this.mixer == null)
		{
			return;
		}
		this.mixer.SetFloat("MasterVol", this.LinearToDecibel(Audio.master));
		float a;
		this.mixer.GetFloat("MusicVol", ref a);
		if (!LevelManager.isLoaded || !MainCamera.isValid)
		{
			this.mixer.SetFloat("MusicVol", Mathf.Lerp(a, this.LinearToDecibel(Audio.musicvolumemenu), UnityEngine.Time.deltaTime));
		}
		else
		{
			this.mixer.SetFloat("MusicVol", Mathf.Lerp(a, this.LinearToDecibel(Audio.musicvolume), UnityEngine.Time.deltaTime));
		}
		this.mixer.SetFloat("WorldVol", this.LinearToDecibel(Audio.game));
		this.mixer.SetFloat("VoiceVol", this.LinearToDecibel(Audio.voices));
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x00067148 File Offset: 0x00065348
	private float LinearToDecibel(float linear)
	{
		float result;
		if (linear > 0f)
		{
			result = 20f * Mathf.Log10(linear);
		}
		else
		{
			result = -144f;
		}
		return result;
	}
}
