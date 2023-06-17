using System;
using UnityEngine;

// Token: 0x020003C9 RID: 969
public class MeshGroupCull : LODComponent
{
	// Token: 0x040014F5 RID: 5365
	public float Distance = 100f;

	// Token: 0x040014F6 RID: 5366
	private Renderer[] meshRenderers;

	// Token: 0x040014F7 RID: 5367
	private int curlod;

	// Token: 0x040014F8 RID: 5368
	private bool force;

	// Token: 0x06001868 RID: 6248 RVA: 0x000146E9 File Offset: 0x000128E9
	protected override void InitLOD()
	{
		this.meshRenderers = base.GetComponentsInChildren<Renderer>();
	}

	// Token: 0x06001869 RID: 6249 RVA: 0x000146F7 File Offset: 0x000128F7
	protected override void EnableLOD()
	{
		this.force = true;
	}

	// Token: 0x0600186A RID: 6250 RVA: 0x00002ECE File Offset: 0x000010CE
	protected override void DisableLOD()
	{
	}

	// Token: 0x0600186B RID: 6251 RVA: 0x0008CF4C File Offset: 0x0008B14C
	protected override void Show()
	{
		for (int i = 0; i < this.meshRenderers.Length; i++)
		{
			this.meshRenderers[i].enabled = true;
		}
	}

	// Token: 0x0600186C RID: 6252 RVA: 0x0008CF7C File Offset: 0x0008B17C
	protected override void Hide()
	{
		for (int i = 0; i < this.meshRenderers.Length; i++)
		{
			this.meshRenderers[i].enabled = false;
		}
	}

	// Token: 0x0600186D RID: 6253 RVA: 0x0008CFAC File Offset: 0x0008B1AC
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

	// Token: 0x0600186E RID: 6254 RVA: 0x00014700 File Offset: 0x00012900
	protected override int GetLOD(float distance)
	{
		if (distance < LODUtil.VerifyDistance(this.Distance))
		{
			return 0;
		}
		return 1;
	}
}
