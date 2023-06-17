using System;
using UnityEngine;

// Token: 0x020004B7 RID: 1207
public class ApplyTerrainAnchors : MonoBehaviour
{
	// Token: 0x06001BE9 RID: 7145 RVA: 0x0009A178 File Offset: 0x00098378
	protected void Awake()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		TerrainAnchor[] anchors = null;
		if (component.isClient)
		{
			anchors = PrefabAttribute.client.FindAll<TerrainAnchor>(component.prefabID);
		}
		base.transform.ApplyTerrainAnchors(anchors);
		GameManager.Destroy(this, 0f);
	}
}
