using System;

namespace GameAnalyticsSDK.Setup
{
	// Token: 0x0200091C RID: 2332
	public class Game
	{
		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x060031D5 RID: 12757 RVA: 0x00025F61 File Offset: 0x00024161
		// (set) Token: 0x060031D6 RID: 12758 RVA: 0x00025F69 File Offset: 0x00024169
		public string Name { get; private set; }

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x060031D7 RID: 12759 RVA: 0x00025F72 File Offset: 0x00024172
		// (set) Token: 0x060031D8 RID: 12760 RVA: 0x00025F7A File Offset: 0x0002417A
		public int ID { get; private set; }

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x060031D9 RID: 12761 RVA: 0x00025F83 File Offset: 0x00024183
		// (set) Token: 0x060031DA RID: 12762 RVA: 0x00025F8B File Offset: 0x0002418B
		public string GameKey { get; private set; }

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x060031DB RID: 12763 RVA: 0x00025F94 File Offset: 0x00024194
		// (set) Token: 0x060031DC RID: 12764 RVA: 0x00025F9C File Offset: 0x0002419C
		public string SecretKey { get; private set; }

		// Token: 0x060031DD RID: 12765 RVA: 0x00025FA5 File Offset: 0x000241A5
		public Game(string name, int id, string gameKey, string secretKey)
		{
			this.Name = name;
			this.ID = id;
			this.GameKey = gameKey;
			this.SecretKey = secretKey;
		}
	}
}
