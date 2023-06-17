using System;

namespace Rust
{
	// Token: 0x020008B0 RID: 2224
	public static class Protocol
	{
		// Token: 0x04002AE4 RID: 10980
		public const int network = 2153;

		// Token: 0x04002AE5 RID: 10981
		public const int save = 176;

		// Token: 0x04002AE6 RID: 10982
		public const int report = 1;

		// Token: 0x04002AE7 RID: 10983
		public const int persistance = 3;

		// Token: 0x04002AE8 RID: 10984
		public const int storage = 0;

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x0600301A RID: 12314 RVA: 0x00025005 File Offset: 0x00023205
		public static string printable
		{
			get
			{
				return string.Concat(new object[]
				{
					2153,
					".",
					176,
					".",
					1
				});
			}
		}
	}
}
