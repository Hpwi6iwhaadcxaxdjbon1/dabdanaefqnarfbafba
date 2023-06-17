using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class SnowMachine : FogMachine
{
	// Token: 0x0400078F RID: 1935
	public AdaptMeshToTerrain snowMesh;

	// Token: 0x04000790 RID: 1936
	public TriggerTemperature tempTrigger;

	// Token: 0x06000B34 RID: 2868 RVA: 0x0000508F File Offset: 0x0000328F
	public override bool MotionModeEnabled()
	{
		return false;
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x00005E1B File Offset: 0x0000401B
	public override void PostNetworkUpdate()
	{
		base.PostNetworkUpdate();
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00057C80 File Offset: 0x00055E80
	protected override void ClientInit(Entity info)
	{
		base.ClientInit(info);
		Vector3 position = this.snowMesh.transform.position;
		position.y = TerrainMeta.HeightMap.GetHeight(position);
		this.snowMesh.transform.position = position;
	}
}
