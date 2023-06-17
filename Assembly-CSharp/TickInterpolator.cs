using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200075C RID: 1884
public class TickInterpolator
{
	// Token: 0x04002436 RID: 9270
	private List<TickInterpolator.Segment> points = new List<TickInterpolator.Segment>();

	// Token: 0x04002437 RID: 9271
	private int index;

	// Token: 0x04002438 RID: 9272
	public float Length;

	// Token: 0x04002439 RID: 9273
	public Vector3 CurrentPoint;

	// Token: 0x0400243A RID: 9274
	public Vector3 StartPoint;

	// Token: 0x0400243B RID: 9275
	public Vector3 EndPoint;

	// Token: 0x0600291E RID: 10526 RVA: 0x0001FFA2 File Offset: 0x0001E1A2
	public void Reset()
	{
		this.index = 0;
		this.CurrentPoint = this.StartPoint;
	}

	// Token: 0x0600291F RID: 10527 RVA: 0x000D2384 File Offset: 0x000D0584
	public void Reset(Vector3 point)
	{
		this.points.Clear();
		this.index = 0;
		this.Length = 0f;
		this.EndPoint = point;
		this.StartPoint = point;
		this.CurrentPoint = point;
	}

	// Token: 0x06002920 RID: 10528 RVA: 0x000D23C8 File Offset: 0x000D05C8
	public void AddPoint(Vector3 point)
	{
		TickInterpolator.Segment segment = new TickInterpolator.Segment(this.EndPoint, point);
		this.points.Add(segment);
		this.Length += segment.length;
		this.EndPoint = segment.point;
	}

	// Token: 0x06002921 RID: 10529 RVA: 0x000D2410 File Offset: 0x000D0610
	public bool MoveNext(float distance)
	{
		float num = 0f;
		while (num < distance && this.index < this.points.Count)
		{
			TickInterpolator.Segment segment = this.points[this.index];
			this.CurrentPoint = segment.point;
			num += segment.length;
			this.index++;
		}
		return num > 0f;
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x0001FFB7 File Offset: 0x0001E1B7
	public bool HasNext()
	{
		return this.index < this.points.Count;
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x000D247C File Offset: 0x000D067C
	public void TransformEntries(Matrix4x4 matrix)
	{
		for (int i = 0; i < this.points.Count; i++)
		{
			TickInterpolator.Segment segment = this.points[i];
			segment.point = matrix.MultiplyPoint3x4(segment.point);
			this.points[i] = segment;
		}
		this.CurrentPoint = matrix.MultiplyPoint3x4(this.CurrentPoint);
		this.StartPoint = matrix.MultiplyPoint3x4(this.StartPoint);
		this.EndPoint = matrix.MultiplyPoint3x4(this.EndPoint);
	}

	// Token: 0x0200075D RID: 1885
	private struct Segment
	{
		// Token: 0x0400243C RID: 9276
		public Vector3 point;

		// Token: 0x0400243D RID: 9277
		public float length;

		// Token: 0x06002925 RID: 10533 RVA: 0x0001FFDF File Offset: 0x0001E1DF
		public Segment(Vector3 a, Vector3 b)
		{
			this.point = b;
			this.length = Vector3.Distance(a, b);
		}
	}
}
