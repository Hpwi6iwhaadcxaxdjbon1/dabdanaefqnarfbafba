using System;
using UnityEngine.AI;

// Token: 0x02000367 RID: 871
public abstract class BuildingManager
{
	// Token: 0x04001342 RID: 4930
	public static ClientBuildingManager client = new ClientBuildingManager();

	// Token: 0x04001343 RID: 4931
	protected ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);

	// Token: 0x04001344 RID: 4932
	protected ListDictionary<uint, BuildingManager.Building> buildingDictionary = new ListDictionary<uint, BuildingManager.Building>(8);

	// Token: 0x06001664 RID: 5732 RVA: 0x00086C54 File Offset: 0x00084E54
	public BuildingManager.Building GetBuilding(uint buildingID)
	{
		BuildingManager.Building result = null;
		this.buildingDictionary.TryGetValue(buildingID, ref result);
		return result;
	}

	// Token: 0x06001665 RID: 5733 RVA: 0x00086C74 File Offset: 0x00084E74
	public void Add(DecayEntity ent)
	{
		if (ent.buildingID == 0U)
		{
			if (!this.decayEntities.Contains(ent))
			{
				this.decayEntities.Add(ent);
			}
			return;
		}
		BuildingManager.Building building = this.GetBuilding(ent.buildingID);
		if (building == null)
		{
			building = this.CreateBuilding(ent.buildingID);
			this.buildingDictionary.Add(ent.buildingID, building);
		}
		building.Add(ent);
		building.Dirty();
	}

	// Token: 0x06001666 RID: 5734 RVA: 0x00086CE0 File Offset: 0x00084EE0
	public void Remove(DecayEntity ent)
	{
		if (ent.buildingID == 0U)
		{
			this.decayEntities.Remove(ent);
			return;
		}
		BuildingManager.Building building = this.GetBuilding(ent.buildingID);
		if (building == null)
		{
			return;
		}
		building.Remove(ent);
		if (building.IsEmpty())
		{
			this.buildingDictionary.Remove(ent.buildingID);
			this.DisposeBuilding(ref building);
			return;
		}
		building.Dirty();
	}

	// Token: 0x06001667 RID: 5735 RVA: 0x00012E08 File Offset: 0x00011008
	public void Clear()
	{
		this.buildingDictionary.Clear();
	}

	// Token: 0x06001668 RID: 5736
	protected abstract BuildingManager.Building CreateBuilding(uint id);

	// Token: 0x06001669 RID: 5737
	protected abstract void DisposeBuilding(ref BuildingManager.Building building);

	// Token: 0x02000368 RID: 872
	public class Building
	{
		// Token: 0x04001345 RID: 4933
		public uint ID;

		// Token: 0x04001346 RID: 4934
		public ListHashSet<BuildingPrivlidge> buildingPrivileges = new ListHashSet<BuildingPrivlidge>(8);

		// Token: 0x04001347 RID: 4935
		public ListHashSet<BuildingBlock> buildingBlocks = new ListHashSet<BuildingBlock>(8);

		// Token: 0x04001348 RID: 4936
		public ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);

		// Token: 0x04001349 RID: 4937
		public NavMeshObstacle buildingNavMeshObstacle;

		// Token: 0x0400134A RID: 4938
		public ListHashSet<NavMeshObstacle> navmeshCarvers;

		// Token: 0x0400134B RID: 4939
		public bool isNavMeshCarvingDirty;

		// Token: 0x0400134C RID: 4940
		public bool isNavMeshCarveOptimized;

		// Token: 0x0600166C RID: 5740 RVA: 0x00012E41 File Offset: 0x00011041
		public bool IsEmpty()
		{
			return !this.HasBuildingPrivileges() && !this.HasBuildingBlocks() && !this.HasDecayEntities();
		}

		// Token: 0x0600166D RID: 5741 RVA: 0x00086D44 File Offset: 0x00084F44
		public BuildingPrivlidge GetDominatingBuildingPrivilege()
		{
			BuildingPrivlidge buildingPrivlidge = null;
			if (this.HasBuildingPrivileges())
			{
				for (int i = 0; i < this.buildingPrivileges.Count; i++)
				{
					BuildingPrivlidge buildingPrivlidge2 = this.buildingPrivileges[i];
					if (!(buildingPrivlidge2 == null) && buildingPrivlidge2.IsOlderThan(buildingPrivlidge))
					{
						buildingPrivlidge = buildingPrivlidge2;
					}
				}
			}
			return buildingPrivlidge;
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x00012E62 File Offset: 0x00011062
		public bool HasBuildingPrivileges()
		{
			return this.buildingPrivileges != null && this.buildingPrivileges.Count > 0;
		}

		// Token: 0x0600166F RID: 5743 RVA: 0x00012E7C File Offset: 0x0001107C
		public bool HasBuildingBlocks()
		{
			return this.buildingBlocks != null && this.buildingBlocks.Count > 0;
		}

		// Token: 0x06001670 RID: 5744 RVA: 0x00012E96 File Offset: 0x00011096
		public bool HasDecayEntities()
		{
			return this.decayEntities != null && this.decayEntities.Count > 0;
		}

		// Token: 0x06001671 RID: 5745 RVA: 0x00012EB0 File Offset: 0x000110B0
		public void AddBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.buildingPrivileges.Contains(ent))
			{
				this.buildingPrivileges.Add(ent);
			}
		}

		// Token: 0x06001672 RID: 5746 RVA: 0x00012ED6 File Offset: 0x000110D6
		public void RemoveBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (ent == null)
			{
				return;
			}
			this.buildingPrivileges.Remove(ent);
		}

		// Token: 0x06001673 RID: 5747 RVA: 0x00012EEF File Offset: 0x000110EF
		public void AddBuildingBlock(BuildingBlock ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.buildingBlocks.Contains(ent))
			{
				this.buildingBlocks.Add(ent);
			}
		}

		// Token: 0x06001674 RID: 5748 RVA: 0x00012F15 File Offset: 0x00011115
		public void RemoveBuildingBlock(BuildingBlock ent)
		{
			if (ent == null)
			{
				return;
			}
			this.buildingBlocks.Remove(ent);
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x00012F2E File Offset: 0x0001112E
		public void AddDecayEntity(DecayEntity ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.decayEntities.Contains(ent))
			{
				this.decayEntities.Add(ent);
			}
		}

		// Token: 0x06001676 RID: 5750 RVA: 0x00012F54 File Offset: 0x00011154
		public void RemoveDecayEntity(DecayEntity ent)
		{
			if (ent == null)
			{
				return;
			}
			this.decayEntities.Remove(ent);
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x00012F6D File Offset: 0x0001116D
		public void Add(DecayEntity ent)
		{
			this.AddDecayEntity(ent);
			this.AddBuildingBlock(ent as BuildingBlock);
			this.AddBuildingPrivilege(ent as BuildingPrivlidge);
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x00012F8E File Offset: 0x0001118E
		public void Remove(DecayEntity ent)
		{
			this.RemoveDecayEntity(ent);
			this.RemoveBuildingBlock(ent as BuildingBlock);
			this.RemoveBuildingPrivilege(ent as BuildingPrivlidge);
		}

		// Token: 0x06001679 RID: 5753 RVA: 0x00086D94 File Offset: 0x00084F94
		public void Dirty()
		{
			BuildingPrivlidge dominatingBuildingPrivilege = this.GetDominatingBuildingPrivilege();
			if (dominatingBuildingPrivilege != null)
			{
				dominatingBuildingPrivilege.BuildingDirty();
			}
		}
	}
}
