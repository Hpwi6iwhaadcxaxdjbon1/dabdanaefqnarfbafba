using System;
using UnityEngine;

// Token: 0x02000392 RID: 914
public class FoliageSpawn : MonoBehaviour, IClientComponent
{
	// Token: 0x0400140F RID: 5135
	public FoliagePlacement Placement;

	// Token: 0x0600174A RID: 5962 RVA: 0x000138D5 File Offset: 0x00011AD5
	protected void Awake()
	{
		if (SingletonComponent<FoliageGrid>.Instance)
		{
			SingletonComponent<FoliageGrid>.Instance.AddPlacement(this.Placement);
		}
	}
}
