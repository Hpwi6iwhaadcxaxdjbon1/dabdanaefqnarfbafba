using System;

// Token: 0x02000334 RID: 820
public class BaseVehicleMountPoint : BaseMountable
{
	// Token: 0x060015B9 RID: 5561 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool DirectlyMountable()
	{
		return false;
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x000049AE File Offset: 0x00002BAE
	public BaseVehicle GetVehicleParent()
	{
		return base.GetParentEntity() as BaseVehicle;
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x00085114 File Offset: 0x00083314
	public override float WaterFactorForPlayer(BasePlayer player)
	{
		BaseVehicle vehicleParent = this.GetVehicleParent();
		if (vehicleParent == null)
		{
			return 0f;
		}
		return vehicleParent.WaterFactorForPlayer(player);
	}
}
