using System;

// Token: 0x020003FB RID: 1019
public interface ISpawnGroup
{
	// Token: 0x0600191E RID: 6430
	void Clear();

	// Token: 0x0600191F RID: 6431
	void Fill();

	// Token: 0x06001920 RID: 6432
	void SpawnInitial();

	// Token: 0x06001921 RID: 6433
	void SpawnRepeating();

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06001922 RID: 6434
	int currentPopulation { get; }
}
