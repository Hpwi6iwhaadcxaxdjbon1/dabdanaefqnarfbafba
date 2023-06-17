using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008CA RID: 2250
	public class ScientistSpawner : SpawnGroup
	{
		// Token: 0x04002B32 RID: 11058
		[Header("Scientist Spawner")]
		public bool Mobile = true;

		// Token: 0x04002B33 RID: 11059
		public bool NeverMove;

		// Token: 0x04002B34 RID: 11060
		public bool SpawnHostile;

		// Token: 0x04002B35 RID: 11061
		public bool OnlyAggroMarkedTargets = true;

		// Token: 0x04002B36 RID: 11062
		public bool IsPeacekeeper = true;

		// Token: 0x04002B37 RID: 11063
		public bool IsBandit;

		// Token: 0x04002B38 RID: 11064
		public bool IsMilitaryTunnelLab;

		// Token: 0x04002B39 RID: 11065
		public NPCPlayerApex.EnemyRangeEnum MaxRangeToSpawnLoc = NPCPlayerApex.EnemyRangeEnum.LongAttackRange;

		// Token: 0x04002B3A RID: 11066
		public WaypointSet Waypoints;

		// Token: 0x04002B3B RID: 11067
		public Transform[] LookAtInterestPointsStationary;

		// Token: 0x04002B3C RID: 11068
		public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

		// Token: 0x04002B3D RID: 11069
		public Model Model;

		// Token: 0x04002B3E RID: 11070
		[SerializeField]
		private AiLocationManager _mgr;
	}
}
