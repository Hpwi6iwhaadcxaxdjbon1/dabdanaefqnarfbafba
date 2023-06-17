using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000191 RID: 401
public class SoundRandomizer
{
	// Token: 0x04000B13 RID: 2835
	private static Dictionary<SoundDefinition, int> lastClipIndexBySoundDef = new Dictionary<SoundDefinition, int>();

	// Token: 0x06000E02 RID: 3586 RVA: 0x0000CE88 File Offset: 0x0000B088
	public static void Randomize(Sound sound, bool randomizeClip = true)
	{
		SoundRandomizer.RandomizeVolume(sound);
		SoundRandomizer.RandomizePitch(sound);
		if (randomizeClip)
		{
			SoundRandomizer.RandomizeClipIndex(sound);
		}
		if (sound.definition.randomizeStartPosition)
		{
			SoundRandomizer.RandomizeStartPosition(sound);
		}
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x0006319C File Offset: 0x0006139C
	public static void RandomizeVolume(Sound sound)
	{
		if (sound.definition.volumeVariation == 0f)
		{
			return;
		}
		sound.modulation.CreateModulator(SoundModulation.Parameter.Gain).value = 1f + Random.Range(-sound.definition.volumeVariation, sound.definition.volumeVariation);
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x000631F0 File Offset: 0x000613F0
	private static void RandomizePitch(Sound sound)
	{
		if (sound.definition.pitchVariation == 0f)
		{
			return;
		}
		sound.modulation.CreateModulator(SoundModulation.Parameter.Pitch).value = 1f + Random.Range(-sound.definition.pitchVariation, sound.definition.pitchVariation);
	}

	// Token: 0x06000E05 RID: 3589 RVA: 0x00063244 File Offset: 0x00061444
	public static void RandomizeClipIndex(Sound sound)
	{
		if (sound.definition.weightedAudioClips.Count == 0)
		{
			return;
		}
		if (!SoundRandomizer.lastClipIndexBySoundDef.ContainsKey(sound.definition))
		{
			SoundRandomizer.lastClipIndexBySoundDef[sound.definition] = 0;
		}
		int randomClipIndex = SoundRandomizer.GetRandomClipIndex(sound);
		if (sound.definition.weightedAudioClips.Count > 1)
		{
			while (randomClipIndex == SoundRandomizer.lastClipIndexBySoundDef[sound.definition])
			{
				randomClipIndex = SoundRandomizer.GetRandomClipIndex(sound);
			}
		}
		sound.SetClipIndex(randomClipIndex);
		SoundRandomizer.lastClipIndexBySoundDef[sound.definition] = randomClipIndex;
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x0000CEB2 File Offset: 0x0000B0B2
	private static void RandomizeStartPosition(Sound sound)
	{
		if (sound.audioClip == null)
		{
			Debug.LogWarning("[RandomizeStartPosition] Called on sound without audioClip: " + sound.name);
			return;
		}
		sound.timeSamples = Random.Range(0, sound.audioClip.samples);
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x000632D8 File Offset: 0x000614D8
	private static int GetRandomClipIndex(Sound sound)
	{
		List<WeightedAudioClip> weightedAudioClips = sound.definition.weightedAudioClips;
		float num = 0f;
		for (int i = 0; i < weightedAudioClips.Count; i++)
		{
			num += (float)weightedAudioClips[i].weight;
		}
		if (num == 0f)
		{
			return 0;
		}
		float num2 = Random.Range(0f, num);
		for (int j = 0; j < weightedAudioClips.Count; j++)
		{
			if ((num2 -= (float)weightedAudioClips[j].weight) <= 0f)
			{
				return j;
			}
		}
		return 0;
	}
}
