using System;
using UnityEngine;

// Token: 0x020003F5 RID: 1013
public class Spawnable : MonoBehaviour, IServerComponent
{
	// Token: 0x04001578 RID: 5496
	[ReadOnly]
	public SpawnPopulation Population;

	// Token: 0x06001916 RID: 6422 RVA: 0x00014E00 File Offset: 0x00013000
	protected void OnValidate()
	{
		this.Population = null;
	}
}
