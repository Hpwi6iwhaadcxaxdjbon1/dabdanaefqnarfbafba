using System;
using UnityEngine;

// Token: 0x020003F0 RID: 1008
public class RiverInfo : MonoBehaviour
{
	// Token: 0x0600190F RID: 6415 RVA: 0x00014DCB File Offset: 0x00012FCB
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.RiverObjs.Add(this);
		}
	}
}
