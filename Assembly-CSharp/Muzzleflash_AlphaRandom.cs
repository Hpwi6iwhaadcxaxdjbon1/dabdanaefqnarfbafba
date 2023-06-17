using System;
using UnityEngine;

// Token: 0x02000281 RID: 641
public class Muzzleflash_AlphaRandom : MonoBehaviour
{
	// Token: 0x04000EF8 RID: 3832
	public ParticleSystem[] muzzleflashParticles;

	// Token: 0x04000EF9 RID: 3833
	private Gradient grad = new Gradient();

	// Token: 0x04000EFA RID: 3834
	private GradientColorKey[] gck = new GradientColorKey[3];

	// Token: 0x04000EFB RID: 3835
	private GradientAlphaKey[] gak = new GradientAlphaKey[3];

	// Token: 0x06001259 RID: 4697 RVA: 0x00002ECE File Offset: 0x000010CE
	private void Start()
	{
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x000786E8 File Offset: 0x000768E8
	private void OnEnable()
	{
		this.gck[0].color = Color.white;
		this.gck[0].time = 0f;
		this.gck[1].color = Color.white;
		this.gck[1].time = 0.6f;
		this.gck[2].color = Color.black;
		this.gck[2].time = 0.75f;
		float alpha = Random.Range(0.2f, 0.85f);
		this.gak[0].alpha = alpha;
		this.gak[0].time = 0f;
		this.gak[1].alpha = alpha;
		this.gak[1].time = 0.45f;
		this.gak[2].alpha = 0f;
		this.gak[2].time = 0.5f;
		this.grad.SetKeys(this.gck, this.gak);
		foreach (ParticleSystem particleSystem in this.muzzleflashParticles)
		{
			if (particleSystem == null)
			{
				Debug.LogWarning("Muzzleflash_AlphaRandom : null particle system in " + base.gameObject.name);
			}
			else
			{
				particleSystem.colorOverLifetime.color = this.grad;
			}
		}
	}
}
