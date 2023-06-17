using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameMenu
{
	// Token: 0x0200081D RID: 2077
	public interface IProvider
	{
		// Token: 0x06002D25 RID: 11557
		Info GetMenuInformation(GameObject primaryObject, BasePlayer player);

		// Token: 0x06002D26 RID: 11558
		List<Option> GetMenuItems(BasePlayer player);
	}
}
