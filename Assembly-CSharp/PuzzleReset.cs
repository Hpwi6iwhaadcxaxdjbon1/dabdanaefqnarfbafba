using System;
using UnityEngine;

// Token: 0x02000358 RID: 856
public class PuzzleReset : FacepunchBehaviour
{
	// Token: 0x04001316 RID: 4886
	public SpawnGroup[] respawnGroups;

	// Token: 0x04001317 RID: 4887
	public bool playersBlockReset;

	// Token: 0x04001318 RID: 4888
	public float playerDetectionRadius;

	// Token: 0x04001319 RID: 4889
	public Transform playerDetectionOrigin;

	// Token: 0x0400131A RID: 4890
	public float timeBetweenResets = 30f;

	// Token: 0x0400131B RID: 4891
	public bool scaleWithServerPopulation;
}
