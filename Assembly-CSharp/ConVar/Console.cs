using System;

namespace ConVar
{
	// Token: 0x02000849 RID: 2121
	[ConsoleSystem.Factory("console")]
	public class Console : ConsoleSystem
	{
		// Token: 0x06002E48 RID: 11848 RVA: 0x00023C32 File Offset: 0x00021E32
		[ClientVar]
		public static void clear(ConsoleSystem.Arg arg)
		{
			SingletonComponent<ConsoleUI>.Instance.ClearContents();
		}

		// Token: 0x06002E49 RID: 11849 RVA: 0x00023C3E File Offset: 0x00021E3E
		[ClientVar]
		public static void copy(ConsoleSystem.Arg arg)
		{
			SingletonComponent<ConsoleUI>.Instance.Copy();
		}
	}
}
