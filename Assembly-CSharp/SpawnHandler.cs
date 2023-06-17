using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000402 RID: 1026
public class SpawnHandler : SingletonComponent<SpawnHandler>
{
	// Token: 0x0400159D RID: 5533
	public float TickInterval = 60f;

	// Token: 0x0400159E RID: 5534
	public int MinSpawnsPerTick = 100;

	// Token: 0x0400159F RID: 5535
	public int MaxSpawnsPerTick = 100;

	// Token: 0x040015A0 RID: 5536
	public LayerMask PlacementMask;

	// Token: 0x040015A1 RID: 5537
	public LayerMask PlacementCheckMask;

	// Token: 0x040015A2 RID: 5538
	public float PlacementCheckHeight = 25f;

	// Token: 0x040015A3 RID: 5539
	public LayerMask RadiusCheckMask;

	// Token: 0x040015A4 RID: 5540
	public float RadiusCheckDistance = 5f;

	// Token: 0x040015A5 RID: 5541
	public LayerMask BoundsCheckMask;

	// Token: 0x040015A6 RID: 5542
	public SpawnFilter CharacterSpawn;

	// Token: 0x040015A7 RID: 5543
	public SpawnPopulation[] SpawnPopulations;

	// Token: 0x040015A8 RID: 5544
	internal SpawnDistribution[] SpawnDistributions;

	// Token: 0x040015A9 RID: 5545
	internal SpawnDistribution CharDistribution;

	// Token: 0x040015AA RID: 5546
	internal List<ISpawnGroup> SpawnGroups = new List<ISpawnGroup>();

	// Token: 0x040015AB RID: 5547
	internal List<SpawnIndividual> SpawnIndividuals = new List<SpawnIndividual>();

	// Token: 0x040015AC RID: 5548
	[ReadOnly]
	public SpawnPopulation[] ConvarSpawnPopulations;
}
