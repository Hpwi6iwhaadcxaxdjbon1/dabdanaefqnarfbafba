using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020007E0 RID: 2016
	public sealed class MinAttribute : PropertyAttribute
	{
		// Token: 0x040027F3 RID: 10227
		public readonly float min;

		// Token: 0x06002C21 RID: 11297 RVA: 0x000225BF File Offset: 0x000207BF
		public MinAttribute(float min)
		{
			this.min = min;
		}
	}
}
