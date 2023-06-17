using System;
using System.Collections.Generic;
using GameAnalyticsSDK.Setup;
using UnityEngine;

namespace GameAnalyticsSDK.State
{
	// Token: 0x02000915 RID: 2325
	internal static class GAState
	{
		// Token: 0x04002C50 RID: 11344
		private static Settings _settings;

		// Token: 0x060031B6 RID: 12726 RVA: 0x000EE1E8 File Offset: 0x000EC3E8
		public static void Init()
		{
			try
			{
				GAState._settings = (Settings)Resources.Load("GameAnalytics/Settings", typeof(Settings));
			}
			catch (Exception ex)
			{
				Debug.Log("Could not get Settings during event validation \n" + ex.ToString());
			}
		}

		// Token: 0x060031B7 RID: 12727 RVA: 0x00025E31 File Offset: 0x00024031
		private static bool ListContainsString(List<string> _list, string _string)
		{
			return _list.Contains(_string);
		}

		// Token: 0x060031B8 RID: 12728 RVA: 0x00025E3F File Offset: 0x0002403F
		public static bool IsManualSessionHandlingEnabled()
		{
			return GAState._settings.UseManualSessionHandling;
		}

		// Token: 0x060031B9 RID: 12729 RVA: 0x00025E4B File Offset: 0x0002404B
		public static bool HasAvailableResourceCurrency(string _currency)
		{
			return GAState.ListContainsString(GAState._settings.ResourceCurrencies, _currency);
		}

		// Token: 0x060031BA RID: 12730 RVA: 0x00025E62 File Offset: 0x00024062
		public static bool HasAvailableResourceItemType(string _itemType)
		{
			return GAState.ListContainsString(GAState._settings.ResourceItemTypes, _itemType);
		}

		// Token: 0x060031BB RID: 12731 RVA: 0x00025E79 File Offset: 0x00024079
		public static bool HasAvailableCustomDimensions01(string _dimension01)
		{
			return GAState.ListContainsString(GAState._settings.CustomDimensions01, _dimension01);
		}

		// Token: 0x060031BC RID: 12732 RVA: 0x00025E90 File Offset: 0x00024090
		public static bool HasAvailableCustomDimensions02(string _dimension02)
		{
			return GAState.ListContainsString(GAState._settings.CustomDimensions02, _dimension02);
		}

		// Token: 0x060031BD RID: 12733 RVA: 0x00025EA7 File Offset: 0x000240A7
		public static bool HasAvailableCustomDimensions03(string _dimension03)
		{
			return GAState.ListContainsString(GAState._settings.CustomDimensions03, _dimension03);
		}
	}
}
