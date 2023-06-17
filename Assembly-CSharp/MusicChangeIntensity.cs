using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class MusicChangeIntensity : MonoBehaviour
{
	// Token: 0x040009F7 RID: 2551
	public float raiseTo;

	// Token: 0x040009F8 RID: 2552
	public List<MusicChangeIntensity.DistanceIntensity> distanceIntensities = new List<MusicChangeIntensity.DistanceIntensity>();

	// Token: 0x040009F9 RID: 2553
	public float tickInterval = 0.2f;

	// Token: 0x040009FA RID: 2554
	private float lastTick;

	// Token: 0x06000D03 RID: 3331 RVA: 0x0000C0A3 File Offset: 0x0000A2A3
	private void OnEnable()
	{
		if (this.raiseTo > 0.01f)
		{
			MusicManager.RaiseIntensityTo(this.raiseTo, 0);
		}
		this.Tick();
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x0000C0C4 File Offset: 0x0000A2C4
	private void Update()
	{
		if (this.tickInterval != 0f && Time.time > this.lastTick + this.tickInterval)
		{
			this.Tick();
		}
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x0005EC40 File Offset: 0x0005CE40
	private void Tick()
	{
		float num = MainCamera.Distance(base.transform.position);
		for (int i = 0; i < this.distanceIntensities.Count; i++)
		{
			if (num <= this.distanceIntensities[i].distance)
			{
				MusicManager.RaiseIntensityTo(this.distanceIntensities[i].raiseTo, 0);
				if (SingletonComponent<MusicManager>.Instance != null)
				{
					MusicZone musicZone = SingletonComponent<MusicManager>.Instance.CurrentMusicZone();
					if (musicZone != null && !SingletonComponent<MusicManager>.Instance.musicPlaying && this.distanceIntensities[i].forceStartMusicInSuppressedMusicZones && musicZone.suppressAutomaticMusic)
					{
						SingletonComponent<MusicManager>.Instance.StartMusic();
					}
				}
			}
		}
		this.lastTick = Time.time;
	}

	// Token: 0x0200016E RID: 366
	[Serializable]
	public class DistanceIntensity
	{
		// Token: 0x040009FB RID: 2555
		public float distance = 60f;

		// Token: 0x040009FC RID: 2556
		public float raiseTo;

		// Token: 0x040009FD RID: 2557
		public bool forceStartMusicInSuppressedMusicZones;
	}
}
