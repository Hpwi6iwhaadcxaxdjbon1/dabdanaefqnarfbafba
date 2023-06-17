using System;
using UnityEngine;

namespace GameMenu
{
	// Token: 0x0200081E RID: 2078
	public static class Util
	{
		// Token: 0x06002D27 RID: 11559 RVA: 0x000E3158 File Offset: 0x000E1358
		public static Info GetInfo(GameObject obj, BasePlayer player)
		{
			IProvider provider = obj.GetComponentInParent<IProvider>();
			Info result;
			if (provider == null)
			{
				BaseEntity baseEntity = obj.ToBaseEntity();
				if (baseEntity == null)
				{
					result = default(Info);
					return result;
				}
				provider = baseEntity.GetComponent<IProvider>();
				if (provider == null)
				{
					result = default(Info);
					return result;
				}
			}
			using (TimeWarning.New("GetMenuInformation", 0.1f))
			{
				result = provider.GetMenuInformation(obj, player);
			}
			return result;
		}
	}
}
