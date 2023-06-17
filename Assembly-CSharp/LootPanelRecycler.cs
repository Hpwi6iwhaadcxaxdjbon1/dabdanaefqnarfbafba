using System;
using UnityEngine;

// Token: 0x02000666 RID: 1638
public class LootPanelRecycler : LootPanel
{
	// Token: 0x04002079 RID: 8313
	public GameObject controlsOn;

	// Token: 0x0400207A RID: 8314
	public GameObject controlsOff;

	// Token: 0x0600247C RID: 9340 RVA: 0x000C0D14 File Offset: 0x000BEF14
	public override void Update()
	{
		base.Update();
		if (!this.GetRecycler())
		{
			return;
		}
		this.controlsOn.gameObject.SetActive(this.IsOn());
		this.controlsOff.gameObject.SetActive(!this.IsOn());
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x0001CC76 File Offset: 0x0001AE76
	public Recycler GetRecycler()
	{
		return base.GetContainerEntity() as Recycler;
	}

	// Token: 0x0600247E RID: 9342 RVA: 0x0001CC83 File Offset: 0x0001AE83
	public bool IsOn()
	{
		return this.GetRecycler().IsOn();
	}

	// Token: 0x0600247F RID: 9343 RVA: 0x0001CC90 File Offset: 0x0001AE90
	public void Switch(bool onOff)
	{
		if (onOff)
		{
			this.GetRecycler().Menu_TurnOn(LocalPlayer.Entity);
			return;
		}
		this.GetRecycler().Menu_TurnOff(LocalPlayer.Entity);
	}
}
