using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000888 RID: 2184
	[ConsoleSystem.Factory("tree")]
	public class Tree : ConsoleSystem
	{
		// Token: 0x04002A2B RID: 10795
		private static float m_quality = 100f;

		// Token: 0x04002A2C RID: 10796
		private static int m_meshes = 100;

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06002F6B RID: 12139 RVA: 0x000246EF File Offset: 0x000228EF
		// (set) Token: 0x06002F6C RID: 12140 RVA: 0x000246F6 File Offset: 0x000228F6
		[ClientVar(Saved = true)]
		public static float quality
		{
			get
			{
				return Tree.m_quality;
			}
			set
			{
				if (Tree.m_quality != value)
				{
					Tree.m_quality = Mathf.Clamp(value, 0f, 1000f);
					if (SingletonComponent<LODGrid>.Instance)
					{
						SingletonComponent<LODGrid>.Instance.Refresh();
					}
				}
			}
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06002F6D RID: 12141 RVA: 0x0002472B File Offset: 0x0002292B
		public static float lod
		{
			get
			{
				return Tree.quality * 0.01f;
			}
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06002F6E RID: 12142 RVA: 0x00024738 File Offset: 0x00022938
		public static float cull
		{
			get
			{
				return Mathf.Max(0.5f, Tree.quality * 0.01f);
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06002F6F RID: 12143 RVA: 0x0002474F File Offset: 0x0002294F
		// (set) Token: 0x06002F70 RID: 12144 RVA: 0x00024756 File Offset: 0x00022956
		[ClientVar(Saved = true)]
		public static int meshes
		{
			get
			{
				return Tree.m_meshes;
			}
			set
			{
				if (Tree.m_meshes != value)
				{
					Tree.m_meshes = Mathf.Clamp(value, 10, 100);
					if (SingletonComponent<LODGrid>.Instance)
					{
						SingletonComponent<LODGrid>.Instance.Refresh();
					}
				}
			}
		}
	}
}
