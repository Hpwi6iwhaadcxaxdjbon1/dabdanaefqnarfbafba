using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000861 RID: 2145
	[ConsoleSystem.Factory("fps")]
	public class FPS : ConsoleSystem
	{
		// Token: 0x04002982 RID: 10626
		private static int m_graph;

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06002EA2 RID: 11938 RVA: 0x00023EBE File Offset: 0x000220BE
		// (set) Token: 0x06002EA3 RID: 11939 RVA: 0x00023EC5 File Offset: 0x000220C5
		[ClientVar(Saved = true)]
		[ServerVar(Saved = true)]
		public static int limit
		{
			get
			{
				return Application.targetFrameRate;
			}
			set
			{
				Application.targetFrameRate = value;
			}
		}

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06002EA4 RID: 11940 RVA: 0x00023ECD File Offset: 0x000220CD
		// (set) Token: 0x06002EA5 RID: 11941 RVA: 0x000E887C File Offset: 0x000E6A7C
		[ClientVar]
		public static int graph
		{
			get
			{
				return FPS.m_graph;
			}
			set
			{
				FPS.m_graph = value;
				if (!MainCamera.mainCamera)
				{
					return;
				}
				FPSGraph component = MainCamera.mainCamera.GetComponent<FPSGraph>();
				if (!component)
				{
					return;
				}
				component.Refresh();
			}
		}
	}
}
