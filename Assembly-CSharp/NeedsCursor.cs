using System;
using UnityEngine;

// Token: 0x02000616 RID: 1558
public class NeedsCursor : MonoBehaviour, IClientComponent
{
	// Token: 0x060022F3 RID: 8947 RVA: 0x0001BB4D File Offset: 0x00019D4D
	private void Update()
	{
		CursorManager.HoldOpen(false);
	}
}
