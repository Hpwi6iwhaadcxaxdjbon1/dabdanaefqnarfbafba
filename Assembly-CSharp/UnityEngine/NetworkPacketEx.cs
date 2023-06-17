using System;
using Network;

namespace UnityEngine
{
	// Token: 0x02000833 RID: 2099
	public static class NetworkPacketEx
	{
		// Token: 0x06002D80 RID: 11648 RVA: 0x00023634 File Offset: 0x00021834
		public static BasePlayer Player(this Message v)
		{
			if (v.connection == null)
			{
				return null;
			}
			return v.connection.player as BasePlayer;
		}
	}
}
