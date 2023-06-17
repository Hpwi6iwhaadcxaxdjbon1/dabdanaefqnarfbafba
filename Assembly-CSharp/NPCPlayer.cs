using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200012F RID: 303
public class NPCPlayer : BasePlayer
{
	// Token: 0x0400088C RID: 2188
	public Vector3 finalDestination;

	// Token: 0x0400088D RID: 2189
	[NonSerialized]
	private float randomOffset;

	// Token: 0x0400088E RID: 2190
	[NonSerialized]
	private Vector3 spawnPos;

	// Token: 0x0400088F RID: 2191
	public PlayerInventoryProperties[] loadouts;

	// Token: 0x04000890 RID: 2192
	public LayerMask movementMask = 429990145;

	// Token: 0x04000891 RID: 2193
	public NavMeshAgent NavAgent;

	// Token: 0x04000892 RID: 2194
	public float damageScale = 1f;

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x06000BF1 RID: 3057 RVA: 0x00002D44 File Offset: 0x00000F44
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}
}
