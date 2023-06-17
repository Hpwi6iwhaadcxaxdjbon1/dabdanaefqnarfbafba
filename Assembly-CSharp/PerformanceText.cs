using System;
using System.Diagnostics;
using System.Text;
using ConVar;
using Rust;
using Rust.Workshop;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006AB RID: 1707
public class PerformanceText : MonoBehaviour
{
	// Token: 0x0400220D RID: 8717
	public Text text;

	// Token: 0x0400220E RID: 8718
	private StringBuilder sb = new StringBuilder();

	// Token: 0x0400220F RID: 8719
	private Stopwatch fpsTimer = Stopwatch.StartNew();

	// Token: 0x0600260B RID: 9739 RVA: 0x000C83B0 File Offset: 0x000C65B0
	protected void Update()
	{
		if (Global.perf <= 0)
		{
			if (this.text.enabled)
			{
				this.text.enabled = false;
			}
			return;
		}
		if (this.fpsTimer.Elapsed.TotalSeconds < 0.20000000298023224)
		{
			return;
		}
		if (!this.text.enabled)
		{
			this.text.enabled = true;
		}
		this.fpsTimer.Reset();
		this.fpsTimer.Start();
		this.sb.Clear();
		if (Global.perf > 0)
		{
			this.sb.Append(Performance.current.frameRate).Append(" FPS");
		}
		if (Global.perf > 1)
		{
			this.sb.Append(" / ").Append(Performance.current.frameTime.ToString("0.0")).Append("ms");
		}
		if (Global.perf > 2)
		{
			this.sb.AppendLine().Append(Performance.current.memoryAllocations).Append(" MB");
		}
		if (Global.perf > 3)
		{
			this.sb.Append(" / ").Append(Performance.current.memoryCollections).Append(" GC");
		}
		if (Global.perf > 4)
		{
			this.sb.AppendLine().Append(Performance.current.memoryUsageSystem).Append(" RAM");
		}
		if (Global.perf > 5)
		{
			this.sb.Append(" / ").Append(Performance.current.ping).Append(" PING");
		}
		if (Global.perf > 6)
		{
			this.sb.AppendLine().Append(Performance.current.loadBalancerTasks).Append(" TASKS");
		}
		if (Global.perf > 7)
		{
			this.sb.Append(" / ").Append(WorkshopSkin.QueuedCount).Append(" SKINS");
		}
		if (Global.perf > 8)
		{
			this.sb.Append(" / ").Append(Performance.current.invokeHandlerTasks).Append(" INVOKES");
		}
		if (Global.perf > 9)
		{
			this.sb.AppendLine().Append(Rust.GC.Enabled ? "GC ENABLED" : "GC DISABLED");
		}
		this.text.text = this.sb.ToString();
	}
}
