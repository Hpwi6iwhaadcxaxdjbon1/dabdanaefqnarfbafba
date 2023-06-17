using System;
using System.Collections;
using UnityEngine;

namespace GameAnalyticsSDK.Events
{
	// Token: 0x02000925 RID: 2341
	public class GA_SpecialEvents : MonoBehaviour
	{
		// Token: 0x04002CC1 RID: 11457
		private static int _frameCountAvg;

		// Token: 0x04002CC2 RID: 11458
		private static float _lastUpdateAvg;

		// Token: 0x04002CC3 RID: 11459
		private int _frameCountCrit;

		// Token: 0x04002CC4 RID: 11460
		private float _lastUpdateCrit;

		// Token: 0x04002CC5 RID: 11461
		private static int _criticalFpsCount;

		// Token: 0x0600321B RID: 12827 RVA: 0x00026435 File Offset: 0x00024635
		public void Start()
		{
			base.StartCoroutine(this.SubmitFPSRoutine());
			base.StartCoroutine(this.CheckCriticalFPSRoutine());
		}

		// Token: 0x0600321C RID: 12828 RVA: 0x00026451 File Offset: 0x00024651
		private IEnumerator SubmitFPSRoutine()
		{
			while (Application.isPlaying && GameAnalytics.SettingsGA.SubmitFpsAverage)
			{
				yield return new WaitForSeconds(30f);
				GA_SpecialEvents.SubmitFPS();
			}
			yield break;
		}

		// Token: 0x0600321D RID: 12829 RVA: 0x00026459 File Offset: 0x00024659
		private IEnumerator CheckCriticalFPSRoutine()
		{
			while (Application.isPlaying && GameAnalytics.SettingsGA.SubmitFpsCritical)
			{
				yield return new WaitForSeconds((float)GameAnalytics.SettingsGA.FpsCirticalSubmitInterval);
				this.CheckCriticalFPS();
			}
			yield break;
		}

		// Token: 0x0600321E RID: 12830 RVA: 0x00026468 File Offset: 0x00024668
		public void Update()
		{
			if (GameAnalytics.SettingsGA.SubmitFpsAverage)
			{
				GA_SpecialEvents._frameCountAvg++;
			}
			if (GameAnalytics.SettingsGA.SubmitFpsCritical)
			{
				this._frameCountCrit++;
			}
		}

		// Token: 0x0600321F RID: 12831 RVA: 0x000EEFC8 File Offset: 0x000ED1C8
		public static void SubmitFPS()
		{
			if (GameAnalytics.SettingsGA.SubmitFpsAverage)
			{
				float num = Time.time - GA_SpecialEvents._lastUpdateAvg;
				if (num > 1f)
				{
					float num2 = (float)GA_SpecialEvents._frameCountAvg / num;
					GA_SpecialEvents._lastUpdateAvg = Time.time;
					GA_SpecialEvents._frameCountAvg = 0;
					if (num2 > 0f)
					{
						GameAnalytics.NewDesignEvent("GA:AverageFPS", (float)((int)num2));
					}
				}
			}
			if (GameAnalytics.SettingsGA.SubmitFpsCritical && GA_SpecialEvents._criticalFpsCount > 0)
			{
				GameAnalytics.NewDesignEvent("GA:CriticalFPS", (float)GA_SpecialEvents._criticalFpsCount);
				GA_SpecialEvents._criticalFpsCount = 0;
			}
		}

		// Token: 0x06003220 RID: 12832 RVA: 0x000EF050 File Offset: 0x000ED250
		public void CheckCriticalFPS()
		{
			if (GameAnalytics.SettingsGA.SubmitFpsCritical)
			{
				float num = Time.time - this._lastUpdateCrit;
				if (num >= 1f)
				{
					float num2 = (float)this._frameCountCrit / num;
					this._lastUpdateCrit = Time.time;
					this._frameCountCrit = 0;
					if (num2 <= (float)GameAnalytics.SettingsGA.FpsCriticalThreshold)
					{
						GA_SpecialEvents._criticalFpsCount++;
					}
				}
			}
		}
	}
}
