using System;
using System.Collections.Generic;
using Facepunch;
using JSON;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class EngineAudioClip : MonoBehaviour, IClientComponent
{
	// Token: 0x0400098D RID: 2445
	public AudioClip granularClip;

	// Token: 0x0400098E RID: 2446
	public AudioClip accelerationClip;

	// Token: 0x0400098F RID: 2447
	public TextAsset accelerationCyclesJson;

	// Token: 0x04000990 RID: 2448
	public List<EngineAudioClip.EngineCycle> accelerationCycles = new List<EngineAudioClip.EngineCycle>();

	// Token: 0x04000991 RID: 2449
	public List<EngineAudioClip.EngineCycleBucket> cycleBuckets = new List<EngineAudioClip.EngineCycleBucket>();

	// Token: 0x04000992 RID: 2450
	public Dictionary<int, EngineAudioClip.EngineCycleBucket> accelerationCyclesByRPM = new Dictionary<int, EngineAudioClip.EngineCycleBucket>();

	// Token: 0x04000993 RID: 2451
	public Dictionary<int, int> rpmBucketLookup = new Dictionary<int, int>();

	// Token: 0x04000994 RID: 2452
	public int sampleRate = 44100;

	// Token: 0x04000995 RID: 2453
	public int samplesUntilNextGrain;

	// Token: 0x04000996 RID: 2454
	public int lastCycleId;

	// Token: 0x04000997 RID: 2455
	public List<EngineAudioClip.Grain> grains = new List<EngineAudioClip.Grain>();

	// Token: 0x04000998 RID: 2456
	public int currentRPM;

	// Token: 0x04000999 RID: 2457
	public int targetRPM = 1500;

	// Token: 0x0400099A RID: 2458
	public int minRPM;

	// Token: 0x0400099B RID: 2459
	public int maxRPM;

	// Token: 0x0400099C RID: 2460
	public int cyclePadding;

	// Token: 0x0400099D RID: 2461
	[Range(0f, 1f)]
	public float RPMControl;

	// Token: 0x0400099E RID: 2462
	public AudioSource source;

	// Token: 0x0400099F RID: 2463
	private Random random = new Random();

	// Token: 0x040009A0 RID: 2464
	private float[] accelerationAudioData;

	// Token: 0x040009A1 RID: 2465
	private EngineAudioClip.EngineCycle currentCycle;

	// Token: 0x040009A2 RID: 2466
	private bool audioDataLoaded;

	// Token: 0x040009A3 RID: 2467
	public float rpmLerpSpeed = 0.025f;

	// Token: 0x040009A4 RID: 2468
	public float rpmLerpSpeedDown = 0.01f;

	// Token: 0x06000CCC RID: 3276 RVA: 0x0000BE3B File Offset: 0x0000A03B
	private int GetBucketRPM(int RPM)
	{
		return Mathf.RoundToInt((float)(RPM / 25)) * 25;
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x0000BE4A File Offset: 0x0000A04A
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		if (!this.audioDataLoaded)
		{
			this.accelerationClip.LoadAudioData();
		}
		this.SetupRPMBuckets();
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0005DC04 File Offset: 0x0005BE04
	private void Update()
	{
		if (!this.audioDataLoaded && this.accelerationClip.loadState == 2)
		{
			this.sampleRate = this.accelerationClip.frequency;
			this.accelerationAudioData = new float[this.accelerationClip.samples * this.accelerationClip.channels];
			this.accelerationClip.GetData(this.accelerationAudioData, 0);
			this.accelerationClip.UnloadAudioData();
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
			}
		}
		else if (!this.source.isPlaying && this.audioDataLoaded)
		{
			this.source.Play();
		}
		this.targetRPM = Mathf.RoundToInt(Mathf.Lerp((float)this.minRPM, (float)this.maxRPM, this.RPMControl));
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x0005DD24 File Offset: 0x0005BF24
	private void UpdateRPM()
	{
		if (this.currentCycle != null)
		{
			float num = (float)(this.currentCycle.endSample - this.currentCycle.startSample);
			if (this.targetRPM > this.currentRPM)
			{
				num *= this.rpmLerpSpeed;
			}
			else
			{
				num *= this.rpmLerpSpeedDown;
			}
			this.currentRPM = Mathf.RoundToInt(Mathf.MoveTowards((float)this.currentRPM, (float)this.targetRPM, num));
		}
		this.currentRPM = Mathf.RoundToInt((float)Mathf.Clamp(this.currentRPM, this.minRPM, this.maxRPM));
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x0005DDB8 File Offset: 0x0005BFB8
	private void InitAudioClip()
	{
		int num = 1;
		int num2 = 1;
		AudioSettings.GetDSPBufferSize(ref num, ref num2);
		this.granularClip = AudioClip.Create(this.accelerationClip.name + " (granular)", num, this.accelerationClip.channels, this.sampleRate, true, new AudioClip.PCMReaderCallback(this.OnAudioRead));
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x0005DE14 File Offset: 0x0005C014
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

	// Token: 0x06000CD2 RID: 3282 RVA: 0x0005DE88 File Offset: 0x0005C088
	private void SpawnGrain()
	{
		this.UpdateRPM();
		int num = this.rpmBucketLookup[this.currentRPM];
		EngineAudioClip.EngineCycleBucket engineCycleBucket = this.cycleBuckets[num];
		this.currentCycle = engineCycleBucket.GetCycle(this.random, this.lastCycleId);
		this.lastCycleId = this.currentCycle.id;
		EngineAudioClip.Grain grain = Pool.Get<EngineAudioClip.Grain>();
		grain.Init(this.accelerationAudioData, this.currentCycle, this.cyclePadding);
		this.grains.Add(grain);
		this.samplesUntilNextGrain = this.currentCycle.endSample - this.currentCycle.startSample - this.cyclePadding;
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0005DF34 File Offset: 0x0005C134
	private void CleanupFinishedGrains()
	{
		for (int i = this.grains.Count - 1; i >= 0; i--)
		{
			EngineAudioClip.Grain grain = this.grains[i];
			if (grain.finished)
			{
				Pool.Free<EngineAudioClip.Grain>(ref grain);
				this.grains.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x0005DF84 File Offset: 0x0005C184
	private void SetupRPMBuckets()
	{
		for (int i = 0; i < this.accelerationCycles.Count; i++)
		{
			EngineAudioClip.EngineCycle engineCycle = this.accelerationCycles[i];
			int bucketRPM = this.GetBucketRPM(engineCycle.RPM);
			if (this.accelerationCyclesByRPM.ContainsKey(bucketRPM))
			{
				this.accelerationCyclesByRPM[bucketRPM].Add(engineCycle);
			}
			else
			{
				EngineAudioClip.EngineCycleBucket engineCycleBucket = new EngineAudioClip.EngineCycleBucket(bucketRPM);
				this.cycleBuckets.Add(engineCycleBucket);
				engineCycleBucket.Add(engineCycle);
				this.accelerationCyclesByRPM.Add(bucketRPM, engineCycleBucket);
			}
		}
		this.rpmBucketLookup.Clear();
		for (int j = this.minRPM; j <= this.maxRPM; j++)
		{
			int num = 0;
			int num2 = int.MaxValue;
			for (int k = 0; k < this.cycleBuckets.Count; k++)
			{
				EngineAudioClip.EngineCycleBucket engineCycleBucket2 = this.cycleBuckets[k];
				int num3 = Mathf.Abs(j - engineCycleBucket2.RPM);
				if (num3 < num2)
				{
					num = k;
					num2 = num3;
				}
			}
			this.rpmBucketLookup.Add(j, num);
		}
	}

	// Token: 0x02000162 RID: 354
	[Serializable]
	public class EngineCycle
	{
		// Token: 0x040009A5 RID: 2469
		public int RPM;

		// Token: 0x040009A6 RID: 2470
		public int startSample;

		// Token: 0x040009A7 RID: 2471
		public int endSample;

		// Token: 0x040009A8 RID: 2472
		public float period;

		// Token: 0x040009A9 RID: 2473
		public int id;

		// Token: 0x06000CD6 RID: 3286 RVA: 0x0000BE72 File Offset: 0x0000A072
		public EngineCycle(int RPM, int startSample, int endSample, float period, int id)
		{
			this.RPM = RPM;
			this.startSample = startSample;
			this.endSample = endSample;
			this.period = period;
			this.id = id;
		}
	}

	// Token: 0x02000163 RID: 355
	public class EngineCycleBucket
	{
		// Token: 0x040009AA RID: 2474
		public int RPM;

		// Token: 0x040009AB RID: 2475
		public List<EngineAudioClip.EngineCycle> cycles = new List<EngineAudioClip.EngineCycle>();

		// Token: 0x040009AC RID: 2476
		public List<int> remainingCycles = new List<int>();

		// Token: 0x06000CD7 RID: 3287 RVA: 0x0000BE9F File Offset: 0x0000A09F
		public EngineCycleBucket(int RPM)
		{
			this.RPM = RPM;
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x0005E114 File Offset: 0x0005C314
		public EngineAudioClip.EngineCycle GetCycle(Random random, int lastCycleId)
		{
			if (this.remainingCycles.Count == 0)
			{
				this.ResetRemainingCycles(random);
			}
			int num = Extensions.Pop<int>(this.remainingCycles);
			if (this.cycles[num].id == lastCycleId)
			{
				if (this.remainingCycles.Count == 0)
				{
					this.ResetRemainingCycles(random);
				}
				num = Extensions.Pop<int>(this.remainingCycles);
			}
			return this.cycles[num];
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x0005E184 File Offset: 0x0005C384
		private void ResetRemainingCycles(Random random)
		{
			for (int i = 0; i < this.cycles.Count; i++)
			{
				this.remainingCycles.Add(i);
			}
			ListEx.Shuffle<int>(this.remainingCycles, (uint)random.Next());
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x0000BEC4 File Offset: 0x0000A0C4
		public void Add(EngineAudioClip.EngineCycle cycle)
		{
			if (!this.cycles.Contains(cycle))
			{
				this.cycles.Add(cycle);
			}
		}
	}

	// Token: 0x02000164 RID: 356
	public class Grain
	{
		// Token: 0x040009AD RID: 2477
		private float[] sourceData;

		// Token: 0x040009AE RID: 2478
		private int startSample;

		// Token: 0x040009AF RID: 2479
		private int currentSample;

		// Token: 0x040009B0 RID: 2480
		private int attackTimeSamples;

		// Token: 0x040009B1 RID: 2481
		private int sustainTimeSamples;

		// Token: 0x040009B2 RID: 2482
		private int releaseTimeSamples;

		// Token: 0x040009B3 RID: 2483
		private float gain;

		// Token: 0x040009B4 RID: 2484
		private float gainPerSampleAttack;

		// Token: 0x040009B5 RID: 2485
		private float gainPerSampleRelease;

		// Token: 0x040009B6 RID: 2486
		private int attackEndSample;

		// Token: 0x040009B7 RID: 2487
		private int releaseStartSample;

		// Token: 0x040009B8 RID: 2488
		private int endSample;

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000CDB RID: 3291 RVA: 0x0000BEE0 File Offset: 0x0000A0E0
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x0005E1C4 File Offset: 0x0005C3C4
		public void Init(float[] source, EngineAudioClip.EngineCycle cycle, int cyclePadding)
		{
			this.sourceData = source;
			this.startSample = cycle.startSample - cyclePadding;
			this.currentSample = this.startSample;
			this.attackTimeSamples = cyclePadding;
			this.sustainTimeSamples = cycle.endSample - cycle.startSample;
			this.releaseTimeSamples = cyclePadding;
			this.gainPerSampleAttack = 1f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -1f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x0005E280 File Offset: 0x0005C480
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
				if (this.gain > 0.8f)
				{
					this.gain = 0.8f;
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
	}
}
