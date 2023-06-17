using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class GranularAudioClip : MonoBehaviour
{
	// Token: 0x040009C6 RID: 2502
	public AudioClip sourceClip;

	// Token: 0x040009C7 RID: 2503
	private float[] sourceAudioData;

	// Token: 0x040009C8 RID: 2504
	private int sourceChannels = 1;

	// Token: 0x040009C9 RID: 2505
	public AudioClip granularClip;

	// Token: 0x040009CA RID: 2506
	public int sampleRate = 44100;

	// Token: 0x040009CB RID: 2507
	public float sourceTime = 0.5f;

	// Token: 0x040009CC RID: 2508
	public float sourceTimeVariation = 0.1f;

	// Token: 0x040009CD RID: 2509
	public float grainAttack = 0.1f;

	// Token: 0x040009CE RID: 2510
	public float grainSustain = 0.1f;

	// Token: 0x040009CF RID: 2511
	public float grainRelease = 0.1f;

	// Token: 0x040009D0 RID: 2512
	public float grainFrequency = 0.1f;

	// Token: 0x040009D1 RID: 2513
	public int grainAttackSamples;

	// Token: 0x040009D2 RID: 2514
	public int grainSustainSamples;

	// Token: 0x040009D3 RID: 2515
	public int grainReleaseSamples;

	// Token: 0x040009D4 RID: 2516
	public int grainFrequencySamples;

	// Token: 0x040009D5 RID: 2517
	public int samplesUntilNextGrain;

	// Token: 0x040009D6 RID: 2518
	public List<GranularAudioClip.Grain> grains = new List<GranularAudioClip.Grain>();

	// Token: 0x040009D7 RID: 2519
	private Random random = new Random();

	// Token: 0x040009D8 RID: 2520
	private bool inited;

	// Token: 0x06000CE5 RID: 3301 RVA: 0x0005E484 File Offset: 0x0005C684
	private void Update()
	{
		if (!this.inited && this.sourceClip.loadState == 2)
		{
			this.sampleRate = this.sourceClip.frequency;
			this.sourceAudioData = new float[this.sourceClip.samples * this.sourceClip.channels];
			this.sourceClip.GetData(this.sourceAudioData, 0);
			this.InitAudioClip();
			AudioSource component = base.GetComponent<AudioSource>();
			component.clip = this.granularClip;
			component.loop = true;
			component.Play();
			this.inited = true;
		}
		this.RefreshCachedData();
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x0005E520 File Offset: 0x0005C720
	private void RefreshCachedData()
	{
		this.grainAttackSamples = Mathf.FloorToInt(this.grainAttack * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainSustainSamples = Mathf.FloorToInt(this.grainSustain * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainReleaseSamples = Mathf.FloorToInt(this.grainRelease * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainFrequencySamples = Mathf.FloorToInt(this.grainFrequency * (float)this.sampleRate * (float)this.sourceChannels);
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x0005E5B4 File Offset: 0x0005C7B4
	private void InitAudioClip()
	{
		int num = 1;
		int num2 = 1;
		AudioSettings.GetDSPBufferSize(ref num, ref num2);
		this.granularClip = AudioClip.Create(this.sourceClip.name + " (granular)", num, this.sourceClip.channels, this.sampleRate, true, new AudioClip.PCMReaderCallback(this.OnAudioRead));
		this.sourceChannels = this.sourceClip.channels;
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x0005E620 File Offset: 0x0005C820
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

	// Token: 0x06000CE9 RID: 3305 RVA: 0x0005E694 File Offset: 0x0005C894
	private void SpawnGrain()
	{
		if (this.grainFrequencySamples == 0)
		{
			return;
		}
		float num = (float)(this.random.NextDouble() * (double)this.sourceTimeVariation * 2.0) - this.sourceTimeVariation;
		int start = Mathf.FloorToInt((this.sourceTime + num) * (float)this.sampleRate / (float)this.sourceChannels);
		GranularAudioClip.Grain grain = Pool.Get<GranularAudioClip.Grain>();
		grain.Init(this.sourceAudioData, start, this.grainAttackSamples, this.grainSustainSamples, this.grainReleaseSamples);
		this.grains.Add(grain);
		this.samplesUntilNextGrain = this.grainFrequencySamples;
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x0005E72C File Offset: 0x0005C92C
	private void CleanupFinishedGrains()
	{
		for (int i = this.grains.Count - 1; i >= 0; i--)
		{
			GranularAudioClip.Grain grain = this.grains[i];
			if (grain.finished)
			{
				Pool.Free<GranularAudioClip.Grain>(ref grain);
				this.grains.RemoveAt(i);
			}
		}
	}

	// Token: 0x02000169 RID: 361
	public class Grain
	{
		// Token: 0x040009D9 RID: 2521
		private float[] sourceData;

		// Token: 0x040009DA RID: 2522
		private int sourceDataLength;

		// Token: 0x040009DB RID: 2523
		private int startSample;

		// Token: 0x040009DC RID: 2524
		private int currentSample;

		// Token: 0x040009DD RID: 2525
		private int attackTimeSamples;

		// Token: 0x040009DE RID: 2526
		private int sustainTimeSamples;

		// Token: 0x040009DF RID: 2527
		private int releaseTimeSamples;

		// Token: 0x040009E0 RID: 2528
		private float gain;

		// Token: 0x040009E1 RID: 2529
		private float gainPerSampleAttack;

		// Token: 0x040009E2 RID: 2530
		private float gainPerSampleRelease;

		// Token: 0x040009E3 RID: 2531
		private int attackEndSample;

		// Token: 0x040009E4 RID: 2532
		private int releaseStartSample;

		// Token: 0x040009E5 RID: 2533
		private int endSample;

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000CEC RID: 3308 RVA: 0x0000BF37 File Offset: 0x0000A137
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x0005E7FC File Offset: 0x0005C9FC
		public void Init(float[] source, int start, int attack, int sustain, int release)
		{
			this.sourceData = source;
			this.sourceDataLength = this.sourceData.Length;
			this.startSample = start;
			this.currentSample = start;
			this.attackTimeSamples = attack;
			this.sustainTimeSamples = sustain;
			this.releaseTimeSamples = release;
			this.gainPerSampleAttack = 1f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -1f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x0005E8B0 File Offset: 0x0005CAB0
		public float GetSample()
		{
			int num = this.currentSample % this.sourceDataLength;
			if (num < 0)
			{
				num += this.sourceDataLength;
			}
			float num2 = this.sourceData[num];
			if (this.currentSample <= this.attackEndSample)
			{
				this.gain += this.gainPerSampleAttack;
			}
			else if (this.currentSample >= this.releaseStartSample)
			{
				this.gain += this.gainPerSampleRelease;
			}
			this.currentSample++;
			return num2 * this.gain;
		}
	}
}
