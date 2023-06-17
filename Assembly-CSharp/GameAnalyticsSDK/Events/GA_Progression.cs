using System;
using System.Collections.Generic;
using GameAnalyticsSDK.Wrapper;

namespace GameAnalyticsSDK.Events
{
	// Token: 0x02000922 RID: 2338
	public static class GA_Progression
	{
		// Token: 0x06003206 RID: 12806 RVA: 0x000EEF24 File Offset: 0x000ED124
		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, null, null, default(int?), fields);
		}

		// Token: 0x06003207 RID: 12807 RVA: 0x000EEF44 File Offset: 0x000ED144
		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, progression02, null, default(int?), fields);
		}

		// Token: 0x06003208 RID: 12808 RVA: 0x000EEF64 File Offset: 0x000ED164
		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, progression02, progression03, default(int?), fields);
		}

		// Token: 0x06003209 RID: 12809 RVA: 0x0002630A File Offset: 0x0002450A
		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, int score, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, null, null, new int?(score), fields);
		}

		// Token: 0x0600320A RID: 12810 RVA: 0x0002631C File Offset: 0x0002451C
		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, int score, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, progression02, null, new int?(score), fields);
		}

		// Token: 0x0600320B RID: 12811 RVA: 0x0002632F File Offset: 0x0002452F
		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, progression02, progression03, new int?(score), fields);
		}

		// Token: 0x0600320C RID: 12812 RVA: 0x00026343 File Offset: 0x00024543
		private static void CreateEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int? score, IDictionary<string, object> fields)
		{
			if (score != null)
			{
				GA_Wrapper.AddProgressionEventWithScore(progressionStatus, progression01, progression02, progression03, score.Value, fields);
				return;
			}
			GA_Wrapper.AddProgressionEvent(progressionStatus, progression01, progression02, progression03, fields);
		}
	}
}
