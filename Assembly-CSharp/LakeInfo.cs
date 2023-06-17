using System;
using UnityEngine;

// Token: 0x020003AE RID: 942
public class LakeInfo : MonoBehaviour
{
	// Token: 0x0600179B RID: 6043 RVA: 0x00013B8C File Offset: 0x00011D8C
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.LakeObjs.Add(this);
		}
	}
}
