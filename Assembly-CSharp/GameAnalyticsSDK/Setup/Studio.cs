using System;
using System.Collections.Generic;

namespace GameAnalyticsSDK.Setup
{
	// Token: 0x0200091B RID: 2331
	public class Studio
	{
		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x060031CC RID: 12748 RVA: 0x00025F11 File Offset: 0x00024111
		// (set) Token: 0x060031CD RID: 12749 RVA: 0x00025F19 File Offset: 0x00024119
		public string Name { get; private set; }

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x060031CE RID: 12750 RVA: 0x00025F22 File Offset: 0x00024122
		// (set) Token: 0x060031CF RID: 12751 RVA: 0x00025F2A File Offset: 0x0002412A
		public string ID { get; private set; }

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x060031D0 RID: 12752 RVA: 0x00025F33 File Offset: 0x00024133
		// (set) Token: 0x060031D1 RID: 12753 RVA: 0x00025F3B File Offset: 0x0002413B
		public List<Game> Games { get; private set; }

		// Token: 0x060031D2 RID: 12754 RVA: 0x00025F44 File Offset: 0x00024144
		public Studio(string name, string id, List<Game> games)
		{
			this.Name = name;
			this.ID = id;
			this.Games = games;
		}

		// Token: 0x060031D3 RID: 12755 RVA: 0x000EE778 File Offset: 0x000EC978
		public static string[] GetStudioNames(List<Studio> studios, bool addFirstEmpty = true)
		{
			if (studios == null)
			{
				return new string[]
				{
					"-"
				};
			}
			if (addFirstEmpty)
			{
				string[] array = new string[studios.Count + 1];
				array[0] = "-";
				string text = "";
				for (int i = 0; i < studios.Count; i++)
				{
					array[i + 1] = studios[i].Name + text;
					text += " ";
				}
				return array;
			}
			string[] array2 = new string[studios.Count];
			string text2 = "";
			for (int j = 0; j < studios.Count; j++)
			{
				array2[j] = studios[j].Name + text2;
				text2 += " ";
			}
			return array2;
		}

		// Token: 0x060031D4 RID: 12756 RVA: 0x000EE83C File Offset: 0x000ECA3C
		public static string[] GetGameNames(int index, List<Studio> studios)
		{
			if (studios == null || studios[index].Games == null)
			{
				return new string[]
				{
					"-"
				};
			}
			string[] array = new string[studios[index].Games.Count + 1];
			array[0] = "-";
			string text = "";
			for (int i = 0; i < studios[index].Games.Count; i++)
			{
				array[i + 1] = studios[index].Games[i].Name + text;
				text += " ";
			}
			return array;
		}
	}
}
