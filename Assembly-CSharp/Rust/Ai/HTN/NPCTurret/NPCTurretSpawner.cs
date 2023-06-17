using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.NPCTurret
{
	// Token: 0x020008FA RID: 2298
	public class NPCTurretSpawner : MonoBehaviour, IServerComponent
	{
		// Token: 0x04002BF7 RID: 11255
		public GameObjectRef NPCTurretPrefab;

		// Token: 0x04002BF8 RID: 11256
		[NonSerialized]
		public List<NPCTurretDomain> Spawned = new List<NPCTurretDomain>();

		// Token: 0x04002BF9 RID: 11257
		[NonSerialized]
		public BaseSpawnPoint[] SpawnPoints;

		// Token: 0x04002BFA RID: 11258
		public int MaxPopulation = 1;

		// Token: 0x04002BFB RID: 11259
		public bool InitialSpawn;

		// Token: 0x04002BFC RID: 11260
		public float MinRespawnTimeMinutes = 20f;

		// Token: 0x04002BFD RID: 11261
		public float MaxRespawnTimeMinutes = 20f;

		// Token: 0x04002BFE RID: 11262
		public bool OnlyRotateAroundYAxis;

		// Token: 0x04002BFF RID: 11263
		public bool ReducedLongRangeAccuracy;

		// Token: 0x04002C00 RID: 11264
		public bool BurstAtLongRange;
	}
}
