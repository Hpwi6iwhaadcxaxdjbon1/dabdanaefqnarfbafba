using System;
using System.Text.RegularExpressions;
using GameAnalyticsSDK.State;
using UnityEngine;

namespace GameAnalyticsSDK.Validators
{
	// Token: 0x0200091D RID: 2333
	internal static class GAValidator
	{
		// Token: 0x060031DE RID: 12766 RVA: 0x00025FCA File Offset: 0x000241CA
		public static bool StringMatch(string s, string pattern)
		{
			return s != null && pattern != null && Regex.IsMatch(s, pattern);
		}

		// Token: 0x060031DF RID: 12767 RVA: 0x000EE8DC File Offset: 0x000ECADC
		public static bool ValidateBusinessEvent(string currency, int amount, string cartType, string itemType, string itemId)
		{
			if (!GAValidator.ValidateCurrency(currency))
			{
				Debug.Log("Validation fail - business event - currency: Cannot be (null) and need to be A-Z, 3 characters and in the standard at openexchangerates.org. Failed currency: " + currency);
				return false;
			}
			if (!GAValidator.ValidateShortString(cartType, true))
			{
				Debug.Log("Validation fail - business event - cartType. Cannot be above 32 length. String: " + cartType);
				return false;
			}
			if (!GAValidator.ValidateEventPartLength(itemType, false))
			{
				Debug.Log("Validation fail - business event - itemType: Cannot be (null), empty or above 64 characters. String: " + itemType);
				return false;
			}
			if (!GAValidator.ValidateEventPartCharacters(itemType))
			{
				Debug.Log("Validation fail - business event - itemType: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + itemType);
				return false;
			}
			if (!GAValidator.ValidateEventPartLength(itemId, false))
			{
				Debug.Log("Validation fail - business event - itemId. Cannot be (null), empty or above 64 characters. String: " + itemId);
				return false;
			}
			if (!GAValidator.ValidateEventPartCharacters(itemId))
			{
				Debug.Log("Validation fail - business event - itemId: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + itemId);
				return false;
			}
			return true;
		}

		// Token: 0x060031E0 RID: 12768 RVA: 0x000EE990 File Offset: 0x000ECB90
		public static bool ValidateResourceEvent(GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId)
		{
			if (string.IsNullOrEmpty(currency))
			{
				Debug.Log("Validation fail - resource event - currency: Cannot be (null)");
				return false;
			}
			if (flowType == GAResourceFlowType.Undefined)
			{
				Debug.Log("Validation fail - resource event - flowType: Invalid flowType");
			}
			if (!GAState.HasAvailableResourceCurrency(currency))
			{
				Debug.Log("Validation fail - resource event - currency: Not found in list of pre-defined resource currencies. String: " + currency);
				return false;
			}
			if (amount <= 0f)
			{
				Debug.Log("Validation fail - resource event - amount: Float amount cannot be 0 or negative. Value: " + amount);
				return false;
			}
			if (string.IsNullOrEmpty(itemType))
			{
				Debug.Log("Validation fail - resource event - itemType: Cannot be (null)");
				return false;
			}
			if (!GAValidator.ValidateEventPartLength(itemType, false))
			{
				Debug.Log("Validation fail - resource event - itemType: Cannot be (null), empty or above 64 characters. String: " + itemType);
				return false;
			}
			if (!GAValidator.ValidateEventPartCharacters(itemType))
			{
				Debug.Log("Validation fail - resource event - itemType: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + itemType);
				return false;
			}
			if (!GAState.HasAvailableResourceItemType(itemType))
			{
				Debug.Log("Validation fail - resource event - itemType: Not found in list of pre-defined available resource itemTypes. String: " + itemType);
				return false;
			}
			if (!GAValidator.ValidateEventPartLength(itemId, false))
			{
				Debug.Log("Validation fail - resource event - itemId: Cannot be (null), empty or above 64 characters. String: " + itemId);
				return false;
			}
			if (!GAValidator.ValidateEventPartCharacters(itemId))
			{
				Debug.Log("Validation fail - resource event - itemId: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + itemId);
				return false;
			}
			return true;
		}

		// Token: 0x060031E1 RID: 12769 RVA: 0x000EEA94 File Offset: 0x000ECC94
		public static bool ValidateProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03)
		{
			if (progressionStatus == GAProgressionStatus.Undefined)
			{
				Debug.Log("Validation fail - progression event: Invalid progression status.");
				return false;
			}
			if (!string.IsNullOrEmpty(progression03) && string.IsNullOrEmpty(progression02) && !string.IsNullOrEmpty(progression01))
			{
				Debug.Log("Validation fail - progression event: 03 found but 01+02 are invalid. Progression must be set as either 01, 01+02 or 01+02+03.");
				return false;
			}
			if (!string.IsNullOrEmpty(progression02) && string.IsNullOrEmpty(progression01))
			{
				Debug.Log("Validation fail - progression event: 02 found but not 01. Progression must be set as either 01, 01+02 or 01+02+03");
				return false;
			}
			if (string.IsNullOrEmpty(progression01))
			{
				Debug.Log("Validation fail - progression event: progression01 not valid. Progressions must be set as either 01, 01+02 or 01+02+03");
				return false;
			}
			if (!GAValidator.ValidateEventPartLength(progression01, false))
			{
				Debug.Log("Validation fail - progression event - progression01: Cannot be (null), empty or above 64 characters. String: " + progression01);
				return false;
			}
			if (!GAValidator.ValidateEventPartCharacters(progression01))
			{
				Debug.Log("Validation fail - progression event - progression01: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + progression01);
				return false;
			}
			if (!string.IsNullOrEmpty(progression02))
			{
				if (!GAValidator.ValidateEventPartLength(progression02, true))
				{
					Debug.Log("Validation fail - progression event - progression02: Cannot be empty or above 64 characters. String: " + progression02);
					return false;
				}
				if (!GAValidator.ValidateEventPartCharacters(progression02))
				{
					Debug.Log("Validation fail - progression event - progression02: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + progression02);
					return false;
				}
			}
			if (!string.IsNullOrEmpty(progression03))
			{
				if (!GAValidator.ValidateEventPartLength(progression03, true))
				{
					Debug.Log("Validation fail - progression event - progression03: Cannot be empty or above 64 characters. String: " + progression03);
					return false;
				}
				if (!GAValidator.ValidateEventPartCharacters(progression03))
				{
					Debug.Log("Validation fail - progression event - progression03: Cannot contain other characters than A-z, 0-9, -_., ()!?. String: " + progression03);
					return false;
				}
			}
			return true;
		}

		// Token: 0x060031E2 RID: 12770 RVA: 0x00025FDB File Offset: 0x000241DB
		public static bool ValidateDesignEvent(string eventId)
		{
			if (!GAValidator.ValidateEventIdLength(eventId))
			{
				Debug.Log("Validation fail - design event - eventId: Cannot be (null) or empty. Only 5 event parts allowed seperated by :. Each part need to be 32 characters or less. String: " + eventId);
				return false;
			}
			if (!GAValidator.ValidateEventIdCharacters(eventId))
			{
				Debug.Log("Validation fail - design event - eventId: Non valid characters. Only allowed A-z, 0-9, -_., ()!?. String: " + eventId);
				return false;
			}
			return true;
		}

		// Token: 0x060031E3 RID: 12771 RVA: 0x00026012 File Offset: 0x00024212
		public static bool ValidateErrorEvent(GAErrorSeverity severity, string message)
		{
			if (severity == GAErrorSeverity.Undefined)
			{
				Debug.Log("Validation fail - error event - severity: Severity was unsupported value.");
				return false;
			}
			if (!GAValidator.ValidateLongString(message, true))
			{
				Debug.Log("Validation fail - error event - message: Message cannot be above 8192 characters.");
				return false;
			}
			return true;
		}

		// Token: 0x060031E4 RID: 12772 RVA: 0x00026039 File Offset: 0x00024239
		public static bool ValidateSdkErrorEvent(string gameKey, string gameSecret, GAErrorSeverity type)
		{
			if (!GAValidator.ValidateKeys(gameKey, gameSecret))
			{
				return false;
			}
			if (type == GAErrorSeverity.Undefined)
			{
				Debug.Log("Validation fail - sdk error event - type: Type was unsupported value.");
				return false;
			}
			return true;
		}

		// Token: 0x060031E5 RID: 12773 RVA: 0x00026056 File Offset: 0x00024256
		public static bool ValidateKeys(string gameKey, string gameSecret)
		{
			return GAValidator.StringMatch(gameKey, "^[A-z0-9]{32}$") && GAValidator.StringMatch(gameSecret, "^[A-z0-9]{40}$");
		}

		// Token: 0x060031E6 RID: 12774 RVA: 0x00026075 File Offset: 0x00024275
		public static bool ValidateCurrency(string currency)
		{
			return !string.IsNullOrEmpty(currency) && GAValidator.StringMatch(currency, "^[A-Z]{3}$");
		}

		// Token: 0x060031E7 RID: 12775 RVA: 0x00026091 File Offset: 0x00024291
		public static bool ValidateEventPartLength(string eventPart, bool allowNull)
		{
			return (allowNull && string.IsNullOrEmpty(eventPart)) || (!string.IsNullOrEmpty(eventPart) && eventPart.Length <= 64);
		}

		// Token: 0x060031E8 RID: 12776 RVA: 0x000260B7 File Offset: 0x000242B7
		public static bool ValidateEventPartCharacters(string eventPart)
		{
			return GAValidator.StringMatch(eventPart, "^[A-Za-z0-9\\s\\-_\\.\\(\\)\\!\\?]{1,64}$");
		}

		// Token: 0x060031E9 RID: 12777 RVA: 0x000260C9 File Offset: 0x000242C9
		public static bool ValidateEventIdLength(string eventId)
		{
			return !string.IsNullOrEmpty(eventId) && GAValidator.StringMatch(eventId, "^[^:]{1,64}(?::[^:]{1,64}){0,4}$");
		}

		// Token: 0x060031EA RID: 12778 RVA: 0x000260E5 File Offset: 0x000242E5
		public static bool ValidateEventIdCharacters(string eventId)
		{
			return !string.IsNullOrEmpty(eventId) && GAValidator.StringMatch(eventId, "^[A-Za-z0-9\\s\\-_\\.\\(\\)\\!\\?]{1,64}(:[A-Za-z0-9\\s\\-_\\.\\(\\)\\!\\?]{1,64}){0,4}$");
		}

		// Token: 0x060031EB RID: 12779 RVA: 0x00026101 File Offset: 0x00024301
		public static bool ValidateBuild(string build)
		{
			return GAValidator.ValidateShortString(build, false);
		}

		// Token: 0x060031EC RID: 12780 RVA: 0x0002610F File Offset: 0x0002430F
		public static bool ValidateUserId(string uId)
		{
			if (!GAValidator.ValidateString(uId, false))
			{
				Debug.Log("Validation fail - user id: id cannot be (null), empty or above 64 characters.");
				return false;
			}
			return true;
		}

		// Token: 0x060031ED RID: 12781 RVA: 0x00026127 File Offset: 0x00024327
		public static bool ValidateShortString(string shortString, bool canBeEmpty)
		{
			return (canBeEmpty && string.IsNullOrEmpty(shortString)) || (!string.IsNullOrEmpty(shortString) && shortString.Length <= 32);
		}

		// Token: 0x060031EE RID: 12782 RVA: 0x0002614B File Offset: 0x0002434B
		public static bool ValidateString(string s, bool canBeEmpty)
		{
			return (canBeEmpty && string.IsNullOrEmpty(s)) || (!string.IsNullOrEmpty(s) && s.Length <= 64);
		}

		// Token: 0x060031EF RID: 12783 RVA: 0x0002616F File Offset: 0x0002436F
		public static bool ValidateLongString(string longString, bool canBeEmpty)
		{
			return (canBeEmpty && string.IsNullOrEmpty(longString)) || (!string.IsNullOrEmpty(longString) && longString.Length <= 8192);
		}

		// Token: 0x060031F0 RID: 12784 RVA: 0x00026196 File Offset: 0x00024396
		public static bool ValidateConnectionType(string connectionType)
		{
			return GAValidator.StringMatch(connectionType, "^(wwan|wifi|lan|offline)$");
		}

		// Token: 0x060031F1 RID: 12785 RVA: 0x000261A3 File Offset: 0x000243A3
		public static bool ValidateCustomDimensions(params string[] customDimensions)
		{
			return GAValidator.ValidateArrayOfStrings(20L, 32L, false, "custom dimensions", customDimensions);
		}

		// Token: 0x060031F2 RID: 12786 RVA: 0x000EEBB4 File Offset: 0x000ECDB4
		public static bool ValidateResourceCurrencies(params string[] resourceCurrencies)
		{
			if (!GAValidator.ValidateArrayOfStrings(20L, 64L, false, "resource currencies", resourceCurrencies))
			{
				return false;
			}
			foreach (string text in resourceCurrencies)
			{
				if (!GAValidator.StringMatch(text, "^[A-Za-z]+$"))
				{
					Debug.Log("resource currencies validation failed: a resource currency can only be A-Z, a-z. String was: " + text);
					return false;
				}
			}
			return true;
		}

		// Token: 0x060031F3 RID: 12787 RVA: 0x000EEC0C File Offset: 0x000ECE0C
		public static bool ValidateResourceItemTypes(params string[] resourceItemTypes)
		{
			if (!GAValidator.ValidateArrayOfStrings(20L, 32L, false, "resource item types", resourceItemTypes))
			{
				return false;
			}
			foreach (string text in resourceItemTypes)
			{
				if (!GAValidator.ValidateEventPartCharacters(text))
				{
					Debug.Log("resource item types validation failed: a resource item type cannot contain other characters than A-z, 0-9, -_., ()!?. String was: " + text);
					return false;
				}
			}
			return true;
		}

		// Token: 0x060031F4 RID: 12788 RVA: 0x000261B7 File Offset: 0x000243B7
		public static bool ValidateDimension01(string dimension01)
		{
			if (string.IsNullOrEmpty(dimension01))
			{
				Debug.Log("Validation failed - custom dimension01 - value cannot be empty.");
				return false;
			}
			if (!GAState.HasAvailableCustomDimensions01(dimension01))
			{
				Debug.Log("Validation failed - custom dimension 01 - value was not found in list of custom dimensions 01 in the Settings object. \nGiven dimension value: " + dimension01);
				return false;
			}
			return true;
		}

		// Token: 0x060031F5 RID: 12789 RVA: 0x000261E8 File Offset: 0x000243E8
		public static bool ValidateDimension02(string dimension02)
		{
			if (string.IsNullOrEmpty(dimension02))
			{
				Debug.Log("Validation failed - custom dimension01 - value cannot be empty.");
				return false;
			}
			if (!GAState.HasAvailableCustomDimensions02(dimension02))
			{
				Debug.Log("Validation failed - custom dimension 02 - value was not found in list of custom dimensions 02 in the Settings object. \nGiven dimension value: " + dimension02);
				return false;
			}
			return true;
		}

		// Token: 0x060031F6 RID: 12790 RVA: 0x00026219 File Offset: 0x00024419
		public static bool ValidateDimension03(string dimension03)
		{
			if (string.IsNullOrEmpty(dimension03))
			{
				Debug.Log("Validation failed - custom dimension01 - value cannot be empty.");
				return false;
			}
			if (!GAState.HasAvailableCustomDimensions03(dimension03))
			{
				Debug.Log("Validation failed - custom dimension 03 - value was not found in list of custom dimensions 03 in the Settings object. \nGiven dimension value: " + dimension03);
				return false;
			}
			return true;
		}

		// Token: 0x060031F7 RID: 12791 RVA: 0x000EEC60 File Offset: 0x000ECE60
		public static bool ValidateArrayOfStrings(long maxCount, long maxStringLength, bool allowNoValues, string logTag, params string[] arrayOfStrings)
		{
			string text = logTag;
			if (string.IsNullOrEmpty(text))
			{
				text = "Array";
			}
			if (arrayOfStrings == null)
			{
				Debug.Log(text + " validation failed: array cannot be null. ");
				return false;
			}
			if (!allowNoValues && arrayOfStrings.Length == 0)
			{
				Debug.Log(text + " validation failed: array cannot be empty. ");
				return false;
			}
			if (maxCount > 0L && (long)arrayOfStrings.Length > maxCount)
			{
				Debug.Log(string.Concat(new object[]
				{
					text,
					" validation failed: array cannot exceed ",
					maxCount,
					" values. It has ",
					arrayOfStrings.Length,
					" values."
				}));
				return false;
			}
			foreach (string text2 in arrayOfStrings)
			{
				int num = (text2 == null) ? 0 : text2.Length;
				if (num == 0)
				{
					Debug.Log(text + " validation failed: contained an empty string.");
					return false;
				}
				if (maxStringLength > 0L && (long)num > maxStringLength)
				{
					Debug.Log(string.Concat(new object[]
					{
						text,
						" validation failed: a string exceeded max allowed length (which is: ",
						maxStringLength,
						"). String was: ",
						text2
					}));
					return false;
				}
			}
			return true;
		}

		// Token: 0x060031F8 RID: 12792 RVA: 0x0002624A File Offset: 0x0002444A
		public static bool ValidateFacebookId(string facebookId)
		{
			if (!GAValidator.ValidateString(facebookId, false))
			{
				Debug.Log("Validation fail - facebook id: id cannot be (null), empty or above 64 characters.");
				return false;
			}
			return true;
		}

		// Token: 0x060031F9 RID: 12793 RVA: 0x000EED78 File Offset: 0x000ECF78
		public static bool ValidateGender(string gender)
		{
			if (gender == "" || (!(gender == GAGender.male.ToString()) && !(gender == GAGender.female.ToString())))
			{
				Debug.Log("Validation fail - gender: Has to be 'male' or 'female'.Given gender:" + gender);
				return false;
			}
			return true;
		}

		// Token: 0x060031FA RID: 12794 RVA: 0x00026262 File Offset: 0x00024462
		public static bool ValidateBirthyear(int birthYear)
		{
			if (birthYear < 0 || birthYear > 9999)
			{
				Debug.Log("Validation fail - birthYear: Cannot be (null) or invalid range.");
				return false;
			}
			return true;
		}

		// Token: 0x060031FB RID: 12795 RVA: 0x0002627D File Offset: 0x0002447D
		public static bool ValidateClientTs(long clientTs)
		{
			return clientTs >= -9223372036854775807L && clientTs <= 9223372036854775806L;
		}
	}
}
