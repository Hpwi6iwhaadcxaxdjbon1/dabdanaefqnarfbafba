using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x020008BA RID: 2234
	public struct AiAnswer_ShareEnemyTarget : IAiAnswer
	{
		// Token: 0x04002AFD RID: 11005
		public BasePlayer PlayerTarget;

		// Token: 0x04002AFE RID: 11006
		public Vector3? LastKnownPosition;

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x0600302F RID: 12335 RVA: 0x00025137 File Offset: 0x00023337
		// (set) Token: 0x06003030 RID: 12336 RVA: 0x0002513F File Offset: 0x0002333F
		public NPCPlayerApex Source { get; set; }
	}
}
