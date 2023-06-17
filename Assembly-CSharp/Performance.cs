using System;
using Network;
using UnityEngine;

// Token: 0x02000263 RID: 611
public class Performance : SingletonComponent<Performance>
{
	// Token: 0x04000E7C RID: 3708
	public static Performance.Tick current;

	// Token: 0x04000E7D RID: 3709
	public static Performance.Tick report;

	// Token: 0x04000E7E RID: 3710
	private static long cycles = 0L;

	// Token: 0x04000E7F RID: 3711
	private static int[] frameRateHistory = new int[60];

	// Token: 0x04000E80 RID: 3712
	private static float[] frameTimeHistory = new float[60];

	// Token: 0x04000E81 RID: 3713
	private int frames;

	// Token: 0x04000E82 RID: 3714
	private float time;

	// Token: 0x060011D7 RID: 4567 RVA: 0x00075E44 File Offset: 0x00074044
	private void Update()
	{
		using (TimeWarning.New("FPSTimer", 0.1f))
		{
			this.FPSTimer();
		}
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x00075E84 File Offset: 0x00074084
	private void FPSTimer()
	{
		this.frames++;
		this.time += Time.unscaledDeltaTime;
		if (this.time < 1f)
		{
			return;
		}
		Performance.current.frameRate = this.frames;
		Performance.current.frameTime = this.time / (float)this.frames * 1000f;
		checked
		{
			Performance.frameRateHistory[(int)((IntPtr)(Performance.cycles % unchecked((long)Performance.frameRateHistory.Length)))] = Performance.current.frameRate;
			Performance.frameTimeHistory[(int)((IntPtr)(Performance.cycles % unchecked((long)Performance.frameTimeHistory.Length)))] = Performance.current.frameTime;
			Performance.current.frameRateAverage = this.AverageFrameRate();
			Performance.current.frameTimeAverage = this.AverageFrameTime();
		}
		Performance.current.memoryUsageSystem = (long)SystemInfoEx.systemMemoryUsed;
		Performance.current.memoryAllocations = GC.GetTotalMemory(false) / 1048576L;
		Performance.current.memoryCollections = (long)GC.CollectionCount(0);
		Performance.current.loadBalancerTasks = (long)LoadBalancer.Count();
		Performance.current.invokeHandlerTasks = (long)InvokeHandler.Count();
		Performance.current.ping = (Net.cl.IsConnected() ? Net.cl.GetLastPing() : 0);
		this.frames = 0;
		this.time = 0f;
		Performance.cycles += 1L;
		if (DeveloperTools.isOpen)
		{
			return;
		}
		if (MainMenuSystem.isOpen)
		{
			return;
		}
		Performance.report = Performance.current;
	}

	// Token: 0x060011D9 RID: 4569 RVA: 0x00076000 File Offset: 0x00074200
	private float AverageFrameRate()
	{
		float num = 0f;
		for (int i = 0; i < Performance.frameRateHistory.Length; i++)
		{
			num += (float)Performance.frameRateHistory[i];
		}
		return num / (float)Performance.frameRateHistory.Length;
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x0007603C File Offset: 0x0007423C
	private float AverageFrameTime()
	{
		float num = 0f;
		for (int i = 0; i < Performance.frameTimeHistory.Length; i++)
		{
			num += Performance.frameTimeHistory[i];
		}
		return num / (float)Performance.frameTimeHistory.Length;
	}

	// Token: 0x02000264 RID: 612
	public struct Tick
	{
		// Token: 0x04000E83 RID: 3715
		public int frameRate;

		// Token: 0x04000E84 RID: 3716
		public float frameTime;

		// Token: 0x04000E85 RID: 3717
		public float frameRateAverage;

		// Token: 0x04000E86 RID: 3718
		public float frameTimeAverage;

		// Token: 0x04000E87 RID: 3719
		public long memoryUsageSystem;

		// Token: 0x04000E88 RID: 3720
		public long memoryAllocations;

		// Token: 0x04000E89 RID: 3721
		public long memoryCollections;

		// Token: 0x04000E8A RID: 3722
		public long loadBalancerTasks;

		// Token: 0x04000E8B RID: 3723
		public long invokeHandlerTasks;

		// Token: 0x04000E8C RID: 3724
		public int ping;
	}
}
