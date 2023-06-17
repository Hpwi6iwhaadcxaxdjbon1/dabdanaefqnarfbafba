using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class SoundDefinition : ScriptableObject
{
	// Token: 0x04000AC3 RID: 2755
	public GameObjectRef template;

	// Token: 0x04000AC4 RID: 2756
	[Horizontal(2, -1)]
	public List<WeightedAudioClip> weightedAudioClips;

	// Token: 0x04000AC5 RID: 2757
	public List<SoundDefinition.DistanceAudioClipList> distanceAudioClips;

	// Token: 0x04000AC6 RID: 2758
	public SoundClass soundClass;

	// Token: 0x04000AC7 RID: 2759
	public bool defaultToFirstPerson;

	// Token: 0x04000AC8 RID: 2760
	public bool loop;

	// Token: 0x04000AC9 RID: 2761
	public bool randomizeStartPosition;

	// Token: 0x04000ACA RID: 2762
	[Range(0f, 1f)]
	public float volume;

	// Token: 0x04000ACB RID: 2763
	[Range(0f, 1f)]
	public float volumeVariation;

	// Token: 0x04000ACC RID: 2764
	[Range(-3f, 3f)]
	public float pitch;

	// Token: 0x04000ACD RID: 2765
	[Range(0f, 1f)]
	public float pitchVariation;

	// Token: 0x04000ACE RID: 2766
	[Header("Voice limiting")]
	public bool dontVoiceLimit;

	// Token: 0x04000ACF RID: 2767
	public int globalVoiceMaxCount;

	// Token: 0x04000AD0 RID: 2768
	public int localVoiceMaxCount;

	// Token: 0x04000AD1 RID: 2769
	public float localVoiceRange;

	// Token: 0x04000AD2 RID: 2770
	public float voiceLimitFadeOutTime;

	// Token: 0x04000AD3 RID: 2771
	public float localVoiceDebounceTime;

	// Token: 0x04000AD4 RID: 2772
	[Header("Occlusion Settings")]
	public bool forceOccludedPlayback;

	// Token: 0x04000AD5 RID: 2773
	[Header("Custom curves")]
	public AnimationCurve falloffCurve;

	// Token: 0x04000AD6 RID: 2774
	public bool useCustomFalloffCurve;

	// Token: 0x04000AD7 RID: 2775
	public AnimationCurve spatialBlendCurve;

	// Token: 0x04000AD8 RID: 2776
	public bool useCustomSpatialBlendCurve;

	// Token: 0x04000AD9 RID: 2777
	public AnimationCurve spreadCurve;

	// Token: 0x04000ADA RID: 2778
	public bool useCustomSpreadCurve;

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x06000DB5 RID: 3509 RVA: 0x00061E88 File Offset: 0x00060088
	public float maxDistance
	{
		get
		{
			if (this.template == null)
			{
				return 0f;
			}
			AudioSource component = this.template.Get().GetComponent<AudioSource>();
			if (component == null)
			{
				return 0f;
			}
			return component.maxDistance;
		}
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x00061ECC File Offset: 0x000600CC
	public float GetLength()
	{
		float num = 0f;
		for (int i = 0; i < this.weightedAudioClips.Count; i++)
		{
			AudioClip audioClip = this.weightedAudioClips[i].audioClip;
			if (audioClip)
			{
				num = Mathf.Max(audioClip.length, num);
			}
		}
		for (int j = 0; j < this.distanceAudioClips.Count; j++)
		{
			List<WeightedAudioClip> audioClips = this.distanceAudioClips[j].audioClips;
			for (int k = 0; k < audioClips.Count; k++)
			{
				AudioClip audioClip2 = audioClips[k].audioClip;
				if (audioClip2)
				{
					num = Mathf.Max(audioClip2.length, num);
				}
			}
		}
		return num;
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x00061F84 File Offset: 0x00060184
	public SoundDefinition()
	{
		List<WeightedAudioClip> list = new List<WeightedAudioClip>();
		list.Add(new WeightedAudioClip());
		this.weightedAudioClips = list;
		this.volume = 1f;
		this.pitch = 1f;
		this.globalVoiceMaxCount = 100;
		this.localVoiceMaxCount = 100;
		this.localVoiceRange = 10f;
		this.voiceLimitFadeOutTime = 0.05f;
		this.localVoiceDebounceTime = 0.1f;
		base..ctor();
	}

	// Token: 0x02000185 RID: 389
	[Serializable]
	public class DistanceAudioClipList
	{
		// Token: 0x04000ADB RID: 2779
		public int distance;

		// Token: 0x04000ADC RID: 2780
		[Horizontal(2, -1)]
		public List<WeightedAudioClip> audioClips;
	}
}
