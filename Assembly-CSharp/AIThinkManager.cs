using System;
using UnityEngine;

// Token: 0x02000128 RID: 296
public class AIThinkManager : BaseMonoBehaviour
{
	// Token: 0x04000867 RID: 2151
	public static ListHashSet<IThinker> _processQueue = new ListHashSet<IThinker>(8);

	// Token: 0x04000868 RID: 2152
	public static ListHashSet<IThinker> _removalQueue = new ListHashSet<IThinker>(8);

	// Token: 0x04000869 RID: 2153
	[ServerVar]
	[Help("How many miliseconds to budget for processing AI entities per server frame")]
	public static float framebudgetms = 2.5f;

	// Token: 0x0400086A RID: 2154
	private static int lastIndex = 0;

	// Token: 0x06000BE5 RID: 3045 RVA: 0x0005AC28 File Offset: 0x00058E28
	public static void ProcessQueue()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = AIThinkManager.framebudgetms / 1000f;
		if (AIThinkManager._removalQueue.Count > 0)
		{
			foreach (IThinker thinker in AIThinkManager._removalQueue)
			{
				AIThinkManager._processQueue.Remove(thinker);
			}
			AIThinkManager._removalQueue.Clear();
		}
		while (AIThinkManager.lastIndex < AIThinkManager._processQueue.Count && Time.realtimeSinceStartup < realtimeSinceStartup + num)
		{
			IThinker thinker2 = AIThinkManager._processQueue[AIThinkManager.lastIndex];
			if (thinker2 != null)
			{
				thinker2.TryThink();
			}
			AIThinkManager.lastIndex++;
		}
		if (AIThinkManager.lastIndex == AIThinkManager._processQueue.Count)
		{
			AIThinkManager.lastIndex = 0;
		}
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x0000B3CE File Offset: 0x000095CE
	public static void Add(IThinker toAdd)
	{
		AIThinkManager._processQueue.Add(toAdd);
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x0000B3DB File Offset: 0x000095DB
	public static void Remove(IThinker toRemove)
	{
		AIThinkManager._removalQueue.Add(toRemove);
	}
}
