using System;
using UnityEngine;

// Token: 0x02000665 RID: 1637
public class LootPanelOven : LootPanel
{
	// Token: 0x04002077 RID: 8311
	public GameObject controlsOn;

	// Token: 0x04002078 RID: 8312
	public GameObject controlsOff;

	// Token: 0x06002477 RID: 9335 RVA: 0x000C0CC4 File Offset: 0x000BEEC4
	public override void Update()
	{
		base.Update();
		if (!this.GetBaseOven())
		{
			return;
		}
		this.controlsOn.gameObject.SetActive(this.IsOn());
		this.controlsOff.gameObject.SetActive(!this.IsOn());
	}

	// Token: 0x06002478 RID: 9336 RVA: 0x0001CC36 File Offset: 0x0001AE36
	public BaseOven GetBaseOven()
	{
		return base.GetContainerEntity() as BaseOven;
	}

	// Token: 0x06002479 RID: 9337 RVA: 0x0001CC43 File Offset: 0x0001AE43
	public bool IsOn()
	{
		return this.GetBaseOven().IsOn();
	}

	// Token: 0x0600247A RID: 9338 RVA: 0x0001CC50 File Offset: 0x0001AE50
	public void Switch(bool onOff)
	{
		if (onOff)
		{
			this.GetBaseOven().SwitchOn(LocalPlayer.Entity);
			return;
		}
		this.GetBaseOven().SwitchOff(LocalPlayer.Entity);
	}
}
