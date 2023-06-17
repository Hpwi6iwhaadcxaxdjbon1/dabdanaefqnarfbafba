using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200014E RID: 334
public class DynamicNavMesh : SingletonComponent<DynamicNavMesh>, IServerComponent
{
	// Token: 0x0400090D RID: 2317
	public int NavMeshAgentTypeIndex;

	// Token: 0x0400090E RID: 2318
	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "Walkable";

	// Token: 0x0400090F RID: 2319
	public int AsyncTerrainNavMeshBakeCellSize = 80;

	// Token: 0x04000910 RID: 2320
	public int AsyncTerrainNavMeshBakeCellHeight = 100;

	// Token: 0x04000911 RID: 2321
	public Bounds Bounds;

	// Token: 0x04000912 RID: 2322
	public NavMeshData NavMeshData;

	// Token: 0x04000913 RID: 2323
	public NavMeshDataInstance NavMeshDataInstance;

	// Token: 0x04000914 RID: 2324
	public LayerMask LayerMask;

	// Token: 0x04000915 RID: 2325
	public NavMeshCollectGeometry NavMeshCollectGeometry;
}
