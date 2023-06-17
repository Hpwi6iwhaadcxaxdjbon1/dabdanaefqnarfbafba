using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200051C RID: 1308
public class GenerateRoadMeshes : ProceduralComponent
{
	// Token: 0x04001A32 RID: 6706
	public Material RoadMaterial;

	// Token: 0x04001A33 RID: 6707
	public PhysicMaterial RoadPhysicMaterial;

	// Token: 0x06001DE0 RID: 7648 RVA: 0x000A3038 File Offset: 0x000A1238
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Roads)
		{
			foreach (Mesh sharedMesh in pathList.CreateMesh())
			{
				GameObject gameObject = new GameObject("Road Mesh");
				gameObject.AddComponent<MeshFilter>().sharedMesh = sharedMesh;
				MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = this.RoadMaterial;
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
				meshCollider.sharedMaterial = this.RoadPhysicMaterial;
				meshCollider.sharedMesh = sharedMesh;
				gameObject.AddComponent<AddToHeightMap>();
				gameObject.layer = 16;
				gameObject.SetHierarchyGroup(pathList.Name, true, false);
			}
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x06001DE1 RID: 7649 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
