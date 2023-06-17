using System;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class CanvasOrderHack : MonoBehaviour
{
	// Token: 0x06000ECB RID: 3787 RVA: 0x000669C8 File Offset: 0x00064BC8
	private void OnEnable()
	{
		foreach (Canvas canvas in base.GetComponentsInChildren<Canvas>(true))
		{
			if (canvas.overrideSorting)
			{
				Canvas canvas2 = canvas;
				int sortingOrder = canvas2.sortingOrder;
				canvas2.sortingOrder = sortingOrder + 1;
			}
		}
		foreach (Canvas canvas3 in base.GetComponentsInChildren<Canvas>(true))
		{
			if (canvas3.overrideSorting)
			{
				Canvas canvas4 = canvas3;
				int sortingOrder = canvas4.sortingOrder;
				canvas4.sortingOrder = sortingOrder - 1;
			}
		}
	}
}
