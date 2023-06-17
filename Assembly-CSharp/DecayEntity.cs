using System;
using System.Collections.Generic;
using ProtoBuf;

// Token: 0x020002CA RID: 714
public class DecayEntity : BaseCombatEntity
{
	// Token: 0x04000FF1 RID: 4081
	[NonSerialized]
	public uint buildingID;

	// Token: 0x04000FF2 RID: 4082
	private Upkeep upkeep;

	// Token: 0x0600138B RID: 5003 RVA: 0x000108EC File Offset: 0x0000EAEC
	public override void ResetState()
	{
		base.ResetState();
		this.buildingID = 0U;
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x000108FB File Offset: 0x0000EAFB
	public void AttachToBuilding(uint id)
	{
		if (base.isClient)
		{
			BuildingManager.client.Remove(this);
			this.buildingID = id;
			BuildingManager.client.Add(this);
		}
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x00010922 File Offset: 0x0000EB22
	public BuildingManager.Building GetBuilding()
	{
		if (base.isClient)
		{
			return BuildingManager.client.GetBuilding(this.buildingID);
		}
		return null;
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x0007BE80 File Offset: 0x0007A080
	public override BuildingPrivlidge GetBuildingPrivilege()
	{
		BuildingManager.Building building = this.GetBuilding();
		if (building != null)
		{
			return building.GetDominatingBuildingPrivilege();
		}
		return base.GetBuildingPrivilege();
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x0007BEA4 File Offset: 0x0007A0A4
	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts, float multiplier)
	{
		if (this.upkeep == null)
		{
			return;
		}
		float num = this.upkeep.upkeepMultiplier * multiplier;
		if (num == 0f)
		{
			return;
		}
		List<ItemAmount> list = this.BuildCost();
		if (list == null)
		{
			return;
		}
		foreach (ItemAmount itemAmount in list)
		{
			if (itemAmount.itemDef.category == ItemCategory.Resources)
			{
				float num2 = itemAmount.amount * num;
				bool flag = false;
				foreach (ItemAmount itemAmount2 in itemAmounts)
				{
					if (itemAmount2.itemDef == itemAmount.itemDef)
					{
						itemAmount2.amount += num2;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					itemAmounts.Add(new ItemAmount(itemAmount.itemDef, num2));
				}
			}
		}
	}

	// Token: 0x06001390 RID: 5008 RVA: 0x0001093E File Offset: 0x0000EB3E
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		this.upkeep = PrefabAttribute.client.Find<Upkeep>(this.prefabID);
		BuildingManager.client.Add(this);
	}

	// Token: 0x06001391 RID: 5009 RVA: 0x00010968 File Offset: 0x0000EB68
	protected override void DoClientDestroy()
	{
		base.DoClientDestroy();
		BuildingManager.client.Remove(this);
	}

	// Token: 0x06001392 RID: 5010 RVA: 0x0001097B File Offset: 0x0000EB7B
	public override bool DisplayHealthInfo(BasePlayer player)
	{
		return this.ShowHealthInfo && (base.healthFraction < 0.95f || player.IsHoldingEntity<Hammer>());
	}

	// Token: 0x06001393 RID: 5011 RVA: 0x0007BFB8 File Offset: 0x0007A1B8
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.decayEntity != null && this.buildingID != info.msg.decayEntity.buildingID)
		{
			this.AttachToBuilding(info.msg.decayEntity.buildingID);
		}
	}
}
