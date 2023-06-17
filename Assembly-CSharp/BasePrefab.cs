using System;
using UnityEngine;

// Token: 0x020006FF RID: 1791
public class BasePrefab : BaseMonoBehaviour, IPrefabPreProcess
{
	// Token: 0x0400234E RID: 9038
	[HideInInspector]
	public uint prefabID;

	// Token: 0x0400234F RID: 9039
	[HideInInspector]
	public bool isClient;

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x06002764 RID: 10084 RVA: 0x0001EBCE File Offset: 0x0001CDCE
	public bool isServer
	{
		get
		{
			return !this.isClient;
		}
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x0001EBD9 File Offset: 0x0001CDD9
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.prefabID = StringPool.Get(name);
		this.isClient = clientside;
	}
}
