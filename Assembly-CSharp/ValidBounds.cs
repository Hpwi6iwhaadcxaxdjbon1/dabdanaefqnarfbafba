using System;
using UnityEngine;

// Token: 0x0200040F RID: 1039
public class ValidBounds : SingletonComponent<ValidBounds>
{
	// Token: 0x040015C7 RID: 5575
	public Bounds worldBounds;

	// Token: 0x0600194F RID: 6479 RVA: 0x00014FE8 File Offset: 0x000131E8
	public static bool Test(Vector3 vPos)
	{
		return !SingletonComponent<ValidBounds>.Instance || SingletonComponent<ValidBounds>.Instance.IsInside(vPos);
	}

	// Token: 0x06001950 RID: 6480 RVA: 0x00015003 File Offset: 0x00013203
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(this.worldBounds.center, this.worldBounds.size);
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x0008F634 File Offset: 0x0008D834
	internal bool IsInside(Vector3 vPos)
	{
		if (Vector3Ex.IsNaNOrInfinity(vPos))
		{
			return false;
		}
		if (!this.worldBounds.Contains(vPos))
		{
			return false;
		}
		if (TerrainMeta.Terrain != null)
		{
			if (vPos.y < TerrainMeta.Position.y)
			{
				return false;
			}
			if (TerrainMeta.OutOfMargin(vPos))
			{
				return false;
			}
		}
		return true;
	}
}
