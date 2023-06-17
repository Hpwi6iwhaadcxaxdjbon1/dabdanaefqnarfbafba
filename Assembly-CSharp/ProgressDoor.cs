using System;

// Token: 0x02000356 RID: 854
public class ProgressDoor : IOEntity
{
	// Token: 0x0400130E RID: 4878
	public float storedEnergy;

	// Token: 0x0400130F RID: 4879
	public float energyForOpen = 1f;

	// Token: 0x04001310 RID: 4880
	public float secondsToClose = 1f;

	// Token: 0x04001311 RID: 4881
	public float openProgress;

	// Token: 0x06001616 RID: 5654 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void UpdateProgress()
	{
	}
}
