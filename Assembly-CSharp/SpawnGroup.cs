using System;
using System.Collections.Generic;

// Token: 0x020003FF RID: 1023
public class SpawnGroup : BaseMonoBehaviour, IServerComponent
{
	// Token: 0x04001592 RID: 5522
	public List<SpawnGroup.SpawnEntry> prefabs;

	// Token: 0x04001593 RID: 5523
	public int maxPopulation = 5;

	// Token: 0x04001594 RID: 5524
	public int numToSpawnPerTickMin = 1;

	// Token: 0x04001595 RID: 5525
	public int numToSpawnPerTickMax = 2;

	// Token: 0x04001596 RID: 5526
	public float respawnDelayMin = 10f;

	// Token: 0x04001597 RID: 5527
	public float respawnDelayMax = 20f;

	// Token: 0x04001598 RID: 5528
	public bool wantsInitialSpawn = true;

	// Token: 0x04001599 RID: 5529
	public bool temporary;

	// Token: 0x02000400 RID: 1024
	[Serializable]
	public class SpawnEntry
	{
		// Token: 0x0400159A RID: 5530
		public GameObjectRef prefab;

		// Token: 0x0400159B RID: 5531
		public int weight = 1;

		// Token: 0x0400159C RID: 5532
		public bool mobile;
	}
}
