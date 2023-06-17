using System;
using System.Collections.Generic;
using GameAnalyticsSDK.Wrapper;

namespace GameAnalyticsSDK.Events
{
	// Token: 0x02000921 RID: 2337
	public static class GA_Error
	{
		// Token: 0x06003204 RID: 12804 RVA: 0x000262F6 File Offset: 0x000244F6
		public static void NewEvent(GAErrorSeverity severity, string message, IDictionary<string, object> fields)
		{
			GA_Error.CreateNewEvent(severity, message, fields);
		}

		// Token: 0x06003205 RID: 12805 RVA: 0x00026300 File Offset: 0x00024500
		private static void CreateNewEvent(GAErrorSeverity severity, string message, IDictionary<string, object> fields)
		{
			GA_Wrapper.AddErrorEvent(severity, message, fields);
		}
	}
}
