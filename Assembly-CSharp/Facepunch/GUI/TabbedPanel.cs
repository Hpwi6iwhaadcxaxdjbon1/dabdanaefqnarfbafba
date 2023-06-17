using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facepunch.GUI
{
	// Token: 0x020008A4 RID: 2212
	internal class TabbedPanel
	{
		// Token: 0x04002A87 RID: 10887
		private int selectedTabID;

		// Token: 0x04002A88 RID: 10888
		private List<TabbedPanel.Tab> tabs = new List<TabbedPanel.Tab>();

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x06002FCB RID: 12235 RVA: 0x00024AFF File Offset: 0x00022CFF
		public TabbedPanel.Tab selectedTab
		{
			get
			{
				return this.tabs[this.selectedTabID];
			}
		}

		// Token: 0x06002FCC RID: 12236 RVA: 0x00024B12 File Offset: 0x00022D12
		public void Add(TabbedPanel.Tab tab)
		{
			this.tabs.Add(tab);
		}

		// Token: 0x06002FCD RID: 12237 RVA: 0x000EB740 File Offset: 0x000E9940
		internal void DrawVertical(float width)
		{
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(width),
				GUILayout.ExpandHeight(true)
			});
			for (int i = 0; i < this.tabs.Count; i++)
			{
				if (GUILayout.Toggle(this.selectedTabID == i, this.tabs[i].name, new GUIStyle("devtab"), Array.Empty<GUILayoutOption>()))
				{
					this.selectedTabID = i;
				}
			}
			if (GUILayout.Toggle(false, "", new GUIStyle("devtab"), new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true)
			}))
			{
				this.selectedTabID = -1;
			}
			GUILayout.EndVertical();
		}

		// Token: 0x06002FCE RID: 12238 RVA: 0x000EB7F4 File Offset: 0x000E99F4
		internal void DrawContents()
		{
			if (this.selectedTabID < 0)
			{
				return;
			}
			TabbedPanel.Tab selectedTab = this.selectedTab;
			GUILayout.BeginVertical(new GUIStyle("devtabcontents"), new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.ExpandWidth(true)
			});
			if (selectedTab.drawFunc != null)
			{
				selectedTab.drawFunc.Invoke();
			}
			GUILayout.EndVertical();
		}

		// Token: 0x020008A5 RID: 2213
		public struct Tab
		{
			// Token: 0x04002A89 RID: 10889
			public string name;

			// Token: 0x04002A8A RID: 10890
			public Action drawFunc;
		}
	}
}
