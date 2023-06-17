using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008C7 RID: 2247
	[DefaultExecutionOrder(-102)]
	public class AiManagedAgent : FacepunchBehaviour, IServerComponent
	{
		// Token: 0x04002B29 RID: 11049
		[Tooltip("TODO: Replace with actual agent type id on the NavMeshAgent when we upgrade to 5.6.1 or above.")]
		public int AgentTypeIndex;
	}
}
