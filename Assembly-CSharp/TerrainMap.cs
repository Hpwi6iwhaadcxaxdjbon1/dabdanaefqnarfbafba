using System;
using UnityEngine;

// Token: 0x020004D8 RID: 1240
public abstract class TerrainMap : TerrainExtension
{
	// Token: 0x04001935 RID: 6453
	internal int res;

	// Token: 0x06001C90 RID: 7312 RVA: 0x0009C99C File Offset: 0x0009AB9C
	public void ApplyFilter(float normX, float normZ, float radius, float fade, Action<int, int, float> action)
	{
		float num = TerrainMeta.OneOverSize.x * radius;
		float num2 = TerrainMeta.OneOverSize.x * fade;
		float num3 = (float)this.res * (num - num2);
		float num4 = (float)this.res * num;
		float num5 = normX * (float)this.res;
		float num6 = normZ * (float)this.res;
		int num7 = this.Index(normX - num);
		int num8 = this.Index(normX + num);
		int num9 = this.Index(normZ - num);
		int num10 = this.Index(normZ + num);
		if (num3 != num4)
		{
			for (int i = num9; i <= num10; i++)
			{
				for (int j = num7; j <= num8; j++)
				{
					float magnitude = new Vector2((float)j + 0.5f - num5, (float)i + 0.5f - num6).magnitude;
					float num11 = Mathf.InverseLerp(num4, num3, magnitude);
					action.Invoke(j, i, num11);
				}
			}
			return;
		}
		for (int k = num9; k <= num10; k++)
		{
			for (int l = num7; l <= num8; l++)
			{
				float num12 = (float)((new Vector2((float)l + 0.5f - num5, (float)k + 0.5f - num6).magnitude < num4) ? 1 : 0);
				action.Invoke(l, k, num12);
			}
		}
	}

	// Token: 0x06001C91 RID: 7313 RVA: 0x0009CAE8 File Offset: 0x0009ACE8
	public void ForEach(Vector3 worldPos, float radius, Action<int, int> action)
	{
		int num = this.Index(TerrainMeta.NormalizeX(worldPos.x - radius));
		int num2 = this.Index(TerrainMeta.NormalizeX(worldPos.x + radius));
		int num3 = this.Index(TerrainMeta.NormalizeZ(worldPos.z - radius));
		int num4 = this.Index(TerrainMeta.NormalizeZ(worldPos.z + radius));
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				action.Invoke(j, i);
			}
		}
	}

	// Token: 0x06001C92 RID: 7314 RVA: 0x0009CB68 File Offset: 0x0009AD68
	public void ForEachParallel(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
	{
		Vector2i v3;
		v3..ctor(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v4;
		v4..ctor(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v5;
		v5..ctor(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		this.ForEachParallel(v3, v4, v5, action);
	}

	// Token: 0x06001C93 RID: 7315 RVA: 0x0009CBFC File Offset: 0x0009ADFC
	public void ForEachParallel(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
	{
		int num = Mathx.Min(v0.x, v1.x, v2.x);
		int num2 = Mathx.Max(v0.x, v1.x, v2.x);
		int num3 = Mathx.Min(v0.y, v1.y, v2.y);
		int num4 = Mathx.Max(v0.y, v1.y, v2.y);
		Vector2i base_min = new Vector2i(num, num3);
		Vector2i vector2i;
		vector2i..ctor(num2, num4);
		Vector2i base_count = vector2i - base_min + Vector2i.one;
		Parallel.Call(delegate(int thread_id, int thread_count)
		{
			Vector2i min = base_min + base_count * thread_id / thread_count;
			Vector2i max = base_min + base_count * (thread_id + 1) / thread_count - Vector2i.one;
			this.ForEachInternal(v0, v1, v2, action, min, max);
		});
	}

	// Token: 0x06001C94 RID: 7316 RVA: 0x0009CD18 File Offset: 0x0009AF18
	public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
	{
		Vector2i v3;
		v3..ctor(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v4;
		v4..ctor(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v5;
		v5..ctor(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		this.ForEach(v3, v4, v5, action);
	}

	// Token: 0x06001C95 RID: 7317 RVA: 0x0009CDAC File Offset: 0x0009AFAC
	public void ForEach(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
	{
		Vector2i min;
		min..ctor(int.MinValue, int.MinValue);
		Vector2i max;
		max..ctor(int.MaxValue, int.MaxValue);
		this.ForEachInternal(v0, v1, v2, action, min, max);
	}

	// Token: 0x06001C96 RID: 7318 RVA: 0x0009CDE8 File Offset: 0x0009AFE8
	private void ForEachInternal(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action, Vector2i min, Vector2i max)
	{
		int num = Mathf.Max(min.x, Mathx.Min(v0.x, v1.x, v2.x));
		int num2 = Mathf.Min(max.x, Mathx.Max(v0.x, v1.x, v2.x));
		int num3 = Mathf.Max(min.y, Mathx.Min(v0.y, v1.y, v2.y));
		int num4 = Mathf.Min(max.y, Mathx.Max(v0.y, v1.y, v2.y));
		int num5 = v0.y - v1.y;
		int num6 = v1.x - v0.x;
		int num7 = v1.y - v2.y;
		int num8 = v2.x - v1.x;
		int num9 = v2.y - v0.y;
		int num10 = v0.x - v2.x;
		Vector2i vector2i;
		vector2i..ctor(num, num3);
		int num11 = (v2.x - v1.x) * (vector2i.y - v1.y) - (v2.y - v1.y) * (vector2i.x - v1.x);
		int num12 = (v0.x - v2.x) * (vector2i.y - v2.y) - (v0.y - v2.y) * (vector2i.x - v2.x);
		int num13 = (v1.x - v0.x) * (vector2i.y - v0.y) - (v1.y - v0.y) * (vector2i.x - v0.x);
		vector2i.y = num3;
		while (vector2i.y <= num4)
		{
			int num14 = num11;
			int num15 = num12;
			int num16 = num13;
			vector2i.x = num;
			while (vector2i.x <= num2)
			{
				if ((num14 | num15 | num16) >= 0)
				{
					action.Invoke(vector2i.x, vector2i.y);
				}
				num14 += num7;
				num15 += num9;
				num16 += num5;
				vector2i.x++;
			}
			num11 += num8;
			num12 += num10;
			num13 += num6;
			vector2i.y++;
		}
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x0009D038 File Offset: 0x0009B238
	public void ForEachParallel(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Action<int, int> action)
	{
		Vector2i v4;
		v4..ctor(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v5;
		v5..ctor(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v6;
		v6..ctor(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		Vector2i v7;
		v7..ctor(this.Index(TerrainMeta.NormalizeX(v3.x)), this.Index(TerrainMeta.NormalizeZ(v3.z)));
		this.ForEachParallel(v4, v5, v6, v7, action);
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x0009D0F8 File Offset: 0x0009B2F8
	public void ForEachParallel(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action)
	{
		int num = Mathx.Min(v0.x, v1.x, v2.x, v3.x);
		int num2 = Mathx.Max(v0.x, v1.x, v2.x, v3.x);
		int num3 = Mathx.Min(v0.y, v1.y, v2.y, v3.y);
		int num4 = Mathx.Max(v0.y, v1.y, v2.y, v3.y);
		Vector2i base_min = new Vector2i(num, num3);
		Vector2i vector2i = new Vector2i(num2, num4) - base_min + Vector2i.one;
		Vector2i size_x = new Vector2i(vector2i.x, 0);
		Vector2i size_y = new Vector2i(0, vector2i.y);
		Parallel.Call(delegate(int thread_id, int thread_count)
		{
			Vector2i min = base_min + size_y * thread_id / thread_count;
			Vector2i max = base_min + size_y * (thread_id + 1) / thread_count + size_x - Vector2i.one;
			this.ForEachInternal(v0, v1, v2, v3, action, min, max);
		});
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x0009D264 File Offset: 0x0009B464
	public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Action<int, int> action)
	{
		Vector2i v4;
		v4..ctor(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v5;
		v5..ctor(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v6;
		v6..ctor(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		Vector2i v7;
		v7..ctor(this.Index(TerrainMeta.NormalizeX(v3.x)), this.Index(TerrainMeta.NormalizeZ(v3.z)));
		this.ForEach(v4, v5, v6, v7, action);
	}

	// Token: 0x06001C9A RID: 7322 RVA: 0x0009D324 File Offset: 0x0009B524
	public void ForEach(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action)
	{
		Vector2i min;
		min..ctor(int.MinValue, int.MinValue);
		Vector2i max;
		max..ctor(int.MaxValue, int.MaxValue);
		this.ForEachInternal(v0, v1, v2, v3, action, min, max);
	}

	// Token: 0x06001C9B RID: 7323 RVA: 0x0009D364 File Offset: 0x0009B564
	private void ForEachInternal(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action, Vector2i min, Vector2i max)
	{
		int num = Mathf.Max(min.x, Mathx.Min(v0.x, v1.x, v2.x, v3.x));
		int num2 = Mathf.Min(max.x, Mathx.Max(v0.x, v1.x, v2.x, v3.x));
		int num3 = Mathf.Max(min.y, Mathx.Min(v0.y, v1.y, v2.y, v3.y));
		int num4 = Mathf.Min(max.y, Mathx.Max(v0.y, v1.y, v2.y, v3.y));
		int num5 = v0.y - v1.y;
		int num6 = v1.x - v0.x;
		int num7 = v1.y - v2.y;
		int num8 = v2.x - v1.x;
		int num9 = v2.y - v0.y;
		int num10 = v0.x - v2.x;
		int num11 = v3.y - v2.y;
		int num12 = v2.x - v3.x;
		int num13 = v2.y - v1.y;
		int num14 = v1.x - v2.x;
		int num15 = v1.y - v3.y;
		int num16 = v3.x - v1.x;
		Vector2i vector2i;
		vector2i..ctor(num, num3);
		int num17 = (v2.x - v1.x) * (vector2i.y - v1.y) - (v2.y - v1.y) * (vector2i.x - v1.x);
		int num18 = (v0.x - v2.x) * (vector2i.y - v2.y) - (v0.y - v2.y) * (vector2i.x - v2.x);
		int num19 = (v1.x - v0.x) * (vector2i.y - v0.y) - (v1.y - v0.y) * (vector2i.x - v0.x);
		int num20 = (v1.x - v2.x) * (vector2i.y - v2.y) - (v1.y - v2.y) * (vector2i.x - v2.x);
		int num21 = (v3.x - v1.x) * (vector2i.y - v1.y) - (v3.y - v1.y) * (vector2i.x - v1.x);
		int num22 = (v2.x - v3.x) * (vector2i.y - v3.y) - (v2.y - v3.y) * (vector2i.x - v3.x);
		vector2i.y = num3;
		while (vector2i.y <= num4)
		{
			int num23 = num17;
			int num24 = num18;
			int num25 = num19;
			int num26 = num20;
			int num27 = num21;
			int num28 = num22;
			vector2i.x = num;
			while (vector2i.x <= num2)
			{
				if ((num23 | num24 | num25) >= 0 || (num26 | num27 | num28) >= 0)
				{
					action.Invoke(vector2i.x, vector2i.y);
				}
				num23 += num7;
				num24 += num9;
				num25 += num5;
				num26 += num13;
				num27 += num15;
				num28 += num11;
				vector2i.x++;
			}
			num17 += num8;
			num18 += num10;
			num19 += num6;
			num20 += num14;
			num21 += num16;
			num22 += num12;
			vector2i.y++;
		}
	}

	// Token: 0x06001C9C RID: 7324 RVA: 0x0009D724 File Offset: 0x0009B924
	public void ForEach(int x_min, int x_max, int z_min, int z_max, Action<int, int> action)
	{
		for (int i = z_min; i <= z_max; i++)
		{
			for (int j = x_min; j <= x_max; j++)
			{
				action.Invoke(j, i);
			}
		}
	}

	// Token: 0x06001C9D RID: 7325 RVA: 0x0009D754 File Offset: 0x0009B954
	public void ForEach(Action<int, int> action)
	{
		for (int i = 0; i < this.res; i++)
		{
			for (int j = 0; j < this.res; j++)
			{
				action.Invoke(j, i);
			}
		}
	}

	// Token: 0x06001C9E RID: 7326 RVA: 0x0009D78C File Offset: 0x0009B98C
	public int Index(float normalized)
	{
		int num = (int)(normalized * (float)this.res);
		if (num < 0)
		{
			return 0;
		}
		if (num <= this.res - 1)
		{
			return num;
		}
		return this.res - 1;
	}

	// Token: 0x06001C9F RID: 7327 RVA: 0x0001731A File Offset: 0x0001551A
	public float Coordinate(int index)
	{
		return ((float)index + 0.5f) / (float)this.res;
	}
}
