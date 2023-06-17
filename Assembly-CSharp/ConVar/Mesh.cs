using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x0200086F RID: 2159
	[ConsoleSystem.Factory("mesh")]
	public class Mesh : ConsoleSystem
	{
		// Token: 0x040029B3 RID: 10675
		private static float m_quality = 100f;

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06002F12 RID: 12050 RVA: 0x0002430B File Offset: 0x0002250B
		// (set) Token: 0x06002F13 RID: 12051 RVA: 0x00024312 File Offset: 0x00022512
		[ClientVar(Saved = true)]
		public static float quality
		{
			get
			{
				return Mesh.m_quality;
			}
			set
			{
				if (Mesh.m_quality != value)
				{
					Mesh.m_quality = Mathf.Clamp(value, 0f, 1000f);
					if (SingletonComponent<LODGrid>.Instance)
					{
						SingletonComponent<LODGrid>.Instance.Refresh();
					}
				}
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06002F14 RID: 12052 RVA: 0x00024347 File Offset: 0x00022547
		public static float lod
		{
			get
			{
				return Mesh.quality * 0.01f;
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06002F15 RID: 12053 RVA: 0x00024354 File Offset: 0x00022554
		public static float cull
		{
			get
			{
				return Mathf.Max(1f, Mesh.quality * 0.01f);
			}
		}
	}
}
