using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

// Token: 0x020004DB RID: 1243
public abstract class TerrainMap<T> : TerrainMap where T : struct
{
	// Token: 0x04001946 RID: 6470
	internal T[] src;

	// Token: 0x04001947 RID: 6471
	internal T[] dst;

	// Token: 0x06001CA5 RID: 7333 RVA: 0x0001732C File Offset: 0x0001552C
	public void Push()
	{
		if (this.src != this.dst)
		{
			return;
		}
		this.dst = (T[])this.src.Clone();
	}

	// Token: 0x06001CA6 RID: 7334 RVA: 0x00017353 File Offset: 0x00015553
	public void Pop()
	{
		if (this.src == this.dst)
		{
			return;
		}
		Array.Copy(this.dst, this.src, this.src.Length);
		this.dst = this.src;
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x00017389 File Offset: 0x00015589
	public IEnumerable<T> ToEnumerable()
	{
		return Enumerable.Cast<T>(this.src);
	}

	// Token: 0x06001CA8 RID: 7336 RVA: 0x00017396 File Offset: 0x00015596
	public int BytesPerElement()
	{
		return Marshal.SizeOf(typeof(T));
	}

	// Token: 0x06001CA9 RID: 7337 RVA: 0x000173A7 File Offset: 0x000155A7
	public long GetMemoryUsage()
	{
		return (long)this.BytesPerElement() * (long)this.src.Length;
	}

	// Token: 0x06001CAA RID: 7338 RVA: 0x0009D8C8 File Offset: 0x0009BAC8
	public byte[] ToByteArray()
	{
		byte[] array = new byte[this.BytesPerElement() * this.src.Length];
		Buffer.BlockCopy(this.src, 0, array, 0, array.Length);
		return array;
	}

	// Token: 0x06001CAB RID: 7339 RVA: 0x000173BA File Offset: 0x000155BA
	public void FromByteArray(byte[] dat)
	{
		Buffer.BlockCopy(dat, 0, this.dst, 0, dat.Length);
	}
}
