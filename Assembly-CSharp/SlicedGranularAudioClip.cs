using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class SlicedGranularAudioClip : MonoBehaviour, IClientComponent
{
	// Token: 0x04000A81 RID: 2689
	public AudioClip sourceClip;

	// Token: 0x04000A82 RID: 2690
	public AudioClip granularClip;

	// Token: 0x04000A83 RID: 2691
	public int sampleRate = 44100;

	// Token: 0x04000A84 RID: 2692
	public float grainAttack = 0.1f;

	// Token: 0x04000A85 RID: 2693
	public float grainSustain = 0.1f;

	// Token: 0x04000A86 RID: 2694
	public float grainRelease = 0.1f;

	// Token: 0x04000A87 RID: 2695
	public float grainFrequency = 0.1f;

	// Token: 0x04000A88 RID: 2696
	public int grainAttackSamples;

	// Token: 0x04000A89 RID: 2697
	public int grainSustainSamples;

	// Token: 0x04000A8A RID: 2698
	public int grainReleaseSamples;

	// Token: 0x04000A8B RID: 2699
	public int grainFrequencySamples;

	// Token: 0x04000A8C RID: 2700
	public int samplesUntilNextGrain;

	// Token: 0x04000A8D RID: 2701
	public List<SlicedGranularAudioClip.Grain> grains = new List<SlicedGranularAudioClip.Grain>();

	// Token: 0x04000A8E RID: 2702
	public List<int> startPositions = new List<int>();

	// Token: 0x04000A8F RID: 2703
	public int lastStartPositionIdx = int.MaxValue;

	// Token: 0x04000A90 RID: 2704
	private float[] sourceAudioData;

	// Token: 0x04000A91 RID: 2705
	private int sourceChannels = 1;

	// Token: 0x04000A92 RID: 2706
	private AudioSource source;

	// Token: 0x04000A93 RID: 2707
	private bool audioDataLoaded;

	// Token: 0x04000A94 RID: 2708
	private Random random = new Random();

	// Token: 0x06000D6D RID: 3437 RVA: 0x0000C63B File Offset: 0x0000A83B
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		if (!this.audioDataLoaded)
		{
			this.sourceClip.LoadAudioData();
		}
		this.RefreshCachedData();
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x00060C24 File Offset: 0x0005EE24
	private void Update()
	{
		this.grainFrequencySamples = Mathf.FloorToInt(this.grainFrequency * (float)this.sampleRate * (float)this.sourceChannels);
		if (!this.audioDataLoaded && this.sourceClip.loadState == 2)
		{
			this.sampleRate = this.sourceClip.frequency;
			this.sourceAudioData = new float[this.sourceClip.samples * this.sourceClip.channels];
			this.sourceClip.GetData(this.sourceAudioData, 0);
			this.sourceClip.UnloadAudioData();
			this.InitAudioClip();
			this.source.clip = this.granularClip;
			this.source.loop = true;
			this.audioDataLoaded = true;
		}
		if (MainCamera.Distance(base.transform.position) > this.source.maxDistance)
		{
			if (this.source.isPlaying)
			{
				this.source.Stop();
				return;
			}
		}
		else if (!this.source.isPlaying && this.audioDataLoaded)
		{
			this.source.Play();
		}
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x00060D40 File Offset: 0x0005EF40
	private void RefreshCachedData()
	{
		this.grainAttackSamples = Mathf.FloorToInt(this.grainAttack * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainSustainSamples = Mathf.FloorToInt(this.grainSustain * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainReleaseSamples = Mathf.FloorToInt(this.grainRelease * (float)this.sampleRate * (float)this.sourceChannels);
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x00060DB0 File Offset: 0x0005EFB0
	private void InitAudioClip()
	{
		int num = 1;
		int num2 = 1;
		AudioSettings.GetDSPBufferSize(ref num, ref num2);
		this.granularClip = AudioClip.Create(this.sourceClip.name + " (granular)", num, this.sourceClip.channels, this.sampleRate, true, new AudioClip.PCMReaderCallback(this.OnAudioRead));
		this.sourceChannels = this.sourceClip.channels;
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x00060E1C File Offset: 0x0005F01C
	private void OnAudioRead(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (this.samplesUntilNextGrain <= 0)
			{
				this.SpawnGrain();
			}
			float num = 0f;
			for (int j = 0; j < this.grains.Count; j++)
			{
				num += this.grains[j].GetSample();
			}
			data[i] = num;
			this.samplesUntilNextGrain--;
		}
		this.CleanupFinishedGrains();
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x00060E90 File Offset: 0x0005F090
	private void SpawnGrain()
	{
		if (this.grainFrequencySamples == 0)
		{
			return;
		}
		int start = this.startPositions[this.random.Next(0, this.startPositions.Count)];
		SlicedGranularAudioClip.Grain grain = Pool.Get<SlicedGranularAudioClip.Grain>();
		grain.Init(this.sourceAudioData, start, this.grainAttackSamples, this.grainSustainSamples, this.grainReleaseSamples);
		this.grains.Add(grain);
		this.grains[0].FadeOut();
		this.samplesUntilNextGrain = this.grainFrequencySamples;
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x00060F18 File Offset: 0x0005F118
	private void CleanupFinishedGrains()
	{
		for (int i = this.grains.Count - 1; i >= 0; i--)
		{
			SlicedGranularAudioClip.Grain grain = this.grains[i];
			if (grain.finished)
			{
				Pool.Free<SlicedGranularAudioClip.Grain>(ref grain);
				this.grains.RemoveAt(i);
			}
		}
	}

	// Token: 0x02000180 RID: 384
	public class Grain
	{
		// Token: 0x04000A95 RID: 2709
		private float[] sourceData;

		// Token: 0x04000A96 RID: 2710
		private int startSample;

		// Token: 0x04000A97 RID: 2711
		private int currentSample;

		// Token: 0x04000A98 RID: 2712
		private int attackTimeSamples;

		// Token: 0x04000A99 RID: 2713
		private int sustainTimeSamples;

		// Token: 0x04000A9A RID: 2714
		private int releaseTimeSamples;

		// Token: 0x04000A9B RID: 2715
		private float gain;

		// Token: 0x04000A9C RID: 2716
		private float gainPerSampleAttack;

		// Token: 0x04000A9D RID: 2717
		private float gainPerSampleRelease;

		// Token: 0x04000A9E RID: 2718
		private int attackEndSample;

		// Token: 0x04000A9F RID: 2719
		private int releaseStartSample;

		// Token: 0x04000AA0 RID: 2720
		private int endSample;

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000D75 RID: 3445 RVA: 0x0000C663 File Offset: 0x0000A863
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x00060FE8 File Offset: 0x0005F1E8
		public void Init(float[] source, int start, int attack, int sustain, int release)
		{
			this.sourceData = source;
			this.startSample = start;
			this.currentSample = start;
			this.attackTimeSamples = attack;
			this.sustainTimeSamples = sustain;
			this.releaseTimeSamples = release;
			this.gainPerSampleAttack = 0.5f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -0.5f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x0006108C File Offset: 0x0005F28C
		public float GetSample()
		{
			if (this.currentSample >= this.sourceData.Length)
			{
				return 0f;
			}
			float num = this.sourceData[this.currentSample];
			if (this.currentSample <= this.attackEndSample)
			{
				this.gain += this.gainPerSampleAttack;
				if (this.gain > 0.5f)
				{
					this.gain = 0.5f;
				}
			}
			else if (this.currentSample >= this.releaseStartSample)
			{
				this.gain += this.gainPerSampleRelease;
				if (this.gain < 0f)
				{
					this.gain = 0f;
				}
			}
			this.currentSample++;
			return num * this.gain;
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x0000C676 File Offset: 0x0000A876
		public void FadeOut()
		{
			this.releaseStartSample = this.currentSample;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
		}
	}
}
