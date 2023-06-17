using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000170 RID: 368
public class MusicClipLoader
{
	// Token: 0x04000A02 RID: 2562
	public List<MusicClipLoader.LoadedAudioClip> loadedClips = new List<MusicClipLoader.LoadedAudioClip>();

	// Token: 0x04000A03 RID: 2563
	public Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip> loadedClipDict = new Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip>();

	// Token: 0x04000A04 RID: 2564
	public List<AudioClip> clipsToLoad = new List<AudioClip>();

	// Token: 0x04000A05 RID: 2565
	public List<AudioClip> clipsToUnload = new List<AudioClip>();

	// Token: 0x06000D0A RID: 3338 RVA: 0x0005ED70 File Offset: 0x0005CF70
	public void Update()
	{
		for (int i = this.clipsToLoad.Count - 1; i >= 0; i--)
		{
			AudioClip audioClip = this.clipsToLoad[i];
			if (audioClip.loadState != 2 && audioClip.loadState != 1)
			{
				audioClip.LoadAudioData();
				this.clipsToLoad.RemoveAt(i);
				return;
			}
		}
		for (int j = this.clipsToUnload.Count - 1; j >= 0; j--)
		{
			AudioClip audioClip2 = this.clipsToUnload[j];
			if (audioClip2.loadState == 2)
			{
				audioClip2.UnloadAudioData();
				this.clipsToUnload.RemoveAt(j);
				return;
			}
		}
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x0005EE0C File Offset: 0x0005D00C
	public void Refresh()
	{
		for (int i = 0; i < SingletonComponent<MusicManager>.Instance.activeMusicClips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = SingletonComponent<MusicManager>.Instance.activeMusicClips[i];
			MusicClipLoader.LoadedAudioClip loadedAudioClip = this.FindLoadedClip(positionedClip.musicClip.audioClip);
			if (loadedAudioClip == null)
			{
				loadedAudioClip = Pool.Get<MusicClipLoader.LoadedAudioClip>();
				loadedAudioClip.clip = positionedClip.musicClip.audioClip;
				loadedAudioClip.unloadTime = (float)AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				this.loadedClips.Add(loadedAudioClip);
				this.loadedClipDict.Add(loadedAudioClip.clip, loadedAudioClip);
				this.clipsToLoad.Add(loadedAudioClip.clip);
			}
			else
			{
				loadedAudioClip.unloadTime = (float)AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				this.clipsToUnload.Remove(loadedAudioClip.clip);
			}
		}
		for (int j = this.loadedClips.Count - 1; j >= 0; j--)
		{
			MusicClipLoader.LoadedAudioClip loadedAudioClip2 = this.loadedClips[j];
			if (AudioSettings.dspTime > (double)loadedAudioClip2.unloadTime)
			{
				this.clipsToUnload.Add(loadedAudioClip2.clip);
				this.loadedClips.Remove(loadedAudioClip2);
				this.loadedClipDict.Remove(loadedAudioClip2.clip);
				Pool.Free<MusicClipLoader.LoadedAudioClip>(ref loadedAudioClip2);
			}
		}
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x0000C138 File Offset: 0x0000A338
	private MusicClipLoader.LoadedAudioClip FindLoadedClip(AudioClip clip)
	{
		if (this.loadedClipDict.ContainsKey(clip))
		{
			return this.loadedClipDict[clip];
		}
		return null;
	}

	// Token: 0x02000171 RID: 369
	public class LoadedAudioClip
	{
		// Token: 0x04000A06 RID: 2566
		public AudioClip clip;

		// Token: 0x04000A07 RID: 2567
		public float unloadTime;
	}
}
