using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000645 RID: 1605
public class BlueprintHeader : MonoBehaviour
{
	// Token: 0x04001FED RID: 8173
	public Text categoryName;

	// Token: 0x04001FEE RID: 8174
	public Text unlockCount;

	// Token: 0x060023C8 RID: 9160 RVA: 0x0001C452 File Offset: 0x0001A652
	public void Setup(ItemCategory name, int unlocked, int total)
	{
		this.categoryName.text = name.ToString().ToUpper();
		this.unlockCount.text = string.Format("UNLOCKED {0}/{1}", unlocked, total);
	}
}
