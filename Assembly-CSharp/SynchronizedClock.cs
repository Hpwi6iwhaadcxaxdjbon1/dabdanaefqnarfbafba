using System;
using System.Collections.Generic;

// Token: 0x0200075A RID: 1882
public class SynchronizedClock
{
	// Token: 0x04002431 RID: 9265
	public List<SynchronizedClock.TimedEvent> events = new List<SynchronizedClock.TimedEvent>();

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x06002919 RID: 10521 RVA: 0x000D2214 File Offset: 0x000D0414
	private static long Ticks
	{
		get
		{
			if (!TOD_Sky.Instance)
			{
				return DateTime.Now.Ticks;
			}
			return TOD_Sky.Instance.Cycle.Ticks;
		}
	}

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x0600291A RID: 10522 RVA: 0x0001FF67 File Offset: 0x0001E167
	private static float DayLengthInMinutes
	{
		get
		{
			if (!TOD_Sky.Instance)
			{
				return 30f;
			}
			return TOD_Sky.Instance.Components.Time.DayLengthInMinutes;
		}
	}

	// Token: 0x0600291B RID: 10523 RVA: 0x000D224C File Offset: 0x000D044C
	public void Add(float delta, float variance, Action<uint> action)
	{
		SynchronizedClock.TimedEvent timedEvent = default(SynchronizedClock.TimedEvent);
		timedEvent.ticks = SynchronizedClock.Ticks;
		timedEvent.delta = delta;
		timedEvent.variance = variance;
		timedEvent.action = action;
		this.events.Add(timedEvent);
	}

	// Token: 0x0600291C RID: 10524 RVA: 0x000D2294 File Offset: 0x000D0494
	public void Tick()
	{
		double num = (double)10000000L;
		double num2 = 1440.0 / (double)SynchronizedClock.DayLengthInMinutes;
		double num3 = num * num2;
		for (int i = 0; i < this.events.Count; i++)
		{
			SynchronizedClock.TimedEvent timedEvent = this.events[i];
			long ticks = timedEvent.ticks;
			long ticks2 = SynchronizedClock.Ticks;
			long num4 = (long)((double)timedEvent.delta * num3);
			long num5 = ticks / num4 * num4;
			uint num6 = (uint)(num5 % (long)((ulong)-1));
			SeedRandom.Wanghash(ref num6);
			long num7 = (long)((double)SeedRandom.Range(ref num6, -timedEvent.variance, timedEvent.variance) * num3);
			long num8 = num5 + num4 + num7;
			if (ticks < num8 && ticks2 >= num8)
			{
				timedEvent.action.Invoke(num6);
				timedEvent.ticks = ticks2;
			}
			else if (ticks2 > ticks || ticks2 < num5)
			{
				timedEvent.ticks = ticks2;
			}
			this.events[i] = timedEvent;
		}
	}

	// Token: 0x0200075B RID: 1883
	public struct TimedEvent
	{
		// Token: 0x04002432 RID: 9266
		public long ticks;

		// Token: 0x04002433 RID: 9267
		public float delta;

		// Token: 0x04002434 RID: 9268
		public float variance;

		// Token: 0x04002435 RID: 9269
		public Action<uint> action;
	}
}
