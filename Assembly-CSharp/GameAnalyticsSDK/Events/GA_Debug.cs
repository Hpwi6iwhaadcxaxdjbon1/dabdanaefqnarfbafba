using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GameAnalyticsSDK.Events
{
	// Token: 0x0200091F RID: 2335
	public static class GA_Debug
	{
		// Token: 0x04002CBD RID: 11453
		public static int MaxErrorCount = 10;

		// Token: 0x04002CBE RID: 11454
		private static int _errorCount = 0;

		// Token: 0x04002CBF RID: 11455
		private static bool _showLogOnGUI = false;

		// Token: 0x04002CC0 RID: 11456
		public static List<string> Messages;

		// Token: 0x060031FD RID: 12797 RVA: 0x000EEDD4 File Offset: 0x000ECFD4
		public static void HandleLog(string logString, string stackTrace, LogType type)
		{
			if (logString.StartsWith("[Physics.PhysX] RigidBody::setRigidBodyFlag"))
			{
				return;
			}
			if (GA_Debug._showLogOnGUI)
			{
				if (GA_Debug.Messages == null)
				{
					GA_Debug.Messages = new List<string>();
				}
				GA_Debug.Messages.Add(logString);
			}
			if (GameAnalytics.SettingsGA.SubmitErrors && GA_Debug._errorCount < GA_Debug.MaxErrorCount && type != LogType.Log)
			{
				if (string.IsNullOrEmpty(stackTrace))
				{
					stackTrace = new StackTrace().ToString();
				}
				GA_Debug._errorCount++;
				string text = logString.Replace('"', '\'').Replace('\n', ' ').Replace('\r', ' ');
				string text2 = stackTrace.Replace('"', '\'').Replace('\n', ' ').Replace('\r', ' ');
				string text3 = text + " " + text2;
				if (text3.Length > 8192)
				{
					text3 = text3.Substring(8192);
				}
				GA_Debug.SubmitError(text3, type);
			}
		}

		// Token: 0x060031FE RID: 12798 RVA: 0x000EEEBC File Offset: 0x000ED0BC
		private static void SubmitError(string message, LogType type)
		{
			GAErrorSeverity severity = GAErrorSeverity.Info;
			switch (type)
			{
			case LogType.Error:
				severity = GAErrorSeverity.Error;
				break;
			case LogType.Assert:
				severity = GAErrorSeverity.Info;
				break;
			case LogType.Warning:
				severity = GAErrorSeverity.Warning;
				break;
			case LogType.Log:
				severity = GAErrorSeverity.Debug;
				break;
			case LogType.Exception:
				severity = GAErrorSeverity.Critical;
				break;
			}
			GA_Error.NewEvent(severity, message, null);
		}

		// Token: 0x060031FF RID: 12799 RVA: 0x000262A9 File Offset: 0x000244A9
		public static void EnabledLog()
		{
			GA_Debug._showLogOnGUI = true;
		}
	}
}
