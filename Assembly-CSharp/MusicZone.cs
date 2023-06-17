using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200017A RID: 378
public class MusicZone : MonoBehaviour, IClientComponent
{
	// Token: 0x04000A51 RID: 2641
	public List<MusicTheme> themes;

	// Token: 0x04000A52 RID: 2642
	public float priority;

	// Token: 0x04000A53 RID: 2643
	public bool suppressAutomaticMusic;

	// Token: 0x06000D55 RID: 3413 RVA: 0x00060654 File Offset: 0x0005E854
	private void OnTriggerEnter(Collider other)
	{
		if (other == null || LocalPlayer.Entity == null || LocalPlayer.Entity.gameObject == null)
		{
			return;
		}
		if (other.gameObject != LocalPlayer.Entity.gameObject)
		{
			return;
		}
		SingletonComponent<MusicManager>.Instance.MusicZoneEntered(this);
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x000606B0 File Offset: 0x0005E8B0
	private void OnTriggerExit(Collider other)
	{
		if (other == null || LocalPlayer.Entity == null || LocalPlayer.Entity.gameObject == null)
		{
			return;
		}
		if (other.gameObject != LocalPlayer.Entity.gameObject)
		{
			return;
		}
		SingletonComponent<MusicManager>.Instance.MusicZoneExited(this);
	}
}
