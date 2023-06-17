using System;
using UnityEngine;

namespace GameMenu
{
	// Token: 0x0200081C RID: 2076
	public struct Option
	{
		// Token: 0x040028BD RID: 10429
		public string title;

		// Token: 0x040028BE RID: 10430
		public string desc;

		// Token: 0x040028BF RID: 10431
		public string requirements;

		// Token: 0x040028C0 RID: 10432
		public string icon;

		// Token: 0x040028C1 RID: 10433
		public bool showOnItem;

		// Token: 0x040028C2 RID: 10434
		public int order;

		// Token: 0x040028C3 RID: 10435
		public float time;

		// Token: 0x040028C4 RID: 10436
		public bool longUseOnly;

		// Token: 0x040028C5 RID: 10437
		public bool showDisabled;

		// Token: 0x040028C6 RID: 10438
		public bool show;

		// Token: 0x040028C7 RID: 10439
		public Action<BasePlayer> function;

		// Token: 0x040028C8 RID: 10440
		public Action timeStart;

		// Token: 0x040028C9 RID: 10441
		public Action timeProgress;

		// Token: 0x040028CA RID: 10442
		public Sprite iconSprite;

		// Token: 0x040028CB RID: 10443
		public string command;

		// Token: 0x1700036F RID: 879
		// (set) Token: 0x06002D22 RID: 11554 RVA: 0x00023302 File Offset: 0x00021502
		public BaseEntity.Menu.Option copyOptionsFrom
		{
			set
			{
				value.CopyTo(ref this);
			}
		}

		// Token: 0x06002D23 RID: 11555 RVA: 0x0002330C File Offset: 0x0002150C
		public void RunTimeStart()
		{
			if (this.timeStart == null)
			{
				return;
			}
			this.timeStart.Invoke();
		}

		// Token: 0x06002D24 RID: 11556 RVA: 0x00023322 File Offset: 0x00021522
		public void RunTimeProgress()
		{
			if (this.timeProgress == null)
			{
				return;
			}
			this.timeProgress.Invoke();
		}
	}
}
