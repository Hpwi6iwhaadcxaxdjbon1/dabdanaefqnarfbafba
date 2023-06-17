using System;

namespace VLB
{
	// Token: 0x020007D2 RID: 2002
	public enum RenderQueue
	{
		// Token: 0x0400279A RID: 10138
		Custom,
		// Token: 0x0400279B RID: 10139
		Background = 1000,
		// Token: 0x0400279C RID: 10140
		Geometry = 2000,
		// Token: 0x0400279D RID: 10141
		AlphaTest = 2450,
		// Token: 0x0400279E RID: 10142
		GeometryLast = 2500,
		// Token: 0x0400279F RID: 10143
		Transparent = 3000,
		// Token: 0x040027A0 RID: 10144
		Overlay = 4000
	}
}
