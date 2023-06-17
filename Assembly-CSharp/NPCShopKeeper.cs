using System;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class NPCShopKeeper : NPCPlayer
{
	// Token: 0x06000B3C RID: 2876 RVA: 0x00057CC8 File Offset: 0x00055EC8
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawCube(base.transform.position + Vector3.up * 1f, new Vector3(0.5f, 1f, 0.5f));
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x00002ECE File Offset: 0x000010CE
	public override void UpdateProtectionFromClothing()
	{
	}
}
