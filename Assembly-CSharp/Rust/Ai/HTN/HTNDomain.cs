using System;
using UnityEngine;

namespace Rust.Ai.HTN
{
	// Token: 0x020008DC RID: 2268
	public abstract class HTNDomain : MonoBehaviour
	{
		// Token: 0x020008DD RID: 2269
		public enum MovementRule
		{
			// Token: 0x04002B9A RID: 11162
			NeverMove,
			// Token: 0x04002B9B RID: 11163
			RestrainedMove,
			// Token: 0x04002B9C RID: 11164
			FreeMove
		}
	}
}
