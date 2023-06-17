using System;
using UnityEngine;

// Token: 0x020004BB RID: 1211
public class TerrainAnchorGenerator : MonoBehaviour, IEditorComponent
{
	// Token: 0x040018D5 RID: 6357
	public float PlacementRadius = 32f;

	// Token: 0x040018D6 RID: 6358
	public float PlacementPadding;

	// Token: 0x040018D7 RID: 6359
	public float PlacementFade = 16f;

	// Token: 0x040018D8 RID: 6360
	public float PlacementDistance = 8f;

	// Token: 0x040018D9 RID: 6361
	public float AnchorExtentsMin = 8f;

	// Token: 0x040018DA RID: 6362
	public float AnchorExtentsMax = 16f;

	// Token: 0x040018DB RID: 6363
	public float AnchorOffsetMin;

	// Token: 0x040018DC RID: 6364
	public float AnchorOffsetMax;
}
