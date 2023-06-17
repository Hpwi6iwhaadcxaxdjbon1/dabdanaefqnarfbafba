using System;

namespace Rust.Ai.HTN.Reasoning
{
	// Token: 0x020008E1 RID: 2273
	public interface INpcReasoner
	{
		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x060030A8 RID: 12456
		// (set) Token: 0x060030A9 RID: 12457
		float TickFrequency { get; set; }

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x060030AA RID: 12458
		// (set) Token: 0x060030AB RID: 12459
		float LastTickTime { get; set; }

		// Token: 0x060030AC RID: 12460
		void Tick(IHTNAgent npc, float deltaTime, float time);
	}
}
