using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000156 RID: 342
public class AmbienceEmitter : MonoBehaviour, IClientComponent, IComparable<AmbienceEmitter>
{
	// Token: 0x0400092F RID: 2351
	public AmbienceDefinitionList baseAmbience;

	// Token: 0x04000930 RID: 2352
	public AmbienceDefinitionList stings;

	// Token: 0x04000931 RID: 2353
	public bool isStatic = true;

	// Token: 0x04000932 RID: 2354
	public bool followCamera;

	// Token: 0x04000933 RID: 2355
	public bool isBaseEmitter;

	// Token: 0x04000934 RID: 2356
	public bool active;

	// Token: 0x04000935 RID: 2357
	public float cameraDistance = float.PositiveInfinity;

	// Token: 0x04000936 RID: 2358
	public BoundingSphere boundingSphere;

	// Token: 0x04000937 RID: 2359
	public float crossfadeTime = 2f;

	// Token: 0x0400093A RID: 2362
	public Dictionary<AmbienceDefinition, float> nextStingTime = new Dictionary<AmbienceDefinition, float>();

	// Token: 0x0400093B RID: 2363
	public float deactivateTime = float.PositiveInfinity;

	// Token: 0x0400093C RID: 2364
	private Sound baseSound;

	// Token: 0x0400093D RID: 2365
	private SoundModulation.Modulator occlusionGain;

	// Token: 0x0400093E RID: 2366
	private float lastCrossfade;

	// Token: 0x0400093F RID: 2367
	private List<int> biomeReadings = new List<int>();

	// Token: 0x04000940 RID: 2368
	private List<int> topologyReadings = new List<int>();

	// Token: 0x04000941 RID: 2369
	private int readingsToKeep = 5;

	// Token: 0x04000942 RID: 2370
	private Vector3 lastPosition;

	// Token: 0x04000943 RID: 2371
	private Dictionary<int, int> readingOccurences = new Dictionary<int, int>();

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000C7B RID: 3195 RVA: 0x0000BA6F File Offset: 0x00009C6F
	// (set) Token: 0x06000C7C RID: 3196 RVA: 0x0000BA77 File Offset: 0x00009C77
	public TerrainTopology.Enum currentTopology { get; private set; }

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000C7D RID: 3197 RVA: 0x0000BA80 File Offset: 0x00009C80
	// (set) Token: 0x06000C7E RID: 3198 RVA: 0x0000BA88 File Offset: 0x00009C88
	public TerrainBiome.Enum currentBiome { get; private set; }

	// Token: 0x06000C7F RID: 3199 RVA: 0x0005BA9C File Offset: 0x00059C9C
	protected void Awake()
	{
		this.boundingSphere = new BoundingSphere(base.transform.position, 1f);
		if (!this.followCamera && this.isStatic && SingletonComponent<AmbienceManager>.Instance != null)
		{
			SingletonComponent<AmbienceManager>.Instance.AddEmitter(this);
		}
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x0000BA91 File Offset: 0x00009C91
	protected void OnEnable()
	{
		this.ReadAndUpdateEnvironment();
		if (!this.followCamera && !this.isStatic && SingletonComponent<AmbienceManager>.Instance != null)
		{
			SingletonComponent<AmbienceManager>.Instance.AddEmitter(this);
		}
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x0000BAC1 File Offset: 0x00009CC1
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (!this.followCamera && !this.isStatic && SingletonComponent<AmbienceManager>.Instance != null)
		{
			SingletonComponent<AmbienceManager>.Instance.EmitterLeaveRange(this);
			SingletonComponent<AmbienceManager>.Instance.RemoveEmitter(this);
		}
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x0000BAFE File Offset: 0x00009CFE
	protected void OnDestroy()
	{
		if (Application.isQuitting)
		{
			return;
		}
		if (!this.followCamera && this.isStatic && SingletonComponent<AmbienceManager>.Instance != null)
		{
			SingletonComponent<AmbienceManager>.Instance.EmitterLeaveRange(this);
			SingletonComponent<AmbienceManager>.Instance.RemoveEmitter(this);
		}
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x0000BB3B File Offset: 0x00009D3B
	public void FadeOut(bool fadeOut = true)
	{
		this.CrossfadeTo(null, this.crossfadeTime * 0.25f);
		this.deactivateTime = Time.time + this.crossfadeTime * 0.25f;
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x0000BB68 File Offset: 0x00009D68
	public bool IsFadingOut()
	{
		return this.deactivateTime != float.PositiveInfinity;
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x0000BB7A File Offset: 0x00009D7A
	public void Reset()
	{
		this.nextStingTime.Clear();
		this.deactivateTime = float.PositiveInfinity;
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x0005BAEC File Offset: 0x00059CEC
	public void DoUpdate()
	{
		if (this.occlusionGain != null)
		{
			float target = MainCamera.InEnvironment((EnvironmentType)19) ? 0.7f : 1f;
			this.occlusionGain.value = Mathf.MoveTowards(this.occlusionGain.value, target, Time.deltaTime);
		}
		if (Time.time >= this.deactivateTime)
		{
			SingletonComponent<AmbienceManager>.Instance.DeactivateEmitter(this);
		}
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x0000BB92 File Offset: 0x00009D92
	public void Tick()
	{
		this.ReadAndUpdateEnvironment();
		this.UpdateBaseSound();
		this.PlayStings();
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x0000BBA6 File Offset: 0x00009DA6
	public void UpdateCameraDistance()
	{
		this.cameraDistance = MainCamera.Distance(base.transform.position);
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x0005BB50 File Offset: 0x00059D50
	private void ReadAndUpdateEnvironment()
	{
		if (Vector3.SqrMagnitude(this.lastPosition - base.transform.position) > 1f)
		{
			this.boundingSphere.position = base.transform.position;
			this.ReadEnvironment();
			this.UpdateCurrentEnvironment();
		}
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x0005BBB4 File Offset: 0x00059DB4
	private void ReadEnvironment()
	{
		if (TerrainMeta.BiomeMap)
		{
			this.biomeReadings.Add(TerrainMeta.BiomeMap.GetBiomeMaxType(base.gameObject.transform.position, -1));
			if (this.biomeReadings.Count > this.readingsToKeep)
			{
				this.biomeReadings.RemoveAt(0);
			}
		}
		if (TerrainMeta.TopologyMap)
		{
			this.topologyReadings.Add(TerrainMeta.TopologyMap.GetTopology(base.gameObject.transform.position));
			if (this.topologyReadings.Count > this.readingsToKeep)
			{
				this.topologyReadings.RemoveAt(0);
			}
		}
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x0000BBBE File Offset: 0x00009DBE
	private void UpdateCurrentEnvironment()
	{
		this.currentBiome = this.GetMostCommonReading(this.biomeReadings);
		this.currentTopology = this.GetMostCommonReading(this.topologyReadings);
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x0005BC64 File Offset: 0x00059E64
	private int GetMostCommonReading(List<int> readings)
	{
		if (readings.Count > 0)
		{
			this.readingOccurences.Clear();
			int num = readings[0];
			for (int i = 0; i < readings.Count; i++)
			{
				int num2 = readings[i];
				if (this.readingOccurences.ContainsKey(num2))
				{
					Dictionary<int, int> dictionary = this.readingOccurences;
					int num3 = num2;
					int num4 = dictionary[num3];
					dictionary[num3] = num4 + 1;
					if (this.readingOccurences[num2] > this.readingOccurences[num])
					{
						num = num2;
					}
				}
				else
				{
					this.readingOccurences.Add(num2, 1);
				}
			}
			return num;
		}
		return 0;
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x0005BD00 File Offset: 0x00059F00
	private float GetScore(AmbienceDefinition definition, float rain, float snow, float wind, bool underground)
	{
		float num = 0f;
		if (TOD_Sky.Instance)
		{
			num += definition.time.Evaluate(TOD_Sky.Instance.Cycle.Hour);
		}
		if (definition.biomes != -1)
		{
			num -= 10f;
			if ((definition.biomes & this.currentBiome) != null)
			{
				num += 11f;
			}
		}
		if (definition.topologies != -1)
		{
			num -= 10f;
			if ((definition.topologies & this.currentTopology) != null)
			{
				num += 12f;
			}
		}
		if (definition.rain.min > 0f || definition.rain.max < 1f)
		{
			num -= 10f;
			if (rain >= definition.rain.min && rain <= definition.rain.max)
			{
				num += 11f;
			}
		}
		if (definition.snow.min > 0f || definition.snow.max < 1f)
		{
			num -= 10f;
			if (snow >= definition.snow.min && snow <= definition.snow.max)
			{
				num += 11f;
			}
		}
		if (definition.wind.min > 0f || definition.wind.max < 1f)
		{
			num -= 10f;
			if (wind >= definition.wind.min && wind <= definition.wind.max)
			{
				num += 11f;
			}
		}
		if (definition.useEnvironmentType)
		{
			num -= 10f;
			if (underground)
			{
				num += 110f;
			}
		}
		return num;
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0005BE9C File Offset: 0x0005A09C
	private void UpdateBaseSound()
	{
		if (Time.time > this.lastCrossfade + this.crossfadeTime)
		{
			SoundDefinition soundDefinition = this.GetBaseSound();
			float fadeTime = this.crossfadeTime;
			if (this.baseSound == null || this.baseSound.definition != soundDefinition)
			{
				this.CrossfadeTo(soundDefinition, fadeTime);
			}
		}
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x0005BEF4 File Offset: 0x0005A0F4
	private void StartSound(SoundDefinition def, float fadeTime)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		using (TimeWarning.New("AmbienceEmitter.StartSound", 0.1f))
		{
			this.baseSound = SoundManager.RequestSoundInstance(def, base.gameObject, default(Vector3), false);
			this.occlusionGain = null;
			if (!(this.baseSound == null))
			{
				this.baseSound.FadeInAndPlay(fadeTime);
				this.occlusionGain = this.baseSound.modulation.CreateModulator(SoundModulation.Parameter.Gain);
			}
		}
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x0005BF94 File Offset: 0x0005A194
	private void CrossfadeTo(SoundDefinition def, float fadeTime)
	{
		this.lastCrossfade = Time.time;
		if (this.baseSound != null)
		{
			this.baseSound.FadeOutAndRecycle(fadeTime);
			this.baseSound = null;
		}
		else
		{
			fadeTime = this.crossfadeTime * 0.75f;
		}
		if (def)
		{
			this.StartSound(def, fadeTime);
		}
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x0005BFF0 File Offset: 0x0005A1F0
	public SoundDefinition GetBaseSound()
	{
		SoundDefinition result;
		using (TimeWarning.New("AmbienceEmitter.GetBaseSound", 0.1f))
		{
			if (this.isBaseEmitter)
			{
				AmbienceZone ambienceZone = SingletonComponent<AmbienceManager>.Instance.CurrentAmbienceZone();
				if (ambienceZone != null)
				{
					return this.GetBaseSound(ambienceZone.baseAmbience);
				}
			}
			result = this.GetBaseSound(this.baseAmbience);
		}
		return result;
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x0005C064 File Offset: 0x0005A264
	private SoundDefinition GetBaseSound(AmbienceDefinitionList ambienceDefinitionList)
	{
		if (ambienceDefinitionList == null || ambienceDefinitionList.defs == null)
		{
			return null;
		}
		float num = 0f;
		AmbienceDefinition ambienceDefinition = ambienceDefinitionList.defs[0];
		float rain = Climate.GetRain(base.transform.position);
		float snow = Climate.GetSnow(base.transform.position);
		float wind = Climate.GetWind(base.transform.position);
		for (int i = 0; i < ambienceDefinitionList.defs.Count; i++)
		{
			float score = this.GetScore(ambienceDefinitionList.defs[i], rain, snow, wind, MainCamera.InEnvironment(EnvironmentType.Underground));
			if (score > num)
			{
				num = score;
				ambienceDefinition = ambienceDefinitionList.defs[i];
			}
		}
		if (ambienceDefinition.sounds.Count > 0)
		{
			return ambienceDefinition.sounds[0];
		}
		return null;
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x0005C13C File Offset: 0x0005A33C
	public void PlayStings()
	{
		if (this.isBaseEmitter)
		{
			AmbienceZone ambienceZone = SingletonComponent<AmbienceManager>.Instance.CurrentAmbienceZone();
			if (ambienceZone != null)
			{
				this.PlayStings(ambienceZone.stings);
				this.ResetStingTimes(this.stings);
				return;
			}
		}
		this.PlayStings(this.stings);
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x0005C18C File Offset: 0x0005A38C
	private void PlayStings(AmbienceDefinitionList ambienceDefinitionList)
	{
		if (ambienceDefinitionList == null || ambienceDefinitionList.defs == null || ambienceDefinitionList.defs.Count == 0)
		{
			return;
		}
		float rain = Climate.GetRain(base.transform.position);
		float snow = Climate.GetSnow(base.transform.position);
		float wind = Climate.GetWind(base.transform.position);
		for (int i = 0; i < ambienceDefinitionList.defs.Count; i++)
		{
			AmbienceDefinition ambienceDefinition = ambienceDefinitionList.defs[i];
			if (!this.nextStingTime.ContainsKey(ambienceDefinition))
			{
				this.nextStingTime.Add(ambienceDefinition, Time.time + Random.Range(0f, ambienceDefinition.stingFrequency.max));
			}
			if (this.nextStingTime[ambienceDefinition] <= Time.time)
			{
				this.nextStingTime[ambienceDefinition] = this.GetNextStingTime(ambienceDefinition);
				if (rain >= ambienceDefinition.rain.min && rain <= ambienceDefinition.rain.max && snow >= ambienceDefinition.snow.min && snow <= ambienceDefinition.snow.max && wind >= ambienceDefinition.wind.min && wind <= ambienceDefinition.wind.max && (!TOD_Sky.Instance || ambienceDefinition.time.Evaluate(TOD_Sky.Instance.Cycle.Hour) >= 0f) && (ambienceDefinition.biomes == -1 || (ambienceDefinition.biomes & this.currentBiome) != null) && (ambienceDefinition.topologies == -1 || (ambienceDefinition.topologies & this.currentTopology) != null))
				{
					bool flag = MainCamera.InEnvironment(EnvironmentType.Underground);
					if ((!ambienceDefinition.useEnvironmentType || flag) && (ambienceDefinition.useEnvironmentType || !flag))
					{
						SoundManager.PlayOneshot(ambienceDefinition.sounds[Random.Range(0, ambienceDefinition.sounds.Count)], base.gameObject, false, default(Vector3));
					}
				}
			}
		}
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x0005C3A4 File Offset: 0x0005A5A4
	private void ResetStingTimes(AmbienceDefinitionList ambienceDefinitionList)
	{
		for (int i = 0; i < ambienceDefinitionList.defs.Count; i++)
		{
			AmbienceDefinition ambienceDefinition = ambienceDefinitionList.defs[i];
			if (!this.nextStingTime.ContainsKey(ambienceDefinition))
			{
				this.nextStingTime.Add(ambienceDefinition, Time.time + Random.Range(0f, ambienceDefinition.stingFrequency.max));
			}
			this.nextStingTime[ambienceDefinition] = Time.time + Random.Range(0f, ambienceDefinition.stingFrequency.max);
		}
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x0000BBE4 File Offset: 0x00009DE4
	public float GetNextStingTime(AmbienceDefinition sting)
	{
		return Time.time + Random.Range(sting.stingFrequency.min, sting.stingFrequency.max);
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x0000BC07 File Offset: 0x00009E07
	public int CompareTo(AmbienceEmitter other)
	{
		return this.cameraDistance.CompareTo(other.cameraDistance);
	}
}
