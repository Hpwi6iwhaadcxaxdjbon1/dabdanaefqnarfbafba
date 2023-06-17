using System;
using UnityEngine;

// Token: 0x02000419 RID: 1049
public class WearableHolsterOffset : MonoBehaviour
{
	// Token: 0x040015FE RID: 5630
	public WearableHolsterOffset.offsetInfo[] Offsets;

	// Token: 0x0200041A RID: 1050
	[Serializable]
	public class offsetInfo
	{
		// Token: 0x040015FF RID: 5631
		public HeldEntity.HolsterInfo.HolsterSlot type;

		// Token: 0x04001600 RID: 5632
		public Vector3 offset;

		// Token: 0x04001601 RID: 5633
		public Vector3 rotationOffset;

		// Token: 0x04001602 RID: 5634
		public int priority;
	}
}
