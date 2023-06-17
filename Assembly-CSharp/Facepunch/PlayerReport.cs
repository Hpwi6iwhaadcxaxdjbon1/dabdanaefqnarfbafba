using System;
using System.Collections.Generic;
using Facepunch.Models;
using Facepunch.Steamworks;
using UnityEngine;

namespace Facepunch
{
	// Token: 0x0200089F RID: 2207
	public static class PlayerReport
	{
		// Token: 0x06002FAD RID: 12205 RVA: 0x0002495D File Offset: 0x00022B5D
		[RuntimeInitializeOnLoadMethod]
		public static void SetupPlayerReportCallback()
		{
			Feedback.GetPlayersForFeedback = new Action<List<PlayerInfo>>(PlayerReport.GetPlayersForFeedback);
		}

		// Token: 0x06002FAE RID: 12206 RVA: 0x000EAE70 File Offset: 0x000E9070
		private static void GetPlayersForFeedback(List<PlayerInfo> obj)
		{
			foreach (ulong num in SteamFriendsList.SeenList)
			{
				SteamFriend steamFriend = Client.Steam.Friends.Get(num);
				if (steamFriend != null)
				{
					int num2 = 0;
					PlayerInfo playerInfo = default(PlayerInfo);
					playerInfo.Id = steamFriend.Id.ToString();
					playerInfo.Name = steamFriend.Name;
					obj.Insert(num2, playerInfo);
				}
			}
			if (LocalPlayer.LastAttackerSteamID > 0UL)
			{
				int num3 = 0;
				PlayerInfo playerInfo = default(PlayerInfo);
				playerInfo.Id = LocalPlayer.LastAttackerSteamID.ToString();
				playerInfo.Name = LocalPlayer.LastAttackerName;
				obj.Insert(num3, playerInfo);
			}
		}
	}
}
