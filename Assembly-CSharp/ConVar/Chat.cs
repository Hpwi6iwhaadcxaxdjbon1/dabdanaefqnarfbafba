using System;
using Facepunch;

namespace ConVar
{
	// Token: 0x02000846 RID: 2118
	[ConsoleSystem.Factory("chat")]
	public class Chat : ConsoleSystem
	{
		// Token: 0x0400293C RID: 10556
		private const float textRange = 50f;

		// Token: 0x0400293D RID: 10557
		private const float textVolumeBoost = 0.2f;

		// Token: 0x0400293E RID: 10558
		[ServerVar]
		[ClientVar]
		public static bool enabled = true;

		// Token: 0x06002E27 RID: 11815 RVA: 0x000E72CC File Offset: 0x000E54CC
		[ClientVar(AllowRunFromServer = true)]
		public static void add(ConsoleSystem.Arg arg)
		{
			if (!Chat.enabled)
			{
				return;
			}
			ulong @uint = arg.GetUInt64(0, 0UL);
			string @string = arg.GetString(1, "");
			float @float = arg.GetFloat(2, 1f);
			if (@string == "" || @float <= 0f)
			{
				return;
			}
			UIChat.Add(@uint, @string, @float);
		}

		// Token: 0x06002E28 RID: 11816 RVA: 0x000E7324 File Offset: 0x000E5524
		[ClientVar(AllowRunFromServer = true)]
		public static void add2(ConsoleSystem.Arg arg)
		{
			if (!Chat.enabled)
			{
				return;
			}
			ulong @uint = arg.GetUInt64(0, 0UL);
			string text = arg.GetString(1, "");
			string text2 = arg.GetString(2, "");
			string @string = arg.GetString(3, "");
			float @float = arg.GetFloat(4, 1f);
			if (text == "" || @float <= 0f)
			{
				return;
			}
			if (Global.streamermode)
			{
				text2 = RandomUsernames.Get(@uint);
			}
			text = string.Format("<color={2}>{0}</color>: {1}", text2, text, @string);
			UIChat.Add(@uint, text, @float);
		}

		// Token: 0x06002E29 RID: 11817 RVA: 0x00023B31 File Offset: 0x00021D31
		[ClientVar]
		public static void open()
		{
			if (!Chat.enabled)
			{
				return;
			}
			if (!Graphics.chat)
			{
				return;
			}
			UIChat.Open();
		}

		// Token: 0x02000847 RID: 2119
		public struct ChatEntry
		{
			// Token: 0x1700038C RID: 908
			// (get) Token: 0x06002E2C RID: 11820 RVA: 0x00023B50 File Offset: 0x00021D50
			// (set) Token: 0x06002E2D RID: 11821 RVA: 0x00023B58 File Offset: 0x00021D58
			public string Message { get; set; }

			// Token: 0x1700038D RID: 909
			// (get) Token: 0x06002E2E RID: 11822 RVA: 0x00023B61 File Offset: 0x00021D61
			// (set) Token: 0x06002E2F RID: 11823 RVA: 0x00023B69 File Offset: 0x00021D69
			public ulong UserId { get; set; }

			// Token: 0x1700038E RID: 910
			// (get) Token: 0x06002E30 RID: 11824 RVA: 0x00023B72 File Offset: 0x00021D72
			// (set) Token: 0x06002E31 RID: 11825 RVA: 0x00023B7A File Offset: 0x00021D7A
			public string Username { get; set; }

			// Token: 0x1700038F RID: 911
			// (get) Token: 0x06002E32 RID: 11826 RVA: 0x00023B83 File Offset: 0x00021D83
			// (set) Token: 0x06002E33 RID: 11827 RVA: 0x00023B8B File Offset: 0x00021D8B
			public string Color { get; set; }

			// Token: 0x17000390 RID: 912
			// (get) Token: 0x06002E34 RID: 11828 RVA: 0x00023B94 File Offset: 0x00021D94
			// (set) Token: 0x06002E35 RID: 11829 RVA: 0x00023B9C File Offset: 0x00021D9C
			public int Time { get; set; }
		}
	}
}
