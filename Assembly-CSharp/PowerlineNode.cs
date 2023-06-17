using System;
using UnityEngine;

// Token: 0x020004B4 RID: 1204
public class PowerlineNode : MonoBehaviour
{
	// Token: 0x040018C6 RID: 6342
	public Material WireMaterial;

	// Token: 0x040018C7 RID: 6343
	public float MaxDistance = 50f;

	// Token: 0x06001BE2 RID: 7138 RVA: 0x00016D6A File Offset: 0x00014F6A
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.AddWire(this);
		}
	}
}
