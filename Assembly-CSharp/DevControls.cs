using System;
using Facepunch.GUI;
using UnityEngine;

// Token: 0x02000257 RID: 599
[ExecuteInEditMode]
public class DevControls : MonoBehaviour
{
	// Token: 0x04000E67 RID: 3687
	public GUISkin skin;

	// Token: 0x04000E68 RID: 3688
	internal TabbedPanel tabbedPanel;

	// Token: 0x060011B8 RID: 4536 RVA: 0x0000F802 File Offset: 0x0000DA02
	private void OnEnable()
	{
		if (this.tabbedPanel != null)
		{
			this.Init();
		}
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x000757D0 File Offset: 0x000739D0
	private void OnGUI()
	{
		GUI.skin = this.skin;
		if (this.tabbedPanel == null)
		{
			this.Init();
		}
		GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width * 0.3f, (float)Screen.height));
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		this.tabbedPanel.DrawVertical(100f);
		this.tabbedPanel.DrawContents();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x0007584C File Offset: 0x00073A4C
	private void Init()
	{
		GUI.skin = this.skin;
		this.tabbedPanel = new TabbedPanel();
		foreach (DevControlsTab devControlsTab in base.GetComponentsInChildren<DevControlsTab>())
		{
			this.tabbedPanel.Add(new TabbedPanel.Tab
			{
				name = devControlsTab.GetTabName(),
				drawFunc = new Action(devControlsTab.OnTabContents)
			});
		}
	}
}
