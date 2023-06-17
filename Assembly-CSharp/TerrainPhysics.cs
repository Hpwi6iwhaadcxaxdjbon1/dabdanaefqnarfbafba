using System;
using UnityEngine;

// Token: 0x020004ED RID: 1261
public class TerrainPhysics : TerrainExtension
{
	// Token: 0x040019B0 RID: 6576
	private TerrainSplatMap splat;

	// Token: 0x040019B1 RID: 6577
	private PhysicMaterial[] materials;

	// Token: 0x06001D5A RID: 7514 RVA: 0x000178AB File Offset: 0x00015AAB
	public override void Setup()
	{
		this.splat = this.terrain.GetComponent<TerrainSplatMap>();
		this.materials = this.config.GetPhysicMaterials();
	}

	// Token: 0x06001D5B RID: 7515 RVA: 0x000178CF File Offset: 0x00015ACF
	public PhysicMaterial GetMaterial(Vector3 worldPos)
	{
		return this.materials[this.splat.GetSplatMaxIndex(worldPos, -1)];
	}
}
