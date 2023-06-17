using System;

namespace GameTips
{
	// Token: 0x020007E9 RID: 2025
	public abstract class BaseTip
	{
		// Token: 0x06002C36 RID: 11318
		public abstract Translate.Phrase GetPhrase();

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06002C37 RID: 11319
		public abstract bool ShouldShow { get; }

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06002C38 RID: 11320 RVA: 0x0002278D File Offset: 0x0002098D
		public string Type
		{
			get
			{
				return base.GetType().Name;
			}
		}
	}
}
