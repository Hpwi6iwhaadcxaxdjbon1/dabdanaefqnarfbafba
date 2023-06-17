using System;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class PathSpeedZone : MonoBehaviour
{
	// Token: 0x040007DA RID: 2010
	public Bounds bounds;

	// Token: 0x040007DB RID: 2011
	public OBB obbBounds;

	// Token: 0x040007DC RID: 2012
	public float maxVelocityPerSec = 5f;

	// Token: 0x06000B67 RID: 2919 RVA: 0x0000AF20 File Offset: 0x00009120
	public OBB WorldSpaceBounds()
	{
		return new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x0000AF4E File Offset: 0x0000914E
	public float GetMaxSpeed()
	{
		return this.maxVelocityPerSec;
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x00058AA8 File Offset: 0x00056CA8
	public virtual void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
		Gizmos.color = new Color(1f, 0.7f, 0f, 1f);
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
	}
}
