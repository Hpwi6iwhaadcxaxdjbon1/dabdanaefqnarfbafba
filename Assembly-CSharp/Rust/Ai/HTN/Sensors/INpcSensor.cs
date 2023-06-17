using System;

namespace Rust.Ai.HTN.Sensors
{
	// Token: 0x020008E0 RID: 2272
	public interface INpcSensor
	{
		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x060030A3 RID: 12451
		// (set) Token: 0x060030A4 RID: 12452
		float TickFrequency { get; set; }

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x060030A5 RID: 12453
		// (set) Token: 0x060030A6 RID: 12454
		float LastTickTime { get; set; }

		// Token: 0x060030A7 RID: 12455
		void Tick(IHTNAgent npc, float deltaTime, float time);
	}
}
