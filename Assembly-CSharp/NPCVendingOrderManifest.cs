using System;
using UnityEngine;

// Token: 0x020000DA RID: 218
[CreateAssetMenu(menuName = "Rust/NPCVendingOrderManifest")]
public class NPCVendingOrderManifest : ScriptableObject
{
	// Token: 0x040006F2 RID: 1778
	public NPCVendingOrder[] orderList;

	// Token: 0x06000A85 RID: 2693 RVA: 0x00055838 File Offset: 0x00053A38
	public int GetIndex(NPCVendingOrder sample)
	{
		for (int i = 0; i < this.orderList.Length; i++)
		{
			NPCVendingOrder y = this.orderList[i];
			if (sample == y)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x0000A5B4 File Offset: 0x000087B4
	public NPCVendingOrder GetFromIndex(int index)
	{
		return this.orderList[index];
	}
}
