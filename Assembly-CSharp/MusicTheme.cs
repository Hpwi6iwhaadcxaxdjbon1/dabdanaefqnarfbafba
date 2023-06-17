using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000175 RID: 373
[CreateAssetMenu(menuName = "Rust/MusicTheme")]
public class MusicTheme : ScriptableObject
{
	// Token: 0x04000A30 RID: 2608
	[Header("Basic info")]
	public float tempo = 80f;

	// Token: 0x04000A31 RID: 2609
	public int intensityHoldBars = 4;

	// Token: 0x04000A32 RID: 2610
	public int lengthInBars;

	// Token: 0x04000A33 RID: 2611
	[Header("Playback restrictions")]
	public bool canPlayInMenus = true;

	// Token: 0x04000A34 RID: 2612
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange rain = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x04000A35 RID: 2613
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange wind = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x04000A36 RID: 2614
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange snow = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x04000A37 RID: 2615
	[InspectorFlags]
	public TerrainBiome.Enum biomes = -1;

	// Token: 0x04000A38 RID: 2616
	[InspectorFlags]
	public TerrainTopology.Enum topologies = -1;

	// Token: 0x04000A39 RID: 2617
	public AnimationCurve time = AnimationCurve.Linear(0f, 0f, 24f, 0f);

	// Token: 0x04000A3A RID: 2618
	[Header("Clip data")]
	public List<MusicTheme.PositionedClip> clips = new List<MusicTheme.PositionedClip>();

	// Token: 0x04000A3B RID: 2619
	public List<MusicTheme.Layer> layers = new List<MusicTheme.Layer>();

	// Token: 0x04000A3C RID: 2620
	private Dictionary<int, List<MusicTheme.PositionedClip>> activeClips = new Dictionary<int, List<MusicTheme.PositionedClip>>();

	// Token: 0x04000A3D RID: 2621
	private List<AudioClip> firstAudioClips = new List<AudioClip>();

	// Token: 0x04000A3E RID: 2622
	private Dictionary<AudioClip, bool> audioClipDict = new Dictionary<AudioClip, bool>();

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06000D34 RID: 3380 RVA: 0x0000C2C2 File Offset: 0x0000A4C2
	public int layerCount
	{
		get
		{
			return this.layers.Count;
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x06000D35 RID: 3381 RVA: 0x0000C2CF File Offset: 0x0000A4CF
	public int samplesPerBar
	{
		get
		{
			return MusicUtil.BarsToSamples(this.tempo, 1f, 44100);
		}
	}

	// Token: 0x06000D36 RID: 3382 RVA: 0x00060128 File Offset: 0x0005E328
	private void OnValidate()
	{
		this.audioClipDict.Clear();
		this.activeClips.Clear();
		this.UpdateLengthInBars();
		for (int i = 0; i < this.clips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.clips[i];
			int num = this.ActiveClipCollectionID(positionedClip.startingBar - 8);
			int num2 = this.ActiveClipCollectionID(positionedClip.endingBar);
			for (int j = num; j <= num2; j++)
			{
				if (!this.activeClips.ContainsKey(j))
				{
					this.activeClips.Add(j, new List<MusicTheme.PositionedClip>());
				}
				if (!this.activeClips[j].Contains(positionedClip))
				{
					this.activeClips[j].Add(positionedClip);
				}
			}
			if (positionedClip.musicClip != null)
			{
				AudioClip audioClip = positionedClip.musicClip.audioClip;
				if (!this.audioClipDict.ContainsKey(audioClip))
				{
					this.audioClipDict.Add(audioClip, true);
				}
				if (positionedClip.startingBar < 8 && !this.firstAudioClips.Contains(audioClip))
				{
					this.firstAudioClips.Add(audioClip);
				}
				positionedClip.musicClip.lengthInBarsWithTail = Mathf.CeilToInt(MusicUtil.SecondsToBars(this.tempo, (double)positionedClip.musicClip.audioClip.length));
			}
		}
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00060270 File Offset: 0x0005E470
	public List<MusicTheme.PositionedClip> GetActiveClipsForBar(int bar)
	{
		int num = this.ActiveClipCollectionID(bar);
		if (!this.activeClips.ContainsKey(num))
		{
			return null;
		}
		return this.activeClips[num];
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x0000C2E6 File Offset: 0x0000A4E6
	private int ActiveClipCollectionID(int bar)
	{
		return Mathf.FloorToInt(Mathf.Max((float)(bar / 4), 0f));
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x0000C2FB File Offset: 0x0000A4FB
	public MusicTheme.Layer LayerById(int id)
	{
		if (this.layers.Count <= id)
		{
			return null;
		}
		return this.layers[id];
	}

	// Token: 0x06000D3A RID: 3386 RVA: 0x000602A4 File Offset: 0x0005E4A4
	public void AddLayer()
	{
		MusicTheme.Layer layer = new MusicTheme.Layer();
		layer.name = "layer " + this.layers.Count;
		this.layers.Add(layer);
	}

	// Token: 0x06000D3B RID: 3387 RVA: 0x000602E4 File Offset: 0x0005E4E4
	private void UpdateLengthInBars()
	{
		int num = 0;
		for (int i = 0; i < this.clips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.clips[i];
			if (!(positionedClip.musicClip == null))
			{
				int num2 = positionedClip.startingBar + positionedClip.musicClip.lengthInBars;
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		this.lengthInBars = num;
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x00060344 File Offset: 0x0005E544
	public bool CanPlayInEnvironment(int currentBiome, int currentTopology, float currentRain, float currentSnow, float currentWind)
	{
		return (!TOD_Sky.Instance || this.time.Evaluate(TOD_Sky.Instance.Cycle.Hour) >= 0f) && (this.biomes == -1 || (this.biomes & currentBiome) != null) && (this.topologies == -1 || (this.topologies & currentTopology) == null) && ((this.rain.min <= 0f && this.rain.max >= 1f) || currentRain >= this.rain.min) && currentRain <= this.rain.max && ((this.snow.min <= 0f && this.snow.max >= 1f) || currentSnow >= this.snow.min) && currentSnow <= this.snow.max && ((this.wind.min <= 0f && this.wind.max >= 1f) || currentWind >= this.wind.min) && currentWind <= this.wind.max;
	}

	// Token: 0x06000D3D RID: 3389 RVA: 0x00060478 File Offset: 0x0005E678
	public bool FirstClipsLoaded()
	{
		for (int i = 0; i < this.firstAudioClips.Count; i++)
		{
			if (this.firstAudioClips[i].loadState != 2)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000D3E RID: 3390 RVA: 0x0000C319 File Offset: 0x0000A519
	public bool ContainsAudioClip(AudioClip clip)
	{
		return this.audioClipDict.ContainsKey(clip);
	}

	// Token: 0x02000176 RID: 374
	[Serializable]
	public class Layer
	{
		// Token: 0x04000A3F RID: 2623
		public string name = "layer";
	}

	// Token: 0x02000177 RID: 375
	[Serializable]
	public class PositionedClip
	{
		// Token: 0x04000A40 RID: 2624
		public MusicTheme theme;

		// Token: 0x04000A41 RID: 2625
		public MusicClip musicClip;

		// Token: 0x04000A42 RID: 2626
		public int startingBar;

		// Token: 0x04000A43 RID: 2627
		public int layerId;

		// Token: 0x04000A44 RID: 2628
		public float minIntensity;

		// Token: 0x04000A45 RID: 2629
		public float maxIntensity = 1f;

		// Token: 0x04000A46 RID: 2630
		public bool allowFadeIn = true;

		// Token: 0x04000A47 RID: 2631
		public bool allowFadeOut = true;

		// Token: 0x04000A48 RID: 2632
		public float fadeInTime = 1f;

		// Token: 0x04000A49 RID: 2633
		public float fadeOutTime = 0.5f;

		// Token: 0x04000A4A RID: 2634
		public float intensityReduction;

		// Token: 0x04000A4B RID: 2635
		public int jumpBarCount;

		// Token: 0x04000A4C RID: 2636
		public float jumpMinimumIntensity = 0.5f;

		// Token: 0x04000A4D RID: 2637
		public float jumpMaximumIntensity = 0.5f;

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000D41 RID: 3393 RVA: 0x0000C33A File Offset: 0x0000A53A
		public int endingBar
		{
			get
			{
				if (!(this.musicClip == null))
				{
					return this.startingBar + this.musicClip.lengthInBarsWithTail;
				}
				return this.startingBar;
			}
		}

		// Token: 0x06000D42 RID: 3394 RVA: 0x0000C363 File Offset: 0x0000A563
		public bool CanPlay(float intensity)
		{
			return (intensity > this.minIntensity || (this.minIntensity == 0f && intensity == 0f)) && intensity <= this.maxIntensity;
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000D43 RID: 3395 RVA: 0x0000C391 File Offset: 0x0000A591
		public bool isControlClip
		{
			get
			{
				return this.musicClip == null;
			}
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x00060584 File Offset: 0x0005E784
		public void CopySettingsFrom(MusicTheme.PositionedClip otherClip)
		{
			if (this.isControlClip != otherClip.isControlClip)
			{
				return;
			}
			if (otherClip == this)
			{
				return;
			}
			this.allowFadeIn = otherClip.allowFadeIn;
			this.fadeInTime = otherClip.fadeInTime;
			this.allowFadeOut = otherClip.allowFadeOut;
			this.fadeOutTime = otherClip.fadeOutTime;
			this.maxIntensity = otherClip.maxIntensity;
			this.minIntensity = otherClip.minIntensity;
			this.intensityReduction = otherClip.intensityReduction;
		}
	}

	// Token: 0x02000178 RID: 376
	[Serializable]
	public class ValueRange
	{
		// Token: 0x04000A4E RID: 2638
		public float min;

		// Token: 0x04000A4F RID: 2639
		public float max;

		// Token: 0x06000D46 RID: 3398 RVA: 0x0000C39F File Offset: 0x0000A59F
		public ValueRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
