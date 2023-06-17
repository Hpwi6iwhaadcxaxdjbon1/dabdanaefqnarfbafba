using System;
using UnityEngine;

// Token: 0x02000533 RID: 1331
public class AddToHeightMap : ProceduralObject
{
	// Token: 0x06001E11 RID: 7697 RVA: 0x000A4914 File Offset: 0x000A2B14
	public override void Process()
	{
		Collider component = base.GetComponent<Collider>();
		Bounds bounds = component.bounds;
		int num = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(bounds.min.x));
		int num2 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(bounds.max.x));
		int num3 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(bounds.min.z));
		int num4 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(bounds.max.z));
		for (int i = num3; i <= num4; i++)
		{
			float normZ = TerrainMeta.HeightMap.Coordinate(i);
			for (int j = num; j <= num2; j++)
			{
				float normX = TerrainMeta.HeightMap.Coordinate(j);
				Vector3 origin = new Vector3(TerrainMeta.DenormalizeX(normX), bounds.max.y, TerrainMeta.DenormalizeZ(normZ));
				Ray ray = new Ray(origin, Vector3.down);
				RaycastHit raycastHit;
				if (component.Raycast(ray, ref raycastHit, bounds.size.y))
				{
					float num5 = TerrainMeta.NormalizeY(raycastHit.point.y);
					float height = TerrainMeta.HeightMap.GetHeight01(j, i);
					if (num5 > height)
					{
						TerrainMeta.HeightMap.SetHeight(j, i, num5);
					}
				}
			}
		}
		GameManager.Destroy(this, 0f);
	}
}
