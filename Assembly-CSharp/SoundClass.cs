using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000182 RID: 386
[CreateAssetMenu(menuName = "Rust/Sound Class")]
public class SoundClass : ScriptableObject
{
	// Token: 0x04000AB8 RID: 2744
	[Header("Mixer Settings")]
	public AudioMixerGroup output;

	// Token: 0x04000AB9 RID: 2745
	public AudioMixerGroup firstPersonOutput;

	// Token: 0x04000ABA RID: 2746
	[Header("Occlusion Settings")]
	public bool enableOcclusion;

	// Token: 0x04000ABB RID: 2747
	public bool playIfOccluded = true;

	// Token: 0x04000ABC RID: 2748
	public float occlusionGain = 1f;

	// Token: 0x04000ABD RID: 2749
	[Tooltip("Use this mixer group when the sound is occluded to save DSP CPU usage. Only works for non-looping sounds.")]
	public AudioMixerGroup occludedOutput;

	// Token: 0x04000ABE RID: 2750
	[Header("Voice Limiting")]
	public int globalVoiceMaxCount = 100;

	// Token: 0x04000ABF RID: 2751
	public List<SoundDefinition> definitions = new List<SoundDefinition>();
}
