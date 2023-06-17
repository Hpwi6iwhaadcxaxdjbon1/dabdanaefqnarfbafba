using System;
using UnityEngine;

// Token: 0x020004E7 RID: 1255
public class TerrainMargin
{
	// Token: 0x06001D0D RID: 7437 RVA: 0x0009F4FC File Offset: 0x0009D6FC
	public static void Create()
	{
		Material materialTemplate = TerrainMeta.Terrain.materialTemplate;
		Vector3 center = TerrainMeta.Center;
		Vector3 size = TerrainMeta.Size;
		Vector3 b = new Vector3(size.x, 0f, 0f);
		Vector3 b2 = new Vector3(0f, 0f, size.z);
		center.y = TerrainMeta.HeightMap.GetHeight(0, 0);
		TerrainMargin.Create(center - b2, size, materialTemplate);
		TerrainMargin.Create(center - b2 - b, size, materialTemplate);
		TerrainMargin.Create(center - b2 + b, size, materialTemplate);
		TerrainMargin.Create(center - b, size, materialTemplate);
		TerrainMargin.Create(center + b, size, materialTemplate);
		TerrainMargin.Create(center + b2, size, materialTemplate);
		TerrainMargin.Create(center + b2 - b, size, materialTemplate);
		TerrainMargin.Create(center + b2 + b, size, materialTemplate);
	}

	// Token: 0x06001D0E RID: 7438 RVA: 0x0009F5F0 File Offset: 0x0009D7F0
	private static void Create(Vector3 position, Vector3 size, Material material)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.name = "TerrainMargin";
		gameObject.layer = 16;
		gameObject.transform.position = position;
		gameObject.transform.localScale = size * 0.1f;
		gameObject.GetComponent<MeshRenderer>().sharedMaterial = material;
	}
}
