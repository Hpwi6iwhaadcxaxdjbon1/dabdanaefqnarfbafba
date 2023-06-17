using System;
using Facepunch.GUI;
using UnityEngine;

// Token: 0x0200025A RID: 602
public class DevEnableDisable : DevControlsTab
{
	// Token: 0x04000E6B RID: 3691
	public GameObject[] Objects;

	// Token: 0x04000E6C RID: 3692
	public string CookieName = "Cookie";

	// Token: 0x04000E6D RID: 3693
	public string TabName = "Scene";

	// Token: 0x060011C0 RID: 4544 RVA: 0x0000F819 File Offset: 0x0000DA19
	private void Start()
	{
		base.Invoke(new Action(this.ApplyLastSettings), 0.5f);
	}

	// Token: 0x060011C1 RID: 4545 RVA: 0x000758C0 File Offset: 0x00073AC0
	private void ApplyLastSettings()
	{
		foreach (GameObject gameObject in this.Objects)
		{
			if (!(gameObject == null))
			{
				int @int = PlayerPrefs.GetInt("DevEnable_" + this.CookieName + "_" + gameObject.name);
				if (@int > 0)
				{
					gameObject.SetActive(@int == 1);
				}
			}
		}
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x0000F832 File Offset: 0x0000DA32
	public override string GetTabName()
	{
		return this.TabName;
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x00075920 File Offset: 0x00073B20
	public override void OnTabContents()
	{
		foreach (GameObject gameObject in this.Objects)
		{
			if (!(gameObject == null))
			{
				bool activeSelf = gameObject.activeSelf;
				bool flag = Controls.Checkbox(gameObject.name, activeSelf);
				if (activeSelf != flag)
				{
					gameObject.SetActive(flag);
					PlayerPrefs.SetInt("DevEnable_" + this.CookieName + "_" + gameObject.name, flag ? 1 : 2);
				}
			}
		}
	}
}
