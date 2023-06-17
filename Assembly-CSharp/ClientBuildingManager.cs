using System;
using Facepunch;

// Token: 0x02000369 RID: 873
public class ClientBuildingManager : BuildingManager
{
	// Token: 0x0600167B RID: 5755 RVA: 0x00086DB8 File Offset: 0x00084FB8
	public void Cycle()
	{
		using (TimeWarning.New("UpdateSkinQueue", 0.1f))
		{
			BuildingBlock.updateSkinQueueClient.RunQueue(1.0);
		}
	}

	// Token: 0x0600167C RID: 5756 RVA: 0x00012FDB File Offset: 0x000111DB
	protected override BuildingManager.Building CreateBuilding(uint id)
	{
		BuildingManager.Building building = Pool.Get<BuildingManager.Building>();
		building.ID = id;
		return building;
	}

	// Token: 0x0600167D RID: 5757 RVA: 0x00012FE9 File Offset: 0x000111E9
	protected override void DisposeBuilding(ref BuildingManager.Building building)
	{
		Pool.Free<BuildingManager.Building>(ref building);
	}
}
