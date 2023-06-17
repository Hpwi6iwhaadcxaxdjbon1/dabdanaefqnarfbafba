using System;
using UnityEngine;

// Token: 0x02000755 RID: 1877
public class PathInterpolator
{
	// Token: 0x04002423 RID: 9251
	public Vector3[] Points;

	// Token: 0x04002424 RID: 9252
	public Vector3[] Tangents;

	// Token: 0x04002429 RID: 9257
	private bool initialized;

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x060028E9 RID: 10473 RVA: 0x0001FD0B File Offset: 0x0001DF0B
	// (set) Token: 0x060028EA RID: 10474 RVA: 0x0001FD13 File Offset: 0x0001DF13
	public int MinIndex { get; set; }

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x060028EB RID: 10475 RVA: 0x0001FD1C File Offset: 0x0001DF1C
	// (set) Token: 0x060028EC RID: 10476 RVA: 0x0001FD24 File Offset: 0x0001DF24
	public int MaxIndex { get; set; }

	// Token: 0x1700029F RID: 671
	// (get) Token: 0x060028ED RID: 10477 RVA: 0x0001FD2D File Offset: 0x0001DF2D
	// (set) Token: 0x060028EE RID: 10478 RVA: 0x0001FD35 File Offset: 0x0001DF35
	public float Length { get; private set; }

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x060028EF RID: 10479 RVA: 0x0001FD3E File Offset: 0x0001DF3E
	// (set) Token: 0x060028F0 RID: 10480 RVA: 0x0001FD46 File Offset: 0x0001DF46
	public float StepSize { get; private set; }

	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x060028F1 RID: 10481 RVA: 0x0000508F File Offset: 0x0000328F
	public int DefaultMinIndex
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x060028F2 RID: 10482 RVA: 0x0001FD4F File Offset: 0x0001DF4F
	public int DefaultMaxIndex
	{
		get
		{
			return this.Points.Length - 1;
		}
	}

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x060028F3 RID: 10483 RVA: 0x0001FD5B File Offset: 0x0001DF5B
	public float StartOffset
	{
		get
		{
			return this.Length * (float)(this.MinIndex - this.DefaultMinIndex) / (float)(this.DefaultMaxIndex - this.DefaultMinIndex);
		}
	}

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x060028F4 RID: 10484 RVA: 0x0001FD81 File Offset: 0x0001DF81
	public float EndOffset
	{
		get
		{
			return this.Length * (float)(this.DefaultMaxIndex - this.MaxIndex) / (float)(this.DefaultMaxIndex - this.DefaultMinIndex);
		}
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x0001FDA7 File Offset: 0x0001DFA7
	public PathInterpolator(Vector3[] points)
	{
		if (points.Length < 2)
		{
			throw new ArgumentException("Point list too short.");
		}
		this.Points = points;
		this.MinIndex = this.DefaultMinIndex;
		this.MaxIndex = this.DefaultMaxIndex;
	}

	// Token: 0x060028F6 RID: 10486 RVA: 0x000D1854 File Offset: 0x000CFA54
	public void RecalculateTangents()
	{
		if (this.Tangents == null || this.Tangents.Length != this.Points.Length)
		{
			this.Tangents = new Vector3[this.Points.Length];
		}
		float num = 0f;
		for (int i = 0; i < this.Tangents.Length; i++)
		{
			Vector3 b = this.Points[Mathf.Max(i - 1, 0)];
			Vector3 a = this.Points[Mathf.Min(i + 1, this.Tangents.Length - 1)] - b;
			float magnitude = a.magnitude;
			num += magnitude;
			this.Tangents[i] = a / magnitude;
		}
		this.Length = num;
		this.StepSize = num / (float)this.Points.Length;
		this.initialized = true;
	}

	// Token: 0x060028F7 RID: 10487 RVA: 0x000D1924 File Offset: 0x000CFB24
	public void Resample(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		Vector3[] array = new Vector3[Mathf.RoundToInt(this.Length / distance)];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.GetPointCubicHermite((float)i * distance);
		}
		this.Points = array;
		this.initialized = false;
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x000D1984 File Offset: 0x000CFB84
	public void Smoothen(int iterations = 1)
	{
		float d = 0.25f;
		for (int i = 0; i < iterations; i++)
		{
			Vector3 a = this.Points[0];
			for (int j = 1; j < this.Points.Length - 1; j++)
			{
				Vector3 vector = this.Points[j];
				Vector3 b = this.Points[j + 1];
				this.Points[j] = (a + vector + vector + b) * d;
				a = vector;
			}
		}
		this.initialized = false;
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x0001FDDF File Offset: 0x0001DFDF
	public Vector3 GetStartPoint()
	{
		return this.Points[this.MinIndex];
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x0001FDF2 File Offset: 0x0001DFF2
	public Vector3 GetEndPoint()
	{
		return this.Points[this.MaxIndex];
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x0001FE05 File Offset: 0x0001E005
	public Vector3 GetStartTangent()
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return this.Tangents[this.MinIndex];
	}

	// Token: 0x060028FC RID: 10492 RVA: 0x0001FE2B File Offset: 0x0001E02B
	public Vector3 GetEndTangent()
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return this.Tangents[this.MaxIndex];
	}

	// Token: 0x060028FD RID: 10493 RVA: 0x000D1A18 File Offset: 0x000CFC18
	public Vector3 GetPoint(float distance)
	{
		float num = distance / this.Length * (float)this.Points.Length;
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartPoint();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndPoint();
		}
		Vector3 a = this.Points[num2];
		Vector3 b = this.Points[num2 + 1];
		float t = num - (float)num2;
		return Vector3.Lerp(a, b, t);
	}

	// Token: 0x060028FE RID: 10494 RVA: 0x000D1A84 File Offset: 0x000CFC84
	public Vector3 GetTangent(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		float num = distance / this.Length * (float)this.Tangents.Length;
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartTangent();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndTangent();
		}
		Vector3 a = this.Tangents[num2];
		Vector3 b = this.Tangents[num2 + 1];
		float t = num - (float)num2;
		return Vector3.Lerp(a, b, t);
	}

	// Token: 0x060028FF RID: 10495 RVA: 0x000D1B04 File Offset: 0x000CFD04
	public Vector3 GetPointCubicHermite(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		float num = distance / this.Length * (float)this.Points.Length;
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartPoint();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndPoint();
		}
		Vector3 a = this.Points[num2];
		Vector3 a2 = this.Points[num2 + 1];
		Vector3 a3 = this.Tangents[num2] * this.StepSize;
		Vector3 a4 = this.Tangents[num2 + 1] * this.StepSize;
		float num3 = num - (float)num2;
		float num4 = num3 * num3;
		float num5 = num3 * num4;
		return (2f * num5 - 3f * num4 + 1f) * a + (num5 - 2f * num4 + num3) * a3 + (-2f * num5 + 3f * num4) * a2 + (num5 - num4) * a4;
	}
}
