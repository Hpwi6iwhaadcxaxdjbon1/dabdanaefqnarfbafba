using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020007E2 RID: 2018
	[Serializable]
	public class Preset
	{
		// Token: 0x040027F7 RID: 10231
		public bool DiagDetection = true;

		// Token: 0x040027F8 RID: 10232
		public bool CornerDetection = true;

		// Token: 0x040027F9 RID: 10233
		[Range(0f, 0.5f)]
		public float Threshold = 0.1f;

		// Token: 0x040027FA RID: 10234
		[Min(0.0001f)]
		public float DepthThreshold = 0.01f;

		// Token: 0x040027FB RID: 10235
		[Range(0f, 112f)]
		public int MaxSearchSteps = 16;

		// Token: 0x040027FC RID: 10236
		[Range(0f, 20f)]
		public int MaxSearchStepsDiag = 8;

		// Token: 0x040027FD RID: 10237
		[Range(0f, 100f)]
		public int CornerRounding = 25;

		// Token: 0x040027FE RID: 10238
		[Min(0f)]
		public float LocalContrastAdaptationFactor = 2f;
	}
}
