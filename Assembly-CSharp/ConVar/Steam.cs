using System;
using System.Linq;
using Facepunch.Steamworks;

namespace ConVar
{
	// Token: 0x02000882 RID: 2178
	public class Steam
	{
		// Token: 0x04002A26 RID: 10790
		private static string[] Stats = new string[]
		{
			"harvest.wood",
			"harvested_wood"
		};

		// Token: 0x06002F5B RID: 12123 RVA: 0x0002460F File Offset: 0x0002280F
		[ClientVar]
		public static object stats()
		{
			return Enumerable.ToArray(Enumerable.Select(Steam.Stats, (string x) => new
			{
				Name = x,
				Value = Client.Instance.Stats.GetInt(x),
				Global = Client.Instance.Stats.GetGlobalInt(x)
			}));
		}

		// Token: 0x06002F5C RID: 12124 RVA: 0x0002463F File Offset: 0x0002283F
		[ClientVar]
		public static object achievements()
		{
			return Enumerable.ToArray(Enumerable.Select(Client.Instance.Achievements.All, (Achievement x) => new
			{
				x.Id,
				x.Name,
				x.State
			}));
		}

		// Token: 0x06002F5D RID: 12125 RVA: 0x000E9CF0 File Offset: 0x000E7EF0
		[ClientVar]
		public static void ResetStats()
		{
			Client.Instance.Stats.ResetAll(false);
			Client.Instance.Stats.StoreStats();
			Client.Instance.Stats.UpdateStats();
			Client.Instance.Stats.ResetAll(true);
			Client.Instance.Stats.StoreStats();
			Client.Instance.Stats.UpdateStats();
		}
	}
}
