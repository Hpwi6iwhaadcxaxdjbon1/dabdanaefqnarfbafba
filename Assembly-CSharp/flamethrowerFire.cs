using System;
using UnityEngine;

// Token: 0x02000773 RID: 1907
public class flamethrowerFire : MonoBehaviour
{
	// Token: 0x04002492 RID: 9362
	public ParticleSystem pilotLightFX;

	// Token: 0x04002493 RID: 9363
	public ParticleSystem[] flameFX;

	// Token: 0x04002494 RID: 9364
	public FlameJet jet;

	// Token: 0x04002495 RID: 9365
	public AudioSource oneShotSound;

	// Token: 0x04002496 RID: 9366
	public AudioSource loopSound;

	// Token: 0x04002497 RID: 9367
	public AudioClip pilotlightIdle;

	// Token: 0x04002498 RID: 9368
	public AudioClip flameLoop;

	// Token: 0x04002499 RID: 9369
	public AudioClip flameStart;

	// Token: 0x0400249A RID: 9370
	public flamethrowerState flameState;

	// Token: 0x0400249B RID: 9371
	private flamethrowerState previousflameState;

	// Token: 0x06002991 RID: 10641 RVA: 0x0002059E File Offset: 0x0001E79E
	public void PilotLightOn()
	{
		this.pilotLightFX.enableEmission = true;
		this.SetFlameStatus(false);
	}

	// Token: 0x06002992 RID: 10642 RVA: 0x000D3788 File Offset: 0x000D1988
	public void SetFlameStatus(bool status)
	{
		ParticleSystem[] array = this.flameFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = status;
		}
	}

	// Token: 0x06002993 RID: 10643 RVA: 0x000205B3 File Offset: 0x0001E7B3
	public void ShutOff()
	{
		this.pilotLightFX.enableEmission = false;
		this.SetFlameStatus(false);
	}

	// Token: 0x06002994 RID: 10644 RVA: 0x000205C8 File Offset: 0x0001E7C8
	public void FlameOn()
	{
		this.pilotLightFX.enableEmission = false;
		this.SetFlameStatus(true);
	}

	// Token: 0x06002995 RID: 10645 RVA: 0x000D37B4 File Offset: 0x000D19B4
	private void Start()
	{
		this.previousflameState = (this.flameState = flamethrowerState.OFF);
	}

	// Token: 0x06002996 RID: 10646 RVA: 0x000D37D4 File Offset: 0x000D19D4
	private void Update()
	{
		if (this.previousflameState != this.flameState)
		{
			switch (this.flameState)
			{
			case flamethrowerState.OFF:
				this.ShutOff();
				break;
			case flamethrowerState.PILOT_LIGHT:
				this.PilotLightOn();
				break;
			case flamethrowerState.FLAME_ON:
				this.FlameOn();
				break;
			}
			this.previousflameState = this.flameState;
			this.jet.SetOn(this.flameState == flamethrowerState.FLAME_ON);
		}
	}
}
