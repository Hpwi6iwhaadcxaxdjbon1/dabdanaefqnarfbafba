using System;

namespace ConVar
{
	// Token: 0x0200087E RID: 2174
	[ConsoleSystem.Factory("server")]
	public class Server : ConsoleSystem
	{
		// Token: 0x040029D1 RID: 10705
		[ServerVar]
		public static string ip = "";

		// Token: 0x040029D2 RID: 10706
		[ServerVar]
		public static int port = 28015;

		// Token: 0x040029D3 RID: 10707
		[ServerVar]
		public static int queryport = 0;

		// Token: 0x040029D4 RID: 10708
		[ServerVar]
		public static int maxplayers = 500;

		// Token: 0x040029D5 RID: 10709
		[ServerVar]
		public static string hostname = "My Untitled Rust Server";

		// Token: 0x040029D6 RID: 10710
		[ServerVar]
		public static string identity = "my_server_identity";

		// Token: 0x040029D7 RID: 10711
		[ServerVar]
		public static string level = "Procedural Map";

		// Token: 0x040029D8 RID: 10712
		[ServerVar]
		public static string levelurl = "";

		// Token: 0x040029D9 RID: 10713
		[ServerVar]
		public static int seed = 0;

		// Token: 0x040029DA RID: 10714
		[ServerVar]
		public static int salt = 0;

		// Token: 0x040029DB RID: 10715
		[ServerVar]
		public static int worldsize = 0;

		// Token: 0x040029DC RID: 10716
		[ServerVar]
		public static int saveinterval = 600;

		// Token: 0x040029DD RID: 10717
		[ServerVar]
		public static bool secure = true;

		// Token: 0x040029DE RID: 10718
		[ServerVar]
		public static int encryption = 2;

		// Token: 0x040029DF RID: 10719
		[ServerVar]
		public static int tickrate = 10;

		// Token: 0x040029E0 RID: 10720
		[ServerVar]
		public static int entityrate = 16;

		// Token: 0x040029E1 RID: 10721
		[ServerVar]
		public static float schematime = 1800f;

		// Token: 0x040029E2 RID: 10722
		[ServerVar]
		public static float cycletime = 500f;

		// Token: 0x040029E3 RID: 10723
		[ServerVar]
		public static bool official = false;

		// Token: 0x040029E4 RID: 10724
		[ServerVar]
		public static bool stats = false;

		// Token: 0x040029E5 RID: 10725
		[ServerVar]
		public static bool globalchat = true;

		// Token: 0x040029E6 RID: 10726
		[ServerVar]
		public static bool stability = true;

		// Token: 0x040029E7 RID: 10727
		[ServerVar]
		public static bool radiation = true;

		// Token: 0x040029E8 RID: 10728
		[ServerVar]
		public static float itemdespawn = 300f;

		// Token: 0x040029E9 RID: 10729
		[ServerVar]
		public static float corpsedespawn = 300f;

		// Token: 0x040029EA RID: 10730
		[ServerVar]
		public static bool pve = false;

		// Token: 0x040029EB RID: 10731
		[ServerVar]
		public static string description = "No server description has been provided.";

		// Token: 0x040029EC RID: 10732
		[ServerVar]
		public static string headerimage = "";

		// Token: 0x040029ED RID: 10733
		[ServerVar]
		public static string url = "";

		// Token: 0x040029EE RID: 10734
		[ServerVar]
		public static string branch = "";

		// Token: 0x040029EF RID: 10735
		[ServerVar]
		public static int queriesPerSecond = 2000;

		// Token: 0x040029F0 RID: 10736
		[ServerVar]
		public static int ipQueriesPerMin = 30;

		// Token: 0x040029F1 RID: 10737
		[ServerVar(Saved = true)]
		public static float meleedamage = 1f;

		// Token: 0x040029F2 RID: 10738
		[ServerVar(Saved = true)]
		public static float arrowdamage = 1f;

		// Token: 0x040029F3 RID: 10739
		[ServerVar(Saved = true)]
		public static float bulletdamage = 1f;

		// Token: 0x040029F4 RID: 10740
		[ServerVar(Saved = true)]
		public static float bleedingdamage = 1f;

		// Token: 0x040029F5 RID: 10741
		[ServerVar(Saved = true)]
		public static float meleearmor = 1f;

		// Token: 0x040029F6 RID: 10742
		[ServerVar(Saved = true)]
		public static float arrowarmor = 1f;

		// Token: 0x040029F7 RID: 10743
		[ServerVar(Saved = true)]
		public static float bulletarmor = 1f;

		// Token: 0x040029F8 RID: 10744
		[ServerVar(Saved = true)]
		public static float bleedingarmor = 1f;

		// Token: 0x040029F9 RID: 10745
		[ServerVar]
		public static int updatebatch = 512;

		// Token: 0x040029FA RID: 10746
		[ServerVar]
		public static int updatebatchspawn = 1024;

		// Token: 0x040029FB RID: 10747
		[ServerVar]
		public static int entitybatchsize = 100;

		// Token: 0x040029FC RID: 10748
		[ServerVar]
		public static float entitybatchtime = 1f;

		// Token: 0x040029FD RID: 10749
		[ServerVar]
		public static float planttick = 60f;

		// Token: 0x040029FE RID: 10750
		[ServerVar]
		public static float planttickscale = 1f;

		// Token: 0x040029FF RID: 10751
		[ServerVar]
		public static float metabolismtick = 1f;

		// Token: 0x04002A00 RID: 10752
		[ServerVar(Saved = true)]
		public static bool woundingenabled = true;

		// Token: 0x04002A01 RID: 10753
		[ServerVar]
		public static bool plantlightdetection = true;

		// Token: 0x04002A02 RID: 10754
		[ServerVar]
		public static float respawnresetrange = 50f;

		// Token: 0x04002A03 RID: 10755
		[ServerVar]
		public static int maxunack = 4;

		// Token: 0x04002A04 RID: 10756
		[ServerVar]
		public static bool netcache = true;

		// Token: 0x04002A05 RID: 10757
		[ServerVar]
		public static bool corpses = true;

		// Token: 0x04002A06 RID: 10758
		[ServerVar]
		public static bool events = true;

		// Token: 0x04002A07 RID: 10759
		[ServerVar]
		public static bool dropitems = true;

		// Token: 0x04002A08 RID: 10760
		[ServerVar]
		public static int netcachesize = 0;

		// Token: 0x04002A09 RID: 10761
		[ServerVar]
		public static int savecachesize = 0;

		// Token: 0x04002A0A RID: 10762
		[ServerVar]
		public static int combatlogsize = 30;

		// Token: 0x04002A0B RID: 10763
		[ServerVar]
		public static int combatlogdelay = 10;

		// Token: 0x04002A0C RID: 10764
		[ServerVar]
		public static int authtimeout = 60;

		// Token: 0x04002A0D RID: 10765
		[ServerVar]
		public static int playertimeout = 60;

		// Token: 0x04002A0E RID: 10766
		[ServerVar]
		public static int idlekick = 30;

		// Token: 0x04002A0F RID: 10767
		[ServerVar]
		public static int idlekickmode = 1;

		// Token: 0x04002A10 RID: 10768
		[ServerVar]
		public static int idlekickadmins = 0;

		// Token: 0x06002F4C RID: 12108 RVA: 0x0002459F File Offset: 0x0002279F
		public static float TickDelta()
		{
			return 1f / (float)Server.tickrate;
		}

		// Token: 0x06002F4D RID: 12109 RVA: 0x000245AD File Offset: 0x000227AD
		public static float TickTime(uint tick)
		{
			return (float)((double)Server.TickDelta() * tick);
		}
	}
}
