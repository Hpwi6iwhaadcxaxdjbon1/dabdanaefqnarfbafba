using System;
using Facepunch.GUI;
using UnityEngine;

// Token: 0x0200025B RID: 603
public class DevEnvironment : DevControlsTab
{
	// Token: 0x04000E6E RID: 3694
	public TOD_Sky sky;

	// Token: 0x060011C5 RID: 4549 RVA: 0x0000F858 File Offset: 0x0000DA58
	private void OnEnable()
	{
		if (this.sky)
		{
			this.sky.Cycle.Hour = PlayerPrefs.GetFloat("DevTime");
		}
	}

	// Token: 0x060011C6 RID: 4550 RVA: 0x0000F881 File Offset: 0x0000DA81
	public override string GetTabName()
	{
		return "Environment";
	}

	// Token: 0x060011C7 RID: 4551 RVA: 0x0007599C File Offset: 0x00073B9C
	public override void OnTabContents()
	{
		if (this.sky)
		{
			float num = this.sky.Cycle.Hour;
			num = Controls.FloatSlider("Time Of Day", num, 0f, 24f, "0.00");
			if (num != TOD_Sky.Instance.Cycle.Hour)
			{
				TOD_Sky.Instance.Cycle.Hour = num;
				PlayerPrefs.SetFloat("DevTime", num);
			}
		}
	}
}
