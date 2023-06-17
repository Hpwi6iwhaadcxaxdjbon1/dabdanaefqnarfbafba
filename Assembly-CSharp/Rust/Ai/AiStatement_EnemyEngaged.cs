using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008BF RID: 2239
	public struct AiStatement_EnemyEngaged : IAiStatement
	{
		// Token: 0x04002B02 RID: 11010
		public BasePlayer Enemy;

		// Token: 0x04002B03 RID: 11011
		public float Score;

		// Token: 0x04002B04 RID: 11012
		public Vector3? LastKnownPosition;
	}
}
