using System;
using System.Collections.Generic;

namespace UnityEngine
{
	// Token: 0x02000830 RID: 2096
	public static class CoroutineEx
	{
		// Token: 0x040028F1 RID: 10481
		public static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

		// Token: 0x040028F2 RID: 10482
		public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

		// Token: 0x040028F3 RID: 10483
		private static Dictionary<float, WaitForSeconds> waitForSecondsBuffer = new Dictionary<float, WaitForSeconds>();

		// Token: 0x06002D79 RID: 11641 RVA: 0x000E49B4 File Offset: 0x000E2BB4
		public static WaitForSeconds waitForSeconds(float seconds)
		{
			WaitForSeconds waitForSeconds;
			if (!CoroutineEx.waitForSecondsBuffer.TryGetValue(seconds, ref waitForSeconds))
			{
				waitForSeconds = new WaitForSeconds(seconds);
				CoroutineEx.waitForSecondsBuffer.Add(seconds, waitForSeconds);
			}
			return waitForSeconds;
		}

		// Token: 0x06002D7A RID: 11642 RVA: 0x000235F1 File Offset: 0x000217F1
		public static WaitForSecondsRealtime waitForSecondsRealtime(float seconds)
		{
			return new WaitForSecondsRealtime(seconds);
		}
	}
}
