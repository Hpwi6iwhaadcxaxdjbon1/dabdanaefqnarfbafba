using System;
using UnityEngine;

// Token: 0x020004BE RID: 1214
public class TerrainCheckGenerator : MonoBehaviour, IEditorComponent
{
	// Token: 0x040018DF RID: 6367
	public float PlacementRadius = 32f;

	// Token: 0x040018E0 RID: 6368
	public float PlacementPadding;

	// Token: 0x040018E1 RID: 6369
	public float PlacementFade = 16f;

	// Token: 0x040018E2 RID: 6370
	public float PlacementDistance = 8f;

	// Token: 0x040018E3 RID: 6371
	public float CheckExtentsMin = 8f;

	// Token: 0x040018E4 RID: 6372
	public float CheckExtentsMax = 16f;

	// Token: 0x040018E5 RID: 6373
	public bool CheckRotate = true;
}
