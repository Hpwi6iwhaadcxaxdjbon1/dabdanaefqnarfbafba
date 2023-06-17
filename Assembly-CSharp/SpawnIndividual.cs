using System;
using UnityEngine;

// Token: 0x020003F7 RID: 1015
public struct SpawnIndividual
{
	// Token: 0x04001579 RID: 5497
	public uint PrefabID;

	// Token: 0x0400157A RID: 5498
	public Vector3 Position;

	// Token: 0x0400157B RID: 5499
	public Quaternion Rotation;

	// Token: 0x06001919 RID: 6425 RVA: 0x00014E09 File Offset: 0x00013009
	public SpawnIndividual(uint prefabID, Vector3 position, Quaternion rotation)
	{
		this.PrefabID = prefabID;
		this.Position = position;
		this.Rotation = rotation;
	}
}
