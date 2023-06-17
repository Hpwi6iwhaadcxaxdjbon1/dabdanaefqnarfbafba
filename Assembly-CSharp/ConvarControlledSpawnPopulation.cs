using System;
using UnityEngine;

// Token: 0x020003F4 RID: 1012
[CreateAssetMenu(menuName = "Rust/Convar Controlled Spawn Population")]
public class ConvarControlledSpawnPopulation : SpawnPopulation
{
	// Token: 0x04001577 RID: 5495
	[Header("Convars")]
	public string PopulationConvar;
}
