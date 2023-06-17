using System;

// Token: 0x02000335 RID: 821
public class BaseVehicleSeat : BaseVehicleMountPoint
{
	// Token: 0x060015BD RID: 5565 RVA: 0x00085140 File Offset: 0x00083340
	public override void UpdatePlayerModel(BasePlayer player)
	{
		base.UpdatePlayerModel(player);
		BaseVehicle vehicleParent = base.GetVehicleParent();
		if (vehicleParent == null)
		{
			return;
		}
		vehicleParent.UpdatePlayerModel(player);
	}
}
