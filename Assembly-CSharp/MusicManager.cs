using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000172 RID: 370
public class MusicManager : SingletonComponent<MusicManager>, IClientComponent
{
	// Token: 0x04000A08 RID: 2568
	public AudioMixerGroup mixerGroup;

	// Token: 0x04000A09 RID: 2569
	public List<MusicTheme> themes;

	// Token: 0x04000A0A RID: 2570
	public MusicTheme currentTheme;

	// Token: 0x04000A0B RID: 2571
	public List<AudioSource> sources = new List<AudioSource>();

	// Token: 0x04000A0C RID: 2572
	public double nextMusic;

	// Token: 0x04000A0D RID: 2573
	public double nextMusicFromIntensityRaise;

	// Token: 0x04000A0E RID: 2574
	[Range(0f, 1f)]
	public float intensity;

	// Token: 0x04000A0F RID: 2575
	public Dictionary<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData> clipPlaybackData = new Dictionary<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData>();

	// Token: 0x04000A10 RID: 2576
	public int holdIntensityUntilBar;

	// Token: 0x04000A11 RID: 2577
	public bool musicPlaying;

	// Token: 0x04000A12 RID: 2578
	public bool loadingFirstClips;

	// Token: 0x04000A13 RID: 2579
	public MusicTheme nextTheme;

	// Token: 0x04000A14 RID: 2580
	public double lastClipUpdate;

	// Token: 0x04000A15 RID: 2581
	public float clipUpdateInterval = 0.1f;

	// Token: 0x04000A16 RID: 2582
	public double themeStartTime;

	// Token: 0x04000A17 RID: 2583
	public int lastActiveClipRefresh = -10;

	// Token: 0x04000A18 RID: 2584
	public int activeClipRefreshInterval = 1;

	// Token: 0x04000A19 RID: 2585
	public bool forceThemeChange;

	// Token: 0x04000A1A RID: 2586
	public float randomIntensityJumpChance;

	// Token: 0x04000A1B RID: 2587
	public int clipScheduleBarsEarly = 1;

	// Token: 0x04000A1C RID: 2588
	public List<MusicTheme.PositionedClip> activeClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x04000A1D RID: 2589
	public List<MusicTheme.PositionedClip> activeMusicClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x04000A1E RID: 2590
	public List<MusicTheme.PositionedClip> activeControlClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x04000A1F RID: 2591
	public List<MusicZone> currentMusicZones = new List<MusicZone>();

	// Token: 0x04000A20 RID: 2592
	public int currentBar;

	// Token: 0x04000A21 RID: 2593
	public int barOffset;

	// Token: 0x04000A22 RID: 2594
	private AudioSource syncSource;

	// Token: 0x04000A23 RID: 2595
	private bool needsResync;

	// Token: 0x04000A24 RID: 2596
	private int fadingClipCount;

	// Token: 0x04000A25 RID: 2597
	private MusicClipLoader clipLoader = new MusicClipLoader();

	// Token: 0x04000A26 RID: 2598
	private List<MusicTheme> validThemes = new List<MusicTheme>();

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x06000D0F RID: 3343 RVA: 0x0000C18A File Offset: 0x0000A38A
	public double currentThemeTime
	{
		get
		{
			return AudioSettings.dspTime - this.themeStartTime;
		}
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000D10 RID: 3344 RVA: 0x0000C198 File Offset: 0x0000A398
	public int themeBar
	{
		get
		{
			return this.currentBar + this.barOffset;
		}
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x0000C1A7 File Offset: 0x0000A3A7
	protected override void Awake()
	{
		base.Awake();
		this.ShuffleThemes();
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x0005EF64 File Offset: 0x0005D164
	public void StartMusic()
	{
		if (!Music.enabled || Audio.musicvolume < 0.01f)
		{
			return;
		}
		MusicZone musicZone = this.CurrentMusicZone();
		this.currentTheme = this.GetThemeToPlay(musicZone);
		if (this.currentTheme == null)
		{
			this.nextMusic += 10.0;
			this.nextMusicFromIntensityRaise += 10.0;
			return;
		}
		if (musicZone != null)
		{
			musicZone.themes.Remove(this.currentTheme);
			musicZone.themes.Add(this.currentTheme);
		}
		else
		{
			this.themes.Remove(this.currentTheme);
			this.themes.Add(this.currentTheme);
		}
		this.musicPlaying = true;
		this.loadingFirstClips = true;
		this.holdIntensityUntilBar = 0;
		this.lastActiveClipRefresh = -10;
		this.barOffset = 0;
		this.syncSource = null;
		foreach (KeyValuePair<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData> keyValuePair in this.clipPlaybackData)
		{
			MusicManager.ClipPlaybackData value = keyValuePair.Value;
			Pool.Free<MusicManager.ClipPlaybackData>(ref value);
		}
		this.clipPlaybackData.Clear();
		this.themeStartTime = AudioSettings.dspTime + (double)this.clipUpdateInterval;
		this.UpdateCurrentBar();
		this.UpdateActiveClips();
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x0005F0C8 File Offset: 0x0005D2C8
	public MusicTheme GetThemeToPlay(MusicZone currentMusicZone)
	{
		List<MusicTheme> list = this.themes;
		if (currentMusicZone != null)
		{
			list = currentMusicZone.themes;
		}
		this.validThemes.Clear();
		if (!LevelManager.isLoaded || !MainCamera.isValid)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].canPlayInMenus)
				{
					this.validThemes.Add(list[i]);
				}
			}
		}
		else
		{
			int currentBiome = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiomeMaxType(MainCamera.position, -1) : 2;
			int currentTopology = TerrainMeta.TopologyMap ? TerrainMeta.TopologyMap.GetTopology(MainCamera.position) : 1;
			float rain = Climate.GetRain(MainCamera.position);
			float snow = Climate.GetSnow(MainCamera.position);
			float wind = Climate.GetWind(MainCamera.position);
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].CanPlayInEnvironment(currentBiome, currentTopology, rain, snow, wind))
				{
					this.validThemes.Add(list[j]);
				}
			}
		}
		if (this.validThemes.Count == 0)
		{
			return null;
		}
		return this.validThemes[0];
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x0005F1F8 File Offset: 0x0005D3F8
	private void Update()
	{
		if (!Music.enabled || Audio.musicvolume < 0.01f)
		{
			return;
		}
		this.UpdateCurrentBar();
		if (this.musicPlaying)
		{
			this.UpdateBarJumpClips();
		}
		if (this.musicPlaying && this.currentBar >= this.lastActiveClipRefresh + this.activeClipRefreshInterval)
		{
			this.UpdateActiveClips();
		}
		if (this.currentTheme != null && this.themeBar >= this.currentTheme.lengthInBars && !this.loadingFirstClips)
		{
			this.StopMusic();
		}
		if (!this.musicPlaying && AudioSettings.dspTime > this.nextMusic)
		{
			MusicZone musicZone = this.CurrentMusicZone();
			if (musicZone != null && musicZone.suppressAutomaticMusic && !this.loadingFirstClips)
			{
				this.nextMusic = AudioSettings.dspTime + (double)Random.Range(60f, 120f);
			}
			else
			{
				this.StartMusic();
			}
		}
		if (this.musicPlaying)
		{
			this.HandleMusicPlayback();
			this.clipLoader.Update();
		}
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x0000C1B5 File Offset: 0x0000A3B5
	private void UpdateCurrentBar()
	{
		this.currentBar = ((this.currentTheme == null) ? 0 : Mathf.FloorToInt(MusicUtil.SecondsToBars(this.currentTheme.tempo, this.currentThemeTime)));
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x0005F2F4 File Offset: 0x0005D4F4
	private void DoBarJump(int offset)
	{
		this.barOffset += offset;
		foreach (KeyValuePair<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData> keyValuePair in this.clipPlaybackData)
		{
			MusicTheme.PositionedClip key = keyValuePair.Key;
			MusicManager.ClipPlaybackData value = keyValuePair.Value;
			if (key.startingBar >= this.themeBar)
			{
				value.isActive = false;
				value.source = null;
			}
		}
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x0005F378 File Offset: 0x0005D578
	private void HandleMusicPlayback()
	{
		if (this.loadingFirstClips)
		{
			this.themeStartTime = AudioSettings.dspTime + (double)this.clipUpdateInterval;
		}
		if (this.loadingFirstClips && this.currentTheme.FirstClipsLoaded())
		{
			this.UpdateClips();
			this.nextMusic = AudioSettings.dspTime + MusicUtil.BarsToSeconds(this.currentTheme.tempo, (float)this.currentTheme.lengthInBars);
			this.loadingFirstClips = false;
			return;
		}
		if (!this.loadingFirstClips)
		{
			if (AudioSettings.dspTime > this.lastClipUpdate + (double)this.clipUpdateInterval)
			{
				this.UpdateClips();
			}
			this.DoClipFades();
			this.ResyncClips();
		}
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x0005F41C File Offset: 0x0005D61C
	private void DoClipFades()
	{
		if (this.fadingClipCount <= 0)
		{
			return;
		}
		for (int i = 0; i < this.activeMusicClips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.activeMusicClips[i];
			MusicManager.ClipPlaybackData playbackData = this.GetPlaybackData(positionedClip);
			if (playbackData.isActive && !(playbackData.source == null))
			{
				if (playbackData.fadingIn)
				{
					double num = MusicUtil.BarsToSeconds(this.currentTheme.tempo, positionedClip.fadeInTime);
					double num2 = (AudioSettings.dspTime - playbackData.fadeStarted) / num;
					playbackData.source.volume = Mathf.Lerp(0f, 1f, (float)num2);
					if (playbackData.source.volume == 1f)
					{
						playbackData.fadingIn = false;
						this.fadingClipCount--;
					}
				}
				else if (playbackData.fadingOut)
				{
					double num3 = MusicUtil.BarsToSeconds(this.currentTheme.tempo, positionedClip.fadeOutTime);
					double num4 = (AudioSettings.dspTime - playbackData.fadeStarted) / num3;
					playbackData.source.volume = Mathf.Lerp(1f, 0f, (float)num4);
					if (playbackData.source.volume == 0f)
					{
						playbackData.source.Stop();
						playbackData.source = null;
						playbackData.fadingOut = false;
						playbackData.isActive = false;
						this.fadingClipCount--;
					}
				}
			}
		}
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x0005F58C File Offset: 0x0005D78C
	private AudioSource GetFreeAudioSource()
	{
		for (int i = 0; i < this.sources.Count; i++)
		{
			if (!this.sources[i].isPlaying)
			{
				return this.sources[i];
			}
		}
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.outputAudioMixerGroup = this.mixerGroup;
		audioSource.dopplerLevel = 0f;
		audioSource.priority = 0;
		this.sources.Add(audioSource);
		return audioSource;
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x0000C1E9 File Offset: 0x0000A3E9
	public void ForceThemeChange(MusicTheme theme)
	{
		this.nextTheme = theme;
		if (this.nextTheme == null)
		{
			this.StopMusic();
		}
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x0000C206 File Offset: 0x0000A406
	private void DoForcedThemeChange()
	{
		if (this.nextTheme == null)
		{
			return;
		}
		this.currentTheme = this.nextTheme;
		this.nextTheme = null;
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x0000C22A File Offset: 0x0000A42A
	private void UpdateClips()
	{
		if (Random.value < this.randomIntensityJumpChance)
		{
			MusicManager.RaiseIntensityTo(Random.value, 0);
		}
		this.UpdateControlClips();
		this.UpdateMusicClips();
		this.lastClipUpdate = AudioSettings.dspTime;
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x0005F608 File Offset: 0x0005D808
	private void UpdateActiveClips()
	{
		this.activeClips.Clear();
		this.activeMusicClips.Clear();
		this.activeControlClips.Clear();
		if (this.currentTheme == null)
		{
			return;
		}
		this.AddActiveClipsForBar(this.themeBar);
		for (int i = 0; i < this.activeControlClips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.activeControlClips[i];
			if (positionedClip.jumpBarCount != 0)
			{
				int bar = this.themeBar + positionedClip.jumpBarCount;
				this.AddActiveClipsForBar(bar);
			}
		}
		this.lastActiveClipRefresh = this.currentBar;
		this.clipLoader.Refresh();
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x0005F6A8 File Offset: 0x0005D8A8
	private void AddActiveClipsForBar(int bar)
	{
		for (int i = 0; i < this.currentTheme.clips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.currentTheme.clips[i];
			if (bar >= positionedClip.startingBar - 8 && bar <= positionedClip.endingBar + 2)
			{
				this.activeClips.Add(positionedClip);
				if (!positionedClip.isControlClip)
				{
					this.activeMusicClips.Add(positionedClip);
				}
				if (positionedClip.isControlClip && bar == this.themeBar)
				{
					this.activeControlClips.Add(positionedClip);
				}
			}
		}
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x0005F738 File Offset: 0x0005D938
	private void UpdateControlClips()
	{
		for (int i = 0; i < this.activeControlClips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.activeControlClips[i];
			MusicManager.ClipPlaybackData playbackData = this.GetPlaybackData(positionedClip);
			if (this.themeBar <= positionedClip.startingBar && !playbackData.isActive && positionedClip.jumpBarCount == 0 && this.themeBar >= positionedClip.startingBar - this.clipScheduleBarsEarly)
			{
				playbackData.isActive = true;
				if (this.currentBar > this.holdIntensityUntilBar)
				{
					this.intensity -= positionedClip.intensityReduction;
				}
				if (this.intensity < 0f)
				{
					this.intensity = 0f;
				}
			}
		}
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x0005F7EC File Offset: 0x0005D9EC
	private void UpdateBarJumpClips()
	{
		for (int i = 0; i < this.activeControlClips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.activeControlClips[i];
			MusicManager.ClipPlaybackData playbackData = this.GetPlaybackData(positionedClip);
			if (this.themeBar <= positionedClip.startingBar && !playbackData.isActive && positionedClip.jumpBarCount != 0 && this.themeBar >= positionedClip.startingBar)
			{
				playbackData.isActive = true;
				if (this.intensity >= positionedClip.jumpMinimumIntensity && this.intensity <= positionedClip.jumpMaximumIntensity)
				{
					this.DoBarJump(positionedClip.jumpBarCount);
					return;
				}
			}
		}
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x0005F884 File Offset: 0x0005DA84
	private void UpdateMusicClips()
	{
		for (int i = 0; i < this.activeMusicClips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.activeMusicClips[i];
			MusicManager.ClipPlaybackData playbackData = this.GetPlaybackData(positionedClip);
			if (this.themeBar > positionedClip.endingBar)
			{
				playbackData.isActive = false;
				playbackData.source = null;
			}
			else
			{
				if (positionedClip.allowFadeOut && !positionedClip.CanPlay(this.intensity) && this.themeBar >= positionedClip.startingBar - 2 && this.themeBar < positionedClip.startingBar + positionedClip.musicClip.lengthInBars - 1 && playbackData.isActive)
				{
					this.FadeOutClip(positionedClip);
				}
				if (positionedClip.CanPlay(this.intensity) && !playbackData.isActive)
				{
					if (this.themeBar >= positionedClip.startingBar - this.clipScheduleBarsEarly && this.themeBar < positionedClip.startingBar)
					{
						this.ScheduleClip(positionedClip);
					}
					else if (positionedClip.allowFadeIn && this.themeBar >= positionedClip.startingBar && this.themeBar < positionedClip.startingBar + positionedClip.musicClip.lengthInBars - this.clipScheduleBarsEarly)
					{
						this.FadeInClip(positionedClip);
					}
				}
			}
		}
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x0005F9B4 File Offset: 0x0005DBB4
	private void ScheduleClip(MusicTheme.PositionedClip clip)
	{
		AudioSource freeAudioSource = this.GetFreeAudioSource();
		if (freeAudioSource == null)
		{
			return;
		}
		double num = this.themeStartTime + MusicUtil.BarsToSeconds(this.currentTheme.tempo, (float)(clip.startingBar - this.barOffset));
		MusicManager.ClipPlaybackData playbackData = this.GetPlaybackData(clip);
		freeAudioSource.clip = clip.musicClip.audioClip;
		freeAudioSource.timeSamples = 0;
		freeAudioSource.volume = 1f;
		freeAudioSource.PlayScheduled(num);
		playbackData.source = freeAudioSource;
		playbackData.isActive = true;
		playbackData.fadingIn = false;
		playbackData.fadingOut = false;
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x0005FA44 File Offset: 0x0005DC44
	private void FadeInClip(MusicTheme.PositionedClip clip)
	{
		AudioSource freeAudioSource = this.GetFreeAudioSource();
		if (freeAudioSource == null)
		{
			return;
		}
		double num = this.themeStartTime + MusicUtil.BarsToSeconds(this.currentTheme.tempo, (float)(clip.startingBar - this.barOffset));
		double seconds = AudioSettings.dspTime - num;
		float currentClipTimeBars = MusicUtil.SecondsToBars(this.currentTheme.tempo, seconds);
		float nextFadeInPoint = clip.musicClip.GetNextFadeInPoint(currentClipTimeBars);
		double num2 = MusicUtil.BarsToSeconds(this.currentTheme.tempo, nextFadeInPoint) - (double)clip.fadeInTime;
		if (nextFadeInPoint == -1f)
		{
			return;
		}
		freeAudioSource.volume = 0f;
		freeAudioSource.clip = clip.musicClip.audioClip;
		freeAudioSource.timeSamples = 0;
		freeAudioSource.Play();
		MusicManager.ClipPlaybackData playbackData = this.GetPlaybackData(clip);
		playbackData.source = freeAudioSource;
		playbackData.isActive = true;
		playbackData.fadingIn = true;
		playbackData.fadingOut = false;
		playbackData.fadeStarted = num + num2;
		playbackData.needsSync = true;
		this.needsResync = true;
		this.fadingClipCount++;
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x0005FB48 File Offset: 0x0005DD48
	private void FadeOutClip(MusicTheme.PositionedClip clip)
	{
		MusicManager.ClipPlaybackData playbackData = this.GetPlaybackData(clip);
		if (playbackData.fadingOut)
		{
			return;
		}
		double num = this.themeStartTime + MusicUtil.BarsToSeconds(this.currentTheme.tempo, (float)(clip.startingBar - this.barOffset));
		if (AudioSettings.dspTime >= num)
		{
			playbackData.fadingOut = true;
			playbackData.fadingIn = false;
			playbackData.fadeStarted = AudioSettings.dspTime;
			this.fadingClipCount++;
			return;
		}
		if (playbackData.source != null)
		{
			playbackData.source.Stop();
		}
		playbackData.source = null;
		playbackData.fadingOut = false;
		playbackData.isActive = false;
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x0005FBEC File Offset: 0x0005DDEC
	private MusicManager.ClipPlaybackData GetPlaybackData(MusicTheme.PositionedClip clip)
	{
		if (this.clipPlaybackData.ContainsKey(clip))
		{
			return this.clipPlaybackData[clip];
		}
		MusicManager.ClipPlaybackData clipPlaybackData = Pool.Get<MusicManager.ClipPlaybackData>();
		clipPlaybackData.positionedClip = clip;
		clipPlaybackData.source = null;
		clipPlaybackData.isActive = false;
		clipPlaybackData.fadingIn = false;
		clipPlaybackData.fadingOut = false;
		clipPlaybackData.needsSync = false;
		this.clipPlaybackData.Add(clip, clipPlaybackData);
		return clipPlaybackData;
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x0005FC54 File Offset: 0x0005DE54
	private MusicManager.ClipPlaybackData GetPlaybackData(AudioSource source)
	{
		foreach (KeyValuePair<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData> keyValuePair in this.clipPlaybackData)
		{
			MusicManager.ClipPlaybackData value = keyValuePair.Value;
			if (value.source == source)
			{
				return value;
			}
		}
		return null;
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x0005FCC0 File Offset: 0x0005DEC0
	private void ResyncClips()
	{
		if (!this.needsResync)
		{
			return;
		}
		this.CheckSyncSource();
		if (this.syncSource == null)
		{
			return;
		}
		int num = 0;
		int timeSamples = this.syncSource.timeSamples;
		for (int i = 0; i < this.sources.Count; i++)
		{
			AudioSource audioSource = this.sources[i];
			if (!(audioSource == this.syncSource) && !(audioSource.clip == null))
			{
				MusicManager.ClipPlaybackData playbackData = this.GetPlaybackData(audioSource);
				if (playbackData != null && playbackData.needsSync)
				{
					num++;
					MusicTheme.PositionedClip positionedClip = playbackData.positionedClip;
					double num2 = MusicUtil.BarsToSeconds(this.currentTheme.tempo, (float)(positionedClip.startingBar - this.barOffset));
					double seconds = AudioSettings.dspTime - (this.themeStartTime + num2);
					float num3 = Mathf.Floor(MusicUtil.SecondsToBars(this.currentTheme.tempo, seconds));
					int num4 = timeSamples % this.currentTheme.samplesPerBar;
					int num5 = this.currentTheme.samplesPerBar * (int)num3 + num4;
					if (num5 >= 0 && num5 < audioSource.clip.samples && audioSource.isPlaying)
					{
						audioSource.timeSamples = num5;
						playbackData.needsSync = false;
						num--;
					}
				}
			}
		}
		this.needsResync = (num > 0);
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x0005FE18 File Offset: 0x0005E018
	private void CheckSyncSource()
	{
		if (this.syncSource == null || !this.syncSource.isPlaying || this.syncSource.timeSamples <= 0)
		{
			for (int i = 0; i < this.sources.Count; i++)
			{
				if (this.sources[i].isPlaying && this.sources[i].timeSamples > 0 && this.currentTheme.ContainsAudioClip(this.sources[i].clip))
				{
					this.syncSource = this.sources[i];
					return;
				}
			}
		}
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x0000C25B File Offset: 0x0000A45B
	public void ShuffleThemes()
	{
		this.themes = Enumerable.ToList<MusicTheme>(Enumerable.OrderBy<MusicTheme, float>(this.themes, (MusicTheme x) => Random.value));
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x0000C292 File Offset: 0x0000A492
	public void MusicZoneEntered(MusicZone zone)
	{
		this.currentMusicZones.Add(zone);
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x0000C2A0 File Offset: 0x0000A4A0
	public void MusicZoneExited(MusicZone zone)
	{
		this.currentMusicZones.Remove(zone);
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x0005FEBC File Offset: 0x0005E0BC
	public MusicZone CurrentMusicZone()
	{
		MusicZone result = null;
		float num = float.NegativeInfinity;
		for (int i = this.currentMusicZones.Count - 1; i >= 0; i--)
		{
			if (this.currentMusicZones[i] == null)
			{
				this.currentMusicZones.RemoveAt(i);
			}
			else if (this.currentMusicZones[i].priority > num)
			{
				result = this.currentMusicZones[i];
				num = this.currentMusicZones[i].priority;
			}
		}
		return result;
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x0005FF40 File Offset: 0x0005E140
	public static void RaiseIntensityTo(float amount, int holdLengthBars = 0)
	{
		if (SingletonComponent<MusicManager>.Instance != null && SingletonComponent<MusicManager>.Instance.intensity < amount)
		{
			MusicZone musicZone = SingletonComponent<MusicManager>.Instance.CurrentMusicZone();
			if (!SingletonComponent<MusicManager>.Instance.musicPlaying && AudioSettings.dspTime > SingletonComponent<MusicManager>.Instance.nextMusicFromIntensityRaise && (!(musicZone != null) || !musicZone.suppressAutomaticMusic))
			{
				SingletonComponent<MusicManager>.Instance.StartMusic();
			}
			if (SingletonComponent<MusicManager>.Instance.musicPlaying)
			{
				if (holdLengthBars == 0)
				{
					holdLengthBars = SingletonComponent<MusicManager>.Instance.currentTheme.intensityHoldBars;
				}
				int num = SingletonComponent<MusicManager>.Instance.currentBar + holdLengthBars;
				SingletonComponent<MusicManager>.Instance.holdIntensityUntilBar = Mathf.RoundToInt((float)(num / 4)) * 4;
				SingletonComponent<MusicManager>.Instance.intensity = amount;
			}
		}
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x00060000 File Offset: 0x0005E200
	public void StopMusic()
	{
		if (!this.musicPlaying)
		{
			return;
		}
		this.intensity = 0f;
		if (this.currentTheme != null)
		{
			this.musicPlaying = false;
			this.nextMusic = AudioSettings.dspTime + MusicUtil.BarsToSeconds(this.currentTheme.tempo, 1f);
			this.nextMusicFromIntensityRaise = this.nextMusic;
			if (LevelManager.isLoaded)
			{
				this.nextMusic = AudioSettings.dspTime + (double)Random.Range(Music.songGapMin, Music.songGapMax);
				this.nextMusicFromIntensityRaise += (double)Music.songGapMin;
			}
		}
	}

	// Token: 0x02000173 RID: 371
	[Serializable]
	public class ClipPlaybackData
	{
		// Token: 0x04000A27 RID: 2599
		public AudioSource source;

		// Token: 0x04000A28 RID: 2600
		public MusicTheme.PositionedClip positionedClip;

		// Token: 0x04000A29 RID: 2601
		public bool isActive;

		// Token: 0x04000A2A RID: 2602
		public bool fadingIn;

		// Token: 0x04000A2B RID: 2603
		public bool fadingOut;

		// Token: 0x04000A2C RID: 2604
		public double fadeStarted;

		// Token: 0x04000A2D RID: 2605
		public bool needsSync;
	}
}
