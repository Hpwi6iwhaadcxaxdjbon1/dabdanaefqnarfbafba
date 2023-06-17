using System;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK.Net;
using GameAnalyticsSDK.State;
using GameAnalyticsSDK.Utilities;
using UnityEngine;

namespace GameAnalyticsSDK.Wrapper
{
	// Token: 0x0200090F RID: 2319
	public class GA_Wrapper
	{
		// Token: 0x04002C3F RID: 11327
		private static readonly GA_Wrapper.UnityCommandCenterListener unityCommandCenterListener = new GA_Wrapper.UnityCommandCenterListener();

		// Token: 0x0600315A RID: 12634 RVA: 0x000ED60C File Offset: 0x000EB80C
		private static void configureAvailableCustomDimensions01(string list)
		{
			IEnumerable<object> enumerable = GA_MiniJSON.Deserialize(list) as IList<object>;
			ArrayList arrayList = new ArrayList();
			foreach (object obj in enumerable)
			{
				arrayList.Add(obj);
			}
			GameAnalytics.ConfigureAvailableCustomDimensions01((string[])arrayList.ToArray(typeof(string)));
		}

		// Token: 0x0600315B RID: 12635 RVA: 0x000ED680 File Offset: 0x000EB880
		private static void configureAvailableCustomDimensions02(string list)
		{
			IEnumerable<object> enumerable = GA_MiniJSON.Deserialize(list) as IList<object>;
			ArrayList arrayList = new ArrayList();
			foreach (object obj in enumerable)
			{
				arrayList.Add(obj);
			}
			GameAnalytics.ConfigureAvailableCustomDimensions02((string[])arrayList.ToArray(typeof(string)));
		}

		// Token: 0x0600315C RID: 12636 RVA: 0x000ED6F4 File Offset: 0x000EB8F4
		private static void configureAvailableCustomDimensions03(string list)
		{
			IEnumerable<object> enumerable = GA_MiniJSON.Deserialize(list) as IList<object>;
			ArrayList arrayList = new ArrayList();
			foreach (object obj in enumerable)
			{
				arrayList.Add(obj);
			}
			GameAnalytics.ConfigureAvailableCustomDimensions03((string[])arrayList.ToArray(typeof(string)));
		}

		// Token: 0x0600315D RID: 12637 RVA: 0x000ED768 File Offset: 0x000EB968
		private static void configureAvailableResourceCurrencies(string list)
		{
			IEnumerable<object> enumerable = GA_MiniJSON.Deserialize(list) as IList<object>;
			ArrayList arrayList = new ArrayList();
			foreach (object obj in enumerable)
			{
				arrayList.Add(obj);
			}
			GameAnalytics.ConfigureAvailableResourceCurrencies((string[])arrayList.ToArray(typeof(string)));
		}

		// Token: 0x0600315E RID: 12638 RVA: 0x000ED7DC File Offset: 0x000EB9DC
		private static void configureAvailableResourceItemTypes(string list)
		{
			IEnumerable<object> enumerable = GA_MiniJSON.Deserialize(list) as IList<object>;
			ArrayList arrayList = new ArrayList();
			foreach (object obj in enumerable)
			{
				arrayList.Add(obj);
			}
			GameAnalytics.ConfigureAvailableResourceItemTypes((string[])arrayList.ToArray(typeof(string)));
		}

		// Token: 0x0600315F RID: 12639 RVA: 0x00025B57 File Offset: 0x00023D57
		private static void configureSdkGameEngineVersion(string unitySdkVersion)
		{
			GameAnalytics.ConfigureSdkGameEngineVersion(unitySdkVersion);
		}

		// Token: 0x06003160 RID: 12640 RVA: 0x00025B5F File Offset: 0x00023D5F
		private static void configureGameEngineVersion(string unityEngineVersion)
		{
			GameAnalytics.ConfigureGameEngineVersion(unityEngineVersion);
		}

		// Token: 0x06003161 RID: 12641 RVA: 0x00025B67 File Offset: 0x00023D67
		private static void configureBuild(string build)
		{
			GameAnalytics.ConfigureBuild(build);
		}

		// Token: 0x06003162 RID: 12642 RVA: 0x00025B6F File Offset: 0x00023D6F
		private static void configureUserId(string userId)
		{
			GameAnalytics.ConfigureUserId(userId);
		}

		// Token: 0x06003163 RID: 12643 RVA: 0x00025B77 File Offset: 0x00023D77
		private static void initialize(string gamekey, string gamesecret)
		{
			GameAnalytics.AddCommandCenterListener(GA_Wrapper.unityCommandCenterListener);
			GameAnalytics.Initialize(gamekey, gamesecret);
		}

		// Token: 0x06003164 RID: 12644 RVA: 0x00025B8A File Offset: 0x00023D8A
		private static void setCustomDimension01(string customDimension)
		{
			GameAnalytics.SetCustomDimension01(customDimension);
		}

		// Token: 0x06003165 RID: 12645 RVA: 0x00025B92 File Offset: 0x00023D92
		private static void setCustomDimension02(string customDimension)
		{
			GameAnalytics.SetCustomDimension02(customDimension);
		}

		// Token: 0x06003166 RID: 12646 RVA: 0x00025B9A File Offset: 0x00023D9A
		private static void setCustomDimension03(string customDimension)
		{
			GameAnalytics.SetCustomDimension03(customDimension);
		}

		// Token: 0x06003167 RID: 12647 RVA: 0x00025BA2 File Offset: 0x00023DA2
		private static void addBusinessEvent(string currency, int amount, string itemType, string itemId, string cartType, string fields)
		{
			GameAnalytics.AddBusinessEvent(currency, amount, itemType, itemId, cartType);
		}

		// Token: 0x06003168 RID: 12648 RVA: 0x00025BAF File Offset: 0x00023DAF
		private static void addResourceEvent(int flowType, string currency, float amount, string itemType, string itemId, string fields)
		{
			GameAnalytics.AddResourceEvent(flowType, currency, amount, itemType, itemId);
		}

		// Token: 0x06003169 RID: 12649 RVA: 0x00025BBC File Offset: 0x00023DBC
		private static void addProgressionEvent(int progressionStatus, string progression01, string progression02, string progression03, string fields)
		{
			GameAnalytics.AddProgressionEvent(progressionStatus, progression01, progression02, progression03);
		}

		// Token: 0x0600316A RID: 12650 RVA: 0x00025BC7 File Offset: 0x00023DC7
		private static void addProgressionEventWithScore(int progressionStatus, string progression01, string progression02, string progression03, int score, string fields)
		{
			GameAnalytics.AddProgressionEvent(progressionStatus, progression01, progression02, progression03, (double)score);
		}

		// Token: 0x0600316B RID: 12651 RVA: 0x00025BD5 File Offset: 0x00023DD5
		private static void addDesignEvent(string eventId, string fields)
		{
			GameAnalytics.AddDesignEvent(eventId, null);
		}

		// Token: 0x0600316C RID: 12652 RVA: 0x00025BDE File Offset: 0x00023DDE
		private static void addDesignEventWithValue(string eventId, float value, string fields)
		{
			GameAnalytics.AddDesignEvent(eventId, (double)value);
		}

		// Token: 0x0600316D RID: 12653 RVA: 0x00025BE8 File Offset: 0x00023DE8
		private static void addErrorEvent(int severity, string message, string fields)
		{
			GameAnalytics.AddErrorEvent(severity, message);
		}

		// Token: 0x0600316E RID: 12654 RVA: 0x00025BF1 File Offset: 0x00023DF1
		private static void setEnabledInfoLog(bool enabled)
		{
			GameAnalytics.SetEnabledInfoLog(enabled);
		}

		// Token: 0x0600316F RID: 12655 RVA: 0x00025BF9 File Offset: 0x00023DF9
		private static void setEnabledVerboseLog(bool enabled)
		{
			GameAnalytics.SetEnabledVerboseLog(enabled);
		}

		// Token: 0x06003170 RID: 12656 RVA: 0x00025C01 File Offset: 0x00023E01
		private static void setManualSessionHandling(bool enabled)
		{
			GameAnalytics.SetEnabledManualSessionHandling(enabled);
		}

		// Token: 0x06003171 RID: 12657 RVA: 0x00025C09 File Offset: 0x00023E09
		private static void gameAnalyticsStartSession()
		{
			GameAnalytics.StartSession();
		}

		// Token: 0x06003172 RID: 12658 RVA: 0x00025C10 File Offset: 0x00023E10
		private static void gameAnalyticsEndSession()
		{
			GameAnalytics.EndSession();
		}

		// Token: 0x06003173 RID: 12659 RVA: 0x00025C17 File Offset: 0x00023E17
		private static void setFacebookId(string facebookId)
		{
			GameAnalytics.SetFacebookId(facebookId);
		}

		// Token: 0x06003174 RID: 12660 RVA: 0x00025C1F File Offset: 0x00023E1F
		private static void setGender(string gender)
		{
			if (gender == "male")
			{
				GameAnalytics.SetGender(1);
				return;
			}
			if (!(gender == "female"))
			{
				return;
			}
			GameAnalytics.SetGender(2);
		}

		// Token: 0x06003175 RID: 12661 RVA: 0x00025C49 File Offset: 0x00023E49
		private static void setBirthYear(int birthYear)
		{
			GameAnalytics.SetBirthYear(birthYear);
		}

		// Token: 0x06003176 RID: 12662 RVA: 0x00025C51 File Offset: 0x00023E51
		private static string getCommandCenterValueAsString(string key, string defaultValue)
		{
			return GameAnalytics.GetCommandCenterValueAsString(key, defaultValue);
		}

		// Token: 0x06003177 RID: 12663 RVA: 0x00025C5A File Offset: 0x00023E5A
		private static bool isCommandCenterReady()
		{
			return GameAnalytics.IsCommandCenterReady();
		}

		// Token: 0x06003178 RID: 12664 RVA: 0x00025C61 File Offset: 0x00023E61
		private static string getConfigurationsContentAsString()
		{
			return GameAnalytics.GetConfigurationsAsString();
		}

		// Token: 0x06003179 RID: 12665 RVA: 0x00025C68 File Offset: 0x00023E68
		public static void SetAvailableCustomDimensions01(string list)
		{
			GA_Wrapper.configureAvailableCustomDimensions01(list);
		}

		// Token: 0x0600317A RID: 12666 RVA: 0x00025C70 File Offset: 0x00023E70
		public static void SetAvailableCustomDimensions02(string list)
		{
			GA_Wrapper.configureAvailableCustomDimensions02(list);
		}

		// Token: 0x0600317B RID: 12667 RVA: 0x00025C78 File Offset: 0x00023E78
		public static void SetAvailableCustomDimensions03(string list)
		{
			GA_Wrapper.configureAvailableCustomDimensions03(list);
		}

		// Token: 0x0600317C RID: 12668 RVA: 0x00025C80 File Offset: 0x00023E80
		public static void SetAvailableResourceCurrencies(string list)
		{
			GA_Wrapper.configureAvailableResourceCurrencies(list);
		}

		// Token: 0x0600317D RID: 12669 RVA: 0x00025C88 File Offset: 0x00023E88
		public static void SetAvailableResourceItemTypes(string list)
		{
			GA_Wrapper.configureAvailableResourceItemTypes(list);
		}

		// Token: 0x0600317E RID: 12670 RVA: 0x00025C90 File Offset: 0x00023E90
		public static void SetUnitySdkVersion(string unitySdkVersion)
		{
			GA_Wrapper.configureSdkGameEngineVersion(unitySdkVersion);
		}

		// Token: 0x0600317F RID: 12671 RVA: 0x00025C98 File Offset: 0x00023E98
		public static void SetUnityEngineVersion(string unityEngineVersion)
		{
			GA_Wrapper.configureGameEngineVersion(unityEngineVersion);
		}

		// Token: 0x06003180 RID: 12672 RVA: 0x00025CA0 File Offset: 0x00023EA0
		public static void SetBuild(string build)
		{
			GA_Wrapper.configureBuild(build);
		}

		// Token: 0x06003181 RID: 12673 RVA: 0x00025CA8 File Offset: 0x00023EA8
		public static void SetCustomUserId(string userId)
		{
			GA_Wrapper.configureUserId(userId);
		}

		// Token: 0x06003182 RID: 12674 RVA: 0x00025CB0 File Offset: 0x00023EB0
		public static void SetEnabledManualSessionHandling(bool enabled)
		{
			GA_Wrapper.setManualSessionHandling(enabled);
		}

		// Token: 0x06003183 RID: 12675 RVA: 0x00025CB8 File Offset: 0x00023EB8
		public static void StartSession()
		{
			if (GAState.IsManualSessionHandlingEnabled())
			{
				GA_Wrapper.gameAnalyticsStartSession();
				return;
			}
			Debug.Log("Manual session handling is not enabled. \nPlease check the \"Use manual session handling\" option in the \"Advanced\" section of the Settings object.");
		}

		// Token: 0x06003184 RID: 12676 RVA: 0x00025CD1 File Offset: 0x00023ED1
		public static void EndSession()
		{
			if (GAState.IsManualSessionHandlingEnabled())
			{
				GA_Wrapper.gameAnalyticsEndSession();
				return;
			}
			Debug.Log("Manual session handling is not enabled. \nPlease check the \"Use manual session handling\" option in the \"Advanced\" section of the Settings object.");
		}

		// Token: 0x06003185 RID: 12677 RVA: 0x00025CEA File Offset: 0x00023EEA
		public static void Initialize(string gamekey, string gamesecret)
		{
			GA_Wrapper.initialize(gamekey, gamesecret);
		}

		// Token: 0x06003186 RID: 12678 RVA: 0x00025CF3 File Offset: 0x00023EF3
		public static void SetCustomDimension01(string customDimension)
		{
			GA_Wrapper.setCustomDimension01(customDimension);
		}

		// Token: 0x06003187 RID: 12679 RVA: 0x00025CFB File Offset: 0x00023EFB
		public static void SetCustomDimension02(string customDimension)
		{
			GA_Wrapper.setCustomDimension02(customDimension);
		}

		// Token: 0x06003188 RID: 12680 RVA: 0x00025D03 File Offset: 0x00023F03
		public static void SetCustomDimension03(string customDimension)
		{
			GA_Wrapper.setCustomDimension03(customDimension);
		}

		// Token: 0x06003189 RID: 12681 RVA: 0x000ED850 File Offset: 0x000EBA50
		public static void AddBusinessEvent(string currency, int amount, string itemType, string itemId, string cartType, IDictionary<string, object> fields)
		{
			string fields2 = GA_Wrapper.DictionaryToJsonString(fields);
			GA_Wrapper.addBusinessEvent(currency, amount, itemType, itemId, cartType, fields2);
		}

		// Token: 0x0600318A RID: 12682 RVA: 0x000ED874 File Offset: 0x000EBA74
		public static void AddResourceEvent(GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId, IDictionary<string, object> fields)
		{
			string fields2 = GA_Wrapper.DictionaryToJsonString(fields);
			GA_Wrapper.addResourceEvent((int)flowType, currency, amount, itemType, itemId, fields2);
		}

		// Token: 0x0600318B RID: 12683 RVA: 0x000ED898 File Offset: 0x000EBA98
		public static void AddProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, IDictionary<string, object> fields)
		{
			string fields2 = GA_Wrapper.DictionaryToJsonString(fields);
			GA_Wrapper.addProgressionEvent((int)progressionStatus, progression01, progression02, progression03, fields2);
		}

		// Token: 0x0600318C RID: 12684 RVA: 0x000ED8B8 File Offset: 0x000EBAB8
		public static void AddProgressionEventWithScore(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score, IDictionary<string, object> fields)
		{
			string fields2 = GA_Wrapper.DictionaryToJsonString(fields);
			GA_Wrapper.addProgressionEventWithScore((int)progressionStatus, progression01, progression02, progression03, score, fields2);
		}

		// Token: 0x0600318D RID: 12685 RVA: 0x000ED8DC File Offset: 0x000EBADC
		public static void AddDesignEvent(string eventID, float eventValue, IDictionary<string, object> fields)
		{
			string fields2 = GA_Wrapper.DictionaryToJsonString(fields);
			GA_Wrapper.addDesignEventWithValue(eventID, eventValue, fields2);
		}

		// Token: 0x0600318E RID: 12686 RVA: 0x000ED8F8 File Offset: 0x000EBAF8
		public static void AddDesignEvent(string eventID, IDictionary<string, object> fields)
		{
			string fields2 = GA_Wrapper.DictionaryToJsonString(fields);
			GA_Wrapper.addDesignEvent(eventID, fields2);
		}

		// Token: 0x0600318F RID: 12687 RVA: 0x000ED914 File Offset: 0x000EBB14
		public static void AddErrorEvent(GAErrorSeverity severity, string message, IDictionary<string, object> fields)
		{
			string fields2 = GA_Wrapper.DictionaryToJsonString(fields);
			GA_Wrapper.addErrorEvent((int)severity, message, fields2);
		}

		// Token: 0x06003190 RID: 12688 RVA: 0x00025D0B File Offset: 0x00023F0B
		public static void SetInfoLog(bool enabled)
		{
			GA_Wrapper.setEnabledInfoLog(enabled);
		}

		// Token: 0x06003191 RID: 12689 RVA: 0x00025D13 File Offset: 0x00023F13
		public static void SetVerboseLog(bool enabled)
		{
			GA_Wrapper.setEnabledVerboseLog(enabled);
		}

		// Token: 0x06003192 RID: 12690 RVA: 0x00025D1B File Offset: 0x00023F1B
		public static void SetFacebookId(string facebookId)
		{
			GA_Wrapper.setFacebookId(facebookId);
		}

		// Token: 0x06003193 RID: 12691 RVA: 0x00025D23 File Offset: 0x00023F23
		public static void SetGender(string gender)
		{
			GA_Wrapper.setGender(gender);
		}

		// Token: 0x06003194 RID: 12692 RVA: 0x00025D2B File Offset: 0x00023F2B
		public static void SetBirthYear(int birthYear)
		{
			GA_Wrapper.setBirthYear(birthYear);
		}

		// Token: 0x06003195 RID: 12693 RVA: 0x00025D33 File Offset: 0x00023F33
		public static string GetCommandCenterValueAsString(string key, string defaultValue)
		{
			return GA_Wrapper.getCommandCenterValueAsString(key, defaultValue);
		}

		// Token: 0x06003196 RID: 12694 RVA: 0x00025D3C File Offset: 0x00023F3C
		public static bool IsCommandCenterReady()
		{
			return GA_Wrapper.isCommandCenterReady();
		}

		// Token: 0x06003197 RID: 12695 RVA: 0x00025D43 File Offset: 0x00023F43
		public static string GetConfigurationsContentAsString()
		{
			return GA_Wrapper.getConfigurationsContentAsString();
		}

		// Token: 0x06003198 RID: 12696 RVA: 0x000ED930 File Offset: 0x000EBB30
		private static string DictionaryToJsonString(IDictionary<string, object> dict)
		{
			Hashtable hashtable = new Hashtable();
			if (dict != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in dict)
				{
					hashtable.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return GA_MiniJSON.Serialize(hashtable);
		}

		// Token: 0x02000910 RID: 2320
		private class UnityCommandCenterListener : ICommandCenterListener
		{
			// Token: 0x0600319B RID: 12699 RVA: 0x00025D56 File Offset: 0x00023F56
			public void OnCommandCenterUpdated()
			{
				GameAnalytics.CommandCenterUpdated();
			}
		}
	}
}
