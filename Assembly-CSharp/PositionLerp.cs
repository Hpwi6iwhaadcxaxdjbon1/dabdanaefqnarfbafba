using System;
using UnityEngine;

// Token: 0x0200021A RID: 538
public class PositionLerp : ListComponent<PositionLerp>
{
	// Token: 0x04000D3A RID: 3386
	public static bool DebugLog;

	// Token: 0x04000D3B RID: 3387
	public static bool DebugDraw;

	// Token: 0x04000D3C RID: 3388
	private Action idleDisable;

	// Token: 0x04000D3D RID: 3389
	private TransformInterpolator interpolator = new TransformInterpolator();

	// Token: 0x04000D3E RID: 3390
	private ILerpTarget target;

	// Token: 0x04000D3F RID: 3391
	private float timeOffset0 = float.MaxValue;

	// Token: 0x04000D40 RID: 3392
	private float timeOffset1 = float.MaxValue;

	// Token: 0x04000D41 RID: 3393
	private float timeOffset2 = float.MaxValue;

	// Token: 0x04000D42 RID: 3394
	private float timeOffset3 = float.MaxValue;

	// Token: 0x04000D43 RID: 3395
	private float lastClientTime;

	// Token: 0x04000D44 RID: 3396
	private float lastServerTime;

	// Token: 0x04000D45 RID: 3397
	private float extrapolatedTime;

	// Token: 0x06001070 RID: 4208 RVA: 0x0000E7C8 File Offset: 0x0000C9C8
	public void Initialize(ILerpTarget target)
	{
		this.target = target;
	}

	// Token: 0x06001071 RID: 4209 RVA: 0x0006E7FC File Offset: 0x0006C9FC
	public void Snapshot(Vector3 position, Quaternion rotation, float serverTime)
	{
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		float num = interpolationDelay + interpolationSmoothing + 1f;
		float num2 = Time.time;
		this.timeOffset0 = this.timeOffset1;
		this.timeOffset1 = this.timeOffset2;
		this.timeOffset2 = this.timeOffset3;
		this.timeOffset3 = num2 - serverTime;
		float num3 = Mathx.Min(this.timeOffset0, this.timeOffset1, this.timeOffset2, this.timeOffset3);
		num2 = serverTime + num3;
		if (PositionLerp.DebugLog && this.interpolator.list.Count > 0 && serverTime < this.lastServerTime)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				this.target.ToString(),
				" adding tick from the past: server time ",
				serverTime,
				" < ",
				this.lastServerTime
			}));
		}
		else if (PositionLerp.DebugLog && this.interpolator.list.Count > 0 && num2 < this.lastClientTime)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				this.target.ToString(),
				" adding tick from the past: client time ",
				num2,
				" < ",
				this.lastClientTime
			}));
		}
		else
		{
			this.lastClientTime = num2;
			this.lastServerTime = serverTime;
			this.interpolator.Add(new TransformInterpolator.Entry
			{
				time = num2,
				pos = position,
				rot = rotation
			});
		}
		this.interpolator.Cull(num2 - num);
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x0000E7D1 File Offset: 0x0000C9D1
	public void SnapTo(Vector3 position, Quaternion rotation, float serverTime)
	{
		this.interpolator.Clear();
		this.Snapshot(position, rotation, serverTime);
		this.target.SetNetworkPosition(position);
		this.target.SetNetworkRotation(rotation);
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x0006E9A0 File Offset: 0x0006CBA0
	public void SnapToEnd()
	{
		float interpolationDelay = this.target.GetInterpolationDelay();
		TransformInterpolator.Segment segment = this.interpolator.Query(Time.time, interpolationDelay, 0f, 0f);
		this.target.SetNetworkPosition(segment.tick.pos);
		this.target.SetNetworkRotation(segment.tick.rot);
		this.interpolator.Clear();
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x0006EA0C File Offset: 0x0006CC0C
	protected void DoCycle()
	{
		if (this.target == null)
		{
			return;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		TransformInterpolator.Segment segment = this.interpolator.Query(Time.time, interpolationDelay, extrapolationTime, interpolationSmoothing);
		if (segment.next.time >= this.interpolator.last.time)
		{
			this.extrapolatedTime = Mathf.Min(this.extrapolatedTime + Time.deltaTime, extrapolationTime);
		}
		else
		{
			this.extrapolatedTime = Mathf.Max(this.extrapolatedTime - Time.deltaTime, 0f);
		}
		if (this.extrapolatedTime > 0f && extrapolationTime > 0f && interpolationSmoothing > 0f)
		{
			float t = Time.deltaTime / (this.extrapolatedTime / extrapolationTime * interpolationSmoothing);
			segment.tick.pos = Vector3.Lerp(this.target.GetNetworkPosition(), segment.tick.pos, t);
			segment.tick.rot = Quaternion.Slerp(this.target.GetNetworkRotation(), segment.tick.rot, t);
		}
		this.target.SetNetworkPosition(segment.tick.pos);
		this.target.SetNetworkRotation(segment.tick.rot);
		if (PositionLerp.DebugDraw)
		{
			this.target.DrawInterpolationState(segment, this.interpolator.list);
		}
		if (Time.time - this.lastClientTime > 10f)
		{
			if (this.idleDisable == null)
			{
				this.idleDisable = new Action(this.IdleDisable);
			}
			InvokeHandler.Invoke(this, this.idleDisable, 0f);
		}
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x0000E7FF File Offset: 0x0000C9FF
	private void IdleDisable()
	{
		base.enabled = false;
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x0006EBB8 File Offset: 0x0006CDB8
	public void TransformEntries(Matrix4x4 matrix)
	{
		Quaternion rotation = matrix.rotation;
		for (int i = 0; i < this.interpolator.list.Count; i++)
		{
			TransformInterpolator.Entry entry = this.interpolator.list[i];
			entry.pos = matrix.MultiplyPoint3x4(entry.pos);
			entry.rot = rotation * entry.rot;
			this.interpolator.list[i] = entry;
		}
		this.interpolator.last.pos = matrix.MultiplyPoint3x4(this.interpolator.last.pos);
		this.interpolator.last.rot = rotation * this.interpolator.last.rot;
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x0006EC80 File Offset: 0x0006CE80
	public Quaternion GetEstimatedAngularVelocity()
	{
		if (this.target == null)
		{
			return Quaternion.identity;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		TransformInterpolator.Segment segment = this.interpolator.Query(Time.time, interpolationDelay, extrapolationTime, interpolationSmoothing);
		TransformInterpolator.Entry next = segment.next;
		TransformInterpolator.Entry prev = segment.prev;
		if (next.time == prev.time)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler((prev.rot.eulerAngles - next.rot.eulerAngles) / (prev.time - next.time));
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x0006ED2C File Offset: 0x0006CF2C
	public Vector3 GetEstimatedVelocity()
	{
		if (this.target == null)
		{
			return Vector3.zero;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		TransformInterpolator.Segment segment = this.interpolator.Query(Time.time, interpolationDelay, extrapolationTime, interpolationSmoothing);
		TransformInterpolator.Entry next = segment.next;
		TransformInterpolator.Entry prev = segment.prev;
		if (next.time == prev.time)
		{
			return Vector3.zero;
		}
		return (prev.pos - next.pos) / (prev.time - next.time);
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x0006EDC8 File Offset: 0x0006CFC8
	public static void Cycle()
	{
		PositionLerp[] buffer = ListComponent<PositionLerp>.InstanceList.Values.Buffer;
		int count = ListComponent<PositionLerp>.InstanceList.Count;
		for (int i = 0; i < count; i++)
		{
			buffer[i].DoCycle();
		}
	}
}
