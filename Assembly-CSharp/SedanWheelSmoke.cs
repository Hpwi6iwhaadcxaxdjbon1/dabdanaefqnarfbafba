using System;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class SedanWheelSmoke : MonoBehaviour
{
	// Token: 0x040000BA RID: 186
	public ParticleSystem[] tireSmoke;

	// Token: 0x040000BB RID: 187
	public ParticleSystem[] tireWaterSplash;

	// Token: 0x040000BC RID: 188
	public ParticleSystem[] tireWaterSplash_Extra;

	// Token: 0x040000BD RID: 189
	private ParticleSystem[] activeTireEffect;

	// Token: 0x040000BE RID: 190
	public bool[] wheelTouching;

	// Token: 0x040000BF RID: 191
	private Vector3 vecLastPosition;

	// Token: 0x040000C0 RID: 192
	private float forwardSpeed;

	// Token: 0x040000C1 RID: 193
	private float updateTime;

	// Token: 0x0600006A RID: 106 RVA: 0x0002905C File Offset: 0x0002725C
	public void InitWheelSmoke()
	{
		this.updateTime = Time.time;
		this.activeTireEffect = new ParticleSystem[4];
		this.wheelTouching = new bool[4];
		this.vecLastPosition = base.transform.position;
		this.forwardSpeed = 0f;
		if (this.activeTireEffect != null && this.activeTireEffect.Length != 0)
		{
			for (int i = 0; i < this.activeTireEffect.Length; i++)
			{
				this.tireSmoke[i].enableEmission = false;
				this.tireWaterSplash[i].enableEmission = false;
				this.tireWaterSplash_Extra[i].enableEmission = false;
			}
		}
	}

	// Token: 0x0600006B RID: 107 RVA: 0x000290F8 File Offset: 0x000272F8
	public void UpdateWheelSmoke()
	{
		if (this.updateTime <= Time.time)
		{
			this.updateTime = Time.time + 0.1f;
			Vector3 lhs = (base.transform.position - this.vecLastPosition) / 0.1f;
			this.vecLastPosition = base.transform.position;
			this.forwardSpeed = Mathf.Lerp(this.forwardSpeed, Vector3.Dot(lhs, base.transform.forward), Time.deltaTime * 18f);
			float num = 15f;
			float speedRatio = Mathf.Clamp(this.forwardSpeed / num, -1f, 1f);
			this.UpdateTireEffects(speedRatio);
			return;
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x000291AC File Offset: 0x000273AC
	private void UpdateTireEffects(float speedRatio)
	{
		if (Mathf.Abs(speedRatio) > 0.05f)
		{
			for (int i = 0; i < this.activeTireEffect.Length; i++)
			{
				bool flag = this.CheckInWater(i);
				ParticleSystem particleSystem = this.activeTireEffect[i];
				if (particleSystem != null && this.wheelTouching[i])
				{
					if (!particleSystem.enableEmission)
					{
						particleSystem.enableEmission = true;
					}
					ParticleSystem.MainModule main = particleSystem.main;
					if (!flag)
					{
						main.startSpeedMultiplier = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 1f, 4f);
						main.startSizeMultiplier = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 0.35f, 1.3f);
						particleSystem.emissionRate = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 15f, 120f);
					}
					else
					{
						particleSystem.shape.rotation.x = (float)Random.RandomRange(60, 120);
						main.startSizeMultiplier = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 0.45f, 1.55f);
						if (!this.tireWaterSplash_Extra[i].enableEmission)
						{
							this.tireWaterSplash_Extra[i].enableEmission = true;
						}
						this.tireWaterSplash_Extra[i].startSize = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 1.8f, 2.7f);
						this.tireWaterSplash_Extra[i].startSpeed = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 2f, 2.7f);
						this.tireWaterSplash_Extra[i].emissionRate = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 45f, 65f);
					}
				}
				else if (particleSystem != null)
				{
					particleSystem.emissionRate = Mathf.Lerp(particleSystem.emissionRate, 0f, Time.deltaTime * 3.5f);
					if (flag)
					{
						this.tireWaterSplash_Extra[i].emissionRate = Mathf.Lerp(particleSystem.emissionRate, 0f, Time.deltaTime * 3.5f);
					}
				}
			}
			return;
		}
		for (int j = 0; j < this.activeTireEffect.Length; j++)
		{
			this.tireSmoke[j].enableEmission = false;
			this.tireWaterSplash[j].enableEmission = false;
			this.tireWaterSplash_Extra[j].enableEmission = false;
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00029420 File Offset: 0x00027620
	private bool CheckInWater(int i)
	{
		bool result = false;
		if (WaterLevel.Test(this.tireWaterSplash[i].transform.position - Vector3.up * 0.12f))
		{
			result = true;
			this.tireWaterSplash[i].enableEmission = true;
			this.tireWaterSplash_Extra[i].enableEmission = true;
			this.tireSmoke[i].enableEmission = false;
			this.activeTireEffect[i] = this.tireWaterSplash[i];
		}
		else
		{
			this.tireSmoke[i].enableEmission = true;
			this.tireWaterSplash[i].enableEmission = false;
			this.tireWaterSplash_Extra[i].enableEmission = false;
			this.activeTireEffect[i] = this.tireSmoke[i];
		}
		return result;
	}
}
