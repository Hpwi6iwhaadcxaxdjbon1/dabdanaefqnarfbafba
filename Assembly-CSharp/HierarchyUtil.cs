using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000748 RID: 1864
public static class HierarchyUtil
{
	// Token: 0x040023F9 RID: 9209
	public static Dictionary<string, GameObject> rootDict = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x0600287A RID: 10362 RVA: 0x000D0568 File Offset: 0x000CE768
	public static GameObject GetRoot(string strName, bool groupActive = true, bool persistant = false)
	{
		GameObject gameObject;
		if (HierarchyUtil.rootDict.TryGetValue(strName, ref gameObject))
		{
			if (gameObject != null)
			{
				return gameObject;
			}
			HierarchyUtil.rootDict.Remove(strName);
		}
		gameObject = new GameObject(strName);
		gameObject.SetActive(groupActive);
		HierarchyUtil.rootDict.Add(strName, gameObject);
		if (persistant)
		{
			Object.DontDestroyOnLoad(gameObject);
		}
		return gameObject;
	}
}
