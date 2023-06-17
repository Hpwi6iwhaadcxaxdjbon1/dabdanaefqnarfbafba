using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021B RID: 539
public class TransformInterpolator
{
	// Token: 0x04000D46 RID: 3398
	public List<TransformInterpolator.Entry> list = new List<TransformInterpolator.Entry>(32);

	// Token: 0x04000D47 RID: 3399
	public TransformInterpolator.Entry last;

	// Token: 0x0600107C RID: 4220 RVA: 0x0000E847 File Offset: 0x0000CA47
	public void Add(TransformInterpolator.Entry tick)
	{
		this.last = tick;
		this.list.Add(tick);
	}

	// Token: 0x0600107D RID: 4221 RVA: 0x0006EE04 File Offset: 0x0006D004
	public void Cull(float beforeTime)
	{
		for (int i = 0; i < this.list.Count; i++)
		{
			if (this.list[i].time < beforeTime)
			{
				this.list.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x0000E85C File Offset: 0x0000CA5C
	public void Clear()
	{
		this.list.Clear();
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x0006EE4C File Offset: 0x0006D04C
	public TransformInterpolator.Segment Query(float time, float interpolation, float extrapolation, float smoothing)
	{
		TransformInterpolator.Segment result = default(TransformInterpolator.Segment);
		if (this.list.Count == 0)
		{
			result.prev = this.last;
			result.next = this.last;
			result.tick = this.last;
			return result;
		}
		float num = time - interpolation - smoothing * 0.5f;
		float num2 = Mathf.Min(time - interpolation, this.last.time);
		float num3 = num2 - smoothing;
		TransformInterpolator.Entry entry = this.list[0];
		TransformInterpolator.Entry entry2 = this.last;
		TransformInterpolator.Entry entry3 = this.list[0];
		TransformInterpolator.Entry entry4 = this.last;
		foreach (TransformInterpolator.Entry entry5 in this.list)
		{
			if (entry5.time < num3)
			{
				entry = entry5;
			}
			else if (entry2.time >= entry5.time)
			{
				entry2 = entry5;
			}
			if (entry5.time < num2)
			{
				entry3 = entry5;
			}
			else if (entry4.time >= entry5.time)
			{
				entry4 = entry5;
			}
		}
		TransformInterpolator.Entry entry6 = default(TransformInterpolator.Entry);
		if (entry2.time - entry.time < Mathf.Epsilon)
		{
			entry6.time = num3;
			entry6.pos = entry2.pos;
			entry6.rot = entry2.rot;
		}
		else
		{
			float t = (num3 - entry.time) / (entry2.time - entry.time);
			entry6.time = num3;
			entry6.pos = Vector3.LerpUnclamped(entry.pos, entry2.pos, t);
			entry6.rot = Quaternion.SlerpUnclamped(entry.rot, entry2.rot, t);
		}
		result.prev = entry6;
		TransformInterpolator.Entry entry7 = default(TransformInterpolator.Entry);
		if (entry4.time - entry3.time < Mathf.Epsilon)
		{
			entry7.time = num2;
			entry7.pos = entry4.pos;
			entry7.rot = entry4.rot;
		}
		else
		{
			float t2 = (num2 - entry3.time) / (entry4.time - entry3.time);
			entry7.time = num2;
			entry7.pos = Vector3.LerpUnclamped(entry3.pos, entry4.pos, t2);
			entry7.rot = Quaternion.SlerpUnclamped(entry3.rot, entry4.rot, t2);
		}
		result.next = entry7;
		if (entry7.time - entry6.time < Mathf.Epsilon)
		{
			result.prev = entry7;
			result.tick = entry7;
			return result;
		}
		if (num - entry7.time > extrapolation)
		{
			result.prev = entry7;
			result.tick = entry7;
			return result;
		}
		TransformInterpolator.Entry tick = default(TransformInterpolator.Entry);
		float t3 = Mathf.Min(num - entry6.time, entry7.time + extrapolation - entry6.time) / (entry7.time - entry6.time);
		tick.time = num;
		tick.pos = Vector3.LerpUnclamped(entry6.pos, entry7.pos, t3);
		tick.rot = Quaternion.SlerpUnclamped(entry6.rot, entry7.rot, t3);
		result.tick = tick;
		return result;
	}

	// Token: 0x0200021C RID: 540
	public struct Segment
	{
		// Token: 0x04000D48 RID: 3400
		public TransformInterpolator.Entry tick;

		// Token: 0x04000D49 RID: 3401
		public TransformInterpolator.Entry prev;

		// Token: 0x04000D4A RID: 3402
		public TransformInterpolator.Entry next;
	}

	// Token: 0x0200021D RID: 541
	public struct Entry
	{
		// Token: 0x04000D4B RID: 3403
		public float time;

		// Token: 0x04000D4C RID: 3404
		public Vector3 pos;

		// Token: 0x04000D4D RID: 3405
		public Quaternion rot;
	}
}
