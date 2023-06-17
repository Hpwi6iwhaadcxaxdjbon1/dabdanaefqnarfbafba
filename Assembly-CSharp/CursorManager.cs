using System;
using UnityEngine;

// Token: 0x02000615 RID: 1557
public class CursorManager : SingletonComponent<CursorManager>
{
	// Token: 0x04001F24 RID: 7972
	private static int iHoldOpen;

	// Token: 0x04001F25 RID: 7973
	private static int iPreviousOpen;

	// Token: 0x060022ED RID: 8941 RVA: 0x0001BAC2 File Offset: 0x00019CC2
	private void Update()
	{
		if (SingletonComponent<CursorManager>.Instance != this)
		{
			return;
		}
		if (CursorManager.iHoldOpen == 0 && CursorManager.iPreviousOpen == 0)
		{
			this.SwitchToGame();
		}
		else
		{
			this.SwitchToUI();
		}
		CursorManager.iPreviousOpen = CursorManager.iHoldOpen;
		CursorManager.iHoldOpen = 0;
	}

	// Token: 0x060022EE RID: 8942 RVA: 0x0001BAFE File Offset: 0x00019CFE
	private void SwitchToGame()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (Cursor.visible)
		{
			Cursor.visible = false;
		}
	}

	// Token: 0x060022EF RID: 8943 RVA: 0x0001BB1B File Offset: 0x00019D1B
	private void SwitchToUI()
	{
		if (Cursor.lockState != CursorLockMode.None)
		{
			Cursor.lockState = CursorLockMode.None;
		}
		if (!Cursor.visible)
		{
			Cursor.visible = true;
		}
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x0001BB37 File Offset: 0x00019D37
	public static void HoldOpen(bool cursorVisible = false)
	{
		CursorManager.iHoldOpen++;
	}
}
