using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000510 RID: 1296
public class GenerateRiverMeshes : ProceduralComponent
{
	// Token: 0x04001A14 RID: 6676
	public Material RiverMaterial;

	// Token: 0x04001A15 RID: 6677
	public PhysicMaterial RiverPhysicMaterial;

	// Token: 0x06001DC6 RID: 7622 RVA: 0x000A2470 File Offset: 0x000A0670
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rivers)
		{
			foreach (Mesh sharedMesh in pathList.CreateMesh())
			{
				GameObject gameObject = new GameObject("River Mesh");
				gameObject.AddComponent<MeshFilter>().sharedMesh = sharedMesh;
				MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = this.RiverMaterial;
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
				meshCollider.sharedMaterial = this.RiverPhysicMaterial;
				meshCollider.sharedMesh = sharedMesh;
				gameObject.AddComponent<RiverInfo>();
				gameObject.AddComponent<WaterBody>();
				gameObject.AddComponent<AddToWaterMap>();
				gameObject.tag = "River";
				gameObject.layer = 4;
				gameObject.SetHierarchyGroup(pathList.Name, true, false);
			}
		}
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06001DC7 RID: 7623 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
