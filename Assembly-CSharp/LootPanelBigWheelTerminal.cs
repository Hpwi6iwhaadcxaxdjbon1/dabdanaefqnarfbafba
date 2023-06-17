using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000EE RID: 238
public class LootPanelBigWheelTerminal : LootPanel
{
	// Token: 0x04000757 RID: 1879
	public Text timeRemainingText;

	// Token: 0x06000B02 RID: 2818 RVA: 0x0000AB3B File Offset: 0x00008D3B
	public BigWheelBettingTerminal GetTerminal()
	{
		return base.GetContainerEntity() as BigWheelBettingTerminal;
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x00056D24 File Offset: 0x00054F24
	public override void Update()
	{
		base.Update();
		float num = this.GetTerminal().nextSpinTime - Time.realtimeSinceStartup;
		this.timeRemainingText.text = ((num <= 0f) ? "..." : num.ToString("N0"));
	}
}
