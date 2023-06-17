using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile
{
	// Token: 0x020008E7 RID: 2279
	public class ScientistJunkpileSpawner : MonoBehaviour, IServerComponent
	{
		// Token: 0x04002BAA RID: 11178
		public GameObjectRef ScientistPrefab;

		// Token: 0x04002BAB RID: 11179
		[NonSerialized]
		public List<ScientistJunkpileDomain> Spawned = new List<ScientistJunkpileDomain>();

		// Token: 0x04002BAC RID: 11180
		[NonSerialized]
		public BaseSpawnPoint[] SpawnPoints;

		// Token: 0x04002BAD RID: 11181
		public int MaxPopulation = 1;

		// Token: 0x04002BAE RID: 11182
		public bool InitialSpawn;

		// Token: 0x04002BAF RID: 11183
		public float MinRespawnTimeMinutes = 120f;

		// Token: 0x04002BB0 RID: 11184
		public float MaxRespawnTimeMinutes = 120f;

		// Token: 0x04002BB1 RID: 11185
		public HTNDomain.MovementRule Movement = HTNDomain.MovementRule.FreeMove;

		// Token: 0x04002BB2 RID: 11186
		public float MovementRadius = -1f;

		// Token: 0x04002BB3 RID: 11187
		public bool ReducedLongRangeAccuracy;

		// Token: 0x04002BB4 RID: 11188
		public ScientistJunkpileSpawner.JunkpileType SpawnType;

		// Token: 0x04002BB5 RID: 11189
		[Range(0f, 1f)]
		public float SpawnBaseChance = 1f;

		// Token: 0x020008E8 RID: 2280
		public enum JunkpileType
		{
			// Token: 0x04002BB7 RID: 11191
			A,
			// Token: 0x04002BB8 RID: 11192
			B,
			// Token: 0x04002BB9 RID: 11193
			C,
			// Token: 0x04002BBA RID: 11194
			D,
			// Token: 0x04002BBB RID: 11195
			E,
			// Token: 0x04002BBC RID: 11196
			F,
			// Token: 0x04002BBD RID: 11197
			G
		}
	}
}
