using System;
using UnityEngine;

// Token: 0x020002F9 RID: 761
public class SphereEntity : BaseEntity
{
	// Token: 0x040010F8 RID: 4344
	public float currentRadius = 1f;

	// Token: 0x040010F9 RID: 4345
	public float lerpRadius = 1f;

	// Token: 0x040010FA RID: 4346
	public float lerpSpeed = 1f;

	// Token: 0x0600141E RID: 5150 RVA: 0x000112C7 File Offset: 0x0000F4C7
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (base.isClient && info.msg.sphereEntity != null)
		{
			this.lerpRadius = info.msg.sphereEntity.radius;
		}
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x000112FB File Offset: 0x0000F4FB
	protected void UpdateScale()
	{
		base.transform.localScale = new Vector3(this.currentRadius, this.currentRadius, this.currentRadius);
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x0007D6F8 File Offset: 0x0007B8F8
	protected void Update()
	{
		if (this.currentRadius == this.lerpRadius)
		{
			return;
		}
		if (base.isClient)
		{
			if (Mathf.Abs(this.currentRadius - this.lerpRadius) > 0.01f)
			{
				this.currentRadius = Mathf.Lerp(this.currentRadius, this.lerpRadius, Time.deltaTime);
			}
			else
			{
				this.currentRadius = this.lerpRadius;
			}
			this.UpdateScale();
		}
	}
}
