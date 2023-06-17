using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200032A RID: 810
[CreateAssetMenu(menuName = "Rust/TrappableWildlife")]
[Serializable]
public class TrappableWildlife : ScriptableObject
{
	// Token: 0x04001269 RID: 4713
	public GameObjectRef worldObject;

	// Token: 0x0400126A RID: 4714
	public ItemDefinition inventoryObject;

	// Token: 0x0400126B RID: 4715
	public int minToCatch;

	// Token: 0x0400126C RID: 4716
	public int maxToCatch;

	// Token: 0x0400126D RID: 4717
	public List<TrappableWildlife.BaitType> baitTypes;

	// Token: 0x0400126E RID: 4718
	public int caloriesForInterest = 20;

	// Token: 0x0400126F RID: 4719
	public float successRate = 1f;

	// Token: 0x04001270 RID: 4720
	public float xpScale = 1f;

	// Token: 0x0200032B RID: 811
	[Serializable]
	public class BaitType
	{
		// Token: 0x04001271 RID: 4721
		public float successRate = 1f;

		// Token: 0x04001272 RID: 4722
		public ItemDefinition bait;

		// Token: 0x04001273 RID: 4723
		public int minForInterest = 1;

		// Token: 0x04001274 RID: 4724
		public int maxToConsume = 1;
	}
}
