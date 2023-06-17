using System;
using System.Collections.Generic;
using GameAnalyticsSDK.Wrapper;

namespace GameAnalyticsSDK.Events
{
	// Token: 0x02000923 RID: 2339
	public static class GA_Resource
	{
		// Token: 0x0600320D RID: 12813 RVA: 0x0002636C File Offset: 0x0002456C
		public static void NewEvent(GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId, IDictionary<string, object> fields)
		{
			GA_Wrapper.AddResourceEvent(flowType, currency, amount, itemType, itemId, fields);
		}
	}
}
