using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x02000756 RID: 1878
public class PooledList<T>
{
	// Token: 0x0400242A RID: 9258
	public List<T> data;

	// Token: 0x06002900 RID: 10496 RVA: 0x0001FE51 File Offset: 0x0001E051
	public void Alloc()
	{
		if (this.data == null)
		{
			this.data = Pool.GetList<T>();
		}
	}

	// Token: 0x06002901 RID: 10497 RVA: 0x0001FE66 File Offset: 0x0001E066
	public void Free()
	{
		if (this.data != null)
		{
			Pool.FreeList<T>(ref this.data);
		}
	}

	// Token: 0x06002902 RID: 10498 RVA: 0x0001FE7B File Offset: 0x0001E07B
	public void Clear()
	{
		if (this.data != null)
		{
			this.data.Clear();
		}
	}
}
