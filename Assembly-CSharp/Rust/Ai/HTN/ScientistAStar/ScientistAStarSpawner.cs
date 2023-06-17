using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistAStar
{
	// Token: 0x020008EE RID: 2286
	public class ScientistAStarSpawner : MonoBehaviour, IServerComponent
	{
		// Token: 0x04002BCB RID: 11211
		public BasePath Path;

		// Token: 0x04002BCC RID: 11212
		public GameObjectRef ScientistAStarPrefab;

		// Token: 0x04002BCD RID: 11213
		[NonSerialized]
		public List<ScientistAStarDomain> Spawned = new List<ScientistAStarDomain>();

		// Token: 0x04002BCE RID: 11214
		[NonSerialized]
		public BaseSpawnPoint[] SpawnPoints;

		// Token: 0x04002BCF RID: 11215
		public int MaxPopulation = 1;

		// Token: 0x04002BD0 RID: 11216
		public bool InitialSpawn;

		// Token: 0x04002BD1 RID: 11217
		public float MinRespawnTimeMinutes = 20f;

		// Token: 0x04002BD2 RID: 11218
		public float MaxRespawnTimeMinutes = 20f;
	}
}
