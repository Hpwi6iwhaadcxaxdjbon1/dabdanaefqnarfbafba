using System;
using UnityEngine;

// Token: 0x02000287 RID: 647
public class Sandstorm : MonoBehaviour
{
	// Token: 0x04000F15 RID: 3861
	public ParticleSystem m_psSandStorm;

	// Token: 0x04000F16 RID: 3862
	public float m_flSpeed;

	// Token: 0x04000F17 RID: 3863
	public float m_flSwirl;

	// Token: 0x04000F18 RID: 3864
	public float m_flEmissionRate;

	// Token: 0x0600126F RID: 4719 RVA: 0x00002ECE File Offset: 0x000010CE
	private void Start()
	{
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x00078D00 File Offset: 0x00076F00
	private void Update()
	{
		base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * this.m_flSwirl);
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = -7f + Mathf.Sin(Time.time * 2.5f) * 7f;
		base.transform.eulerAngles = eulerAngles;
		if (this.m_psSandStorm != null)
		{
			this.m_psSandStorm.startSpeed = this.m_flSpeed;
			this.m_psSandStorm.startSpeed += Mathf.Sin(Time.time * 0.4f) * (this.m_flSpeed * 0.75f);
			this.m_psSandStorm.emissionRate = this.m_flEmissionRate + Mathf.Sin(Time.time * 1f) * (this.m_flEmissionRate * 0.3f);
		}
	}
}
