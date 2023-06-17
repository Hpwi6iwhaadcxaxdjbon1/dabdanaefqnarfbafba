using System;

// Token: 0x020001B9 RID: 441
public class SocketMod : PrefabAttribute
{
	// Token: 0x04000BB4 RID: 2996
	[NonSerialized]
	public Socket_Base baseSocket;

	// Token: 0x04000BB5 RID: 2997
	public Translate.Phrase FailedPhrase;

	// Token: 0x06000E9C RID: 3740 RVA: 0x0000508F File Offset: 0x0000328F
	public virtual bool DoCheck(Construction.Placement place)
	{
		return false;
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x00002ECE File Offset: 0x000010CE
	public virtual void ModifyPlacement(Construction.Placement place)
	{
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x0000D3D4 File Offset: 0x0000B5D4
	protected override Type GetIndexedType()
	{
		return typeof(SocketMod);
	}
}
