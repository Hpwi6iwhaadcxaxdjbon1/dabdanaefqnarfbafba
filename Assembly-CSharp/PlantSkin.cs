using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200030A RID: 778
public class PlantSkin : MonoBehaviour, IClientComponent
{
	// Token: 0x04001150 RID: 4432
	internal List<FruitScale> FruitscaleComponents = new List<FruitScale>();

	// Token: 0x04001151 RID: 4433
	internal List<LifeScale> LifescaleComponents = new List<LifeScale>();

	// Token: 0x06001451 RID: 5201 RVA: 0x00011537 File Offset: 0x0000F737
	public void Awake()
	{
		base.GetComponentsInChildren<FruitScale>(true, this.FruitscaleComponents);
		base.GetComponentsInChildren<LifeScale>(true, this.LifescaleComponents);
	}
}
