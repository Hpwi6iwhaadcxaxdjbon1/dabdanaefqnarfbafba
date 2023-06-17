using System;
using System.Diagnostics;

// Token: 0x020002B3 RID: 691
public class EntityDebug : EntityComponent<BaseEntity>
{
	// Token: 0x04000FA8 RID: 4008
	internal Stopwatch stopwatch = Stopwatch.StartNew();

	// Token: 0x06001355 RID: 4949 RVA: 0x0007B4D0 File Offset: 0x000796D0
	private void Update()
	{
		if (!base.baseEntity.IsValid() || !base.baseEntity.IsDebugging())
		{
			base.enabled = false;
			return;
		}
		if (this.stopwatch.Elapsed.TotalSeconds < 0.5)
		{
			return;
		}
		if (base.baseEntity.isClient)
		{
			base.baseEntity.DebugClient(1, (float)this.stopwatch.Elapsed.TotalSeconds);
		}
		bool isServer = base.baseEntity.isServer;
		this.stopwatch.Reset();
		this.stopwatch.Start();
	}
}
