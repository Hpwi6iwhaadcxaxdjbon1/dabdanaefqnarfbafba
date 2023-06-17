using System;
using UnityEngine;

// Token: 0x020004BF RID: 1215
public class TerrainCheckGeneratorVolumes : MonoBehaviour, IEditorComponent
{
	// Token: 0x040018E6 RID: 6374
	public float PlacementRadius;

	// Token: 0x06001BF9 RID: 7161 RVA: 0x00016E4B File Offset: 0x0001504B
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.PlacementRadius);
	}
}
