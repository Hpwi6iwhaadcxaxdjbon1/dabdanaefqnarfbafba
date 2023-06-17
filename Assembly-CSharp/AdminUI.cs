using System;
using UnityEngine;

// Token: 0x02000604 RID: 1540
public class AdminUI : MonoBehaviour
{
	// Token: 0x06002290 RID: 8848 RVA: 0x0001B772 File Offset: 0x00019972
	public bool IsVisible()
	{
		return LocalPlayer.Entity != null && LocalPlayer.Entity.IsAdmin;
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x0001B78D File Offset: 0x0001998D
	public void OnPanelOpened()
	{
		base.gameObject.SetActive(this.IsVisible());
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x0001B7A0 File Offset: 0x000199A0
	public void Update()
	{
		if (!this.IsVisible())
		{
			base.gameObject.SetActive(false);
		}
	}
}
