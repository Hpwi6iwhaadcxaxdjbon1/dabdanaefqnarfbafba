using System;

// Token: 0x020005FC RID: 1532
public static class SteamUtil
{
	// Token: 0x04001EC6 RID: 7878
	private static string _betaNameFull = "unset";

	// Token: 0x04001EC7 RID: 7879
	private static string _betaNameType = "unset";

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x0600226A RID: 8810 RVA: 0x0001B57C File Offset: 0x0001977C
	public static string betaName
	{
		get
		{
			return SteamUtil.GetBetaName(true);
		}
	}

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x0600226B RID: 8811 RVA: 0x0001B584 File Offset: 0x00019784
	public static string betaBranch
	{
		get
		{
			return SteamUtil.GetBetaName(false);
		}
	}

	// Token: 0x17000243 RID: 579
	// (get) Token: 0x0600226C RID: 8812 RVA: 0x0001B58C File Offset: 0x0001978C
	public static bool isDevBranch
	{
		get
		{
			return SteamUtil.betaBranch == "staging" || SteamUtil.betaBranch == "prerelease";
		}
	}

	// Token: 0x0600226D RID: 8813 RVA: 0x000B8728 File Offset: 0x000B6928
	private static string GetBetaName(bool full)
	{
		if (Client.Steam == null)
		{
			return string.Empty;
		}
		if (SteamUtil._betaNameFull == "unset" || SteamUtil._betaNameType == "unset")
		{
			SteamUtil._betaNameFull = Client.Steam.BetaName;
			if (SteamUtil._betaNameFull == null || SteamUtil._betaNameFull == "public" || SteamUtil._betaNameFull == "debug")
			{
				SteamUtil._betaNameFull = string.Empty;
				SteamUtil._betaNameType = string.Empty;
			}
			else
			{
				SteamUtil._betaNameFull = SteamUtil._betaNameFull.ToLower();
				SteamUtil._betaNameType = SteamUtil._betaNameFull.Replace("-debug", "");
			}
		}
		if (!full)
		{
			return SteamUtil._betaNameType;
		}
		return SteamUtil._betaNameFull;
	}
}
