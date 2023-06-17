using System;
using Rust.Workshop;

namespace ConVar
{
	// Token: 0x0200088E RID: 2190
	[ConsoleSystem.Factory("workshop")]
	public class Workshop : ConsoleSystem
	{
		// Token: 0x06002F7C RID: 12156 RVA: 0x000247D3 File Offset: 0x000229D3
		[ClientVar]
		public static void print_loaded_skins(ConsoleSystem.Arg args)
		{
			args.ReplyWith(WorkshopSkin.GetStatus());
		}
	}
}
