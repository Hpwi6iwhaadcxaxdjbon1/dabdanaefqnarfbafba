using System;
using UnityEngine;

// Token: 0x0200077C RID: 1916
public class SwapKeycard : MonoBehaviour
{
	// Token: 0x040024C4 RID: 9412
	public GameObject[] accessLevels;

	// Token: 0x060029B9 RID: 10681 RVA: 0x000D41B0 File Offset: 0x000D23B0
	public void UpdateAccessLevel(int level)
	{
		GameObject[] array = this.accessLevels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		this.accessLevels[level - 1].SetActive(true);
	}
}
