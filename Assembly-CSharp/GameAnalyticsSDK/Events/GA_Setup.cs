using System;
using System.Collections.Generic;
using GameAnalyticsSDK.Utilities;
using GameAnalyticsSDK.Validators;
using GameAnalyticsSDK.Wrapper;

namespace GameAnalyticsSDK.Events
{
	// Token: 0x02000924 RID: 2340
	public static class GA_Setup
	{
		// Token: 0x0600320E RID: 12814 RVA: 0x0002637B File Offset: 0x0002457B
		public static void SetAvailableCustomDimensions01(List<string> customDimensions)
		{
			if (GAValidator.ValidateCustomDimensions(customDimensions.ToArray()))
			{
				GA_Wrapper.SetAvailableCustomDimensions01(GA_MiniJSON.Serialize(customDimensions));
			}
		}

		// Token: 0x0600320F RID: 12815 RVA: 0x00026395 File Offset: 0x00024595
		public static void SetAvailableCustomDimensions02(List<string> customDimensions)
		{
			if (GAValidator.ValidateCustomDimensions(customDimensions.ToArray()))
			{
				GA_Wrapper.SetAvailableCustomDimensions02(GA_MiniJSON.Serialize(customDimensions));
			}
		}

		// Token: 0x06003210 RID: 12816 RVA: 0x000263AF File Offset: 0x000245AF
		public static void SetAvailableCustomDimensions03(List<string> customDimensions)
		{
			if (GAValidator.ValidateCustomDimensions(customDimensions.ToArray()))
			{
				GA_Wrapper.SetAvailableCustomDimensions03(GA_MiniJSON.Serialize(customDimensions));
			}
		}

		// Token: 0x06003211 RID: 12817 RVA: 0x000263C9 File Offset: 0x000245C9
		public static void SetAvailableResourceCurrencies(List<string> resourceCurrencies)
		{
			if (GAValidator.ValidateResourceCurrencies(resourceCurrencies.ToArray()))
			{
				GA_Wrapper.SetAvailableResourceCurrencies(GA_MiniJSON.Serialize(resourceCurrencies));
			}
		}

		// Token: 0x06003212 RID: 12818 RVA: 0x000263E3 File Offset: 0x000245E3
		public static void SetAvailableResourceItemTypes(List<string> resourceItemTypes)
		{
			if (GAValidator.ValidateResourceItemTypes(resourceItemTypes.ToArray()))
			{
				GA_Wrapper.SetAvailableResourceItemTypes(GA_MiniJSON.Serialize(resourceItemTypes));
			}
		}

		// Token: 0x06003213 RID: 12819 RVA: 0x000263FD File Offset: 0x000245FD
		public static void SetInfoLog(bool enabled)
		{
			GA_Wrapper.SetInfoLog(enabled);
		}

		// Token: 0x06003214 RID: 12820 RVA: 0x00026405 File Offset: 0x00024605
		public static void SetVerboseLog(bool enabled)
		{
			GA_Wrapper.SetVerboseLog(enabled);
		}

		// Token: 0x06003215 RID: 12821 RVA: 0x0002640D File Offset: 0x0002460D
		public static void SetFacebookId(string facebookId)
		{
			GA_Wrapper.SetFacebookId(facebookId);
		}

		// Token: 0x06003216 RID: 12822 RVA: 0x000EEF88 File Offset: 0x000ED188
		public static void SetGender(GAGender gender)
		{
			if (gender == GAGender.male)
			{
				GA_Wrapper.SetGender(GAGender.male.ToString());
				return;
			}
			if (gender != GAGender.female)
			{
				return;
			}
			GA_Wrapper.SetGender(GAGender.female.ToString());
		}

		// Token: 0x06003217 RID: 12823 RVA: 0x00026415 File Offset: 0x00024615
		public static void SetBirthYear(int birthYear)
		{
			GA_Wrapper.SetBirthYear(birthYear);
		}

		// Token: 0x06003218 RID: 12824 RVA: 0x0002641D File Offset: 0x0002461D
		public static void SetCustomDimension01(string customDimension)
		{
			GA_Wrapper.SetCustomDimension01(customDimension);
		}

		// Token: 0x06003219 RID: 12825 RVA: 0x00026425 File Offset: 0x00024625
		public static void SetCustomDimension02(string customDimension)
		{
			GA_Wrapper.SetCustomDimension02(customDimension);
		}

		// Token: 0x0600321A RID: 12826 RVA: 0x0002642D File Offset: 0x0002462D
		public static void SetCustomDimension03(string customDimension)
		{
			GA_Wrapper.SetCustomDimension03(customDimension);
		}
	}
}
