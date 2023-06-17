using System;

// Token: 0x020002C9 RID: 713
public abstract class Decay : PrefabAttribute, IServerComponent
{
	// Token: 0x06001389 RID: 5001 RVA: 0x000108E0 File Offset: 0x0000EAE0
	protected override Type GetIndexedType()
	{
		return typeof(Decay);
	}
}
