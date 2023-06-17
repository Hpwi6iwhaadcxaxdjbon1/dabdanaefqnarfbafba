using System;

// Token: 0x0200034C RID: 844
public class DoorManipulator : IOEntity
{
	// Token: 0x04001300 RID: 4864
	public EntityRef entityRef;

	// Token: 0x04001301 RID: 4865
	public Door targetDoor;

	// Token: 0x04001302 RID: 4866
	public DoorManipulator.DoorEffect powerAction;

	// Token: 0x06001609 RID: 5641 RVA: 0x00002D44 File Offset: 0x00000F44
	public virtual bool PairWithLockedDoors()
	{
		return true;
	}

	// Token: 0x0200034D RID: 845
	public enum DoorEffect
	{
		// Token: 0x04001304 RID: 4868
		Close,
		// Token: 0x04001305 RID: 4869
		Open,
		// Token: 0x04001306 RID: 4870
		Toggle
	}
}
