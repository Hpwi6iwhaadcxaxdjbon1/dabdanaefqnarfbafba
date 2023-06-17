using System;

// Token: 0x020003BC RID: 956
public class DecalCull : LODComponent
{
	// Token: 0x040014AE RID: 5294
	public float Distance = 20f;

	// Token: 0x040014AF RID: 5295
	private DeferredDecal decal;

	// Token: 0x040014B0 RID: 5296
	private int curlod;

	// Token: 0x040014B1 RID: 5297
	private bool force;

	// Token: 0x06001802 RID: 6146 RVA: 0x0001407C File Offset: 0x0001227C
	protected override void InitLOD()
	{
		this.decal = base.GetComponent<DeferredDecal>();
	}

	// Token: 0x06001803 RID: 6147 RVA: 0x0001408A File Offset: 0x0001228A
	protected override void EnableLOD()
	{
		this.force = true;
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x00002ECE File Offset: 0x000010CE
	protected override void DisableLOD()
	{
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x00014093 File Offset: 0x00012293
	protected override void Show()
	{
		this.decal.enabled = true;
	}

	// Token: 0x06001806 RID: 6150 RVA: 0x000140A1 File Offset: 0x000122A1
	protected override void Hide()
	{
		this.decal.enabled = false;
	}

	// Token: 0x06001807 RID: 6151 RVA: 0x0008C294 File Offset: 0x0008A494
	protected override void SetLOD(int newlod)
	{
		if (this.curlod != newlod)
		{
			if (newlod == 0)
			{
				this.Show();
			}
			else
			{
				this.Hide();
			}
			this.curlod = newlod;
			return;
		}
		if (this.force)
		{
			if (newlod == 0)
			{
				this.Show();
			}
			else
			{
				this.Hide();
			}
			this.force = false;
		}
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x000140AF File Offset: 0x000122AF
	protected override int GetLOD(float distance)
	{
		if (distance < LODUtil.VerifyDistance(this.Distance))
		{
			return 0;
		}
		return 1;
	}
}
