using System;
using UnityEngine;

// Token: 0x020006D8 RID: 1752
public class UIBackgroundBlur : ListComponent<UIBackgroundBlur>, IClientComponent
{
	// Token: 0x040022DC RID: 8924
	public float amount = 1f;

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x060026D5 RID: 9941 RVA: 0x000CB3B4 File Offset: 0x000C95B4
	public static float currentMax
	{
		get
		{
			if (ListComponent<UIBackgroundBlur>.InstanceList.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < ListComponent<UIBackgroundBlur>.InstanceList.Count; i++)
			{
				num = Mathf.Max(ListComponent<UIBackgroundBlur>.InstanceList[i].amount, num);
			}
			return num;
		}
	}
}
