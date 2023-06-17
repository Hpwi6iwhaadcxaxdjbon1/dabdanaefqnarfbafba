using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000852 RID: 2130
	[ConsoleSystem.Factory("decor")]
	public class Decor : ConsoleSystem
	{
		// Token: 0x04002965 RID: 10597
		private static float m_quality = 100f;

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06002E72 RID: 11890 RVA: 0x00023D20 File Offset: 0x00021F20
		// (set) Token: 0x06002E73 RID: 11891 RVA: 0x00023D27 File Offset: 0x00021F27
		[ClientVar(Saved = true)]
		public static float quality
		{
			get
			{
				return Decor.m_quality;
			}
			set
			{
				value = Mathf.Clamp(value, 0f, 100f);
				if (Decor.m_quality != value)
				{
					Decor.m_quality = value;
					DecorSpawn.RefreshAll(false);
				}
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06002E74 RID: 11892 RVA: 0x00023D4F File Offset: 0x00021F4F
		// (set) Token: 0x06002E75 RID: 11893 RVA: 0x00023D5C File Offset: 0x00021F5C
		public static float quality01
		{
			get
			{
				return Decor.quality * 0.01f;
			}
			set
			{
				Decor.quality = value * 100f;
			}
		}
	}
}
