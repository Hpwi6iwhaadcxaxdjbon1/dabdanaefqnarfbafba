using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000749 RID: 1865
public class LocalClock
{
	// Token: 0x040023FA RID: 9210
	public List<LocalClock.TimedEvent> events = new List<LocalClock.TimedEvent>();

	// Token: 0x0600287C RID: 10364 RVA: 0x000D05C0 File Offset: 0x000CE7C0
	public void Add(float delta, float variance, Action action)
	{
		LocalClock.TimedEvent timedEvent = default(LocalClock.TimedEvent);
		timedEvent.time = Time.time + delta + Random.Range(-variance, variance);
		timedEvent.delta = delta;
		timedEvent.variance = variance;
		timedEvent.action = action;
		this.events.Add(timedEvent);
	}

	// Token: 0x0600287D RID: 10365 RVA: 0x000D0610 File Offset: 0x000CE810
	public void Tick()
	{
		for (int i = 0; i < this.events.Count; i++)
		{
			LocalClock.TimedEvent timedEvent = this.events[i];
			if (Time.time > timedEvent.time)
			{
				float delta = timedEvent.delta;
				float variance = timedEvent.variance;
				timedEvent.action.Invoke();
				timedEvent.time = Time.time + delta + Random.Range(-variance, variance);
				this.events[i] = timedEvent;
			}
		}
	}

	// Token: 0x0200074A RID: 1866
	public struct TimedEvent
	{
		// Token: 0x040023FB RID: 9211
		public float time;

		// Token: 0x040023FC RID: 9212
		public float delta;

		// Token: 0x040023FD RID: 9213
		public float variance;

		// Token: 0x040023FE RID: 9214
		public Action action;
	}
}
