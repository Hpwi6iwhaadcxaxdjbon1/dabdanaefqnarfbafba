using System;
using UnityEngine;

// Token: 0x020006F9 RID: 1785
public class BaseScriptableObject : ScriptableObject
{
	// Token: 0x0400233A RID: 9018
	[HideInInspector]
	public uint FilenameStringId;

	// Token: 0x0600274C RID: 10060 RVA: 0x0001EA00 File Offset: 0x0001CC00
	public string LookupFileName()
	{
		return StringPool.Get(this.FilenameStringId);
	}

	// Token: 0x0600274D RID: 10061 RVA: 0x0001EA0D File Offset: 0x0001CC0D
	public static bool operator ==(BaseScriptableObject a, BaseScriptableObject b)
	{
		return a == b || (a != null && b != null && a.FilenameStringId == b.FilenameStringId);
	}

	// Token: 0x0600274E RID: 10062 RVA: 0x0001EA2B File Offset: 0x0001CC2B
	public static bool operator !=(BaseScriptableObject a, BaseScriptableObject b)
	{
		return !(a == b);
	}

	// Token: 0x0600274F RID: 10063 RVA: 0x0001EA37 File Offset: 0x0001CC37
	public override int GetHashCode()
	{
		return (int)this.FilenameStringId;
	}

	// Token: 0x06002750 RID: 10064 RVA: 0x0001EA3F File Offset: 0x0001CC3F
	public override bool Equals(object o)
	{
		return o != null && o is BaseScriptableObject && o as BaseScriptableObject == this;
	}
}
