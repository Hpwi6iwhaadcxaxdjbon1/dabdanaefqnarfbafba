using System;

namespace ConVar
{
	// Token: 0x02000840 RID: 2112
	[ConsoleSystem.Factory("global")]
	public class Admin : ConsoleSystem
	{
		// Token: 0x06002DFE RID: 11774 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar(Help = "Print out currently connected clients")]
		public static void status(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002DFF RID: 11775 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar(Help = "Print out stats of currently connected clients")]
		public static void stats(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E00 RID: 11776 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void kick(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E01 RID: 11777 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void kickall(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E02 RID: 11778 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void ban(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E03 RID: 11779 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void moderatorid(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E04 RID: 11780 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void ownerid(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E05 RID: 11781 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void removemoderator(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E06 RID: 11782 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void removeowner(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E07 RID: 11783 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void banid(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E08 RID: 11784 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void unban(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E09 RID: 11785 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void skipqueue(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E0A RID: 11786 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar(Help = "Print out currently connected clients etc")]
		public static void players(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E0B RID: 11787 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar(Help = "Sends a message in chat")]
		public static void say(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E0C RID: 11788 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar(Help = "Show user info for players on server.")]
		public static void users(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E0D RID: 11789 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar(Help = "List of banned users (sourceds compat)")]
		public static void banlist(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E0E RID: 11790 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar(Help = "List of banned users - shows reasons and usernames")]
		public static void banlistex(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E0F RID: 11791 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar(Help = "List of banned users, by ID (sourceds compat)")]
		public static void listid(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E10 RID: 11792 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void mutevoice(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E11 RID: 11793 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void unmutevoice(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E12 RID: 11794 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void mutechat(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E13 RID: 11795 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void unmutechat(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E14 RID: 11796 RVA: 0x00002ECE File Offset: 0x000010CE
		[ServerVar]
		public static void clientperf(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06002E15 RID: 11797 RVA: 0x000E6F90 File Offset: 0x000E5190
		[ClientVar(Help = "Sends a command to the server followed by the ID of the entity we're looking at")]
		public static string ent(string command = "")
		{
			HitTest hitTest = MainCamera.Trace(50f, LocalPlayer.Entity, 0f);
			if (hitTest == null || hitTest.HitEntity == null)
			{
				return "No entity";
			}
			return ConsoleSystem.Run(ConsoleSystem.Option.Client.Quiet(), "entid", new object[]
			{
				command,
				hitTest.HitEntity.net.ID
			});
		}
	}
}
