using System;

// Token: 0x020001E0 RID: 480
public abstract class ComponentInfo<T> : ComponentInfo
{
	// Token: 0x04000C52 RID: 3154
	public T component;

	// Token: 0x06000F68 RID: 3944 RVA: 0x0000DBCD File Offset: 0x0000BDCD
	public void Initialize(T source)
	{
		this.component = source;
		this.Setup();
	}
}
