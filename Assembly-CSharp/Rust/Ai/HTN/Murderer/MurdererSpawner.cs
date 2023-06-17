using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.Murderer
{
	// Token: 0x020008FF RID: 2303
	public class MurdererSpawner : MonoBehaviour, IServerComponent
	{
		// Token: 0x04002C07 RID: 11271
		public GameObjectRef MurdererPrefab;

		// Token: 0x04002C08 RID: 11272
		[NonSerialized]
		public List<MurdererDomain> Spawned = new List<MurdererDomain>();

		// Token: 0x04002C09 RID: 11273
		[NonSerialized]
		public BaseSpawnPoint[] SpawnPoints;

		// Token: 0x04002C0A RID: 11274
		public int MaxPopulation = 1;

		// Token: 0x04002C0B RID: 11275
		public bool InitialSpawn;

		// Token: 0x04002C0C RID: 11276
		public float MinRespawnTimeMinutes = 20f;

		// Token: 0x04002C0D RID: 11277
		public float MaxRespawnTimeMinutes = 20f;

		// Token: 0x04002C0E RID: 11278
		public HTNDomain.MovementRule Movement = HTNDomain.MovementRule.FreeMove;

		// Token: 0x04002C0F RID: 11279
		public float MovementRadius = -1f;

		// Token: 0x04002C10 RID: 11280
		public bool ReducedLongRangeAccuracy;
	}
}
