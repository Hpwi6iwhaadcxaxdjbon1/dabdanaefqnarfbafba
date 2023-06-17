using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameAnalyticsSDK.Setup
{
	// Token: 0x02000916 RID: 2326
	public class Settings : ScriptableObject
	{
		// Token: 0x04002C51 RID: 11345
		[HideInInspector]
		public static string VERSION = "5.0.5";

		// Token: 0x04002C52 RID: 11346
		[HideInInspector]
		public static bool CheckingForUpdates = false;

		// Token: 0x04002C53 RID: 11347
		public int TotalMessagesSubmitted;

		// Token: 0x04002C54 RID: 11348
		public int TotalMessagesFailed;

		// Token: 0x04002C55 RID: 11349
		public int DesignMessagesSubmitted;

		// Token: 0x04002C56 RID: 11350
		public int DesignMessagesFailed;

		// Token: 0x04002C57 RID: 11351
		public int QualityMessagesSubmitted;

		// Token: 0x04002C58 RID: 11352
		public int QualityMessagesFailed;

		// Token: 0x04002C59 RID: 11353
		public int ErrorMessagesSubmitted;

		// Token: 0x04002C5A RID: 11354
		public int ErrorMessagesFailed;

		// Token: 0x04002C5B RID: 11355
		public int BusinessMessagesSubmitted;

		// Token: 0x04002C5C RID: 11356
		public int BusinessMessagesFailed;

		// Token: 0x04002C5D RID: 11357
		public int UserMessagesSubmitted;

		// Token: 0x04002C5E RID: 11358
		public int UserMessagesFailed;

		// Token: 0x04002C5F RID: 11359
		public string CustomArea = string.Empty;

		// Token: 0x04002C60 RID: 11360
		[SerializeField]
		private List<string> gameKey = new List<string>();

		// Token: 0x04002C61 RID: 11361
		[SerializeField]
		private List<string> secretKey = new List<string>();

		// Token: 0x04002C62 RID: 11362
		[SerializeField]
		public List<string> Build = new List<string>();

		// Token: 0x04002C63 RID: 11363
		[SerializeField]
		public List<string> SelectedPlatformStudio = new List<string>();

		// Token: 0x04002C64 RID: 11364
		[SerializeField]
		public List<string> SelectedPlatformGame = new List<string>();

		// Token: 0x04002C65 RID: 11365
		[SerializeField]
		public List<int> SelectedPlatformGameID = new List<int>();

		// Token: 0x04002C66 RID: 11366
		[SerializeField]
		public List<int> SelectedStudio = new List<int>();

		// Token: 0x04002C67 RID: 11367
		[SerializeField]
		public List<int> SelectedGame = new List<int>();

		// Token: 0x04002C68 RID: 11368
		public string NewVersion = "";

		// Token: 0x04002C69 RID: 11369
		public string Changes = "";

		// Token: 0x04002C6A RID: 11370
		public bool SignUpOpen = true;

		// Token: 0x04002C6B RID: 11371
		public string StudioName = "";

		// Token: 0x04002C6C RID: 11372
		public string GameName = "";

		// Token: 0x04002C6D RID: 11373
		public string EmailGA = "";

		// Token: 0x04002C6E RID: 11374
		[NonSerialized]
		public string PasswordGA = "";

		// Token: 0x04002C6F RID: 11375
		[NonSerialized]
		public string TokenGA = "";

		// Token: 0x04002C70 RID: 11376
		[NonSerialized]
		public string ExpireTime = "";

		// Token: 0x04002C71 RID: 11377
		[NonSerialized]
		public string LoginStatus = "Not logged in.";

		// Token: 0x04002C72 RID: 11378
		[NonSerialized]
		public bool JustSignedUp;

		// Token: 0x04002C73 RID: 11379
		[NonSerialized]
		public bool HideSignupWarning;

		// Token: 0x04002C74 RID: 11380
		public bool IntroScreen = true;

		// Token: 0x04002C75 RID: 11381
		[NonSerialized]
		public List<Studio> Studios;

		// Token: 0x04002C76 RID: 11382
		public bool InfoLogEditor = true;

		// Token: 0x04002C77 RID: 11383
		public bool InfoLogBuild = true;

		// Token: 0x04002C78 RID: 11384
		public bool VerboseLogBuild;

		// Token: 0x04002C79 RID: 11385
		public bool UseManualSessionHandling;

		// Token: 0x04002C7A RID: 11386
		public bool SendExampleGameDataToMyGame;

		// Token: 0x04002C7B RID: 11387
		public bool InternetConnectivity;

		// Token: 0x04002C7C RID: 11388
		public List<string> CustomDimensions01 = new List<string>();

		// Token: 0x04002C7D RID: 11389
		public List<string> CustomDimensions02 = new List<string>();

		// Token: 0x04002C7E RID: 11390
		public List<string> CustomDimensions03 = new List<string>();

		// Token: 0x04002C7F RID: 11391
		public List<string> ResourceItemTypes = new List<string>();

		// Token: 0x04002C80 RID: 11392
		public List<string> ResourceCurrencies = new List<string>();

		// Token: 0x04002C81 RID: 11393
		public RuntimePlatform LastCreatedGamePlatform;

		// Token: 0x04002C82 RID: 11394
		public List<RuntimePlatform> Platforms = new List<RuntimePlatform>();

		// Token: 0x04002C83 RID: 11395
		public Settings.InspectorStates CurrentInspectorState;

		// Token: 0x04002C84 RID: 11396
		public List<Settings.HelpTypes> ClosedHints = new List<Settings.HelpTypes>();

		// Token: 0x04002C85 RID: 11397
		public bool DisplayHints;

		// Token: 0x04002C86 RID: 11398
		public Vector2 DisplayHintsScrollState;

		// Token: 0x04002C87 RID: 11399
		public Texture2D Logo;

		// Token: 0x04002C88 RID: 11400
		public Texture2D UpdateIcon;

		// Token: 0x04002C89 RID: 11401
		public Texture2D InfoIcon;

		// Token: 0x04002C8A RID: 11402
		public Texture2D DeleteIcon;

		// Token: 0x04002C8B RID: 11403
		public Texture2D GameIcon;

		// Token: 0x04002C8C RID: 11404
		public Texture2D HomeIcon;

		// Token: 0x04002C8D RID: 11405
		public Texture2D InstrumentIcon;

		// Token: 0x04002C8E RID: 11406
		public Texture2D QuestionIcon;

		// Token: 0x04002C8F RID: 11407
		public Texture2D UserIcon;

		// Token: 0x04002C90 RID: 11408
		public Texture2D AmazonIcon;

		// Token: 0x04002C91 RID: 11409
		public Texture2D GooglePlayIcon;

		// Token: 0x04002C92 RID: 11410
		public Texture2D iosIcon;

		// Token: 0x04002C93 RID: 11411
		public Texture2D macIcon;

		// Token: 0x04002C94 RID: 11412
		public Texture2D windowsPhoneIcon;

		// Token: 0x04002C95 RID: 11413
		[NonSerialized]
		public GUIStyle SignupButton;

		// Token: 0x04002C96 RID: 11414
		public bool UsePlayerSettingsBuildNumber;

		// Token: 0x04002C97 RID: 11415
		public bool SubmitErrors = true;

		// Token: 0x04002C98 RID: 11416
		public int MaxErrorCount = 10;

		// Token: 0x04002C99 RID: 11417
		public bool SubmitFpsAverage = true;

		// Token: 0x04002C9A RID: 11418
		public bool SubmitFpsCritical = true;

		// Token: 0x04002C9B RID: 11419
		public bool IncludeGooglePlay = true;

		// Token: 0x04002C9C RID: 11420
		public int FpsCriticalThreshold = 20;

		// Token: 0x04002C9D RID: 11421
		public int FpsCirticalSubmitInterval = 1;

		// Token: 0x04002C9E RID: 11422
		public List<bool> PlatformFoldOut = new List<bool>();

		// Token: 0x04002C9F RID: 11423
		public bool CustomDimensions01FoldOut;

		// Token: 0x04002CA0 RID: 11424
		public bool CustomDimensions02FoldOut;

		// Token: 0x04002CA1 RID: 11425
		public bool CustomDimensions03FoldOut;

		// Token: 0x04002CA2 RID: 11426
		public bool ResourceItemTypesFoldOut;

		// Token: 0x04002CA3 RID: 11427
		public bool ResourceCurrenciesFoldOut;

		// Token: 0x04002CA4 RID: 11428
		public static readonly RuntimePlatform[] AvailablePlatforms;

		// Token: 0x060031BE RID: 12734 RVA: 0x00025EBE File Offset: 0x000240BE
		public void SetCustomUserID(string customID)
		{
			customID != string.Empty;
		}

		// Token: 0x060031BF RID: 12735 RVA: 0x000EE240 File Offset: 0x000EC440
		public void RemovePlatformAtIndex(int index)
		{
			if (index >= 0 && index < this.Platforms.Count)
			{
				this.gameKey.RemoveAt(index);
				this.secretKey.RemoveAt(index);
				this.Build.RemoveAt(index);
				this.SelectedPlatformStudio.RemoveAt(index);
				this.SelectedPlatformGame.RemoveAt(index);
				this.SelectedPlatformGameID.RemoveAt(index);
				this.SelectedStudio.RemoveAt(index);
				this.SelectedGame.RemoveAt(index);
				this.PlatformFoldOut.RemoveAt(index);
				this.Platforms.RemoveAt(index);
			}
		}

		// Token: 0x060031C0 RID: 12736 RVA: 0x000EE2DC File Offset: 0x000EC4DC
		public void AddPlatform(RuntimePlatform platform)
		{
			this.gameKey.Add("");
			this.secretKey.Add("");
			this.Build.Add("0.1");
			this.SelectedPlatformStudio.Add("");
			this.SelectedPlatformGame.Add("");
			this.SelectedPlatformGameID.Add(-1);
			this.SelectedStudio.Add(0);
			this.SelectedGame.Add(0);
			this.PlatformFoldOut.Add(true);
			this.Platforms.Add(platform);
		}

		// Token: 0x060031C1 RID: 12737 RVA: 0x000EE378 File Offset: 0x000EC578
		public string[] GetAvailablePlatforms()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < Settings.AvailablePlatforms.Length; i++)
			{
				RuntimePlatform runtimePlatform = Settings.AvailablePlatforms[i];
				if (runtimePlatform == RuntimePlatform.IPhonePlayer)
				{
					if (!this.Platforms.Contains(RuntimePlatform.tvOS) && !this.Platforms.Contains(runtimePlatform))
					{
						list.Add(runtimePlatform.ToString());
					}
					else if (!this.Platforms.Contains(runtimePlatform))
					{
						list.Add(runtimePlatform.ToString());
					}
				}
				else if (runtimePlatform == RuntimePlatform.tvOS)
				{
					if (!this.Platforms.Contains(RuntimePlatform.IPhonePlayer) && !this.Platforms.Contains(runtimePlatform))
					{
						list.Add(runtimePlatform.ToString());
					}
					else if (!this.Platforms.Contains(runtimePlatform))
					{
						list.Add(runtimePlatform.ToString());
					}
				}
				else if (runtimePlatform == RuntimePlatform.MetroPlayerARM)
				{
					if (!this.Platforms.Contains(runtimePlatform))
					{
						list.Add("WSA");
					}
				}
				else if (!this.Platforms.Contains(runtimePlatform))
				{
					list.Add(runtimePlatform.ToString());
				}
			}
			return list.ToArray();
		}

		// Token: 0x060031C2 RID: 12738 RVA: 0x000EE4B0 File Offset: 0x000EC6B0
		public bool IsGameKeyValid(int index, string value)
		{
			bool result = true;
			for (int i = 0; i < this.Platforms.Count; i++)
			{
				if (index != i && value.Equals(this.gameKey[i]))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		// Token: 0x060031C3 RID: 12739 RVA: 0x000EE4F4 File Offset: 0x000EC6F4
		public bool IsSecretKeyValid(int index, string value)
		{
			bool result = true;
			for (int i = 0; i < this.Platforms.Count; i++)
			{
				if (index != i && value.Equals(this.secretKey[i]))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		// Token: 0x060031C4 RID: 12740 RVA: 0x000EE538 File Offset: 0x000EC738
		public void UpdateGameKey(int index, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (this.IsGameKeyValid(index, value))
				{
					this.gameKey[index] = value;
					return;
				}
				if (this.gameKey[index].Equals(value))
				{
					this.gameKey[index] = "";
					return;
				}
			}
			else
			{
				this.gameKey[index] = value;
			}
		}

		// Token: 0x060031C5 RID: 12741 RVA: 0x000EE598 File Offset: 0x000EC798
		public void UpdateSecretKey(int index, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (this.IsSecretKeyValid(index, value))
				{
					this.secretKey[index] = value;
					return;
				}
				if (this.secretKey[index].Equals(value))
				{
					this.secretKey[index] = "";
					return;
				}
			}
			else
			{
				this.secretKey[index] = value;
			}
		}

		// Token: 0x060031C6 RID: 12742 RVA: 0x00025ECC File Offset: 0x000240CC
		public string GetGameKey(int index)
		{
			return this.gameKey[index];
		}

		// Token: 0x060031C7 RID: 12743 RVA: 0x00025EDA File Offset: 0x000240DA
		public string GetSecretKey(int index)
		{
			return this.secretKey[index];
		}

		// Token: 0x060031C8 RID: 12744 RVA: 0x00002ECE File Offset: 0x000010CE
		public void SetCustomArea(string customArea)
		{
		}

		// Token: 0x060031C9 RID: 12745 RVA: 0x00002ECE File Offset: 0x000010CE
		public void SetKeys(string gamekey, string secretkey)
		{
		}

		// Token: 0x060031CB RID: 12747 RVA: 0x00025EE8 File Offset: 0x000240E8
		// Note: this type is marked as 'beforefieldinit'.
		static Settings()
		{
			RuntimePlatform[] array = new RuntimePlatform[9];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.6E5E6830F0A82B8603B122C02111F18D3639978F).FieldHandle);
			Settings.AvailablePlatforms = array;
		}

		// Token: 0x02000917 RID: 2327
		public enum HelpTypes
		{
			// Token: 0x04002CA6 RID: 11430
			None,
			// Token: 0x04002CA7 RID: 11431
			IncludeSystemSpecsHelp,
			// Token: 0x04002CA8 RID: 11432
			ProvideCustomUserID
		}

		// Token: 0x02000918 RID: 2328
		public enum MessageTypes
		{
			// Token: 0x04002CAA RID: 11434
			None,
			// Token: 0x04002CAB RID: 11435
			Error,
			// Token: 0x04002CAC RID: 11436
			Info,
			// Token: 0x04002CAD RID: 11437
			Warning
		}

		// Token: 0x02000919 RID: 2329
		public struct HelpInfo
		{
			// Token: 0x04002CAE RID: 11438
			public string Message;

			// Token: 0x04002CAF RID: 11439
			public Settings.MessageTypes MsgType;

			// Token: 0x04002CB0 RID: 11440
			public Settings.HelpTypes HelpType;
		}

		// Token: 0x0200091A RID: 2330
		public enum InspectorStates
		{
			// Token: 0x04002CB2 RID: 11442
			Account,
			// Token: 0x04002CB3 RID: 11443
			Basic,
			// Token: 0x04002CB4 RID: 11444
			Debugging,
			// Token: 0x04002CB5 RID: 11445
			Pref
		}
	}
}
