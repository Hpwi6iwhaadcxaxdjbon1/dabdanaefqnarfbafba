using System;
using UnityEngine;

// Token: 0x020003C2 RID: 962
public abstract class LODComponentParticleSystem : LODComponent
{
	// Token: 0x040014CD RID: 5325
	protected ParticleSystem particleSystem;

	// Token: 0x040014CE RID: 5326
	protected Renderer particleSystemRenderer;

	// Token: 0x040014CF RID: 5327
	protected float maxEmission;

	// Token: 0x040014D0 RID: 5328
	protected int curlod;

	// Token: 0x040014D1 RID: 5329
	protected bool force;

	// Token: 0x040014D2 RID: 5330
	private bool initialized;

	// Token: 0x06001839 RID: 6201 RVA: 0x000142C1 File Offset: 0x000124C1
	protected override void InitLOD()
	{
		if (this.initialized)
		{
			return;
		}
		this.particleSystem = base.GetComponent<ParticleSystem>();
		this.particleSystemRenderer = this.particleSystem.GetComponent<Renderer>();
		this.maxEmission = this.particleSystem.emissionRate;
		this.initialized = true;
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x00014301 File Offset: 0x00012501
	protected override void EnableLOD()
	{
		this.force = true;
	}

	// Token: 0x0600183B RID: 6203 RVA: 0x00002ECE File Offset: 0x000010CE
	protected override void DisableLOD()
	{
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x0001430A File Offset: 0x0001250A
	protected override void Show()
	{
		this.InitLOD();
		base.CancelInvoke(new Action(this.DisableParticleRenderer));
		this.EnableParticleRenderer();
		this.particleSystem.Play();
	}

	// Token: 0x0600183D RID: 6205 RVA: 0x00014335 File Offset: 0x00012535
	protected override void Hide()
	{
		this.InitLOD();
		this.particleSystem.Stop();
		base.Invoke(new Action(this.DisableParticleRenderer), this.particleSystem.startLifetime);
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x00014365 File Offset: 0x00012565
	private void EnableParticleRenderer()
	{
		this.particleSystemRenderer.enabled = true;
	}

	// Token: 0x0600183F RID: 6207 RVA: 0x00014373 File Offset: 0x00012573
	private void DisableParticleRenderer()
	{
		this.particleSystemRenderer.enabled = false;
	}

	// Token: 0x06001840 RID: 6208 RVA: 0x00014381 File Offset: 0x00012581
	protected void SetEmissionRate(float rate)
	{
		this.InitLOD();
		if (this.particleSystem.emissionRate == rate)
		{
			return;
		}
		this.particleSystem.emissionRate = rate;
		if (rate > 0f)
		{
			this.Show();
			return;
		}
		this.Hide();
	}
}
