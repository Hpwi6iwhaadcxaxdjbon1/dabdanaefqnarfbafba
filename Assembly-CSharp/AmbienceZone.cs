using System;
using UnityEngine;

// Token: 0x0200015C RID: 348
public class AmbienceZone : MonoBehaviour, IClientComponent
{
	// Token: 0x04000964 RID: 2404
	public AmbienceDefinitionList baseAmbience;

	// Token: 0x04000965 RID: 2405
	public AmbienceDefinitionList stings;

	// Token: 0x04000966 RID: 2406
	public float priority;

	// Token: 0x04000967 RID: 2407
	public bool overrideCrossfadeTime;

	// Token: 0x04000968 RID: 2408
	public float crossfadeTime = 1f;

	// Token: 0x06000CBE RID: 3262 RVA: 0x0005D068 File Offset: 0x0005B268
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
		SingletonComponent<AmbienceManager>.Instance.AmbienceZoneEntered(this);
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x0005D0C4 File Offset: 0x0005B2C4
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
		SingletonComponent<AmbienceManager>.Instance.AmbienceZoneExited(this);
	}
}
