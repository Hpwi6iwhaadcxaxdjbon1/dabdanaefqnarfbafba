using System;
using ConVar;
using UnityEngine;

// Token: 0x020003D0 RID: 976
public class ParticleSystemLOD : LODComponentParticleSystem
{
	// Token: 0x0400150B RID: 5387
	[Horizontal(1, 0)]
	public ParticleSystemLOD.State[] States;

	// Token: 0x06001886 RID: 6278 RVA: 0x0008D35C File Offset: 0x0008B55C
	protected override void SetLOD(int newlod)
	{
		if (this.curlod != newlod)
		{
			base.SetEmissionRate(this.States[newlod].emission * this.maxEmission);
			this.curlod = newlod;
			return;
		}
		if (this.force)
		{
			base.SetEmissionRate(this.States[newlod].emission * this.maxEmission);
			this.force = false;
		}
	}

	// Token: 0x06001887 RID: 6279 RVA: 0x0008D3C0 File Offset: 0x0008B5C0
	protected override int GetLOD(float distance)
	{
		for (int i = this.States.Length - 1; i >= 0; i--)
		{
			ParticleSystemLOD.State state = this.States[i];
			float num;
			if (state.emission != 0f)
			{
				num = Particle.lod;
				if (num < 1f && i > 1)
				{
					num = 1f;
				}
			}
			else
			{
				num = Particle.cull;
			}
			if (distance >= LODUtil.VerifyDistance(state.distance * num))
			{
				return i;
			}
		}
		return 0;
	}

	// Token: 0x020003D1 RID: 977
	[Serializable]
	public class State
	{
		// Token: 0x0400150C RID: 5388
		public float distance;

		// Token: 0x0400150D RID: 5389
		[Range(0f, 1f)]
		public float emission;
	}
}
