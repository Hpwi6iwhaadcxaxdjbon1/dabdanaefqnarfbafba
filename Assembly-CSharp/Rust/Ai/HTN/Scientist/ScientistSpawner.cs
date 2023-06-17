using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.Scientist
{
	// Token: 0x020008F4 RID: 2292
	public class ScientistSpawner : MonoBehaviour, IServerComponent
	{
		// Token: 0x04002BE0 RID: 11232
		public GameObjectRef ScientistPrefab;

		// Token: 0x04002BE1 RID: 11233
		[NonSerialized]
		public List<ScientistDomain> Spawned = new List<ScientistDomain>();

		// Token: 0x04002BE2 RID: 11234
		[NonSerialized]
		public BaseSpawnPoint[] SpawnPoints;

		// Token: 0x04002BE3 RID: 11235
		public int MaxPopulation = 1;

		// Token: 0x04002BE4 RID: 11236
		public bool InitialSpawn;

		// Token: 0x04002BE5 RID: 11237
		public float MinRespawnTimeMinutes = 20f;

		// Token: 0x04002BE6 RID: 11238
		public float MaxRespawnTimeMinutes = 20f;

		// Token: 0x04002BE7 RID: 11239
		public HTNDomain.MovementRule Movement = HTNDomain.MovementRule.FreeMove;

		// Token: 0x04002BE8 RID: 11240
		public float MovementRadius = -1f;

		// Token: 0x04002BE9 RID: 11241
		public bool ReducedLongRangeAccuracy;
	}
}
