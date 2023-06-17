using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000866 RID: 2150
	[ConsoleSystem.Factory("grass")]
	public class Grass : ConsoleSystem
	{
		// Token: 0x040029A7 RID: 10663
		[ClientVar(Saved = true)]
		public static bool displace = false;

		// Token: 0x040029A8 RID: 10664
		private static float m_quality = 100f;

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06002EF3 RID: 12019 RVA: 0x000241EB File Offset: 0x000223EB
		// (set) Token: 0x06002EF4 RID: 12020 RVA: 0x000241F2 File Offset: 0x000223F2
		[ClientVar(Saved = true)]
		public static float quality
		{
			get
			{
				return Grass.m_quality;
			}
			set
			{
				if (Grass.m_quality != value)
				{
					Grass.m_quality = Mathf.Clamp(value, 0f, 100f);
					if (SingletonComponent<FoliageGrid>.Instance)
					{
						SingletonComponent<FoliageGrid>.Instance.Refresh(false);
					}
				}
			}
		}

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06002EF5 RID: 12021 RVA: 0x00024228 File Offset: 0x00022428
		// (set) Token: 0x06002EF6 RID: 12022 RVA: 0x00024235 File Offset: 0x00022435
		public static float quality01
		{
			get
			{
				return Grass.quality * 0.01f;
			}
			set
			{
				Grass.quality = value * 100f;
			}
		}
	}
}
