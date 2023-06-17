using System;
using Facepunch;

namespace ConVar
{
	// Token: 0x0200086A RID: 2154
	[ConsoleSystem.Factory("input")]
	public class Input : ConsoleSystem
	{
		// Token: 0x040029A9 RID: 10665
		[ClientVar(Saved = true)]
		public static bool flipy = false;

		// Token: 0x040029AA RID: 10666
		[ClientVar(Saved = true)]
		public static bool autocrouch = false;

		// Token: 0x040029AB RID: 10667
		[ClientVar(Saved = true)]
		public static float sensitivity = 1f;

		// Token: 0x040029AC RID: 10668
		[ClientVar(Saved = true)]
		public static float vehicle_sensitivity = 1f;

		// Token: 0x040029AD RID: 10669
		[ClientVar(Saved = true)]
		public static bool vehicle_flipy = false;

		// Token: 0x040029AE RID: 10670
		[ClientVar(Saved = true)]
		public static float holdtime = 0.2f;

		// Token: 0x06002EFC RID: 12028 RVA: 0x000E9038 File Offset: 0x000E7238
		[ClientVar]
		public static string bind(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, "");
			if (arg.FullString.Length == @string.Length)
			{
				return Input.GetBind(@string);
			}
			string text = arg.FullString.Substring(@string.Length).Trim().Trim(new char[]
			{
				'"'
			});
			Input.SetBind(@string, text);
			ConsoleSystem.HasChanges = true;
			return null;
		}

		// Token: 0x06002EFD RID: 12029 RVA: 0x00024255 File Offset: 0x00022455
		[ClientVar]
		public static void clearbinds(ConsoleSystem.Arg arg)
		{
			Input.ClearBinds();
		}
	}
}
