using System;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class BradleySpawner : MonoBehaviour, IServerComponent
{
	// Token: 0x040007D2 RID: 2002
	public BasePath path;

	// Token: 0x040007D3 RID: 2003
	public GameObjectRef bradleyPrefab;

	// Token: 0x040007D4 RID: 2004
	[NonSerialized]
	public BradleyAPC spawned;

	// Token: 0x040007D5 RID: 2005
	public bool initialSpawn;

	// Token: 0x040007D6 RID: 2006
	public float minRespawnTimeMinutes = 5f;

	// Token: 0x040007D7 RID: 2007
	public float maxRespawnTimeMinutes = 5f;

	// Token: 0x040007D8 RID: 2008
	public static BradleySpawner singleton;
}
