using System;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class TreadEffects : MonoBehaviour
{
	// Token: 0x040007F2 RID: 2034
	public ParticleSystem[] rearTreadDirt;

	// Token: 0x040007F3 RID: 2035
	public ParticleSystem[] rearTreadSmoke;

	// Token: 0x040007F4 RID: 2036
	public ParticleSystem[] middleTreadSmoke;

	// Token: 0x040007F5 RID: 2037
	private Vector3 vecLastPosition;

	// Token: 0x040007F6 RID: 2038
	private float forwardSpeed;

	// Token: 0x06000B74 RID: 2932 RVA: 0x0000AF8D File Offset: 0x0000918D
	private void Start()
	{
		this.vecLastPosition = base.transform.position;
		this.forwardSpeed = 0f;
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x000590D0 File Offset: 0x000572D0
	private void Update()
	{
		Vector3 lhs = (base.transform.position - this.vecLastPosition) / Time.deltaTime;
		this.vecLastPosition = base.transform.position;
		this.forwardSpeed = Mathf.Lerp(this.forwardSpeed, Vector3.Dot(lhs, base.transform.forward), Time.deltaTime * 18f);
		float num = 15f;
		float speedRatio = Mathf.Clamp(this.forwardSpeed / num, -1f, 1f);
		this.UpdateTreadDirtEffects(speedRatio);
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x00059164 File Offset: 0x00057364
	private void UpdateTreadDirtEffects(float speedRatio)
	{
		if (Mathf.Abs(speedRatio) > 0.05f)
		{
			foreach (ParticleSystem particleSystem in this.rearTreadDirt)
			{
				if (particleSystem != null)
				{
					particleSystem.enableEmission = true;
					particleSystem.main.startSpeedMultiplier = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 1f, 7f);
					particleSystem.emissionRate = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 0.6f, 25f, 125f);
				}
			}
			foreach (ParticleSystem particleSystem2 in this.rearTreadSmoke)
			{
				if (particleSystem2 != null)
				{
					particleSystem2.enableEmission = true;
					ParticleSystem.MainModule main = particleSystem2.main;
					main.startSpeedMultiplier = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 1f, 6f);
					main.startSizeMultiplier = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 0.35f, 1.1f);
					particleSystem2.emissionRate = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 5f, 35f);
				}
			}
			foreach (ParticleSystem particleSystem3 in this.middleTreadSmoke)
			{
				if (particleSystem3 != null)
				{
					ParticleSystem.MainModule main2 = particleSystem3.main;
					main2.startSpeedMultiplier = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 0.3f, 6f);
					main2.startSizeMultiplier = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 0.5f, 1f);
					particleSystem3.emissionRate = Mathx.RemapValClamped(Mathf.Abs(speedRatio), 0f, 1f, 5f, 45f);
				}
			}
			return;
		}
		foreach (ParticleSystem particleSystem4 in this.rearTreadDirt)
		{
			if (particleSystem4 != null)
			{
				particleSystem4.enableEmission = false;
			}
		}
		foreach (ParticleSystem particleSystem5 in this.rearTreadSmoke)
		{
			if (particleSystem5 != null)
			{
				particleSystem5.enableEmission = false;
			}
		}
		foreach (ParticleSystem particleSystem6 in this.middleTreadSmoke)
		{
			if (particleSystem6 != null)
			{
				particleSystem6.emissionRate = 0f;
			}
		}
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x0000AFAB File Offset: 0x000091AB
	public void EnableTreadSmoke(int iWheel, bool bEnable)
	{
		this.middleTreadSmoke[iWheel].enableEmission = bEnable;
	}
}
