using System;
using UnityEngine;

// Token: 0x02000282 RID: 642
public class MuzzleFlash_Flamelet : MonoBehaviour
{
	// Token: 0x04000EFC RID: 3836
	public ParticleSystem flameletParticle;

	// Token: 0x0600125C RID: 4700 RVA: 0x00078874 File Offset: 0x00076A74
	private void OnEnable()
	{
		this.flameletParticle.shape.angle = (float)Random.Range(6, 13);
		float num = Random.Range(7f, 9f);
		this.flameletParticle.startSpeed = Random.Range(2.5f, num);
		this.flameletParticle.startSize = Random.Range(0.05f, num * 0.015f);
	}
}
