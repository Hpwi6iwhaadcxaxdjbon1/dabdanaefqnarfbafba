using System;
using UnityEngine;

// Token: 0x02000255 RID: 597
public class DevBotSpawner : FacepunchBehaviour
{
	// Token: 0x04000E5E RID: 3678
	public GameObjectRef bot;

	// Token: 0x04000E5F RID: 3679
	public Transform waypointParent;

	// Token: 0x04000E60 RID: 3680
	public bool autoSelectLatestSpawnedGameObject = true;

	// Token: 0x04000E61 RID: 3681
	public float spawnRate = 1f;

	// Token: 0x04000E62 RID: 3682
	public int maxPopulation = 1;
}
