using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008BE RID: 2238
	public struct AiStatement_EnemySeen : IAiStatement
	{
		// Token: 0x04002AFF RID: 11007
		public BasePlayer Enemy;

		// Token: 0x04002B00 RID: 11008
		public float Score;

		// Token: 0x04002B01 RID: 11009
		public Vector3? LastKnownPosition;
	}
}
