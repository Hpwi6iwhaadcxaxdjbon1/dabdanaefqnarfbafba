using System;
using UnityEngine;

// Token: 0x020000D2 RID: 210
public class FlasherLight : IOEntity
{
	// Token: 0x040006D3 RID: 1747
	public EmissionToggle toggler;

	// Token: 0x040006D4 RID: 1748
	public Light myLight;

	// Token: 0x040006D5 RID: 1749
	public float flashSpacing = 0.2f;

	// Token: 0x040006D6 RID: 1750
	public float flashBurstSpacing = 0.5f;

	// Token: 0x040006D7 RID: 1751
	public float flashOnTime = 0.1f;

	// Token: 0x040006D8 RID: 1752
	public int numFlashesPerBurst = 5;

	// Token: 0x040006D9 RID: 1753
	private int flashBurstCount;

	// Token: 0x06000A72 RID: 2674 RVA: 0x000553A4 File Offset: 0x000535A4
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
		if (base.isServer)
		{
			return;
		}
		if (base.IsPowered())
		{
			if (!base.IsInvoking(new Action(this.Flash)))
			{
				base.Invoke(new Action(this.Flash), 0f);
				return;
			}
		}
		else
		{
			base.CancelInvoke(new Action(this.Flash));
			this.SetOff();
		}
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x0005540C File Offset: 0x0005360C
	public void Flash()
	{
		this.flashBurstCount++;
		this.FlashOn();
		if (this.flashBurstCount >= this.numFlashesPerBurst)
		{
			base.Invoke(new Action(this.Flash), this.flashBurstSpacing);
			this.flashBurstCount = 0;
			return;
		}
		base.Invoke(new Action(this.Flash), this.flashSpacing);
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x0000A4BF File Offset: 0x000086BF
	public void FlashOn()
	{
		this.toggler.SetEmissionEnabled(true);
		this.myLight.enabled = true;
		base.Invoke(new Action(this.SetOff), this.flashOnTime);
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x0000A4F1 File Offset: 0x000086F1
	public void SetOff()
	{
		this.toggler.SetEmissionEnabled(false);
		this.myLight.enabled = false;
	}
}
