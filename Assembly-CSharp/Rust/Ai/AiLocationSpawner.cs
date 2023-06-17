using System;

namespace Rust.Ai
{
	// Token: 0x020008CC RID: 2252
	public class AiLocationSpawner : SpawnGroup
	{
		// Token: 0x04002B46 RID: 11078
		public AiLocationSpawner.SquadSpawnerLocation Location;

		// Token: 0x04002B47 RID: 11079
		public AiLocationManager Manager;

		// Token: 0x04002B48 RID: 11080
		public JunkPile Junkpile;

		// Token: 0x04002B49 RID: 11081
		public bool IsMainSpawner = true;

		// Token: 0x04002B4A RID: 11082
		public float chance = 1f;

		// Token: 0x04002B4B RID: 11083
		private int defaultMaxPopulation;

		// Token: 0x04002B4C RID: 11084
		private int defaultNumToSpawnPerTickMax;

		// Token: 0x04002B4D RID: 11085
		private int defaultNumToSpawnPerTickMin;

		// Token: 0x020008CD RID: 2253
		public enum SquadSpawnerLocation
		{
			// Token: 0x04002B4F RID: 11087
			MilitaryTunnels,
			// Token: 0x04002B50 RID: 11088
			JunkpileA,
			// Token: 0x04002B51 RID: 11089
			JunkpileG,
			// Token: 0x04002B52 RID: 11090
			CH47,
			// Token: 0x04002B53 RID: 11091
			None,
			// Token: 0x04002B54 RID: 11092
			Compound,
			// Token: 0x04002B55 RID: 11093
			BanditTown,
			// Token: 0x04002B56 RID: 11094
			CargoShip
		}
	}
}
