using System;
using Rust;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000862 RID: 2146
	[ConsoleSystem.Factory("gc")]
	public class GC : ConsoleSystem
	{
		// Token: 0x04002983 RID: 10627
		private static int m_buffer = 256;

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06002EA7 RID: 11943 RVA: 0x00023ED4 File Offset: 0x000220D4
		// (set) Token: 0x06002EA8 RID: 11944 RVA: 0x00023EDB File Offset: 0x000220DB
		[ClientVar]
		public static int buffer
		{
			get
			{
				return GC.m_buffer;
			}
			set
			{
				GC.m_buffer = Mathf.Clamp(value, 64, 2048);
			}
		}

		// Token: 0x06002EA9 RID: 11945 RVA: 0x00023EEF File Offset: 0x000220EF
		[ServerVar]
		[ClientVar]
		public static void collect()
		{
			GC.Collect();
		}

		// Token: 0x06002EAA RID: 11946 RVA: 0x00023EF6 File Offset: 0x000220F6
		[ServerVar]
		[ClientVar]
		public static void unload()
		{
			Resources.UnloadUnusedAssets();
		}
	}
}
