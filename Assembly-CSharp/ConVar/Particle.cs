using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000875 RID: 2165
	[ConsoleSystem.Factory("particle")]
	public class Particle : ConsoleSystem
	{
		// Token: 0x040029BD RID: 10685
		private static float m_quality = 100f;

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06002F25 RID: 12069 RVA: 0x000243B9 File Offset: 0x000225B9
		// (set) Token: 0x06002F26 RID: 12070 RVA: 0x000243C0 File Offset: 0x000225C0
		[ClientVar(Saved = true)]
		public static float quality
		{
			get
			{
				return Particle.m_quality;
			}
			set
			{
				if (Particle.m_quality != value)
				{
					Particle.m_quality = Mathf.Clamp(value, 0f, 100f);
					if (SingletonComponent<LODGrid>.Instance)
					{
						SingletonComponent<LODGrid>.Instance.Refresh();
					}
				}
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06002F27 RID: 12071 RVA: 0x000243F5 File Offset: 0x000225F5
		public static float lod
		{
			get
			{
				return Particle.quality * 0.01f;
			}
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06002F28 RID: 12072 RVA: 0x00024402 File Offset: 0x00022602
		public static float cull
		{
			get
			{
				return Mathf.Max(1f, Particle.quality * 0.01f);
			}
		}
	}
}
