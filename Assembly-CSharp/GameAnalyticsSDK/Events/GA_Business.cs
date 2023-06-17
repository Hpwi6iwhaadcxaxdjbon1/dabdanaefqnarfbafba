using System;
using System.Collections.Generic;
using GameAnalyticsSDK.Wrapper;

namespace GameAnalyticsSDK.Events
{
	// Token: 0x0200091E RID: 2334
	public static class GA_Business
	{
		// Token: 0x060031FC RID: 12796 RVA: 0x0002629A File Offset: 0x0002449A
		public static void NewEvent(string currency, int amount, string itemType, string itemId, string cartType, IDictionary<string, object> fields)
		{
			GA_Wrapper.AddBusinessEvent(currency, amount, itemType, itemId, cartType, fields);
		}
	}
}
