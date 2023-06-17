using System;
using System.Collections.Generic;
using GameAnalyticsSDK.Wrapper;

namespace GameAnalyticsSDK.Events
{
	// Token: 0x02000920 RID: 2336
	public static class GA_Design
	{
		// Token: 0x06003201 RID: 12801 RVA: 0x000262C6 File Offset: 0x000244C6
		public static void NewEvent(string eventName, float eventValue, IDictionary<string, object> fields)
		{
			GA_Design.CreateNewEvent(eventName, new float?(eventValue), fields);
		}

		// Token: 0x06003202 RID: 12802 RVA: 0x000EEF04 File Offset: 0x000ED104
		public static void NewEvent(string eventName, IDictionary<string, object> fields)
		{
			GA_Design.CreateNewEvent(eventName, default(float?), fields);
		}

		// Token: 0x06003203 RID: 12803 RVA: 0x000262D5 File Offset: 0x000244D5
		private static void CreateNewEvent(string eventName, float? eventValue, IDictionary<string, object> fields)
		{
			if (eventValue != null)
			{
				GA_Wrapper.AddDesignEvent(eventName, eventValue.Value, fields);
				return;
			}
			GA_Wrapper.AddDesignEvent(eventName, fields);
		}
	}
}
