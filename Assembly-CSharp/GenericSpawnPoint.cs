using System;
using UnityEngine.Events;

// Token: 0x020003FA RID: 1018
public class GenericSpawnPoint : BaseSpawnPoint
{
	// Token: 0x0400158A RID: 5514
	public bool dropToGround = true;

	// Token: 0x0400158B RID: 5515
	public bool randomRot;

	// Token: 0x0400158C RID: 5516
	public GameObjectRef spawnEffect;

	// Token: 0x0400158D RID: 5517
	public UnityEvent OnObjectSpawnedEvent = new UnityEvent();

	// Token: 0x0400158E RID: 5518
	public UnityEvent OnObjectRetiredEvent = new UnityEvent();
}
