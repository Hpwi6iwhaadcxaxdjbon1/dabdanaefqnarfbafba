using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class Sound : MonoBehaviour, IClientComponent, IOnParentDestroying, IComparable<Sound>, ISoundBudgetedUpdate
{
	// Token: 0x04000AA1 RID: 2721
	public static float volumeExponent = Mathf.Log(Mathf.Sqrt(10f), 2f);

	// Token: 0x04000AA2 RID: 2722
	public SoundDefinition definition;

	// Token: 0x04000AA3 RID: 2723
	public SoundModifier[] modifiers;

	// Token: 0x04000AA4 RID: 2724
	public SoundSource soundSource;

	// Token: 0x04000AA5 RID: 2725
	public AudioSource[] audioSources = new AudioSource[2];

	// Token: 0x04000AA6 RID: 2726
	[SerializeField]
	private SoundFade _fade;

	// Token: 0x04000AA7 RID: 2727
	[SerializeField]
	private SoundModulation _modulation;

	// Token: 0x04000AA8 RID: 2728
	[SerializeField]
	private SoundOcclusion _occlusion;

	// Token: 0x04000AA9 RID: 2729
	private AudioSource audioSource;

	// Token: 0x04000AAA RID: 2730
	private AudioSource distantAudioSource;

	// Token: 0x04000AAD RID: 2733
	public bool playing;

	// Token: 0x04000AAE RID: 2734
	public bool isFirstPerson;

	// Token: 0x04000AB1 RID: 2737
	private List<WeightedAudioClip> closeClips = new List<WeightedAudioClip>();

	// Token: 0x04000AB2 RID: 2738
	private List<WeightedAudioClip> farClips = new List<WeightedAudioClip>();

	// Token: 0x04000AB3 RID: 2739
	private float distanceScale;

	// Token: 0x04000AB4 RID: 2740
	private int clipIndex;

	// Token: 0x04000AB5 RID: 2741
	private bool hasDistantSound;

	// Token: 0x04000AB6 RID: 2742
	private float length;

	// Token: 0x04000AB7 RID: 2743
	private int FrameUpdateIndex;

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000D7A RID: 3450 RVA: 0x0000C697 File Offset: 0x0000A897
	public SoundFade fade
	{
		get
		{
			return this._fade;
		}
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000D7B RID: 3451 RVA: 0x0000C69F File Offset: 0x0000A89F
	public SoundModulation modulation
	{
		get
		{
			return this._modulation;
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000D7C RID: 3452 RVA: 0x0000C6A7 File Offset: 0x0000A8A7
	public SoundOcclusion occlusion
	{
		get
		{
			return this._occlusion;
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000D7D RID: 3453 RVA: 0x0000C6AF File Offset: 0x0000A8AF
	// (set) Token: 0x06000D7E RID: 3454 RVA: 0x0000C6B7 File Offset: 0x0000A8B7
	public float initialMaxDistance { get; private set; }

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000D7F RID: 3455 RVA: 0x0000C6C0 File Offset: 0x0000A8C0
	// (set) Token: 0x06000D80 RID: 3456 RVA: 0x0000C6C8 File Offset: 0x0000A8C8
	public float initialSpread { get; private set; }

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000D81 RID: 3457 RVA: 0x0000C6D1 File Offset: 0x0000A8D1
	public float audioSourceVolue
	{
		get
		{
			return this.audioSource.volume;
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000D82 RID: 3458 RVA: 0x0000C6DE File Offset: 0x0000A8DE
	public bool isAudioSourcePlaying
	{
		get
		{
			if (this.hasDistantSound)
			{
				return this.audioSource.isPlaying || this.distantAudioSource.isPlaying;
			}
			return this.audioSource.isPlaying;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000D83 RID: 3459 RVA: 0x0000C70E File Offset: 0x0000A90E
	// (set) Token: 0x06000D84 RID: 3460 RVA: 0x0000C71B File Offset: 0x0000A91B
	public AudioClip audioClip
	{
		get
		{
			return this.audioSource.clip;
		}
		set
		{
			this.audioSource.clip = value;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000D85 RID: 3461 RVA: 0x0000C729 File Offset: 0x0000A929
	// (set) Token: 0x06000D86 RID: 3462 RVA: 0x0000C736 File Offset: 0x0000A936
	public AudioClip distantAudioClip
	{
		get
		{
			return this.distantAudioSource.clip;
		}
		set
		{
			this.distantAudioSource.clip = value;
		}
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x06000D87 RID: 3463 RVA: 0x0000C744 File Offset: 0x0000A944
	// (set) Token: 0x06000D88 RID: 3464 RVA: 0x0000C751 File Offset: 0x0000A951
	public int timeSamples
	{
		get
		{
			return this.audioSource.timeSamples;
		}
		set
		{
			this.audioSource.timeSamples = value;
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x06000D89 RID: 3465 RVA: 0x0000C75F File Offset: 0x0000A95F
	// (set) Token: 0x06000D8A RID: 3466 RVA: 0x0000C76C File Offset: 0x0000A96C
	public float pan
	{
		get
		{
			return this.audioSource.panStereo;
		}
		set
		{
			this.audioSource.panStereo = value;
			if (this.hasDistantSound)
			{
				this.distantAudioSource.panStereo = value;
			}
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x06000D8B RID: 3467 RVA: 0x0000C78E File Offset: 0x0000A98E
	public float maxDistance
	{
		get
		{
			return this.audioSource.maxDistance;
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x06000D8C RID: 3468 RVA: 0x0000C79B File Offset: 0x0000A99B
	// (set) Token: 0x06000D8D RID: 3469 RVA: 0x0000C7A3 File Offset: 0x0000A9A3
	public float startTime { get; private set; }

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000D8E RID: 3470 RVA: 0x0000C7AC File Offset: 0x0000A9AC
	// (set) Token: 0x06000D8F RID: 3471 RVA: 0x0000C7B4 File Offset: 0x0000A9B4
	public float endTime { get; private set; }

	// Token: 0x06000D90 RID: 3472 RVA: 0x00061148 File Offset: 0x0005F348
	protected void Awake()
	{
		this.initialSpread = 0f;
		this.initialMaxDistance = 0f;
		this.startTime = 0f;
		this.endTime = float.PositiveInfinity;
		this.audioSource = this.audioSources[0];
		this.distantAudioSource = this.audioSources[1];
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x000611A0 File Offset: 0x0005F3A0
	public void Init(SoundSource source = null)
	{
		this.soundSource = source;
		this.pan = 0f;
		this.playing = false;
		this.startTime = 0f;
		this.endTime = float.PositiveInfinity;
		this.hasDistantSound = (this.definition.distanceAudioClips.Count > 0 && this.distantAudioSource != null);
		this.CalcLength();
		this.modulation.Init();
		this.fade.Init();
		this.occlusion.Init();
		this.InitAudioSource(this.audioSource);
		this.audioSource.clip = this.definition.weightedAudioClips[this.clipIndex].audioClip;
		if (this.hasDistantSound)
		{
			this.InitAudioSource(this.distantAudioSource);
			this.distantAudioSource.clip = this.definition.distanceAudioClips[0].audioClips[0].audioClip;
		}
		for (int i = 0; i < this.modifiers.Length; i++)
		{
			this.modifiers[i].Init(this);
			this.modifiers[i].ApplyModification();
		}
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x000612CC File Offset: 0x0005F4CC
	private void InitAudioSource(AudioSource source)
	{
		source.enabled = true;
		source.volume = Mathf.Pow(this.definition.volume, Sound.volumeExponent);
		source.pitch = this.definition.pitch;
		source.loop = this.definition.loop;
		source.timeSamples = 0;
		source.playOnAwake = false;
		source.outputAudioMixerGroup = this.definition.soundClass.output;
		if (this.initialMaxDistance == 0f)
		{
			this.initialMaxDistance = source.maxDistance;
			this.initialSpread = source.spread;
			return;
		}
		source.maxDistance = this.initialMaxDistance;
		if (!this.definition.useCustomSpreadCurve)
		{
			source.spread = this.initialSpread;
		}
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x0000C7BD File Offset: 0x0000A9BD
	private void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.Stop();
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x0006138C File Offset: 0x0005F58C
	public void DoUpdate()
	{
		if (Time.time < this.endTime && this.playing)
		{
			this.fade.DoUpdate();
			this.ApplyModifications();
			this.ApplyModulations();
			if (!this.definition.loop)
			{
				this.SetDistantVolumes();
				return;
			}
			this.DoDistantCrossfade();
			if (this.definition.soundClass.enableOcclusion)
			{
				this.occlusion.UpdateOcclusion(true);
			}
			float num = MainCamera.Distance(base.transform.position);
			if (num > this.maxDistance && this.isAudioSourcePlaying)
			{
				this.Pause();
			}
			if (num < this.maxDistance && !this.isAudioSourcePlaying)
			{
				this.UnPause();
				return;
			}
		}
		else if (Time.time > this.endTime)
		{
			if (this.playing && this.startTime + this.length > this.endTime)
			{
				this.Stop();
			}
			SoundManager.RecycleSound(this);
		}
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x00061478 File Offset: 0x0005F678
	public void DoDistantCrossfade()
	{
		if (!this.hasDistantSound)
		{
			return;
		}
		using (TimeWarning.New("Sound.DoDistantCrossfade", 0.1f))
		{
			float num = MainCamera.Distance(base.transform.position);
			this.closeClips = this.definition.weightedAudioClips;
			this.farClips = this.definition.distanceAudioClips[0].audioClips;
			float num2 = 0f;
			int distance = this.definition.distanceAudioClips[0].distance;
			this.audioSource = this.audioSources[0];
			this.distantAudioSource = this.audioSources[1];
			if (this.definition.distanceAudioClips.Count > 1)
			{
				for (int i = 0; i < this.definition.distanceAudioClips.Count - 1; i++)
				{
					if ((float)this.definition.distanceAudioClips[i].distance < num)
					{
						this.closeClips = this.definition.distanceAudioClips[i].audioClips;
						this.farClips = this.definition.distanceAudioClips[i + 1].audioClips;
						num2 = (float)this.definition.distanceAudioClips[i].distance;
						distance = this.definition.distanceAudioClips[i + 1].distance;
						AudioSource audioSource = this.audioSource;
						this.audioSource = this.distantAudioSource;
						this.distantAudioSource = audioSource;
					}
				}
			}
			this.distanceScale = Mathf.Clamp((num - num2) / ((float)distance - num2), 0f, 1f);
			this.SetDistantVolumes();
			this.audioSource.clip = this.closeClips[this.clipIndex].audioClip;
			this.distantAudioSource.clip = this.farClips[this.clipIndex].audioClip;
			if (this.definition.loop && !this.audioSource.isPlaying && this.distantAudioSource.isPlaying)
			{
				this.audioSource.timeSamples = Mathf.Clamp(this.distantAudioSource.timeSamples, 0, this.audioSource.clip.samples);
				this.audioSource.Play();
			}
			if (this.definition.loop && this.audioSource.isPlaying && !this.distantAudioSource.isPlaying)
			{
				this.distantAudioSource.timeSamples = Mathf.Clamp(this.audioSource.timeSamples, 0, this.distantAudioSource.clip.samples);
				this.distantAudioSource.Play();
			}
		}
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00061744 File Offset: 0x0005F944
	private void SetDistantVolumes()
	{
		if (!this.hasDistantSound)
		{
			return;
		}
		this.audioSource.volume = Mathf.Pow(this.definition.volume * (1f - this.distanceScale) * this.modulation.ModulationValue(SoundModulation.Parameter.Gain), Sound.volumeExponent);
		this.distantAudioSource.volume = Mathf.Pow(this.definition.volume * this.distanceScale * this.modulation.ModulationValue(SoundModulation.Parameter.Gain), Sound.volumeExponent);
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x000617C8 File Offset: 0x0005F9C8
	public void ApplyModulations()
	{
		if (this.modulation == null)
		{
			return;
		}
		using (TimeWarning.New("Sound.ApplyModulations", 0.1f))
		{
			this.modulation.CalculateValues();
			this.modulation.ApplyModulations(this.audioSource);
			if (this.hasDistantSound)
			{
				this.modulation.ApplyModulations(this.distantAudioSource);
			}
		}
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x00061848 File Offset: 0x0005FA48
	public void ApplyModifications()
	{
		using (TimeWarning.New("Sound.ApplyModifications", 0.1f))
		{
			for (int i = 0; i < this.modifiers.Length; i++)
			{
				this.modifiers[i].ApplyModification();
			}
		}
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x0000C7CD File Offset: 0x0000A9CD
	public void MakeThirdPerson()
	{
		this.MakeThirdPerson(this.audioSource);
		if (this.hasDistantSound)
		{
			this.MakeThirdPerson(this.distantAudioSource);
		}
		this.isFirstPerson = false;
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x0000C7F6 File Offset: 0x0000A9F6
	public void MakeThirdPerson(AudioSource source)
	{
		source.spatialBlend = 1f;
		source.outputAudioMixerGroup = this.definition.soundClass.output;
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x0000C819 File Offset: 0x0000AA19
	public void MakeFirstPerson()
	{
		this.MakeFirstPerson(this.audioSource);
		if (this.hasDistantSound)
		{
			this.MakeFirstPerson(this.distantAudioSource);
		}
		this.isFirstPerson = true;
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x000618A4 File Offset: 0x0005FAA4
	public void MakeFirstPerson(AudioSource source)
	{
		source.spatialBlend = 0f;
		source.dopplerLevel = 0f;
		if (this.definition.soundClass.firstPersonOutput != null)
		{
			source.outputAudioMixerGroup = this.definition.soundClass.firstPersonOutput;
			return;
		}
		source.outputAudioMixerGroup = this.definition.soundClass.output;
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x0000C842 File Offset: 0x0000AA42
	public float GetLength()
	{
		return this.length;
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x0006190C File Offset: 0x0005FB0C
	public void CalcLength()
	{
		float num = 0f;
		foreach (WeightedAudioClip weightedAudioClip in this.definition.weightedAudioClips)
		{
			if (weightedAudioClip.audioClip)
			{
				num = Mathf.Max(num, weightedAudioClip.audioClip.length);
			}
		}
		if (this.hasDistantSound)
		{
			foreach (SoundDefinition.DistanceAudioClipList distanceAudioClipList in this.definition.distanceAudioClips)
			{
				foreach (WeightedAudioClip weightedAudioClip2 in distanceAudioClipList.audioClips)
				{
					if (weightedAudioClip2.audioClip)
					{
						num = Mathf.Max(num, weightedAudioClip2.audioClip.length);
					}
				}
			}
		}
		float num2 = 1f / (this.definition.pitch - this.definition.pitchVariation);
		this.length = num / num2;
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x0000C84A File Offset: 0x0000AA4A
	public int CompareTo(Sound other)
	{
		if (this.endTime < other.endTime)
		{
			return -1;
		}
		if (this.endTime <= other.endTime)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x00061A50 File Offset: 0x0005FC50
	public void Play()
	{
		using (TimeWarning.New("Sound.Play", 0.1f))
		{
			if (this.definition.soundClass.enableOcclusion)
			{
				this.occlusion.UpdateOcclusion(false);
				if (!this.definition.soundClass.playIfOccluded && this.occlusion.isOccluded)
				{
					return;
				}
				if (this.occlusion.isOccluded && this.definition.soundClass.occludedOutput != null)
				{
					this.audioSource.outputAudioMixerGroup = this.definition.soundClass.occludedOutput;
					if (this.hasDistantSound)
					{
						this.distantAudioSource.outputAudioMixerGroup = this.definition.soundClass.occludedOutput;
					}
				}
				else
				{
					this.audioSource.outputAudioMixerGroup = this.definition.soundClass.output;
					if (this.hasDistantSound)
					{
						this.distantAudioSource.outputAudioMixerGroup = this.definition.soundClass.output;
					}
				}
			}
			this.startTime = Time.time;
			this.endTime = float.PositiveInfinity;
			this.playing = true;
			for (int i = 0; i < this.modifiers.Length; i++)
			{
				this.modifiers[i].OnSoundPlay();
			}
			this.ApplyModifications();
			this.ApplyModulations();
			if (this.hasDistantSound)
			{
				this.SetDistantVolumes();
			}
			this.PlayAudioSources();
		}
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x00061BDC File Offset: 0x0005FDDC
	public void PlayAudioSources()
	{
		if (!this.definition.loop && this.hasDistantSound)
		{
			if (this.audioSource.volume > 0.05f)
			{
				this.PlayAudioSource(this.audioSource);
			}
			if (this.distantAudioSource.volume > 0.05f)
			{
				this.PlayAudioSource(this.distantAudioSource);
				return;
			}
		}
		else
		{
			this.PlayAudioSource(this.audioSource);
			if (this.hasDistantSound)
			{
				this.PlayAudioSource(this.distantAudioSource);
			}
		}
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x0000C86D File Offset: 0x0000AA6D
	public void PlayAudioSource(AudioSource source)
	{
		if (source.gameObject.activeInHierarchy)
		{
			source.Stop();
			source.Play();
		}
		if (this.definition.loop)
		{
			source.playOnAwake = true;
		}
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x00061C5C File Offset: 0x0005FE5C
	public void Stop()
	{
		this.playing = false;
		if (this.audioSource != null)
		{
			this.audioSource.Stop();
			if (this.definition != null && this.definition.loop)
			{
				this.audioSource.playOnAwake = false;
			}
		}
		if (this.hasDistantSound && this.distantAudioSource != null)
		{
			this.distantAudioSource.Stop();
			if (this.definition != null && this.definition.loop)
			{
				this.distantAudioSource.playOnAwake = false;
			}
		}
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x0000C89C File Offset: 0x0000AA9C
	public void Pause()
	{
		this.audioSource.Pause();
		if (this.hasDistantSound)
		{
			this.distantAudioSource.Pause();
		}
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x0000C8BC File Offset: 0x0000AABC
	public void UnPause()
	{
		this.audioSource.UnPause();
		if (this.hasDistantSound)
		{
			this.distantAudioSource.UnPause();
		}
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x0000C8DC File Offset: 0x0000AADC
	public void RecycleAfterPlaying()
	{
		this.endTime = this.startTime + this.GetLength();
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x0000C8F1 File Offset: 0x0000AAF1
	public void StopAndRecycle(float delay = 0f)
	{
		this.endTime = Time.time + delay;
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x0000C900 File Offset: 0x0000AB00
	public void SetClipIndex(int i)
	{
		this.clipIndex = i;
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x0000C909 File Offset: 0x0000AB09
	public void FadeInAndPlay(float time)
	{
		this.fade.FadeIn(time);
		this.Play();
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x0000C91D File Offset: 0x0000AB1D
	public void FadeOutAndRecycle(float time)
	{
		this.fade.FadeOut(time);
		this.StopAndRecycle(time);
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x0000C932 File Offset: 0x0000AB32
	public void DisconnectFromParent()
	{
		if (base.transform.parent != null)
		{
			base.transform.parent = null;
		}
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x00061CF8 File Offset: 0x0005FEF8
	public void OnParentDestroying()
	{
		if (this.definition.loop)
		{
			if (this.fade.isFadingOut)
			{
				this.StopAndRecycle(this.fade.fadeTimeLeft);
			}
			else
			{
				this.FadeOutAndRecycle(0.1f);
			}
		}
		else
		{
			this.RecycleAfterPlaying();
		}
		this.DisconnectFromParent();
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x00061D4C File Offset: 0x0005FF4C
	public void SetCustomFalloffCurve(AnimationCurve curve)
	{
		for (int i = 0; i < this.audioSources.Length; i++)
		{
			if (!(this.audioSources[i] == null))
			{
				this.audioSources[i].SetCustomCurve(0, curve);
			}
		}
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x00061D8C File Offset: 0x0005FF8C
	public void SetCustomSpatialBlendCurve(AnimationCurve curve)
	{
		for (int i = 0; i < this.audioSources.Length; i++)
		{
			if (!(this.audioSources[i] == null))
			{
				this.audioSources[i].SetCustomCurve(1, curve);
			}
		}
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x00061DCC File Offset: 0x0005FFCC
	public void SetCustomSpreadCurve(AnimationCurve curve)
	{
		for (int i = 0; i < this.audioSources.Length; i++)
		{
			if (!(this.audioSources[i] == null))
			{
				this.audioSources[i].SetCustomCurve(3, curve);
			}
		}
	}
}
