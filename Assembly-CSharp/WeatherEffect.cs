using System;
using UnityEngine;

// Token: 0x0200041B RID: 1051
public abstract class WeatherEffect : MonoBehaviour, IClientComponent
{
	// Token: 0x04001603 RID: 5635
	public ParticleSystem[] emitOnStart;

	// Token: 0x04001604 RID: 5636
	public ParticleSystem[] emitOnStop;

	// Token: 0x04001605 RID: 5637
	public ParticleSystem[] emitOnLoop;

	// Token: 0x04001606 RID: 5638
	private float currentIntensity = -1f;

	// Token: 0x04001607 RID: 5639
	private float[] maxEmissionRates;

	// Token: 0x0600196E RID: 6510 RVA: 0x00090104 File Offset: 0x0008E304
	protected void Awake()
	{
		this.currentIntensity = -1f;
		this.maxEmissionRates = new float[this.emitOnLoop.Length];
		for (int i = 0; i < this.emitOnLoop.Length; i++)
		{
			this.maxEmissionRates[i] = this.emitOnLoop[i].emissionRate;
		}
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x00090158 File Offset: 0x0008E358
	protected void LateUpdate()
	{
		if (this.currentIntensity > 0f)
		{
			base.transform.position = MainCamera.mainCamera.transform.position;
		}
		float num = this.GetCurrentIntensity();
		if (num == this.currentIntensity)
		{
			return;
		}
		for (int i = 0; i < this.emitOnLoop.Length; i++)
		{
			this.emitOnLoop[i].emissionRate = this.maxEmissionRates[i] * num;
		}
		if (this.currentIntensity < 0.5f && num >= 0.5f)
		{
			for (int j = 0; j < this.emitOnStop.Length; j++)
			{
				this.emitOnStop[j].Stop();
			}
			for (int k = 0; k < this.emitOnStart.Length; k++)
			{
				this.emitOnStart[k].Play();
			}
		}
		if (this.currentIntensity >= 0.5f && num < 0.5f)
		{
			for (int l = 0; l < this.emitOnStart.Length; l++)
			{
				this.emitOnStart[l].Stop();
			}
			for (int m = 0; m < this.emitOnStop.Length; m++)
			{
				this.emitOnStop[m].Play();
			}
		}
		this.currentIntensity = num;
	}

	// Token: 0x06001970 RID: 6512
	protected abstract float GetCurrentIntensity();
}
