using System;
using System.Text.RegularExpressions;
using Facepunch;
using GameAnalyticsSDK.Events;
using GameAnalyticsSDK.Net;
using GameAnalyticsSDK.Setup;
using GameAnalyticsSDK.State;
using GameAnalyticsSDK.Wrapper;
using UnityEngine;

namespace GameAnalyticsSDK
{
	// Token: 0x0200090E RID: 2318
	[RequireComponent(typeof(GA_SpecialEvents))]
	[ExecuteInEditMode]
	public class GameAnalytics : MonoBehaviour
	{
		// Token: 0x04002C3B RID: 11323
		private static Settings _settings;

		// Token: 0x04002C3C RID: 11324
		private static GameAnalytics _instance;

		// Token: 0x04002C3D RID: 11325
		private static bool _hasInitializeBeenCalled;

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06003131 RID: 12593 RVA: 0x00025989 File Offset: 0x00023B89
		// (set) Token: 0x06003132 RID: 12594 RVA: 0x000259A2 File Offset: 0x00023BA2
		public static Settings SettingsGA
		{
			get
			{
				if (GameAnalytics._settings == null)
				{
					GameAnalytics.InitAPI();
				}
				return GameAnalytics._settings;
			}
			private set
			{
				GameAnalytics._settings = value;
			}
		}

		// Token: 0x06003133 RID: 12595 RVA: 0x000ED218 File Offset: 0x000EB418
		public void Awake()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (GameAnalytics._instance != null)
			{
				Debug.LogWarning("Destroying duplicate GameAnalytics object - only one is allowed per scene!");
				Object.Destroy(base.gameObject);
				return;
			}
			GameAnalytics._instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			Application.logMessageReceived += GA_Debug.HandleLog;
			GameAnalytics.InternalInitialize();
		}

		// Token: 0x06003134 RID: 12596 RVA: 0x000259AA File Offset: 0x00023BAA
		private void OnDestroy()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (GameAnalytics._instance == this)
			{
				GameAnalytics._instance = null;
			}
		}

		// Token: 0x06003135 RID: 12597 RVA: 0x000259C7 File Offset: 0x00023BC7
		private void OnApplicationQuit()
		{
			if (!GameAnalytics.SettingsGA.UseManualSessionHandling)
			{
				GameAnalytics.OnStop();
			}
		}

		// Token: 0x06003136 RID: 12598 RVA: 0x000ED278 File Offset: 0x000EB478
		private static void InitAPI()
		{
			try
			{
				GameAnalytics._settings = (Settings)Resources.Load("GameAnalytics/Settings", typeof(Settings));
				GAState.Init();
			}
			catch (Exception ex)
			{
				Debug.Log("Error getting Settings in InitAPI: " + ex.Message);
			}
		}

		// Token: 0x06003137 RID: 12599 RVA: 0x000ED2D4 File Offset: 0x000EB4D4
		private static void InternalInitialize()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (GameAnalytics.SettingsGA.InfoLogBuild)
			{
				GA_Setup.SetInfoLog(true);
			}
			if (GameAnalytics.SettingsGA.VerboseLogBuild)
			{
				GA_Setup.SetVerboseLog(true);
			}
			GA_Wrapper.SetUnitySdkVersion("unity " + Settings.VERSION);
			GA_Wrapper.SetUnityEngineVersion("unity " + GameAnalytics.GetUnityVersion());
			GA_Wrapper.SetBuild(BuildInfo.Current.Scm.ChangeId);
			if (GameAnalytics.SettingsGA.CustomDimensions01.Count > 0)
			{
				GA_Setup.SetAvailableCustomDimensions01(GameAnalytics.SettingsGA.CustomDimensions01);
			}
			if (GameAnalytics.SettingsGA.CustomDimensions02.Count > 0)
			{
				GA_Setup.SetAvailableCustomDimensions02(GameAnalytics.SettingsGA.CustomDimensions02);
			}
			if (GameAnalytics.SettingsGA.CustomDimensions03.Count > 0)
			{
				GA_Setup.SetAvailableCustomDimensions03(GameAnalytics.SettingsGA.CustomDimensions03);
			}
			if (GameAnalytics.SettingsGA.ResourceItemTypes.Count > 0)
			{
				GA_Setup.SetAvailableResourceItemTypes(GameAnalytics.SettingsGA.ResourceItemTypes);
			}
			if (GameAnalytics.SettingsGA.ResourceCurrencies.Count > 0)
			{
				GA_Setup.SetAvailableResourceCurrencies(GameAnalytics.SettingsGA.ResourceCurrencies);
			}
			if (GameAnalytics.SettingsGA.UseManualSessionHandling)
			{
				GameAnalytics.SetEnabledManualSessionHandling(true);
			}
		}

		// Token: 0x06003138 RID: 12600 RVA: 0x000259DA File Offset: 0x00023BDA
		public static void Initialize()
		{
			if (Application.isEditor)
			{
				GameAnalytics._hasInitializeBeenCalled = true;
				return;
			}
			GA_Wrapper.Initialize("ab258be05882d64cb4e285ac8e8110f2", "e9e34daf4b7aff6505eccfc99eef811136b4c96c");
			GameAnalytics._hasInitializeBeenCalled = true;
		}

		// Token: 0x06003139 RID: 12601 RVA: 0x000259FF File Offset: 0x00023BFF
		public static void NewBusinessEvent(string currency, int amount, string itemType, string itemId, string cartType)
		{
			if (!GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Business.NewEvent(currency, amount, itemType, itemId, cartType, null);
		}

		// Token: 0x0600313A RID: 12602 RVA: 0x00025A15 File Offset: 0x00023C15
		public static void NewDesignEvent(string eventName)
		{
			if (!GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Design.NewEvent(eventName, null);
		}

		// Token: 0x0600313B RID: 12603 RVA: 0x00025A26 File Offset: 0x00023C26
		public static void NewDesignEvent(string eventName, float eventValue)
		{
			if (!GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Design.NewEvent(eventName, eventValue, null);
		}

		// Token: 0x0600313C RID: 12604 RVA: 0x00025A38 File Offset: 0x00023C38
		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01)
		{
			if (!GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, null);
		}

		// Token: 0x0600313D RID: 12605 RVA: 0x00025A4A File Offset: 0x00023C4A
		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02)
		{
			if (!GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, progression02, null);
		}

		// Token: 0x0600313E RID: 12606 RVA: 0x00025A5D File Offset: 0x00023C5D
		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03)
		{
			if (!GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, progression02, progression03, null);
		}

		// Token: 0x0600313F RID: 12607 RVA: 0x00025A71 File Offset: 0x00023C71
		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01, int score)
		{
			if (!GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, score, null);
		}

		// Token: 0x06003140 RID: 12608 RVA: 0x00025A84 File Offset: 0x00023C84
		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, int score)
		{
			if (!GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, progression02, score, null);
		}

		// Token: 0x06003141 RID: 12609 RVA: 0x00025A98 File Offset: 0x00023C98
		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score)
		{
			if (!GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, progression02, progression03, score, null);
		}

		// Token: 0x06003142 RID: 12610 RVA: 0x00025AAE File Offset: 0x00023CAE
		public static void NewResourceEvent(GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId)
		{
			if (!GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Resource.NewEvent(flowType, currency, amount, itemType, itemId, null);
		}

		// Token: 0x06003143 RID: 12611 RVA: 0x00025AC4 File Offset: 0x00023CC4
		public static void NewErrorEvent(GAErrorSeverity severity, string message)
		{
			if (!GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Error.NewEvent(severity, message, null);
		}

		// Token: 0x06003144 RID: 12612 RVA: 0x00025AD6 File Offset: 0x00023CD6
		public static void SetFacebookId(string facebookId)
		{
			GA_Setup.SetFacebookId(facebookId);
		}

		// Token: 0x06003145 RID: 12613 RVA: 0x00025ADE File Offset: 0x00023CDE
		public static void SetGender(GAGender gender)
		{
			GA_Setup.SetGender(gender);
		}

		// Token: 0x06003146 RID: 12614 RVA: 0x00025AE6 File Offset: 0x00023CE6
		public static void SetBirthYear(int birthYear)
		{
			GA_Setup.SetBirthYear(birthYear);
		}

		// Token: 0x06003147 RID: 12615 RVA: 0x00025AEE File Offset: 0x00023CEE
		public static void SetCustomId(string userId)
		{
			GA_Wrapper.SetCustomUserId(userId);
		}

		// Token: 0x06003148 RID: 12616 RVA: 0x00025AF6 File Offset: 0x00023CF6
		public static void SetEnabledManualSessionHandling(bool enabled)
		{
			GA_Wrapper.SetEnabledManualSessionHandling(enabled);
		}

		// Token: 0x06003149 RID: 12617 RVA: 0x00025AFE File Offset: 0x00023CFE
		public static void StartSession()
		{
			GA_Wrapper.StartSession();
		}

		// Token: 0x0600314A RID: 12618 RVA: 0x00025B05 File Offset: 0x00023D05
		public static void EndSession()
		{
			GA_Wrapper.EndSession();
		}

		// Token: 0x0600314B RID: 12619 RVA: 0x00025B0C File Offset: 0x00023D0C
		public static void SetCustomDimension01(string customDimension)
		{
			GA_Setup.SetCustomDimension01(customDimension);
		}

		// Token: 0x0600314C RID: 12620 RVA: 0x00025B14 File Offset: 0x00023D14
		public static void SetCustomDimension02(string customDimension)
		{
			GA_Setup.SetCustomDimension02(customDimension);
		}

		// Token: 0x0600314D RID: 12621 RVA: 0x00025B1C File Offset: 0x00023D1C
		public static void SetCustomDimension03(string customDimension)
		{
			GA_Setup.SetCustomDimension03(customDimension);
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600314E RID: 12622 RVA: 0x000ED400 File Offset: 0x000EB600
		// (remove) Token: 0x0600314F RID: 12623 RVA: 0x000ED434 File Offset: 0x000EB634
		public static event Action OnCommandCenterUpdatedEvent;

		// Token: 0x06003150 RID: 12624 RVA: 0x00025B24 File Offset: 0x00023D24
		public void OnCommandCenterUpdated()
		{
			if (GameAnalytics.OnCommandCenterUpdatedEvent != null)
			{
				GameAnalytics.OnCommandCenterUpdatedEvent.Invoke();
			}
		}

		// Token: 0x06003151 RID: 12625 RVA: 0x00025B24 File Offset: 0x00023D24
		public static void CommandCenterUpdated()
		{
			if (GameAnalytics.OnCommandCenterUpdatedEvent != null)
			{
				GameAnalytics.OnCommandCenterUpdatedEvent.Invoke();
			}
		}

		// Token: 0x06003152 RID: 12626 RVA: 0x00025B37 File Offset: 0x00023D37
		public static string GetCommandCenterValueAsString(string key)
		{
			return GameAnalytics.GetCommandCenterValueAsString(key, null);
		}

		// Token: 0x06003153 RID: 12627 RVA: 0x00025B40 File Offset: 0x00023D40
		public static string GetCommandCenterValueAsString(string key, string defaultValue)
		{
			return GA_Wrapper.GetCommandCenterValueAsString(key, defaultValue);
		}

		// Token: 0x06003154 RID: 12628 RVA: 0x00025B49 File Offset: 0x00023D49
		public static bool IsCommandCenterReady()
		{
			return GA_Wrapper.IsCommandCenterReady();
		}

		// Token: 0x06003155 RID: 12629 RVA: 0x00025B50 File Offset: 0x00023D50
		public static string GetConfigurationsContentAsString()
		{
			return GA_Wrapper.GetConfigurationsContentAsString();
		}

		// Token: 0x06003156 RID: 12630 RVA: 0x000ED468 File Offset: 0x000EB668
		private static string GetUnityVersion()
		{
			string text = "";
			string[] array = Application.unityVersion.Split(new char[]
			{
				'.'
			});
			for (int i = 0; i < array.Length; i++)
			{
				int num;
				if (int.TryParse(array[i], ref num))
				{
					if (i == 0)
					{
						text = array[i];
					}
					else
					{
						text = text + "." + array[i];
					}
				}
				else
				{
					string[] array2 = Regex.Split(array[i], "[^\\d]+");
					if (array2.Length != 0 && int.TryParse(array2[0], ref num))
					{
						text = text + "." + array2[0];
					}
				}
			}
			return text;
		}

		// Token: 0x06003157 RID: 12631 RVA: 0x000ED4F8 File Offset: 0x000EB6F8
		private static int GetPlatformIndex()
		{
			RuntimePlatform platform = Application.platform;
			int result;
			if (platform == RuntimePlatform.IPhonePlayer)
			{
				if (!GameAnalytics.SettingsGA.Platforms.Contains(platform))
				{
					result = GameAnalytics.SettingsGA.Platforms.IndexOf(RuntimePlatform.tvOS);
				}
				else
				{
					result = GameAnalytics.SettingsGA.Platforms.IndexOf(platform);
				}
			}
			else if (platform == RuntimePlatform.tvOS)
			{
				if (!GameAnalytics.SettingsGA.Platforms.Contains(platform))
				{
					result = GameAnalytics.SettingsGA.Platforms.IndexOf(RuntimePlatform.IPhonePlayer);
				}
				else
				{
					result = GameAnalytics.SettingsGA.Platforms.IndexOf(platform);
				}
			}
			else if (platform == RuntimePlatform.MetroPlayerARM || platform == RuntimePlatform.MetroPlayerX64 || platform == RuntimePlatform.MetroPlayerX86 || platform == RuntimePlatform.MetroPlayerARM || platform == RuntimePlatform.MetroPlayerX64 || platform == RuntimePlatform.MetroPlayerX86)
			{
				result = GameAnalytics.SettingsGA.Platforms.IndexOf(RuntimePlatform.MetroPlayerARM);
			}
			else
			{
				result = GameAnalytics.SettingsGA.Platforms.IndexOf(platform);
			}
			return result;
		}

		// Token: 0x06003158 RID: 12632 RVA: 0x000ED5D4 File Offset: 0x000EB7D4
		public static void SetBuildAllPlatforms(string build)
		{
			for (int i = 0; i < GameAnalytics.SettingsGA.Build.Count; i++)
			{
				GameAnalytics.SettingsGA.Build[i] = build;
			}
		}
	}
}
