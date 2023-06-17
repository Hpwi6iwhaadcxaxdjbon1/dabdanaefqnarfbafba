using System;

// Token: 0x020000C7 RID: 199
public class ElectricBattery : IOEntity
{
	// Token: 0x040006C2 RID: 1730
	public int maxOutput;

	// Token: 0x040006C3 RID: 1731
	public float maxCapactiySeconds;

	// Token: 0x040006C4 RID: 1732
	public float capacitySeconds;

	// Token: 0x040006C5 RID: 1733
	public bool rechargable;

	// Token: 0x040006C6 RID: 1734
	public float chargeRatio = 0.25f;

	// Token: 0x040006C7 RID: 1735
	private const float tickRateSeconds = 1f;

	// Token: 0x06000A62 RID: 2658 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x0000A422 File Offset: 0x00008622
	public override void ProcessAdditionalData(int first, int second, float third, float fourth)
	{
		base.ProcessAdditionalData(first, second, third, fourth);
		this.capacitySeconds = third;
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x0000A437 File Offset: 0x00008637
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.capacitySeconds = info.msg.ioEntity.genericFloat1;
		}
	}
}
