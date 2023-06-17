using System;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class HitNumber : MonoBehaviour
{
	// Token: 0x04000750 RID: 1872
	public HitNumber.HitType hitType;

	// Token: 0x06000AFF RID: 2815 RVA: 0x0000AAEF File Offset: 0x00008CEF
	public int ColorToMultiplier(HitNumber.HitType type)
	{
		switch (type)
		{
		case HitNumber.HitType.Yellow:
			return 1;
		case HitNumber.HitType.Green:
			return 3;
		case HitNumber.HitType.Blue:
			return 5;
		case HitNumber.HitType.Purple:
			return 10;
		case HitNumber.HitType.Red:
			return 20;
		default:
			return 0;
		}
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x0000AB1A File Offset: 0x00008D1A
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(base.transform.position, 0.025f);
	}

	// Token: 0x020000ED RID: 237
	public enum HitType
	{
		// Token: 0x04000752 RID: 1874
		Yellow,
		// Token: 0x04000753 RID: 1875
		Green,
		// Token: 0x04000754 RID: 1876
		Blue,
		// Token: 0x04000755 RID: 1877
		Purple,
		// Token: 0x04000756 RID: 1878
		Red
	}
}
