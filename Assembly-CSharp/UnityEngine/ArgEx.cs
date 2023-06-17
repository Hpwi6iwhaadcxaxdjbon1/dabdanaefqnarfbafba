using System;

namespace UnityEngine
{
	// Token: 0x0200082B RID: 2091
	public static class ArgEx
	{
		// Token: 0x06002D73 RID: 11635 RVA: 0x000E4870 File Offset: 0x000E2A70
		public static BasePlayer GetPlayer_Clientside(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, "");
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.Find_Clientside(@string);
		}
	}
}
