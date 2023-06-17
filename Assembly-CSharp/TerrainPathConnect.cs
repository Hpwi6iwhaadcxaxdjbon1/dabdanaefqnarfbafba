using System;
using UnityEngine;

// Token: 0x020004B6 RID: 1206
public class TerrainPathConnect : MonoBehaviour
{
	// Token: 0x040018CF RID: 6351
	public InfrastructureType Type;

	// Token: 0x06001BE7 RID: 7143 RVA: 0x0009A114 File Offset: 0x00098314
	public PathFinder.Point GetPoint(int res)
	{
		Vector3 position = base.transform.position;
		float num = TerrainMeta.NormalizeX(position.x);
		float num2 = TerrainMeta.NormalizeZ(position.z);
		return new PathFinder.Point
		{
			x = Mathf.Clamp((int)(num * (float)res), 0, res - 1),
			y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1)
		};
	}
}
