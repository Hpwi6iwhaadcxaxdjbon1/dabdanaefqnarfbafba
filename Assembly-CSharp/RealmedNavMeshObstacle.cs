using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000150 RID: 336
public class RealmedNavMeshObstacle : BasePrefab
{
	// Token: 0x04000921 RID: 2337
	public NavMeshObstacle Obstacle;

	// Token: 0x06000C72 RID: 3186 RVA: 0x0005B914 File Offset: 0x00059B14
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		base.PreProcess(process, rootObj, name, serverside, clientside, false);
		if (this.isClient && this.Obstacle)
		{
			process.RemoveComponent(this.Obstacle);
			this.Obstacle = null;
		}
		process.RemoveComponent(this);
	}
}
