using System;

// Token: 0x02000759 RID: 1881
public class SimpleList<T>
{
	// Token: 0x0400242D RID: 9261
	private const int defaultCapacity = 16;

	// Token: 0x0400242E RID: 9262
	private static readonly T[] emptyArray = new T[0];

	// Token: 0x0400242F RID: 9263
	public T[] array;

	// Token: 0x04002430 RID: 9264
	public int count;

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x0600290B RID: 10507 RVA: 0x0001FEB8 File Offset: 0x0001E0B8
	public T[] Array
	{
		get
		{
			return this.array;
		}
	}

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x0600290C RID: 10508 RVA: 0x0001FEC0 File Offset: 0x0001E0C0
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x0600290D RID: 10509 RVA: 0x0001FEC8 File Offset: 0x0001E0C8
	// (set) Token: 0x0600290E RID: 10510 RVA: 0x000D20F4 File Offset: 0x000D02F4
	public int Capacity
	{
		get
		{
			return this.array.Length;
		}
		set
		{
			if (value != this.array.Length)
			{
				if (value > 0)
				{
					T[] array = new T[value];
					if (this.count > 0)
					{
						System.Array.Copy(this.array, 0, array, 0, this.count);
					}
					this.array = array;
					return;
				}
				this.array = SimpleList<T>.emptyArray;
			}
		}
	}

	// Token: 0x170002A8 RID: 680
	public T this[int index]
	{
		get
		{
			return this.array[index];
		}
		set
		{
			this.array[index] = value;
		}
	}

	// Token: 0x06002911 RID: 10513 RVA: 0x0001FEEF File Offset: 0x0001E0EF
	public SimpleList()
	{
		this.array = SimpleList<T>.emptyArray;
	}

	// Token: 0x06002912 RID: 10514 RVA: 0x0001FF02 File Offset: 0x0001E102
	public SimpleList(int capacity)
	{
		this.array = ((capacity == 0) ? SimpleList<T>.emptyArray : new T[capacity]);
	}

	// Token: 0x06002913 RID: 10515 RVA: 0x000D2148 File Offset: 0x000D0348
	public void Add(T item)
	{
		if (this.count == this.array.Length)
		{
			this.EnsureCapacity(this.count + 1);
		}
		T[] array = this.array;
		int num = this.count;
		this.count = num + 1;
		array[num] = item;
	}

	// Token: 0x06002914 RID: 10516 RVA: 0x0001FF20 File Offset: 0x0001E120
	public void Clear()
	{
		if (this.count > 0)
		{
			System.Array.Clear(this.array, 0, this.count);
			this.count = 0;
		}
	}

	// Token: 0x06002915 RID: 10517 RVA: 0x000D2190 File Offset: 0x000D0390
	public bool Contains(T item)
	{
		for (int i = 0; i < this.count; i++)
		{
			if (this.array[i].Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002916 RID: 10518 RVA: 0x0001FF44 File Offset: 0x0001E144
	public void CopyTo(T[] array)
	{
		System.Array.Copy(this.array, 0, array, 0, this.count);
	}

	// Token: 0x06002917 RID: 10519 RVA: 0x000D21D4 File Offset: 0x000D03D4
	public void EnsureCapacity(int min)
	{
		if (this.array.Length < min)
		{
			int num = (this.array.Length == 0) ? 16 : (this.array.Length * 2);
			num = ((num < min) ? min : num);
			this.Capacity = num;
		}
	}
}
