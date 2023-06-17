using System;
using UnityEngine;

// Token: 0x020003D4 RID: 980
public class RigidbodyLOD : LODComponent
{
	// Token: 0x04001517 RID: 5399
	public float Distance = 100f;

	// Token: 0x04001518 RID: 5400
	private Rigidbody rigidbody;

	// Token: 0x04001519 RID: 5401
	private int curlod;

	// Token: 0x0400151A RID: 5402
	private bool force;

	// Token: 0x0400151B RID: 5403
	private bool kinematic;

	// Token: 0x06001898 RID: 6296 RVA: 0x000148E6 File Offset: 0x00012AE6
	protected override void InitLOD()
	{
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.kinematic = this.rigidbody.isKinematic;
	}

	// Token: 0x06001899 RID: 6297 RVA: 0x00014905 File Offset: 0x00012B05
	protected override void EnableLOD()
	{
		this.force = true;
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x00002ECE File Offset: 0x000010CE
	protected override void DisableLOD()
	{
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x0001490E File Offset: 0x00012B0E
	protected override void Show()
	{
		if (this.rigidbody != null)
		{
			this.rigidbody.isKinematic = this.kinematic;
			this.rigidbody.detectCollisions = true;
		}
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x0001493B File Offset: 0x00012B3B
	protected override void Hide()
	{
		if (this.rigidbody != null)
		{
			this.rigidbody.isKinematic = true;
			this.rigidbody.detectCollisions = false;
		}
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x0008D734 File Offset: 0x0008B934
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

	// Token: 0x0600189E RID: 6302 RVA: 0x00014963 File Offset: 0x00012B63
	protected override int GetLOD(float distance)
	{
		if (distance < LODUtil.VerifyDistance(this.Distance))
		{
			return 0;
		}
		return 1;
	}
}
