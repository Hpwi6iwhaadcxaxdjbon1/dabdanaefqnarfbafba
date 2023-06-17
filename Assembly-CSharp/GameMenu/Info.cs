using System;
using UnityEngine;

namespace GameMenu
{
	// Token: 0x0200081B RID: 2075
	public struct Info
	{
		// Token: 0x040028B8 RID: 10424
		public string action;

		// Token: 0x040028B9 RID: 10425
		public string icon;

		// Token: 0x040028BA RID: 10426
		public bool hasMoreOptions;

		// Token: 0x040028BB RID: 10427
		public string entityName;

		// Token: 0x040028BC RID: 10428
		public Sprite iconSprite;

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06002D21 RID: 11553 RVA: 0x000232F7 File Offset: 0x000214F7
		public bool IsValid
		{
			get
			{
				return this.action != null;
			}
		}
	}
}
