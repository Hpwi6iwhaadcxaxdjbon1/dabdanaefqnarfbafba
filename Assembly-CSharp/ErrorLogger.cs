using System;
using Facepunch;
using Network;
using UnityEngine;

// Token: 0x0200024A RID: 586
public static class ErrorLogger
{
	// Token: 0x04000E48 RID: 3656
	private static bool installed;

	// Token: 0x0600118D RID: 4493 RVA: 0x0000F62A File Offset: 0x0000D82A
	public static void Install()
	{
		if (ErrorLogger.installed)
		{
			return;
		}
		ErrorLogger.installed = true;
		Output.OnMessage += new Action<string, string, LogType>(ErrorLogger.CaptureLog);
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x00074B44 File Offset: 0x00072D44
	internal static void CaptureLog(string error, string stacktrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert)
		{
			return;
		}
		if (type == LogType.Exception)
		{
			bool flag = true;
			if (LocalPlayer.isAdmin || LocalPlayer.isDeveloper)
			{
				flag = false;
			}
			if (flag && Net.cl != null && Net.cl.IsConnected())
			{
				Net.cl.Disconnect(string.Concat(new string[]
				{
					error,
					"\n",
					stacktrace,
					"\nVersion: ",
					BuildInfo.Current.Scm.ChangeId
				}), true);
			}
		}
	}
}
