using System;

namespace ConVar
{
	// Token: 0x02000887 RID: 2183
	public class TrackIR
	{
		// Token: 0x06002F68 RID: 12136 RVA: 0x000E9D60 File Offset: 0x000E7F60
		[ClientVar]
		public static void refresh()
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (LocalPlayer.Entity.input == null)
			{
				return;
			}
			if (LocalPlayer.Entity.input.trackir == null)
			{
				return;
			}
			LocalPlayer.Entity.input.trackir.Refresh();
		}

		// Token: 0x06002F69 RID: 12137 RVA: 0x000E9DB4 File Offset: 0x000E7FB4
		[ClientVar]
		public static void recenter()
		{
			if (LocalPlayer.Entity == null)
			{
				return;
			}
			if (LocalPlayer.Entity.input == null)
			{
				return;
			}
			if (LocalPlayer.Entity.input.trackir == null)
			{
				return;
			}
			LocalPlayer.Entity.input.trackir.ReCenter();
		}
	}
}
