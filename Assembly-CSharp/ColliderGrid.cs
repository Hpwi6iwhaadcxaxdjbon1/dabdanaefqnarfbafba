using System;

// Token: 0x0200023A RID: 570
public class ColliderGrid : SingletonComponent<ColliderGrid>, IServerComponent
{
	// Token: 0x04000DDF RID: 3551
	public static bool Paused;

	// Token: 0x04000DE0 RID: 3552
	public GameObjectRef BatchPrefab;

	// Token: 0x04000DE1 RID: 3553
	public float CellSize = 50f;

	// Token: 0x04000DE2 RID: 3554
	public float MaxMilliseconds = 0.1f;
}
