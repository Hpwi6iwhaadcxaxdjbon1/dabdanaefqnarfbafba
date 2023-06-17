using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020007E1 RID: 2017
	[Serializable]
	public class PredicationPreset
	{
		// Token: 0x040027F4 RID: 10228
		[Min(0.0001f)]
		public float Threshold = 0.01f;

		// Token: 0x040027F5 RID: 10229
		[Range(1f, 5f)]
		public float Scale = 2f;

		// Token: 0x040027F6 RID: 10230
		[Range(0f, 1f)]
		public float Strength = 0.4f;
	}
}
