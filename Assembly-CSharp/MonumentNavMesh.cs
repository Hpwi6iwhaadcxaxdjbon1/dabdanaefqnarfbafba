using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200014F RID: 335
public class MonumentNavMesh : FacepunchBehaviour, IServerComponent
{
	// Token: 0x04000916 RID: 2326
	public int NavMeshAgentTypeIndex;

	// Token: 0x04000917 RID: 2327
	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "HumanNPC";

	// Token: 0x04000918 RID: 2328
	public int CellCount = 1;

	// Token: 0x04000919 RID: 2329
	public int CellSize = 80;

	// Token: 0x0400091A RID: 2330
	public int Height = 100;

	// Token: 0x0400091B RID: 2331
	public float NavmeshResolutionModifier = 0.5f;

	// Token: 0x0400091C RID: 2332
	public Bounds Bounds;

	// Token: 0x0400091D RID: 2333
	public NavMeshData NavMeshData;

	// Token: 0x0400091E RID: 2334
	public NavMeshDataInstance NavMeshDataInstance;

	// Token: 0x0400091F RID: 2335
	public LayerMask LayerMask;

	// Token: 0x04000920 RID: 2336
	public NavMeshCollectGeometry NavMeshCollectGeometry;
}
