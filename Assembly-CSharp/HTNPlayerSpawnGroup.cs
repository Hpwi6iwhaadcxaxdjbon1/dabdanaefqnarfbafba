using System;
using Rust.Ai.HTN;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class HTNPlayerSpawnGroup : SpawnGroup
{
	// Token: 0x0400090B RID: 2315
	[Header("HTN Player Spawn Group")]
	public HTNDomain.MovementRule Movement = HTNDomain.MovementRule.FreeMove;

	// Token: 0x0400090C RID: 2316
	public float MovementRadius = -1f;
}
