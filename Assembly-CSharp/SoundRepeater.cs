using System;
using UnityEngine;

// Token: 0x02000192 RID: 402
[RequireComponent(typeof(SoundPlayer))]
public class SoundRepeater : MonoBehaviour
{
	// Token: 0x04000B14 RID: 2836
	public float interval = 5f;

	// Token: 0x04000B15 RID: 2837
	public SoundPlayer player;

	// Token: 0x06000E0A RID: 3594 RVA: 0x0000CEFB File Offset: 0x0000B0FB
	private void Start()
	{
		this.player.CreateSound();
		base.InvokeRepeating("Play", this.interval, this.interval);
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x0000CF1F File Offset: 0x0000B11F
	private void Play()
	{
		this.player.Play();
	}
}
