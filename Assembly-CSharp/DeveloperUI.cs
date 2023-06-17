using System;
using UnityEngine;

// Token: 0x0200061E RID: 1566
public class DeveloperUI : MonoBehaviour
{
	// Token: 0x06002307 RID: 8967 RVA: 0x0001BC0D File Offset: 0x00019E0D
	public bool IsVisible()
	{
		return LocalPlayer.Entity != null && LocalPlayer.Entity.IsDeveloper;
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x0001BC28 File Offset: 0x00019E28
	public void OnPanelOpened()
	{
		base.gameObject.SetActive(this.IsVisible());
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x0001BC3B File Offset: 0x00019E3B
	public void Update()
	{
		if (!this.IsVisible())
		{
			base.gameObject.SetActive(false);
		}
	}
}
