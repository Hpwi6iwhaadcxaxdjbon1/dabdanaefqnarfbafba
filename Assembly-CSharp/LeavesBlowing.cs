using System;
using UnityEngine;

// Token: 0x02000472 RID: 1138
public class LeavesBlowing : MonoBehaviour
{
	// Token: 0x0400178E RID: 6030
	public ParticleSystem m_psLeaves;

	// Token: 0x0400178F RID: 6031
	public float m_flSwirl;

	// Token: 0x04001790 RID: 6032
	public float m_flSpeed;

	// Token: 0x04001791 RID: 6033
	public float m_flEmissionRate;

	// Token: 0x06001AAD RID: 6829 RVA: 0x00002ECE File Offset: 0x000010CE
	private void Start()
	{
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x00093614 File Offset: 0x00091814
	private void Update()
	{
		base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * this.m_flSwirl);
		if (this.m_psLeaves != null)
		{
			this.m_psLeaves.startSpeed = this.m_flSpeed;
			this.m_psLeaves.startSpeed += Mathf.Sin(Time.time * 0.4f) * (this.m_flSpeed * 0.75f);
			this.m_psLeaves.emissionRate = this.m_flEmissionRate + Mathf.Sin(Time.time * 1f) * (this.m_flEmissionRate * 0.3f);
		}
	}
}
